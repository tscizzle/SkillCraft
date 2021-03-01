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

        canvasGroup.alpha = isAvailable() ? 0.9f : 0.6f;
    }

    /* Helpers. */

    private bool isAvailable()
    /* Whether or not this component is available for use in a new slot right now (for
    example, it may already be in use). If not available, we don't allow it to be
    clicked, and we fade it in the list.

    :return bool:
    */
    {
        bool isAvailable = true;

        if (component.parentComponentId != -1)
            isAvailable = false;

        return isAvailable;
    }
}
