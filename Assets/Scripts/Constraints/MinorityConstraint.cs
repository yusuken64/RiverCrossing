using System.Collections.Generic;
using System.Linq;

public class MinorityConstraint : GameConstraint
{
    public string Animal;

    public override string Description()
    {
        return $"{Animal} cannot be majority";
    }

    public override bool IsGameOverFunc(
        ActorData owner,
        IEnumerable<ActorData> leftSideActors,
        IEnumerable<ActorData> rightSideActors,
        IEnumerable<ActorData> boatActors,
        out string message)
    {
        var containingSide = GetContainingSide(leftSideActors, rightSideActors, owner);

        int animalCount = CountItem(containingSide, Animal);
        int otherCount = containingSide.Count(actor => actor.ActorName != Animal);

        if (animalCount > 0 &&
            animalCount > otherCount)
        {
            message = $"{Animal} outnumber the others";
            return true;
        }

        message = "";
        return false;
    }
}