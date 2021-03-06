﻿using System;
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

                    alKeys.Add(split[0]);
                    alCategory.Add(split[1]);
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
                alKeys.Remove(selectCategory.tbKey.Text);
                alCategory.Remove(selectCategory.tbCategory.Text);
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
                MessageBox.Show("Clear Categories.");
#endif
            }

            using (StreamWriter sw = File.CreateText(strIniFile))
            {
                foreach (ucCategory category in wpCategory.Children)
                {
                    sw.WriteLine(category.tbKey.Text + ":" + category.tbCategory.Text);
                }
            }

            MessageBox.Show("Save Categories Complite.");
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
            lstFiles.Items.Clear();

            string [] imageList = Directory.GetFiles(tbSelectFolder.Text);

            foreach(string imageName in imageList)
            {
                ListBoxItem lbi = new ListBoxItem();

                lbi.Content = imageName.Split('\\')[imageName.Split('\\').Length - 1];
                lbi.Tag = imageName;

                lstFiles.Items.Add(lbi);
            }

            tbImageCount.Text = lstFiles.Items.Count.ToString();
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

        public static ImageSource BitmapFromUri(Uri source)
        {
            string ext = source.AbsolutePath.Split('.')[source.AbsolutePath.Split('.').Length - 1].ToUpper();

            var bitmap = new BitmapImage();

            bitmap.BeginInit();
            bitmap.UriSource = source;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();

            return bitmap;
        }

        private void SetImage()
        {
            selectListBoxItem = null;

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
                    string imageName = selectListBoxItem.Tag.ToString();

                    string extName = imageName.Split('.')[imageName.Split('.').Length - 1].ToUpper();

                    if (extName == "GIF")
                    {
                        mediaElement.Visibility = Visibility.Hidden;
                        imgMain.Visibility = Visibility.Hidden;

                        wfhImgGif.Visibility = Visibility.Visible;
                        imgGif.ImageLocation = imageName;

                    }
                    else if(extName == "MP4")
                    {
                        wfhImgGif.Visibility = Visibility.Hidden;
                        imgMain.Visibility = Visibility.Hidden;

                        mediaElement.Visibility = Visibility.Visible;
                        mediaElement.Source = new Uri(imageName);
                        mediaElement.Play();
                    }
                    else
                    {
                        wfhImgGif.Visibility = Visibility.Hidden;
                        mediaElement.Visibility = Visibility.Hidden;

                        imgMain.Visibility = Visibility.Visible;
                        ImageSource imageSrc = BitmapFromUri(new Uri(imageName));
                        imgMain.Source = imageSrc;
                    }
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

        private void ReleaseResources()
        {
            imgMain.Source = null;

            if (imgGif.Image != null)
            {
                imgGif.Image.Dispose();
                imgGif.ImageLocation = null;
            }

            mediaElement.Stop();
            mediaElement.Source = null;
        }

        private void MW_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                //var alt = e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Alt);
                var ctrl = e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control);
                //var altGr = alt & ctrl;
                //var shift = e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Shift);

                if (ctrl == true)
                {
                    if (e.Key == Key.Z)
                    {
                        undo();
                    }

                    return;
                }

                object obj = htCategiry[e.Key.ToString().ToUpper()];

                if (obj != null)
                {
                    ucCategory category = obj as ucCategory;

                    string categoryPath = tbSelectFolder.Text + "\\" + category.tbCategory.Text;

                    if (!Directory.Exists(categoryPath))
                    {
                        Directory.CreateDirectory(categoryPath);
                    }

                    try
                    {
                        ReleaseResources();

                        string strTrgetName = categoryPath + "\\" + selectListBoxItem.Content.ToString();

                        try
                        {
                            File.SetLastAccessTime(selectListBoxItem.Tag.ToString(), DateTime.Now);
                        }
                        catch (Exception ex)
                        {
                            System.Threading.Thread.Sleep(1000);
                            File.SetLastAccessTime(selectListBoxItem.Tag.ToString(), DateTime.Now);
                        }

                        File.Move(selectListBoxItem.Tag.ToString(), strTrgetName);

                        tbLog.Text += "\n" + selectListBoxItem.Tag.ToString() + "→" + strTrgetName;
                        tbLog.ScrollToEnd();
                        lstFiles.Items.Remove(selectListBoxItem);
                    }
                    catch (Exception ex)
                    {
                        string err = ex.Message + "\r\n" + ex.StackTrace;
                        tbLog.Text += err;
                        tbLog.ScrollToEnd();
                        selectListBoxItem.Background = Brushes.Red;
                    }

                    SetImage();

                    tbImageCount.Text = lstFiles.Items.Count.ToString();
                }
            }
            catch(Exception ex)
            {
                string err = ex.Message + "\n" + ex.StackTrace;
                MessageBox.Show(err);
            }
        }

        private void undo()
        {
            string strUndo = tbLog.Text.Split('\n')[tbLog.Text.Split('\n').Length - 1];

            if(strUndo.Trim().Length == 0
                || !strUndo.Contains("→"))
            {
                return;
            }

            ReleaseResources();

            string [] strSplit = strUndo.Split('→');

            try
            {
                File.Move(strSplit[1], strSplit[0]);
                File.SetLastAccessTime(strSplit[0], DateTime.Now);
                ListBoxItem lbi = new ListBoxItem();

                lbi.Content = strSplit[0].Split('\\')[strSplit[0].Split('\\').Length - 1];
                lbi.Tag = strSplit[0];

                lstFiles.Items.Insert(0, lbi);

                tbLog.Text = tbLog.Text.Replace("\n" + strUndo, "");

                tbLog.ScrollToEnd();

                SetImage();
            }
            catch(Exception ex)
            {
                string err = ex.Message + "\n" + ex.StackTrace;
                tbLog.Text += err;
                tbLog.ScrollToEnd();
            }

            tbImageCount.Text = lstFiles.Items.Count.ToString();
        }
    }
}
