using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillButton : MonoBehaviour
{
    /* References. */
    public List<Sprite> iconOptions; // populated in the Inspector

    /* PUBLIC API. */

    public Sprite getIconByName(string iconName)
    /* Given a string name of an icon, get the Texture2D object for that icon from the
    list of options stored on this prefab.

    :param string iconName: the name of the Texture2D as displayed in the Inspector,
        like "icon_14"
    
    :return Texture2D icon:
    */
    {
        foreach (Sprite icon in iconOptions)
        {
            if (icon.name == iconName)
            {
                return icon;
            }
        }
        // Return null if no icon by that name was found.
        return null;
    }

    // TODO: write a function to do something when used (basic damage, to start)
    // TODO: make clickable to use
    // TODO: add cooldown logic and display
}
