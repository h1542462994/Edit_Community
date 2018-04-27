using System;
using System.Windows;
using System.Windows.Media;
using User.SoftWare;
using User.SoftWare.Service;

namespace Edit_Community
{
    /// <summary>
    /// 本地设置,只用于实例方法.
    /// </summary>
    public sealed class Local
    {
        public USettings uSettings = new USettings(AppData.LocalFolder, "Settings");
        public readonly USettingsProperty<bool> IsFullScreenProperty;
        readonly USettingsProperty<Size> AppSizeProperty;
        readonly USettingsProperty<Point> AppLocationProperty;
        readonly USettingsProperty<bool> IsMaxShowProperty;
        public readonly USettingsProperty<Color> EditBackgroundColorProperty;
        readonly USettingsProperty<Color> EditBackgroundColorOldproperty;
        readonly USettingsProperty<double> ColumnDefiMinProperty;
        readonly USettingsProperty<double> RowDefiMinProperty;
        public readonly USettingsProperty<Color[]> EditColorProperty;
        public readonly USettingsProperty<Color[]> EditBackgroundColorHistoryProperty;
        readonly USettingsProperty<int> ExitEditIntervalProperty;
        public readonly USettingsProperty<Color[]> EditColorHistoryProperty;
        public readonly USettingsProperty<bool> IsEditBrushOpenProperty;
        public readonly USettingsProperty<bool> IsRtxHiddenProperty;
        public readonly USettingsProperty<int> InkColorIndexProperty;
        public readonly USettingsProperty<double> InkPenWidthProperty;
        public readonly USettingsProperty<int> BackgroundModeProperty;
        readonly USettingsProperty<string> BackgroundPicPathProperty;
        readonly USettingsProperty<string> BackgroundPicFolderProperty;
        readonly USettingsProperty<double> BackgroundPicTimestampProperty;
        readonly USettingsProperty<DateTime> BackgroundPicLastTimeProperty;
        readonly USettingsProperty<int> BackgroundPicCurrentindexProperty;
        public readonly USettingsProperty<bool> WeatherisOpenProperty;
        public readonly USettingsProperty<string> WeathercityProperty;
        readonly USettingsProperty<double> WeatherTimestampProperty;
        readonly USettingsProperty<DateTime> WeatherLastTimeProperty;
        public readonly USettingsProperty<bool> CheckisOpenProperty;
        readonly USettingsProperty<AutoCheckCollection> CheckDataProperty;
        public readonly USettingsProperty<bool> IsAutoUpdateProperty;
        readonly USettingsProperty<double> UpdateTimestampProperty;
        readonly USettingsProperty<DateTime> UpdateLastTimeProperty;
        readonly USettingsProperty<DateTime> NoticeLastTimeProperty;
        readonly USettingsProperty<bool> AllowTranspancyProperty;
        public Local()
        {
            IsFullScreenProperty = uSettings.Register("isFullScreen", false, true);
            AppSizeProperty = uSettings.Register("appSize", new Size(0.7, 0.7));
            AppLocationProperty = uSettings.Register("appLocation", new Point(0.15, 0.15));
            IsMaxShowProperty = uSettings.Register("isMaxShow", false);
            EditBackgroundColorProperty = uSettings.Register("editBackgroundColor", Color.FromRgb(20, 32, 0), true);
            EditBackgroundColorOldproperty = uSettings.Register("editBackgroundColorOld", Color.FromRgb(20, 32, 0));
            ColumnDefiMinProperty = uSettings.Register("columnDefiMin", 0.0);
            RowDefiMinProperty = uSettings.Register("rowDefiMin", 0.0);
            EditColorProperty = uSettings.Register("editcolor", new Color[]{
                Color.FromRgb(255,172,17),
                Color.FromRgb(253,99,40),
                Color.FromRgb(205,119,251),
                Color.FromRgb(1,199,252),
                Color.FromRgb(12,116,102),
                Colors.Chocolate,
                Color.FromRgb(12,234,145),
                Colors.White}, true);
            EditBackgroundColorHistoryProperty = uSettings.Register("editBackgroundColorHistory", new Color[0]);
            ExitEditIntervalProperty = uSettings.Register("exitEditInterval", 12);
            EditColorHistoryProperty = uSettings.Register("editcolorHistory", new Color[0], true);
            IsEditBrushOpenProperty = uSettings.Register("isEditBrushOpen", false, true);
            IsRtxHiddenProperty = uSettings.Register("isRtxHidden", false, true);
            InkColorIndexProperty = uSettings.Register("inkColorIndex", 0);
            InkPenWidthProperty = uSettings.Register("inkPenWidth", 4.0);
            BackgroundModeProperty = uSettings.Register("BackgroundMode", 0, true);
            BackgroundPicPathProperty = uSettings.Register("BackgroundPicPath", "");
            BackgroundPicFolderProperty = uSettings.Register("BackgroundPicFolderPath", "");
            BackgroundPicTimestampProperty = uSettings.Register("BackgroundPicTimestamp", 15.0);
            BackgroundPicLastTimeProperty = uSettings.Register("BackgroundPicLastTime", new DateTime());
            BackgroundPicCurrentindexProperty = uSettings.Register("BackgroundPicCurrentindex", 0);
            WeatherisOpenProperty = uSettings.Register("WeatherisOpen", false, true);
            WeathercityProperty = uSettings.Register("Weathercity", "杭州", true);
            WeatherTimestampProperty = uSettings.Register("WeatherTimestamp", 120.0);
            WeatherLastTimeProperty = uSettings.Register("WeatherLastTime", new DateTime());
            CheckisOpenProperty = uSettings.Register("CheckisOpen", true,true);
            CheckDataProperty = uSettings.Register("CheckData", new AutoCheckCollection() { new AutoCheck("ZWY") { Num = 100 } });
            IsAutoUpdateProperty = uSettings.Register("IsAutoUpdate", true, true);
            UpdateTimestampProperty = uSettings.Register("UpdateTimestamp", 120.0);
            UpdateLastTimeProperty = uSettings.Register("UpdateLastTime", new DateTime());
            NoticeLastTimeProperty = uSettings.Register("NoticeLastTime", new DateTime());
            AllowTranspancyProperty = uSettings.Register("AllowTranspancy", false);
        }
        public readonly Color[] EditBackgroundColorDefault = new Color[] { Color.FromRgb(20, 30, 0), Color.FromRgb(16, 28, 58), Color.FromRgb(44, 44, 44), Color.FromRgb(54, 54, 8) };
        /// <summary>
        /// 是否为全屏模式.
        /// </summary>
        public bool IsFullScreen { get => IsFullScreenProperty.Value; set => IsFullScreenProperty.Value = value; }
        /// <summary>
        /// Application的大小,不实时更新.
        /// </summary>
        public Size AppSize { get => AppSizeProperty.Value; set => AppSizeProperty.Value = value; }
        /// <summary>
        /// Application的位置,不实时更新.
        /// </summary>
        public Point AppLocation { get => AppLocationProperty.Value; set => AppLocationProperty.Value = value; }
        /// <summary>
        /// 是否是最大化模式,不实时更新.
        /// </summary>
        public bool IsMaxShow { get => IsMaxShowProperty.Value; set => IsMaxShowProperty.Value = value; }
        /// <summary>
        /// Edit的背景颜色
        /// </summary>
        public Color EditBackgroundColor { get => EditBackgroundColorProperty.Value; set => EditBackgroundColorProperty.Value = value; }
        public Color EditBackgroundColorOld { get => EditBackgroundColorOldproperty.Value; set => EditBackgroundColorOldproperty.Value = value; }
        public double ColumnDefiMin { get => ColumnDefiMinProperty.Value; set => ColumnDefiMinProperty.Value = value; }
        public double RowDefiMin { get => RowDefiMinProperty.Value; set => RowDefiMinProperty.Value = value; }
        public Color[] Editcolor { get => EditColorProperty.Value; set => EditColorProperty.Value = value; }
        public Color[] EditBackgroundColorHistory { get => EditBackgroundColorHistoryProperty.Value; set => EditBackgroundColorHistoryProperty.Value = value; }
        public int ExitEditInterval { get => ExitEditIntervalProperty.Value; set => ExitEditIntervalProperty.Value = value; }
        public Color[] EditcolorHistory { get => EditColorHistoryProperty.Value; set => EditColorHistoryProperty.Value = value; }
        public bool IsEditBrushOpen { get => IsEditBrushOpenProperty.Value; set => IsEditBrushOpenProperty.Value = value; }
        public bool IsRtxHidden { get => IsRtxHiddenProperty.Value; set => IsRtxHiddenProperty.Value = value; }
        public int BackgroundMode { get => BackgroundModeProperty.Value; set => BackgroundModeProperty.Value = value; }
        public string BackgroundPicPath { get => BackgroundPicPathProperty.Value; set => BackgroundPicPathProperty.Value = value; }
        public string BackgroundPicFolder { get => BackgroundPicFolderProperty.Value; set => BackgroundPicFolderProperty.Value = value; }
        public double BackgroundPicTimestamp { get => BackgroundPicTimestampProperty.Value; set => BackgroundPicTimestampProperty.Value = value; }
        public DateTime BackgroundPicLastTime { get => BackgroundPicLastTimeProperty.Value; set => BackgroundPicLastTimeProperty.Value = value; }
        public int BackgroundPicCurrentindex { get => BackgroundPicCurrentindexProperty.Value; set => BackgroundPicCurrentindexProperty.Value = value; }
        public bool WeatherisOpen { get => WeatherisOpenProperty.Value; set => WeatherisOpenProperty.Value = value; }
        public string Weathercity { get => WeathercityProperty.Value; set => WeathercityProperty.Value = value; }
        public double WeatherTimestamp { get => WeatherTimestampProperty.Value; set => WeatherTimestampProperty.Value = value; }
        public DateTime WeatherLastTime { get => WeatherLastTimeProperty.Value; set => WeatherLastTimeProperty.Value = value; }
        public bool CheckisOpen { get => CheckisOpenProperty.Value; set => CheckisOpenProperty.Value = value; }
        public AutoCheckCollection CheckData { get => CheckDataProperty.Value; set => CheckDataProperty.Value = value; }
        public bool IsAutoUpdate { get=>IsAutoUpdateProperty.Value; set=>IsAutoUpdateProperty.Value =value; }
        public double UpdateTiemstamp { get => UpdateTimestampProperty.Value; set => UpdateTimestampProperty.Value = value; }
        public DateTime UpdateLastTime { get => UpdateLastTimeProperty.Value; set => UpdateLastTimeProperty.Value = value; }
        public DateTime NoticeLastTime { get => NoticeLastTimeProperty.Value; set => NoticeLastTimeProperty.Value = value; }
        public bool AllowTranspancy { get => AllowTranspancyProperty.Value; set => AllowTranspancyProperty.Value = value; }
        public void Flush()
        {
            uSettings.USettingsChanged += AppData.MainWindow.Local_PropertyChanged;
            uSettings.Flush();
        }
    }
}
