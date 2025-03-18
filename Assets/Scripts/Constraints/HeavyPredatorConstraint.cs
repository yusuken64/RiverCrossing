using System.Collections.Generic;
using System.Linq;

public class HeavyPredatorConstraint : GameConstraint
{
    public override string Description()
    {
        var owner = GetComponent<Actor>();
        return $"{owner.ActorName} must outnumber heavy predators";
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

        var heavyPredatorCount = containingSide.Count(x => x.IsPredator() && x.IsHeavy);
        if (heavyPredatorCount >= familyCount)
        {
            message = $"{owner.ActorName} must outnumber heavy predators";
            return true;
        }

        message = "";
        return false;
    }
}
