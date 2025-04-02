using System.Collections.Generic;
using System.Linq;

public class PredatorConstraint : GameConstraint
{
    public override string Description()
    {
        var owner = GetComponent<Actor>();
        return $"{owner.ActorName} {owner.ActorAsText()} must outnumber predators";
    }

    public override bool IsGameOverFunc(ActorData owner, IEnumerable<ActorData> leftSideActors, IEnumerable<ActorData> rightSideActors, IEnumerable<ActorData> boatActors, out string message)
    {
        var containingSide = GetContainingSide(leftSideActors, rightSideActors, owner);
        var familyCount = GetFamilyCount(containingSide, owner);

        var predatorCount = containingSide.Count(x => x.IsPredator);
        if (predatorCount >= familyCount)
        {
            message = $"{owner.ActorName} must outnumber predators";
            return true;
        }

        message = "";
        return false;
    }
}