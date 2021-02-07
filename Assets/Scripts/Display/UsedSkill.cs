using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UsedSkill : MonoBehaviour
{
    /* References. */
    private FightState fightState;
    private RectTransform rect;
    private Image skillBackground;
    private Image skillImage;
    private Tween currentTween;

    void Awake()
    {
        fightState = GameObject.Find("GeneralScripts").GetComponent<FightState>();
        rect = transform.GetComponent<RectTransform>();
        skillBackground = transform.GetComponent<Image>();
        skillImage = transform.Find("UsedSkillImage").GetComponent<Image>();

        reset();
    }

    void Start()
    {
        fightState.addSkillUsedListener(animate);
    }

    /* PUBLIC API. */

    public void animate()
    /* Slide an image of the cued skill (which, presumably, was just used) into view.
    
    https://easings.net/ has info on the Ease values.
    */
    {
        // Start it in the correct state (off screen and transparent).
        reset();

        /* Set the image to be the used skill. */
        Sprite skillIcon = SkillButton.getIconByName(fightState.cuedSkill.iconName);
        skillImage.sprite = skillIcon;

        /* Fade+move the it into view and then out.
        
        The fade-in gets to opaque quickly, with OutQuad.
        The fade-out stays opaque longer, with InQuad.
        The move-in wiggles, with OutElastic.
        The move-out goes down briefly then up quickly, with InBack and faraway target.
        */
        float inDuration = 1;
        float stayDuration = 0.5f;
        float outDuration = 1;
        Sequence seq = DOTween.Sequence();
        // Fade the skill background.
        Sequence backgroundFadeSeq = DOTween.Sequence()
            .Append(skillBackground.DOFade(1, inDuration).SetEase(Ease.OutQuad))
            .AppendInterval(stayDuration)
            .Append(skillBackground.DOFade(0, outDuration).SetEase(Ease.InQuad));
        // Fade the skill image.
        Sequence imageFadeSeq = DOTween.Sequence()
            .Append(skillImage.DOFade(1, inDuration).SetEase(Ease.OutQuad))
            .AppendInterval(stayDuration)
            .Append(skillImage.DOFade(0, outDuration).SetEase(Ease.InQuad));
        // Movement.
        Sequence moveSeq = DOTween.Sequence()
            .Append(rect.DOAnchorPosY(-200, inDuration).SetEase(Ease.OutElastic, 0.3f))
            .AppendInterval(stayDuration)
            .Append(rect.DOAnchorPosY(10000, outDuration).SetEase(Ease.InBack, 0.5f));
        // Combine them all.
        seq.Insert(0, backgroundFadeSeq);
        seq.Insert(0, imageFadeSeq);
        seq.Insert(0, moveSeq);

        currentTween = seq;
    }

    /* Helpers. */

    private void reset()
    /* Start this image off screen and transparent, so animating it gradually brings it
    into view.
    */
    {
        if (currentTween != null) currentTween.Kill();

        // Start the background and skill image as transparent.
        Color transparentBackgroundColor = skillBackground.color;
        transparentBackgroundColor.a = 0;
        skillBackground.color = transparentBackgroundColor;
        Color transparentImageColor = skillImage.color;
        transparentImageColor.a = 0;
        skillImage.color = transparentImageColor;
        // Start it just above screen (its anchor and pivot are such that (0, 0) is just
        // above the middle of the screen).
        rect.anchoredPosition = Vector3.zero;
    }
}
