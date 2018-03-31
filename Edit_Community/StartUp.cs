using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.SoftWare;

namespace Edit_Community
{
    /// <summary>
    /// 启动设置.
    /// </summary>
    public static class StartUp
    {
        static USettings uSettings = new USettings(AppDomain.CurrentDomain.BaseDirectory, "StartUp");
        static USettingsProperty<bool> IsCurrentDomainProperty = uSettings.Register("IsCurrentDomain", true);
        static USettingsProperty<string> RootFolderProperty = uSettings.Register("RootFolder", AppDomain.CurrentDomain.BaseDirectory);

        public static bool IsCurrentDomain { get => IsCurrentDomainProperty.Value; set => IsCurrentDomainProperty.Value = value; }
        public static string RootFolder { get => RootFolderProperty.Value; set => RootFolderProperty.Value = value; }
        public static string LocalFolder
        {
            get
            {
#if DEBUG
                return AppDomain.CurrentDomain.BaseDirectory;
#else
                if (IsCurrentDomain == true)
                {
                    return AppDomain.CurrentDomain.BaseDirectory;
                }
                else
                {
                    return RootFolder;
                }
#endif

            }
        }

        static StartUp()
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
