using DiiagramrModel;
using Stylet;
using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace DiiagramrAPI.Application
{
    /// <summary>
    /// Base class for all stylet based view models.
    /// </summary>
    public abstract class ViewModel<ModelType> : Screen
        where ModelType : ModelBase
    {
        private readonly List<Action> _viewLoadedActions = new List<Action>();

        /// <summary>
        /// The model this view model represents.
        /// </summary>
        public ModelType Model { get; set; }

        /// <summary>
        /// Gets the current adorner on this view model.
        /// </summary>
        public Adorner Adorner { get; private set; }

        /// <summary>
        /// Gets or sets whether the view for this view model should be visible.
        /// </summary>
        public virtual bool Visible { get; set; } = true;

        /// <summary>
        /// Sets the current adorner on the view.
        /// </summary>
        /// <param name="adorner">The adorner to show.</param>
        public void SetAdorner(Adorner adorner)
        {
            RemoveExistingAdorners();
            Adorner = adorner;
            if (adorner != null)
            {
                AdornerLayer.GetAdornerLayer(View).Add(adorner);
            }
        }

        /// <summary>
        /// Executes an action immediately if the view is loaded. Otherwise executes the action when the view is loaded.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        public void ExecuteWhenViewLoaded(Action action)
        {
            if (View is object)
            {
                action();
            }
            else
            {
                _viewLoadedActions.Add(action);
            }
        }

        /// <inheritdoc/>
        protected override void OnViewLoaded()
        {
            base.OnViewLoaded();
            foreach (var action in _viewLoadedActions)
            {
                action();
            }
            _viewLoadedActions.Clear();
        }

        private void RemoveExistingAdorners()
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(View);
            if (adornerLayer != null)
            {
                Adorner[] toRemoveArray = adornerLayer.GetAdorners(View);
                if (toRemoveArray != null)
                {
                    for (int x = 0; x < toRemoveArray.Length; x++)
                    {
                        adornerLayer.Remove(toRemoveArray[x]);
                    }
                }
            }
        }
    }
}