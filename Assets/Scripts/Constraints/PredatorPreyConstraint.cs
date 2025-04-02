using System.Collections.Generic;
using System.Linq;

public class PredatorPreyConstraint : GameConstraint
{
    public string Predator;
    public string Prey;
    public string Guard;

    public override string Description()
    {
        Game game = FindObjectOfType<Game>();
        var predatorActor = game.Actors.First(x => x.ActorName == Predator);
        var preyActor = game.Actors.First(x => x.ActorName == Prey);
        var guardActor = game.Actors.First(x => x.ActorName == Guard);
        return $"{Predator} {predatorActor.ActorAsText()} will eat the {Prey} {preyActor.ActorAsText()} Unless {Guard} {guardActor.ActorAsText()} is present";
    }

    public override bool IsGameOverFunc(
        ActorData owner,
        IEnumerable<ActorData> leftSideActors,
        IEnumerable<ActorData> rightSideActors,
        IEnumerable<ActorData> boatActors,
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
