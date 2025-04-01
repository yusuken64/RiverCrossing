using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public string ActorName;
    public Sprite InfoSprite;

    public Cell CurrentCell;
    public bool CanPilotBoat;
    public bool IsHeavy;

    void Start()
    {
        var draggable = GetComponent<Draggable>();
        draggable.OnHold = () =>
        {
            Game game = FindObjectOfType<Game>();
            game.InfoPanel.Setup(this);
            game.InfoPanel.gameObject.SetActive(true);

            PlayPickupSound();
        };
        draggable.OnReleased = (destinationCell) =>
        {
            DropOnCell(destinationCell);
        };
        draggable.OnClcked = () =>
        {
            Game game = FindObjectOfType<Game>();

            bool onLeftShore = game.LeftShore.Cells.Any(x => x.CurrentActor == this);
            bool onRightShore = game.RightShore.Cells.Any(x => x.CurrentActor == this);
            bool onBoat = game.Boat.Cells.Any(x => x.CurrentActor == this);
            bool boatIsOnRight = game.Boat.IsBoatRight;

            Cell destinationCell = null;

            if (onLeftShore && !boatIsOnRight)
            {
                destinationCell = game.Boat.GetFirstEmptyCell();
            }
            else if (onRightShore && boatIsOnRight)
            {
                destinationCell = game.Boat.GetFirstEmptyCell();
            }
            else if (onBoat && boatIsOnRight)
            {
                destinationCell = game.RightShore.GetFirstEmptyCell();
            }
            else if (onBoat && !boatIsOnRight)
            {
                destinationCell = game.LeftShore.GetFirstEmptyCell();
            }

            if (destinationCell != null)
            {
                DropOnCell(destinationCell);
            }
        };
    }

    private void DropOnCell(Cell destinationCell)
    {
        if (destinationCell != null)
        {
            if (CanDrop(CurrentCell, destinationCell))
            {
                CurrentCell?.SetActor(null);
                destinationCell?.SetActor(this);
            }
            else if (CanSwap(CurrentCell, destinationCell))
            {
                var originalActor = destinationCell?.CurrentActor;
                CurrentCell?.SetActor(originalActor);
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
        game.UpdateCrossButton();

        PlayDropSound();
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

        return SameSide(currentCell, destinationCell);
    }

    private static bool SameSide(Cell currentCell, Cell destinationCell)
    {
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

    private bool CanSwap(Cell currentCell, Cell destinationCell)
    {
        if (currentCell == destinationCell) { return false; }

        return SameSide(currentCell, destinationCell);
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