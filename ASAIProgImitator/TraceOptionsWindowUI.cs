using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace ASAIProgImitator
{
    public partial class TraceOptionsWindow : Window
    {
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = (bool?)true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = (bool?)false;
            this.Close();
        }

        private void indvNumbTextBox_TextChanged(object sender, RoutedEventArgs e)
        {
            UInt32 nmb = 0;
            if ((UInt32.TryParse(indvNumbTextBox.Text, out nmb)) &&
                (nmb < 100000))
            {
                indvNumbTextBox.Foreground = Brushes.Black;
                indvNumbTextBox.Tag = true;
            }
            else
            {
                indvNumbTextBox.Foreground = Brushes.Red;
                indvNumbTextBox.Tag = false;
            }
            OkButton_Lock();
        }

        private void otTextBox_TextChanged(object sender, RoutedEventArgs e)
        {
            String s = (sender as TextBox).Text;
            double ot = 0.0;
            if ((double.TryParse(s, out ot)) &&
                (ot <= 100.0) && (ot >= 0.0))
            {
                (sender as TextBox).Foreground = Brushes.Black;
                (sender as TextBox).Tag = true;
            }
            else
            {
                (sender as TextBox).Foreground = Brushes.Red;
                (sender as TextBox).Tag = false;
            }
            OkButton_Lock();
        }

        private void hTextBox_TextChanged(object sender, RoutedEventArgs e)
        {
            String s = (sender as TextBox).Text;
            double h = 0.0;
            if ((double.TryParse(s, out h)) &&
                (h <= 39.990) && (h >= 0.0))
            {
                (sender as TextBox).Foreground = Brushes.Black;
                (sender as TextBox).Tag = true;
                OkButton.IsEnabled = true;
            }
            else
            {
                (sender as TextBox).Foreground = Brushes.Red;
                (sender as TextBox).Tag = false;
            }
            OkButton_Lock();
        }

        private void OkButton_Lock()
        {
            if (endHTextBox != null)
                OkButton.IsEnabled = (bool)indvNumbTextBox.Tag &&
                                     (bool)bgnOTTextBox.Tag && (bool)endOTTextBox.Tag &&
                                     (bool)bgnHTextBox.Tag && (bool)endHTextBox.Tag;
        }
    }
}
