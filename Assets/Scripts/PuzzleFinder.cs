#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PuzzleFinder : MonoBehaviour
{
    public Solver Solver;
    public Generator Generator;

    [Range(1, 300f)]
    public float MinDifficulty;

    [Range(1, 100f)]
    public int MinDepth;

    [Range(1, 12)]
    public int ActorCountMin;
    [Range(1, 12)]
    public int ActorCountMax;

    public int PuzzleCountMax;

    public int MaxTry = 200;

    [ContextMenu("Find Puzzles")]
    public void FindPuzzles()
    {
        List<FoundPuzzleData> foundPuzzles = FindPuzzleWithDifficulty();
        if (Generator.GeneratePuzzleFiles)
        {
            foreach (var foundPuzzle in foundPuzzles)
            {
                Generator.CreatePuzzleData(foundPuzzle.Combination, foundPuzzle.BoatSize, foundPuzzle.Key);
            }
        }
    }

    private List<FoundPuzzleData> FindPuzzleWithDifficulty()
    {
        int tries = 0;
        Solver.warnNoSolutions = false;
        List<FoundPuzzleData> foundPuzzles = new();
        for (int actorCount = ActorCountMin; actorCount <= ActorCountMax; actorCount++)
        {
            for (int boatSize = 2; boatSize < 4; boatSize++)
            {
                var generatedPuzzles = Generator.GenerateConstrainedPuzzles(actorCount, boatSize);
                foreach(var generatedPuzzle in generatedPuzzles)
                {
                    tries++;
                    if (tries > MaxTry)
                    {
                        Debug.Log($"Exceed Max Try {MaxTry}");
                        return null;
                    }
                    if (generatedPuzzle.solution.difficulty >= MinDifficulty &&
                        generatedPuzzle.solution.path.Count >= MinDepth)
                    {
                        string key = Solver.ToKey(generatedPuzzle.combination);
                        Debug.Log($"Found puzzle {key}_b{boatSize} with difficulty {generatedPuzzle.solution.difficulty} (d = {generatedPuzzle.solution.path.Count()})");
                        foundPuzzles.Add(new FoundPuzzleData()
                        {
                            Combination = generatedPuzzle.combination,
                            BoatSize = boatSize,
                            Key = key
                        });

                        if (foundPuzzles.Count() > PuzzleCountMax)
                        {
                            return foundPuzzles;
                        }
                    }
                }
            }
        }

        if (!foundPuzzles.Any())
        {
            Debug.LogError("Failed to find a suitable puzzle within the attempt limit.");
        }
        return foundPuzzles;
    }

    public class FoundPuzzleData
    {
        public List<Actor> Combination { get; internal set; }
        public int BoatSize { get; internal set; }
        public string Key { get; internal set; }
    }
}
#endif