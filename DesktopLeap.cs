using System;

public class DesktopLeap : MVRScript
{
    public override void Init()
    {
        try
        {
            SuperController.LogMessage($"{nameof(DesktopLeap)} initialized");
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
            SuperController.LogMessage($"{nameof(DesktopLeap)} enabled");
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
            SuperController.LogMessage($"{nameof(DesktopLeap)} disabled");
        }
        catch (Exception e)
        {
            SuperController.LogError($"{nameof(DesktopLeap)}.{nameof(OnDisable)}: {e}");
        }
    }

    public void OnDestroy()
    {
        try
        {
            SuperController.LogMessage($"{nameof(DesktopLeap)} destroyed");
        }
        catch (Exception e)
        {
            SuperController.LogError($"{nameof(DesktopLeap)}.{nameof(OnDestroy)}: {e}");
        }
    }
}
