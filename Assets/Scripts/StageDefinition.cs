using System.Collections.Generic;
using UnityEngine;

public abstract class StageDefinitionBase : ScriptableObject
{
    public abstract List<PuzzleDefinition> GetPuzzles();
}

[CreateAssetMenu(fileName = "StageDefinition", menuName = "ScriptableObjects/StageDefinition")]
public class StageDefinition : StageDefinitionBase
{
    public List<PuzzleDefinition> Puzzles;

    public Sprite LeftImage;
    public Sprite RightImage;

    public override List<PuzzleDefinition> GetPuzzles()
    {
        return Puzzles;
    }
}
