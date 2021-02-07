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
    public Color playerBackgroundColor;
    public Color playerTextColor;
    public Color enemyBackgroundColor;
    public Color enemyTextColor;

    /* References. */
    private FightState fightState;
    private RectTransform rect;
    private Image backgroundImage;
    private Text turnText;
    private Tween currentTween;

    void Awake()
    {
        fightState = GameObject.Find("GeneralScripts").GetComponent<FightState>();
        rect = GetComponent<RectTransform>();
        backgroundImage = GetComponent<Image>();
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
    /* Update the turn indicator and slide it into view. */
    {
        // Start it in the correct state (off screen and transparent).
        reset();

        // Set the text and color to be for the current fighter.
        bool isPlayerTurn = fightState.currentFighter.isPlayer;
        Color backgroundColor = isPlayerTurn
            ? playerBackgroundColor
            : enemyBackgroundColor;
        Color textColor = isPlayerTurn ? playerTextColor : enemyTextColor;
        string whoseTurn = fightState.currentFighter.isPlayer ? "Your" : "Enemy's";
        backgroundImage.color = backgroundColor;
        turnText.color = textColor;
        turnText.text = $"{whoseTurn} Turn";

        // Move it into view.
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
