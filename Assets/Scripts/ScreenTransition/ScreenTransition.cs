using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenTransition : MonoBehaviour
{
    public GameObject BlockScreen;
    public GameObject ShutterScreen;

    public GameObject StartPositionObject;
    public GameObject EndPositionObject;

    public float TransitionTimeSeconds;
    public float OpenDelayTimeSeconds;

    public AudioClip CloseClip;
    public AudioClip OpenClip;

    private void Start()
    {
        ShutterScreen.gameObject.SetActive(false);
        BlockScreen.gameObject.SetActive(false);
    }

    public void DoTransition(Action postTransition, bool showAd)
    {
        StartCoroutine(DoTransitionRoutine(postTransition, showAd));
    }

    private IEnumerator DoTransitionRoutine(Action postTransition, bool showAd)
    {
        BlockScreen.gameObject.SetActive(true);
        ShutterScreen.gameObject.SetActive(true);
        ShutterScreen.transform.position = StartPositionObject.transform.position;

        AudioManager.Instance?.PlaySound(CloseClip);
        var closeTween = ShutterScreen.transform.DOMove(EndPositionObject.transform.position, TransitionTimeSeconds);
        yield return closeTween.WaitForCompletion();

        if (showAd)
        {
            var levelPlaySample = FindObjectOfType<LevelPlaySample>();
            if (levelPlaySample != null)
            {
                levelPlaySample.ShowInterstitialAd();
                while (levelPlaySample.showingAd)
                {
                    yield return null;
                }
            }
        }

        postTransition?.Invoke();
        yield return new WaitForSeconds(OpenDelayTimeSeconds);

        AudioManager.Instance?.PlaySound(OpenClip);
        var openTween = ShutterScreen.transform.DOMove(StartPositionObject.transform.position, TransitionTimeSeconds);
        yield return openTween.WaitForCompletion();

        ShutterScreen.gameObject.SetActive(false);
        BlockScreen.gameObject.SetActive(false);
    }

    [ContextMenu("Test Transition")]
    public void TestTransition()
    {
        DoTransition(null, false);
    }
}