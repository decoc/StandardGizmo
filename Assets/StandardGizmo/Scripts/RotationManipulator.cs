using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;

public class RotationManipulator : MonoBehaviour, IFocusable, IInputHandler, ISourceStateHandler {

	public enum RotationType { Roll, Yaw, Pitch };
	public RotationType rotationType;

	public event Action StartedDragging;
	public event Action StoppedDragging;

    #region Private Values
    private StandardGizmoManager gizmoSetting;
    private Camera mainCamera;
	private bool isDragging;
	private bool isGazed;

    private Vector3 startHandPosition;
    private Vector3 orthogonalRotationAxisVec;

    private IInputSource currentInputSource = null;
	private uint currentInputSourceId;

    private Renderer rend;
    private Vector3 ringScale;
    private float hoverScaleRatio = 1.5f;
    private Color defaultColor;

    private Vector3 center;

	#endregion
    

	void Start () {
        mainCamera = Camera.main;
        gizmoSetting = StandardGizmoManager.Instance;
        rend = GetComponent<Renderer>();
        ringScale = transform.localScale;
    }
    void OnDestroy() {
        if (isDragging) { StoppedDragging(); }
        if (isGazed)    { OnFocusExit(); }
    }
	
	void Update () {
	    if (gizmoSetting.IsDraggingEnabled && isDragging) {
            UpdateDragging();
        }	
	}

    public void StartDragging() {
        if (!gizmoSetting.IsDraggingEnabled) { return; }
        if (isDragging) { return; }

        InputManager.Instance.PushModalInputHandler(gameObject);
        isDragging = true;

        currentInputSource.TryGetPosition(currentInputSourceId, out startHandPosition);

        Vector3 rotationAxisVec;
        switch (rotationType) {
            case RotationType.Roll:
                rotationAxisVec = gizmoSetting.HostTransform.right;
                break;
            case RotationType.Yaw:
                rotationAxisVec = gizmoSetting.HostTransform.up;
                break;
            case RotationType.Pitch:
                rotationAxisVec = gizmoSetting.HostTransform.forward;
                break;
            default:
                return;               
        }

        Vector3 projectionVect = Vector3.ProjectOnPlane(rotationAxisVec, mainCamera.transform.forward);
        projectionVect.Normalize();
        orthogonalRotationAxisVec = Vector3.Cross(mainCamera.transform.forward, projectionVect);
        orthogonalRotationAxisVec.Normalize();
        rend.material.color = gizmoSetting.rotDraggingColor;
        gizmoSetting.IsDraggingRotator = true;
        StartedDragging.RaiseEvent();
    }

    void UpdateDragging() {

        transform.localScale = Vector3.Scale(ringScale, new Vector3(1, 1, hoverScaleRatio));

        Vector3 newHandPosition;
        currentInputSource.TryGetPosition(currentInputSourceId, out newHandPosition);
        Vector3 moveVect = newHandPosition - startHandPosition;

        Vector3 projectMoveVect = Vector3.Project(moveVect, orthogonalRotationAxisVec);

        float rotationVal = Vector3.Dot(projectMoveVect, orthogonalRotationAxisVec) * gizmoSetting.rotationLerpSpeed;

        

        if (rotationType == RotationType.Roll)  gizmoSetting.transform.RotateAround(gizmoSetting.transform.position, gizmoSetting.transform.right,   rotationVal);
        if (rotationType == RotationType.Yaw)   gizmoSetting.transform.RotateAround(gizmoSetting.transform.position, gizmoSetting.transform.up,      rotationVal);
        if (rotationType == RotationType.Pitch) gizmoSetting.transform.RotateAround(gizmoSetting.transform.position, gizmoSetting.transform.forward, rotationVal);

        gizmoSetting.HostTransform.transform.rotation = gizmoSetting.transform.rotation;
    }

    public void Init(RotationType rotationType, Color rotationColor, float hoverScaleRatio) {
        this.rotationType = rotationType;
        rend = GetComponent<Renderer>();
        rend.material.color = rotationColor;
        defaultColor = rotationColor;

        this.hoverScaleRatio = hoverScaleRatio;
    }

    public void StopDragging() {
        if (!isDragging) { return; }

        InputManager.Instance.PopModalInputHandler();

        isDragging = false;
        currentInputSource = null;
        rend.material.color = defaultColor;
        gizmoSetting.IsDraggingRotator = false;
        StoppedDragging.RaiseEvent();
    }

    public void OnFocusEnter() {
        if (!gizmoSetting.IsDraggingEnabled) { return; }
        if (isGazed) { return; }
        isGazed = true;

        if (gizmoSetting.IsDraggingRotator || gizmoSetting.IsDraggingMover) return;
        transform.localScale = Vector3.Scale(ringScale, new Vector3(1, 1, hoverScaleRatio));
    }

	public void OnFocusExit() {
        if (!gizmoSetting.IsDraggingEnabled) { return; }
        if (!isGazed) { return; }
        isGazed = false;

        transform.localScale = ringScale;
    }

	public void OnInputUp(InputEventData eventData) {
        if (currentInputSource != null && eventData.SourceId == currentInputSourceId) {
            StopDragging();
        }
    }

	public void OnInputDown(InputEventData eventData) {
        if (isDragging) { return; }
        if (!eventData.InputSource.SupportsInputInfo(eventData.SourceId, SupportedInputInfo.Position)) { return; }

        currentInputSource = eventData.InputSource;
        currentInputSourceId = eventData.SourceId;
        StartDragging();
    }

	public void OnSourceDetected(SourceStateEventData eventData) {
            //Do nothing
    }

	public void OnSourceLost(SourceStateEventData eventData) {
        if (currentInputSource != null && eventData.SourceId == currentInputSourceId) {
            StopDragging();
        }
    }
}
