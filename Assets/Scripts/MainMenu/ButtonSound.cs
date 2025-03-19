using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSound : MonoBehaviour
{
    public AudioClip ClickSound;

    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(PlayClickSound);
    }

    void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(PlayClickSound);
        }
    }

    void PlayClickSound()
    {
        AudioManager.Instance?.PlaySound(ClickSound);
    }
}