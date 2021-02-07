using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TurnDisplay : MonoBehaviour
{
    /* Parameters. */
    public float duration;
    public float amplitude;
    public float period;

    /* References. */
    private FightState fightState;
    private RectTransform rect;
    private Text turnText;
    private Tween currentTween;

    void Awake()
    {
        fightState = GameObject.Find("GeneralScripts").GetComponent<FightState>();
        rect = GetComponent<RectTransform>();
        turnText = transform.Find("TurnText").GetComponent<Text>();

        reset();
    }

    void Start()
    {
        animate();
        fightState.addTurnListener(animate);
    }

    /* Helpers. */

    private void animate()
    /* Update the turn indicator and slide it into view.
    
    https://easings.net/ has info on the Ease values.
    */
    {
        // Start it in the correct state (off screen and transparent).
        reset();

        /* Set the text to be for the current fighter. */
        string currentTurn = fightState.currentFighter.isPlayer ? "Your" : "Enemy's";
        turnText.text = $"{currentTurn} Turn";

        /* Move it into view.

        The move-in wiggles, with OutElastic.
        */
        currentTween = rect
            .DOAnchorPosX(20, duration)
            .SetEase(Ease.OutElastic, amplitude, period);
    }

    private void reset()
    /* Start off screen and transparent, so animating it gradually brings it into view.
    Also, if there is a tween currently occurring, kill it.
    */
    {
        if (currentTween != null) currentTween.Kill();

        // Start it just left of the screen.
        Vector3 startingPos = rect.anchoredPosition;
        startingPos.x = -400;
        rect.anchoredPosition = startingPos;
    }
}
