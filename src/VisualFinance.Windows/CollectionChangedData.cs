using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace VisualFinance.Windows
{
    public sealed class CollectionChangedData<T>
    {
        private readonly NotifyCollectionChangedAction _action;
        private readonly ReadOnlyCollection<T> _newItems;
        private readonly ReadOnlyCollection<T> _oldItems;

        public CollectionChangedData(NotifyCollectionChangedEventArgs collectionChangedEventArgs):
            this(collectionChangedEventArgs.OldItems, collectionChangedEventArgs.NewItems)
        {
            _action = collectionChangedEventArgs.Action;
        }

        public CollectionChangedData(T changedItem)
        {
            _action = NotifyCollectionChangedAction.Replace;
            _newItems = new ReadOnlyCollection<T>(new T[] { changedItem });
            _oldItems = new ReadOnlyCollection<T>(new T[] { });
        }

        public CollectionChangedData(IEnumerable oldItems, IEnumerable newItems)
        {
            _newItems = newItems == null
                            ? new ReadOnlyCollection<T>(new T[] { })
                            : new ReadOnlyCollection<T>(newItems.Cast<T>().ToList());

            _oldItems = oldItems == null
                            ? new ReadOnlyCollection<T>(new T[] { })
                            : new ReadOnlyCollection<T>(oldItems.Cast<T>().ToList());

            _action = _newItems.Count==0 
                          ? NotifyCollectionChangedAction.Reset 
                          : NotifyCollectionChangedAction.Replace;
        }

        public NotifyCollectionChangedAction Action
        {
            get { return _action; }
        }

        public ReadOnlyCollection<T> NewItems
        {
            get { return _newItems; }
        }

        public ReadOnlyCollection<T> OldItems
        {
            get { return _oldItems; }
        }
    }
}