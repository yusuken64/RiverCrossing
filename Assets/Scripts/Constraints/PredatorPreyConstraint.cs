using System.Collections.Generic;

public class PredatorPreyConstraint : GameConstraint
{
    public string Predator;
    public string Prey;
    public string Guard;

    public override string Description()
    {
        return $"{Predator} will eat the {Prey} Unless {Guard} is present";
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
        bool hasPrey = HasItem(containingSide, Prey);
        bool hasGuard = HasItem(containingSide, Guard);

        if (hasPredator && hasPrey && !hasGuard)
        {
            message = $"{Predator} ate the {Prey}";
            return true;
        }

        message = "";
        return false;
    }
}
