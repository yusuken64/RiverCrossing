using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public string ActorName;

    public Cell CurrentCell;
    public bool CanPilotBoat;

    public List<string> KeyWords;

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
            game.CheckConstraints();
        };
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

    internal bool IsGameOver(IEnumerable<Actor> leftSideActors, IEnumerable<Actor> rightSideActors, out string message)
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
}
