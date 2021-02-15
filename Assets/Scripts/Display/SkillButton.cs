using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillButton
    : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    /* Constants. */
    static public Color uncuedColor = new Color(0.75f, 0.75f, 0.75f);
    static public Color cuedColor = new Color(0.92f, 0.75f, 0.36f);
    static public List<Sprite> iconOptionsStatic;

    /* References. */
    public List<Sprite> iconOptions; // populated on the prefab in the Inspector
    private FightState fightState;
    public SkillState skill;
    private GameObject buttonBackgroundObj;
    private Image skillImage;
    private NiceShine buttonShine;
    private GameObject cooldownObj;
    private Text cooldownText;
    private GameObject skillInfoObj;

    /* State. */
    private int turnListenerId;
    private int actionListenerId;
    private int cuedSkillListenerId;
    private int shineListenerId;

    void Awake()
    {
        // When the first SkillButton is created, populate this field so other objects
        // can access the icons off the SkillButton class itself.
        if (iconOptionsStatic == null)
            iconOptionsStatic = iconOptions;

        fightState = GameObject.Find("GeneralScripts").GetComponent<FightState>();

        buttonBackgroundObj = transform.Find("ButtonBackground").gameObject;
        skillImage = transform.Find("SkillImage").GetComponent<Image>();
        buttonShine = transform
            .Find("ButtonBackground/ButtonShine")
            .GetComponent<NiceShine>();
        cooldownObj = transform.Find("SkillImage/CooldownBackground").gameObject;
        cooldownText = transform
            .Find("SkillImage/CooldownBackground/CooldownText")
            .GetComponent<Text>();
    }

    void Start()
    {
        // Set the icon (must be in Start, since Awake runs before `skill` is set).
        Sprite skillIcon = getIconByName(skill.iconName);
        skillImage.sprite = skillIcon;

        skillInfoObj = PrefabInstantiator.P.CreateSkillInfo(skill, transform);
        skillInfoObj.SetActive(false);

        updateThisSkillButton();
        turnListenerId = fightState.addTurnListener(updateThisSkillButton);
        actionListenerId = fightState.addActionListener(updateThisSkillButton);
        cuedSkillListenerId = fightState.addCuedSkillListener(updateThisSkillButton);

        shineListenerId = fightState.addCuedSkillListener(updateThisSkillButtonShine);
    }

    void OnDestroy()
    {
        fightState.removeTurnListener(turnListenerId);
        fightState.removeActionListener(actionListenerId);
        fightState.removeCuedSkillListener(cuedSkillListenerId);

        fightState.removeCuedSkillListener(shineListenerId);
    }

    public void OnPointerEnter(PointerEventData ped)
    {
        skillInfoObj.SetActive(true);
    }

    public void OnPointerExit(PointerEventData ped)
    {
        skillInfoObj.SetActive(false);
    }

    public void OnPointerClick(PointerEventData ped)
    {
        // Don't do anything if not enough actions to cue this button.
        bool canBeCued = fightState.canSkillBeCued(skill.skillId);
        if (!canBeCued)
            return;

        // Toggle this skill as cued or not.
        if (fightState.cuedSkillId == skill.skillId)
        {
            fightState.uncueSkill();
        }
        else
        {
            fightState.cueSkill(skill.skillId);
        }
    }

    /* Helpers. */

    static public Sprite getIconByName(string iconName)
    /* Given a string name of an icon, get the Sprite object for that icon from the
    list of options stored on this prefab.

    :param string iconName: the name of the Sprite as displayed in the Inspector,
        like "icon_14"
    
    :return Sprite icon:
    */
    {
        foreach (Sprite icon in iconOptionsStatic)
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
        // When cued, skill button background is larger and gold.
        bool isThisSkillCued = fightState.cuedSkillId == skill.skillId;
        float cuedSize = 80;
        float uncuedSize = 77;
        float buttonBackgroundSize = isThisSkillCued ? cuedSize : uncuedSize;
        buttonBackgroundObj.GetComponent<RectTransform>().sizeDelta =
            new Vector2(buttonBackgroundSize, buttonBackgroundSize);
        Color buttonBackgroundColor = isThisSkillCued ? cuedColor : uncuedColor;
        buttonBackgroundObj.GetComponent<Image>().color = buttonBackgroundColor;

        // Fade this skill if not enough actions left to use.
        bool canBeCued = fightState.currentFighter.currentActions >= skill.actionCost;
        Color skillImageColor = new Color(1, 1, 1);
        if (!canBeCued)
            skillImageColor.a = 0.5f;
        skillImage.color = skillImageColor;

        // When on cooldown, darken the button and show the number of turns remaining.
        bool isOnCooldown = skill.currentCooldown > 0;
        cooldownText.text = skill.currentCooldown.ToString();
        cooldownObj.SetActive(isOnCooldown);
    }

    private void updateThisSkillButtonShine()
    /* Start or stop button shine based on whether or not this skill is cued. */
    {
        bool isThisSkillCued = fightState.cuedSkillId == skill.skillId;
        if (isThisSkillCued)
            buttonShine.animate();
        else
            buttonShine.stop();
    }
}
