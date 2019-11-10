using DiiagramrModel;
using Stylet;
using System.Windows.Documents;

namespace DiiagramrAPI.Application
{
    public abstract class ViewModel : Screen
    {
        public ModelBase Model { get; set; }

        public Adorner Adorner { get; private set; }

        public virtual bool Visible { get; set; } = true;

        public void SetAdorner(Adorner adorner)
        {
            RemoveAllAdornersFromTerminal();
            Adorner = adorner;
            if (adorner != null)
            {
                AdornerLayer.GetAdornerLayer(View).Add(adorner);
            }
        }

        private void RemoveAllAdornersFromTerminal()
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