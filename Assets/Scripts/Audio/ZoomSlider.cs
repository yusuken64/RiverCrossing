using UnityEngine;
using UnityEngine.UI;

public class ZoomSlider : MonoBehaviour
{
    public Slider Slider;

    void Start()
    {
        var cam = FindFirstObjectByType<Camera>();
        Slider.value = cam.orthographicSize;
    }
}
