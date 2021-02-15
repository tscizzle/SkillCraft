using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SkillCreationCameraView : MonoBehaviour
{
    /* Constants. */
    private Dictionary<CameraView, (Vector3, Vector3)> viewMap =
        new Dictionary<CameraView, (Vector3, Vector3)>();
    private Vector3 rightWallViewPosition = new Vector3(1.25f, 1.7f, 0.9f);
    private Vector3 rightWallViewRotation = new Vector3(5, 0, 0);
    private Vector3 leftWallViewPosition = new Vector3(1.6f, 1.7f, 1.25f);
    private Vector3 leftWallViewRotation = new Vector3(5, -90, 0);

    /* Parameters. */
    public float duration;

    /* State */
    public CameraView currentView;
    public Tween currentTween;

    void Awake()
    {
        viewMap[CameraView.Start] = (
            Camera.main.transform.position, Camera.main.transform.rotation.eulerAngles
        );
        viewMap[CameraView.Right] = (rightWallViewPosition, rightWallViewRotation);
        viewMap[CameraView.Left] = (leftWallViewPosition, leftWallViewRotation);
    }

    /* PUBLIC API. */

    public void setView(CameraView view)
    /* Move to viewing a different part of the SkillCreation scene, including animating
    the camera to get there.
    */
    {
        // If currently animating, do nothing.
        if (currentTween != null)
            return;

        // If already at this view, do nothing.
        if (view == currentView)
            return;

        Sequence seq = DOTween.Sequence();
        (Vector3 nextPosition, Vector3 nextRotation) = viewMap[view];
        Tween positionTween = Camera.main.transform
            .DOMove(nextPosition, duration)
            .SetEase(Ease.OutQuart);
        Tween rotationTween = Camera.main.transform
            .DORotate(nextRotation, duration)
            .SetEase(Ease.OutQuart);
        seq.Insert(0, positionTween);
        seq.Insert(0, rotationTween);
        seq.OnComplete(() =>
        {
            currentTween = null;
            currentView = view;
        });
        currentTween = seq;
    }

    public bool isBetweenViews()
    /* If currently animating, we are between views. */
    {
        return currentTween != null;
    }
}

public enum CameraView { Start, Right, Left }
