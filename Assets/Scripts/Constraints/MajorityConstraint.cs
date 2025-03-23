using System.Collections.Generic;

public class MajorityConstraint : GameConstraint
{
    public string Predator;
    public string Prey;

    public override string Description()
    {
        return $"{Predator}s will eat the {Prey} if they outnumber them";
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
