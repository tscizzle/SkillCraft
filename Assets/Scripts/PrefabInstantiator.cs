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
    public GameObject skillInfoPrefab;

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

    :param SkillState skill: Which skill this button is for.
    
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

        // The health bar is a UI object, so make it a child of Canvas.
        healthBarObj.transform.SetParent(canvasObj.transform);

        return healthBarObj;
    }

    public GameObject CreateSkillInfo(SkillState skill, Transform parent = null)
    /* Create a display for info about a skill (damage, actions, cooldown, etc.).

    :param SkillState skill: Which skill this display is about.
    :param Transform parent: If supplied, make this skill info a child of `parent`.
    
    :returns GameObject skillInfoObj:
    */
    {
        GameObject skillInfoObj = Instantiate(
            skillInfoPrefab, Vector3.zero, Quaternion.identity
        );

        SkillInfo skillInfo = skillInfoObj.GetComponent<SkillInfo>();
        skillInfo.skill = skill;

        // The skill info is a UI object, so make it a child of Canvas.
        parent = parent != null ? parent : canvasObj.transform;
        skillInfoObj.transform.SetParent(parent);

        return skillInfoObj;
    }
}
