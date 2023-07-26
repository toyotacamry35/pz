using UnityEngine;
using Assets.Src.Shared;
using Assets.Src.Lib.Cheats;
using Assets.Src.Tools;
using Core.Cheats;

public class SetTarget : MonoBehaviour
{
    public TargetHolder targetHolder;
    public static Vector2 deltaPosition = new Vector2(0.5f, 0.5f);
    Camera _camera;
    const float DISTANCE_RAYCAST = 5f;


    private void Start()
    {
        _camera = GetComponent<Camera>();
    }

    private void Update()
    {
        if (targetHolder == null || _camera == null)
        {
            return;
        }

        RaycastHit hit;
        bool tr = Physics.queriesHitTriggers;
        Physics.queriesHitTriggers = true;
        //Ray ray = new Ray(transform.position, transform.forward);
        Ray ray = _camera.ScreenPointToRay(new Vector3(Screen.width * deltaPosition.x, Screen.height * deltaPosition.y, 0));
        if (Physics.Raycast(ray, out hit, PhysicsChecker.CheckDistance(DISTANCE_RAYCAST, "SetTarget"), PhysicsLayers.InteractiveAndDestrMask))
        {
            targetHolder.CurrentTarget.Value = hit.transform.gameObject;
        }
        else
        {
            targetHolder.CurrentTarget.Value = null;
        }
        Physics.queriesHitTriggers = tr;
    }

    [Cheat]
    public static void DeltaTargetRay(float x, float y)
    {
        deltaPosition = new Vector2(x, y);
    }
}