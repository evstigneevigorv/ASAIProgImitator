using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASAIProgImitator
{
    public partial class BuildSaveWindow : Window
    {
        private void upButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void downButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void restoreButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = (bool?)true;
            this.Close();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = (bool?)false;
            this.Close();
        }
    }
}
