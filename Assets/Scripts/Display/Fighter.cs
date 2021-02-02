using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    /* Parameters. */
    //[System.NonSerialized] // TODO: setting this in inspector for now. later, don't.
    public bool isPlayer;
    [System.NonSerialized]
    public int fighterId;

    /* References. */
    private FightState fightState;
    private GameObject characterObj;
    private GameObject hoverLightObj;

    /* State. */
    private bool isHovered;

    void Awake()
    {
        fightState = GameObject.Find("GeneralScripts").GetComponent<FightState>();
        characterObj = transform.Find("Character").gameObject;
        hoverLightObj = transform.Find("HoverLight").gameObject;
    }

    void Start()
    {
        // TODO: While we are hard-coding the Fighter objects instead of creating them
        // dynamically, use this hack which assumes 1 player and 1 enemy.
        fighterId = isPlayer
            ? fightState.player.fighterId
            : fightState.enemies.Values.ToList()[0].fighterId;

        PrefabInstantiator.P.CreateHealthBar(gameObject);
    }

    void Update()
    {
        // Highlight this fighter or not, based on the cued skill and if hovering this
        // fighter with the mouse.
        bool doHighlightTarget = false;
        if (fightState.cuedSkill != null)
        {
            bool isThisFightersTurn = fighterId == fightState.currentFighter.fighterId;
            bool canCuedSkillTargetThisFighter = (
                (!isThisFightersTurn && fightState.cuedSkill.canTargetEnemy)
                || (isThisFightersTurn && fightState.cuedSkill.canTargetSelf)
            );
            if (isHovered && canCuedSkillTargetThisFighter)
            {
                doHighlightTarget = true;
            }
        }
        hoverLightObj.SetActive(doHighlightTarget);
    }

    void OnMouseEnter()
    {
        isHovered = true;
    }

    void OnMouseExit()
    {
        isHovered = false;
    }

    void OnMouseUpAsButton()
    /* When clicking a fighter, if a skill is cued, attempt to use it on that fighter.
    If it happens
    */
    {
        if (fightState.cuedSkill != null)
        {
            string failureReason = fightState.useSkill(fighterId);

            // If skill failed, display the reason to the user.
            // If it worked, give feedback like camera shake or audio clip.
            if (failureReason != null)
            {
                // TODO: display reason, like "Not enough actions."
            }
            else
            {
                StartCoroutine(CameraShake.shake());
            }
        }
    }
}
