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

    /* State. */
    private bool previousIsAvailable = false;

    void Awake()
    {
        fightState = GameObject.Find("GeneralScripts").GetComponent<FightState>();

        actionOrbRowObj = GameObject.Find("ActionOrbRow");
        actionOrbImageObj = transform.Find("OrbImage").gameObject;
        orbShine = transform.Find("OrbBackground/OrbShine").GetComponent<NiceShine>();

        // Get which action orb in the row this is (i.e. set `actionIdx`).
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

    void Update()
    {
        // If this action orb represents an action that is available, highlight it with
        // shine and color (though only need to if it wasn't previously).

        bool isAvailable = isActionOrbAvailable();

        if (isAvailable && !previousIsAvailable)
        {
            orbShine.animate();
        }
        actionOrbImageObj.GetComponent<Image>().color =
            isAvailable ? new Color(1, 1, 1) : new Color(0.8f, 0.8f, 0.8f, 0.6f);

        previousIsAvailable = isAvailable;
    }

    /* Helpers. */

    private bool isActionOrbAvailable()
    /* If the current fighter has N action orbs available, this method returns true if
        this action orb is one of the first N from left to right.
    */
    {
        return actionIdx < fightState.currentFighter.currentActions;
    }
}
