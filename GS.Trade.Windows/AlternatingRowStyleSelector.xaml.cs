using System.Windows;
using System.Windows.Controls;

namespace GS.Trade.Windows
{
    public class AlternatingRowStyleSelector : StyleSelector
    {
        public Style DefaultStyle { get; set; }
        public Style AlternateStyle { get; set; }

        private bool _isAlternate = false;

        public override Style SelectStyle( object item, DependencyObject container)
        {
            Style style = _isAlternate ? AlternateStyle : DefaultStyle;
            _isAlternate = !_isAlternate;
            return style;
        }
    }
}