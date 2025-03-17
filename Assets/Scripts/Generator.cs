using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public List<Actor> PossiblePrefabs;

    [Range(1, 12)]
    public int ActorCount;

    [Range(1, 3)]
    public int BoatSize;

    public Solver Solver;
    public bool GeneratePuzzleFiles;

    public List<Actor> MustContain;

#if UNITY_EDITOR
    [ContextMenu("Generate Puzzles")]
    public void GeneratePuzzle()
    {
        IEnumerable<((List<GameState> path, double difficulty) solution, List<Actor> combination)> generatedPuzzles = GenerateConstrainedPuzzles(ActorCount, BoatSize);
        if (GeneratePuzzleFiles)
        {
            foreach (var puzzle in generatedPuzzles)
            {
                var key = Solver.ToKey(puzzle.combination.ToList());
                CreatePuzzleData(puzzle.combination.ToList(), BoatSize, key);
            }
        }
    }

    public IEnumerable<((List<GameState> path, double difficulty) solution, List<Actor> combination)> GenerateConstrainedPuzzles(int actorCount, int boatSize)
    {
        Solver.warnNoSolutions = false;
        var combinations = GenerateCombinations(PossiblePrefabs, actorCount);

        var requiredCounts = MustContain
            .GroupBy(x => x.ActorName)
            .ToDictionary(g => g.Key, g => g.Count());

        var solvedPuzzles = combinations
            .Where(combination => requiredCounts.All(req => combination.Count(x => x.name == req.Key) >= req.Value))
            .Select(combination => (
                solution: Solver.Solve(combination, boatSize),
                combination: combination
            ))
            .Where(solution => solution.solution.path != null);
     
        Debug.Log($"Done Generating {solvedPuzzles.Count()} solvable puzzles");
        return solvedPuzzles;
    }

    public void CreatePuzzleData(List<Actor> combination, int boatSize, string key)
    {
        var (width, height) = GetDimensions(combination.Count());
        PuzzleDefinition asset = ScriptableObject.CreateInstance<PuzzleDefinition>();
        asset.BoatSize = boatSize;
        asset.Width = width;
        asset.Height = height;
        asset.ActorPrefabs = new();
        asset.ActorPrefabs.AddRange(combination);

        // Generate a consistent folder path based on prefab names
        string directoryPath = $"Assets/Prefabs/Generated/{key}_Actors{combination.Count()}_Boat{boatSize}";
        
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs/Generated"))
        {
            AssetDatabase.CreateFolder("Assets/Prefabs", "Generated");
        }

        // Ensure directory exists
        if (!AssetDatabase.IsValidFolder(directoryPath))
        {
            AssetDatabase.CreateFolder("Assets/Prefabs/Generated", $"{key}_Actors{combination.Count()}_Boat{boatSize}");
            AssetDatabase.Refresh();
        }

        // Define the asset path
        string baseAssetPath = $"{directoryPath}/{key}_b{boatSize}.asset";

        // Refresh AssetDatabase to ensure it detects existing assets
        AssetDatabase.Refresh();

        // Check if the asset already exists
        var puzzleDefinition = AssetDatabase.LoadAssetAtPath<ScriptableObject>(baseAssetPath);
        if (puzzleDefinition != null)
        {
            //Debug.Log($"Asset already exists at {baseAssetPath}. Skipping creation.");
            return;
        }

        // Generate a unique path (if needed)
        string assetPath = AssetDatabase.GenerateUniqueAssetPath(baseAssetPath);

        // Create and save the asset
        AssetDatabase.CreateAsset(asset, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;

        Debug.Log($"Created new asset at: {assetPath}");
    }
#endif
    public static (int width, int height) GetDimensions(int number)
    {
        if (number < 1 || number > 12)
            throw new ArgumentOutOfRangeException(nameof(number), "Number must be between 1 and 12.");

        // Define the possible width and height pairs
        (int width, int height)[] sizes = new (int, int)[]
        {
            (1, 1), // 1
            (1, 2), // 2
            (1, 3), // 3
            (1, 4), // 4
            (2, 2), // 5
            (2, 3), // 6
            (2, 3), // 7
            (2, 4), // 8
            (3, 3), // 9
            (3, 4), // 10
            (3, 4), // 11
            (3, 4)  // 12
        };

        // Return the width and height for the given number
        return sizes[number - 1];
    }

    public static List<List<Actor>> GenerateCombinations(List<Actor> items, int length, bool noDuplicates = true)
    {
        List<List<Actor>> result = new List<List<Actor>>();
        GenerateCombinationsRecursive(items, new List<Actor>(), length, result, 0);

        if (!noDuplicates)
        {
            return result;
        }

        var distinctByKey = result
            .Select(x => new GameState(x.ToArray(), new Actor[0], true))
            .GroupBy(p => p.cachedKey)
            .Select(g => g.First())
            .Select(x => x.LeftSide.ToList())
            .ToList();

        return distinctByKey.ToList();
    }
    public static IEnumerable<T> DistinctBy<T, TKey>(IEnumerable<T> source, Func<T, TKey> keySelector)
    {
        var seenKeys = new HashSet<TKey>();
        foreach (var item in source)
        {
            if (seenKeys.Add(keySelector(item)))  // Adds the key and checks if it's already seen
            {
                yield return item;
            }
        }
    }

    static void GenerateCombinationsRecursive(
        List<Actor> items,
        List<Actor> current,
        int length,
        List<List<Actor>> result,
        int startIndex)
    {
        if (current.Count == length)
        {
            result.Add(new List<Actor>(current));
            return;
        }

        for (int i = startIndex; i < items.Count; i++)
        {
            current.Add(items[i]);
            GenerateCombinationsRecursive(items, current, length, result, i);
            current.RemoveAt(current.Count - 1);
        }
    }
}
