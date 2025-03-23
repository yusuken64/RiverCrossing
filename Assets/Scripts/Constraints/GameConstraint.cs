using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class GameConstraint : MonoBehaviour
{
    public static IEnumerable<T> GetContainingSide<T>(
        IEnumerable<T> leftSideActors,
        IEnumerable<T> rightSideActors,
        T owner)
    {
        var isLeft = leftSideActors.Contains(owner);

        return isLeft ? leftSideActors : rightSideActors;
    }

    public abstract string Description();

    public bool IsGameOver(
        Actor owner,
        IEnumerable<Actor> leftSideActors,
        IEnumerable<Actor> rightSideActors,
        IEnumerable<Actor> boatActors,
        out string message)
    {
        return IsGameOverFunc(ToLightWeightActorData(owner),
            leftSideActors.Select(ToLightWeightActorData),
            rightSideActors.Select(ToLightWeightActorData),
            boatActors.Select(ToLightWeightActorData),
            out message);
    }


    public abstract bool IsGameOverFunc(ActorData owner,
        IEnumerable<ActorData> leftSideActors,
        IEnumerable<ActorData> rightSideActors,
        IEnumerable<ActorData> boatActors,
     out string message);

    internal static int CountItem(IEnumerable<ActorData> actors, string item)
    {
        return actors.Count(actor => actor.ActorName == item);
    }

    internal static bool HasItem(IEnumerable<ActorData> actors, string item)
    {
        if (string.IsNullOrWhiteSpace(item)) { return false; }
        return actors.Any(actor => actor.ActorName == item);
    }

    internal static int GetFamilyCount(IEnumerable<ActorData> containingSide, ActorData owner)
    {
        return containingSide.Count(x => x.ActorName == owner.ActorName);
    }

    internal static ActorData ToLightWeightActorData(Actor actor)
    {
        return new(
            actor.ActorName,
            actor.CanPilotBoat,
            actor.IsHeavy,
            actor.IsPredator(),
            null);
    }
}
