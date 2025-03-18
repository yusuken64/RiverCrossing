using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "PuzzleDefinition", menuName = "ScriptableObjects/PuzzleDefinition")]
public class PuzzleDefinition : ScriptableObject
{
    public string PuzzleName;
    public int PuzzleNum;

    public int Width;
    public int Height;
    public int BoatSize;

    public List<Actor> ActorPrefabs;
    public float Difficulty;
    public float SolveDepth;
}
