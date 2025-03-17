using System.Collections.Generic;
using System.Linq;

public class PredatorGuardedConstraint : GameConstraint
{
    public string Predator;
    public string Guard;

    public override string Description()
    {
        return $"{Predator} will eat everyone unless {Guard} is present";
    }

    public override bool IsGameOver(
        Actor owner,
        IEnumerable<Actor> leftSideActors,
        IEnumerable<Actor> rightSideActors,
        IEnumerable<Actor> boatActors,
        out string message)
    {
        var containingSide = GetContainingSide(leftSideActors, rightSideActors, owner);

        bool hasPredator = HasItem(containingSide, Predator);
        bool hasGuard = HasItem(containingSide, Guard);
        bool notAlone = containingSide.Any(x => x != owner);

        if (hasPredator && !hasGuard && notAlone)
        {
            message = $"{Predator} attacked the others";
            return true;
        }

        message = "";
        return false;
    }
}
