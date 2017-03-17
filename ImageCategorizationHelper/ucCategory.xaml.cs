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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImageCategorizationHelper
{
    /// <summary>
    /// ucCategory.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ucCategory : UserControl
    {
        string[] hashItem = new string[2];

        MainWindow parent = null;

        public bool IsSelected = false;

        Brush brDefault;
        Brush brSelect;

        public ucCategory(MainWindow mw, string strKey, string strValue)
        {
            InitializeComponent();

            parent = mw;

            brDefault = border.BorderBrush;
            brSelect = Brushes.Red;

            tbKey.Text = strKey;
            tbCategory.Text = strValue;
        }

        public void SetSelect(bool IsEnable)
        {
            if (IsEnable)
            {
                border.BorderBrush = brSelect;
                this.IsSelected = true;

                if (parent.selectCategory != null)
                {
                    parent.selectCategory.SetSelect(false);
                }

                parent.selectCategory = this;
            }
            else
            {
                border.BorderBrush = brDefault;
                this.IsSelected = false;
            }
        }

        private void layoutRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.SetSelect(true);
        }
    }
}
