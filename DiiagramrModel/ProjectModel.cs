namespace DiiagramrModel
{
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;
    using PropertyChanged;

    /// <summary>
    /// A serializable representation of a project.
    /// </summary>
    [DataContract]
    [AddINotifyPropertyChangedInterface]
    public class ProjectModel : ModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectModel"/> class.
        /// </summary>
        public ProjectModel()
        {
            Name = "NewProject";
        }

        [DataMember]
        public virtual ObservableCollection<DiagramModel> Diagrams { get; } = new ObservableCollection<DiagramModel>();

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
