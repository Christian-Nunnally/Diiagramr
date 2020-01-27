using DiiagramrCore;
using DiiagramrModel;
using Stylet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace DiiagramrAPI.Application
{
    // NOTE: Currently not really used. Just experimenting.
    public class ViewModelCollection<TViewModel, TModel> : IObservableCollection<TViewModel>
        where TViewModel : ViewModel
        where TModel : ModelBase
    {
        private readonly Func<ObservableCollection<TModel>> _modelCollectionGetter;
        private readonly Func<TModel, TViewModel> _viewModelFactory;
        private ObservableCollection<TModel> _models;

        public ViewModelCollection(
            INotifyPropertyChanged collectionOwner,
            Func<ObservableCollection<TModel>> modelCollectionGetter,
            Func<TModel, TViewModel> viewModelFactory)
        {
            collectionOwner.PropertyChanged += CollectionOwnerPropertyChanged;
            _modelCollectionGetter = modelCollectionGetter;
            _viewModelFactory = viewModelFactory;

            Models = _modelCollectionGetter.Invoke();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public IObservableCollection<TViewModel> ViewModels { get; } = new BindableCollection<TViewModel>();

        public int Count => ViewModels.Count;

        public bool IsReadOnly => ViewModels.IsReadOnly;

        public ObservableCollection<TModel> Models
        {
            get => _models;
            set
            {
                ViewModels.Clear();
                _models.RunIfNotNull(() => _models.CollectionChanged -= ModelCollectionChanged);
                _models = value;
                _models.RunIfNotNull(() => _models.CollectionChanged += ModelCollectionChanged);
                _models.RunIfNotNull(() => ViewModels.AddRange(_models.Select(_viewModelFactory)));
            }
        }

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

        private void ModelCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                ViewModels.RemoveRange(ViewModels.Where(x => e.OldItems.Contains(x.Model)).ToArray());
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                ViewModels.AddRange(e.NewItems.Cast<TModel>().Select(_viewModelFactory));
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                ViewModels.Clear();
            }
        }

        private void CollectionOwnerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.Model))
            {
                Models = _modelCollectionGetter.Invoke();
            }
        }
    }
}