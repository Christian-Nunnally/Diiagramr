using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace DiiagramrAPI.Model
{
    [DataContract(IsReference = true)]
    public class ModelBase : INotifyPropertyChanged
    {
        public ModelBase()
        {
            Id = StaticId++;
        }

        [DataMember]
        public int Id { get; set; }

        private string _name;

        [DataMember]
        public virtual string Name
        {
            get => _name;

            set
            {
                if (_name != value)
                {
                    OnModelPropertyChanged(nameof(Name));
                }

                _name = value;
            }
        }

        private static int StaticId { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnModelPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
