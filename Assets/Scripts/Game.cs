using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Game : MonoBehaviour
{
    public ResultsCanvas ResultsCanvas;
    public GameObject GameCanvas;
    public PuzzleDefinition PuzzleDefinition;

    public Container LeftShore;
    public Container RightShore;
    public Boat Boat;

    public InfoPanel InfoPanel;

    public CrossButton CrossButton;

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

        InfoPanel.LevelText.text = PuzzleDefinition.PuzzleName;
        for (int i = 0; i < PuzzleDefinition.ActorPrefabs.Count; i++)
        {
            Actor prefab = PuzzleDefinition.ActorPrefabs[i];
            Actor newActor = Instantiate(prefab);
            newActor.Initialize();
            LeftShore.Cells[i].SetActor(newActor);
            Actors.Add(newActor);
        }
        GameCanvas.gameObject.SetActive(true);
        UpdateCrossButton();
        AudioManager.Instance?.PlayMusic(Sounds.Instance?.Music);
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
        InfoPanel.gameObject.SetActive(false);
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
            DisableInput();
            HandleLoss(message);
        }
        else if (CheckWin())
        {
            DisableInput();
            HandleWin();
        }
    }

    internal void UpdateCrossButton()
    {
        if (CanBoatMove())
        {
            CrossButton.SetToClickable();
        }
        else
        {
            CrossButton.SetToUnClickable();
        }
    }

    private void DisableInput()
    {
        Actors.ForEach(x => x.GetComponent<Draggable>().IsDraggable = false);
    }

    internal void HandleLoss(string message)
    {
        AudioManager.Instance?.StopMusic();
        Debug.Log("Game Over!");
        GameCanvas.gameObject.SetActive(false);
        ResultsCanvas.Setup("You Lose", message);
        ResultsCanvas.gameObject.SetActive(true);
    }

    internal void HandleWin()
    {
        AudioManager.Instance?.StopMusic();
        AudioManager.Instance?.PlaySound(Sounds.Instance?.Success);

        SingletonSaveData.Instance.SaveData.GameData.ClearedStageIds.Add(PuzzleDefinition.PuzzleNum);
        SingletonSaveData.Instance.Save();
        Debug.Log("Game Win!");
        GameCanvas.gameObject.SetActive(false);
        ResultsCanvas.Setup("You Win", "Everyone made it safely across the River");
        ResultsCanvas.gameObject.SetActive(true);
    }

    public bool IsGameOver(out string message)
    {
        foreach (var actor in Actors)
        {
            (List<Actor> leftSideActors, List<Actor> rightSideActors, List<Actor> boatActors) = Game.GetActors(this);
            if (actor.IsGameOver(leftSideActors, rightSideActors, boatActors, out message))
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

    public static (List<Actor> leftSideActors, List<Actor> rightSideActors, List<Actor> boatActors) GetActors(Game game)
    {
        var leftSideActors = game.LeftShore.Cells.Where(x => x.CurrentActor != null)
            .Select(x => x.CurrentActor)
            .ToList();
        var rightSideActors = game.RightShore.Cells.Where(x => x.CurrentActor != null)
            .Select(x => x.CurrentActor)
            .ToList();
        var boatActors = game.Boat.Cells
                .Where(x => x.CurrentActor != null)
                .Select(x => x.CurrentActor).ToList();

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

        return (leftSideActors, rightSideActors, boatActors);
    }

    public void Pause_Clicked()
    {
        var active = ResultsCanvas.gameObject.activeSelf;
        ResultsCanvas.gameObject.SetActive(!active);
        ResultsCanvas.Setup("Paused", "Get all animals to the right side");
    }
}
