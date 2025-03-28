using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public string ActorName;

    public Cell CurrentCell;
    public bool CanPilotBoat;
    public bool IsHeavy;

    void Start()
    {
        var draggable = GetComponent<Draggable>();
        draggable.OnHold = () =>
        {
            List<string> infos = new List<string>();
            if (CanPilotBoat)
            {
                infos.Add($"{ActorName} can move the Boat");
            }

            var gameConstraints = GetComponents<GameConstraint>();
            foreach (var gameConstraint in gameConstraints)
            {
                infos.Add(gameConstraint.Description());
            }

            Game game = FindObjectOfType<Game>();
            string infoTextString = string.Join(Environment.NewLine, infos);
            if (!string.IsNullOrEmpty(infoTextString))
            {
                game.InfoText.text = infoTextString;
                game.InfoObject.gameObject.SetActive(true);
            }
            else
            {
                game.InfoObject.gameObject.SetActive(false);
            }

            PlayPickupSound();
        };
        draggable.OnReleased = (destinationCell) =>
        {
            if (destinationCell != null)
            {
                if (CanDrop(CurrentCell, destinationCell))
                {
                    CurrentCell?.SetActor(null);
                    destinationCell?.SetActor(this);
                }
                else
                {
                    CurrentCell?.SetActor(this);
                }
            }
            else
            {
                CurrentCell?.SetActor(this);
            }

            Game game = FindObjectOfType<Game>();
            if (game.CheckWin())
            {
                game.HandleWin();
            }

            PlayDropSound();
        };
    }

    internal ActorData _actorData;

    internal void Initialize()
    {
        _actorData = ToActorData(this);
    }

    private void PlayPickupSound()
    {
        if (IsHeavy)
        {
            AudioManager.Instance?.PlaySound(Sounds.Instance?.PickUpLarge);
        }
        else if (IsPredator())
        {
            AudioManager.Instance?.PlaySound(Sounds.Instance?.PickUpLarge);
        }
        else
        {
            AudioManager.Instance?.PlaySound(Sounds.Instance?.PickUpSmall);
        }
    }

    private void PlayDropSound()
    {
        if (IsHeavy)
        {
            AudioManager.Instance?.PlaySound(Sounds.Instance?.DropLarge);
        }
        else if (IsPredator())
        {
            AudioManager.Instance?.PlaySound(Sounds.Instance?.DropLarge);
        }
        else
        {
            AudioManager.Instance?.PlaySound(Sounds.Instance?.DropSmall);
        }
    }

    internal bool IsPredator()
    {
        var gameConstraints = GetComponents<GameConstraint>();
        return gameConstraints.Any(constraint =>
        {
            switch (constraint)
            {
                case PredatorPreyConstraint predatorPreyConstraint:
                    return predatorPreyConstraint.Predator == ActorName;
                case PredatorGuardedConstraint predatorGuardedConstraint:
                    return predatorGuardedConstraint.Predator == ActorName;
                default:
                    return false;
            }
        });
    }

    private bool CanDrop(Cell currentCell, Cell destinationCell)
    {
        if (destinationCell.CurrentActor != null) { return false; }

        var game = FindObjectOfType<Game>();

        // Get all possible groups of cells
        var leftShoreCells = game.LeftShore.Cells;
        var rightShoreCells = game.RightShore.Cells;
        var boatCells = game.Boat.Cells;

        // Check if both cells belong to the same group
        if (leftShoreCells.Contains(currentCell) && leftShoreCells.Contains(destinationCell))
            return true;

        if (rightShoreCells.Contains(currentCell) && rightShoreCells.Contains(destinationCell))
            return true;

        if (boatCells.Contains(currentCell) && boatCells.Contains(destinationCell))
            return true;

        // Check if the boat is considered part of the left or right shore
        if (game.Boat.IsBoatRight)
        {
            if (rightShoreCells.Contains(currentCell) && boatCells.Contains(destinationCell))
                return true;
            if (boatCells.Contains(currentCell) && rightShoreCells.Contains(destinationCell))
                return true;
        }
        else // Boat is on the left shore
        {
            if (leftShoreCells.Contains(currentCell) && boatCells.Contains(destinationCell))
                return true;
            if (boatCells.Contains(currentCell) && leftShoreCells.Contains(destinationCell))
                return true;
        }

        return false;
    }

    internal bool IsGameOver(IEnumerable<Actor> leftSideActors, IEnumerable<Actor> rightSideActors, IEnumerable<Actor> boatActors, out string message)
    {
        var gameConstraints = GetComponents<GameConstraint>();

        if (gameConstraints == null ||
            gameConstraints.Length == 0)
        {
            message = null;
            return false;
        }

        foreach (var constraint in gameConstraints)
        {
            if (constraint.IsGameOver(
                this,
                leftSideActors,
                rightSideActors,
                boatActors,
                out message))
            {
                return true;
            }
        }

        message = null;
        return false;
    }

    internal bool CanMoveBoat()
    {
        return CanPilotBoat;
    }

    public static ActorData ToActorData(Actor actor)
    {
        return new(
            actor.ActorName,
            actor.CanPilotBoat,
            actor.IsHeavy,
            actor.IsPredator(),
            GetConstraintDatas(actor));
    }

    private static Func<ActorData, ActorData[], ActorData[], ActorData[], bool>[] GetConstraintDatas(Actor actor)
    {
        var gameConstraints = actor.GetComponents<GameConstraint>();

        return gameConstraints
            .Select(x => new Func<ActorData, ActorData[], ActorData[], ActorData[], bool>((actorData, left, right, boat) =>
            {
                bool result = x.IsGameOverFunc(actorData, left, right, boat, out _);
                return result;
            }))
            .ToArray();
    }
}

public record ActorData
{
    public readonly string ActorName;
    public readonly bool CanPilotBoat;
    public readonly bool IsHeavy;
    public readonly bool IsPredator;
    public readonly Func<ActorData, ActorData[], ActorData[], ActorData[], bool>[] GameOverConstraints;

    public ActorData(
        string actorName,
        bool canPilotBoat,
        bool isHeavy,
        bool isPredator,
        Func<ActorData, ActorData[], ActorData[], ActorData[], bool>[] constraints)
    {
        ActorName = actorName;
        CanPilotBoat = canPilotBoat;
        IsHeavy = isHeavy;
        IsPredator = isPredator;
        GameOverConstraints = constraints;
    }

    internal bool IsGameOver(
        ActorData[] left,
        ActorData[] right,
        ActorData[] boat)
    {
        if (GameOverConstraints == null || GameOverConstraints.Length == 0)
        {
            return false;
        }

        return GameOverConstraints.All(constraint => constraint.Invoke(this, left, right, boat));
    }
}