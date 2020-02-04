using DiiagramrModel;
using Stylet;
using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace DiiagramrAPI.Application
{
    public abstract class ViewModel : Screen
    {
        private readonly List<Action> _viewLoadedActions = new List<Action>();

        public ModelBase Model { get; set; }

        public Adorner Adorner { get; private set; }

        public virtual bool Visible { get; set; } = true;

        public void SetAdorner(Adorner adorner)
        {
            RemoveExistingAdorners();
            Adorner = adorner;
            if (adorner != null)
            {
                AdornerLayer.GetAdornerLayer(View).Add(adorner);
            }
        }

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