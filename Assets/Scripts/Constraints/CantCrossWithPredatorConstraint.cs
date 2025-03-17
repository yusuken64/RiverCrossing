using System.Collections.Generic;
using System.Linq;

public class CantCrossWithPredatorConstraint : GameConstraint
{
    public override string Description()
    {
        var owner = GetComponent<Actor>();
        return $"{owner.ActorName} can't cross with predators";
    }

    public override bool IsGameOver(
        Actor owner,
        IEnumerable<Actor> leftSideActors,
        IEnumerable<Actor> rightSideActors,
        IEnumerable<Actor> boatActors,
        out string message)
    {
        var predatorCount = boatActors.Count(x => x.IsPredator());
        if (predatorCount >= 1)
        {
            message = $"{owner.ActorName} can't cross with predators";
            return true;
        }

        message = "";
        return false;
    }
}