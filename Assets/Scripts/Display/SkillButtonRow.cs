using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButtonRow : MonoBehaviour
{
    /* References. */
    FightState fightState;

    void Awake()
    {
        fightState = GameObject.Find("GeneralScripts").GetComponent<FightState>();
    }

    void Start()
    {
        // Display a SkillButton for each of the current fighter's skills.
        Dictionary<int, SkillState> currentSkills = fightState.currentFighter.skills;
        int numSkills = currentSkills.Count;
        int skillIdx = 0;
        foreach (SkillState skill in currentSkills.Values)
        {
            // Create the SkillButton.
            GameObject skillButtonObj = PrefabInstantiator.P.CreateSkillButton(skill);
            // Position it based on SkillButton size and number of skills.
            RectTransform skillRect = skillButtonObj.GetComponent<RectTransform>();
            float width = skillRect.rect.width;
            float leftMostX = -1 * (numSkills / 2) * width;
            float posX = leftMostX + skillIdx * width;
            skillRect.offsetMin = new Vector2(posX, 0);
            skillRect.offsetMax = new Vector2(posX + width, 0);
            // Set the skill icon.
            SkillButton skillButton = skillButtonObj.GetComponent<SkillButton>();
            Sprite skillIcon = skillButton.getIconByName(skill.iconName);
            GameObject skillImageObj =
                skillButtonObj.transform.Find("SkillImage").gameObject;
            skillImageObj.GetComponent<Image>().sprite = skillIcon;

            skillIdx += 1;
        }
    }

    void Update()
    {

    }
}
