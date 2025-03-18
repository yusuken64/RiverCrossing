using System.Collections.Generic;
using System.Linq;

public class PredatorConstraint : GameConstraint
{
    public override string Description()
    {
        var owner = GetComponent<Actor>();
        return $"{owner.ActorName} must outnumber predators";
    }

    public override bool IsGameOver(
        Actor owner,
        IEnumerable<Actor> leftSideActors,
        IEnumerable<Actor> rightSideActors,
        IEnumerable<Actor> boatActors,
        out string message)
    {
        var containingSide = GetContainingSide(leftSideActors, rightSideActors, owner);
        var familyCount = GetFamilyCount(containingSide, owner);

        var predatorCount = containingSide.Count(x => x.IsPredator());
        if (predatorCount >= familyCount)
        {
            message = $"{owner.ActorName} must outnumber predators";
            return true;
        }

        message = "";
        return false;
    }

    private int GetFamilyCount(IEnumerable<Actor> containingSide, Actor owner)
    {
        return containingSide.Count(x => x.ActorName == owner.ActorName);
    }
}