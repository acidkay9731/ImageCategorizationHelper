using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        public ArrayList alKeys = new ArrayList();
        public ArrayList alCategory = new ArrayList();
        public ucCategory selectCategory = null;
        public Hashtable htCategiry = new Hashtable();

        ListBoxItem selectListBoxItem = null;

        string strPath;
        string strIniFile;

        public MainWindow()
        {
            InitializeComponent();

            strPath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);

            strIniFile = strPath + "\\categorys.ini";

            GetCategories();
        }

        private void GetCategories()
        {
            if (File.Exists(strIniFile))
            {
                IEnumerable<string> categoryList = File.ReadLines(strIniFile);

                foreach (string category in categoryList)
                {
                    string[] split = category.Split(':');

                    ucCategory uccategory = new ucCategory(this, split[0], split[1]);

                    wpCategory.Children.Add(uccategory);
                    htCategiry.Add(split[0], uccategory);
                }
            }
        }

        private void btnCAdd_Click(object sender, RoutedEventArgs e)
        {
            winCategory winCategory = new winCategory(this);

            winCategory.Owner = this;

            if (winCategory.ShowDialog() == true)
            {
                ucCategory category = new ucCategory(this, winCategory.tbKey.Text, winCategory.tbCategory.Text);
                wpCategory.Children.Add(category);

                alKeys.Add(winCategory.tbKey.Text);
                alCategory.Add(winCategory.tbCategory.Text);
                htCategiry.Add(winCategory.tbKey.Text, category);
            }
        }

        private void btnCDelete_Click(object sender, RoutedEventArgs e)
        {
            if (selectCategory == null)
            {
                MessageBox.Show("Select Category.");
            }
            else
            {
                wpCategory.Children.Remove(selectCategory);
                htCategiry.Remove(selectCategory.tbKey.Text);

                selectCategory = null;
            }
        }

        private void btnCSave_Click(object sender, RoutedEventArgs e)
        {
            if(wpCategory.Children.Count == 0)
            {
#if false
                MessageBox.Show("There are no Categories.");
                return;
#else
                MessageBox.Show("Clear Categorie list.");
#endif
            }

            using (StreamWriter sw = File.CreateText(strIniFile))
            {
                foreach (ucCategory category in wpCategory.Children)
                {
                    sw.WriteLine(category.tbKey.Text + ":" + category.tbCategory.Text);
                }
            }
        }

        private void btnSelFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();

            if(dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                tbSelectFolder.Text = dialog.SelectedPath;

                SetImageList();
            }
        }

        private void SetImageList()
        {
            string [] imageList = Directory.GetFiles(tbSelectFolder.Text);

            foreach(string imageName in imageList)
            {
                ListBoxItem lbi = new ListBoxItem();

                lbi.Content = imageName.Split('\\')[imageName.Split('\\').Length - 1];
                lbi.Tag = imageName;

                lstFiles.Items.Add(lbi);
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if(btnStart.Content.ToString() == "START CATEGORIZATION")
            {
                btnStart.Content = "END CATEGORIZATION";
                MW.KeyUp += MW_KeyUp;

                SetImage();
            }
            else if (btnStart.Content.ToString() == "END CATEGORIZATION")
            {
                btnStart.Content = "START CATEGORIZATION";
                MW.KeyUp -= MW_KeyUp;
            }
        }

        private void SetImage()
        {
            if (lstFiles.Items.Count != 0)
            {
                for(int i = 0; i < lstFiles.Items.Count; i++)
                {
                    selectListBoxItem = lstFiles.Items[i] as ListBoxItem;

                    if(selectListBoxItem != null
                        && selectListBoxItem.Background != Brushes.Red)
                    {
                        break;
                    }
                }

                try
                {
                    ImageSource imageSrc = BitmapFromUri(new Uri(selectListBoxItem.Tag.ToString()));
                    imgMain.Source = imageSrc;
                }
                catch(Exception ex)
                {
                    string err = ex.Message + "\r\n" + ex.StackTrace;
                    tbLog.Text += err;
                    tbLog.ScrollToEnd();

                    selectListBoxItem.Background = Brushes.Red;
                }
            }
        }

        public static ImageSource BitmapFromUri(Uri source)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = source;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            return bitmap;
        }

        private void MW_KeyUp(object sender, KeyEventArgs e)
        {
            object obj = htCategiry[e.Key.ToString().ToUpper()];

            if(obj != null)
            {
                ucCategory category = obj as ucCategory;

                string categoryPath = strPath + "\\" + category.tbCategory.Text;

                if (!Directory.Exists(categoryPath))
                {
                    Directory.CreateDirectory(categoryPath);
                }

                try
                {
                    imgMain.Source = null;
                    File.Move(selectListBoxItem.Tag.ToString(), tbSelectFolder.Text + "\\" + selectListBoxItem.Content.ToString());
                    lstFiles.Items.Remove(selectListBoxItem);
                }
                catch(Exception ex)
                {
                    string err = ex.Message + "\r\n" + ex.StackTrace;
                    tbLog.Text += err;
                    tbLog.ScrollToEnd();
                    selectListBoxItem.Background = Brushes.Red;
                }

                SelectNextListBoxItem();
            }
        }

        private void SelectNextListBoxItem()
        {
            selectListBoxItem = null;

            for(int i = 0; i< lstFiles.Items.Count; i++)
            {
                ListBoxItem lbi = lstFiles.Items[i] as ListBoxItem;

                if(lbi.Background != Brushes.Red)
                {
                    selectListBoxItem = lbi;
                    break;
                }
            }
        }
    }
}
