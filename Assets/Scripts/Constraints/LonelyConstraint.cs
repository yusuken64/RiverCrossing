using System.Collections.Generic;
using System.Linq;

public class LonelyConstraint : GameConstraint
{
    public override string Description()
    {
        var owner = GetComponent<Actor>();
        return $"{owner.ActorName} cannot be left alone";
    }

    public override bool IsGameOver(
        Actor owner,
        IEnumerable<Actor> leftSideActors,
        IEnumerable<Actor> rightSideActors,
        IEnumerable<Actor> boatActors,
        out string message)
    {
        var containingSide = GetContainingSide(leftSideActors, rightSideActors, owner);
        var alone = containingSide.Count() == 1;

        if (alone)
        {
            message = $"{owner.ActorName} cannot be left alone";
            return true;
        }
    
        message = "";
        return false;
    }
}