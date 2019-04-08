using Microsoft.Win32;
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
using TileMakerLibrary;

namespace TileMakerWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            string info = "";
            if (Image150x150Logo.Source == null && Image70x70Logo.Source == null)
                info += "At least one image should be selected\n";
            if (AppPathBox.Text == "")
                info += "A desktop app file should be attached\n";
            if (info != "")
            {
                MessageBox.Show(info, "Attention", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            
            try
            {
                var maker = new TileMaker(AppPathBox.Text, ShortcutPathBox.Text);
                if (Image150x150Logo.Source != null && Image70x70Logo.Source != null)
                {
                    maker.SetSquare150x150Logo(Image150x150Logo.Tag.ToString());
                    maker.SetSquare70x70Logo(Image70x70Logo.Tag.ToString());
                }
                else if (Image150x150Logo.Source != null)
                    maker.SetSquareLogo(Image150x150Logo.Tag.ToString());
                else
                    maker.SetSquareLogo(Image70x70Logo.Tag.ToString());
                maker.SetShowNameOnSquare150x150Logo((bool)ShowNameCheckBox.IsChecked);
                maker.SetForegroundColor(ForegroundColorComboBox.SelectedIndex == 0 ? true : false);
                maker.MakeTile();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }            
            MessageBox.Show("Your Tile has been customized successfully", "Excellent", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Load150x150LogoButton_Click(object sender, RoutedEventArgs e)
        {
            string path = "";
            Image150x150Logo.Source = LoadImageFromOpenFileDialog(ref path);
            Image150x150Logo.Tag = path;
        }

        private void Load70x70LogoButton_Click(object sender, RoutedEventArgs e)
        {
            string path = "";
            Image70x70Logo.Source = LoadImageFromOpenFileDialog(ref path);
            Image70x70Logo.Tag = path;
        }

        private BitmapImage LoadImageFromOpenFileDialog(ref string path)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpg;*.jpeg;*.gif)|*.png;*.jpg;*.jpeg;*.gif"
            };
            if (dialog.ShowDialog() == true)
            {
                path = dialog.FileName;
                return GetImage(path);
            }                
            return null;
        }
        
        private BitmapImage GetImage(string path)
        {
            var bitmap = new BitmapImage();
            if (System.IO.File.Exists(path))
            {
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                using (var ms = new System.IO.MemoryStream(System.IO.File.ReadAllBytes(path)))
                {
                    bitmap.StreamSource = ms;
                    bitmap.EndInit();
                    bitmap.Freeze();
                }
            }
            return bitmap;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            Image150x150Logo.Source = null;
            Image70x70Logo.Source = null;
            ForegroundColorComboBox.SelectedIndex = 0;
            BackgroundColorCodeBox.Text = "";
            //TileNameBox.Text = "";
            ShowNameCheckBox.IsChecked = false;
            AppPathBox.Text = "";
            ShortcutPathBox.Text = "";
        }

        private void AttachShortcutButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Shortcut files (*.lnk)|*.lnk",
                InitialDirectory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu), "Programs")
            };
            if (dialog.ShowDialog() == true)
            {
                var shortcut = dialog.FileName;
                ShortcutPathBox.Text = shortcut;
                AppPathBox.Text = Utilities.GetTargetPath(shortcut);
            }
                
        }

        private void AttachAppButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Executable files (*.exe)|*.exe"
            };
            if (dialog.ShowDialog() == true)
                AppPathBox.Text = dialog.FileName;            
        }

        private void RecoverButton_Click(object sender, RoutedEventArgs e)
        {
            string appPath = AppPathBox.Text, shortcutPath = ShortcutPathBox.Text;
            if (appPath == "" || shortcutPath == "")
            {
                MessageBox.Show("To remove the customization, the desktop app's path and its shortcut's path should be provided", 
                    "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                var maker = new TileMaker(appPath, shortcutPath);
                maker.RemoveCustomization();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            MessageBox.Show("The original Tile has been recovered", "Calm down", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
