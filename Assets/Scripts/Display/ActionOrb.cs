using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionOrb : MonoBehaviour
{
    /* References. */
    private FightState fightState;
    private GameObject actionOrbRowObj;
    private GameObject actionOrbImageObj;
    private NiceShine orbShine;

    /* Parameters. */
    private int actionIdx;

    void Awake()
    {
        fightState = GameObject.Find("GeneralScripts").GetComponent<FightState>();

        actionOrbRowObj = GameObject.Find("ActionOrbRow");
        actionOrbImageObj = transform.Find("OrbImage").gameObject;
        orbShine = transform.Find("OrbBackground/OrbShine").GetComponent<NiceShine>();

        setActionIdx();

        // Register this object to update its display when needed.
        fightState.addActionListener(updateThisActionOrb);
        fightState.addCuedSkillListener(updateThisActionOrb);
    }

    /* Helpers. */

    private void setActionIdx()
    /* Save the index of this action orb based on its position in the hierarchy. */
    {
        int childIdx = 0;
        foreach (Transform child in actionOrbRowObj.transform)
        {
            if (child.name == name)
            {
                actionIdx = childIdx;
                break;
            }
            childIdx++;
        }
    }

    private void updateThisActionOrb()
    /* Update the look of this action orb, based on number of actions available, which
    orb in the order this is, and the cued skill (e.g. unavailable actions are faded,
    actions about to be used by the cued skill are colored, etc.).
    */
    {
        bool isAvailable = isActionOrbAvailable();
        bool isCuedToBeUsed = isActionOrbCuedToBeUsed();

        if (isAvailable)
            orbShine.animate();
        else
            orbShine.stop();

        Color color = new Color(1, 1, 1);
        if (!isAvailable)
        {
            color.a = 0.4f;
        }
        else if (isCuedToBeUsed)
        {
            color.g = 0;
            color.a = 0.8f;
        }
        actionOrbImageObj.GetComponent<Image>().color = color;
    }

    private bool isActionOrbAvailable()
    /* If the current fighter has N actions available, the first N action orbs from left
    to right are displayed as available.
    */
    {
        return actionIdx < fightState.currentFighter.currentActions;
    }

    private bool isActionOrbCuedToBeUsed()
    /* If the cued skill would use N actions, the first N action orbs from right to
    left (only including available ones) are displayed as cued to be used.
    */
    {
        // No action orbs are cued to be used if no skill is cued.
        if (fightState.cuedSkill == null)
        {
            return false;
        }

        int actions = fightState.currentFighter.currentActions;
        int cost = fightState.cuedSkill.actionCost;
        return actions - cost <= actionIdx && actionIdx < actions;
    }
}
