using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;

namespace VisualFinance.Windows.Controls
{
    /// <summary>
    /// Specialized collection to allow columns to be hidden in the <see cref="TreeListView"/>.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridViewColumn"/> can not be included in multiple <see cref="GridViewColumnCollection"/> instances.
    /// To work around this the <see cref="TreeListViewColumnCollection"/> is just an implementation of an <see cref="ObservableCollection{GridViewColumn}"/>
    /// that is extended to have a VisibleColumns property. The VisibleColumns property is effectively a syncronised view of the columns that have
    /// the <see cref="TreeListView.IsColumnVisibleProperty"/> set to true.
    /// </remarks>
    public class TreeListViewColumnCollection : ObservableCollection<GridViewColumn>
    {
        private readonly GridViewColumnCollection _visibleColumns = new GridViewColumnCollection();
        private bool _isBound;

        public GridViewColumnCollection VisibleColumns
        {
            get { return _visibleColumns; }
        }

        //Only apply the UpdateVisibility method once it is bound. This is a problem in the GridViewHeaderRowPresenter.RemoveHeader method.
        // It throws an error as it tries to access its visual children before they are created. .NET 3.5sp1 -LC
        internal bool IsBound
        {
            get { return _isBound; }
            set
            {
                if (_isBound != value)
                {
                    _isBound = value;
                    SynchronizeVisibleColumns();
                }
            }
        }

        protected override void ClearItems()
        {
            foreach (var column in this)
            {
                RemoveObserver(column);
            }
            _visibleColumns.Clear();

            base.ClearItems();
        }

        protected override void InsertItem(int index, GridViewColumn item)
        {
            base.InsertItem(index, item);
            if (TreeListView.GetIsColumnVisible(item))
            {
                var idx = VisibleIndex(index);
                _visibleColumns.Insert(idx, item);
            }

            AddObserver(item);
        }

        protected override void RemoveItem(int index)
        {
            var old = this[index];
            RemoveObserver(old);
            var visIdx = _visibleColumns.IndexOf(this[index]);
            if (visIdx != -1) _visibleColumns.RemoveAt(visIdx);

            base.RemoveItem(index);
        }

        protected override void SetItem(int index, GridViewColumn item)
        {
            var old = this[index];
            RemoveObserver(old);
            var visIdx = _visibleColumns.IndexOf(this[index]);

            base.SetItem(index, item);

            if (visIdx != -1) _visibleColumns[visIdx] = item;
            AddObserver(item);
        }

        private int VisibleIndex(int logicalIndex)
        {
            var column = this[logicalIndex++];
            while (!TreeListView.GetIsColumnVisible(column) && logicalIndex < Count)
            {
                column = this[logicalIndex++];
            }

            if (logicalIndex >= _visibleColumns.Count) return _visibleColumns.Count;

            var idx = _visibleColumns.IndexOf(column);
            if (idx == -1 && logicalIndex >= _visibleColumns.Count) return _visibleColumns.Count;
            return idx;
        }

        private void SynchronizeVisibleColumns()
        {
            for (int i = 0; i < Count; i++)
            {
                var column = this[i];
                if (TreeListView.GetIsColumnVisible(column))
                {
                    if (!_visibleColumns.Contains(column))
                        _visibleColumns.Add(column);
                }
                else
                {
                    //It is safe to remove items that aren't there, and saves on duplicate idx look up.
                    _visibleColumns.Remove(column);
                }
            }
        }

        private void AddObserver(GridViewColumn column)
        {
            var isVisibleDescr = DependencyPropertyDescriptor.FromProperty(TreeListView.IsColumnVisibleProperty, typeof(TreeListView));
            if (isVisibleDescr != null)
            {
                isVisibleDescr.AddValueChanged(column, OnIsColumnVisibleChanged);
            }
        }

        private void RemoveObserver(GridViewColumn column)
        {
            var isVisibleDescr = DependencyPropertyDescriptor.FromProperty(TreeListView.IsColumnVisibleProperty, typeof(TreeListView));
            if (isVisibleDescr != null)
            {
                isVisibleDescr.RemoveValueChanged(column, OnIsColumnVisibleChanged);
            }
        }

        private void OnIsColumnVisibleChanged(object sender, EventArgs e)
        {
            if (!IsBound)
                return;

            var column = (GridViewColumn)sender;
            if (TreeListView.GetIsColumnVisible(column))
            {
                var logicalIdx = IndexOf(column);
                var insertIndex = this.Take(logicalIdx).Count(TreeListView.GetIsColumnVisible);
                _visibleColumns.Insert(insertIndex, column);
            }
            else
            {
                _visibleColumns.Remove(column);
            }
        }
    }
}