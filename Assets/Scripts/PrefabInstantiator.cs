using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrefabInstantiator : MonoBehaviour
{
    // Global var that even a prefab can reference. Will be assigned our 1 instance of
    // PrefabInstantiator.
    public static PrefabInstantiator P;

    /* Misc references. */
    private GameObject canvasObj;
    private GameObject skillButtonRowObj;

    /* Prefab references. */
    public GameObject skillButtonPrefab;
    public GameObject healthBarPrefab;

    void Awake()
    {
        // Since there should only be 1 PrefabInstantiator instance, assign this
        // instance to a global var.
        P = this;

        canvasObj = GameObject.Find("Canvas");
        skillButtonRowObj = GameObject.Find("SkillButtonRow");
    }

    /* PUBLIC API */

    public GameObject CreateSkillButton(SkillState skill)
    /* Create a SkillButton in the row at the bottom of the screen.
    
    :returns GameObject skillButtonObj:
    */
    {
        GameObject skillButtonObj = Instantiate(
            skillButtonPrefab, Vector3.zero, Quaternion.identity
        );

        // Associate it with the skill.
        SkillButton skillButton = skillButtonObj.GetComponent<SkillButton>();
        skillButton.skill = skill;

        // Put the button inside the row at the bottom for skills.
        skillButtonObj.transform.SetParent(skillButtonRowObj.transform);

        return skillButtonObj;
    }

    public GameObject CreateHealthBar(GameObject fighterObj)
    /* Create a HealthBar for a Fighter.

    :param GameObject fighterObj: Fighter this health bar is for.
    
    :returns GameObject healthBarObj:
    */
    {
        GameObject healthBarObj = Instantiate(
            healthBarPrefab, Vector3.zero, Quaternion.identity
        );

        HealthBar healthBar = healthBarObj.GetComponent<HealthBar>();
        healthBar.fighterObj = fighterObj;

        // The health bar is a UI image, so make it a child of Canvas.
        healthBarObj.transform.SetParent(canvasObj.transform);

        return healthBarObj;
    }
}
