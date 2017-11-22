using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;

public class MovementManipulator : MonoBehaviour, IFocusable, IInputHandler, ISourceStateHandler {

    public enum AxisType { X, Y, Z };
    public AxisType axisType;

    public event Action StartedDragging;
    public event Action StoppedDragging;


    #region Private Values
    private StandardGizmoManager gizmoSetting;
    private Camera mainCamera;
    private bool isDragging;
    private bool isGazed;

    Vector3 startHandPosition;

    private IInputSource currentInputSource = null;
    private uint currentInputSourceId;

    private Renderer rend;
    private Vector3 arrowScale;
    private float hoverScaleRatio = 1.5f;
    private Color defaultColor;

    #endregion


    void Start() {
        mainCamera = Camera.main;
        gizmoSetting = StandardGizmoManager.Instance;
        rend = GetComponent<Renderer>();
        arrowScale = transform.localScale;
    }
    void OnDestroy() {
        if (isDragging) { StoppedDragging(); }
        if (isGazed) { OnFocusExit(); }
    }

    void Update() {
        if (gizmoSetting.IsDraggingEnabled && isDragging) {
            UpdateDragging();
        }
    }

    public void StartDragging() {
        if (!gizmoSetting.IsDraggingEnabled) { return; }
        if (isDragging) { return; }

        InputManager.Instance.PushModalInputHandler(gameObject);
        isDragging = true;

        Vector3 targetPosition = gizmoSetting.HostTransform.position;
        currentInputSource.TryGetPosition(currentInputSourceId, out startHandPosition);
        
        rend.material.color = gizmoSetting.movDraggingColor;
        gizmoSetting.IsDraggingMover = true;
        StartedDragging.RaiseEvent();
    }

    void UpdateDragging() {

        transform.localScale = arrowScale * hoverScaleRatio;

        Vector3 newHandPosition;
        currentInputSource.TryGetPosition(currentInputSourceId, out newHandPosition);
        Vector3 moveVect = newHandPosition - startHandPosition;

        Vector3 movementAxisVect = Vector3.zero;
        switch (axisType) {
            case AxisType.X:
                movementAxisVect = gizmoSetting.HostTransform.right;
                break;
            case AxisType.Y:
                movementAxisVect = gizmoSetting.HostTransform.up;
                break;
            case AxisType.Z:
                movementAxisVect = gizmoSetting.HostTransform.forward;
                break;
        }

        float projectMoveVect = Vector3.Dot(moveVect, movementAxisVect) * gizmoSetting.movementLerpSpeed;

        if (axisType == AxisType.X) gizmoSetting.HostTransform.position += projectMoveVect * gizmoSetting.HostTransform.right;
        if (axisType == AxisType.Y) gizmoSetting.HostTransform.position += projectMoveVect * gizmoSetting.HostTransform.up;
        if (axisType == AxisType.Z) gizmoSetting.HostTransform.position += projectMoveVect * gizmoSetting.HostTransform.forward;

        
    }
    
    public void Init(AxisType axisType, Color axisColor, float hoverScaleRatio) {
        this.axisType = axisType;
        rend = GetComponent<Renderer>();
        rend.material.color = axisColor;
        defaultColor = axisColor;

        this.hoverScaleRatio = hoverScaleRatio;
    }

    public void StopDragging() {
        if (!isDragging) { return; }

        InputManager.Instance.PopModalInputHandler();

        isDragging = false;
        currentInputSource = null;
        rend.material.color = defaultColor;
        gizmoSetting.IsDraggingMover = false;
        StoppedDragging.RaiseEvent();
    }
    public void OnFocusEnter() {
        if (!gizmoSetting.IsDraggingEnabled) { return; }
        if (isGazed) { return; }
        isGazed = true;

        if (gizmoSetting.IsDraggingRotator || gizmoSetting.IsDraggingMover) return;
        transform.localScale = arrowScale * hoverScaleRatio;
    }
    public void OnFocusExit() {
        if (!gizmoSetting.IsDraggingEnabled) { return; }
        if (!isGazed) { return; }
        isGazed = false;

        transform.localScale = arrowScale;

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
        //Nothing to do
    }
    public void OnSourceLost(SourceStateEventData eventData) {
        if (currentInputSource != null && eventData.SourceId == currentInputSourceId) {
            StopDragging();
        }
    }
}
