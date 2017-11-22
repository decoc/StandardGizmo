using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class StandardGizmoTarget : MonoBehaviour, IInputClickHandler {

    public Transform HostTransform;

    public bool ArrowMovement = true;
    public bool ArrowRotation = true;

    private void Start() {
        if(HostTransform == null) {
            HostTransform = this.transform;
        }
    }

    public void OnInputClicked(InputClickedEventData eventData) {
        var gizmoSetting = new GizmoShowSetting();
        gizmoSetting.movement = ArrowMovement;
        gizmoSetting.rotation = ArrowRotation;
        StandardGizmoManager.Instance.UpdateTarget(HostTransform, gizmoSetting);
    }
}
