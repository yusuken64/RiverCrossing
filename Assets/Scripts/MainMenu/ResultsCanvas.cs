using System.Linq;
using TMPro;
using UnityEngine;

public class ResultsCanvas : MonoBehaviour
{
    public TextMeshProUGUI HeaderText;
    public TextMeshProUGUI MessageText;

    public GameObject NextButton;

    public void Setup(string header, string message)
    {
        HeaderText.text = header;
        MessageText.text = message;

        Game game = FindObjectOfType<Game>(true);
        MainMenu mainMenu = FindObjectOfType<MainMenu>(true);

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
        FindObjectOfType<Game>(true).Retry();
    }

    public void Home_Clicked()
    {
        FindObjectOfType<MainMenu>(true).gameObject.SetActive(true);
        FindObjectOfType<Game>(true).GameCanvas.gameObject.SetActive(false);
        FindObjectOfType<Game>(true).ClearGame();
    }

    public void Next_Clicked()
    {
        Game game = FindObjectOfType<Game>(true);
        game.GameCanvas.gameObject.SetActive(false);

        FindObjectOfType<MainMenu>(true).NextPuzzle(game.PuzzleDefinition);
    }
}
