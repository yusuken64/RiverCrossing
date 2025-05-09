using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "InfiniteStageDefinition", menuName = "ScriptableObjects/InfiniteStageDefinition")]
public class InfiniteStageDefinition : StageDefinitionBase
{
    private List<PuzzleDefinition> puzzles = new();

    public void ResetPuzzles()
    {
        var puzzleFinder = FindAnyObjectByType<PuzzleFinder>();
        List<(int actorCount, int minDepth, float difficulty)> counts = new List<(int, int, float)>()
        {
            (Random.Range(4, 6), 10, 50f),
            (Random.Range(8, 10), 15, 200f),
            //(Random.Range(10, 12), 22, 300f)
        };

        puzzles = new();
        for (int i = 0; i < counts.Count; i++)
        {
            int count = counts[i].actorCount;
            int minDepth = counts[i].minDepth;
            float difficulty = counts[i].difficulty;
            PuzzleDefinition item = puzzleFinder.FindFirstPuzzleWithCount(i, count, minDepth, difficulty);
            if (item != null)
            {
                puzzles.Add(item);
            }
        }
    }

    public override List<PuzzleDefinition> GetPuzzles()
    {
        //if (puzzles == null)
        //{
        //    ResetPuzzles();
        //}

        return puzzles.Where(x => x != null).ToList();
    }
}
