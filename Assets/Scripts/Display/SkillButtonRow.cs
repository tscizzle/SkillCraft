using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillButtonRow : MonoBehaviour
{
    /* References. */
    private FightState fightState;
    private List<GameObject> skillButtonObjs = new List<GameObject>();

    void Awake()
    {
        fightState = GameObject.Find("GeneralScripts").GetComponent<FightState>();
    }

    void Start()
    {
        updateCurrentSkillButtons();

        fightState.addTurnChangeListener(updateCurrentSkillButtons);
    }

    /* PUBLIC API */

    public void updateCurrentSkillButtons()
    /* When it becomes a new fighter's turn, call this method to remove the skill
    buttons that were displayed and show the ones for the new current fighter.
    */
    {
        // Delete SkillButtons from the previous fighter's turn.
        foreach (GameObject skillButtonObj in skillButtonObjs)
        {
            Destroy(skillButtonObj);
        }
        skillButtonObjs = new List<GameObject>();
        // Create a SkillButton for each of the current fighter's skills.
        Dictionary<int, SkillState> currentSkills = fightState.currentFighter.skills;
        int numSkills = currentSkills.Count;
        int skillIdx = 0;
        foreach (SkillState skill in currentSkills.Values)
        {
            // Create the SkillButton.
            GameObject skillButtonObj = PrefabInstantiator.P.CreateSkillButton(skill);
            skillButtonObjs.Add(skillButtonObj);
            // Position it based on SkillButton size and number of skills.
            RectTransform skillRect = skillButtonObj.GetComponent<RectTransform>();
            float width = skillRect.rect.width;
            float leftMostX = -1 * (numSkills / 2) * width;
            float posX = leftMostX + skillIdx * width;
            skillRect.offsetMin = new Vector2(posX, 0);
            skillRect.offsetMax = new Vector2(posX + width, 0);

            skillIdx += 1;
        }
    }
}
