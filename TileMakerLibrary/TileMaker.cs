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
        private readonly string _desktopAppPath;
        private readonly StartMenuTile _tile = new StartMenuTile();
        private string _assetsPath;
        private string _manifestPath;
        private string _desktopName;


        public TileMaker(string appPath, string shortcutPath = "")
        {
            if (File.Exists(appPath))
                _desktopAppPath = appPath;
            else
                _desktopAppPath = Utilities.GetTargetPath(shortcutPath);
            if (File.Exists(_desktopAppPath))
                Initialise();
            else
                throw new ArgumentException("Invalid desktop app's path");
        }

        private void Initialise()
        {
            var file = new FileInfo(_desktopAppPath);
            var dir = file.Directory;
            dir.CreateSubdirectory(_asseteDirName);
            _assetsPath = Path.Combine(dir.FullName, _asseteDirName);
            _desktopName = Path.GetFileNameWithoutExtension(file.Name);
            _manifestPath = Path.Combine(dir.FullName, $"{_desktopName}.VisualElementsManifest.xml");
        }

        public void SetSquareLogo(string path)
        {
            // full path of the logo's imgae file
            SetSquare150x150Logo(path);
            SetSquare70x70Logo(path);            
        }

        public void SetSquare150x150Logo(string path)
        {
            string name = "150x150Logo.png";
            File.Copy(path, Path.Combine(_assetsPath, name), true);
            _tile.Square150x150Logo = $"{_asseteDirName}\\{name}";
        }

        public void SetSquare70x70Logo(string path)
        {
            string name = "70x70Logo.png";
            File.Copy(path, Path.Combine(_assetsPath, name), true);
            _tile.Square70x70Logo = $"{_asseteDirName}\\{name}";
        }

        public void SetShowNameOnSquare150x150Logo(bool show)
        {
            _tile.SetShowNameOnSquare150x150Logo(show);
        }

        public void SetForegroundColor(bool isLight)
        {
            _tile.SetForegroundColor(isLight);
        }

        public void MakeTile(string path = "")
        {
            WriteManifest();
            CreateShortcut(path);
        }

        public void WriteManifest()
        {
            if (File.Exists(_manifestPath))
            {
                var bak = Path.ChangeExtension(_manifestPath, ".xml-wtm");
                if (!File.Exists(bak))
                    File.Copy(_manifestPath, bak, true);
            }
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

        public void CreateShortcut(string path = "")
        {
            if (path == "")
            {
                string commonStartMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);
                path = Path.Combine(commonStartMenuPath, "Programs", _desktopName + ".lnk");
            }
            if (!File.Exists(path))
            {
                var shell = new IWshRuntimeLibrary.WshShell();
                var shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(path);                
                string pathToExe = new FileInfo(_desktopAppPath).FullName;
                shortcut.TargetPath = pathToExe;
                shortcut.Save();
            }
            else
                UpdateShortcut(path);
        }

        public void RemoveCustomization(string shortcutPath)
        {
            var bakFile = Path.ChangeExtension(_manifestPath, ".xml-wtm");
            if (File.Exists(bakFile))
            {
                File.Copy(bakFile, _manifestPath, true);
                File.Delete(bakFile);
            }
            if (Directory.Exists(_assetsPath))
                Directory.Delete(_assetsPath, true);
            UpdateShortcut(shortcutPath);
        }

        private void UpdateShortcut(string path)
        {
            File.SetLastWriteTime(path, DateTime.Now);
        }
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
                shortcut.Save();
            }
            return ret;
        }
    }
}
