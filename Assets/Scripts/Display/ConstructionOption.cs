using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using JoshH.UI;

public class ConstructionOption : MonoBehaviour, IPointerClickHandler
{
    /* Constants. */
    public Color defaultBackgroundColor_1 = new Color(0.8f, 0.5f, 1f);
    public Color defaultBackgroundColor_2 = new Color(0.6f, 0.2f, 1f);
    public Color defaultTextColor = new Color(0.2f, 0.2f, 0.2f);
    public Color selectedBackgroundColor_1 = new Color(0.67f, 0.25f, 1f);
    public Color selectedBackgroundColor_2 = new Color(0.4f, 0f, 0.9f);
    public Color selectedTextColor = new Color(0.7f, 0.7f, 0.7f);

    /* Parameters. */
    public string optionKey;

    /* References. */
    private SkillCreationState skillCreationState;
    private Image backgroundImage;
    private UIGradient backgroundGradient;
    private Text text;

    void Awake()
    {
        skillCreationState =
            GameObject.Find("SkillCreationCanvas").GetComponent<SkillCreationState>();
        backgroundImage = GetComponent<Image>();
        backgroundGradient = GetComponent<UIGradient>();
        text = transform.Find("Text").GetComponent<Text>();
    }

    void Update()
    {
        // Set the text of this option.
        text.text = optionKey;

        // Indicate the selected option (with color and boldness).
        if (skillCreationState.editedComponent != null)
        {
            bool isSelected = (
                skillCreationState.editedComponent.selectedConstruction == optionKey
            );

            backgroundGradient.LinearColor1 = isSelected
                ? selectedBackgroundColor_1
                : defaultBackgroundColor_1;
            backgroundGradient.LinearColor2 = isSelected
                ? selectedBackgroundColor_2
                : defaultBackgroundColor_2;
            text.color = isSelected ? selectedTextColor : defaultTextColor;
            text.fontStyle = isSelected ? FontStyle.Bold : FontStyle.Normal;
        }
    }

    public void OnPointerClick(PointerEventData ped)
    {
        skillCreationState.setSelectedConstruction(optionKey);
    }
}
