using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace UAM.Panels;

public class AssetPanel : Panel
{
    readonly TextBlock idLbl;
    readonly TextBlock categoryLbl;
    readonly TextBlock typeLbl;
    readonly TextBlock guidLbl;
    readonly TextBlock nameLbl;
    readonly TextBox idTxt;
    readonly TextBox guidTxt;
    readonly TextBox nameTxt;
    readonly ComboBox categorySelection;
    readonly ComboBox typeSelection;
    readonly CheckBox localFileCheckBox;
    readonly CheckBox excludeFromMBCheckBox;
    readonly TextBox localNameTxt;
    readonly TextBox localDescTxt;
    readonly TextBlock localNameLbl;
    readonly TextBlock localDescLbl;
    readonly EAssetType[] types2;
    readonly ComboBoxItem[] typesItems;
    readonly CheckBox isUseableCheckBox;
    readonly TextBlock useableTypeLbl;
    readonly ComboBox useableTypeSelection;
    readonly TextBlock allowedSlotLbl;
    readonly ComboBox allowedSlotSelection;
    readonly TextBlock rarityLbl;
    readonly ComboBox raritySelection;
    readonly TextBlock sizeLbl;
    readonly TextBox sizeXTxt;
    readonly TextBox sizeYTxt;
    readonly TextBlock sizeXLbl;
    readonly TextBlock zoomLbl;
    readonly TextBox sizeZTxt;
    public AssetFile? loaded;
    private bool isBeingPopulated = false;
    public AssetPanel() : base(EAssetCategory.NONE)
    {
        loaded = null;
        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1d, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1d, GridUnitType.Star) });

        yShift = 10d;

#pragma warning disable CS8601 // Possible null reference assignment.
        CreateLabel(ref idLbl, nameof(idLbl), "ID:", 10d, 23d);
        CreateTextBox(ref idTxt, nameof(idTxt), 101d, 22d, 43d);
        CreateLabel(ref guidLbl, nameof(guidLbl), "GUID:", 10d, 46d);
        CreateTextBox(ref guidTxt, nameof(guidTxt), 101d, 45d, 215d);
        CreateLabel(ref nameLbl, nameof(nameLbl), "Technical Name:", 10d, 69d);
        CreateTextBox(ref nameTxt, nameof(nameTxt), 101d, 68d, 215d);
        CreateLabel(ref categoryLbl, nameof(categoryLbl), "Category", 155d, 2d);
        CreateLabel(ref typeLbl, nameof(typeLbl), "Type", 252d, 2d);
        CreateComboBox<EAssetCategory>(ref categorySelection, nameof(categorySelection), 149d, 20d, 59d, EAssetCategory.NONE);
        CreateComboBox(ref typeSelection, nameof(typeSelection), 213d, 20d, 103d);

        CreateCheckBox(ref localFileCheckBox, nameof(localFileCheckBox), "Localization File", 10d, 2d, column: 1);
        CreateLabel(ref localNameLbl, nameof(localNameLbl), "English Name:", 10d, 23d, column: 1);
        CreateLabel(ref localDescLbl, nameof(localDescLbl), "English Desc:", 10d, 46d, column: 1);
        CreateTextBox(ref localNameTxt, nameof(localNameTxt), 100d, 22d, 216d, startDisabled: true, column: 1);
        CreateTextBox(ref localDescTxt, nameof(localDescTxt), 100d, 45d, 216d, startDisabled: true, enableWrapping: true, column: 1);
        localDescTxt.Height = 94d;

        CreateCheckBox(ref excludeFromMBCheckBox, nameof(excludeFromMBCheckBox), "Exclude From Master Bundle", 10d, 235d, column: 1);


        CreateCheckBox(ref isUseableCheckBox, nameof(isUseableCheckBox), "Is Useable", 10d, 92d);
        CreateLabel(ref useableTypeLbl, nameof(useableTypeLbl), "Useable Type:", 10d, 112d, startCollapsed: true);
        CreateLabel(ref allowedSlotLbl, nameof(allowedSlotLbl), "Allowed Slot:", 10d, 139d, startCollapsed: true);
        CreateLabel(ref rarityLbl, nameof(rarityLbl), "Rarity:", 10d, 166d, startCollapsed: true);
        CreateLabel(ref sizeLbl, nameof(sizeLbl), "Size:", 10d, 191d, startCollapsed: true);
        CreateLabel(ref sizeXLbl, nameof(sizeXLbl), "x", 146d, 191d, startCollapsed: true);
        CreateLabel(ref zoomLbl, nameof(zoomLbl), "Icon Zoom:", 205d, 191d, startCollapsed: true);
        CreateTextBox(ref sizeXTxt, nameof(sizeXTxt), 101d, 190d, 40d, startDisabled: true, startCollapsed: true);
        CreateTextBox(ref sizeYTxt, nameof(sizeYTxt), 157d, 190d, 40d, startDisabled: true, startCollapsed: true);
        CreateTextBox(ref sizeZTxt, nameof(sizeZTxt), 273d, 190d, 40d, startDisabled: true, startCollapsed: true);
        CreateComboBox<EUseableType>(ref useableTypeSelection, nameof(useableTypeSelection), 101d, 109d, 215d, EUseableType.UNKNOWN, true, true);
        CreateComboBox<ESlotType>(ref allowedSlotSelection, nameof(allowedSlotSelection), 101d, 136d, 215d, null, true, true);
        CreateComboBox<EItemRarity>(ref raritySelection, nameof(raritySelection), 101d, 163d, 215d, null, true, true);
#pragma warning restore CS8601 // Possible null reference assignment.

        types2 = Enum.GetValues(typeof(EAssetType)).Cast<EAssetType>().OrderBy(x => (int)x.ToString()[0]).ToArray();
        typesItems = new ComboBoxItem[types2.Length];

        for (int i = 0; i < typesItems.Length; i++)
            typesItems[i] = new ComboBoxItem() { Content = types2[i].ToString() };

        categorySelection.SelectionChanged += OnCategoryChanged;
        typeSelection.SelectionChanged += OnTypeChanged;
        localFileCheckBox.Checked += OnLocalizationCheckBoxUpdated;
        localFileCheckBox.Unchecked += OnLocalizationCheckBoxUpdated;
        isUseableCheckBox.Checked += OnUseableCheckBoxUpdated;
        isUseableCheckBox.Unchecked += OnUseableCheckBoxUpdated;
        AddElementsToGrid();
    }
    private void OnUseableCheckBoxUpdated(object sender, RoutedEventArgs e)
    {
        if (isBeingPopulated) return;
        bool val2 = isUseableCheckBox.IsChecked ?? false;
        useableTypeSelection.IsEnabled = val2;
        if (val2)
        {
            EUseableType type = MainWindow.Instance!.loadedItem!.GetEnumType("Useable", EUseableType.UNKNOWN);
            if (type != EUseableType.UNKNOWN)
                SelectEnum(type, useableTypeSelection);
            else if (Assets.defaultUseableTypes.TryGetValue(MainWindow.Instance!.loadedItem!.types[0], out type))
                SelectEnum(type, useableTypeSelection);
        }
        else
        {
            useableTypeSelection.SelectedIndex = -1;
        }
    }

    private void UpdateTypeSelection()
    {
        if ((categorySelection.SelectedItem as ComboBoxItem)?.Content is not string val) return;
        if (Enum.TryParse(val, true, out EAssetCategory enew))
        {
            typeSelection.Items.Clear();
            ItemLoader.SelectedCategory = enew;
            for (int i = 0; i < types2.Length; i++)
            {
                for (int j = 0; j < Assets.Hierarchy.Length; j++)
                {
                    if (Assets.Hierarchy[j].category == enew && types2[i] == Assets.Hierarchy[j].type)
                    {
                        typeSelection.Items.Add(typesItems[i]);
                        break;
                    }
                }
            }
            if (!isBeingPopulated)
            {
                if (MainWindow.Instance!.loadedItem is not null && ItemLoader.SelectedCategory == MainWindow.Instance!.loadedItem.category && MainWindow.Instance!.loadedItem.types.Length > 0)
                {
                    SelectEnum(MainWindow.Instance!.loadedItem.types[0], typeSelection);
                }
                else
                {
                    typeSelection.SelectedIndex = 0;
                }
            }
        }
    }
    private void OnLocalizationCheckBoxUpdated(object sender, RoutedEventArgs e)
    {
        bool val2 = localFileCheckBox.IsChecked ?? false;
        localNameTxt.IsEnabled = val2;
        localDescTxt.IsEnabled = val2;
    }
    public override void Populate(AssetFile file, bool isNewItem)
    {
        if (loaded is not null && file.file.FullName == loaded.file.FullName) return;
        loaded = file;
        idTxt.Text = file.ID.ToString(AssetFile.Locale);
        guidTxt.Text = file.guid.ToString("N");
        nameTxt.Text = file.Name;
        excludeFromMBCheckBox.IsChecked = file.HasProperty("Exclude_From_Master_Bundle");
        if (file.types.Length < 1) return;
        isBeingPopulated = true;
        if (isNewItem)
        {
            SelectEnum(file.category, categorySelection);
            UpdateTypeSelection();
            SelectEnum(file.types[0], typeSelection);
            UpdatePanelsFromType();
        }
        if (file.localPairs != null)
        {
            localFileCheckBox.IsChecked = true;
            localNameTxt.Text = file.GetLocalName() ?? string.Empty;
            localDescTxt.Text = file.GetLocalDescription() ?? string.Empty;
        }
        else
        {
            localFileCheckBox.IsChecked = false;
            localNameTxt.Text = string.Empty;
            localDescTxt.Text = string.Empty;
        }

        Visibility vis = file.category == EAssetCategory.ITEM ? Visibility.Visible : Visibility.Collapsed;
        isUseableCheckBox.Visibility = vis;
        useableTypeLbl.Visibility = vis;
        allowedSlotLbl.Visibility = vis;
        rarityLbl.Visibility = vis;
        sizeLbl.Visibility = vis;
        sizeXLbl.Visibility = vis;
        zoomLbl.Visibility = vis;
        useableTypeSelection.Visibility = vis;
        allowedSlotSelection.Visibility = vis;
        raritySelection.Visibility = vis;
        sizeXTxt.Visibility = vis;
        sizeYTxt.Visibility = vis;
        sizeZTxt.Visibility = vis;
        bool isItem = vis == Visibility.Visible;
        if (isItem)
        {
            isUseableCheckBox.IsEnabled = true;
            EUseableType type = file.GetEnumType("Useable", EUseableType.UNKNOWN);
            if (type != EUseableType.UNKNOWN)
            {
                isUseableCheckBox.IsChecked = true;
                useableTypeSelection.IsEnabled = true;
                SelectEnum(type, useableTypeSelection);
            }
            else
            {
                useableTypeSelection.SelectedIndex = -1;
                isUseableCheckBox.IsChecked = false;
                useableTypeSelection.IsEnabled = false;
            }
            ESlotType slot = file.GetEnumType("Slot", ESlotType.NONE);
            SelectEnum(slot, allowedSlotSelection);
            EItemRarity rarity = file.GetEnumType("Rarity", EItemRarity.COMMON);
            SelectEnum(rarity, raritySelection);
            int sizeX = file.GetIntegerType("Size_X", 1);
            int sizeY = file.GetIntegerType("Size_Y", 1);
            float sizeZ = file.GetFloatType("Size_Z", -1f);
            sizeXTxt.Text = sizeX.ToString(AssetFile.Locale);
            sizeYTxt.Text = sizeY.ToString(AssetFile.Locale);
            sizeZTxt.Text = sizeZ.ToString(AssetFile.Locale);
        }
        else
        {
            isUseableCheckBox.IsEnabled = false;
            isUseableCheckBox.IsChecked = false;
            useableTypeSelection.IsEnabled = false;
        }
        allowedSlotSelection.IsEnabled = isItem;
        raritySelection.IsEnabled = isItem;
        sizeXTxt.IsEnabled = isItem;
        sizeYTxt.IsEnabled = isItem;
        sizeZTxt.IsEnabled = isItem;

        isBeingPopulated = false;
    }
    private void OnCategoryChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!isBeingPopulated) UpdateTypeSelection();
    }
    private void UpdatePanelsFromType()
    {
        if ((typeSelection.SelectedItem as ComboBoxItem)?.Content is not string val) return;
        if (Enum.TryParse(val, true, out EAssetType enew))
        {
            ItemLoader.SelectedType = enew;
            for (int j = 0; j < Assets.Hierarchy.Length; j++)
            {
                if (Assets.Hierarchy[j].type == enew)
                {
                    AssetTypeHierarchy h = Assets.Hierarchy[j];
                    EAssetType[] types = new EAssetType[h.parents.Length + 1];
                    types[0] = h.type;
                    if (types.Length > 1)
                        for (int i = 0; i < h.parents.Length; i++)
                            types[i + 1] = h.parents[i];

                    ItemLoader.types = types;
                    ItemLoader.LoadPanels(false);
                    return;
                }
            }
        }
    }
    private void OnTypeChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!isBeingPopulated) UpdatePanelsFromType();
    }
    public override void GetStringPairs(List<StringPair> refStringPairs, List<StringPair> refLocalStringPairs)
    {
        if (Guid.TryParse(guidTxt.Text, out Guid guid))
            SetOrAddKey("GUID", guid.ToString("N"), refStringPairs);
        if (typeSelection.SelectedItem is ComboBoxItem item && item.Content is string value && Enum.TryParse(value, out EAssetType type) && type != EAssetType.UNKNOWN)
            SetOrAddKey("Type", type.ToString(), refStringPairs);
        if (ushort.TryParse(idTxt.Text, out ushort id))
            SetOrAddKey("ID", id.ToString(), refStringPairs);
    }
}