using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    public Image AnimalImage;
    public List<Sprite> Portraits;

    public void Setup()
    {
        AnimalImage.sprite = Portraits[UnityEngine.Random.Range(0, Portraits.Count())];
    }
}
