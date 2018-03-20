using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.IO;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Windows.Ink;
using System.Windows.Controls;
using System.Windows.Input;
using User;
using User.SoftWare;
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
        Weather,
        BackgroundPic
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
        public USettings uSettings;
        private USettingsProperty<DateTime> CreateTimeProperty;
        public USettingsProperty<string> TitleProperty;
        private USettingsProperty<int> EditFileTypeProperty;
        string editTempFolder;
        private EditTemp(string folder)
        {
            editTempFolder = Area.EditFolder + folder + @"\";
            uSettings = new USettings(editTempFolder, "Edit");
            CreateTimeProperty = uSettings.Register("createTime", new DateTime());
            TitleProperty = uSettings.Register("title", "");
            EditFileTypeProperty = uSettings.Register("editfiletype", 1);
        }

        /// <summary>
        /// Edit的文件类别.
        /// </summary>
        private EditType EditType => Area.GetEditType(EditFileTypeProperty.Value);
        /// <summary>
        /// 创建时间.
        /// </summary>
        private DateTime CreateTime { get => CreateTimeProperty.Value; }
        /// <summary>
        /// Edit的标题.
        /// </summary>
        public string Title { get => TitleProperty.Value; set => TitleProperty.Value = value; }
        public string EditTempFolder => editTempFolder;

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
                    if (Tools.IsDateTimeString(editfolder.Name))
                    {
                        if (editTemp.CreateTime.Date != Tools.GetDateTimeFromstring(editfolder.Name).Date)
                        {
                            date = Tools.GetDateTimeFromstring(editfolder.Name);
                        }
                    }
                    else
                    {
                        if (editTemp.CreateTime == new DateTime())
                        {
                            date = Directory.GetCreationTime(editTemp.EditTempFolder);
                        }
                    }
                    editinfos.Add(new EditInfo(editTemp.EditTempFolder, editTemp.EditType, editTemp.Title, date));
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
        public USettings uSettings = new USettings(Area.EditBranchFolder, "Edit");
        public USettingsProperty<double[]> ColumnDefiProperty;
        public USettingsProperty<double> RowDefi0Property;
        public USettingsProperty<double> RowDefi1Property;
        public USettingsProperty<double> RowDefi2Property;
        public USettingsProperty<double> ColumnElp0Property;
        public USettingsProperty<double> ColumnElp1Property;
        public USettingsProperty<double> RowElo0Property;
        public USettingsProperty<double> RowElp1Property;
        public USettingsProperty<double> RowElp2Property;
        private USettingsProperty<DateTime> CreateTimeProperty;
        public USettingsProperty<string> TitleProperty;
        private USettingsProperty<int> EditFileTypeProperty;

        private Edit()
        {
            ColumnDefiProperty = uSettings.Register("columnDefi", new double[] { 0.33, 0.33 },true);
            RowDefi0Property = uSettings.Register("rowDefi0", 0.5,true);
            RowDefi1Property = uSettings.Register("rowDefi1", 0.5,true);
            RowDefi2Property = uSettings.Register("rowDefi2", 0.5,true);
            ColumnElp0Property = uSettings.Register("columnElp0", 0.4,true);
            ColumnElp1Property = uSettings.Register("columnElp1", 0.4,true);
            RowElo0Property = uSettings.Register("rowElp0", 0.5,true);
            RowElp1Property = uSettings.Register("rowElp1", 0.5,true);
            RowElp2Property = uSettings.Register("rowElp2", 0.5,true);
            CreateTimeProperty = uSettings.Register("createTime", new DateTime());
            TitleProperty = uSettings.Register("title", "");
            EditFileTypeProperty = uSettings.Register("editfiletype", 1);
        }

        public double[] ColumnDefi { get => ColumnDefiProperty.Value; set => ColumnDefiProperty.Value = value; }
        public double RowDefi0 { get => RowDefi0Property.Value; set => RowDefi0Property.Value = value; }
        public double RowDefi1 { get => RowDefi1Property.Value; set => RowDefi1Property.Value = value; }
        public double RowDefi2 { get => RowDefi2Property.Value; set => RowDefi2Property.Value = value; }
        public EditType EditType
        {
            get
            {
                return Area.GetEditType(EditFileTypeProperty.Value);
            }
        }
        public double ColumnElp0 { get => ColumnElp0Property.Value; set => ColumnElp0Property.Value = value; }
        public double ColumnElp1 { get => ColumnElp1Property.Value; set => ColumnElp1Property.Value = value; }
        public double RowElp0 { get => RowElo0Property.Value; set => RowElo0Property.Value = value; }
        public double RowElp1 { get => RowElp1Property.Value; set => RowElp1Property.Value = value; }
        public double RowElp2 { get => RowElp2Property.Value; set => RowElp2Property.Value = value; }
        private DateTime _createTime { set => CreateTimeProperty.Value = value; }
        public DateTime CreateTime => CreateTimeProperty.Value;
        public string Title { get => TitleProperty.Value; set => TitleProperty.Value = value; }
        private int EditFileType { get => EditFileTypeProperty.Value; set => EditFileTypeProperty.Value = value; }
        public static Edit SelectMod()
        {
            Area.EditBranchFolder = Area.ModFolder;
            return new Edit
            {
                EditFileType = 0
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
                Area.Edit.EditFileType = 1;
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
                Area.Edit.EditFileType = 1;
            }
            if (Tools.IsDateTimeString(info.Name))
            {
                if (Area.Edit.CreateTime.Date != Tools.GetDateTimeFromstring(info.Name).Date)
                {
                    Area.Edit._createTime = Tools.GetDateTimeFromstring(info.Name);
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
                EditFileType = 0,
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
            uSettings.USettingsChanged += Area.MainWindow.Edit_PropertyChanged;
            uSettings.Flush();
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
        public USettings uSettings = new USettings(Area.LocalFolder, "Settings");
        public readonly USettingsProperty<bool> IsFullScreenProperty;
        public readonly USettingsProperty<Size> AppSizeProperty;
        public readonly USettingsProperty<Point> AppLocationProperty;
        public readonly USettingsProperty<bool> IsMaxShowProperty;
        public readonly USettingsProperty<Color> EditBackgroundColorProperty;
        public readonly USettingsProperty<Color> EditBackgroundColorOldproperty;
        public readonly USettingsProperty<double> ColumnDefiMinProperty;
        public readonly USettingsProperty<double> RowDefiMinProperty;
        public readonly USettingsProperty<Color[]> EditColorProperty;
        public readonly USettingsProperty<Color[]> EditBackgroundColorHistoryProperty;
        public readonly USettingsProperty<int> ExitEditIntervalProperty;
        public readonly USettingsProperty<Color[]> EditColorHistoryProperty;
        public readonly USettingsProperty<bool> IsEditBrushOpenProperty;
        public readonly USettingsProperty<bool> IsRtxHiddenProperty;
        public readonly USettingsProperty<int> InkColorIndexProperty;
        public readonly USettingsProperty<double> InkPenWidthProperty;
        public readonly USettingsProperty<int> BackgroundModeProperty;
        public readonly USettingsProperty<string> BackgroundPicPathProperty;
        public readonly USettingsProperty<string> BackgroundPicFolderProperty;
        public readonly USettingsProperty<double> BackgroundPicTimestampProperty;
        public readonly USettingsProperty<DateTime> BackgroundPicLastTimeProperty;
        public readonly USettingsProperty<int> BackgroundPicCurrentindexProperty;
        public Local()
        {
            IsFullScreenProperty = uSettings.Register("isFullScreen", false,true);
            AppSizeProperty = uSettings.Register("appSize", new Size(0.7, 0.7));
            AppLocationProperty = uSettings.Register("appLocation", new Point(0.15, 0.15));
            IsMaxShowProperty = uSettings.Register("isMaxShow", false);
            EditBackgroundColorProperty = uSettings.Register("editBackgroundColor", Color.FromRgb(20, 32, 0),true);
            EditBackgroundColorOldproperty = uSettings.Register("editBackgroundColorOld", Color.FromRgb(20, 32, 0));
            ColumnDefiMinProperty = uSettings.Register("columnDefiMin", 0.04);
            RowDefiMinProperty = uSettings.Register("rowDefiMin", 0.04);
            EditColorProperty = uSettings.Register("editcolor", new Color[]{
                Color.FromRgb(255,172,17),
                Color.FromRgb(253,99,40),
                Color.FromRgb(205,119,251),
                Color.FromRgb(1,199,252),
                Color.FromRgb(12,116,102),
                Colors.Chocolate,
                Color.FromRgb(12,234,145),
                Colors.White},true);
            EditBackgroundColorHistoryProperty = uSettings.Register("editBackgroundColorHistory", new Color[0]);
            ExitEditIntervalProperty = uSettings.Register("exitEditInterval", 12);
            EditColorHistoryProperty = uSettings.Register("editcolorHistory", new Color[0],true);
            IsEditBrushOpenProperty = uSettings.Register("isEditBrushOpen", false,true);
            IsRtxHiddenProperty = uSettings.Register("isRtxHidden", false,true);
            InkColorIndexProperty = uSettings.Register("inkColorIndex", 0);
            InkPenWidthProperty = uSettings.Register("inkPenWidth", 4.0);
            BackgroundModeProperty = uSettings.Register("BackgroundMode", 0,true);
            BackgroundPicPathProperty = uSettings.Register("BackgroundPicPath", "");
            BackgroundPicFolderProperty = uSettings.Register("BackgroundPicFolderPath", "");
            BackgroundPicTimestampProperty = uSettings.Register("BackgroundPicTimestamp", 15.0);
            BackgroundPicLastTimeProperty = uSettings.Register("BackgroundPicLastTime", new DateTime());
            BackgroundPicCurrentindexProperty = uSettings.Register("BackgroundPicCurrentindex", 0);
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
        public int InkColorIndex { get => InkColorIndexProperty.Value; set => InkColorIndexProperty.Value = value; }
        public double InkPenwidth { get => InkPenWidthProperty.Value; set => InkPenWidthProperty.Value = value; }
        public int BackgroundMode { get => BackgroundModeProperty.Value; set => BackgroundModeProperty.Value = value; }
        public string BackgroundPicPath { get => BackgroundPicPathProperty.Value; set => BackgroundPicPathProperty.Value = value; }
        public string BackgroundPicFolder { get => BackgroundPicFolderProperty.Value; set => BackgroundPicFolderProperty.Value = value; }
        public double BackgroundPicTimestamp { get => BackgroundPicTimestampProperty.Value; set => BackgroundPicTimestampProperty.Value = value; }
        public DateTime BackgroundPicLastTime { get => BackgroundPicLastTimeProperty.Value; set => BackgroundPicLastTimeProperty.Value = value; }
        public int BackgroundPicCurrentindex { get => BackgroundPicCurrentindexProperty.Value; set => BackgroundPicCurrentindexProperty.Value = value; }
        public void Flush()
        {
            uSettings.USettingsChanged += Area.MainWindow.Local_PropertyChanged;
            uSettings.Flush();
        }
    }
    /// <summary>
    /// Application的应用程序域.
    /// </summary>
    public static class Area
    {
        static MainWindow mainWindow;
        static Local local = new Local();
        static PageNavigationHelper pageNavigationHelper = new PageNavigationHelper();
        static Edit edit;
        static string editbranchfolder;
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
        public static TimerInventory<TimerDisplayName> TimerInventory => timerInventory;
        public static DialogInventory<DialogDisplayName> DialogInventory => dialogInventory;
        public static EditInfo[] EditInfos { get => editInfos; set => editInfos = value; }
        public static int EditIndex { get => editIndex; set => editIndex = value; }
        public static PageNavigationHelper PageNavigationHelper { get => pageNavigationHelper; set => pageNavigationHelper = value; }

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
    }
}
