using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ASAIProgImitator
{
    /// <summary>
    /// Логика взаимодействия для RLSOptionsWindow.xaml
    /// </summary>
    public partial class RLSOptionsWindow : Window
    {
        public RLSOptionsWindow()
        {
            InitializeComponent();
            this.rls_ui = new RLS_UI();
        }

        private void RLSOptionsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.rlsDataGrid.DataContext = this.rls_ui;
        }
    }
}
