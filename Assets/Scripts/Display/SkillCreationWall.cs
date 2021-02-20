using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class SkillCreationWall
    : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    /* Constants. */
    private float hoveredAlpha = 0.2f;
    private float viewedAlpha = 0.16f;

    /* References. */
    private SkillCreationState skillCreationState;
    private InterfightCameraView interfightCameraView;
    private Image backgroundImage;
    private InputField skillNameInput;
    private GameObject imageInputObj;
    private GameObject imageInputTextObj;
    private GameObject imageInputImageObj;
    private GameObject imageOptionsObj;
    private GameObject stepsContainerObj;

    /* Parameters. */
    public List<Sprite> iconOptions; // populated in the Inspector

    /* State. */
    private bool isHovered = false;

    void Awake()
    {
        skillCreationState =
            GameObject.Find("SkillCreationWall").GetComponent<SkillCreationState>();
        interfightCameraView =
            GameObject.Find("GeneralScripts").GetComponent<InterfightCameraView>();
        backgroundImage = transform.Find("TransparentBackground").GetComponent<Image>();
        skillNameInput =
            transform.Find("EditedSkill/NameInput").GetComponent<InputField>();
        imageInputObj = transform.Find("EditedSkill/ImageInput").gameObject;
        imageInputTextObj = transform.Find("EditedSkill/ImageInput/Text").gameObject;
        imageInputImageObj = transform.Find("EditedSkill/ImageInput/Image").gameObject;
        imageOptionsObj =
            transform.Find("EditedSkill/ImageInput/ImageOptionsScrollView").gameObject;
        stepsContainerObj = transform.Find("EditedSkill/StepsContainer").gameObject;

        // Update the skill name variable when the user types.
        skillNameInput.onValueChanged.AddListener(
            newValue => skillCreationState.setEditedName(newValue)
        );
    }

    void Start()
    {
        populateImageOptions();
    }

    void Update()
    {
        setBackgroundAlpha();
    }

    public void OnPointerEnter(PointerEventData ped)
    {
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData ped)
    {
        isHovered = false;
    }

    public void OnPointerClick(PointerEventData ped)
    {
        if (interfightCameraView.currentView == CameraView.Start)
        // Not viewing a wall yet. Any click here brings the view to this wall.
        {
            interfightCameraView.setView(CameraView.Right);
        }
        else if (interfightCameraView.currentView == CameraView.Right)
        // Already viewing this wall. Clicks here can do actual skill creation logic.
        {
            GameObject clickedObj = ped.rawPointerPress;

            if (clickedObj == imageInputTextObj || clickedObj == imageInputImageObj)
            // Toggle the image options scroll view open or closed.
            {
                imageOptionsObj.SetActive(!imageOptionsObj.activeSelf);
            }
            else if (clickedObj.name.Contains("ImageOption_"))
            // Select a new image for this skill.
            {
                Sprite clickedImage = clickedObj.GetComponent<Image>().sprite;

                skillCreationState.setEditedImage(clickedImage);

                imageInputTextObj.SetActive(false);
                imageInputImageObj.SetActive(true);
                imageInputImageObj.GetComponent<Image>().sprite = clickedImage;
                imageInputImageObj.GetComponent<Image>().DOFade(1, 1).From(0);

                imageOptionsObj.SetActive(false);
            }
        }
    }

    /* Helpers. */

    private void setBackgroundAlpha()
    /* Set the transparency of the background that covers the whole canvas (keeping the
    same RGB color it already was).
    */
    {
        CameraView currentView = interfightCameraView.currentView;

        float alpha;
        if (currentView == CameraView.Right)
            alpha = viewedAlpha;
        else if (currentView == CameraView.Start && isHovered)
            alpha = hoveredAlpha;
        else
            alpha = 0;

        Color color = backgroundImage.color;
        color.a = alpha;
        backgroundImage.color = color;
    }

    private void populateImageOptions()
    /* For every icon option, place an object in the scrollable list of images. */
    {
        GameObject contentObj =
            imageOptionsObj.transform.Find("Viewport/Content").gameObject;

        foreach (Sprite icon in iconOptions)
        {
            // Create an object (and name it something we can look for in the wall's
            // click handler).
            GameObject imageOptionObj = new GameObject($"ImageOption_{icon.name}");
            // Give this object the image of the relevant icon.
            Image imageOption = imageOptionObj.AddComponent<Image>();
            imageOption.sprite = icon;
            // Place this object in the hierarchy in the content of the scroll view.
            imageOptionObj
                .GetComponent<RectTransform>()
                .SetParent(contentObj.transform);
            // Set position and size.
            Vector3 position =
                imageOptionObj.GetComponent<RectTransform>().localPosition;
            position.z = 0;
            imageOptionObj.GetComponent<RectTransform>().localPosition = position;
            imageOptionObj.GetComponent<RectTransform>().localScale = Vector3.one;
        }
    }
}
