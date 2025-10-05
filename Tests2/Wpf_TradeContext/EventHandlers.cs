using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace Wpf_TradeContext
{
    public partial class MainWindow : Window
    {
        private void WindowClosing(object sender, CancelEventArgs e)
        {
            var result = MessageBox.Show("Are you sure to Exit?", "GS.Trading Tools", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.Cancel)
                e.Cancel = true;
        }
        private void WindowClosed(object sender, EventArgs e)
        {
          //  _tx.Stop();
          //  _tx.Close();
        }
    }
}
