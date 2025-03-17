using System.Collections.Generic;
using System.Linq;

public class CantCrossWithHeavyConstraint : GameConstraint
{
    public override string Description()
    {
        var owner = GetComponent<Actor>();
        return $"{owner.ActorName} can't cross the with other heavy";
    }

    public override bool IsGameOver(
        Actor owner,
        IEnumerable<Actor> leftSideActors,
        IEnumerable<Actor> rightSideActors,
        IEnumerable<Actor> boatActors,
        out string message)
    {
        var heavyCount = boatActors.Count(x => x.IsHeavy);
        if (heavyCount >= 2)
        {
            message = $"{owner.ActorName} can't cross with another heavy";
            return true;
        }

        message = "";
        return false;
    }
}
