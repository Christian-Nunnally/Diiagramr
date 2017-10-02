using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using ColorOrgan5.Annotations;

namespace Diiagramr.Model
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

        private static int StaticId { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnModelPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}