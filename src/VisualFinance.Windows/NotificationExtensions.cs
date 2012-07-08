using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Disposables;

//TODO: Allow the ability to provide multiple properties to WhenPropertyChanges
//TODO: Allow the ability to filter which properties notify on change for WhenCollectionItemsChange (ie as above todo).
namespace VisualFinance.Windows
{
    public static class NotificationExtensions
    {
        /// <summary>
        /// Exectutes an action when <paramref name="source"/> raises <seealso cref="INotifyPropertyChanged.PropertyChanged"/>.
        /// </summary>
        /// <typeparam name="T">The type of the source object. Type must implement <seealso cref="INotifyPropertyChanged"/>.</typeparam>
        /// <param name="source">The object to observe property changes on.</param>
        /// <param name="onPropertyChange">The action to perform when a property change event is raised.</param>
        /// <returns>Returns a token to allow consumer to stop observing.</returns>
        public static IDisposable WhenAnyPropertyChanges<T>(this T source, Action<T> onPropertyChange)
            where T : INotifyPropertyChanged
        {
            PropertyChangedEventHandler handler = (sender, e) => onPropertyChange((T)sender);
            source.PropertyChanged += handler;
            return Disposable.Create(() => source.PropertyChanged -= handler);
        }

        /// <summary>
        /// Exectutes an action when <paramref name="source"/> raises <seealso cref="INotifyPropertyChanged.PropertyChanged"/> for the given property.
        /// </summary>
        /// <typeparam name="T">The type of the source object. Type must implement <seealso cref="INotifyPropertyChanged"/>.</typeparam>
        /// <typeparam name="TProperty">The type of the property that is being observed.</typeparam>
        /// <param name="source">The object to observe property changes on.</param>
        /// <param name="property">An expression that describes which property to observe.</param>
        /// <param name="onPropertyChange">The action to execute when the property changes. The new value of the property is provided as the parameter.</param>
        /// <returns>Returns a token to allow consumer to stop observing.</returns>
        public static IDisposable WhenPropertyChanges<T, TProperty>(this T source, Expression<Func<T, TProperty>> property, Action<TProperty> onPropertyChange)
            where T : INotifyPropertyChanged
        {
            var propertyName = property.GetPropertyInfo().Name;
            var propertySelector = property.Compile();

            PropertyChangedEventHandler handler =
                (sender, e) =>
                {
                    if (e.PropertyName == propertyName)
                    {
                        var newValue = propertySelector(source);
                        onPropertyChange(newValue);
                    }
                };

            source.PropertyChanged += handler;
            return Disposable.Create(() => source.PropertyChanged -= handler);
        }

        public static IDisposable WhenCollectionChanges<TItem>(this ObservableCollection<TItem> collection, Action<CollectionChangedData<TItem>> onPropertyChange)
        {
            return WhenCollectionChangesImp(collection, onPropertyChange);
        }

         public static IDisposable WhenCollectionChanges<TItem>(
           this ReadOnlyObservableCollection<TItem> collection,
           Action<CollectionChangedData<TItem>> onPropertyChange)
         {
             return WhenCollectionChangesImp(collection, onPropertyChange);
         }

         private static IDisposable WhenCollectionChangesImp<TCollection, TItem>(
            TCollection collection,
            Action<CollectionChangedData<TItem>> onPropertyChange)
               where TCollection : IList<TItem>, INotifyCollectionChanged
         {
             NotifyCollectionChangedEventHandler onCollectionChanged = (sender, e) =>
             {
                 var payload = new CollectionChangedData<TItem>(e);
                 onPropertyChange(payload);
             };
             collection.CollectionChanged += onCollectionChanged;
             return Disposable.Create(() => { collection.CollectionChanged -= onCollectionChanged; });
         }


        //TODO: Allow the ability to push which property changed on underlying Item. (string.Empty for entire object)
        //TODO: Make Rx. Should allow filter by PropName (which would still push on string.Empty?)
        /// <summary>
        /// Executes an action when a collection or its items raise a change notification event.
        /// </summary>
        /// <typeparam name="TItem">The type of the collection items</typeparam>
        /// <param name="collection">The collection to observe.</param>
        /// <param name="onPropertyChange">The action to perform when the collection or its items change.</param>
        /// <returns>Returns a token to allow consumer to stop observing.</returns>
        public static IDisposable WhenCollectionItemsChange<TItem>(
            this ObservableCollection<TItem> collection,
            Action<CollectionChangedData<TItem>> onPropertyChange)
        {
            return WhenItemsPropertyChange(collection, onPropertyChange, _ => true);
        }

        public static IDisposable WhenItemsPropertyChange<TItem, TProperty>(
           this ObservableCollection<TItem> collection,
           Expression<Func<TItem, TProperty>> property,
           Action<CollectionChangedData<TItem>> onPropertyChange)
            where TItem : INotifyPropertyChanged
        {
            var propertyName = property.GetPropertyInfo().Name;
            return WhenItemsPropertyChange(collection, onPropertyChange, propName => propName == propertyName);
        }

        public static IDisposable WhenItemsPropertyChange<TItem, TProperty>(
           this ReadOnlyObservableCollection<TItem> collection,
           Expression<Func<TItem, TProperty>> property,
           Action<CollectionChangedData<TItem>> onPropertyChange)
            where TItem : INotifyPropertyChanged
        {
            var propertyName = property.GetPropertyInfo().Name;
            return WhenItemsPropertyChange(collection, onPropertyChange, propName => propName == propertyName);
        }

        private static IDisposable WhenItemsPropertyChange<TCollection, TItem>(
           TCollection collection,
           Action<CollectionChangedData<TItem>> onPropertyChange,
           Predicate<string> isPropertyNameRelevant)
              where TCollection : IList<TItem>, INotifyCollectionChanged
        {
            var trackedItems = new List<INotifyPropertyChanged>();
            PropertyChangedEventHandler onItemChanged =
                (sender, e) =>
                {
                    if (isPropertyNameRelevant(e.PropertyName))
                    {
                        var payload = new CollectionChangedData<TItem>((TItem)sender);
                        onPropertyChange(payload);
                    }
                };
            Action<IEnumerable<TItem>> registerItemChangeHandlers =
                items =>
                {
                    foreach (var notifier in items.OfType<INotifyPropertyChanged>())
                    {
                        trackedItems.Add(notifier);
                        notifier.PropertyChanged += onItemChanged;
                    }
                };
            Action<IEnumerable<TItem>> unRegisterItemChangeHandlers =
                items =>
                {
                    foreach (var notifier in items.OfType<INotifyPropertyChanged>())
                    {
                        notifier.PropertyChanged -= onItemChanged;
                        trackedItems.Remove(notifier);
                    }
                };
            NotifyCollectionChangedEventHandler onCollectionChanged =
                (sender, e) =>
                {
                    if (e.Action == NotifyCollectionChangedAction.Reset)
                    {
                        foreach (var notifier in trackedItems)
                        {
                            notifier.PropertyChanged -= onItemChanged;
                        }

                        var payload = new CollectionChangedData<TItem>(trackedItems, collection);
                        trackedItems.Clear();
                        registerItemChangeHandlers(collection);
                        onPropertyChange(payload);
                    }
                    else
                    {
                        var payload = new CollectionChangedData<TItem>(e);
                        unRegisterItemChangeHandlers(payload.OldItems);
                        registerItemChangeHandlers(payload.NewItems);
                        onPropertyChange(payload);
                    }
                };

            registerItemChangeHandlers(collection);
            collection.CollectionChanged += onCollectionChanged;

            return Disposable.Create(
                () =>
                {
                    collection.CollectionChanged -= onCollectionChanged;
                    unRegisterItemChangeHandlers(collection);
                });
        }
    }
}