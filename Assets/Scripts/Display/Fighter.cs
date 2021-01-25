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
        bool isAnySkillCued = fightState.cuedSkillId != -1;
        bool doHighlightTarget = isHovered && isAnySkillCued;
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
    {
        // TODO: if a skill was selected and it targets a fighter, apply the skill
    }
}
