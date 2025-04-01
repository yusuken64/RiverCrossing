using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
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
        //this.transform.position = RightPosition.position;
        this.transform.DOMove(RightPosition.position, 0.2f);
        IsBoatRight = true;
    }

    public void GoLeft()
    {
        //this.transform.position = LeftPosition.position;
        this.transform.DOMove(LeftPosition.position, 0.2f);
        IsBoatRight = false;
    }

    public void GoRight_Clicked()
    {
        Game game = FindObjectOfType<Game>();
        if (game.CanBoatMove())
        {
            AudioManager.Instance?.PlaySound(Sounds.Instance.BoatCross);
            GoRight();
            game.CheckConstraints();
            game.InfoPanel.gameObject.SetActive(false);
        }
    }

    public void GoLeft_Clicked()
    {
        Game game = FindObjectOfType<Game>();
        if (game.CanBoatMove())
        {
            AudioManager.Instance?.PlaySound(Sounds.Instance.BoatCross);
            GoLeft();
            game.CheckConstraints();
            game.InfoPanel.gameObject.SetActive(false);
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

    internal Cell GetFirstEmptyCell()
    {
        return Cells.FirstOrDefault(x => x.CurrentActor == null);
    }
}
