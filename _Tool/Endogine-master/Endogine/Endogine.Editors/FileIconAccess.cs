using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;

namespace Endogine.Editors
{
    class FileIconAccess
    {
        //TODO: look at http://www.codeproject.com/csharp/IconHandler.asp
        public enum IconSize
        {
            Large = 0,
            Small = 1,
            Open = 2,
            Shell = 4
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, int cbFileInfo, uint uFlags);

        const uint SHGFI_USEFILEATTRIBUTES = 0x4000;
        const uint FILE_ATTRIBUTRE_NORMAL = 0x4000;
        const uint SHGFI_SYSICONINDEX = 0x4000;
        const uint ILD_TRANSPARENT = 0x1;
        const uint SHGFI_ICON = 0x1; // large icon
        const uint SHGFI_LARGEICON = 0x0;// large icon
        const uint SHGFI_SHELLICONSIZE = 0x4;
        const uint SHGFI_SMALLICON = 0x1; // small icon
        const uint SHGFI_TYPENAME = 0x400;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        struct SHFILEINFO
        {
            private const int MAX_PATH = 260;
            public IntPtr hIcon;
            public int iIcon;
            public int dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }


        static Dictionary<IntPtr, Icon> _loadedIcons;

        public FileIconAccess()
        {
        }

        public static Icon GetIcon(string fileName, IconSize size)
        {
            if (_loadedIcons == null)
                _loadedIcons = new Dictionary<IntPtr, Icon>();

            SHFILEINFO info = new SHFILEINFO();

            IntPtr ptr = SHGetFileInfo(fileName, FILE_ATTRIBUTRE_NORMAL, ref info, Marshal.SizeOf(typeof(SHFILEINFO)), (uint)size | SHGFI_USEFILEATTRIBUTES);

            Icon icon = _loadedIcons[info.hIcon]; //ptr?
            if (icon == null)
            {
                icon = System.Drawing.Icon.FromHandle(info.hIcon);
                _loadedIcons[info.hIcon] = icon;
            }

            return icon;
        }

    }
}
