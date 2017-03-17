using System;
using System.Collections;
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
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public Hashtable htCategory = new Hashtable();
        public ArrayList alKeys = new ArrayList();
        public ArrayList alValues = new ArrayList();
        public ucCategory selectCategory = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnCAdd_Click(object sender, RoutedEventArgs e)
        {
            winCategory winCategory = new winCategory(this);

            winCategory.Owner = this;

            if (winCategory.ShowDialog() == true)
            {
                ucCategory ucCategory = new ucCategory(this, winCategory.tbKey.Text, winCategory.tbCategory.Text);
                wpCategory.Children.Add(ucCategory);

                alKeys.Add(winCategory.tbKey.Text);
                alValues.Add(winCategory.tbCategory.Text);
                htCategory.Add(winCategory.tbKey.Text, ucCategory);
            }
        }

        private void btnCEdit_Click(object sender, RoutedEventArgs e)
        {
            if(selectCategory == null)
            {
                MessageBox.Show("Select Category.");
            }
        }

        private void btnCDelete_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
