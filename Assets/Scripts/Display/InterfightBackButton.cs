using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InterfightBackButton : MonoBehaviour, IPointerClickHandler
{
    InterfightCameraView interfightCameraView;

    void Awake()
    {
        interfightCameraView =
            GameObject.Find("GeneralScripts").GetComponent<InterfightCameraView>();
    }

    public void OnPointerClick(PointerEventData ped)
    {
        interfightCameraView.setView(CameraView.Start);
    }
}
