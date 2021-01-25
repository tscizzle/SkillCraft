using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillButton : MonoBehaviour, IPointerClickHandler
{
    // TODO: write a function to do something when used (basic damage, to start)
    // TODO: add cooldown logic and display

    /* References. */
    public List<Sprite> iconOptions; // populated on the prefab in the Inspector
    private FightState fightState;
    public SkillState skill;
    private GameObject buttonBackgroundObj;
    private GameObject skillImageObj;
    private GameObject buttonShineObj;

    void Awake()
    {
        fightState = GameObject.Find("GeneralScripts").GetComponent<FightState>();

        buttonBackgroundObj = transform.Find("ButtonBackground").gameObject;
        skillImageObj = transform.Find("SkillImage").gameObject;
        buttonShineObj = transform.Find("ButtonBackground/ButtonShine").gameObject;
    }

    void Start()
    {
        // Set the icon.
        Sprite skillIcon = getIconByName(skill.iconName);
        skillImageObj.GetComponent<Image>().sprite = skillIcon;

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    void Update()
    {
        bool isAnySkillCued = fightState.cuedSkillId != -1;
        bool isThisSkillCued = fightState.cuedSkillId == skill.skillId;

        /* Toggle background border based on if this skill is cued. */
        buttonBackgroundObj.SetActive(isThisSkillCued);
    }

    public void OnPointerClick(PointerEventData ped)
    {
        // Toggle this skill as cued or not.
        fightState.cuedSkillId =
            fightState.cuedSkillId == skill.skillId ? -1 : skill.skillId;
    }

    /* PUBLIC API. */

    public Sprite getIconByName(string iconName)
    /* Given a string name of an icon, get the Sprite object for that icon from the
    list of options stored on this prefab.

    :param string iconName: the name of the Sprite as displayed in the Inspector,
        like "icon_14"
    
    :return Sprite icon:
    */
    {
        foreach (Sprite icon in iconOptions)
        {
            if (icon.name == iconName)
            {
                return icon;
            }
        }
        // Return null if no icon by that name was found.
        return null;
    }
}
