using PropertyChanged;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace DiiagramrAPI.Diagram.Model
{
    [DataContract]
    [AddINotifyPropertyChangedInterface]
    public class ProjectModel : ModelBase
    {
        public ProjectModel()
        {
            Diagrams = new ObservableCollection<DiagramModel>();
            Name = "NewProject";
        }

        [DataMember]
        public virtual ObservableCollection<DiagramModel> Diagrams { get; set; }

        public bool IsDirty { get; set; }

        public virtual void AddDiagram(DiagramModel diagram)
        {
            if (Diagrams.Contains(diagram))
            {
                return;
            }

            Diagrams.Add(diagram);
            ProjectChanged();
        }

        public void ProjectChanged()
        {
            IsDirty = true;
        }

        public virtual void RemoveDiagram(DiagramModel diagram)
        {
            if (!Diagrams.Contains(diagram))
            {
                return;
            }

            Diagrams.Remove(diagram);
            ProjectChanged();
        }

        protected override void NotifyPropertyChanged(string propertyName = null)
        {
            base.NotifyPropertyChanged(propertyName);
            if (propertyName.Equals(nameof(Name)))
            {
                ProjectChanged();
            }
        }
    }
}
