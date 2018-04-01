using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edit_CommunityUpdater.HTStudioService;

namespace Edit_CommunityUpdater
{
    public class SoftWareService
    {
        public SoftWareService(Version version, string softWareName)
        {
            Version = version;
            SoftWareName = softWareName;
        }
        public Version Version { get; private set; }
        public string SoftWareName { get; private set; }
        public string Root { get; set; } = AppDomain.CurrentDomain.BaseDirectory;
        public string Folder => Root + @"SoftWareCache\";
        public string UpdateFolder => Folder + @"Update\";
        public string UpdateBackupFolder => Folder + @"UpdateBackup";
        HTStudioService.HTStudioService service = new HTStudioService.HTStudioService();
        DownloadTask[] CurrentTask { get; set; }

        public event CheckUpdateEventHandler CheckUpdateCompleted;
        public event ChannelFreshEventHandler ChannelFreshed;
        public event EventHandler<bool> UpdateCompleted;

        /// <summary>
        /// 检查更新,触发<see cref="CheckUpdateCompleted"/>事件.
        /// </summary>
        public void CheckUpdate()
        {
            try
            {
                if (CheckHasDownload())
                {
                    return;
                }
                service.CheckUpdate(SoftWareName, Version.ToString(), out UpdateType type, out bool m);
                string version = service.GetSoftWareVersion(SoftWareName);
                long size = 0;
                if (type == UpdateType.Download)
                {
                    CurrentTask = service.GetUpdateTask(SoftWareName, Version.ToString());
                    foreach (var item in CurrentTask)
                    {
                        size += item.Size;
                    }
                }
                CheckUpdateCompleted?.Invoke(this, new CheckUpdateEventArgs(ChannelState.Completed, type, version, size));
            }
            catch (Exception)
            {
                CheckUpdateCompleted?.Invoke(this, new CheckUpdateEventArgs(ChannelState.Failed));
            }
        }
        /// <summary>
        /// 下载更新,下载到<see cref="UpdateFolder"/>指定的文件夹,并触发<see cref="ChannelFreshed"/>事件,以显示进度.
        /// </summary>
        public void DownloadUpdate()
        {
            try
            {
                if (CheckHasDownload())
                {
                    return;
                }
                CheckHasDownload();
                long p = 0;
                long size = 0;
                DownloadTask[] task = service.GetUpdateTask(SoftWareName, Version.ToString());
                foreach (var item in task)
                {
                    size += item.Size;
                }
                foreach (var item in task)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(UpdateFolder + item.ExtendedPath.Last));
                    using (FileStream fs = new FileStream(UpdateFolder + item.ExtendedPath.Last, FileMode.Create))
                    {
                        for (int i = 0; i < item.Num; i++)
                        {
                            fs.Position = i * 1024;
                            foreach (var d in service.Download(item, i, true).Data)
                            {
                                fs.WriteByte(d);
                                p++;
                            }
                            ChannelFreshed?.Invoke(this, new ChannelFreshEventArgs(ChannelState.Doing, p, size));
                        }
                    }
                }
                ChannelFreshed?.Invoke(this, new ChannelFreshEventArgs(ChannelState.Completed));
                File.WriteAllText(UpdateFolder + "UpdateInfo.txt", Version.ToString());
            }
            catch (IOException)
            {
                ChannelFreshed?.Invoke(this, new ChannelFreshEventArgs(ChannelState.FileFailed));
            }
            catch (Exception)
            {
                ChannelFreshed?.Invoke(this, new ChannelFreshEventArgs(ChannelState.Failed));
            }
        }
        /// <summary>
        /// 正式更新,将<see cref="UpdateFolder"/>中的文件复制到<see cref="AppDomain.BaseDirectory"/>.
        /// </summary>
        public void ApplyUpdate()
        {
            try
            {
                foreach (var item in new DirectoryInfo(UpdateFolder).GetFiles())
                {
                    string newpath = Root + Tools.GetRelativePath(item.FullName, UpdateFolder);
                    if (Tools.GetRelativePath(item.FullName, UpdateFolder) != "UpdateInfo.txt")
                    {
                        File.Copy(item.FullName, newpath, true);
                        File.Delete(item.FullName);
                    }
                }
                File.Delete(UpdateFolder + "UpdateInfo.txt");
            }
            catch (Exception)
            {

            }
            UpdateCompleted?.Invoke(this, true);
        }
        bool CheckHasDownload()
        {
            if (File.Exists(UpdateFolder + "UpdateInfo.txt"))
            {
                string s = File.ReadAllLines(UpdateFolder + "UpdateInfo.txt")[0];
                if (Version.TryParse(s, out Version v))
                {
                    if (v >= Version)
                    {
                        ChannelFreshed?.Invoke(this, new ChannelFreshEventArgs(ChannelState.Completed));
                        return true;
                    }
                }
            }
            return false;
        }
    }

    public delegate void CheckUpdateEventHandler(object sender, CheckUpdateEventArgs e);
    public class CheckUpdateEventArgs : EventArgs
    {
        public CheckUpdateEventArgs(ChannelState channelState)
        {
            ChannelState = channelState;
        }
        public CheckUpdateEventArgs(ChannelState channelState, UpdateType updateType, string version, long length)
        {
            ChannelState = channelState;
            UpdateType = updateType;
            Version = version;
            Length = length;
        }

        public ChannelState ChannelState { get; private set; } = ChannelState.Completed;
        public UpdateType UpdateType { get; private set; }
        public string Version { get; private set; }
        public long Length { get; private set; }
    }
    public delegate void ChannelFreshEventHandler(object sender, ChannelFreshEventArgs e);
    public class ChannelFreshEventArgs : EventArgs
    {
        public ChannelFreshEventArgs(ChannelState channelState)
        {
            ChannelState = channelState;
        }

        public ChannelFreshEventArgs(ChannelState channelState, long location, long length)
        {
            ChannelState = channelState;
            Location = location;
            Length = length;
        }

        public ChannelState ChannelState { get; private set; }
        public long Location { get; set; }
        public long Length { get; private set; }
        public double Percent => (int)((double)Location / Length * 1000) / 10.0;
    }
    public enum ChannelState
    {
        Doing,
        Failed,
        FileFailed,
        Completed,
    }
}
