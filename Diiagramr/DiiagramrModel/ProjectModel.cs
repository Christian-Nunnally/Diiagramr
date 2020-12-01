namespace DiiagramrModel
{
    using PropertyChanged;
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;

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

        /// <summary>
        /// The list of diagrams contained within this project.
        /// </summary>
        [DataMember]
        public virtual ObservableCollection<DiagramModel> Diagrams { get; set; } = new ObservableCollection<DiagramModel>();

        /// <summary>
        /// Gets or sets whether this project is considered 'dirty' and would lose information if it is closed without being saved.
        /// </summary>
        public bool IsDirty { get; set; }

        /// <summary>
        /// Adds a diagram to the project.
        /// </summary>
        /// <param name="diagram">The diagram to add.</param>
        public virtual void AddDiagram(DiagramModel diagram)
        {
            if (Diagrams.Contains(diagram))
            {
                return;
            }

            Diagrams.Add(diagram);
            ProjectChanged();
        }

        /// <summary>
        /// Notify this project that it has changed.
        /// </summary>
        public void ProjectChanged()
        {
            IsDirty = true;
        }

        /// <summary>
        /// Removes a diagram from the project.
        /// </summary>
        /// <param name="diagram">The diagram to remove.</param>
        public virtual void RemoveDiagram(DiagramModel diagram)
        {
            if (!Diagrams.Contains(diagram))
            {
                return;
            }

            Diagrams.Remove(diagram);
            ProjectChanged();
        }

        /// <inheritdoc/>
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