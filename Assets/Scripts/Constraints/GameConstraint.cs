using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class GameConstraint : MonoBehaviour
{
    public static IEnumerable<Actor> GetContainingSide(IEnumerable<Actor> leftSideActors, IEnumerable<Actor> rightSideActors, Actor owner)
    {
        var isLeft = leftSideActors.Contains(owner);

        return isLeft ? leftSideActors : rightSideActors;
    }

    public abstract string Description();
    public abstract bool IsGameOver(Actor owner,
        IEnumerable<Actor> leftSideActors,
        IEnumerable<Actor> rightSideActors,
        out string message);

    internal int CountItem(IEnumerable<Actor> actors, string item)
    {
        return actors.Count(actor => actor.KeyWords.Contains(item));
    }

    internal bool HasItem(IEnumerable<Actor> actors, string item)
    {
        return actors.Any(actor => actor.KeyWords.Contains(item));
    }
}
