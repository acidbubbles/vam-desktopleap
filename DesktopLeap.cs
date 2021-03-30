using System;
using Leap.Unity;
using UnityEngine;

public class DesktopLeap : MVRScript
{
    private readonly JSONStorableBool _leftHandDetectedJSON = new JSONStorableBool("Left hand detected", false) {isStorable = false};
    private readonly JSONStorableBool _rightHandDetectedJSON = new JSONStorableBool("Right hand detected", false) {isStorable = false};
    private readonly JSONStorableFloat _handsOffsetX = new JSONStorableFloat("Offset X", 0f, -1f, 1f) {isStorable = true};
    private readonly JSONStorableFloat _handsOffsetY = new JSONStorableFloat("Offset Y", -0.2f, -1f, 1f) {isStorable = true};
    private readonly JSONStorableFloat _handsOffsetZ = new JSONStorableFloat("Offset Z", 0.5f, 0f, 2f) {isStorable = true};
    private readonly JSONStorableFloat _handsRotateX = new JSONStorableFloat("Rotate X", -20f, -180f, 180f) {isStorable = true};
    private readonly JSONStorableFloat _handsRotateY = new JSONStorableFloat("Rotate Y", 0f, -180f, 180f) {isStorable = true};
    private readonly JSONStorableFloat _handsRotateZ = new JSONStorableFloat("Rotate Z", 0f, -180f, 180f) {isStorable = true};
    private GameObject _handsRig;
    private GameObject _handsContainer;
    private LeapServiceProvider _provider;
    private Transform _originalLeftHand;
    private Transform _originalLeftHandParent;
    private Transform _originalRightHand;
    private Transform _originalRightHandParent;
    private GameObject _handDisableFakeTargetLeft;
    private GameObject _handDisableFakeTargetRight;

    public override void Init()
    {
        try
        {
            CreateToggle(_leftHandDetectedJSON, false).toggle.interactable = false;
            CreateToggle(_rightHandDetectedJSON, true).toggle.interactable = false;

            _handsOffsetX.setCallbackFunction = SyncHandsContainer;
            CreateSlider(_handsOffsetX, false);
            RegisterFloat(_handsOffsetX);
            _handsOffsetY.setCallbackFunction = SyncHandsContainer;
            CreateSlider(_handsOffsetY, false);
            RegisterFloat(_handsOffsetY);
            _handsOffsetZ.setCallbackFunction = SyncHandsContainer;
            CreateSlider(_handsOffsetZ, false);
            RegisterFloat(_handsOffsetZ);

            _handsRotateX.setCallbackFunction = SyncHandsContainer;
            CreateSlider(_handsRotateX, true);
            RegisterFloat(_handsRotateX);
            _handsRotateY.setCallbackFunction = SyncHandsContainer;
            CreateSlider(_handsRotateY, true);
            RegisterFloat(_handsRotateY);
            _handsRotateZ.setCallbackFunction = SyncHandsContainer;
            CreateSlider(_handsRotateZ, true);
            RegisterFloat(_handsRotateZ);

            SyncHandsContainer(0);
        }
        catch (Exception e)
        {
            SuperController.LogError($"{nameof(DesktopLeap)}.{nameof(Init)}: {e}");
        }
    }

    private void SyncHandsContainer(float val)
    {
        if (_handsRig == null) return;
        _handsRig.transform.localPosition = new Vector3(
            _handsOffsetX.val,
            _handsOffsetY.val,
            _handsOffsetZ.val
        );
        _handsContainer.transform.localEulerAngles = new Vector3(
            _handsRotateX.val,
            _handsRotateY.val,
            _handsRotateZ.val
        );
    }

    public void Update()
    {
        _leftHandDetectedJSON.val = SuperController.singleton.leapHandLeft.gameObject.activeInHierarchy;
        _rightHandDetectedJSON.val = SuperController.singleton.leapHandRight.gameObject.activeInHierarchy;
    }

    public void OnEnable()
    {
        try
        {
            if (!SuperController.singleton.IsMonitorOnly)
            {
                enabled = false;
                return;
            }

            var centerCamera = SuperController.singleton.MonitorCenterCamera;
            if (centerCamera == null) throw new NullReferenceException(nameof(centerCamera));
            var handModelManager = SuperController.singleton.leapHandModelControl.GetComponent<HandModelManager>();
            if (handModelManager == null) throw new NullReferenceException(nameof(handModelManager));

            _handsRig = new GameObject("DesktopLeapHandsRig");
            _handsRig.transform.SetParent(centerCamera.transform, false);
            _handsRig.transform.localPosition = new Vector3(_handsOffsetX.val, _handsOffsetY.val, _handsOffsetZ.val);

            _handsContainer = new GameObject("DesktopLeapHandsContainer");
            _handsContainer.transform.SetParent(_handsRig.transform, false);
            _handsContainer.transform.localEulerAngles = new Vector3(_handsRotateX.val, _handsRotateY.val, _handsRotateZ.val);

            _provider = _handsContainer.AddComponent<LeapServiceProvider>();
            handModelManager.leapProvider = _provider;

            _originalLeftHand = SuperController.singleton.leftHand;
            _originalLeftHandParent = _originalLeftHand.parent;
            _originalRightHand = SuperController.singleton.rightHand;
            _originalRightHandParent = _originalLeftHand.parent;
            _handDisableFakeTargetLeft = new GameObject("DesktopLeapFakeLeftHand");
            _handDisableFakeTargetLeft.transform.SetParent(_originalLeftHand, false);
            SuperController.singleton.leftHand = _handDisableFakeTargetLeft.transform;
            _handDisableFakeTargetRight = new GameObject("DesktopLeapFakeRightHand");
            _handDisableFakeTargetRight.transform.SetParent(_originalRightHand, false);
            SuperController.singleton.rightHand = _handDisableFakeTargetRight.transform;
            _originalLeftHand.gameObject.SetActive(true);
            _originalRightHand.gameObject.SetActive(true);
            _originalLeftHand.SetParent(SuperController.singleton.leapHandMountLeft, false);
            _originalRightHand.SetParent(SuperController.singleton.leapHandMountRight, false);

            SuperController.singleton.transform.parent.BroadcastMessage("DevToolsGameObjectExplorerShow", _originalLeftHand.transform.gameObject);
        }
        catch (Exception e)
        {
            SuperController.LogError($"{nameof(DesktopLeap)}.{nameof(OnEnable)}: {e}");
        }
    }

    public void OnDisable()
    {
        try
        {
            var handModelManager = SuperController.singleton.leapHandModelControl.GetComponent<HandModelManager>();
            if (handModelManager != null)
            {
                if(handModelManager.leapProvider == _provider)
                    handModelManager.leapProvider = null;
            }
            _provider = null;
            DestroyImmediate(_handsRig);
            _handsRig = null;
            _handsContainer = null;

            if (_originalLeftHand != null)
            {
                _originalLeftHand.SetParent(_originalLeftHandParent, false);
                SuperController.singleton.leftHand = _originalLeftHand;
                _originalLeftHand = null;
                _originalLeftHandParent = null;
            }
            Destroy(_handDisableFakeTargetLeft);
            _handDisableFakeTargetLeft = null;

            if (_originalRightHand != null)
            {
                _originalRightHand.SetParent(_originalRightHandParent, false);
                SuperController.singleton.rightHand = _originalRightHand;
                _originalRightHand = null;
                _originalRightHandParent = null;
            }
            Destroy(_handDisableFakeTargetRight);
            _handDisableFakeTargetRight = null;
        }
        catch (Exception e)
        {
            SuperController.LogError($"{nameof(DesktopLeap)}.{nameof(OnDisable)}: {e}");
        }
    }
}
