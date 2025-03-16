using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour
{
    public Transform RightPosition;
    public Transform LeftPosition;

    public bool IsBoatRight; //false means left;

    public List<Cell> Cells => GetComponent<Container>().Cells;

    private void Start()
    {
        GoLeft();
    }

    public void GoRight()
    {
        this.transform.position = RightPosition.position;
        IsBoatRight = true;
    }

    public void GoLeft()
    {
        this.transform.position = LeftPosition.position;
        IsBoatRight = false;
    }

    public void GoRight_Clicked()
    {
        Game game = FindObjectOfType<Game>();
        if (game.CanBoatMove())
        {
            GoRight();
            game.CheckConstraints();
            game.InfoObject.gameObject.SetActive(false);
        }
    }

    public void GoLeft_Clicked()
    {
        Game game = FindObjectOfType<Game>();
        if (game.CanBoatMove())
        {
            GoLeft();
            game.CheckConstraints();
            game.InfoObject.gameObject.SetActive(false);
        }
    }

    public void CrossRiver_Clicked()
    {
        if (IsBoatRight)
        {
            GoLeft_Clicked();
        }
        else
        {
            GoRight_Clicked();
        }
    }
}
