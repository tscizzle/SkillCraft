using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ComponentBubble :
    MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    /* References. */
    private SkillCreationWall skillCreationWall;
    private SkillCreationState skillCreationState;
    private GameObject writtenAsWordsObj;
    private Text writtenAsWordsText;

    /* State. */
    private bool isHovered = false;

    void Awake()
    {
        skillCreationWall =
            GameObject.Find("SkillCreationWall").GetComponent<SkillCreationWall>();
        skillCreationState =
            GameObject.Find("SkillCreationState").GetComponent<SkillCreationState>();
        writtenAsWordsObj = transform.Find("WrittenAsWords").gameObject;
        writtenAsWordsText = transform.Find("WrittenAsWords/Text").GetComponent<Text>();
    }

    void Start()
    {

    }

    void Update()
    {
        // TODO: get the different structures this component could be and display them
        // as buttons lined up horizontally in this bubble

        writtenAsWordsObj.SetActive(isHovered);
        writtenAsWordsText.text = representAsWords();
    }

    public void OnPointerClick(PointerEventData ped)
    {

    }

    public void OnPointerEnter(PointerEventData ped)
    {
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData ped)
    {
        isHovered = false;
    }

    /* Helpers. */

    private string representAsWords()
    /* This ComponentBubble represents some Component (a Step, a Status, etc., see
    SkillGrammar) which can each be represented in words, as in a skill's description.

    :return string:
    */
    {
        return "Do 5 Fire damage (Piercing).";
    }
}
