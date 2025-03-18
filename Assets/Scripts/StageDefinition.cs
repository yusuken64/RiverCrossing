using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageDefinition", menuName = "ScriptableObjects/StageDefinition")]
public class StageDefinition : ScriptableObject
{
    public List<PuzzleDefinition> Puzzles;

    public Sprite LeftImage;
    public Sprite RightImage;
}
