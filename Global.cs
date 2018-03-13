using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.IO;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Windows.Ink;
using System.Windows.Controls;
using System.Windows.Input;
using User.UI;

namespace Edit_Community
{
    /// <summary>
    /// 计时器的友好名称.
    /// </summary>
    public enum TimerDisplayName
    {
        SaveBitmap,
        ExitEdit,
        HideImg,
        HideMouse,
        Weather
    }
    /// <summary>
    /// 对话框的友好名称.
    /// </summary>
    public enum DialogDisplayName
    {
        Dialog,
        GridEditBox,
        Weather
    }
    /// <summary>
    /// Edit的类别.
    /// </summary>
    public enum EditType
    {
        Mod,
        System,
        Users
    }
    /// <summary>
    /// 为Edit查找提供快照服务.
    /// </summary>
    public sealed class EditTemp
    {
        public class Property<TValue> : UProperty<TValue>
        {
            protected override string Folder => Area.EditTempBranchFolder;
            protected override string RootName => "Edit";
            public Property(string name, TValue value) : base(name, value)
            {
            }

        }
        private Property<int> editfiletype = new Property<int>("editfiletype", 1);
        private Property<DateTime> createTime = new Property<DateTime>("createTime", new DateTime());
        private Property<string> title = new Property<string>("title", "");
        /// <summary>
        /// Edit的文件类别.
        /// </summary>
        private EditType EditType => Area.GetEditType(editfiletype.Value);
        /// <summary>
        /// 创建时间.
        /// </summary>
        private DateTime CreateTime { get => createTime.Value; }
        /// <summary>
        /// Edit的标题.
        /// </summary>
        public string Title { get => title.Value; set => title.Value = value; }
        private EditTemp(string folder)
        {
            Area.EditTempBranchFolder = Area.EditFolder + folder + @"\";
        }
        /// <summary>
        /// 从edit文件夹中获取有效的信息集合.
        /// </summary>
        /// <returns></returns>
        public static EditInfo[] GetEditInfoArray()
        {
            DirectoryInfo Folder = new DirectoryInfo(Area.EditFolder);
            List<EditInfo> editinfos = new List<EditInfo>();
            foreach (DirectoryInfo editfolder in Folder.GetDirectories())
            {
                bool isRtfExist = true;
                string pathrtftemp = "";
                for (int i = 0; i < 6; i++)
                {
                    pathrtftemp = Area.EditFolder + editfolder + @"\" + i + ".rtf";
                    if (!File.Exists(pathrtftemp))
                    {
                        isRtfExist = false;
                        break;
                    }
                }
                if (isRtfExist)
                {
                    EditTemp editTemp = new EditTemp(editfolder.Name);
                    DateTime date = editTemp.CreateTime;
                    if (Switcher.IsDateTimeString(editfolder.Name))
                    {
                        if (editTemp.CreateTime.Date != Switcher.GetDateTimeFromstring(editfolder.Name).Date)
                        {
                           date = Switcher.GetDateTimeFromstring(editfolder.Name);
                        }
                    }
                    else
                    {
                        if (editTemp.CreateTime == new DateTime())
                        {
                            date = Directory.GetCreationTime(Area.EditTempBranchFolder);
                        }
                    }
                    editinfos.Add(new EditInfo(Area.EditTempBranchFolder, editTemp.EditType, editTemp.Title, date));
                }
            }
            editinfos.Sort();
            return editinfos.ToArray();
        }
    }
    public struct EditInfo : IComparable<EditInfo>
    {
        string folder;
        EditType editType;
        string title;
        DateTime createTime;

        public EditInfo(string folder, EditType editType, string title, DateTime createTime)
        {
            this.folder = folder;
            this.editType = editType;
            this.title = title;
            this.createTime = createTime;
        }

        /// <summary>
        /// 文件夹名称.
        /// </summary>
        public string Folder { get => folder; set => folder = value; }
        /// <summary>
        /// edit文件类别,储存在Edit.xml.
        /// </summary>
        public EditType EditType { get => editType; set => editType = value; }
        /// <summary>
        /// 标题,储存在Edit.xml.
        /// </summary>
        public string Title { get => title; set => title = value; }
        /// <summary>
        /// 创建时间,储存在Edit.xml.
        /// </summary>
        public DateTime CreateTime { get => createTime; set => createTime = value; }

        public int CompareTo(EditInfo other)
        {
            return DateTime.Compare(CreateTime, other.CreateTime);
        }

        public override string ToString()
        {
            return Folder + "\n" + EditType + "\n" +
                Title + "\n" + CreateTime;
        }
    }
    /// <summary>
    /// 作业编辑器相关的设置.
    /// </summary>
    public sealed class Edit
    {
        public class Property<TValue> : UProperty<TValue>
        {
            protected override string Folder => Area.EditBranchFolder;
            protected override string RootName => "Edit";
            public Property(string name, TValue value) : base(name, value)
            {
            }
        }
        public Property<double[]> columnDefi = new Property<double[]>("columnDefi", new double[] { 0.33, 0.33 });
        public Property<double> rowDefi0 = new Property<double>("rowDefi0", 0.5);
        public Property<double> rowDefi1 = new Property<double>("rowDefi1", 0.5);
        public Property<double> rowDefi2 = new Property<double>("rowDefi2", 0.5);
        private Property<int> editfiletype = new Property<int>("editfiletype", 1);
        private int Editfiletype { get => editfiletype.Value; set => editfiletype.Value = value; }
        public Property<double> columnElp0 = new Property<double>("columnElp0", 0.4);
        public Property<double> columnElp1 = new Property<double>("columnElp1", 0.4);
        public Property<double> rowElp0 = new Property<double>("rowElp0", 0.5);
        public Property<double> rowElp1 = new Property<double>("rowElp1", 0.5);
        public Property<double> rowElp2 = new Property<double>("rowElp2", 0.5);
        private Property<DateTime> createTime = new Property<DateTime>("createTime", new DateTime());
        public Property<string> title = new Property<string>("title", "");

        public double[] ColumnDefi { get => columnDefi.Value; set => columnDefi.Value = value; }
        public double RowDefi0 { get => rowDefi0.Value; set => rowDefi0.Value = value; }
        public double RowDefi1 { get => rowDefi1.Value; set => rowDefi1.Value = value; }
        public double RowDefi2 { get => rowDefi2.Value; set => rowDefi2.Value = value; }
        public EditType EditType
        {
            get
            {
                return Area.GetEditType(editfiletype.Value);
            }
        }
        public double ColumnElp0 { get => columnElp0.Value; set => columnElp0.Value = value; }
        public double ColumnElp1 { get => columnElp1.Value; set => columnElp1.Value = value; }
        public double RowElp0 { get => rowElp0.Value; set => rowElp0.Value = value; }
        public double RowElp1 { get => rowElp1.Value; set => rowElp1.Value = value; }
        public double RowElp2 { get => rowElp2.Value; set => rowElp2.Value = value; }
        private DateTime _createTime { set => createTime.Value = value; }
        public DateTime CreateTime => createTime.Value;
        public string Title { get => title.Value; set => title.Value = value; }

        private Edit()
        {

        }
        public static Edit SelectMod()
        {
            Area.EditBranchFolder = Area.ModFolder;
            return new Edit
            {
                Editfiletype = 0
            };
        }
        /// <summary>
        ///移动Editindex并加载.
        /// </summary>
        /// <param name="isnext"></param>
        public static void Move(bool isnext)
        {
            if (isnext)
            {
                if (Area.EditIndex < Area.EditInfos.Length - 1)
                {
                    Area.EditIndex++;
                    Load(Area.EditInfos[Area.EditIndex].Folder);
                }
            }
            else
            {
                if (Area.EditIndex > 0)
                {
                    Area.EditIndex--;
                    Load(Area.EditInfos[Area.EditIndex].Folder);
                }
            }
        }
        /// <summary>
        /// 根据标准时间字符串的文件夹加载Edit.
        /// </summary>
        /// <param name="date"></param>
        public static void Load(DateTime date)
        {
            Area.EditBranchFolder = Area.GetEditBranchFolder(date);
            Area.Edit = new Edit();
            if (!File.Exists(Area.EditBranchFolder + "Edit.xml"))
            {
                Area.Edit.Editfiletype = 1;
            }
            if (date.Date == DateTime.Now.Date)
            {
                if (Area.Edit.CreateTime == new DateTime())
                {
                    Area.Edit._createTime = DateTime.Now;
                }
            }
            else
            {
                if (Area.Edit.CreateTime.Date != date.Date)
                {
                    Area.Edit._createTime = date;
                }
            }
            Area.Edit.Flush();
            for (int i = 0; i < 6; i++)
            {
                Edit.ReadRtfFile(i);
            }
            Edit.ReadBrush();
        }
        /// <summary>
        /// 根据文件的绝对路径加载Edit,只能到edit或mod中加载.
        /// 如果指定mod,重定向至SelectMod方法.
        /// </summary>
        /// <param name="folder"></param>
        public static void Load(string folder)
        {
            DirectoryInfo info = new DirectoryInfo(folder);
            if (info.Parent.FullName + @"\" == Area.EditFolder)
            {

            }
            else if (info.Parent.FullName + @"\" == Area.ModFolder)
            {
                LoadMod();
                return;
            }
            else
            {
                throw new ArgumentException("folder路径无效.");
            }
            Area.EditBranchFolder = folder;
            Area.Edit = new Edit();
            if (!File.Exists(Area.EditBranchFolder + "Edit.xml"))
            {
                Area.Edit.Editfiletype = 1;
            }
            if (Switcher.IsDateTimeString(info.Name))
            {
                if (Area.Edit.CreateTime.Date != Switcher.GetDateTimeFromstring(info.Name).Date)
                {
                    Area.Edit._createTime = Switcher.GetDateTimeFromstring(info.Name);
                }
            }
            else
            {
                if (Area.Edit.CreateTime == new DateTime())
                {
                    Area.Edit._createTime = Directory.GetCreationTime(Area.EditBranchFolder);
                }
            }
            Area.Edit.Flush();
            for (int i = 0; i < 6; i++)
            {
                Edit.ReadRtfFile(i);
            }
            Edit.ReadBrush();
        }
        public static void LoadMod()
        {
            Area.EditBranchFolder = Area.ModFolder;
            Area.Edit = new Edit()
            {
                Editfiletype = 0,
            };
            Area.Edit.Flush();
            for (int i = 0; i < 6; i++)
            {
                Edit.ReadRtfFile(i);
            }
            Area.MainWindow.ElpC1.Visibility = Visibility.Hidden;
            Area.MainWindow.ElpC2.Visibility = Visibility.Hidden;
            Area.MainWindow.ElpR1.Visibility = Visibility.Hidden;
            Area.MainWindow.ElpR2.Visibility = Visibility.Hidden;
            Area.MainWindow.ElpR3.Visibility = Visibility.Hidden;
            Area.MainWindow.ImgBefore.Visibility = Visibility.Hidden;
            Area.MainWindow.ImgNext.Visibility = Visibility.Hidden;
            Area.MainWindow.TbxEditI.Text = "正在编辑模板";

        }
        public static void ExitMod()
        {
            Load(Area.EditInfos[Area.EditIndex].Folder);
            if (Area.MainWindow.Rtx0.BorderThickness == new Thickness(1))
            {
                Area.MainWindow.ElpC1.Visibility = Visibility.Visible;
                Area.MainWindow.ElpC2.Visibility = Visibility.Visible;
                Area.MainWindow.ElpR1.Visibility = Visibility.Visible;
                Area.MainWindow.ElpR2.Visibility = Visibility.Visible;
                Area.MainWindow.ElpR3.Visibility = Visibility.Visible;
            }
            SetInfos();
        }
        /// <summary>
        /// 获取edit文件夹中有效的文件夹.
        /// </summary>
        public static void GetInfos()
        {
            Area.EditInfos = EditTemp.GetEditInfoArray();
            for (int i = 0; i < Area.EditInfos.Length; i++)
            {
                if (Area.EditBranchFolder == Area.EditInfos[i].Folder)
                {
                    Area.EditIndex = i;
                    break;
                }
            }
        }
        public static void SetInfos()
        {
            Area.MainWindow.TbxEditI.Text = Area.EditInfos[Area.EditIndex].CreateTime.GetDateString() + (Area.EditIndex + 1) + "/" + (Area.EditInfos.Length);
            if (Area.EditIndex == Area.EditInfos.Length - 1)
            {
                Area.MainWindow.ImgNext.Visibility = Visibility.Hidden;
            }
            else
            {
                Area.MainWindow.ImgNext.Visibility = Visibility.Visible;
            }
            if (Area.EditIndex == 0)
            {
                Area.MainWindow.ImgBefore.Visibility = Visibility.Hidden;
            }
            else
            {
                Area.MainWindow.ImgBefore.Visibility = Visibility.Visible;
            }
        }
        /// <summary>
        /// 初始化方法,用于注册事件及初始化设置.
        /// </summary>
        public void Flush()
        {
            Area.MainWindow.IsWindowLoaded = false;
            columnDefi.UPropertyChanged += Area.MainWindow.PropertyChanged;
            rowDefi0.UPropertyChanged += Area.MainWindow.PropertyChanged;
            rowDefi1.UPropertyChanged += Area.MainWindow.PropertyChanged;
            rowDefi2.UPropertyChanged += Area.MainWindow.PropertyChanged;
            columnElp0.UPropertyChanged += Area.MainWindow.PropertyChanged;
            columnElp1.UPropertyChanged += Area.MainWindow.PropertyChanged;
            rowElp0.UPropertyChanged += Area.MainWindow.PropertyChanged;
            rowElp1.UPropertyChanged += Area.MainWindow.PropertyChanged;
            rowElp2.UPropertyChanged += Area.MainWindow.PropertyChanged;

            dynamic t;
            t = ColumnDefi;
            t = RowDefi0;
            t = RowDefi1;
            t = RowDefi2;
            t = ColumnElp0;
            t = ColumnElp1;
            t = RowElp0;
            t = RowElp1;
            t = RowElp2;
            Area.MainWindow.IsWindowLoaded = true;
        }

        /// <summary>
        ///读取rtf并导入到RichTextbox,若不存在,则从模板处创建.
        /// </summary>
        public static void ReadRtfFile(int index)
        {
            //=>互操作.
            Area.MainWindow.IsEditLoaded = false;
            string filename = index.ToString() + ".rtf";
            if (!File.Exists(Area.EditBranchFolder + filename))
            {
                File.Copy(Area.ModFolder + filename, Area.EditBranchFolder + filename, true);
            }
            TextRange t = new TextRange(Area.MainWindow.RTbx[index].Document.ContentStart, Area.MainWindow.RTbx[index].Document.ContentEnd);
            FileStream file = new FileStream(Area.EditBranchFolder + filename, FileMode.Open);
            t.Load(file, DataFormats.Rtf);
            file.Close();
            //=>互操作.
            Area.MainWindow.IsEditLoaded = true;
        }
        /// <summary>
        /// 将对应索引的RichTextBox的内容保存为rtf.
        /// </summary>
        /// <param name="index"></param>
        public static void SaveRtfFile(int index)
        {
            if (Area.MainWindow.IsEditLoaded)
            {
                string filename = index.ToString() + ".rtf";
                TextRange t = new TextRange(Area.MainWindow.RTbx[index].Document.ContentStart, Area.MainWindow.RTbx[index].Document.ContentEnd);
                FileStream file = new FileStream(Area.EditBranchFolder + filename, FileMode.Create);
                t.Save(file, DataFormats.Rtf);
                file.Close();
            }
        }
        //public static void SaveRtfBitmap()
        //{
        //    string filepath = Area.EditBranchFolder + "BitmapBig.png";
        //    FileStream file = new FileStream(filepath, FileMode.Create);
        //    RenderTargetBitmap rtb = new RenderTargetBitmap(1920, 1080, 96.0, 96.0, PixelFormats.Default);
        //    rtb.Render(Area.MainWindow.GridEditVisual);
        //    PngBitmapEncoder be = new PngBitmapEncoder();
        //    be.Frames.Add(BitmapFrame.Create(rtb));
        //    be.Save(file);
        //    file.Close();
        //    //Area.MainWindow.ImgVisual.Source = BitmapFrame.Create(rtb);
        //    //string filepath2 = Area.EditBranchFolder + "BitmapSmall.png";
        //    //FileStream file2 = new FileStream(filepath2, FileMode.Create);
        //    //RenderTargetBitmap rtb2 = new RenderTargetBitmap((int)Area.MainWindow.ImgVisual.Width, (int)Area.MainWindow.ImgVisual.Height, 96.0, 96.0, PixelFormats.Default);
        //    //rtb2.Render(Area.MainWindow.ImgVisual);
        //    //PngBitmapEncoder be2 = new PngBitmapEncoder();
        //    //be2.Frames.Add(BitmapFrame.Create(rtb2));
        //    //be2.Save(file2);
        //    //file2.Close();
        //}
        public static void SaveBrush(StrokeCollection value)
        {
            try
            {
                string path = Area.EditBranchFolder + "Brush.dat";
                FileStream fs = new FileStream(path, FileMode.Create);
                value.Save(fs);
            }
            catch (Exception)
            {
            }
        }
        public static void ReadBrush()
        {
            string path = Area.EditBranchFolder + "Brush.dat";
            if (File.Exists(path))
            {
                try
                {
                    FileStream fs = new FileStream(path, FileMode.Open);
                    StrokeCollection strokeCollection = new StrokeCollection(fs);
                    Area.MainWindow.EditICs.Load(strokeCollection);
                }
                catch (Exception)
                {
                }
            }
            else
            {
                Area.MainWindow.EditICs.Load(new StrokeCollection());
            }
        }
    }
    /// <summary>
    /// 本地设置,只用于实例方法.
    /// </summary>
    public sealed class Local
    {
        public class Property<TValue> : UProperty<TValue>
        {
            protected override string Folder => Area.LocalFolder;
            protected override string RootName => "Settings";
            public Property(string name, TValue value) : base(name, value)
            {
            }
        }

        public Property<bool> isFullScreen = new Property<bool>("isFullScreen", false);
        public Property<Size> appSize = new Property<Size>("appSize", new Size(0.7, 0.7));
        public Property<Point> appLocation = new Property<Point>("appLocation", new Point(0.15, 0.15));
        public Property<bool> isMaxShow = new Property<bool>("isMaxShow", false);
        public Property<Color> editBackgroundColor = new Property<Color>("editBackgroundColor", Color.FromRgb(20, 32, 0));
        public Property<double> columnDefiMin = new Property<double>("columnDefiMin", 0.04);
        public Property<double> rowDefiMin = new Property<double>("rowDefiMin", 0.04);
        public Property<Color[]> editcolor = new Property<Color[]>("editcolor", new Color[]
        {   Color.FromRgb(255,172,17),
            Color.FromRgb(253,99,40),
            Color.FromRgb(205,119,251),
            Color.FromRgb(1,199,252),
            Color.FromRgb(12,116,102),
            Colors.Chocolate,
            Color.FromRgb(12,234,145),
            Colors.White});
        public Property<Color[]> editBackgroundColorHistory = new Property<Color[]>("editBackgroundColorHistory", new Color[0]);
        public Property<int> exitEditInterval = new Property<int>("exitEditInterval", 12);
        public readonly Color[] EditBackgroundColorDefault = new Color[] { Color.FromRgb(20, 30, 0), Color.FromRgb(16, 28, 58), Color.FromRgb(44, 44, 44), Color.FromRgb(54, 54, 8) };
        public Property<Color[]> editcolorHistory = new Property<Color[]>("editcolorHistory", new Color[0]);
        public Property<bool> isEditBrushOpen = new Property<bool>("isEditBrushOpen", false);
        public Property<bool> isRtxHidden = new Property<bool>("isRtxHidden", false);
        public Property<int> inkColorIndex = new Property<int>("inkColorIndex", 0);
        public Property<double> inkPenwidth = new Property<double>("inkPenwidth", 4);
        public Property<bool> isWeatherOpen = new Property<bool>("isWeatherOpen", false);
        public Property<Point> gridWeatherLocation = new Property<Point>("gridWeatherLocation", new Point(0.2, 0.2));

        /// <summary>
        /// 是否为全屏模式.
        /// </summary>
        public bool IsFullScreen { get => isFullScreen.Value; set => isFullScreen.Value = value; }
        /// <summary>
        /// Application的大小,不实时更新.
        /// </summary>
        public Size AppSize { get => appSize.Value; set => appSize.Value = value; }
        /// <summary>
        /// Application的位置,不实时更新.
        /// </summary>
        public Point AppLocation { get => appLocation.Value; set => appLocation.Value = value; }
        /// <summary>
        /// 是否是最大化模式,不实时更新.
        /// </summary>
        public bool IsMaxShow { get => isMaxShow.Value; set => isMaxShow.Value = value; }
        /// <summary>
        /// Edit的背景颜色
        /// </summary>
        public Color EditBackgroundColor { get => editBackgroundColor.Value; set => editBackgroundColor.Value = value; }
        public double ColumnDefiMin { get => columnDefiMin.Value; set => columnDefiMin.Value = value; }
        public double RowDefiMin { get => rowDefiMin.Value; set => rowDefiMin.Value = value; }
        public Color[] Editcolor { get => editcolor.Value; set => editcolor.Value = value; }
        public Color[] EditBackgroundColorHistory { get => editBackgroundColorHistory.Value; set => editBackgroundColorHistory.Value = value; }
        public int ExitEditInterval { get => exitEditInterval.Value; set => exitEditInterval.Value = value; }
        public Color[] EditcolorHistory { get => editcolorHistory.Value; set => editcolorHistory.Value = value; }
        public bool IsEditBrushOpen { get => isEditBrushOpen.Value; set => isEditBrushOpen.Value = value; }
        public bool IsRtxHidden { get => isRtxHidden.Value; set => isRtxHidden.Value = value; }
        public int InkColorIndex { get => inkColorIndex.Value; set => inkColorIndex.Value = value; }
        public double InkPenwidth { get => inkPenwidth.Value; set => inkPenwidth.Value = value; }
        public bool IsWeatherOpen { get => isWeatherOpen.Value; set => isWeatherOpen.Value = value; }
        public Point GridWeatherLocation { get => gridWeatherLocation.Value; set => gridWeatherLocation.Value = value; }
        /// <summary>
        /// 初始化方法,用于注册事件及初始化设置.
        /// </summary>
        public void Flush()
        {
            #region 注册事件
            isFullScreen.UPropertyChanged += Area.MainWindow.PropertyChanged;
            editBackgroundColor.UPropertyChanged += Area.MainWindow.PropertyChanged;
            editcolor.UPropertyChanged += Area.MainWindow.PropertyChanged;
            editBackgroundColorHistory.UPropertyChanged += Area.MainWindow.PropertyChanged;
            editcolorHistory.UPropertyChanged += Area.MainWindow.PropertyChanged;
            isEditBrushOpen.UPropertyChanged += Area.MainWindow.PropertyChanged;
            isRtxHidden.UPropertyChanged += Area.MainWindow.PropertyChanged;
            inkColorIndex.UPropertyChanged += Area.MainWindow.PropertyChanged;
            isWeatherOpen.UPropertyChanged += Area.MainWindow.PropertyChanged;
            gridWeatherLocation.UPropertyChanged += Area.MainWindow.PropertyChanged;
            #endregion
            #region 刷新值
            dynamic t;
            t = IsFullScreen;
            t = EditBackgroundColor;
            t = Editcolor;
            t = EditBackgroundColorHistory;
            t = EditcolorHistory;
            t = IsEditBrushOpen;
            t = IsRtxHidden;
            t = InkColorIndex;
            t = InkPenwidth;
            t = IsWeatherOpen;
            t = GridWeatherLocation;
            #endregion
        }
    }
    /// <summary>
    /// Application的应用程序域.
    /// </summary>
    public static class Area
    {
        static MainWindow mainWindow;
        static Local local = new Local();
        static PageNavigationHelper pageNavigationHelper = new PageNavigationHelper() ;
        static Edit edit;
        static string editbranchfolder;
        static string edittempbranchfolder;
        static string whiteBoardBranchFolder;
        static TimerInventory<TimerDisplayName> timerInventory = new TimerInventory<TimerDisplayName>();
        static DialogInventory<DialogDisplayName> dialogInventory = new DialogInventory<DialogDisplayName>();
        public static readonly SolidColorBrush CheckedBrush = new SolidColorBrush(Color.FromArgb(120, 60, 144, 144));
        static EditInfo[] editInfos;
        static int editIndex;

        /// <summary>
        /// 本地文件夹路径.
        /// </summary>
        public static string LocalFolder => AppDomain.CurrentDomain.BaseDirectory;
        /// <summary>
        /// 模板路径.
        /// </summary>
        public static string ModFolder
        {
            get
            {
                string path = LocalFolder + @"mod\";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
        }
        /// <summary>
        /// 作业显示器数据总路径.
        /// </summary>
        public static string EditFolder
        {
            get
            {
                string path = LocalFolder + @"edit\";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
        }
        /// <summary>
        /// 获取某一天作业的路径.
        /// </summary>
        public static string GetEditBranchFolder(DateTime date)
        {
            string path = EditFolder + date.GetDateString() + @"\";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }
        /// <summary>
        /// 正在操作的作业的路径.
        /// </summary>
        public static string EditBranchFolder { get => editbranchfolder; set => editbranchfolder = value; }
        /// <summary>
        /// 为EditTemp提供设置路径.
        /// </summary>
        public static string EditTempBranchFolder { get => edittempbranchfolder; set => edittempbranchfolder = value; }
        /// <summary>
        /// 正在操作的白板的路径.
        /// </summary>
        public static string WhiteBoardBranchFolder { get => whiteBoardBranchFolder; set => whiteBoardBranchFolder = value; }
        /// <summary>
        /// MainWindow的引用
        /// </summary>
        public static MainWindow MainWindow { get => mainWindow; set => mainWindow = value; }
        /// <summary>
        /// 本地设置.
        /// </summary>
        public static Local Local => local;
        /// <summary>
        /// 作业编辑器设置.
        /// </summary>
        public static Edit Edit { get => edit; set => edit = value; }
        /// <summary>
        /// 窗体大小.
        /// </summary>
        public static Size ScreenSize => new Size(SystemParameters.PrimaryScreenWidth, SystemParameters.PrimaryScreenHeight);
        public static TimerInventory<TimerDisplayName> TimerInventory => timerInventory;
        public static DialogInventory<DialogDisplayName> DialogInventory => dialogInventory;
        public static EditInfo[] EditInfos { get => editInfos; set => editInfos = value; }
        public static int EditIndex { get => editIndex; set => editIndex = value; }
        public static PageNavigationHelper PageNavigationHelper { get => pageNavigationHelper; set => pageNavigationHelper = value; }

        /// <summary>
        /// 全屏状态改变.
        /// </summary>
        public static void FullScreenChanged(bool isfullscreen)
        {
            if (isfullscreen)
            {
                if (Area.MainWindow.IsWindowLoaded && MainWindow.WindowState == WindowState.Normal)//记录位置和大小.
                {
                    Area.Local.AppSize = new Size(MainWindow.Width / ScreenSize.Width, MainWindow.Height / ScreenSize.Height);
                    Area.Local.AppLocation = new Point(MainWindow.Left / ScreenSize.Width, MainWindow.Top / ScreenSize.Height);
                }
                MainWindow.WindowStyle = WindowStyle.None;
                MainWindow.ResizeMode = ResizeMode.NoResize;
                Area.Local.IsMaxShow = MainWindow.WindowState == WindowState.Maximized;
                MainWindow.WindowState = WindowState.Normal;
                MainWindow.Left = 0;
                MainWindow.Top = 0;
                MainWindow.Width = ScreenSize.Width;
                MainWindow.Height = ScreenSize.Height;
                MainWindow.ImgEditBrush.Visibility = Visibility.Visible;
            }
            else
            {
                MainWindow.WindowStyle = WindowStyle.SingleBorderWindow;
                MainWindow.ResizeMode = ResizeMode.CanResize;
                if (Area.Local.IsMaxShow)
                {
                    Area.MainWindow.WindowState = WindowState.Maximized;
                }
                MainWindow.Left = Area.Local.AppLocation.X * ScreenSize.Width;
                MainWindow.Top = Area.Local.AppLocation.Y * ScreenSize.Height;
                MainWindow.Width = Area.Local.AppSize.Width * ScreenSize.Width;
                MainWindow.Height = Area.Local.AppSize.Height * ScreenSize.Height;
                MainWindow.ImgEditBrush.Visibility = Visibility.Hidden;
            }
        }
        public static EditType GetEditType(int arg)
        {
            if (arg == 0)
            {
                return EditType.Mod;
            }
            else if (arg == 1)
            {
                return EditType.System;
            }
            else
            {
                return EditType.Users;
            }
        }
        public static void LoadBackgroundImage()
        {
            if (File.Exists(Area.LocalFolder + @"background\image.png"))
            {
                Area.MainWindow.ImageBack.Source = new BitmapImage(new Uri(Area.LocalFolder + @"background\image.png"));
            }
        }
    }
}
