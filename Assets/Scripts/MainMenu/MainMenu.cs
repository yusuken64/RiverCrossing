using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenu : MonoBehaviour
{
    public LevelButton ButtonPrefab;
    public Transform ButtonContainer;

    public List<StageDefinition> Stages;

    public TextMeshProUGUI StageText;
    public int CurrentPage;
    public GameObject StageRightButton;
    public GameObject StageLeftButton;
    public Image LeftImage;
    public Image RightImage;

    public GameObject AudioSettings;

    public PuzzleDefinition DebugPuzzle;

    private void Start()
    {
        Setup();
    }

    public void Setup()
    {
        AudioSettings.gameObject.SetActive(false);
        CurrentPage = 0;
        Game game = FindObjectOfType<Game>();
        game.GameCanvas.SetActive(false);
        game.ClearAllActors();

        SetupPuzzles();
    }

    public void SetupPuzzles()
    {
        AudioManager.Instance?.PlayMusic(Sounds.Instance?.Music);

        foreach (Transform child in ButtonContainer)
        {
            Destroy(child.gameObject);
        }

        var currentStage = Stages[CurrentPage];
        StageText.text = currentStage.name;
        var puzzles = currentStage.Puzzles;
        for (int i = 0; i < puzzles.Count; i++)
        {
            PuzzleDefinition puzzleDefinition = puzzles[i];
            var newButton = Instantiate(ButtonPrefab, ButtonContainer);
            newButton.Setup(puzzleDefinition);
            newButton.LevelSelected = (puzzle) => LevelClicked(puzzle);
        }

        LeftImage.gameObject.SetActive(currentStage.LeftImage != null);
        RightImage.gameObject.SetActive(currentStage.RightImage != null);
        LeftImage.sprite = currentStage.LeftImage;
        RightImage.sprite = currentStage.RightImage;
        StageLeftButton.gameObject.SetActive(CurrentPage > 0);
        StageRightButton.gameObject.SetActive(CurrentPage < Stages.Count() - 1);
    }

    [ContextMenu("Debug_Puzzle")]
    public void Debug_Puzzle()
    {
        LevelClicked(DebugPuzzle);
    }

#if UNITY_EDITOR
    [ContextMenu("Solve Main Puzzles")]
    public void SolveMainPuzzles()
    {
        int puzzleNum = 0;
        for (int i = 0; i < Stages.Count; i++)
        {
            StageDefinition stage = Stages[i];
            stage.name = $"Stage{i + 1}";
            EditorUtility.SetDirty(stage);
            string stageAssetPath = AssetDatabase.GetAssetPath(stage);
            AssetDatabase.RenameAsset(stageAssetPath, stage.name);

            foreach (var puzzle in stage.Puzzles)
            {
                var solver = FindObjectOfType<Solver>();
                (List<GameState> path, double difficulty) solution =
                    solver.Solve(puzzle.ActorPrefabs.Select(Solver.ToActorData).ToList(), puzzle.BoatSize);

                if (solution.path == null)
                {
                    Debug.LogError($"{puzzle.name} was not solvable", this);
                }
                else
                {
                    Debug.Log($"{name} took {solution.path.Count()} steps" +
                        $" {solution.difficulty} difficulty");
                }

                (int width, int height) dimensions = Generator.GetDimensions(puzzle.ActorPrefabs.Count);
                puzzle.Width = dimensions.width;
                puzzle.Height = dimensions.height;
                puzzle.SolveDepth = (float)solution.path.Count();
                puzzle.Difficulty = (float)solution.difficulty;
            }

            IOrderedEnumerable<PuzzleDefinition> orderedPuzzles = stage.Puzzles
                            .OrderBy(x => x.ActorPrefabs.Count())
                            .ThenBy(x => x.SolveDepth)
                            .ThenBy(x => x.Difficulty);
            stage.Puzzles = orderedPuzzles.ToList();

            foreach (var puzzle in stage.Puzzles)
            {
                puzzleNum++;
                var key = $"Level{puzzleNum}_{Solver.ToKey(puzzle.ActorPrefabs)}";
                puzzle.name = key;
                puzzle.PuzzleName = $"Puzzle {puzzleNum}";
                puzzle.PuzzleNum = puzzleNum;
                EditorUtility.SetDirty(puzzle);
                string assetPath = AssetDatabase.GetAssetPath(puzzle);
                AssetDatabase.RenameAsset(assetPath, key);
            }
        }
        AssetDatabase.Refresh();
    }
#endif

    private void LevelClicked(PuzzleDefinition puzzle)
    {
        var screenTransition = FindObjectOfType<ScreenTransition>();
        screenTransition.DoTransition(() =>
        {
            this.gameObject.SetActive(false);
            Game game = FindObjectOfType<Game>();

            game.PuzzleDefinition = puzzle;
            game.SetupGame();
        });
    }

    public void NextPuzzle(PuzzleDefinition currentPuzzle)
    {
        var index = currentPuzzle.PuzzleNum;
        var nextPuzzle = Stages.SelectMany(x => x.Puzzles)
            .FirstOrDefault(x => x.PuzzleNum == index + 1);

        LevelClicked(nextPuzzle);
    }
    public void ResetDataClicked()
    {
        SingletonSaveData.Instance?.ResetData();
        Setup();
    }

    public void ExitClicked()
    {
        Application.Quit();
    }

    public void CreditsClicked()
    {

    }

    public void VolumeButton_Clicked()
    {
        AudioSettings.SetActive(!AudioSettings.activeSelf);
    }

    //StageSelect
    public void RightClicked()
    {
        CurrentPage++;
        CurrentPage = Mathf.Clamp(CurrentPage, 0, Stages.Count() - 1);
        SetupPuzzles();
    }

    public void LeftClicked()
    {
        CurrentPage--;
        CurrentPage = Mathf.Clamp(CurrentPage, 0, Stages.Count() - 1);
        SetupPuzzles();
    }
}
