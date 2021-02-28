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
    private Text componentNameText;

    void Awake()
    {
        componentNameText = transform.Find("Text").GetComponent<Text>();
    }

    void Start()
    {

    }

    void Update()
    {
        componentNameText.text = component.getName();
    }
}
