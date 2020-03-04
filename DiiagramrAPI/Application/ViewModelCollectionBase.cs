using DiiagramrModel;
using Stylet;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace DiiagramrAPI.Application
{
    /// <summary>
    /// Base class for <see cref="ViewModelCollection{TViewModel, TModel}"/> to isolate the messy boilerplate <see cref="IObservableCollection<TViewModel>"/> implementation.
    /// </summary>
    /// <typeparam name="TViewModel">
    /// The type of <see cref="ViewModel"/> that represents the <typeparamref name="TModel"/>.
    /// </typeparam>
    /// <typeparam name="TModel">
    /// The <see cref="ModelBase"/> type that this <see cref="ViewModelCollection{TViewModel, TModel}"/>
    /// will create instances of <typeparamref name="TViewModel"/> for.
    /// </typeparam>
    public abstract class ViewModelCollectionBase<TViewModel, TModel> : IObservableCollection<TViewModel>
        where TViewModel : ViewModel
        where TModel : ModelBase
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public abstract event NotifyCollectionChangedEventHandler CollectionChanged;

        public IObservableCollection<TViewModel> ViewModels { get; set; } = new BindableCollection<TViewModel>();

        public int Count => ViewModels.Count;

        public bool IsReadOnly => ViewModels.IsReadOnly;

        public TViewModel this[int index] { get => ViewModels[index]; set => ViewModels[index] = value; }

        public void Add(TViewModel item) => ViewModels.Add(item);

        public void AddRange(IEnumerable<TViewModel> items) => ViewModels.AddRange(items);

        public void Clear() => ViewModels.Clear();

        public bool Contains(TViewModel item) => ViewModels.Contains(item);

        public void CopyTo(TViewModel[] array, int arrayIndex) => ViewModels.CopyTo(array, arrayIndex);

        public IEnumerator<TViewModel> GetEnumerator() => ViewModels.GetEnumerator();

        public int IndexOf(TViewModel item) => ViewModels.IndexOf(item);

        public void Insert(int index, TViewModel item) => ViewModels.Insert(index, item);

        public bool Remove(TViewModel item) => ViewModels.Remove(item);

        public void RemoveAt(int index) => ViewModels.RemoveAt(index);

        public void RemoveRange(IEnumerable<TViewModel> items) => ViewModels.RemoveRange(items);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}