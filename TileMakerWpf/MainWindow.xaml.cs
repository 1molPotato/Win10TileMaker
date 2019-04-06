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
                var maker = new TileMaker(AppPathBox.Text);
                if (Image150x150Logo.Source != null && Image70x70Logo.Source != null)
                {
                    maker.SetSquare150x150Logo(((BitmapImage)Image150x150Logo.Source).UriSource.LocalPath);
                    maker.SetSquare70x70Logo(((BitmapImage)Image70x70Logo.Source).UriSource.LocalPath);
                }
                else if (Image150x150Logo.Source != null)
                    maker.SetSquareLogo(((BitmapImage)Image150x150Logo.Source).UriSource.LocalPath);
                else
                    maker.SetSquareLogo(((BitmapImage)Image70x70Logo.Source).UriSource.LocalPath);
                maker.SetShowNameOnSquare150x150Logo((bool)ShowNameCheckBox.IsChecked);
                maker.SetForegroundColor(ForegroundColorComboBox.SelectedIndex == 0 ? true : false);
                maker.MakeTile(ShortcutPathBox.Text);
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
            Image150x150Logo.Source = LoadImageFromOpenFileDialog();
        }

        private void Load70x70LogoButton_Click(object sender, RoutedEventArgs e)
        {
            Image70x70Logo.Source = LoadImageFromOpenFileDialog();
        }

        private BitmapImage LoadImageFromOpenFileDialog()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpg;*.jpeg;*.gif)|*.png;*.jpg;*.jpeg;*.gif"
            };
            if (dialog.ShowDialog() == true)
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(dialog.FileName);
                bitmap.EndInit();
                return bitmap;
            }            
            return null;
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
                ShortcutPathBox.Text = dialog.FileName;            
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
                var maker = new TileMaker(appPath);
                maker.RemoveCustomization(shortcutPath);
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
