using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileMakerLibrary
{
    class StartMenuTile
    {
        private bool _showNameOnSquare150x150Logo;
        private bool _foregroundTextLight;
        public string Square150x150Logo { get; set; }
        public string Square70x70Logo { get; set; }
        public string BackgroundColor { get; set; }
        public string ShowNameOnSquare150x150Logo
        {
            get
            {
                return _showNameOnSquare150x150Logo ? "on" : "off";
            }
        }
        public string ForeGroudText
        {
            get
            {
                return _foregroundTextLight ? "light" : "dark";
            }
        }
        public StartMenuTile()
        {
            BackgroundColor = "#2D2D30";
            _showNameOnSquare150x150Logo = true;
            _foregroundTextLight = true;
        }
        public void SetShowNameOnSquare150x150Logo(bool show)
        {
            _showNameOnSquare150x150Logo = show;
        }
        public void SetForegroundColor(bool isLight)
        {
            _foregroundTextLight = isLight;
        }
    }
}
