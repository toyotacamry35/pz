using UnityEngine;
using UnityEngine.Assertions;

namespace UnityWeld.Binding
{
    /// <summary>
    /// Template for use in collection bindings.
    /// </summary>
    public interface ITemplate
    {
        /// <summary>
        /// Set the view model and initialise all binding objects down the hierarchy
        /// </summary>
        void InitChildBindings(object viewModel);
    }

    /// <summary>
    /// Template for use in collection bindings.
    /// </summary>
    [HelpURL("https://github.com/Real-Serious-Games/Unity-Weld")]
    public class Template : MonoBehaviour, IViewModelProvider, ITemplate
    {
        /// <inheritdoc />
        public object GetViewModel()
        {
            return viewModel;
        }

        /// <inheritdoc />
        public string GetViewModelTypeName()
        {
            return viewModelTypeName;
        }

        public string viewModelTypeName = string.Empty;

        /// <summary>
        /// Cached view-model object.
        /// </summary>
        private object viewModel;

        /// <inheritdoc />
        public void InitChildBindings(object viewModel)
        {
            Assert.IsNotNull(viewModel, "Cannot initialise child bindings with null view model.");

            // Set the bound view to the new view model.
            this.viewModel = viewModel;

            foreach (var binding in GetComponentsInChildren<AbstractMemberBinding>())
            {
                binding.Init();
            }
        }
    }
}