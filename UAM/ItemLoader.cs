using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

#nullable enable

namespace UAM;

public static class ItemLoader
{
    [DllImport("shell32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref FileInfo psfi, uint cbFileInfo, uint uFlags);
    [DllImport("User32.dll")]
    public static extern int DestroyIcon(IntPtr hIcon);
    public static void LoadItem(AssetFile file)
    {
        MainWindow.Instance!.loadedItem = file;
        MainWindow.Instance!.FilePath.Text = ShortenPath(file.file, includeFileName: false);
        MainWindow.Instance!.FileName.Text = file.file.Name;
        MainWindow.Instance!.FileIcon.Source = GetFileIcon(file.file);
    }
    public static string ShortenPath(System.IO.FileInfo path, int sigFigs = 4, bool includeFileName = true)
    {
        string[] folders = path.Directory.FullName.Split(Path.DirectorySeparatorChar);
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = folders.Length > sigFigs ? folders.Length - sigFigs : 0; i < folders.Length; i++)
        {
            stringBuilder.Append(folders[i]).Append(Path.DirectorySeparatorChar);
        }
        if (includeFileName) stringBuilder.Append(path.Name);
        return stringBuilder.ToString();
    }
    public static ImageSource GetFileIcon(System.IO.FileInfo path)
    {
        FileInfo fileinfo = new FileInfo();
        uint size = (uint)Marshal.SizeOf(fileinfo);
        SHGetFileInfo(path.FullName, 128, ref fileinfo, size, /* request icon */ 0x100u | /* large icon */ 0x0u);
        return Imaging.CreateBitmapSourceFromHIcon(fileinfo.hIcon, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
    }
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public struct FileInfo
{
    public IntPtr hIcon;
    public int iIcon;
    public uint dwAttributes;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
    public string szDisplayName;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
    public string szTypeName;
}
