using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillCreationWall : MonoBehaviour
{
    /* Constants. */
    private float hoverAlpha = 0.2f;

    /* References. */
    private SkillCreationCameraView skillCreationCameraView;
    private Image backgroundImage;

    /* State. */
    private bool isHovered = false;

    void Awake()
    {
        skillCreationCameraView =
            GameObject.Find("GeneralScripts").GetComponent<SkillCreationCameraView>();
        backgroundImage = transform.Find("TransparentBackground").GetComponent<Image>();
    }

    void Update()
    {
        if (noHoverEffect())
            setBackgroundAlpha(0);
        else
        {
            float alpha = isHovered ? hoverAlpha : 0;
            setBackgroundAlpha(alpha);
        }
    }

    /* PUBLIC API. */

    public void OnClickSkillCreationWall()
    /* Moves the camera to view the wall the skill creation UI is on. This function is
    registered as handler for when the wall is clicked.
    */
    {
        CameraView nextView = skillCreationCameraView.currentView == CameraView.Right
            ? CameraView.Start
            : CameraView.Right;
        skillCreationCameraView.setView(nextView);
    }

    public void OnHoverSkillCreationWall()
    /* Highlight the wall when hovered, to indicate that it's clickable. This function
    is registered as handler for when the mouse pointer enters the wall.
    */
    {
        isHovered = true;
    }

    public void OnUnhoverSkillCreationWall()
    /* Stop highlighting the wall when no longer hovering. This function is registered
    as handler for when the mouse pointer exits the wall.
    */
    {
        isHovered = false;
    }

    /* Helpers. */

    private bool noHoverEffect()
    /* If already at this view, or currently animating between views, don't highlight
    the background.
    */
    {
        bool noHover = (
            skillCreationCameraView.currentView == CameraView.Right
            || skillCreationCameraView.isBetweenViews()
        );
        return noHover;
    }

    private void setBackgroundAlpha(float alpha)
    /* Set the transparency of the background that covers the whole canvas (keeping the
    same RGB color it already was).
    
    :param float alpha: Between 0 for transparent and 1 for opaque.
    */
    {
        Color color = backgroundImage.color;
        color.a = alpha;
        backgroundImage.color = color;
    }
}
