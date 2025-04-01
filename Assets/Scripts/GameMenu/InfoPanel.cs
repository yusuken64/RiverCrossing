using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour
{
    public TextMeshProUGUI InfoText;
    public Image InfoImage;
    public TextMeshProUGUI LevelText;
    public TextMeshProUGUI NameText;

    internal void Setup(Actor actor)
    {
        List<string> infos = new List<string>();
        if (actor.CanPilotBoat)
        {
            infos.Add($"{actor.ActorName} can move the Boat");
        }

        var gameConstraints = actor.GetComponents<GameConstraint>();
        foreach (var gameConstraint in gameConstraints)
        {
            infos.Add(gameConstraint.Description());
        }

        var game = FindObjectOfType<Game>();
        var otherGameConstraints = game.Actors
            .Where(x => x.ActorName != actor.ActorName)
            .GroupBy(x => x.ActorName)
            .SelectMany(x => x.First().GetComponents<GameConstraint>());
        foreach(var gameConstraint in otherGameConstraints)
        {
            if (gameConstraint is PredatorPreyConstraint predatorPreyConstraint)
            {
                if (predatorPreyConstraint.Prey == actor.ActorName)
                {
                    infos.Add(predatorPreyConstraint.Description());
                }
            }
        }

        string infoTextString = string.Join(Environment.NewLine, infos);

        InfoText.text = infoTextString;
        InfoImage.sprite = actor.InfoSprite;
        NameText.text = actor.ActorName;
    }
}
