using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PuzzleFinder : MonoBehaviour
{
    public Solver Solver;
    public Generator Generator;

    [Range(1, 30000f)]
    public float MinDifficulty;

    [Range(1, 100f)]
    public int MinDepth;

    [Range(1, 12)]
    public int ActorCountMin;
    [Range(1, 12)]
    public int ActorCountMax;

    public int PuzzleCountMax;

    public int MaxTry = 200;
    public bool RandomSearch;

    internal PuzzleDefinition FindFirstPuzzleWithCount(int index, int actorCount, int minDepth, float minDifficulty)
    {
        var puzzleData = Generator.GenerateConstrainedPuzzleFast(actorCount, 2, minDepth, minDifficulty, MaxTry);
        if (puzzleData.solution.path == null) { return null; }

        (int width, int height) = Generator.GetDimensions(actorCount);

        List<Actor> actors = puzzleData.combination.Select(data => Generator.PossiblePrefabs.First(x => x.ActorName == data.ActorName)).ToList();
        PuzzleDefinition asset = ScriptableObject.CreateInstance<PuzzleDefinition>();
        asset.PuzzleShortName = $"R{actorCount}";
        asset.PuzzleNum = 100 + index;
        asset.PuzzleName = $"Infinite {actorCount}";
        asset.BoatSize = 2;
        asset.Width = width;
        asset.Height = height;
        asset.ActorPrefabs = new();
        asset.ActorPrefabs.AddRange(actors);

        return asset;
    }

#if UNITY_EDITOR
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

    [ContextMenu("Find Puzzles Fast")]
    public void FindPuzzlesFast()
    {
        var puzzleData = Generator.GenerateConstrainedPuzzleFast(ActorCountMin, 2, MinDepth, MinDifficulty, MaxTry);

        List<Actor> actors = puzzleData.combination.Select(data => Generator.PossiblePrefabs.First(x => x.ActorName == data.ActorName)).ToList();
        string key = Solver.ToKey(actors);
        Debug.Log($"{key}, d = {puzzleData.solution.path.Count}, difficulty = {puzzleData.solution.difficulty}");
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
                if (RandomSearch)
                {
                    generatedPuzzles = generatedPuzzles.OrderBy(x => Guid.NewGuid());
                }

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
                        List<Actor> combination = generatedPuzzle.combination.Select(data => Generator.PossiblePrefabs.First(x => x.ActorName == data.ActorName)).ToList();
                        string key = Solver.ToKey(combination);
                        Debug.Log($"Found puzzle {key}_b{boatSize} with difficulty {generatedPuzzle.solution.difficulty} (d = {generatedPuzzle.solution.path.Count()})");
                        foundPuzzles.Add(new FoundPuzzleData()
                        {
                            Combination = combination,
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

    [ContextMenu("Solve Generated Puzzles")]
    public void OrderGeneratedsPuzzles()
    {
        string directoryPath = $"Assets/Prefabs/Generated/";

        var puzzleDefinitions = GetAllPuzzleDefinitionsInFolder(directoryPath);

        Debug.Log($"found {puzzleDefinitions.Count()} puzzles");

        foreach(var puzzleDefinition in puzzleDefinitions)
        {
            (List<GameState> path, double difficulty) solution = Solver.Solve(puzzleDefinition.ActorPrefabs.Select(Actor.ToActorData).ToList(),
                puzzleDefinition.BoatSize);

            if (solution.path != null)
            {
                puzzleDefinition.Difficulty = (float)solution.difficulty;
                puzzleDefinition.SolveDepth = solution.path.Count();
            }
        }

        AssetDatabase.Refresh();
    }

    [ContextMenu("Order Generated Puzzles")]
    public void OrderGeneratePuzzles()
    {
        string directoryPath = $"Assets/Prefabs/Generated/";
        var puzzleDefinitions = GetAllPuzzleDefinitionsInFolder(directoryPath);
        var orderedPuzzles = puzzleDefinitions.OrderBy(x => x.Difficulty);

        var requiredCounts = Generator.MustContain
            .GroupBy(x => x.ActorName)
            .ToDictionary(g => g.Key, g => g.Count());

        var candidatePuzzles = orderedPuzzles
            .Where(combination => requiredCounts.All(req => combination.ActorPrefabs
            .Count(x => x.ActorName == req.Key) >= req.Value));

        foreach (var puzzle in candidatePuzzles
            .OrderByDescending(x => x.SolveDepth)
            .Take(20))
        {
            Debug.Log($"{puzzle.PuzzleName} difficulty{puzzle.Difficulty} d{puzzle.SolveDepth}", puzzle);
        }
    }

    public static PuzzleDefinition[] GetAllPuzzleDefinitionsInFolder(string folderPath)
    {
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            Debug.LogError($"Invalid folder path: {folderPath}");
            return new PuzzleDefinition[0];
        }

        string[] guids = AssetDatabase.FindAssets("t:PuzzleDefinition", new[] { folderPath });
        return guids
            .Select(guid => AssetDatabase.LoadAssetAtPath<PuzzleDefinition>(AssetDatabase.GUIDToAssetPath(guid)))
            .Where(asset => asset != null)
            .ToArray();
    }
#endif

    public class FoundPuzzleData
    {
        public List<Actor> Combination { get; internal set; }
        public int BoatSize { get; internal set; }
        public string Key { get; internal set; }
    }
}