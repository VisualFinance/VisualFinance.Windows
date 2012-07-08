using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VisualFinance.Windows.Controls
{   
    //http://dotnetlearning.wordpress.com/2010/10/14/multi-selection-tree-view/

    public sealed class TreeListViewItem : TreeViewItem
    {
        #region Standard Control theme support
        static TreeListViewItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeListViewItem), new FrameworkPropertyMetadata(typeof(TreeListViewItem)));
        }

        public TreeListViewItem()
        {
            IsKeyboardFocusWithinChanged += TreeListViewItem_IsKeyboardFocusWithinChanged;
            DataContextChanged += TreeListViewItem_DataContextChanged;
            var hasItemsDescr = DependencyPropertyDescriptor.FromProperty(HasItemsProperty, typeof(ItemsControl));
            if (hasItemsDescr != null)
            {
                hasItemsDescr.AddValueChanged(this, OnHasItemsChanged);
            }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TreeListViewItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TreeListViewItem;
        }
        #endregion

        #region Columns DependencyProperty

        public GridViewColumnCollection Columns
        {
            get { return (GridViewColumnCollection)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }
        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register("Columns", typeof(GridViewColumnCollection), typeof(TreeListViewItem), new UIPropertyMetadata());

        #endregion

        #region Drag drop features.

        private Point _lastMouseDown;
        private bool _isLeftMouseInOurControl;

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            _isLeftMouseInOurControl = true;
            _lastMouseDown = e.GetPosition(this);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.Handled)
                return;
            if (_isLeftMouseInOurControl && e.LeftButton == MouseButtonState.Pressed)
            {
                var currentPosition = e.GetPosition(this);

                // Note: This should be based on some accessibility number and not just 2 pixels
                if ((Math.Abs(currentPosition.X - _lastMouseDown.X) > 2.0) ||
                    (Math.Abs(currentPosition.Y - _lastMouseDown.Y) > 2.0))
                {
                    DataObject dragData;
                    TreeViewItem dragSource;
                    var originalSource = e.OriginalSource as DependencyObject;

                    if (originalSource.TryGetDragData(out dragSource, out dragData))
                    {
                        DragDrop.DoDragDrop(dragSource, dragData, DragDropEffects.Move);
                    }
                }
            }
            else
            {
                _isLeftMouseInOurControl = false;
            }
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);
            e.Effects = DragDropEffects.None;
            e.Handled = true;

            var target = e.OriginalSource.GetDataContext() as IDragTarget;
            var dragSource = e.Data.GetData(typeof(IDraggable)) as IDraggable;
            if (target != null && dragSource != null)
            {
                e.Effects = DragDropEffects.Move;
                target.ReceiveDrop(dragSource);
            }
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);
            CheckDropTarget(e);
        }

        protected override void OnDragLeave(DragEventArgs e)
        {
            base.OnDragLeave(e);
            CheckDropTarget(e);
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);
            CheckDropTarget(e);
        }

        private static void CheckDropTarget(DragEventArgs e)
        {
            if (!IsValidDropTarget(e))
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        private static bool IsValidDropTarget(DragEventArgs e)
        {
            var target = e.OriginalSource.GetDataContext() as IDragTarget;
            var dragSource = e.Data.GetData(typeof(IDraggable)) as IDraggable;
            if (target == null || dragSource == null)
            {
                return false;
            }

            return target.CanReceiveDrop(dragSource);
        }

        #endregion

        #region Multiselect

        //TODO: Instead of making static, expose this from the Parent (ie Reduce the scope from static(global) to just a single TreeListView)
        private static int _selectionSupressionDepth;
        private static bool IsSelectionChangeIgnored { get { return _selectionSupressionDepth > 0; } }
        private static IDisposable IgnoreSelectionChanges()
        {
            _selectionSupressionDepth++;
            return Disposable.Create(() => { _selectionSupressionDepth--; });
        }

        protected override void OnSelected(RoutedEventArgs e)
        {
            base.OnSelected(e);
            if (!IsSelectionChangeIgnored && ParentTreeListView != null)
            {
                ParentTreeListView.OnItemSelected(this);
            }
        }

        /// <summary>
        /// Get the UI Parent Control of this node.
        /// </summary>
        public ItemsControl ParentItemsControl
        {
            get { return ItemsControlFromItemContainer(this); }
        }

        /// <summary>
        /// Get the TreeListView in which this node is hosted in.
        /// Null value means that this node is not hosted into a TreeListView control.
        /// </summary>
        public TreeListView ParentTreeListView
        {
            get
            {
                for (var container = ParentItemsControl; container != null; container = ItemsControlFromItemContainer(container))
                {
                    var view = container as TreeListView;
                    if (view != null)
                    {
                        return view;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Get the Parent MultipleSelectionTreeViewItem of this node.
        /// Remark: Null value means that this node is hosted into a control (e.g. TreeListView).
        /// </summary>
        public TreeListViewItem ParentTreeListViewItem
        {
            get { return (ParentItemsControl as TreeListViewItem); }
        }

        public IEnumerable<TreeListViewItem> Children
        {
            get { return this.Children<TreeListViewItem>(); }
        }

        /// <summary>
        /// Recursively un-select all children.
        /// </summary>
        public void UnselectAllChildren()
        {
            foreach (var child in Children)
            {
                child.UnselectAllChildren();
            }

            if (IsSelected)
            {
                IsSelected = false;
            }
        }

        /// <summary>
        /// Recursively select all children.
        /// </summary>
        public void SelectAllExpandedChildren()
        {
            foreach (var child in Children)
            {
                child.SelectAllExpandedChildren();
            }

            if (!IsSelected)
            {
                IsSelected = true;
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (ParentTreeListView == null)
                return;

            if (e.LeftButton == MouseButtonState.Released &&
                e.RightButton == MouseButtonState.Pressed)
                return;

            //Setting the Selection could affect selections for other items (depending on implementations).
            //  We only want to tell the parent about this selection, so suppress all others from telling parent.
            using (IgnoreSelectionChanges())
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    IsSelected = !IsSelected;
                }
                else
                {
                    IsSelected = true;
                }
            }
            ParentTreeListView.OnItemSelected(this);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            try
            {
                TreeListViewItem itemToSelect = null;

                if (e.Key == Key.Left)
                {
                    IsExpanded = false;
                    e.Handled = true;
                }
                else if (e.Key == Key.Right)
                {
                    IsExpanded = true;
                    e.Handled = true;
                }
                else if (e.Key == Key.Up)
                {
                    // In this case we need to select the last child of the last expandend node of
                    // - the previous at the same level (if this index node is NOT 0)
                    // - the parent node (if this index node is 0)

                    int currentNodeIndex = ParentItemsControl.ItemContainerGenerator.IndexFromContainer(this);

                    if (currentNodeIndex == 0)
                    {
                        itemToSelect = ParentTreeListViewItem;
                    }
                    else
                    {
                        var tmp = GetPreviousNodeAtSameLevel(this);
                        itemToSelect = GetLastVisibleChildNodeOf(tmp);
                    }
                    e.Handled = true;
                }
                else if (e.Key == Key.Down)
                {
                    // In this case we need to select:
                    // - the first child node (if this node is expanded)
                    // - the next at the same level (if this not the last child)
                    // - the next at the same level of the parent node (if this is the last child)

                    if (IsExpanded && Items.Count > 0)
                    { // Select first Child
                        itemToSelect = ItemContainerGenerator.ContainerFromIndex(0) as TreeListViewItem;
                    }
                    else
                    {
                        itemToSelect = GetNextNodeAtSameLevel(this);

                        if (itemToSelect == null) // current node has no subsequent node at the same level
                        {
                            var tmp = ParentTreeListViewItem;

                            while (itemToSelect == null && tmp != null) // searhing for the first parent that has a subsequent node at the same level
                            {
                                itemToSelect = GetNextNodeAtSameLevel(tmp);
                                tmp = tmp.ParentTreeListViewItem;
                            }

                        }
                    }
                    e.Handled = true;
                }

                if (itemToSelect != null)
                {
                    itemToSelect.Focus();
                    itemToSelect.IsSelected = true;
                }
            }
            catch (Exception ex)
            {
                /* Silently ignore */
                Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Retrieve the previous node that is at the same level.
        /// </summary>
        /// <param name="item">The node starting with you want to retrieve the previous one.</param>
        /// <returns>Null if there is no previous node at the same level.</returns>
        private static TreeListViewItem GetPreviousNodeAtSameLevel(TreeListViewItem item)
        {
            if (item == null)
                return null;

            TreeListViewItem previousNodeAtSameLevel = null;

            var parentControl = item.ParentItemsControl;
            if (parentControl != null)
            {
                int index = parentControl.ItemContainerGenerator.IndexFromContainer(item);
                if (index != 0) // if this is not the last item
                {
                    previousNodeAtSameLevel = parentControl.ItemContainerGenerator.ContainerFromIndex(index - 1) as TreeListViewItem;
                }
            }

            return previousNodeAtSameLevel;
        }

        /// <summary>
        /// Retrieve the last displayed child node of the given one.
        /// </summary>
        /// <param name="item">The node starting with you want to retrieve the last visible node.</param>
        /// <returns>The last child node that is displayed, or the node itself in case it is not expanded.</returns>
        private static TreeListViewItem GetLastVisibleChildNodeOf(TreeListViewItem item)
        {
            var lastVisibleNode = item;

            // Retrieving last child of last expanded node
            while (lastVisibleNode != null && lastVisibleNode.Items.Count > 0 && lastVisibleNode.IsExpanded)
                lastVisibleNode = lastVisibleNode.ItemContainerGenerator.ContainerFromIndex(lastVisibleNode.Items.Count - 1) as TreeListViewItem;

            return lastVisibleNode;
        }

        /// <summary>
        /// Retrieve the subsequent node that is at the same level.
        /// </summary>
        /// <param name="item">The node starting with you want to retrieve the subsequent one.</param>
        /// <returns>Null if there is no subsequent node at the same level.</returns>
        private static TreeListViewItem GetNextNodeAtSameLevel(TreeListViewItem item)
        {
            if (item == null)
                return null;

            TreeListViewItem nextNodeAtSameLevel = null;

            var parentControl = item.ParentItemsControl;
            if (parentControl != null)
            {
                int index = parentControl.ItemContainerGenerator.IndexFromContainer(item);
                if (index != parentControl.Items.Count - 1) // if this is not the last item
                {
                    nextNodeAtSameLevel = parentControl.ItemContainerGenerator.ContainerFromIndex(index + 1) as TreeListViewItem;
                }
            }

            return nextNodeAtSameLevel;
        }
        #endregion

        #region Select when row has focus

        private void OnHasItemsChanged(object sender, EventArgs e)
        {
            if (HasItems)
            {
                this.IsKeyboardFocusWithinChanged -= TreeListViewItem_IsKeyboardFocusWithinChanged;
            }
            else
            {
                this.IsKeyboardFocusWithinChanged += TreeListViewItem_IsKeyboardFocusWithinChanged;
            }
        }

        void TreeListViewItem_IsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue) IsSelected = true;
        }

        #endregion

        #region AutoResizeColumns

        private IDisposable _propertyChangedSubscription;

        void TreeListViewItem_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_propertyChangedSubscription != null)
            {
                _propertyChangedSubscription.Dispose();
            }
            var newContext = e.NewValue as INotifyPropertyChanged;
            if (newContext != null)
            {
                _propertyChangedSubscription = Observable.FromEventPattern
                    <PropertyChangedEventHandler, PropertyChangedEventArgs>(
                        handler => handler.Invoke,
                        h => newContext.PropertyChanged += h,
                        h => newContext.PropertyChanged -= h)
                    //Ignore fast moving changes as upadting the UI will be costly.
                    .Throttle(TimeSpan.FromMilliseconds(200))
                    //HACK: Hmmmm a bit lazy here. I should really inject an ISchedulerProvider to make this testable. -LC
                    .ObserveOn(SynchronizationContext.Current)
                    .Subscribe(_ => ResizeColumns());
            }
        }

        private void ResizeColumns()
        {
            //Resize columns
            foreach (var column in Columns)
            {
                if (double.IsNaN(column.Width))
                {
                    column.Width = column.ActualWidth;
                    column.Width = double.NaN;
                }
            }
        }

        #endregion

        //protected override AutomationPeer OnCreateAutomationPeer()
        //{
        //    return new TreeListViewItemAutomationPeer(this);
        //}
    }
}