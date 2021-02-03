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
    private NiceShine buttonShine;

    /* State. */
    private int actionListenerId;
    private int cuedSkillListenerId;

    void Awake()
    {
        fightState = GameObject.Find("GeneralScripts").GetComponent<FightState>();

        buttonBackgroundObj = transform.Find("ButtonBackground").gameObject;
        skillImageObj = transform.Find("SkillImage").gameObject;
        buttonShine =
            transform.Find("ButtonBackground/ButtonShine").GetComponent<NiceShine>();

        actionListenerId = fightState.addActionListener(updateThisSkillButton);
        cuedSkillListenerId = fightState.addCuedSkillListener(updateThisSkillButton);
    }

    void Start()
    {
        // Set the icon (must be in Start, since Awake runs before `skill` is set).
        Sprite skillIcon = getIconByName(skill.iconName);
        skillImageObj.GetComponent<Image>().sprite = skillIcon;
    }

    void OnDestroy()
    {
        fightState.removeActionListener(actionListenerId);
        fightState.removeCuedSkillListener(cuedSkillListenerId);
    }

    public void OnPointerClick(PointerEventData ped)
    {
        // Don't do anything if not enough actions to cue this button.
        bool canBeCued = fightState.currentFighter.currentActions >= skill.actionCost;
        if (!canBeCued)
            return;

        // Toggle this skill as cued or not.
        if (fightState.cuedSkillId == skill.skillId)
        {
            fightState.uncueSkill();
            buttonShine.stop();
        }
        else
        {
            fightState.cueSkill(skill.skillId);
            buttonShine.animate();
        }
    }

    /* Helpers. */

    private Sprite getIconByName(string iconName)
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
                return icon;
        }
        // Return null if no icon by that name was found.
        return null;
    }

    private void updateThisSkillButton()
    /* Update the look of this skill button, based on number of actions available and
    the cued skill (e.g. skills which cost more than the available actions are faded,
    the skill that is cued is bordered, etc.)
    */
    {
        // When not cued, skill button background is smaller and gray.
        // When cued, skill button background is larged and gold.
        bool isThisSkillCued = fightState.cuedSkillId == skill.skillId;
        float cuedSize = 80;
        float uncuedSize = 76;
        float buttonBackgroundSize = isThisSkillCued ? cuedSize : uncuedSize;
        buttonBackgroundObj.GetComponent<RectTransform>().sizeDelta =
            new Vector2(buttonBackgroundSize, buttonBackgroundSize);
        Color uncuedColor = new Color(0.75f, 0.75f, 0.75f);
        Color cuedColor = new Color(0.92f, 0.75f, 0.36f);
        Color buttonBackgroundColor = isThisSkillCued ? cuedColor : uncuedColor;
        buttonBackgroundObj.GetComponent<Image>().color = buttonBackgroundColor;

        // When not cued, disable button shine.
        if (!isThisSkillCued)
            buttonShine.stop();

        // Fade this skill if not enough actions left to use.
        bool canBeCued = fightState.currentFighter.currentActions >= skill.actionCost;
        Color skillImageColor = new Color(1, 1, 1);
        if (!canBeCued)
            skillImageColor.a = 0.5f;
        skillImageObj.GetComponent<Image>().color = skillImageColor;
    }
}
