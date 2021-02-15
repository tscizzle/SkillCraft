using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillCreationWall
    : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    /* Constants. */
    private float hoveredAlpha = 0.2f;
    private float viewedAlpha = 0.16f;

    /* References. */
    private InterfightCameraView interfightCameraView;
    private Image backgroundImage;
    private InputField skillNameInput;

    /* State. */
    private bool isHovered = false;
    // `edited*` arefields of the skill being edited.
    private string editedName;
    private Sprite editedImage;

    void Awake()
    {
        interfightCameraView =
            GameObject.Find("GeneralScripts").GetComponent<InterfightCameraView>();
        backgroundImage = transform.Find("TransparentBackground").GetComponent<Image>();
        skillNameInput =
            transform.Find("EditedSkill/NameInput").GetComponent<InputField>();

        // Update the skill name variable when the user types.
        skillNameInput.onValueChanged.AddListener(newValue => editedName = newValue);
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
            interfightCameraView.setView(CameraView.Right);
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
}
