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

        public ucCategory()
        {
            InitializeComponent();
        }

        public void SetEnable(bool IsEnable)
        {
            this.tbKey.IsEnabled = IsEnabled;
            this.tbCategory.IsEnabled = IsEnabled;
        }

        private void layoutRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.SetEnable(true);
        }
    }
}
