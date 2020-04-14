using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Net;

namespace ASAIProgImitator
{
    public partial class RLSOptionsWindow : Window
    {
        public RLS_UI rls_ui;
        private class BoolTandem
        {
            public bool enable;
            public bool bool1;
            public bool bool2;

            public BoolTandem()
            {
                this.enable = true;
                this.bool1 = true;
                this.bool2 = true;
            }
        }

        public void typeComboBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            switch (typeComboBox.SelectedIndex)
            {
                case 0 : // ПРЛ
                {
                    // Канал
                    chComboBox.Items.Clear();
                    ComboBoxItem cbi = new ComboBoxItem();
                    cbi.Content = "1й канал";
                    chComboBox.Items.Add(cbi);
                    cbi = new ComboBoxItem();
                    cbi.Content = "2й канал";
                    chComboBox.Items.Add(cbi);
                    chComboBox.SelectedIndex = 0;

                    // Сигнал запроса
                    reqComboBox.Visibility = Visibility.Collapsed;
                    reqTextBlock.Visibility = Visibility.Collapsed;

                    // Элементы параметров
                    strDurCaptTextBlock.Visibility = Visibility.Visible;
                    strDurTextBox.Visibility = Visibility.Visible;
                    strDurUnitTextBlock.Visibility = Visibility.Visible;
                    strDurSlider.Visibility = Visibility.Visible;

                    trshCaptTextBlock.Visibility = Visibility.Visible;
                    trshTextBox.Visibility = Visibility.Visible;
                    trshUnitTextBlock.Visibility = Visibility.Visible;
                    trshSlider.Visibility = Visibility.Visible;

                    diagTypeTextBlock.Visibility = Visibility.Visible;
                    diagTypeComboBox.Visibility = Visibility.Visible;

                    // Состояние НРЗ
                    statNRZGroupBox.Visibility = Visibility.Collapsed;

                    // Категории
                    ctg0CheckBox.Content = "Категория 107";
                    ctg1CheckBox.Content = "Категория 255";
                    ctg2CheckBox.IsChecked = false;
                    (ctgListBox.Items[2] as ListBoxItem).Visibility = Visibility.Collapsed;
                } break;
                case 1 : // ВРЛ
                {
                    // Канал
                    chComboBox.Items.Clear();
                    ComboBoxItem cbi = new ComboBoxItem();
                    cbi.Content = "Канал УВД";
                    chComboBox.Items.Add(cbi);
                    cbi = new ComboBoxItem();
                    cbi.Content = "Канал RBS";
                    chComboBox.Items.Add(cbi);
                    chComboBox.SelectedIndex = 0;

                    // Сигнал запроса
                    reqComboBox.Visibility = Visibility.Visible;
                    reqComboBox.SelectedIndex = 0;
                    reqTextBlock.Visibility = Visibility.Visible;

                    // Элементы параметров
                    strDurCaptTextBlock.Visibility = Visibility.Collapsed;
                    strDurTextBox.Visibility = Visibility.Collapsed;
                    strDurUnitTextBlock.Visibility = Visibility.Collapsed;
                    strDurSlider.Visibility = Visibility.Collapsed;

                    trshCaptTextBlock.Visibility = Visibility.Collapsed;
                    trshTextBox.Visibility = Visibility.Collapsed;
                    trshUnitTextBlock.Visibility = Visibility.Collapsed;
                    trshSlider.Visibility = Visibility.Collapsed;

                    diagTypeTextBlock.Visibility = Visibility.Collapsed;
                    diagTypeComboBox.Visibility = Visibility.Collapsed;

                    // Состояние НРЗ
                    statNRZGroupBox.Visibility = Visibility.Collapsed;

                    // Категории
                    ctg0CheckBox.Content = "Категория 1";
                    ctg1CheckBox.Content = "Категория 2";
                    ctg2CheckBox.Content = "Категория 255";

                    (ctgListBox.Items[2] as ListBoxItem).Visibility = Visibility.Visible;
                } break;
                case 2 : // НРЗ
                {
                    // Канал
                    chComboBox.Items.Clear();
                    ComboBoxItem cbi = new ComboBoxItem();
                    cbi.Content = "1й канал";
                    chComboBox.Items.Add(cbi);
                    cbi = new ComboBoxItem();
                    cbi.Content = "2й канал";
                    chComboBox.Items.Add(cbi);
                    chComboBox.SelectedIndex = 0;
                    
                    // Сигнал запроса
                    reqComboBox.Visibility = Visibility.Visible;
                    reqComboBox.SelectedIndex = 0;
                    reqTextBlock.Visibility = Visibility.Visible;

                    // Элементы параметров
                    strDurCaptTextBlock.Visibility = Visibility.Collapsed;
                    strDurTextBox.Visibility = Visibility.Collapsed;
                    strDurUnitTextBlock.Visibility = Visibility.Collapsed;
                    strDurSlider.Visibility = Visibility.Collapsed;

                    trshCaptTextBlock.Visibility = Visibility.Collapsed;
                    trshTextBox.Visibility = Visibility.Collapsed;
                    trshUnitTextBlock.Visibility = Visibility.Collapsed;
                    trshSlider.Visibility = Visibility.Collapsed;

                    diagTypeTextBlock.Visibility = Visibility.Collapsed;
                    diagTypeComboBox.Visibility = Visibility.Collapsed;
                    
                    // Состояние НРЗ
                    statNRZGroupBox.Visibility = Visibility.Visible;
                    
                    // Категории
                    ctg0CheckBox.Content = "Категория 1";
                    ctg1CheckBox.Content = "Категория 2";
                    ctg2CheckBox.Content = "Категория 255";

                    (ctgListBox.Items[2] as ListBoxItem).Visibility = Visibility.Visible;
                } break;
            }
        }

        public void okButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = (bool?)true;
            this.Close();
        }

        public void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = (bool?)false;
            this.Close();
        }

        private void ctgCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((((sender as CheckBox).Parent
                              as Grid).Parent
                       as ListBoxItem).Tag == null)
                (((sender as CheckBox).Parent
                              as Grid).Parent
                       as ListBoxItem).Tag = new BoolTandem();

            ((((sender as CheckBox).Parent
                           as Grid).Parent
                    as ListBoxItem).Tag as BoolTandem).enable = true;
            okButton_Lock();
        }

        private void ctgCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ((((sender as CheckBox).Parent
                           as Grid).Parent
                    as ListBoxItem).Tag as BoolTandem).enable = false;
            okButton_Lock();
        }

        private void ctgIPTextBox_TextChanged(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            object tag = ((tb.Parent as Grid).Parent
                              as ListBoxItem).Tag;
            if (tag == null) tag = new BoolTandem();
            IPAddress ip;
            if (IPAddress.TryParse(tb.Text, out ip))
            {
                tb.Foreground = Brushes.Black;
                (tag as BoolTandem).bool1 = true;
            }
            else
            {
                tb.Foreground = Brushes.Red;
                (tag as BoolTandem).bool1 = false;
            }
            okButton_Lock();
        }

        private void ctgPortTextBox_TextChanged(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            object tag = ((tb.Parent as Grid).Parent
                              as ListBoxItem).Tag;
            if (tag == null) tag = new BoolTandem();
            int port;
            if (int.TryParse(tb.Text, out port))
            {
                tb.Foreground = Brushes.Black;
                (tag as BoolTandem).bool2 = true;
            }
            else
            {
                tb.Foreground = Brushes.Red;
                (tag as BoolTandem).bool2 = false;
            }
            okButton_Lock();
        }

        private void okButton_Lock()
        {
            bool solve = true;
            foreach (ListBoxItem lbi in ctgListBox.Items)
            {
                if (lbi.Tag != null)
                {
                    if ((lbi.Tag as BoolTandem).enable &&
                        !((lbi.Tag as BoolTandem).bool1 &&
                          (lbi.Tag as BoolTandem).bool2)) solve = false;
                }
            }
            okButton.IsEnabled = solve;
        }
    }
}
