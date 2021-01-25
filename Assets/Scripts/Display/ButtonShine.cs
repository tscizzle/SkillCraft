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
    public float repeatDelay;

    /* State. */
    private Tween currentTween = null;

    /* PUBLIC API */

    public void animate(float delay = 0)
    /* Start (or restart if it's already going) the shiny animation.
    
    The first loop begins after `delay` seconds, but the loop repeats are spaced out
    based on the `repeatDelay` variable.
    */
    {
        // If there already is one going, end it.
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill();
        }

        // Move back to beginning position.
        shine.DOLocalMoveY(-offset, 0);

        // Run the animation.
        currentTween = shine
            .DOLocalMoveY(offset, duration)
            .SetEase(Ease.Linear)
            .SetDelay(delay)
            .OnComplete(() =>
            {
                // Start the animation again.
                animate(delay: repeatDelay);
            });
    }
}
