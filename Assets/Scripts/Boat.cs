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
    public bool Moving;

    private void Start()
    {
        GoLeft();
    }

    public void GoRight()
    {
        SetAnimalsBusy(true);

        //this.transform.position = RightPosition.position;
        this.transform.DOMove(RightPosition.position, 0.2f)
            .OnComplete(() => {
                SetAnimalsBusy(false);
                Moving = false;
            });
        IsBoatRight = true;
    }

    public void GoLeft()
    {
        SetAnimalsBusy(true);

        //this.transform.position = LeftPosition.position;
        this.transform.DOMove(LeftPosition.position, 0.2f)
            .OnComplete(() => {
                SetAnimalsBusy(false);
                Moving = false;
            });
        IsBoatRight = false;
    }

    private void SetAnimalsBusy(bool busy)
    {
        var boatActors = Cells.Where(x => x.CurrentActor != null)
            .Select(x => x.CurrentActor);

        foreach (var actor in boatActors)
        {
            actor.GetComponent<Draggable>().IsBusy = busy;
        }
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
        if (Moving) { return; }
        Moving = true;

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
