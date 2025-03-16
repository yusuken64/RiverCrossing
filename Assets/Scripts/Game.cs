using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Game : MonoBehaviour
{
    public ResultsCanvas ResultsCanvas;
    public GameObject GameCanvas;
    public PuzzleDefinition PuzzleDefinition;

    public Container LeftShore;
    public Container RightShore;
    public Boat Boat;

    public TextMeshProUGUI InfoText;
    public GameObject InfoObject;
    public TextMeshProUGUI LevelText;

    public List<Actor> Actors;

    private void Start()
    {
        ResultsCanvas.gameObject.SetActive(false);
        GameCanvas.gameObject.SetActive(false);
    }

    internal void Retry()
    {
        SetupGame();
    }

    [ContextMenu("SetupGame")]
    public void SetupGame()
    {
        ClearGame();
        Container boatContainer = Boat.GetComponent<Container>();
        boatContainer.Width = 1;
        boatContainer.Height = PuzzleDefinition.BoatSize;
        boatContainer.SetupCells();

        LeftShore.SetupCells();
        RightShore.SetupCells();

        LevelText.text = PuzzleDefinition.name;
        for (int i = 0; i < PuzzleDefinition.ActorPrefabs.Count; i++)
        {
            Actor prefab = PuzzleDefinition.ActorPrefabs[i];
            Actor newActor = Instantiate(prefab);
            LeftShore.Cells[i].SetActor(newActor);
            Actors.Add(newActor);
        }
        GameCanvas.gameObject.SetActive(true);
    }

    public void ClearGame()
    {
        GameCanvas.gameObject.SetActive(false);
        ResultsCanvas.gameObject.SetActive(false);
        ClearAllActors();

        LeftShore.Width = PuzzleDefinition.Width;
        LeftShore.Height = PuzzleDefinition.Height;

        RightShore.Width = PuzzleDefinition.Width;
        RightShore.Height = PuzzleDefinition.Height;

        LeftShore.Cells.Clear();
        RightShore.Cells.Clear();

        Container boatContainer = Boat.GetComponent<Container>();
        boatContainer.ClearChildren();
        Boat.GoLeft();

        Actors.Clear();
        InfoObject.gameObject.SetActive(false);
    }

    internal bool CheckWin()
    {
        var actorCellMap = RightShore.Cells
            .Where(cell => cell.CurrentActor != null)
            .ToDictionary(cell => cell.CurrentActor, cell => cell.CurrentActor.CurrentCell);

        return Actors.All(actor => actorCellMap.ContainsKey(actor) && actorCellMap[actor] == actor.CurrentCell);
    }

    [ContextMenu("ClearAllActors")]
    public void ClearAllActors()
    {
        {
#if UNITY_EDITOR
            var actors = FindObjectsOfType<Actor>(true);
            foreach (var actor in actors)
            {
                DestroyImmediate(actor.gameObject);
            }
#else
            var actors = FindObjectsOfType<Actor>(true);
            foreach (var actor in actors)
            {
                Destroy(actor.gameObject);
            }
#endif
        }

        LeftShore.ClearChildren();
        RightShore.ClearChildren();
    }

    internal void CheckConstraints()
    {
        if (IsGameOver(out string message))
        {
            Debug.Log("Game Over!");
            GameCanvas.gameObject.SetActive(false);
            ResultsCanvas.Setup("You Lose", message);
            ResultsCanvas.gameObject.SetActive(true);
        }
        else if (CheckWin())
        {
            Debug.Log("Game Win!");
            GameCanvas.gameObject.SetActive(false);
            ResultsCanvas.Setup("You Win", "Everyone made it safely across the River");
            ResultsCanvas.gameObject.SetActive(true);
        }
    }

    public bool IsGameOver(out string message)
    {
        foreach (var actor in Actors)
        {
            (List<Actor> leftSideActors, List<Actor> rightSideActors) = Game.GetActors(this);
            if (actor.IsGameOver(leftSideActors, rightSideActors, out message))
            {
                Debug.Log(message);
                return true;
            }
        }

        message = null;
        return false;
    }

    internal bool CanBoatMove()
    {
        return Actors.Any(actor => Boat.Cells.Any(cell => cell.CurrentActor == actor) 
            && actor.CanMoveBoat());
    }

    public static (List<Actor> leftSideActors, List<Actor> rightSideActors) GetActors(Game game)
    {
        var leftSideActors = game.LeftShore.Cells.Where(x => x.CurrentActor != null)
            .Select(x => x.CurrentActor)
            .ToList();
        var rightSideActors = game.RightShore.Cells.Where(x => x.CurrentActor != null)
            .Select(x => x.CurrentActor)
            .ToList();
        if (game.Boat.IsBoatRight)
        {
            rightSideActors.AddRange(game.Boat.Cells
                .Where(x => x.CurrentActor != null)
                .Select(x => x.CurrentActor));
        }
        else
        {
            leftSideActors.AddRange(game.Boat.Cells
                .Where(x => x.CurrentActor != null)
                .Select(x => x.CurrentActor));
        }

        return (leftSideActors, rightSideActors);
    }

    public void Pause_Clicked()
    {
        var active = ResultsCanvas.gameObject.activeSelf;
        ResultsCanvas.gameObject.SetActive(!active);
        ResultsCanvas.Setup("Paused", "Get all animals to the right side");
    }
}
