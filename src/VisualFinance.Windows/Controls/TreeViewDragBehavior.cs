using System.Windows;
using System.Windows.Controls;

namespace VisualFinance.Windows.Controls
{
    /// <summary>
    /// Implements an attached property used for styling TreeViewItems when
    /// they're a possible drop target.
    /// </summary>
    public static class TreeViewDragBehavior
    {
        #region private variables
        /// <summary>
        /// the TreeViewItem that is the current drop target
        /// </summary>
        private static TreeViewItem _currentItem = null;

        /// <summary>
        /// Indicates whether the current TreeViewItem is a possible
        /// drop target
        /// </summary>
        private static bool _dropPossible;
        #endregion

        #region IsPossibleDropTarget
        /// <summary>
        /// Property key (since this is a read-only DP) for the IsPossibleDropTarget property.
        /// </summary>
        private static readonly DependencyPropertyKey IsPossibleDropTargetKey =
            DependencyProperty.RegisterAttachedReadOnly(
                "IsPossibleDropTarget",
                typeof(bool),
                typeof(TreeViewDragBehavior),
                new FrameworkPropertyMetadata(null,
                                              new CoerceValueCallback(CalculateIsPossibleDropTarget)));


        /// <summary>
        /// Dependency Property IsPossibleDropTarget.
        /// Is true if the TreeViewItem is a possible drop target (i.e., if it would receive
        /// the OnDrop event if the mouse button is released right now).
        /// </summary>
        public static readonly DependencyProperty IsPossibleDropTargetProperty = IsPossibleDropTargetKey.DependencyProperty;

        /// <summary>
        /// Getter for IsPossibleDropTarget
        /// </summary>
        public static bool GetIsPossibleDropTarget(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsPossibleDropTargetProperty);
        }

        /// <summary>
        /// Coercion method which calculates the IsPossibleDropTarget property.
        /// </summary>
        private static object CalculateIsPossibleDropTarget(DependencyObject item, object value)
        {
            return (item == _currentItem) && (_dropPossible);
        }

        #endregion

        /// <summary>
        /// Initializes the <see cref="TreeViewDragBehavior"/> class.
        /// </summary>
        static TreeViewDragBehavior()
        {
            // Get all drag enter/leave events for TreeViewItem.
            EventManager.RegisterClassHandler(typeof(TreeViewItem),
                                              TreeViewItem.PreviewDragEnterEvent,
                                              new DragEventHandler(OnDragEvent), true);
            EventManager.RegisterClassHandler(typeof(TreeViewItem),
                                              TreeViewItem.PreviewDragLeaveEvent,
                                              new DragEventHandler(OnDragLeave), true);
            EventManager.RegisterClassHandler(typeof(TreeViewItem),
                                              TreeViewItem.PreviewDragOverEvent,
                                              new DragEventHandler(OnDragEvent), true);
            EventManager.RegisterClassHandler(typeof(TreeViewItem),
                                              TreeViewItem.DropEvent,
                                              new DragEventHandler(OnDropEvent), true);
        }

        #region event handlers
        /// <summary>
        /// Called when an item is dragged over the TreeViewItem.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.DragEventArgs"/> instance containing the event data.</param>
        static void OnDragEvent(object sender, DragEventArgs args)
        {
            lock (IsPossibleDropTargetProperty)
            {
                _dropPossible = false;

                ReleaseItem();
                if (args.Effects != DragDropEffects.None)
                {
                    _dropPossible = true;
                }
                AquireItem(sender);
            }
        }

        /// <summary>
        /// Called when the drag cursor leaves the TreeViewItem
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.DragEventArgs"/> instance containing the event data.</param>
        static void OnDragLeave(object sender, DragEventArgs args)
        {
            lock (IsPossibleDropTargetProperty)
            {
                _dropPossible = false;
                ReleaseItem();
                AquireItem(sender);
            }
        }

        static void OnDropEvent(object sender, DragEventArgs e)
        {
            _dropPossible = false;
            _currentItem.InvalidateProperty(IsPossibleDropTargetProperty);
        }
        #endregion

        private static void AquireItem(object sender)
        {
            var tvi = sender as TreeViewItem;
            if (tvi != null)
            {
                _currentItem = tvi;
                // Tell that item to re-calculate the IsPossibleDropTarget property
                _currentItem.InvalidateProperty(IsPossibleDropTargetProperty);
            }
        }

        private static void ReleaseItem()
        {
            if (_currentItem != null)
            {
                // Tell the item that previously had the mouse that it no longer does.
                DependencyObject oldItem = _currentItem;
                _currentItem = null;
                oldItem.InvalidateProperty(IsPossibleDropTargetProperty);
            }
        }
    }
}