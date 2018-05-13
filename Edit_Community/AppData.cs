using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User;
using User.SoftWare;

namespace Edit_Community
{
    /// <summary>
    /// 数据.
    /// </summary>
    public static partial class AppData
    {
        static USettings uSettings = new USettings(AppDomain.CurrentDomain.BaseDirectory, "StartUp");
        static USettingsProperty<bool> IsCurrentDomainProperty = uSettings.Register("IsCurrentDomain", true);
        static USettingsProperty<string> RootFolderProperty = uSettings.Register("RootFolder", AppDomain.CurrentDomain.BaseDirectory + @"LocalCache\");
        private static string editBranchFolder;

        public static bool IsCurrentDomain { get => IsCurrentDomainProperty.Value; set => IsCurrentDomainProperty.Value = value; }
        public static string RootFolder { get => RootFolderProperty.Value; set => RootFolderProperty.Value = value; }
        public static string LocalFolder
        {
            get
            {
#if DEBUG
                return AppDomain.CurrentDomain.BaseDirectory + @"LocalCache\";
#else
                if (IsCurrentDomain == true)
                {
                    return AppDomain.CurrentDomain.BaseDirectory + @"LocalCache\";
                }
                else
                {
                    return RootFolder;
                }
#endif

            }
        }
        /// <summary>
        /// 模板路径.
        /// </summary>
        public static string ModFolder
        {
            get
            {
                string path = AppData.LocalFolder + @"mod\";
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
                string path = AppData.LocalFolder + @"edit\";
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
        public static string EditBranchFolder
        {
            get => editBranchFolder; set
            {
                editBranchFolder = value;
                EditViewPage.Select();
            }
        }
        static AppData()
        {
#if !DEBUG
            foreach (var item in uSettings.Properties)
            {
                item.Take();
            }
#endif
        }
    }
}
