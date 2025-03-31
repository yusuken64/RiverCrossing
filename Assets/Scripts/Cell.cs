using DG.Tweening;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public Actor CurrentActor;

    internal void SetActor(Actor actor)
    {
        CurrentActor = actor;
        if (actor != null)
        {
            actor.CurrentCell = this;
            actor.transform.SetParent(this.transform);
            //actor.transform.localPosition = new Vector3(0, 0, -0.1f);
            actor.transform.DOLocalMove(new Vector3(0, 0, -0.1f), 0.2f);
        }
    }
}
