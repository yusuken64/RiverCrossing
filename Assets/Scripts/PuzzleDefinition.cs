using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PuzzleDefinition", menuName = "ScriptableObjects/PuzzleDefinition")]
public class PuzzleDefinition : ScriptableObject
{
    public int Width;
    public int Height;
    public int BoatSize;

    public List<Actor> ActorPrefabs;
}
