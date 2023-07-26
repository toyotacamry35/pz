using Core.Environment.Logging.Extension;
using NLog;
using ReactivePropsNs;
using Uins;
using UnityEngine;
using Logger = NLog.Logger;

public class TargetHolder : HasDisposablesMonoBehaviour
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    
    public delegate void TargetChangedDelegate(GameObject oldObject, GameObject newObject, bool hasAutority);

    public event TargetChangedDelegate TargetChanged;
    public bool HasAutority;

    public ReactiveProperty<GameObject> CurrentTarget = new ReactiveProperty<GameObject>();

    public IStream<(GameObject, GameObject)> PrevCurrentTarget;

    private void Awake()
    {
        CurrentTarget.Value = null;
        PrevCurrentTarget = CurrentTarget.PrevAndCurrent(D);
        PrevCurrentTarget.Action(D, (prev, curr) =>
        {
            if (Logger.IsTraceEnabled)  Logger.IfTrace()?.Message("Target changed | {@}",  new {NewTarget = curr?.transform.FullName(), OldTarget = prev?.transform.FullName()}).Write();
            TargetChanged?.Invoke(prev, curr, HasAutority);
        });
    }
}