using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class NiceShine : MonoBehaviour
{
    /* References. */
    public Transform shine;

    /* Parameters. */
    public float offset;
    public float duration;
    public float repeatDelay;

    /* State. */
    private Tween currentTween = null;

    void OnDestroy()
    {
        // Kill the ongoing animation so it doesn't error once this object is destroyed.
        stop();
    }

    /* PUBLIC API */

    public void animate(float delay = 0)
    /* Start (or restart if it's already going) the shiny animation.
    
    :param float delay: The first loop begins after `delay` seconds, but the loop
        repeats are spaced out based on the `repeatDelay` variable.
    */
    {
        // If there already is one going, end it so we'll start a new one.
        stop();

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

    public void stop()
    /* Kill the current animation, stop it repeating, and reset the shine out of view.
    Essentially removes the shine visually (like for a faded/disabled button).
    */
    {
        // Stop the animation and stop it repeating.
        if (currentTween != null)
        {
            currentTween.Kill();
        }
        // Reset the position of the shine to not be visible.
        shine.DOLocalMoveY(-offset, 0);
    }
}
