using System.Collections.Generic;
using System.Linq;

public class MustCrossAloneConstraint : GameConstraint
{
    public override string Description()
    {
        var owner = GetComponent<Actor>();
        return $"{owner.ActorName} {owner.ActorAsText()} must cross the river Alone";
    }

    public override bool IsGameOverFunc(ActorData owner, IEnumerable<ActorData> leftSideActors, IEnumerable<ActorData> rightSideActors, IEnumerable<ActorData> boatActors, out string message)
    {
        if (boatActors.Contains(owner) && boatActors.Count() > 1)
        {
            message = $"{owner.ActorName} must cross the river Alone";
            return true;
        }

        message = "";
        return false;
    }
}
