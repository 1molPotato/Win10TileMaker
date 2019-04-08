using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace TileMakerLibrary
{
    public class TileMaker
    {
        private readonly string _asseteDirName = "Win10TileMaker_Assets";
        private readonly string _image150x150Name = "150x150Logo.png";
        private readonly string _image70x70Name = "70x70Logo.png";
        private readonly string _desktopAppPath;        
        private readonly StartMenuTile _tile = new StartMenuTile();
        private string _assetsPath;
        private string _manifestPath;
        private string _shortcutPath;
        private string _desktopName;
        private string _image150x150Path;
        private string _image70x70Path;


        public TileMaker(string appPath, string shortcutPath = "")
        {
            _shortcutPath = shortcutPath;
            if (File.Exists(appPath))
                _desktopAppPath = appPath;
            else
                _desktopAppPath = Utilities.GetTargetPath(_shortcutPath);
            if (File.Exists(_desktopAppPath))
                Initialise();
            else
                throw new ArgumentException("Invalid desktop app's path");
        }

        private void Initialise()
        {
            var file = new FileInfo(_desktopAppPath);
            var dir = file.Directory;
            //dir.CreateSubdirectory(_asseteDirName);
            _assetsPath = Path.Combine(dir.FullName, _asseteDirName);
            _desktopName = Path.GetFileNameWithoutExtension(file.Name);
            _manifestPath = Path.Combine(dir.FullName, $"{_desktopName}.VisualElementsManifest.xml");
        }

        #region Interfaces

        public void SetSquareLogo(string path)
        {
            // full path of the logo's imgae file
            SetSquare150x150Logo(path);
            SetSquare70x70Logo(path);            
        }

        public void SetSquare150x150Logo(string path)
        {
            _image150x150Path = path;
            //File.Copy(path, Path.Combine(_assetsPath, name), true);
            _tile.Square150x150Logo = $"{_asseteDirName}\\{_image150x150Name}";
        }

        public void SetSquare70x70Logo(string path)
        {
            _image70x70Path = path;
            //File.Copy(path, Path.Combine(_assetsPath, name), true);
            _tile.Square70x70Logo = $"{_asseteDirName}\\{_image70x70Name}";
        }

        public void SetShowNameOnSquare150x150Logo(bool show)
        {
            _tile.SetShowNameOnSquare150x150Logo(show);
        }

        public void SetForegroundColor(bool isLight)
        {
            _tile.SetForegroundColor(isLight);
        }

        public void MakeTile()
        {
            CreateBackup();
            CreateAssets();
            CreateManifest();
            CreateShortcut();
        }

        public void RemoveCustomization()
        {
            var bakFile = Path.ChangeExtension(_manifestPath, ".xml-wtm");
            if (File.Exists(bakFile))
            {
                File.Copy(bakFile, _manifestPath, true);
                File.Delete(bakFile);
            }
            else
            {
                if (Directory.Exists(_assetsPath)/* && Directory.GetFiles(_assetsPath).Length != 0*/)
                    File.Delete(_manifestPath);
            }
            if (Directory.Exists(_assetsPath))
                Directory.Delete(_assetsPath, true);
            UpdateShortcut(_shortcutPath);
        }
        #endregion

        #region Utilities

        private void CreateBackup()
        {
            var bakFile = Path.ChangeExtension(_manifestPath, ".xml-wtm");
            if (!Directory.Exists(_assetsPath) && File.Exists(_manifestPath) && !File.Exists(bakFile))
                File.Move(_manifestPath, bakFile);
        }

        private void CreateAssets()
        {
            var dir = Directory.CreateDirectory(_assetsPath);
            File.Copy(_image150x150Path, Path.Combine(_assetsPath, _image150x150Name), true);
            File.Copy(_image70x70Path, Path.Combine(_assetsPath, _image70x70Name), true);
        }        

        private void CreateManifest()
        {            
            var settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "\t",
                NewLineOnAttributes = true
            };
            using (var writer = XmlWriter.Create(_manifestPath, settings))
            {
                writer.WriteStartElement("Application");
                writer.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
                writer.WriteStartElement("VisualElements");
                writer.WriteAttributeString("BackgroundColor", _tile.BackgroundColor);
                writer.WriteAttributeString("ShowNameOnSquare150x150Logo", _tile.ShowNameOnSquare150x150Logo);
                writer.WriteAttributeString("ForegroundText", _tile.ForeGroudText);
                if (_tile.Square150x150Logo != null && _tile.Square150x150Logo != "")
                    writer.WriteAttributeString("Square150x150Logo", _tile.Square150x150Logo);
                if (_tile.Square70x70Logo != null && _tile.Square70x70Logo != "")
                    writer.WriteAttributeString("Square70x70Logo", _tile.Square70x70Logo);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }

        private void CreateShortcut()
        {
            if (_shortcutPath == "")
            {
                string commonStartMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);
                _shortcutPath = Path.Combine(commonStartMenuPath, "Programs", _desktopName + ".lnk");
            }
            if (!File.Exists(_shortcutPath))
            {
                var shell = new IWshRuntimeLibrary.WshShell();
                var shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(_shortcutPath);                
                string pathToExe = new FileInfo(_desktopAppPath).FullName;
                shortcut.TargetPath = pathToExe;
                shortcut.Save();
            }
            else
                UpdateShortcut(_shortcutPath);
        }

        private void UpdateShortcut(string path)
        {
            File.SetLastWriteTime(path, DateTime.Now);
        }

        #endregion
    }

    public class Utilities
    {
        public static string GetTargetPath(string shortcutPath)
        {
            string ret = "";
            if (File.Exists(shortcutPath))
            {
                var shell = new IWshRuntimeLibrary.WshShell();
                var shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(shortcutPath);
                ret = shortcut.TargetPath;
                //shortcut.Save();
            }
            return ret;
        }
    }
}
