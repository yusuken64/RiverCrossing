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

    private void Start()
    {
        ShutterScreen.gameObject.SetActive(false);
        BlockScreen.gameObject.SetActive(false);
    }

    public void DoTransition(Action postTransition)
    {
        StartCoroutine(DoTransitionRoutine(postTransition));
    }

    private IEnumerator DoTransitionRoutine(Action postTransition)
    {
        BlockScreen.gameObject.SetActive(true);
        ShutterScreen.gameObject.SetActive(true);
        ShutterScreen.transform.position = StartPositionObject.transform.position;

        var closeTween = ShutterScreen.transform.DOMove(EndPositionObject.transform.position, TransitionTimeSeconds);
        yield return closeTween.WaitForCompletion();

        postTransition?.Invoke();
        yield return new WaitForSeconds(OpenDelayTimeSeconds);

        var openTween = ShutterScreen.transform.DOMove(StartPositionObject.transform.position, TransitionTimeSeconds);
        yield return openTween.WaitForCompletion();

        ShutterScreen.gameObject.SetActive(false);
        BlockScreen.gameObject.SetActive(false);
    }

    [ContextMenu("Test Transition")]
    public void TestTransition()
    {
        DoTransition(null);
    }
}