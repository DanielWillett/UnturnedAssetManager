using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace UAM.Panels;
public class GunPanel : Panel
{
    public GunPanel() : base(EAssetCategory.ITEM, EAssetType.Gun)
    {
        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1d, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1d, GridUnitType.Star) });

        AddElementsToGrid();
    }

    public override void GetStringPairs(List<StringPair> refStringPairs, List<StringPair> refLocalStringPairs)
    {

    }
    public override void Populate(AssetFile file, bool isNewItem)
    {

    }
}