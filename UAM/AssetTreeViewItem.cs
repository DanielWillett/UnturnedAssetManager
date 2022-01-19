using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace UAM;

public class AssetTreeViewItem : TreeViewItem
{
    public readonly AssetFile AssetFile;
    public static readonly FontFamily family = new FontFamily("Consolas");
    public AssetTreeViewItem(AssetFile asset)
    {
        AssetFile = asset;
        string idstr = asset.ID.ToString();
        for (int s = idstr.Length; s < 5; s++)
            idstr = "0" + idstr;
        Header = idstr + "| " + asset.Name;
        FontFamily = family;
    }
}
