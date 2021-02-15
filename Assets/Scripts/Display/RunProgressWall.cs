using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunProgressWall : MonoBehaviour
{
    /* References. */
    private InterfightCameraView skillCreationCameraView;

    void Awake()
    {
        skillCreationCameraView =
            GameObject.Find("GeneralScripts").GetComponent<InterfightCameraView>();
    }

    /* PUBLIC API. */

    public void OnClickRunProgressWall()
    /* Moves the camera to view the wall the run overview is on. This function is
    registered as handler for when the wall is clicked.
    */
    {
        CameraView nextView = skillCreationCameraView.currentView == CameraView.Left
            ? CameraView.Start
            : CameraView.Left;
        skillCreationCameraView.setView(nextView);
    }
}
