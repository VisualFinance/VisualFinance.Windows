using System.Windows.Controls;

namespace VisualFinance.Windows.Controls
{
    internal class DirectionFilter
    {
        private static readonly DirectionFilter _allAllowed = new DirectionFilter();
        private static readonly DirectionFilter _comboBoxFilter = new ComboBoxDirectionFilter();
        public virtual bool CanNavigateUp { get { return true; } }
        public virtual bool CanNavigateDown { get { return true; } }
        public virtual bool CanNavigateLeft { get { return true; } }
        public virtual bool CanNavigateRight { get { return true; } }
        //TODO: Would be nice to have all of these working too. -LC
        //public virtual bool CanNavigatePageUp { get { return true; } }
        //public virtual bool CanNavigatePageDown { get { return true; } }
        //public virtual bool CanNavigateLineHome { get { return true; } }
        //public virtual bool CanNavigateLineEnd { get { return true; } }
        //public virtual bool CanNavigateListHome { get { return true; } }
        //public virtual bool CanNavigateListEnd { get { return true; } }

        private DirectionFilter()
        {
        }

        public static DirectionFilter Create(object originalSource)
        {
            var textBox = originalSource as TextBox;
            if (textBox != null) return new TextBoxDirectionFilter(textBox);

            var comboBox = originalSource as ComboBox;
            if (comboBox != null) return _comboBoxFilter;

            var comboBoxItem = originalSource as ComboBoxItem;
            if (comboBoxItem != null) return _comboBoxFilter;

            return _allAllowed;
        }

        private sealed class TextBoxDirectionFilter : DirectionFilter
        {
            private readonly TextBox _source;

            public TextBoxDirectionFilter(TextBox source)
            {
                _source = source;
            }

            public override bool CanNavigateLeft
            {
                get { return IsAtStart(); }
            }

            public override bool CanNavigateRight
            {
                get { return IsAtEnd(); }
            }

            private bool IsAtStart()
            {
                return string.IsNullOrEmpty(_source.Text) || _source.SelectionStart + _source.SelectionLength == 0;
            }

            private bool IsAtEnd()
            {
                return string.IsNullOrEmpty(_source.Text) || _source.SelectionStart + _source.SelectionLength == _source.Text.Length;
            }
        }

        private sealed class ComboBoxDirectionFilter : DirectionFilter
        {
            public override bool CanNavigateUp
            {
                get { return false; }
            }

            public override bool CanNavigateDown
            {
                get { return false; }
            }
        }
    }
}