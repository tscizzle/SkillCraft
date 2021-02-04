using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillInfo : MonoBehaviour
{
    /* Parameters. */
    public SkillState skill;

    /* References. */
    private Text nameText;
    private Image skillImage;
    private Text actionCostText;
    private Text cooldownText;
    private Text detailsText;

    void Awake()
    {

        nameText = transform.Find("NameText").GetComponent<Text>();
        skillImage = transform.Find("ImageRow/SkillImage").GetComponent<Image>();
        actionCostText = transform
            .Find("ImageRow/ActionInfoContainer/ActionCostText")
            .GetComponent<Text>();
        cooldownText = transform.Find("CooldownText").GetComponent<Text>();
        detailsText = transform.Find("DetailsText").GetComponent<Text>();
    }

    void Start()
    {
        nameText.text = skill.name;
        skillImage.sprite =
            transform.parent.Find("SkillImage").GetComponent<Image>().sprite;
        actionCostText.text = $"x{skill.actionCost}";
        cooldownText.text = $"Cooldown: {skill.cooldown}";
        detailsText.text = $"Damage: {skill.damage} physical";
    }

    void Update()
    {
        Vector3 offsetFromCursor = new Vector3(0, 20, 0);
        Vector3 cursorPosition = Input.mousePosition;
        transform.position = cursorPosition + offsetFromCursor;
    }
}
