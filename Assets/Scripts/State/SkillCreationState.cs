using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCreationState : MonoBehaviour
{
    /* State. */
    private int editedSkillId;
    private int selectedComponentId;
    private string editedName;
    private Sprite editedImage;

    void Start()
    {

    }

    void Update()
    {

    }

    /* PUBLIC API. */

    public void setEditedSkill(int skillId)
    {
        editedSkillId = skillId;
    }

    public void setSelectedComponent(int componentId)
    {
        selectedComponentId = componentId;
    }

    public void setEditedName(string name)
    {
        editedName = name;
    }

    public void setEditedImage(Sprite image)
    {
        editedImage = image;
    }
}
