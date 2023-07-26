using System;
using System.Linq;
using UnityEngine;
using UnityWeld.Binding.Exceptions;
using UnityWeld.Binding.Internal;

namespace UnityWeld.Binding
{
    /// <summary>
    /// Base class for binders to Unity MonoBehaviours.
    /// </summary>
    [HelpURL("https://github.com/Real-Serious-Games/Unity-Weld")]
    public abstract class AbstractMemberBinding : MonoBehaviour, IMemberBinding
    {
        public bool DontFindViewModelOnAwake;

        /// <summary>
        /// Initialise this binding. Used when we first start the scene.
        /// Detaches any attached view models, finds available view models afresh and then connects the binding.
        /// </summary>
        public virtual void Init()
        {
            Disconnect();

            Connect();
        }

        /// <summary>
        /// Scan up the hierarchy and find a view model that corresponds to the specified name.
        /// </summary>
        private object FindViewModel(string viewModelName)
        {
            var trans = transform;
            while (trans != null)
            {
                var components = trans.GetComponents<MonoBehaviour>(); // is this GetComponentsInParent?

                MonoBehaviour monoBehaviourViewModel = null;
                foreach (var component in components)
                    if (component.GetType().ToString() == viewModelName) // FIXME: почему ToString(), а не Name?
                    {
                        monoBehaviourViewModel = component;
                        break;
                    }

                if (monoBehaviourViewModel != null)
                {
                    return monoBehaviourViewModel;
                }

                IViewModelProvider providedViewModel = null;
                foreach (var component in components)
                    if (!ReferenceEquals(component, this) &&
                        component is IViewModelProvider provider
                        && provider.GetViewModelTypeName() == viewModelName)
                    {
                        providedViewModel = provider;
                        break;
                    }
                
                if (providedViewModel != null)
                {
                    return providedViewModel.GetViewModel();
                }

                trans = trans.parent;
            }

            throw new ViewModelNotFoundException(
                $"Tried to get view model {viewModelName} but it could not be found on object {gameObject.name}. " +
                "Check that a ViewModelBinding for that view model exists further up in the scene hierarchy. ");
        }

        /// <summary>
        /// Find the type of the adapter with the specified name and create it.
        /// </summary>
        protected static IAdapter CreateAdapter(string adapterTypeName)
        {
            if (string.IsNullOrEmpty(adapterTypeName))
            {
                return null;
            }

            var adapterType = TypeResolver.FindAdapterType(adapterTypeName);
            if (adapterType == null)
            {
                throw new NoSuchAdapterException($"<{adapterTypeName}>");
            }

            if (!typeof(IAdapter).IsAssignableFrom(adapterType))
            {
                throw new InvalidAdapterException(string.Format("Type '{0}' does not implement IAdapter and cannot be used as an adapter.",
                    adapterTypeName));
            }

            return (IAdapter) Activator.CreateInstance(adapterType);
        }

        /// <summary>
        /// Make a property end point for a property on the view model.
        /// </summary>
        protected PropertyEndPoint MakeViewModelEndPoint(string viewModelPropertyName, string adapterTypeName,
            AdapterOptions adapterOptions)
        {
            string propertyName;
            object viewModel;
            ParseViewModelEndPointReference(viewModelPropertyName, out propertyName, out viewModel);

            var adapter = CreateAdapter(adapterTypeName);

            return new PropertyEndPoint(viewModel, propertyName, adapter, adapterOptions, "view-model", this);
        }

        /// <summary>
        /// Parse an end-point reference including a type name and member name separated by a period.
        /// </summary>
        protected static bool ParseEndPointReference(string endPointReference, out string memberName, out string typeName)
        {
            memberName = null;
            typeName = null;
            var lastPeriodIndex = endPointReference.LastIndexOf('.');
            if (lastPeriodIndex == -1)
            {
                Debug.LogError("No period was found, expected end-point reference in the following format: <type-name>.<member-name>. " +
                               $"Provided end-point reference: '{endPointReference}'");
                return false;
            }

            typeName = endPointReference.Substring(0, lastPeriodIndex);
            memberName = endPointReference.Substring(lastPeriodIndex + 1);
            if (typeName.Length == 0 || memberName.Length == 0)
            {
                //throw new InvalidEndPointException(
                Debug.LogError("Bad format for end-point reference, expected the following format: <type-name>.<member-name>. " +
                               $"Provided end-point reference: '{endPointReference}'");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Parse an end-point reference and search up the hierarchy for the named view-model.
        /// </summary>
        protected void ParseViewModelEndPointReference(string endPointReference, out string memberName, out object viewModel)
        {
            if (!ParseEndPointReference(endPointReference, out memberName, out var viewModelName))
            {
                var msg = $"{nameof(ParseEndPointReference)}({nameof(endPointReference)}='{endPointReference}') is failed @'{name}'";
                Debug.LogError(msg, gameObject);
                throw new InvalidEndPointException(msg);
            }

            viewModel = FindViewModel(viewModelName);
            if (viewModel == null)
            {
                var msg = $"Failed to find view-model in hierarchy: '{viewModelName}' @'{name}'";
                Debug.LogError(msg, gameObject);
                throw new ViewModelNotFoundException(msg);
            }
        }

        /// <summary>
        /// Parse an end-point reference and get the component for the view.
        /// </summary>
        protected void ParseViewEndPointReference(string endPointReference, out string memberName, out Component view)
        {
            memberName = null;
            view = null;
            string boundComponentType;
            if (!ParseEndPointReference(endPointReference, out memberName, out boundComponentType))
            {
                var msg = $"{nameof(ParseEndPointReference)}({nameof(endPointReference)}='{endPointReference}') is failed";
                Debug.LogError(msg, gameObject);
                return;
            }

            view = GetComponent(boundComponentType);
            if (view == null)
            {
                var msg = $"Failed to find component on current game object: {boundComponentType}";
                Debug.LogError(msg, gameObject);
                throw new ComponentNotFoundException(msg);
            }
        }

        /// <summary>
        /// Connect to all the attached view models
        /// </summary>
        public abstract void Connect();

        /// <summary>
        /// Disconnect from all attached view models.
        /// </summary>
        public abstract void Disconnect();

        protected void Awake()
        {
            if (!DontFindViewModelOnAwake)
                Init();
        }

        /// <summary>
        /// Clean up when the game object is destroyed.
        /// </summary>
        public void OnDestroy()
        {
            Disconnect();
        }
    }
}