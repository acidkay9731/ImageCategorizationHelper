using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ImageCategorizationHelper
{
    /// <summary>
    /// winCategory.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class winCategory : Window
    {
        MainWindow parent = null;

        bool IsCancelCategoryEnter = false;

        public winCategory(MainWindow mw)
        {
            InitializeComponent();

            parent = mw;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            OK();
        }

        private void OK()
        {
            if(tbKey.Text.Length == 0
                || tbCategory.Text.Length == 0)
            {
                MessageBox.Show("Input Key and Category.");
                return;
            }

            if (parent.alKeys.Contains(tbKey.Text))
            {
                MessageBox.Show("Existing Key.");
                tbKey.Focus();
                tbKey.SelectAll();
            }
            else if (parent.alValues.Contains(tbCategory.Text))
            {
                MessageBox.Show("Existing Category.");
                tbCategory.Focus();
                IsCancelCategoryEnter = true;
            }
            else
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        private void btnCANCLE_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void tbKey_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
         
        }

        private void tbKey_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(tbKey.Text.Length == 1)
            {
                tbCategory.Focus();
            }
        }

        private void tbCategory_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
            
        }

        private void tbCategory_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (!IsCancelCategoryEnter)
                {
                    btnOK.Focus();
                    btnOK_Click(sender, null);
                }
                else
                {
                    IsCancelCategoryEnter = false;
                }
            }
        }

        private void winMain_Loaded(object sender, RoutedEventArgs e)
        {
            tbKey.Focus();
        }

        private void winMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void winMain_Closed(object sender, EventArgs e)
        {

        }
    }
}
