using Assets.Src.Camera;

public class MainCamera : CameraNotifier<MainCamera>
{
    public static void InvokeCameraCreatedIfNeeded()
    {
        if (Camera != null)
            InvokeCameraCreated();
    }
}
