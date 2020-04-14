using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ASAIProgImitator
{
    public partial class BuildWindow : Window
    {
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.progressBar.Value == 100.0) this.DialogResult = (bool?)true;
            else this.DialogResult = (bool?)false;
            this.Close();
        }
    }
}
