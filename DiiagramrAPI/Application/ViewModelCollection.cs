using DiiagramrCore;
using DiiagramrModel;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace DiiagramrAPI.Application
{
    /// <summary>
    /// Tracks a collection of <see cref="ModelBase"/> objects and maintains a synchronized collection of
    /// <see cref="ViewModel"/> objects that wrap each <see cref="ModelBase"/> in the tracked collection.
    /// </summary>
    /// <typeparam name="TViewModel">
    /// The type of <see cref="ViewModel"/> that represents the <typeparamref name="TModel"/>.
    /// </typeparam>
    /// <typeparam name="TModel">
    /// The <see cref="ModelBase"/> type that this <see cref="ViewModelCollection{TViewModel, TModel}"/>
    /// will create instances of <typeparamref name="TViewModel"/> for.
    /// </typeparam>
    public class ViewModelCollection<TViewModel, TModel> : ViewModelCollectionBase<TViewModel, TModel>
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
            ViewModels.CollectionChanged += ViewModelsCollectionChanged;
            collectionOwner.PropertyChanged += CollectionOwnerPropertyChanged;
            _modelCollectionGetter = modelCollectionGetter;
            _viewModelFactory = viewModelFactory;
            UpdateModels();
        }

        public override event NotifyCollectionChangedEventHandler CollectionChanged;

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

        private void ViewModelsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(sender, e);
        }

        private void UpdateModels()
        {
            Models = _modelCollectionGetter();
        }

        private void ModelCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var oldItem in e.OldItems.Cast<TModel>())
                {
                    var firstItem = ViewModels.FirstOrDefault(d => d.Model == oldItem);
                    ViewModels.Remove(firstItem);
                }
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
                UpdateModels();
            }
        }
    }
}