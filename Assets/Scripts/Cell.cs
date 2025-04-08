using DG.Tweening;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public Actor CurrentActor;

    internal void SetActor(Actor actor, bool jump = false)
    {
        CurrentActor = actor;
        if (actor != null)
        {
            actor.CurrentCell = this;
            actor.transform.SetParent(this.transform);

            Vector3 targetLocalPosition = new Vector3(0, 0, -0.1f);
            if (jump)
            {
                var jumpTween = actor.transform.DOLocalJump(new Vector3(0, 0, -0.1f), 3f, 1, 0.5f, snapping: false)
                    .SetEase(Ease.OutQuad);

                float flipDirection = Random.value > 0.5f ? 360f : -360f;
                var flipTween = actor.transform.DOLocalRotate(
                    new Vector3(0, 0, flipDirection), 0.5f, RotateMode.FastBeyond360
                ).SetEase(Ease.OutCubic);

                DOTween.Sequence()
                    .Join(jumpTween)
                    .Join(flipTween)
                    .OnComplete(() =>
                    {
                        actor.transform.localPosition = new Vector3(0, 0, -0.1f);
                        actor.transform.localRotation = Quaternion.identity;
                    });
            }
            else
            {
                actor.transform.DOLocalMove(targetLocalPosition, 0.2f);
            }
        }
    }
}
