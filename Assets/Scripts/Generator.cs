using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public List<Actor> PossiblePrefabs;

    public int ActorCount;
    public int BoatSize;

    public Solver Solver;
    public bool GeneratePuzzleFiles;

    public List<Actor> MustContain;

#if UNITY_EDITOR
    [ContextMenu("Generate Puzzles")]
    public void GeneratePuzzle()
    {
        var combinations = GenerateCombinations(PossiblePrefabs, ActorCount);

        foreach(var combination in combinations)
        {
            var path = Solver.Solve(combination, BoatSize);
            if (path != null)
            {
                Debug.LogWarning($"{path[0].ToKey()} {path.Count()} steps");
                if (GeneratePuzzleFiles)
                {
                    if (MustContain.All(x => combination.Contains(x)))
                    {
                        CreatePuzzleData(combination, BoatSize, $"{path[0].ToKey()}_{path.Count()}steps");
                    }
                }
            }
        }
    }

    public static void CreatePuzzleData(List<Actor> combination, int boatSize, string key)
    {
        PuzzleDefinition asset = ScriptableObject.CreateInstance<PuzzleDefinition>();
        asset.BoatSize = boatSize;
        asset.Width = 2;
        asset.Height = 4;
        asset.ActorPrefabs = new();
        asset.ActorPrefabs.AddRange(combination);

        string path = $"Assets/Prefabs/Generated/Actors{combination.Count()}_Boat{boatSize}";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string assetPath = AssetDatabase.GenerateUniqueAssetPath($"{path}/{key}.asset");

        AssetDatabase.CreateAsset(asset, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
#endif
    static List<List<Actor>> GenerateCombinations(List<Actor> items, int length)
    {
        List<List<Actor>> result = new List<List<Actor>>();
        GenerateCombinationsRecursive(items, new List<Actor>(), length, result, 0);
        return result;
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
