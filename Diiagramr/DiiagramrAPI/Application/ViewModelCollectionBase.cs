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
        where TViewModel : ViewModel<TModel>
        where TModel : ModelBase
    {
        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc/>
        public abstract event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// The collection of view models.
        /// </summary>
        public IObservableCollection<TViewModel> ViewModels { get; set; } = new BindableCollection<TViewModel>();

        /// <inheritdoc/>
        public int Count => ViewModels.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => ViewModels.IsReadOnly;

        /// <inheritdoc/>
        public TViewModel this[int index] { get => ViewModels[index]; set => ViewModels[index] = value; }

        /// <inheritdoc/>
        public void Add(TViewModel item) => ViewModels.Add(item);

        /// <inheritdoc/>
        public void AddRange(IEnumerable<TViewModel> items) => ViewModels.AddRange(items);

        /// <inheritdoc/>
        public void Clear() => ViewModels.Clear();

        /// <inheritdoc/>
        public bool Contains(TViewModel item) => ViewModels.Contains(item);

        /// <inheritdoc/>
        public void CopyTo(TViewModel[] array, int arrayIndex) => ViewModels.CopyTo(array, arrayIndex);

        /// <inheritdoc/>
        public IEnumerator<TViewModel> GetEnumerator() => ViewModels.GetEnumerator();

        /// <inheritdoc/>
        public int IndexOf(TViewModel item) => ViewModels.IndexOf(item);

        /// <inheritdoc/>
        public void Insert(int index, TViewModel item) => ViewModels.Insert(index, item);

        /// <inheritdoc/>
        public bool Remove(TViewModel item) => ViewModels.Remove(item);

        /// <inheritdoc/>
        public void RemoveAt(int index) => ViewModels.RemoveAt(index);

        /// <inheritdoc/>
        public void RemoveRange(IEnumerable<TViewModel> items) => ViewModels.RemoveRange(items);

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}