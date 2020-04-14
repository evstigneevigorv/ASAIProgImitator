using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Forms = System.Windows.Forms;

namespace ASAIProgImitator
{
    public partial class RLSListWindow : Window
    {
        public List<RLS> rlsList;

        public void AddButton_Click(object sender, RoutedEventArgs e)
        {
            RLSOptionsWindow dlg = new RLSOptionsWindow();
            dlg.ShowDialog();
            
            if (dlg.DialogResult.Value == true)
            {
                RLS rls = new RLS();
                rls.Set(ref dlg.rls_ui);
                rlsList.Add(rls);

                ListBoxItem lbi = new ListBoxItem();
                RLSList_Set(ref lbi, ref dlg);
                rlsListBox.Items.Add(lbi);
            }
        }

        public void OptionsButton_Click(object sender, RoutedEventArgs e)
        {
            RLSOptionsWindow dlg = new RLSOptionsWindow();
            dlg.rls_ui.Set(rlsList[rlsListBox.SelectedIndex]);

            dlg.ShowDialog();
            if (dlg.DialogResult.Value == true)
            {
                RLS rls = new RLS();
                rls.Set(ref dlg.rls_ui);
                rlsList[rlsListBox.SelectedIndex] = rls;

                ListBoxItem lbi = new ListBoxItem();
                RLSList_Set(ref lbi, ref dlg);
                int i = rlsListBox.SelectedIndex;
                rlsListBox.Items.RemoveAt(i);
                rlsListBox.Items.Insert(i, lbi);
            }
        }

        public void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            rlsList.RemoveAt(rlsListBox.SelectedIndex);
            rlsListBox.Items.RemoveAt(rlsListBox.SelectedIndex);
            if (!rlsListBox.Items.IsEmpty) rlsListBox.SelectedIndex = 0;
        }

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

        private void RLSList_Set(ref ListBoxItem lbi, ref RLSOptionsWindow dlg)
        {
            Thickness thick = new Thickness(5.0);
            Grid gr = new Grid();
            gr.ColumnDefinitions.Add(new ColumnDefinition());
            gr.ColumnDefinitions[0].Width = new GridLength(2.0, GridUnitType.Star);
            gr.ColumnDefinitions.Add(new ColumnDefinition());
            gr.ColumnDefinitions[1].Width = new GridLength(0.7, GridUnitType.Star);
            gr.ColumnDefinitions.Add(new ColumnDefinition());
            gr.ColumnDefinitions[2].Width = new GridLength(0.7, GridUnitType.Star);
            gr.ColumnDefinitions.Add(new ColumnDefinition());
            gr.ColumnDefinitions[3].Width = new GridLength(1.0, GridUnitType.Star);
            gr.ColumnDefinitions.Add(new ColumnDefinition());
            gr.ColumnDefinitions[4].Width = new GridLength(1.0, GridUnitType.Star);
            gr.HorizontalAlignment = HorizontalAlignment.Stretch;
            gr.ShowGridLines = true;

            TextBlock nameTextBlock = new TextBlock();
            nameTextBlock.Text = dlg.nameTextBox.Text;
            nameTextBlock.Margin = thick;
            Grid.SetColumn(nameTextBlock, 0);

            TextBlock typeTextBlock = new TextBlock();
            typeTextBlock.Text = dlg.typeComboBox.Text;
            typeTextBlock.Margin = thick;
            Grid.SetColumn(typeTextBlock, 1);

            TextBlock distTextBlock = new TextBlock();
            distTextBlock.Text = dlg.distSlider.Value.ToString() + " км";
            distTextBlock.Margin = thick;
            Grid.SetColumn(distTextBlock, 2);
            
            Button grdColorButton = new Button();
            grdColorButton.Background = Brushes.DarkGreen;
            grdColorButton.Margin = thick;
            Grid.SetColumn(grdColorButton, 3);
            grdColorButton.Click += new RoutedEventHandler(ColorButton_Click);

            Button vsrColorButton = new Button();
            vsrColorButton.Background = Brushes.White;
            vsrColorButton.Margin = thick;
            Grid.SetColumn(vsrColorButton, 4);
            vsrColorButton.Click += new RoutedEventHandler(ColorButton_Click);

            gr.Children.Add(nameTextBlock);
            gr.Children.Add(typeTextBlock);
            gr.Children.Add(distTextBlock);
            gr.Children.Add(grdColorButton);
            gr.Children.Add(vsrColorButton);
            
            lbi.Content = gr;
        }

        public void RLSList_Set(ref ListBoxItem lbi, RLS rls)
        {
            Thickness thick = new Thickness(5.0);
            Grid gr = new Grid();
            gr.ColumnDefinitions.Add(new ColumnDefinition());
            gr.ColumnDefinitions[0].Width = new GridLength(2.0, GridUnitType.Star);
            gr.ColumnDefinitions.Add(new ColumnDefinition());
            gr.ColumnDefinitions[1].Width = new GridLength(0.7, GridUnitType.Star);
            gr.ColumnDefinitions.Add(new ColumnDefinition());
            gr.ColumnDefinitions[2].Width = new GridLength(0.7, GridUnitType.Star);
            gr.ColumnDefinitions.Add(new ColumnDefinition());
            gr.ColumnDefinitions[3].Width = new GridLength(1.0, GridUnitType.Star);
            gr.ColumnDefinitions.Add(new ColumnDefinition());
            gr.ColumnDefinitions[4].Width = new GridLength(1.0, GridUnitType.Star);
            gr.HorizontalAlignment = HorizontalAlignment.Stretch;
            gr.ShowGridLines = true;

            TextBlock nameTextBlock = new TextBlock();
            nameTextBlock.Text = rls.Name;
            nameTextBlock.Margin = thick;
            Grid.SetColumn(nameTextBlock, 0);

            TextBlock typeTextBlock = new TextBlock();
            switch (rls.Type)
            {
                case RLSType.PRL: { typeTextBlock.Text = "ПРЛ"; }; break;
                case RLSType.VRL: { typeTextBlock.Text = "ВРЛ"; }; break;
                case RLSType.NRZ: { typeTextBlock.Text = "НРЗ"; }; break;
            }
            typeTextBlock.Margin = thick;
            Grid.SetColumn(typeTextBlock, 1);

            TextBlock distTextBlock = new TextBlock();
            distTextBlock.Text = rls.Distance.ToString() + " км";
            distTextBlock.Margin = thick;
            Grid.SetColumn(distTextBlock, 2);

            Button grdColorButton = new Button();
            grdColorButton.Background = Brushes.DarkGreen;
            grdColorButton.Margin = thick;
            Grid.SetColumn(grdColorButton, 3);
            grdColorButton.Click += new RoutedEventHandler(ColorButton_Click);

            Button vsrColorButton = new Button();
            vsrColorButton.Background = Brushes.White;
            vsrColorButton.Margin = thick;
            Grid.SetColumn(vsrColorButton, 4);
            vsrColorButton.Click += new RoutedEventHandler(ColorButton_Click);

            gr.Children.Add(nameTextBlock);
            gr.Children.Add(typeTextBlock);
            gr.Children.Add(distTextBlock);
            gr.Children.Add(grdColorButton);
            gr.Children.Add(vsrColorButton);

            lbi.Content = gr;
        }

        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog dlg = new Forms.ColorDialog();
            if (dlg.ShowDialog() == Forms.DialogResult.OK)
                (sender as Button).Background =
                    new SolidColorBrush(Color.FromArgb(dlg.Color.A,
                                                       dlg.Color.R,
                                                       dlg.Color.G,
                                                       dlg.Color.B));
        }
    }
}
