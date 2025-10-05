using System;
using System.Collections.Generic;
using GS.ICharts;
using GS.Trade.Data.Chart;

namespace GS.Trade.Data.Studies.GS.xMa018
{
    public partial class Xma018 : TimeSeries //, IBandSeries, IBandStudy
    {
        private readonly List<ILevel> _levelCollection = new List<ILevel>();
        public IList<ILevel> GetLevelCollection(string s)
        {
            if (Count > 1)
            {
                Level l;
                _levelCollection.Clear();
                /*
                l = new Level{ Value = ((Item) this[1]).High, Color = 0x0000ff, Text = "High"};
                _levelCollection.Add(l);
                l = new Level { Value = ((Item)this[1]).Low, Color = 0xff0000, Text = "Low" };
                _levelCollection.Add(l);
                */
                // BackGroundColor = 0x8ed8ff;
                // l = new Level { Value = ((Item)this[1]).High2, Color = 0x000000, BackGroundColor = 0xb6cdff, Text = "High2" };
                l = new Level { Value = ((Item)this[1]).High2, Color = 0x0000ff, BackGroundColor = 0xffffff, Text = "High2" };
                _levelCollection.Add(l);
                //l = new Level { Value = ((Item)this[1]).Low2, Color = 0x000000, BackGroundColor =  0xffc2bd, Text = "Low2" };
                l = new Level { Value = ((Item)this[1]).Low2, Color = 0xff0000, BackGroundColor = 0xffffff, Text = "Low2" };
                _levelCollection.Add(l);
               // l = new Level { Value = ((Item)this[1]).High3, Color = 0x0008ff, BackGroundColor = 0xffffff, Text = "High3" };
               // _levelCollection.Add(l);
               // l = new Level { Value = ((Item)this[1]).Low3, Color = 0xff0800, BackGroundColor = 0xffffff, Text = "Low3" };
               // _levelCollection.Add(l);
                l = new Level { Value = _prevTrendLastHigh, Color = 0x0008ff, BackGroundColor = 0xffffff, Text = "PrevH1" };
                _levelCollection.Add(l);
                l = new Level { Value = _prevTrendLastLow, Color = 0xff0800, BackGroundColor = 0xffffff, Text = "PrevL1" };
                _levelCollection.Add(l);
               // l = new Level { Value = _prevTrendLastHigh2, Color = 0x0008ff, BackGroundColor = 0xffffff, Text = "PrevH2" };
               // _levelCollection.Add(l);
               // l = new Level { Value = _prevTrendLastLow2, Color = 0xff0800, BackGroundColor = 0xffffff, Text = "PrevL2" };
               // _levelCollection.Add(l);
                l = new Level { Value = TrHigh1, Color = 0x0000ff, BackGroundColor = 0xffffff, Text = "TrHigh1" };
                _levelCollection.Add(l);

                l = new Level { Value = TrLow1, Color = 0xff0000, BackGroundColor = 0xffffff, Text = "TrLow1" };
                _levelCollection.Add(l);

                return _levelCollection;
            }
            return null;
        }
        
        public double GetLine(int i)
        {
            return ((Item)this[i]).Ma;
        }
        public double GetLine()
        {
            return ((Item)this[1]).Ma;
        }

        public double GetHigh(int i)
        {
            return ((Item)this[i]).High;
        }
        public double GetLow(int i)
        {
            return ((Item)this[i]).Low;
        }

        public double GetHigh2()
        {
            return ((Item)this[1]).High2;
        }
        public double GetLow2()
        {
            return ((Item)this[1]).Low2;
        }
        public double GetHigh3()
        {
            return ((Item)this[1]).High3;
        }
        public double GetLow3()
        {
            return ((Item)this[1]).Low3;
        }
       
        public override IList<ILineSeries> ChartLines { get { return CreateChartLineList(); } }
        private IList<ILineSeries> CreateChartLineList()
        {
            return new List<ILineSeries>
                       {
                           new ChartLine
                               {
                                   Name = "L.x18",
                                   Color = 0x0000ff,
                                   CallGetCount = GetCount,
                                   CallGetDateTime = GetDateTime,
                                   CallGetLine = GetLine
                               }
                       };
        }

        public override IList<IBandSeries> ChartBands { get { return CreateChartBandList(); } }
        private IList<IBandSeries> CreateChartBandList()
        {
            return new List<IBandSeries>
                       {
                           new ChartBand
                               {
                                   BandName="B.x18",
                                   BandColor = 0x9999ff,
                                   BandLineColor = 0x000ff,
                                   BandFillColor = unchecked((int)0xc06666ff),
                                   CallGetCount = GetCount,
                                   CallGetDateTime = GetDateTime,
                                   CallGetLine = GetLine,
                                   CallGetHigh = GetHigh,
                                   CallGetLow = GetLow
                               }
                       };
        }

        private IList<ILevel> _chartLevels;
        public override IList<ILevel> ChartLevels
        {
            get { return _chartLevels ?? CreateChartLevels(); }
        }
        public IList<ILevel> CreateChartLevels()
        {
            var ll = new List<ILevel>
                               {
                                   new Level
                                       {
                                           Color = 0xff0000,
                                           BackGroundColor = 0xffffff,
                                           Text = "Low2",
                                           GetValue = GetLow2,
                                           IsValid = () => GetCount() > 1
                                       },
                                   new Level
                                       {
                                           Color = 0x0000ff,
                                           BackGroundColor = 0xffffff,
                                           Text = "High2",
                                           GetValue = GetHigh2,
                                           IsValid = () => GetCount() > 1
                                       },
                                   new Level
                                       {
                                           Color = 0xff0000,
                                           BackGroundColor = 0xffffff,
                                           LineWidth = 3,
                                           Text = "TrLow1",
                                           GetValue = () => TrLow1 ,
                                           IsValid = () => GetCount() > 1
                                       },
                                   new Level
                                       {
                                           Color = 0x0000ff,
                                           BackGroundColor = 0xffffff,
                                           LineWidth = 3,
                                           Text = "TrHigh1",
                                           GetValue = () => TrHigh1 ,
                                           IsValid = () => GetCount() > 1
                                       }
                                   // ,
                                   //new Level
                                   //    {
                                   //        Color = 0xff0000,
                                   //        BackGroundColor = 0xffffff,
                                   //        LineWidth = 3,
                                   //        Text = "TrLow2",
                                   //        GetValue = () => TrLow2 ,
                                   //        IsValid = () => GetCount() > 1
                                   //    },
                                   //new Level
                                   //    {
                                   //        Color = 0x0000ff,
                                   //        BackGroundColor = 0xffffff,
                                   //        LineWidth = 3,
                                   //        Text = "TrHigh2",
                                   //        GetValue = () => TrHigh2 ,
                                   //        IsValid = () => GetCount() > 1
                                   //    }
                                       // ,
                                       /*
                                       new Level
                                       {
                                           Color = 0xff0000,
                                           BackGroundColor = 0xffffff,
                                           Text = "Low3",
                                           GetValue = GetLow3,
                                           IsValid = () => GetCount() > 1
                                       },
                                   new Level
                                       {
                                           Color = 0x0000ff,
                                           BackGroundColor = 0xffffff,
                                           Text = "High3",
                                           GetValue = GetHigh3,
                                           IsValid = () => GetCount() > 1
                                       }
                                       */
                                       /*
                                       ,
                                       new Level
                                       {
                                           Color = 0x000000,
                                           BackGroundColor = 0xffffff,
                                           Text = "(H+L)/2",
                                           GetValue = () => (High2 + Low2)/2,
                                           IsValid = () => GetCount() > 1
                                       }
                                       */
                                       /*
                                       ,
                                       new Level
                                       {
                                           Color = 0x0000ff,
                                           BackGroundColor = 0xffffff,
                                           GetText = () => string.Format("Trend2={0}", _trend2 > 0 ? "Up" : _trend2 < 0 ? "Down" : "Neutral" ),
                                           GetValue = GetLine,
                                           IsValid = () => GetCount() > 1
                                       }
                                       */
                               };
            _chartLevels = ll;
            return _chartLevels;
        }

        private IList<IChartText> _chartTexts;
        public override IList<IChartText> ChartTexts
        {
            get { return _chartTexts ?? CreateChartTexts(); }
        }
        public IList<IChartText> CreateChartTexts()
        {
            var ll = new List<IChartText>
                               {
                                    new ChartText
                                       {
                                           Color = 0x0,
                                           GetBackGroundColor = () => unchecked((int)0xa0ff0000),
                                           Header = "DT:",
                                           GetText = () => $" {CurrentDateTime:G}",
                                           IsValid = () => GetCount() > 1
                                       },
                                   //new ChartText
                                   //    {
                                   //        Color = 0x0,
                                   //        GetBackGroundColor = () => FlatCount != 0 ? unchecked((int)0xa00000ff) :  unchecked((int)0xa0ff0000),
                                   //        Header = "FlatCnt:",
                                   //        GetText = () => string.Format(" {0}", FlatCount ),
                                   //        IsValid = () => GetCount() > 1
                                   //    },
                                   new ChartText
                                       {
                                           Color = 0x0,
                                           GetBackGroundColor = () => SwingCount != 0 ? unchecked((int)0xa00000ff) :  unchecked((int)0xa0ff0000),
                                           Header = "18SwCnt:",
                                           GetText = () => $" {SwingCount}",
                                           IsValid = () => GetCount() > 1
                                       },
                                   new ChartText
                                       {
                                           Color = 0x0,
                                           GetBackGroundColor = () => Trend > 0 ? unchecked((int)0xa00000ff) : Trend < 0 ? unchecked((int)0xa0ff0000) : 0x6666ff,
                                           Header = "KAtr:",
                                           GetText = () => $" {KAtr.ToString("G")}",
                                           IsValid = () => GetCount() > 1
                                       },
                                   new ChartText
                                       {
                                           Color = 0x0,
                                           GetBackGroundColor = () => Trend > 0 ? unchecked((int)0xa00000ff) : Trend < 0 ? unchecked((int)0xa0ff0000) : 0x6666ff, 
                                           Header = "Trend:",
                                           GetText = () => $" {(Trend > 0 ? "UP" : Trend < 0 ? "DOWN" : "FLAT")}",
                                           IsValid = () => GetCount() > 1
                                       },
                                   new ChartText
                                       {
                                           Color = 0x0,
                                           GetBackGroundColor = () => _trend2 > 0 ? unchecked((int)0xa00000ff) : _trend2 < 0 ? unchecked((int)0xa0ff0000) : unchecked((int)0xa06666ff),
                                           Header = "Trend2:",
                                           GetText = () => $" {(_trend2 > 0 ? "UP" : _trend2 < 0 ? "DOWN" : "FLAT")}",
                                           IsValid = () => GetCount() > 1
                                       },
                                       new ChartText
                                       {
                                           Color = 0x0,
                                           GetBackGroundColor = () => Trend5 > 0 ? unchecked((int)0xa00000ff) : Trend5 < 0 ? unchecked((int)0xa0ff0000) : unchecked((int)0xa06666ff),
                                           Header = "Trend5:",
                                           GetText = () => $" {(Trend5 > 0 ? "UP" : Trend5 < 0 ? "DOWN" : "FLAT")}",
                                           IsValid = () => GetCount() > 1
                                       },
                                       new ChartText
                                       {
                                           Color = 0x0,
                                           GetBackGroundColor = () => IsTrend55Up ? unchecked((int)0xa00000ff) : IsTrend55Down ? unchecked((int)0xa0ff0000) : unchecked((int)0xa06666ff),
                                           Header = "Trend55:",
                                           GetText = () =>
                                               $" {(IsTrend55Up ? "UP" : IsTrend55Down ? "DOWN" : "FLAT")}",
                                           IsValid = () => GetCount() > 1
                                       },
                                       new ChartText
                                       {
                                           Color = 0x0,
                                           GetBackGroundColor = () => Trend55Changed  ? unchecked((int)0xa00000ff) :  unchecked((int)0xa0ff0000),
                                           Header = "Trend55Changed:",
                                           GetText = () => $" {(Trend55Changed ? "True" : "False")}",
                                           IsValid = () => GetCount() > 1
                                       },
                                       new ChartText
                                       {
                                           Color = 0x0,
                                           GetBackGroundColor = () => TrendHigh5 > 0 ? unchecked((int)0xa00000ff) : TrendHigh5 < 0 ? unchecked((int)0xa0ff0000) : unchecked((int)0xa06666ff),
                                           Header = "TrendHigh5:",
                                           GetText = () =>
                                               $" {(TrendHigh5 > 0 ? "UP" : TrendHigh5 < 0 ? "DOWN" : "FLAT")}",
                                           IsValid = () => GetCount() > 1
                                       },
                                       new ChartText
                                       {
                                           Color = 0x0,
                                           GetBackGroundColor = () => TrendLow5 > 0 ? unchecked((int)0xa00000ff) : TrendLow5 < 0 ? unchecked((int)0xa0ff0000) : unchecked((int)0xa06666ff),
                                           Header = "TrendLow5:",
                                           GetText = () =>
                                               $" {(TrendLow5 > 0 ? "UP" : TrendLow5 < 0 ? "DOWN" : "FLAT")}",
                                           IsValid = () => GetCount() > 1
                                       }
                                      // ,
                                      

                                       //new ChartText
                                       //{
                                       //    Color = 0x0,
                                       //    GetBackGroundColor = () => TrPos1 > 100 ? unchecked((int)0xa00000ff) : TrPos1 < 0 ? unchecked((int)0xa0ff0000) : unchecked((int)0xa06666ff),
                                       //    Header = "TrPos1:",
                                       //    GetText = () => string.Format(" {0:N2}", TrPos1 ),
                                       //    IsValid = () => GetCount() > 1
                                       //},
                                       //new ChartText
                                       //{
                                       //    Color = 0x0,
                                       //    GetBackGroundColor = () => TrPos2 > 100 ? unchecked((int)0xa00000ff) : TrPos2 < 0 ? unchecked((int)0xa0ff0000) : unchecked((int)0xa06666ff),
                                       //    Header = "TrPos2:",
                                       //    GetText = () => string.Format(" {0:N2}", TrPos2 ),
                                       //    IsValid = () => GetCount() > 1
                                       //},
                                       // new ChartText
                                       //{
                                       //    Color = 0x0,
                                       //    GetBackGroundColor = () => TrPos12 > 100 ? unchecked((int)0xa00000ff) : TrPos12 < 0 ? unchecked((int)0xa0ff0000) : unchecked((int)0xa06666ff),
                                       //    Header = "TrPos12:",
                                       //    GetText = () => string.Format(" {0:N2}", TrPos12 ),
                                       //    IsValid = () => GetCount() > 1
                                       //},
                                       //new ChartText
                                       //{
                                       //    Color = 0xffffff,
                                       //    BackGroundColor =  unchecked((int)0xa00000ff),
                                       //    Header = "TrHigh1",
                                       //    GetText = () => string.Format(" {0:N2}", TrHigh1 ),
                                       //    IsValid = () => GetCount() > 1
                                       //},
                                       //new ChartText
                                       //{
                                       //    Color = 0x0,
                                       //    BackGroundColor = unchecked((int)0xa0ff0000),
                                       //    Header = "TrLow1",
                                       //    GetText = () => string.Format(" {0:N2}", TrLow1 ),
                                       //    IsValid = () => GetCount() > 1
                                       //},
                                       //new ChartText
                                       //{
                                       //    Color = 0xffffff,
                                       //    BackGroundColor =  unchecked((int)0xa00000ff),
                                       //    Header = "TrHigh2",
                                       //    GetText = () => string.Format(" {0:N2}", TrHigh2 ),
                                       //    IsValid = () => GetCount() > 1
                                       //},
                                       //new ChartText
                                       //{
                                       //    Color = 0x0,
                                       //    BackGroundColor = unchecked((int)0xa0ff0000),
                                       //    Header = "TrLow2",
                                       //    GetText = () => string.Format(" {0:N2}", TrLow2 ),
                                       //    IsValid = () => GetCount() > 1
                                       //},
                                       //new ChartText
                                       //{
                                       //    Color = 0x0,
                                       //    GetBackGroundColor = () => IsTrend09Up ? unchecked((int)0xa00000ff) : (IsTrend09Down ? unchecked((int)0xa0ff0000) : unchecked((int)0xa06666ff)),
                                       //    Header = "Tr9:",
                                       //    GetText = () => $" {Trend09}",
                                       //    IsValid = () => GetCount() > 1
                                       //}


                                       /*
                                       ,
                                       new ChartText
                                       {
                                           Color = 0xffffff,
                                           GetBackGroundColor = () => Glue ? unchecked((int)0xa00000ff) : unchecked((int) 0xa0ff0000),
                                           Header = "GlueCrnt:",
                                           GetText = () => string.Format(" {0}", Glue ? "ON" : "OFF" ),
                                           IsValid = () => GetCount() > 1
                                       }
                                       ,
                                       new ChartText
                                       {
                                           Color = 0xffffff,
                                           GetBackGroundColor = () => PrevGlue ? unchecked((int)0xa00000ff) :  unchecked((int)0xa0ff0000),
                                           Header = "GluePrev:",
                                           GetText = () => string.Format(" {0}", PrevGlue ? "ON" : "OFF" ),
                                           IsValid = () => GetCount() > 1
                                       }
                                       */

                               };
            _chartTexts = ll;
            return _chartTexts;
        }

    }
}
