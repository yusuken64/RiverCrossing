using System.Linq;
using TMPro;
using UnityEngine;

public class ResultsCanvas : MonoBehaviour
{
    public TextMeshProUGUI HeaderText;
    public TextMeshProUGUI MessageText;

    public GameObject NextButton;

    public Game game;
    public MainMenu mainMenu;

    public void Setup(string header, string message)
    {
        HeaderText.text = header;
        MessageText.text = message;

        bool showNext = game.PuzzleDefinition != mainMenu.Stages.SelectMany(x => x.Puzzles).Last();
        NextButton.gameObject.SetActive(showNext);
    }

    [ContextMenu("Test Results")]
    public void TestResults()
    {
        this.gameObject.SetActive(true);
        this.Setup("test", "test message");
    }

    public void Retry_Clicked()
    {
        var screenTransition = FindObjectOfType<ScreenTransition>();
        screenTransition.DoTransition(() =>
        {
            game.Retry();
        });
    }

    public void Home_Clicked()
    {
        AudioManager.Instance?.StopMusic();
        var screenTransition = FindObjectOfType<ScreenTransition>();
        screenTransition.DoTransition(() =>
        {
            mainMenu.gameObject.SetActive(true);
            mainMenu.SetupPuzzles();
            game.GameCanvas.gameObject.SetActive(false);
            game.ClearGame();
        });
    }

    public void Next_Clicked()
    {
        game.GameCanvas.gameObject.SetActive(false);
        mainMenu.NextPuzzle(game.PuzzleDefinition);
    }
}
