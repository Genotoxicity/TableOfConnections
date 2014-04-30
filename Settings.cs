using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KSPE3Lib;

namespace KSP.E3.TableOfConnections
{
    [Serializable]
    class Settings
    {
        public int electricSchemeTypeCode;
        public string sheetTypeAttribute;
        public string firstPageFormat;
        public string subsequentPageFormat;
        public string separatingSymbols;
        public E3Font lineFont;
        public E3Font headerFont;
        public double verticalLineWidth;
        public double horizontalLineWidth;
        public double headerVerticalLineWidth;
        public double headerHorizontalLineWidth;
        public double bottomLineWidth;
        public double firstPageHeaderLineX;
        public double firstPageHeaderLineY;
        public double firstPageUttermostPositionY;
        public double subsequentPageHeaderLineX;
        public double subsequentPageHeaderLineY;
        public double subsequentPageUttermostPositionY;
    }
}
