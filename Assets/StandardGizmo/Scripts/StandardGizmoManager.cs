using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GizmoShowSetting {
    public bool movement;
    public bool rotation;
}

public class StandardGizmoManager : SingletonMonoBehaviour<StandardGizmoManager> {

    #region Common Settings
    public Transform HostTransform;

    [Tooltip("ドラッグの有効/無効")]
    public bool IsDraggingEnabled = true;
    [Tooltip("True: 親オブジェクトに合わせてサイズを変更、False: 固定サイズ")]
    public bool IsStaticScale = false;
    #endregion

    #region Movement Settings
    [Tooltip("矢印 Prefab")]
    public GameObject mover;
    [Range(0.01f, 5.0f)]
    [Tooltip("移動量")]
    public float movementLerpSpeed = .2f;
    [Tooltip("矢印の大きさ")]
    public float arrowScale = 1f;
    [Tooltip("ホバー時の大きさ比率")]
    public float movHoverScale = 1.5f;
    [Tooltip("負方向にも軸を伸ばす")]
    public bool useNegativeAxis = false;
    [Tooltip("ドラッグ時のカラー")]
    public Color movDraggingColor = new Color(1, 155f / 255f, 0);
    public Color xColor = Color.red;
    public Color yColor = Color.green;
    public Color zColor = Color.blue;

    private GameObject xArrow;
    private GameObject yArrow;
    private GameObject zArrow;

    private GameObject negativexArrow;
    private GameObject negativeyArrow;
    private GameObject negativezArrow;

    [HideInInspector]
    public bool IsDraggingMover = false;

    #endregion

    #region Rotation Settings
    [Tooltip("リング Prefab")]
    public GameObject rotator;
    [Range(0.01f, 5.0f)]
    [Tooltip("回転量")]
    public float rotationLerpSpeed = .2f;
    [Tooltip("リングの幅")]
    public float ringWidth = 1f;
    [Tooltip("リングの直径")]
    public float ringDiameter = 1f;
    [Tooltip("ホバー時の大きさホバー時の大きさ比率")]
    public float rotHoverScale = 1.5f;
    public Color rotDraggingColor = new Color(1,155f/255f,0);
    public Color rollColor        = Color.magenta;
    public Color yawColor         = Color.cyan;
    public Color pitchColor       = Color.yellow;

    private GameObject roll;
    private GameObject yaw;
    private GameObject pitch;
    
    [HideInInspector]
    public bool IsDraggingRotator = false;
    [HideInInspector]
    public Vector3 Center = Vector3.zero;
    #endregion

    Vector3 positionDiff = Vector3.zero;

    // Use this for initialization
    void Start () {
        CreateGizmos();
        DisableGizmos();
	}

    void CreateGizmos() {
        #region X Arrow
        xArrow = Instantiate(mover, transform, false);
        xArrow.name = "x-Axis";
        xArrow.transform.localRotation = Quaternion.Euler(0, 90, 0);
        xArrow.transform.localScale = arrowScale * Vector3.one;
        xArrow.GetComponent<MovementManipulator>().Init(MovementManipulator.AxisType.X, xColor, movHoverScale);
        #endregion

        #region Y Arrow
        yArrow = Instantiate(mover, transform, false);
        yArrow.name = "y-Axis";
        yArrow.transform.localRotation = Quaternion.Euler(-90, 0, 0);
        yArrow.transform.localScale = arrowScale * Vector3.one;
        yArrow.GetComponent<MovementManipulator>().Init(MovementManipulator.AxisType.Y, yColor, movHoverScale);
        #endregion

        #region Z Arrow
        zArrow = Instantiate(mover, transform, false);
        zArrow.name = "z-Axis";
        zArrow.transform.localRotation = Quaternion.Euler(0, 0, 0);
        zArrow.transform.localScale = arrowScale * Vector3.one;
        zArrow.GetComponent<MovementManipulator>().Init(MovementManipulator.AxisType.Z, zColor, movHoverScale);
        #endregion

        #region Negative X
        negativexArrow = Instantiate(xArrow, transform);
        negativexArrow.name = "Negative x-Arrow";
        negativexArrow.transform.localRotation = Quaternion.Euler(0, -90, 0);
        negativexArrow.GetComponent<MovementManipulator>().Init(MovementManipulator.AxisType.X, xColor, movHoverScale);
        #endregion

        #region Negative Y
        negativeyArrow = Instantiate(yArrow, transform);
        negativeyArrow.name = "Negative y-Arrow";
        negativeyArrow.transform.localRotation = Quaternion.Euler(90, 0, 0);
        negativeyArrow.GetComponent<MovementManipulator>().Init(MovementManipulator.AxisType.Y, yColor, movHoverScale);
        #endregion

        #region Negative Z
        negativezArrow = Instantiate(zArrow, transform);
        negativezArrow.name = "Negative z-Arrow";
        negativezArrow.transform.localRotation = Quaternion.Euler(0, 180, 0);
        negativezArrow.GetComponent<MovementManipulator>().Init(MovementManipulator.AxisType.Z, zColor, movHoverScale);
        #endregion

        #region Roll
        roll = Instantiate(rotator, transform, false);
        roll.name = "Roll";
        roll.transform.localRotation = Quaternion.Euler(0, 90, 0);
        roll.transform.localScale = new Vector3(ringDiameter - .05f, ringDiameter - .05f, ringWidth);
        roll.GetComponent<RotationManipulator>().Init(RotationManipulator.RotationType.Roll, rollColor, rotHoverScale);
        #endregion

        #region Yaw
        yaw = Instantiate(rotator, transform, false);
        yaw.name = "Yaw";
        yaw.transform.localRotation = Quaternion.Euler(90, 0, 0);
        yaw.transform.localScale = new Vector3(ringDiameter - .01f, ringDiameter - .01f, ringWidth);
        yaw.GetComponent<RotationManipulator>().Init(RotationManipulator.RotationType.Yaw, yawColor, rotHoverScale);
        #endregion

        #region Pitch
        pitch = Instantiate(rotator, transform, false);
        pitch.name = "Pitch";
        pitch.transform.localRotation = Quaternion.Euler(0, 0, 0);
        pitch.transform.localScale = new Vector3(ringDiameter + .05f, ringDiameter + .05f, ringWidth);
        pitch.GetComponent<RotationManipulator>().Init(RotationManipulator.RotationType.Pitch, pitchColor, rotHoverScale);
        #endregion
    }

    void Update () {
        if(HostTransform == null) { return; }

        var bounds = BoundsHelper.GetBounds(HostTransform);
        var scaler = 1f;
        
        if (!IsStaticScale) {
            var hostScale = bounds.size;
            scaler = hostScale.x;
            if (scaler < hostScale.y) scaler = hostScale.y;
            if (scaler < hostScale.z) scaler = hostScale.z;
        }

        transform.position   = HostTransform.position + positionDiff;
        transform.localScale = scaler * Vector3.one;
	}

    public void UpdateTarget(Transform hostTransform, GizmoShowSetting gizmoSetting) {

        if(HostTransform == hostTransform) {
            DisableGizmos();
            HostTransform = null;
            return;
        }

        //TODO FIX
        HostTransform = hostTransform;
        var rot = hostTransform.rotation;
        HostTransform.rotation = Quaternion.identity;
        var bounds = BoundsHelper.GetBounds(HostTransform);
        HostTransform.rotation = rot;
        transform.rotation = rot;
        Center = bounds.center;
        positionDiff = Center - HostTransform.transform.position;

        xArrow.SetActive(gizmoSetting.movement);
        yArrow.SetActive(gizmoSetting.movement);
        zArrow.SetActive(gizmoSetting.movement);
        if (useNegativeAxis) {
            negativexArrow.SetActive(gizmoSetting.movement);
            negativeyArrow.SetActive(gizmoSetting.movement);
            negativezArrow.SetActive(gizmoSetting.movement);
        }

        roll.SetActive(gizmoSetting.rotation);
        yaw.SetActive(gizmoSetting.rotation);
        pitch.SetActive(gizmoSetting.rotation);
    }

    private void DisableGizmos() {
        xArrow.SetActive(false);
        yArrow.SetActive(false);
        zArrow.SetActive(false);
        negativexArrow.SetActive(false);
        negativeyArrow.SetActive(false);
        negativezArrow.SetActive(false);
        roll.SetActive(false);
        yaw.SetActive(false);
        pitch.SetActive(false);
    }
}
