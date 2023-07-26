using UnityEngine;
using OutlineEffect;
using SharedCode.Utils;
using System.Reflection;
using SharedCode.Aspects.Item.Templates;

namespace Assets.Src.BuildingSystem
{
    public class VisualBehaviour : MonoBehaviour
    {
        public enum VisualState
        {
            Invisible = 0,
            Suitable = 1,
            Unsuitable = 2,
            UnderConstruction = 3,
            Constructed = 4,
            Destroyed = 5
        }

        private static OutlineColor GetOutlineColor(bool dataValid)
        {
            return dataValid ? OutlineColor.Blue : OutlineColor.Red;
        }

        public static void UpdateState(GameObject gameObject, VisualState state, OutlineColor color, string visualCommon, string visualVersion, bool force, float fracturedChunkScale)
        {
            var visualBehaviour = gameObject?.GetComponent<VisualBehaviour>();
            if (visualBehaviour != null)
            {
                visualBehaviour.SetVisualState(state, color, visualCommon, visualVersion, force, fracturedChunkScale);
            }
        }

        public static void UpdateVisualBehaviour(GameObject gameObject, VisualData data)
        {
            var visualBehaviour = gameObject?.GetComponent<VisualBehaviour>();
            if (visualBehaviour != null)
            {
                if ((data != null) && data.Visible)
                {
                    visualBehaviour.SetSelected(data.Selected, GetOutlineColor(data.Valid));
                    visualBehaviour.SetVisualState(data.GetVisualState(), GetOutlineColor(data.Valid), data.GetVisualCommonName(), data.GetVisualVersionName(), false, data.GetFracturedChunkScale() );
                }
                else
                {
                    visualBehaviour.SetSelected(false, 0);
                    visualBehaviour.SetVisualState(VisualState.Invisible, 0, string.Empty, string.Empty, false, 1.0f);
                }
            }
        }

        // realtime fields fields -----------------------------------------------------------------
        public VisualState State;
        public GameObject[] StateElements;

        public void Awake()
        {
            for (var index = 0; index < StateElements.Length; ++index)
            {
                if ((int)State == index)
                {
                    StateElements[index].SetActive(false);
                }
            }
        }

        private int activeState = -1;
        public void SetVisualState(VisualState _state, OutlineColor color, string visualCommon, string visualVersion, bool force, float fracturedChunkScale)
        {
            if (StateElements == null) { return; }
            if (force || (activeState != (int)(_state)))
            {
                activeState = (int)(_state);
                bool wasSelected = isSelected;
                if (wasSelected)
                {
                    SetSelected(false, 0);
                }
                for (int i = 0; i < StateElements.Length; i++) { StateElements[i].SetActive(false); }
                State = (VisualState)(activeState);
                StateElements[activeState].SetActive(true);
                if ( State == VisualState.Destroyed)
                {
                    var facturedObject = StateElements[activeState].GetComponentInChildren<FracturedObject>();
                    if (facturedObject != null)
                    {
                        facturedObject.InitDetachedChunks(fracturedChunkScale, UnityEngine.Vector3.zero);
                    }
                }
                else if ((State == VisualState.Constructed) && !string.IsNullOrEmpty(visualCommon) && !string.IsNullOrEmpty(visualVersion))
                {
                    for (int i = 0; i < StateElements[activeState].transform.childCount; i++)
                    {
                        var child = StateElements[activeState].transform.GetChild(i).gameObject;
                        if (child != null)
                        {
                            child.SetActive(child.name.Equals(visualCommon) || child.name.Equals(visualVersion));
                        }
                    }
                }
                if (wasSelected)
                {
                    SetSelected(true, color);
                }
            }
        }

        private bool isSelected = false;
        public void SetSelected(bool _isSelected, OutlineColor color)
        {
            if (isSelected != _isSelected)
            {
                isSelected = _isSelected;
                if (isSelected)
                {
                    //TODO PZ-7353, { turn off TargetHolder override
                    //if ((State == VisualState.Suitable) || (State == VisualState.Unsuitable))
                    //TODO PZ-7353, } turn off TargetHolder override
                    if (Constants.WorldConstants.EnableOutline)
                    {
                        Renderer[] renderers = gameObject.transform.GetComponentsInChildren<Renderer>(false);
                        if (renderers != null)
                        {
                            for (int i = 0; i < renderers.Length; ++i)
                            {
                                if (i == 0)
                                {
                                    if (renderers[i] != null)
                                    {
                                        var outline = renderers[i].transform.gameObject.GetComponent<Outline>();
                                        if (outline == null)
                                        {
                                            outline = renderers[i].transform.gameObject.AddComponent<Outline>();
                                        }
                                        else
                                        {
                                            BuildUtils.Message?.Report($"outline component already present", MethodBase.GetCurrentMethod().DeclaringType.Name);
                                        }
                                        if (outline != null)
                                        {
                                            outline.color = color;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    Outline[] outlines = gameObject.transform.GetComponentsInChildren<Outline>(false);
                    if (outlines != null)
                    {
                        for (int i = 0; i < outlines.Length; ++i)
                        {
                            Destroy(outlines[i]);
                        }
                    }
                }
            }
        }
    }
}
