using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VisualFinance.Windows.Controls
{
    internal abstract class Navigator
    {
        public static Navigator Create(TreeListView treeListView, KeyEventArgs keyEvent)
        {
            var directionFilter = DirectionFilter.Create(GetFocusedControlElement());

            switch (keyEvent.Key)
            {
                case Key.Left:
                    return new MoveLeftCellNavigator(directionFilter);
                case Key.Right:
                    return new MoveRightCellNavigator(directionFilter);
                case Key.Up:
                    return new MoveUpCellNavigator(directionFilter);
                case Key.Down:
                case Key.Return:    //Key.Enter is same key.
                    return new MoveDownCellNavigator(directionFilter);
            }
            return new NoOpNavigator();
        }



        private readonly DirectionFilter _directionFilter;
        private Navigator(DirectionFilter directionFilter)
        {
            _directionFilter = directionFilter;
        }

        protected abstract FocusNavigationDirection Direction { get; }

        protected abstract bool CanNavigate(DirectionFilter directionFilter);

        public bool Navigate()
        {
            if (CanNavigate(_directionFilter))
            {
                var elementWithFocus = GetFocusedElement();
                if (elementWithFocus != null)
                {
                    return MoveFocus(elementWithFocus, new TraversalRequest(Direction));
                }
            }
            return false;
        }

        private static object GetFocusedElement()
        {
            var elementWithFocus = Keyboard.FocusedElement;
            var framworkElement = elementWithFocus as FrameworkElement;
            if (framworkElement != null)
                return framworkElement;
            
            var contentElement = elementWithFocus as FrameworkContentElement;
            if (contentElement != null)
                return contentElement;
            
            return elementWithFocus;
        }

        private static object GetFocusedControlElement()
        {
            var elementWithFocus = Keyboard.FocusedElement;
            var framworkElement = elementWithFocus as FrameworkElement;
            if (framworkElement != null)
            {
                if (framworkElement.TemplatedParent != null && !(framworkElement.TemplatedParent is ContentPresenter))
                {
                    return framworkElement.TemplatedParent;
                }
                return framworkElement;
            }

            var contentElement = elementWithFocus as FrameworkContentElement;
            if (contentElement != null)
            {
                if (contentElement.TemplatedParent != null)
                {
                    return contentElement.TemplatedParent;
                }
                return contentElement;
            }
            return elementWithFocus;
        }


        private static bool MoveFocus(object source, TraversalRequest request)
        {
            var uiElement = source as UIElement;
            if (uiElement != null)
                return uiElement.MoveFocus(request);

            var contentElement = source as ContentElement;
            if (contentElement != null)
                return contentElement.MoveFocus(request);

            var uiElement3D = source as UIElement3D;
            if (uiElement3D != null)
                return uiElement3D.MoveFocus(request);

            return false;
        }


        #region Implementations
        private class NoOpNavigator : Navigator
        {
            public NoOpNavigator()
                : base(null)
            {
            }

            protected override FocusNavigationDirection Direction { get { throw new NotImplementedException(); } }

            protected override bool CanNavigate(DirectionFilter _)
            {
                return false;
            }
        }

        private class MoveLeftCellNavigator : Navigator
        {
            public MoveLeftCellNavigator(DirectionFilter directionFilter)
                : base(directionFilter)
            {
            }

            protected override FocusNavigationDirection Direction { get { return FocusNavigationDirection.Previous; } }

            protected override bool CanNavigate(DirectionFilter directionFilter)
            {
                return directionFilter.CanNavigateLeft;
            }
        }

        private class MoveRightCellNavigator : Navigator
        {
            public MoveRightCellNavigator(DirectionFilter directionFilter)
                : base(directionFilter)
            {
            }

            protected override FocusNavigationDirection Direction { get { return FocusNavigationDirection.Next; } }

            protected override bool CanNavigate(DirectionFilter directionFilter)
            {
                return directionFilter.CanNavigateRight;
            }
        }

        private class MoveUpCellNavigator : Navigator
        {
            public MoveUpCellNavigator(DirectionFilter directionFilter)
                : base(directionFilter)
            {
            }

            protected override FocusNavigationDirection Direction { get { return FocusNavigationDirection.Up; } }

            protected override bool CanNavigate(DirectionFilter directionFilter)
            {
                return directionFilter.CanNavigateUp;
            }
        }

        private class MoveDownCellNavigator : Navigator
        {
            public MoveDownCellNavigator(DirectionFilter directionFilter)
                : base(directionFilter)
            {
            }

            protected override FocusNavigationDirection Direction { get { return FocusNavigationDirection.Down; } }

            protected override bool CanNavigate(DirectionFilter directionFilter)
            {
                return directionFilter.CanNavigateDown;
            }
        }
        #endregion
    }
}