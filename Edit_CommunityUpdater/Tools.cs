using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edit_CommunityUpdater
{
    public static class Tools
    {
        public static string GetRelativePath(string full, string root)
        {
            return full.Substring(root.Length, full.Length - root.Length);
        }
    }
}
