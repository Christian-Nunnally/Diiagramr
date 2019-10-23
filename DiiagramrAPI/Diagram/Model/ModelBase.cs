using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace DiiagramrAPI.Diagram.Model
{
    [DataContract(IsReference = true)]
    public class ModelBase : INotifyPropertyChanged
    {
        private string _name;

        public ModelBase()
        {
            Id = StaticId++;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public virtual string Name
        {
            get => _name;

            set
            {
                if (_name != value)
                {
                    NotifyPropertyChanged(nameof(Name));
                }

                _name = value;
            }
        }

        private static int StaticId { get; set; }

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ModelValidationException : InvalidOperationException
    {
        public ModelValidationException(ModelBase model, string solutionRecomendation)
            : base($"{model.ToString()} - {solutionRecomendation}") { }
    }
}
