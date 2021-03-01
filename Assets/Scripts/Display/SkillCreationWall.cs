using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using SG = SkillGrammar;

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
    private GameObject constructionOptionsObj;
    private Text editedComponentRootText;
    private GameObject availableComponentsContentObj;

    /* Parameters. */
    public List<Sprite> iconOptions; // populated in the Inspector

    /* State. */
    private bool isHovered = false;

    void Awake()
    {
        skillCreationState =
            GameObject.Find("SkillCreationCanvas").GetComponent<SkillCreationState>();
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
        constructionOptionsObj =
            transform.Find("ComponentPlayground/ConstructionOptions").gameObject;
        editedComponentRootText = transform
            .Find("ComponentPlayground/EditedComponentRoot/Text")
            .GetComponent<Text>();
        availableComponentsContentObj = transform
            .Find("AvailableComponents/ScrollView/Viewport/Content").gameObject;

        // Update the skill name variable when the user types.
        skillNameInput.onValueChanged.AddListener(
            newValue => skillCreationState.setEditedName(newValue)
        );

        // Update the display when certain state changes.
        skillCreationState.addEditedComponentListener(updateEditedComponent);
        skillCreationState.addAvailableComponentsListener(updateAvailableComponents);
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
                bool isOpening = !imageOptionsObj.activeSelf;
                // Show the image options overlay.
                imageOptionsObj.SetActive(isOpening);
                // Update the button text (only visible if not image selected yet).
                string text = isOpening ? "X" : "Select image...";
                int fontSize = isOpening ? 50 : 24;
                imageInputTextObj.GetComponent<Text>().text = text;
                imageInputTextObj.GetComponent<Text>().fontSize = fontSize;
                // Move the image options overlay to be top-level in the UI hierarchy,
                // and last, so that it appears in front of the rest. (Doing this in
                // script so in editor it can still be child of the appropriate object,
                // and if that object moves around this one will follow accordingly.)
                imageOptionsObj.transform.SetParent(transform);
                imageOptionsObj.transform.SetAsLastSibling();
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

    private void updateEditedComponent()
    /* When the edited component is changed, update the component playground display. */
    {
        // Clear out previous construction options and create the correct new ones.
        foreach (Transform child in constructionOptionsObj.transform)
            Destroy(child.gameObject);
        if (skillCreationState.editedComponent != null)
        {
            List<string> constructionOptionKeys =
                skillCreationState.editedComponent.constructionOptions.Keys.ToList();
            foreach (string optionKey in constructionOptionKeys)
                PrefabInstantiator.P.CreateConstructionOption(
                    optionKey, constructionOptionsObj.transform
                );
        }

        // Display the type of component being edited.
        editedComponentRootText.text =
            skillCreationState.editedComponent.GetType().Name;

        // TODO: if component has a parent, display a "zoom out 1 level" button to the left

        // TODO: display the subcomponents beneath the root, based on the type of
        //  component and the selectedConstruction
    }

    private void updateAvailableComponents()
    /* When an available component is added or removed, update the available components
    list.
    */
    {
        // Sync available components between what's displayed and what's in state:
        // - Any in state and not displayed need to be added.
        // - Any displayed and not in state need to be removed.
        AvailableComponent[] displayedAvailableComponents =
            availableComponentsContentObj.GetComponentsInChildren<AvailableComponent>();
        List<int> displayedIds = displayedAvailableComponents.Select(
            ac => ac.component.componentId
        ).ToList();
        foreach (AvailableComponent availableComponent in displayedAvailableComponents)
        {
            int componentId = availableComponent.component.componentId;
            if (!skillCreationState.componentMap.ContainsKey(componentId))
            {
                Destroy(availableComponent);
            }
        }
        foreach (KeyValuePair<int, SG.Component> kvp in skillCreationState.componentMap)
        {
            int componentId = kvp.Key;
            SG.Component component = kvp.Value;
            if (!isComponentAvailable(component))
                continue;
            if (!displayedIds.Contains(componentId))
            {
                PrefabInstantiator.P.CreateAvailableComponent(
                    component, availableComponentsContentObj.transform
                );
            }
        }
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
    private bool isComponentAvailable(SG.Component component)
    {
        bool isAvailable = true;

        // Consider Step components unavailable for use in other components, since they
        // are the outermost component type (besides skills themselves).
        if (component.GetType() == typeof(SG.Step))
            isAvailable = false;

        return isAvailable;
    }
}
