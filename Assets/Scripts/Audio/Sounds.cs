using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sounds : MonoBehaviour
{
    public static Sounds Instance;

    public AudioClip Music;

    public AudioClip PickUpSmall;
    public AudioClip PickUpLarge;

    public AudioClip DropSmall;
    public AudioClip DropLarge;

    public AudioClip DropBoatSmall;
    public AudioClip DropBoatLarge;

    public AudioClip BoatCross;

    public AudioClip Success;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }
    }
}
