using System.Collections.Generic;
using System.Linq;

public class MinorityConstraint : GameConstraint
{
    public string Animal;

    public override string Description()
    {
        return $"{Animal}s cannot outnumber non {Animal}";
    }

    public override bool IsGameOver(Actor owner,
        IEnumerable<Actor> leftSideActors,
        IEnumerable<Actor> rightSideActors,
        out string message)
    {
        var containingSide = GetContainingSide(leftSideActors, rightSideActors, owner);

        int animalCount = CountItem(containingSide, Animal);
        int otherCount = containingSide.Count(actor => !actor.KeyWords.Contains(Animal));

        if (animalCount > 0 &&
            animalCount > otherCount)
        {
            message = $"{Animal}s outnumber the others";
            return true;
        }

        message = "";
        return false;
    }
}