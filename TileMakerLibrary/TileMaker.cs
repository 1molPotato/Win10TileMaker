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


        public TileMaker(string path)
        {
            _desktopAppPath = path;
            Initialise();
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
            // full path of the logo's imgae
            string name = "SquareLogo.png";
            File.Copy(path, Path.Combine(_assetsPath, name), true);
            _tile.Square150x150Logo = $"{_asseteDirName}\\{name}";
            //name = "70x70Logo.png";
            //File.Copy(path, Path.Combine(_assetsPath, name), true);
            _tile.Square70x70Logo = $"{_asseteDirName}\\{name}";
        }

        public void ShowNameOnSquare150x150Logo(bool show)
        {
            _tile.SetShowNameOnSquare150x150Logo(show);
        }

        public void MakeTile()
        {
            WriteManifest();
            UpdateShortcut();
        }

        public void WriteManifest()
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

        public void UpdateShortcut()
        {
            string commonStartMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);
            string appStartMenuPath = Path.Combine(commonStartMenuPath, "Programs", _desktopName + ".lnk");
            if (!File.Exists(appStartMenuPath))
            {
                var shell = new IWshRuntimeLibrary.WshShell();
                var shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(appStartMenuPath);
                string pathToExe = new FileInfo(_desktopAppPath).FullName;
                shortcut.TargetPath = pathToExe;
                shortcut.Save();
            }
            else
            {
                File.SetLastWriteTime(appStartMenuPath, DateTime.Now);
            }
        }
    }
}
