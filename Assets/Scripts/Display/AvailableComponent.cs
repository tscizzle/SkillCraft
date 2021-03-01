using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SG = SkillGrammar;

public class AvailableComponent : MonoBehaviour
{
    /* Parameters. */
    public SG.Component component;

    /* References. */
    private CanvasGroup canvasGroup;
    private Text labelText;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        labelText = transform.Find("Text").GetComponent<Text>();
    }

    void Start()
    {

    }

    void Update()
    {
        labelText.text = component.getLabel();

        canvasGroup.alpha = component.isAvailable ? 0.9f : 0.6f;
    }
}
