using DiiagramrAPI.Service;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace DiiagramrAPI.Model
{
    [DataContract]
    [AddINotifyPropertyChangedInterface]
    public class ProjectModel : ModelBase
    {
        [DataMember]
        public virtual ObservableCollection<DiagramModel> Diagrams { get; set; }

        public bool IsDirty { get; set; }

        public ProjectModel()
        {
            Diagrams = new ObservableCollection<DiagramModel>();
            Name = "NewProject";
        }

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

        private void TriggerProjectChangeWhenDiagramChanges(DiagramModel diagram)
        {
            diagram.PresentationChanged += ProjectChanged;
            diagram.SemanticsChanged += ProjectChanged;
        }

        private void RemoveTriggerProjectChangeWhenDiagramChanges(DiagramModel diagram)
        {
            diagram.PresentationChanged -= ProjectChanged;
            diagram.SemanticsChanged -= ProjectChanged;
        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            Diagrams.ForEach(TriggerProjectChangeWhenDiagramChanges);
        }

        protected override void OnModelPropertyChanged(string propertyName = null)
        {
            base.OnModelPropertyChanged(propertyName);
            if (propertyName.Equals(nameof(Name)))
            {
                ProjectChanged();
            }
        }

        public void ProjectChanged()
        {
            IsDirty = true;
        }
    }
}
