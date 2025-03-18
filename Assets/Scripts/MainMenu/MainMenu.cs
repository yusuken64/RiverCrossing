using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

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

    public PuzzleDefinition DebugPuzzle;

    private void Start()
    {
        Setup();
    }

    public void Setup()
    {
        CurrentPage = 0;
        Game game = FindObjectOfType<Game>();
        game.GameCanvas.SetActive(false);
        game.ClearAllActors();

        SetupPuzzles();
    }

    private void SetupPuzzles()
    {
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
            newButton.Setup(i + 1, puzzleDefinition);
            newButton.LevelSelected = (puzzle) => LevelClicked(puzzle);
        }
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
                (List<GameState> path, double difficulty) solution = solver.Solve(puzzle.ActorPrefabs, puzzle.BoatSize);

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
        this.gameObject.SetActive(false);
        Game game = FindObjectOfType<Game>();

        game.PuzzleDefinition = puzzle;
        game.SetupGame();
    }

    public void NextPuzzle(PuzzleDefinition currentPuzzle)
    {
        //var index = Puzzles.IndexOf(currentPuzzle);
        //var nextPuzzle = Puzzles[index + 1];

        //LevelClicked(nextPuzzle);
    }


    public void ExitClicked()
    {
        Application.Quit();
    }

    public void CreditsClicked()
    {

    }

    //StageSelect
    public void RightClicked()
    {
        CurrentPage++;
        Mathf.Clamp(CurrentPage, 0, Stages.Count() - 1);
        SetupPuzzles();
    }

    public void LeftClicked()
    {
        CurrentPage--;
        Mathf.Clamp(CurrentPage, 0, Stages.Count() - 1);
        SetupPuzzles();
    }
}
