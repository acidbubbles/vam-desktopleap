using System;
using Leap.Unity;

public class DesktopLeap : MVRScript
{
    public override void Init()
    {
        try
        {
        }
        catch (Exception e)
        {
            SuperController.LogError($"{nameof(DesktopLeap)}.{nameof(Init)}: {e}");
        }
    }

    public void OnEnable()
    {
        try
        {
            if (SuperController.singleton.isOVR || SuperController.singleton.isOpenVR)
            {
                enabled = false;
                return;
            }

            var handModelManager = SuperController.singleton.leapHandModelControl.GetComponent<HandModelManager>();
            var centerCamera = SuperController.singleton.MonitorCenterCamera.gameObject;
            var provider = centerCamera.AddComponent<LeapServiceProvider>();
            handModelManager.leapProvider = provider;

            SuperController.singleton.LeapRig.gameObject.SetActive(true);
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
            var centerCamera = SuperController.singleton.MonitorCenterCamera.gameObject;
            DestroyImmediate(centerCamera.GetComponent<LeapServiceProvider>());
        }
        catch (Exception e)
        {
            SuperController.LogError($"{nameof(DesktopLeap)}.{nameof(OnDisable)}: {e}");
        }
    }
}
