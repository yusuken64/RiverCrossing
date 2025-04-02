using System.Collections.Generic;
using System.Linq;

public class PredatorGuardedConstraint : GameConstraint
{
    public string Predator;
    public string Guard;

    public override string Description()
    {
        Game game = FindObjectOfType<Game>();
        var predatorActor = game.Actors.First(x => x.ActorName == Predator);
        var guardActor = game.Actors.First(x => x.ActorName == Guard);
        return $"{Predator} {predatorActor.ActorAsText()} will eat everyone unless {Guard} {guardActor.ActorAsText()} is present";
    }

    public override bool IsGameOverFunc(ActorData owner, IEnumerable<ActorData> leftSideActors, IEnumerable<ActorData> rightSideActors, IEnumerable<ActorData> boatActors, out string message)
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
