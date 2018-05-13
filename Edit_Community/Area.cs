using System.Windows.Media;
using User.UI;

namespace Edit_Community
{
    public static partial class AppData
    {
        public static readonly SolidColorBrush CheckedBrush = new SolidColorBrush(Color.FromArgb(120, 60, 144, 144));
        private static Edit edit;

        /// <summary>
        /// MainWindow的引用
        /// </summary>
        public static MainWindow MainWindow { get; set; }
        /// <summary>
        /// 本地设置.
        /// </summary>
        public static Local Local { get; } = new Local();

        /// <summary>
        /// 作业编辑器设置.
        /// </summary>
        public static Edit Edit
        {
            get => edit; set
            {
                edit = value;
            }
        }
        /// <summary>
        /// 窗体大小.
        /// </summary>
        public static TimerInventory<TimerDisplayName> TimerInventory { get; } = new TimerInventory<TimerDisplayName>();

        public static DialogInventory<DialogDisplayName> DialogInventory { get; } = new DialogInventory<DialogDisplayName>();
        public static EditInfo[] EditInfos { get; set; }
        public static int EditIndex { get; set; }
        public static PageNavigationHelper PageNavigationHelper { get; set; } = new PageNavigationHelper();
        public static NoticeHelper NoticeHelper { get; } = new NoticeHelper();

        public static EditItemType GetEditType(int arg)
        {
            return (EditItemType)arg;
        }
    }
}
