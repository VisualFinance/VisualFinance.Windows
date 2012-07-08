using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace VisualFinance.Windows
{
    public static class UiElementExtensions
    {
        public static T FindParent<T>(this DependencyObject element) where T : DependencyObject
        {
            // Walk up the element tree to the nearest T item.
            var parent = element as T;
            while ((parent == null) && (element != null))
            {
                element = VisualTreeHelper.GetParent(element);
                parent = element as T;
            }
            return parent;
        }

        public static object GetDataContext(this object source)
        {
            if (source == null) return null;

            var frameworkElement = source as FrameworkElement;
            if (frameworkElement != null)
                return frameworkElement.DataContext;


            var frameworkContentElement = source as FrameworkContentElement;
            return frameworkContentElement != null
                       ? frameworkContentElement.DataContext
                       : null;
        }

        public static bool TryGetDragData<TParent>(this DependencyObject originalSource, out TParent dragSource, out DataObject data)
            where TParent : FrameworkElement
        {
            data = null;
            dragSource = originalSource.FindParent<TParent>();
            if (dragSource != null)
            {
                var dataContext = dragSource.DataContext as IDraggable;
                if (dataContext != null && dataContext.CanDrag())
                {
                    data = new DataObject(dataContext);
                    data.SetData(typeof(IDraggable), dataContext);
                    return true;
                }
            }
            return false;
        }

        public static Binding Clone(this Binding source)
        {
            var clone = new Binding();
            if (source.ElementName != null) clone.ElementName = source.ElementName;
            else if (source.RelativeSource != null) clone.RelativeSource = source.RelativeSource;
            else if (source.Source != null) clone.Source = source.Source;

            clone.AsyncState = source.AsyncState;
            clone.BindingGroupName = source.BindingGroupName;
            clone.BindsDirectlyToSource = source.BindsDirectlyToSource;
            clone.Converter = source.Converter;
            clone.ConverterCulture = source.ConverterCulture;
            clone.ConverterParameter = source.ConverterParameter;
            clone.FallbackValue = source.FallbackValue;
            clone.IsAsync = source.IsAsync;
            clone.Mode = source.Mode;
            clone.NotifyOnSourceUpdated = source.NotifyOnSourceUpdated;
            clone.NotifyOnTargetUpdated = source.NotifyOnTargetUpdated;
            clone.NotifyOnValidationError = source.NotifyOnValidationError;
            clone.Path = source.Path;
            clone.StringFormat = source.StringFormat;
            clone.TargetNullValue = source.TargetNullValue;
            clone.UpdateSourceExceptionFilter = source.UpdateSourceExceptionFilter;
            clone.UpdateSourceTrigger = source.UpdateSourceTrigger;
            clone.ValidatesOnDataErrors = source.ValidatesOnDataErrors;
            clone.ValidatesOnExceptions = source.ValidatesOnExceptions;
            foreach (var validationRule in source.ValidationRules)
            {
                clone.ValidationRules.Add(validationRule);
            }
            clone.XPath = source.XPath;
            return clone;
        }

    }
}