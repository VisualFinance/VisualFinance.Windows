using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Threading;
using System.Windows.Media;

//BUG:  for [1[ABC]2[DEF]] first select C then Shift Select to D. When you then ShiftSelect 2 you should get C2DEF selected, but get CEF
//TODO: Implement multiple Select for Drag.

namespace VisualFinance.Windows.Controls
{
    //Straight copy of MSDN example
    //http://msdn.microsoft.com/en-us/library/ms771523.aspx
    //Original code can be found here.
    //http://archive.msdn.microsoft.com/wpfsamples
    //Scroll with Header freeze added
    //http://social.msdn.microsoft.com/Forums/en/wpf/thread/52a62ade-ada3-421a-9e8f-98f0ebefe919
    //then rewrite for multi select from 
    //http://dotnetlearning.wordpress.com/2010/10/14/multi-selection-tree-view/
    //Further code added to support drag drop. (Custom implementation)
    //Further code added to support hiding columns. (Custom implementation)

    //TODO: Support left/right/up/down arrow keys to navigate around focusable controls.

    [TemplatePart(Name = HeaderRowPresenterTemplateName, Type = typeof(GridViewHeaderRowPresenter)),]
    [TemplatePart(Name = HeaderScrollViewTemplateName, Type = typeof(ScrollViewer)),]
    [TemplatePart(Name = BodyScrollViewTemplateName, Type = typeof(ScrollViewer)),]
    public sealed class TreeListView : ItemsControl
    {
        private const string HeaderRowPresenterTemplateName = "PART_HeaderRowPresenter";
        private const string HeaderScrollViewTemplateName = "PART_HeaderScroll";
        private const string BodyScrollViewTemplateName = "PART_BodyScroll";

        private ScrollViewer _scrollViewerHeader;
        private ScrollViewer _scrollViewerBody;
        private GridViewHeaderRowPresenter _headerRowPresenter;

        public TreeListView()
        {
            _roSelectedItems = new ReadOnlyObservableCollection<TreeListViewItem>(_selectedItems);
            AddHandler(Keyboard.PreviewKeyDownEvent, new RoutedEventHandler(OnPreviewKeyDown), true);
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, ExecuteCopy, CanExecuteCopy));
        }

        private void CanExecuteCopy(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (DataContext is IDataObject);// || Descendants().Select(c => c.DataContext).OfType<IDataObject>().Any();
        }

        private void ExecuteCopy(object sender, ExecutedRoutedEventArgs e)
        {
            if (DataContext is IDataObject)
            {
                Clipboard.SetDataObject(DataContext, false);
            }
            //else //loop through each of the items, through each col and build up a tsv.
            //{
            //    //DataFormats.Html;
            //    //DataFormats.Text;
            //    //DataFormats.UnicodeText;
            //    //DataFormats.CommaSeparatedValue;
            //    //DataFormats.Serializable;
            //    //DataFormats.StringFormat;
            // 
            //    Clipboard.SetDataObject(data, false);
            //}
        }


        #region Standard Control theme support
        static TreeListView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeListView), new FrameworkPropertyMetadata(typeof(TreeListView)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_headerRowPresenter != null)
                _headerRowPresenter.Loaded -= OnHeaderRowPresenterLoaded;
            _headerRowPresenter = (GridViewHeaderRowPresenter)Template.FindName(HeaderRowPresenterTemplateName, this);
            if (_headerRowPresenter != null)
                _headerRowPresenter.Loaded += OnHeaderRowPresenterLoaded;

            if (_scrollViewerBody != null)
                _scrollViewerBody.ScrollChanged -= OnScrollViewerBodyScrollChanged;
            _scrollViewerBody = (ScrollViewer)Template.FindName(BodyScrollViewTemplateName, this);
            if (_scrollViewerBody != null)
                _scrollViewerBody.ScrollChanged += OnScrollViewerBodyScrollChanged;

            _scrollViewerHeader = (ScrollViewer)Template.FindName(HeaderScrollViewTemplateName, this);
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

        public TreeListViewColumnCollection Columns
        {
            get { return (TreeListViewColumnCollection)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register("Columns", typeof(TreeListViewColumnCollection), typeof(TreeListView), new UIPropertyMetadata());

        #endregion

        #region IsColumnVisible AttachedProperty

        public static bool GetIsColumnVisible(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsColumnVisibleProperty);
        }

        public static void SetIsColumnVisible(DependencyObject obj, bool value)
        {
            obj.SetValue(IsColumnVisibleProperty, value);
        }

        public static readonly DependencyProperty IsColumnVisibleProperty =
            DependencyProperty.RegisterAttached("IsColumnVisible", typeof(bool), typeof(TreeListView), new UIPropertyMetadata(true));

        #endregion

        #region IsFocusOnLoadEnabled DependencyProperty

        public bool IsFocusOnLoadEnabled
        {
            get { return (bool)GetValue(IsFocusOnLoadEnabledProperty); }
            set { SetValue(IsFocusOnLoadEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsFocusOnLoadEnabledProperty =
            DependencyProperty.Register("IsFocusOnLoadEnabled", typeof(bool), typeof(TreeListView), new UIPropertyMetadata(false));

        #endregion

        #region FocusOnLoad attached property

        public static bool GetFocusOnLoad(DependencyObject obj)
        {
            return (bool)obj.GetValue(FocusOnLoadProperty);
        }

        public static void SetFocusOnLoad(DependencyObject obj, bool value)
        {
            obj.SetValue(FocusOnLoadProperty, value);
        }

        public static readonly DependencyProperty FocusOnLoadProperty =
            DependencyProperty.RegisterAttached("FocusOnLoad", typeof(bool), typeof(TreeListView), new UIPropertyMetadata(false, OnFocusOnLoadChanged));

        private static void OnFocusOnLoadChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var parentTreeView = d.FindParent<TreeListView>();
            if (parentTreeView == null || !parentTreeView.IsFocusOnLoadEnabled) return;

            var frameworkElement = d as FrameworkElement;
            if (frameworkElement != null)
            {
                if (frameworkElement.IsLoaded)
                {
                    Keyboard.Focus(frameworkElement);
                }
                else
                {
                    frameworkElement.Loaded += FrameworkElement_Loaded;
                }
            }
        }

        static void FrameworkElement_Loaded(object sender, RoutedEventArgs e)
        {
            var frameworkElement = (FrameworkElement)sender;
            frameworkElement.Loaded -= FrameworkElement_Loaded;
            Keyboard.Focus(frameworkElement);
        }

        #endregion

        #region CanUpdateParents DependencyProperty

        public bool CanUpdateParents
        {
            get { return (bool)GetValue(CanUpdateParentsProperty); }
            set { SetValue(CanUpdateParentsProperty, value); }
        }

        public static readonly DependencyProperty CanUpdateParentsProperty =
            DependencyProperty.Register("CanUpdateParents", typeof(bool), typeof(TreeListView), new UIPropertyMetadata(false));

        #endregion

        #region Indepedent header scrolling
        /*
         * Allows the header to horizontally scroll with the column content, but freezes the header when scrolling vertically.
         */

        void OnHeaderRowPresenterLoaded(object sender, RoutedEventArgs e)
        {
            //Columns should only update once the _headerRowPresenter is loaded and has been laid out, else WPF engine throws indexOutOfRane on a Visual Children collection.
            if (Columns != null && _headerRowPresenter.IsLoaded && _headerRowPresenter.IsArrangeValid && _headerRowPresenter.IsMeasureValid)
            {
                Columns.IsBound = true;
            }
        }

        void OnScrollViewerBodyScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            /*Previously used the ScrollChangedEventArgs for every thing. 
             1st sometimes e.ViewportWidth would come back as Infinity. This will throw.
             2nd Child objects (specificaly TextBoxes) when focused would push events with tiny widths (as it it was the width of the textbox)

            //You cant set _scrollViewerHeader.Width to infinity so just ignore these events. (Seems to work fine) -LC
            if (double.IsInfinity(e.ViewportWidth)) return;

            _scrollViewerHeader.Width = e.ViewportWidth;    //Setting focus to child TextBoxes set this value to about the width of the text box?!
            _scrollViewerHeader.ScrollToHorizontalOffset(e.HorizontalOffset);
            */

            //Try using the _scrollViewerBody directly instead. -LC
            _scrollViewerHeader.Width = _scrollViewerBody.Width;
            _scrollViewerHeader.ScrollToHorizontalOffset(_scrollViewerBody.HorizontalOffset);
        }

        #endregion

        #region MultiSelect
        private readonly ReadOnlyObservableCollection<TreeListViewItem> _roSelectedItems;
        private readonly ObservableCollection<TreeListViewItem> _selectedItems = new ObservableCollection<TreeListViewItem>();
        private readonly List<TreeListViewItem> _rangeSelection = new List<TreeListViewItem>();
        private TreeListViewItem _rangeSelectionStart;
        private bool _ignoreSelectionChanged;

        #region Properties
        public ReadOnlyObservableCollection<TreeListViewItem> SelectedItems
        {
            get { return _roSelectedItems; }
        }

        public IEnumerable<TreeListViewItem> Children
        {
            get { return this.Children<TreeListViewItem>(); }
        }

        #region IsSelectionManagementSuppressed DependencyProperty

        public bool IsSelectionManagementSuppressed
        {
            get { return (bool)GetValue(IsSelectionManagementSuppressedProperty); }
            set { SetValue(IsSelectionManagementSuppressedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSelectionManagementSuppressed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSelectionManagementSuppressedProperty =
            DependencyProperty.Register("IsSelectionManagementSuppressed", typeof(bool), typeof(TreeListView), new UIPropertyMetadata(false));

        #endregion

        #endregion

        #region Methods
        private bool IgnoreSelectionChanged()
        {
            return _ignoreSelectionChanged || IsSelectionManagementSuppressed;
        }

        public void UnselectAll()
        {
            foreach (var child in Children)
            {
                child.UnselectAllChildren();
            }
        }

        public void SelectAllExpandedItems()
        {
            foreach (var child in Children)
            {
                child.SelectAllExpandedChildren();
            }
        }

        internal void OnItemSelected(TreeListViewItem viewItem)
        {
            if (viewItem == null || IgnoreSelectionChanged())
                return;

            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                ExtendSelectionTo(viewItem);
            }
            else
            {
                _rangeSelection.Clear();
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    UpdateSelectedItems(viewItem);
                }
                else
                {
                    SelectSingleItem(viewItem);
                }

                _rangeSelectionStart = viewItem.IsSelected ? viewItem : null;
            }
        }
        #endregion

        #region Helper Methods

        private void SelectSingleItem(TreeListViewItem viewItem)
        {
            var selectedItems = Descendants()
                .Where(child => child.IsSelected
                                && !ReferenceEquals(child, viewItem)
                                && (CanUpdateParents || child.ParentTreeListViewItem != null))
                .ToList();

            var isSelected = viewItem.IsSelected;
            using (SupressSelectionHandler())
            {
                foreach (var selectedItem in selectedItems)
                {
                    selectedItem.IsSelected = false;
                    UpdateSelectedItems(selectedItem);
                }
                //Ensure that modifications to other items have not unselected this item.
                viewItem.IsSelected = isSelected;
            }

            UpdateSelectedItems(viewItem);
        }

        private void ExtendSelectionTo(TreeListViewItem rangeSelectionEnd)
        {
            //if _rangeSelectionStart == null then it is just a single select so: _rangeSelectionStart=rangeSelectionEnd
            if (_rangeSelectionStart == null)
            {
                _rangeSelectionStart = rangeSelectionEnd;
                UpdateSelectedItems(rangeSelectionEnd);
            }
            else
            {
                //SelectAllItemsBetween _rangeSelectionStart and rangeSelectionEnd
                var descendants = Descendants()
                    //Filter out parent rows, unless a parent row was explicitly selected.
                   .Where(child =>
                       CanUpdateParents
                       || child.ParentTreeListViewItem != null
                       || ReferenceEquals(child, _rangeSelectionStart)
                       || ReferenceEquals(child, rangeSelectionEnd))
                   .ToList();

                var startIdx = descendants.IndexOf(_rangeSelectionStart);
                var endIdx = descendants.IndexOf(rangeSelectionEnd);

                if (startIdx > endIdx)
                {
                    endIdx = Interlocked.Exchange(ref startIdx, endIdx);
                }
                var lastRangeSelection = _rangeSelection.ToArray().ToList();
                _rangeSelection.Clear();

                using (SupressSelectionHandler())
                {
                    for (int i = startIdx; i <= endIdx; i++)
                    {
                        var item = descendants[i];
                        if (!item.IsSelected || ReferenceEquals(item, rangeSelectionEnd))
                        {
                            item.IsSelected = true;
                            UpdateSelectedItems(item);
                            _rangeSelection.Add(item);
                        }
                        lastRangeSelection.Remove(item);
                    }
                    foreach (var oldItem in lastRangeSelection)
                    {
                        oldItem.IsSelected = false;
                        UpdateSelectedItems(oldItem);
                    }
                }
            }
        }

        private void UpdateSelectedItems(TreeListViewItem viewItem)
        {
            if (viewItem.IsSelected)
            {
                if (!_selectedItems.Contains(viewItem))
                    _selectedItems.Add(viewItem);
            }
            else
            {
                _selectedItems.Remove(viewItem);
            }
        }

        private IDisposable SupressSelectionHandler()
        {
            _ignoreSelectionChanged = true;
            return Disposable.Create(() => _ignoreSelectionChanged = false);
        }

        private IEnumerable<TreeListViewItem> Descendants()
        {
            return Children.SelectMany(child => child.Flatten(treeviewitem => treeviewitem.Children));
        }
        #endregion

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);
            if (e.OriginalSource == this
                || e.OriginalSource == _scrollViewerHeader
                || e.OriginalSource == _scrollViewerBody)
            {
                UnselectAll();
            }
        }

        #endregion

        #region Cell navigation

        private void OnPreviewKeyDown(object sender, RoutedEventArgs e)
        {
            //React to Left/Right/Up/Down/Enter key presses to navigate like Excel. -LC
            var navigator = Navigator.Create(this, (KeyEventArgs)e);
            e.Handled = navigator.Navigate() | e.Handled;
        }

        #endregion

        //protected override AutomationPeer OnCreateAutomationPeer()
        //{
        //    return new TreeListViewAutomationPeer(this);
        //} 
    }
}
