using System.Windows.Media;
using User.UI;

namespace Edit_Community
{
    public static partial class AppData
    {
        static MainWindow mainWindow;
        static Local local = new Local();
        static PageNavigationHelper pageNavigationHelper = new PageNavigationHelper();
        static Edit edit;
        static TimerInventory<TimerDisplayName> timerInventory = new TimerInventory<TimerDisplayName>();
        static DialogInventory<DialogDisplayName> dialogInventory = new DialogInventory<DialogDisplayName>();
        public static readonly SolidColorBrush CheckedBrush = new SolidColorBrush(Color.FromArgb(120, 60, 144, 144));
        static EditInfo[] editInfos;
        static int editIndex;

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
        public static NoticeHelper NoticeHelper { get; } = new NoticeHelper();

        public static EditItemType GetEditType(int arg)
        {
            return (EditItemType)arg;
        }
    }
}
