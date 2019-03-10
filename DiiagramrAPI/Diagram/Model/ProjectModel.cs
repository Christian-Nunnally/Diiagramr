using DiiagramrAPI.Service;
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
            TriggerProjectChangeWhenDiagramChanges(diagram);
        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            Diagrams.ForEach(TriggerProjectChangeWhenDiagramChanges);
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
            RemoveTriggerProjectChangeWhenDiagramChanges(diagram);
        }

        protected override void OnModelPropertyChanged(string propertyName = null)
        {
            base.OnModelPropertyChanged(propertyName);
            if (propertyName.Equals(nameof(Name)))
            {
                ProjectChanged();
            }
        }

        private void RemoveTriggerProjectChangeWhenDiagramChanges(DiagramModel diagram)
        {
            diagram.PresentationChanged -= ProjectChanged;
            diagram.SemanticsChanged -= ProjectChanged;
        }

        private void TriggerProjectChangeWhenDiagramChanges(DiagramModel diagram)
        {
            diagram.PresentationChanged += ProjectChanged;
            diagram.SemanticsChanged += ProjectChanged;
        }
    }
}
