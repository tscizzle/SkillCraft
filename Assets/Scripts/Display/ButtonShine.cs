using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ButtonShine : MonoBehaviour
{
    /* References. */
    public Transform shine;

    /* Parameters. */
    public float offset;
    public float duration;
    public float minDelay;
    public float maxDelay;

    /* State. */
    private Tween currentTween;

    void Start()
    {
        animate();
    }

    Tween animate()
    {
        if (currentTween)

            // Move back to beginning position.
            shine.DOLocalMoveY(-offset, 0);

        currentTween = shine
            .DOLocalMoveY(offset, duration)
            .SetEase(Ease.Linear)
            .SetDelay(Random.Range(minDelay, maxDelay))
            .OnComplete(() =>
            {
                // Start the animation again.
                animate();
            });
    }
}
