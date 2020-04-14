using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace ASAIProgImitator
{
    public partial class PrjOptionsWindow : Window
    {
        public void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = (bool?)true;
        }

        public void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = (bool?)false;
        }

        public void durTextBox_TextChanged(object sender, RoutedEventArgs e)
        {
            TimeSpan ts = new TimeSpan();
            if (TimeSpan.TryParse(durTextBox.Text, out ts))
            {
                durTextBox.Foreground = Brushes.Black;
                OkButton.IsEnabled = true;
            }
            else
            {
                durTextBox.Foreground = Brushes.Red;
                OkButton.IsEnabled = false;
            }
        }
    }
}
