using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.IO;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Windows.Ink;
using System.Windows.Controls;
using System.Windows.Input;
using User;
using User.SoftWare;

namespace Edit_Community
{
    public enum EditItemType
    {
        Reserved = -1,
        Daily = 0,
        Note = 1,
        Holiday = 2,
        Summervacation = 3,
        Wintervacation = 4,
        Hugeevent = 5
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

        public EditTemp(string folder, bool isRoot = false)
        {
            if (!isRoot)
            {
                if (folder != null)
                {
                    EditTempFolder = AppData.EditFolder + folder + @"\";
                }
                else
                {
                    EditTempFolder = AppData.ModFolder;
                }
            }
            else
            {
                EditTempFolder = folder;
            }
            uSettings = new USettings(EditTempFolder, "Edit");
            CreateTimeProperty = uSettings.Register("createTime", new DateTime());
            TitleProperty = uSettings.Register("title", "");
            EditFileTypeProperty = uSettings.Register("editfiletype", 0);
        }

        /// <summary>
        /// Edit的文件类别.
        /// </summary>
        private EditItemType EditType => AppData.GetEditType(EditFileTypeProperty.Value);
        /// <summary>
        /// 创建时间.
        /// </summary>
        private DateTime CreateTime { get => CreateTimeProperty.Value; }
        /// <summary>
        /// Edit的标题.
        /// </summary>
        public string Title { get => TitleProperty.Value; set => TitleProperty.Value = value; }
        public string EditTempFolder { get; }

        /// <summary>
        /// 从edit文件夹中获取有效的信息集合.
        /// </summary>
        /// <returns></returns>
        public static EditInfo[] GetEditInfoArray()
        {
            DirectoryInfo Folder = new DirectoryInfo(AppData.EditFolder);
            List<EditInfo> editinfos = new List<EditInfo>();
            foreach (DirectoryInfo editfolder in Folder.GetDirectories())
            {
                bool isRtfExist = true;
                string pathrtftemp = "";
                for (int i = 0; i < 6; i++)
                {
                    pathrtftemp = AppData.EditFolder + editfolder + @"\" + i + ".rtf";
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
        public static EditInfo GetModEditInfo()
        {
            DirectoryInfo folder = new DirectoryInfo(AppData.ModFolder);
            EditTemp editTemp = new EditTemp(null);
            return new EditInfo(editTemp.EditTempFolder, EditItemType.Reserved, "模板", editTemp.CreateTime);
        }
    }
    public struct EditInfo : IComparable<EditInfo>
    {
        public EditInfo(string folder, EditItemType editType, string title, DateTime createTime)
        {
            this.Folder = folder;
            this.EditType = editType;
            this.Title = title;
            this.CreateTime = createTime;
        }

        /// <summary>
        /// 文件夹名称.
        /// </summary>
        public string Folder { get; set; }
        /// <summary>
        /// edit文件类别,储存在Edit.xml.
        /// </summary>
        public EditItemType EditType { get; set; }
        /// <summary>
        /// 标题,储存在Edit.xml.
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 创建时间,储存在Edit.xml.
        /// </summary>
        public DateTime CreateTime { get; set; }

        public int CompareTo(EditInfo other)
        {
            return DateTime.Compare(CreateTime, other.CreateTime);
        }

        public static bool operator ==(EditInfo left, EditInfo right)
        {
            return left.Folder == right.Folder;              
        }
        public static bool operator !=(EditInfo left, EditInfo right)
        {
            return left.Folder != right.Folder;
        }

        public override string ToString()
        {
            return Folder + "\n" + EditType + "\n" +
                Title + "\n" + CreateTime;
        }
        public override bool Equals(object obj)
        {
            if (obj is EditInfo info)
            {
                return this == info;
            }
            else
            {
                return base.Equals(obj);
            }
        }
        public override int GetHashCode()
        {
            return Folder.GetHashCode();
        }
    }
    /// <summary>
    /// 作业编辑器相关的设置.
    /// </summary>
    public sealed class Edit
    {
        public USettings uSettings = new USettings(AppData.EditBranchFolder, "Edit");
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
            ColumnDefiProperty = uSettings.Register("columnDefi", new double[] { 0.0, 0.5 }, true);
            RowDefi0Property = uSettings.Register("rowDefi0", 0.5, true);
            RowDefi1Property = uSettings.Register("rowDefi1", 0.5, true);
            RowDefi2Property = uSettings.Register("rowDefi2", 0.5, true);
            ColumnElp0Property = uSettings.Register("columnElp0", 0.4, true);
            ColumnElp1Property = uSettings.Register("columnElp1", 0.4, true);
            RowElo0Property = uSettings.Register("rowElp0", 0.5, true);
            RowElp1Property = uSettings.Register("rowElp1", 0.5, true);
            RowElp2Property = uSettings.Register("rowElp2", 0.5, true);
            CreateTimeProperty = uSettings.Register("createTime", new DateTime());
            TitleProperty = uSettings.Register("title", "");
            EditFileTypeProperty = uSettings.Register("editfiletype", 0);
        }

        public double[] ColumnDefi { get => ColumnDefiProperty.Value; set => ColumnDefiProperty.Value = value; }
        public double RowDefi0 { get => RowDefi0Property.Value; set => RowDefi0Property.Value = value; }
        public double RowDefi1 { get => RowDefi1Property.Value; set => RowDefi1Property.Value = value; }
        public double RowDefi2 { get => RowDefi2Property.Value; set => RowDefi2Property.Value = value; }
        public EditItemType EditType
        {
            get
            {
                return AppData.GetEditType(EditFileTypeProperty.Value);
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
        /// <summary>
        ///移动Editindex并加载.
        /// </summary>
        /// <param name="isnext"></param>
        public static void Move(bool isnext)
        {
            if (isnext)
            {
                if (AppData.EditIndex < AppData.EditInfos.Length - 1)
                {
                    AppData.EditIndex++;
                    Load(AppData.EditInfos[AppData.EditIndex].Folder);
                }
            }
            else
            {
                if (AppData.EditIndex > 0)
                {
                    AppData.EditIndex--;
                    Load(AppData.EditInfos[AppData.EditIndex].Folder);
                }
            }
        }
        /// <summary>
        /// 根据标准时间字符串的文件夹加载Edit.
        /// </summary>
        /// <param name="date"></param>
        public static void Load(DateTime date)
        {
            AppData.EditBranchFolder = AppData.GetEditBranchFolder(date);
            AppData.Edit = new Edit();
            if (date.Date == DateTime.Now.Date)
            {
                if (AppData.Edit.CreateTime == new DateTime())
                {
                    AppData.Edit._createTime = DateTime.Now;
                }
            }
            else
            {
                if (AppData.Edit.CreateTime.Date != date.Date)
                {
                    AppData.Edit._createTime = date;
                }
            }
            AppData.Edit.Flush();
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
            if (info.Parent.FullName + @"\" == AppData.EditFolder)
            {

            }
            else if (info.FullName == AppData.ModFolder)
            {
                LoadMod();
                return;
            }
            else
            {
                Console.WriteLine("Error folder:{0}",folder);
                throw new ArgumentException("folder路径无效.");
            }
            AppData.EditBranchFolder = folder;
            AppData.Edit = new Edit();
            if (!File.Exists(AppData.EditBranchFolder + "Edit.xml"))
            {
                AppData.Edit.EditFileType = 1;
            }
            if (Tools.IsDateTimeString(info.Name))
            {
                if (AppData.Edit.CreateTime.Date != Tools.GetDateTimeFromstring(info.Name).Date)
                {
                    AppData.Edit._createTime = Tools.GetDateTimeFromstring(info.Name);
                }
            }
            else
            {
                if (AppData.Edit.CreateTime == new DateTime())
                {
                    AppData.Edit._createTime = Directory.GetCreationTime(AppData.EditBranchFolder);
                }
            }
            AppData.Edit.Flush();
            for (int i = 0; i < 6; i++)
            {
                Edit.ReadRtfFile(i);
            }
            Edit.ReadBrush();
            SetInfos();
        }
        public static void LoadMod()
        {
            AppData.EditBranchFolder = AppData.ModFolder;
            AppData.Edit = new Edit()
            {
                EditFileType = 0,
            };
            AppData.Edit.Flush();
            for (int i = 0; i < 6; i++)
            {
                ReadRtfFile(i);
            }
            AppData.MainWindow.ElpC1.Visibility = Visibility.Hidden;
            AppData.MainWindow.ElpC2.Visibility = Visibility.Hidden;
            AppData.MainWindow.ElpR1.Visibility = Visibility.Hidden;
            AppData.MainWindow.ElpR2.Visibility = Visibility.Hidden;
            AppData.MainWindow.ElpR3.Visibility = Visibility.Hidden;
            AppData.MainWindow.ImgBefore.Visibility = Visibility.Hidden;
            AppData.MainWindow.ImgNext.Visibility = Visibility.Hidden;
            AppData.MainWindow.TbxEditI.Text = "正在编辑模板";

        }
        public static void ExitMod()
        {
            Load(AppData.EditInfos[AppData.EditIndex].Folder);
            if (AppData.MainWindow.Rtx0.BorderThickness == new Thickness(1))
            {
                AppData.MainWindow.ElpC1.Visibility = Visibility.Visible;
                AppData.MainWindow.ElpC2.Visibility = Visibility.Visible;
                AppData.MainWindow.ElpR1.Visibility = Visibility.Visible;
                AppData.MainWindow.ElpR2.Visibility = Visibility.Visible;
                AppData.MainWindow.ElpR3.Visibility = Visibility.Visible;
            }
            SetInfos();
        }
        /// <summary>
        /// 获取edit文件夹中有效的文件夹.
        /// </summary>
        public static void GetInfos()
        {
            AppData.EditInfos = EditTemp.GetEditInfoArray();
            for (int i = 0; i < AppData.EditInfos.Length; i++)
            {
                if (AppData.EditBranchFolder == AppData.EditInfos[i].Folder)
                {
                    AppData.EditIndex = i;
                    break;
                }
            }
        }
        public static void SetInfos()
        {
            AppData.MainWindow.TbxEditI.Text = AppData.EditInfos[AppData.EditIndex].CreateTime.GetDateString() + (AppData.EditIndex + 1) + "/" + (AppData.EditInfos.Length);
            if (AppData.EditIndex == AppData.EditInfos.Length - 1)
            {
                AppData.MainWindow.ImgNext.Visibility = Visibility.Hidden;
            }
            else
            {
                AppData.MainWindow.ImgNext.Visibility = Visibility.Visible;
            }
            if (AppData.EditIndex == 0)
            {
                AppData.MainWindow.ImgBefore.Visibility = Visibility.Hidden;
            }
            else
            {
                AppData.MainWindow.ImgBefore.Visibility = Visibility.Visible;
            }
        }
        /// <summary>
        /// 初始化方法,用于注册事件及初始化设置.
        /// </summary>
        public void Flush()
        {
            AppData.MainWindow.IsWindowLoaded = false;
            uSettings.USettingsChanged += AppData.MainWindow.Edit_PropertyChanged;
            uSettings.Flush();
            AppData.MainWindow.IsWindowLoaded = true;
        }

        /// <summary>
        ///读取rtf并导入到RichTextbox,若不存在,则从模板处创建.
        /// </summary>
        public static void ReadRtfFile(int index)
        {
            //=>互操作.
            AppData.MainWindow.IsEditLoaded = false;
            try
            {
                string filename = index.ToString() + ".rtf";
                if (!File.Exists(AppData.EditBranchFolder + filename))
                {
                    File.Copy(AppData.ModFolder + filename, AppData.EditBranchFolder + filename, true);
                }
                TextRange t = new TextRange(AppData.MainWindow.RTbx[index].Document.ContentStart, AppData.MainWindow.RTbx[index].Document.ContentEnd);
                using (FileStream file = new FileStream(AppData.EditBranchFolder + filename, FileMode.Open))
                {
                    t.Load(file, DataFormats.Rtf);
                }
            }
            catch (Exception)
            {
            }
            //=>互操作.
            AppData.MainWindow.IsEditLoaded = true;
        }
        /// <summary>
        /// 将对应索引的RichTextBox的内容保存为rtf.
        /// </summary>
        /// <param name="index"></param>
        public static void SaveRtfFile(int index)
        {
            if (AppData.MainWindow.IsEditLoaded)
            {
                string filename = index.ToString() + ".rtf";
                TextRange t = new TextRange(AppData.MainWindow.RTbx[index].Document.ContentStart, AppData.MainWindow.RTbx[index].Document.ContentEnd);
                FileStream file = new FileStream(AppData.EditBranchFolder + filename, FileMode.Create);
                t.Save(file, DataFormats.Rtf);
                file.Close();
            }
        }
        public static void SaveBrush(StrokeCollection value)
        {
            try
            {
                string path = AppData.EditBranchFolder + "Brush.dat";
                FileStream fs = new FileStream(path, FileMode.Create);
                value.Save(fs);
            }
            catch (Exception)
            {
            }
        }
        public static void ReadBrush()
        {
            string path = AppData.EditBranchFolder + "Brush.dat";
            if (File.Exists(path))
            {
                try
                {
                    FileStream fs = new FileStream(path, FileMode.Open);
                    StrokeCollection strokeCollection = new StrokeCollection(fs);
                    AppData.MainWindow.EditICs.Load(strokeCollection);
                }
                catch (Exception)
                {
                }
            }
            else
            {
                AppData.MainWindow.EditICs.Load(new StrokeCollection());
            }
        }
    }
}
