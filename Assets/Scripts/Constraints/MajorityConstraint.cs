using System.Collections.Generic;
using System.Linq;

public class MajorityConstraint : GameConstraint
{
    public string Predator;
    public string Prey;

    public override string Description()
    {
        Game game = FindObjectOfType<Game>();
        var predatorActor = game.Actors.FirstOrDefault(x => x.ActorName == Predator);
        var preyActor = game.Actors.FirstOrDefault(x => x.ActorName == Prey);

        if(predatorActor == null || preyActor == null) { return null; }

        return $"{predatorActor.ActorName} {predatorActor.ActorAsText()} will eat the {preyActor.ActorName} {preyActor.ActorAsText()} if they outnumber them";
    }

    public override bool IsGameOverFunc(ActorData owner, IEnumerable<ActorData> leftSideActors, IEnumerable<ActorData> rightSideActors, IEnumerable<ActorData> boatActors, out string message)
    {
        var containingSide = GetContainingSide(leftSideActors, rightSideActors, owner);

        int predatorCount = CountItem(containingSide, Predator);
        int preyCount = CountItem(containingSide, Prey);

        if (predatorCount > 0 &&
            preyCount > 0 &&
            predatorCount > preyCount)
        {
            message = $"{Predator}s outnumber the {Prey}";
            return true;
        }

        message = "";
        return false;
    }
}
