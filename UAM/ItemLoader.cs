using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

#nullable enable

namespace UAM;

public static class ItemLoader
{
    public static EAssetCategory SelectedCategory = EAssetCategory.NONE;
    public static EAssetType SelectedType = EAssetType.UNKNOWN;
    internal static EAssetType[] types = new EAssetType[0];

    [DllImport("shell32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref FileInfo psfi, uint cbFileInfo, uint uFlags);

    public static void LoadItem(AssetFile file)
    {
        try
        {
            if (file.types.Length == 0) return;
            MainWindow.Instance!.loadedItem = file;
            MainWindow.Instance!.FilePath.Text = ShortenPath(file.file, includeFileName: false);
            MainWindow.Instance!.FileName.Text = file.file.Name;
            MainWindow.Instance!.FileIcon.Source = GetFileIcon(file.file);
            SelectedCategory = file.category;
            SelectedType = file.types[0];
            types = file.types;
            if (loadedPanels != null && loadedPanels.Length > 0 && loadedPanels[0] is AssetPanel pnl)
            {
                pnl.loaded = null;
            }
            LoadPanels(true);
        }
        catch (Exception ex)
        {
            string ext = ex.ToString();
            MessageBox.Show(ext, "Error loading file: " + file.file.FullName);
            Debug.WriteLine(ext);
        }
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
    private static Panel[]? loadedPanels = null;
    public static void LoadPanels(bool isNewItem)
    {
        if (!MainWindow.Instance!.loadedItem!.HasValue) return;
        int l = 0;
        if (SelectedCategory != EAssetCategory.NONE)
        {
            l += 2;
        }
        if (types != null)
        {
            l += types.Length;
        }
        Panel[] panels = new Panel[l];
        int b = 0;
        if (SelectedCategory != EAssetCategory.NONE)
        {
            bool a = false;
            for (int i = 0; i < AllPanels.Length; i++)
            {
                Panel panel = AllPanels[i];
                if (panel.types[0] == EAssetType.UNKNOWN && (panel.baseCategory == SelectedCategory || panel.baseCategory == EAssetCategory.NONE))
                {
                    if (!a)
                    {
                        panels[0] = AllPanels[i];
                        b++;
                        a = true;
                    }
                    else
                    {
                        panels[1] = AllPanels[i];
                        b++;
                        break;
                    }
                }
            }
        }
        if (types != null)
        {
            for (int i = 0; i < AllPanels.Length; i++)
            {
                Panel panel = AllPanels[i];
                for (int j = types.Length - 1; j >= 0; j--)
                {
                    for (int k = 0; k < panel.types.Length; k++)
                    {
                        if (panel.types[k] == types[j])
                        {
                            panels[b] = AllPanels[i];
                            b++;
                            goto next;
                        }
                    }
                }
                next: ;
            }
        }
        if (loadedPanels != null)
        {
            for (int i = 0; i < loadedPanels.Length; i++)
            {
                Panel panel = loadedPanels[i];
                if (panel == null) break;
                panel.grid.Visibility = Visibility.Collapsed;
                panel.grid.IsEnabled = false;
                MainWindow.Instance!.PanelGrid.Children.Remove(panel.grid);
            }
        }

        loadedPanels = new Panel[b];
        int i3 = 0;
        for (int i = 0; i < panels.Length; i++)
        {
            Panel panel = panels[i];
            if (panel == null) continue;
            loadedPanels[i3] = panels[i];
            i3++;
            panel.X = i % 3;
            panel.Y = i / 3;
            panel.grid.Visibility = Visibility.Visible;
            panel.grid.IsEnabled = true;
            panel.Populate(MainWindow.Instance!.loadedItem!.Value, isNewItem);
            if (!MainWindow.Instance!.PanelGrid.Children.Contains(panel.grid))
                MainWindow.Instance!.PanelGrid.Children.Add(panel.grid);
        }
    }

    public static readonly Panel[] AllPanels;
    static ItemLoader()
    {
        try
        {
            AllPanels = new Panel[]
            {
                new AssetPanel(),
                new ItemPanel()
            };
        }
        catch (Exception ex)
        {
            string ext = ex.ToString();
            MessageBox.Show(ext, "Error initializing panels");
            Debug.WriteLine(ext);
            AllPanels = new Panel[0];
        }
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

public abstract class Panel
{
    public readonly EAssetCategory baseCategory;
    public readonly EAssetType[] types;
    public readonly string name;
    public readonly Grid grid;
    protected readonly List<UIElement> children = new List<UIElement>(48);
    protected double xShift = 0d;
    protected double yShift = 0d;
    public Panel(EAssetCategory baseCategory, params EAssetType[] types)
    {
        this.types = types;
        this.baseCategory = baseCategory;
        if (this.types.Length == 0)
        {
            this.types = new EAssetType[1] { EAssetType.UNKNOWN };
            name = this.baseCategory == EAssetCategory.NONE ? "AssetPanel" : ToProperCase(this.baseCategory.ToString()) + "Panel";
        }
        else
            name = string.Join("_", this.types.Select(x => ToProperCase(x.ToString())));
        grid = new Grid()
        {
            Name = name,
            IsEnabled = false,
            Background = System.Windows.Media.Brushes.SlateGray,
            Margin = new Thickness(5, 5, 5, 5),
            Visibility = Visibility.Collapsed
        };
    }
    protected void AddElementsToGrid()
    {
        for (int i = 0; i < children.Count; i++)
            grid.Children.Add(children[i]);
    }
    protected void CreateLabel(ref TextBlock variable, string name, string text, double posX, double posY, bool startCollapsed = false, int column = 0, int row = 0)
    {
        variable = new TextBlock()
        {
            Name = name,
            Text = text,
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(xShift + posX, yShift + posY, 0d, 0d),
            TextWrapping = TextWrapping.NoWrap,
            VerticalAlignment = VerticalAlignment.Top
        };
        if (startCollapsed) variable.Visibility = Visibility.Collapsed;

        if (column > 0) Grid.SetColumn(variable, column);
        if (row > 0) Grid.SetRow(variable, row);
        children.Add(variable);
    }
    protected void CreateTextBox(ref TextBox variable, string name, double posX, double posY, double width, bool startDisabled = false, bool startCollapsed = false, bool enableWrapping = false, int column = 0, int row = 0)
    {
        variable = new TextBox()
        {
            Name = name,
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(xShift + posX, yShift + posY, 0d, 0d),
            TextWrapping = enableWrapping ? TextWrapping.Wrap : TextWrapping.NoWrap,
            VerticalAlignment = VerticalAlignment.Top,
            Width = width
        };
        if (startDisabled) variable.IsEnabled = false;
        if (startCollapsed) variable.Visibility = Visibility.Collapsed;

        if (column > 0) Grid.SetColumn(variable, column);
        if (row > 0) Grid.SetRow(variable, row);
        children.Add(variable);
    }
    protected void CreateCheckBox(ref CheckBox variable, string name, string text, double posX, double posY, bool startDisabled = false, bool startCollapsed = false, int column = 0, int row = 0)
    {
        variable = new CheckBox()
        {
            Name = name,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Content = text,
            Margin = new Thickness(xShift + posX, yShift + posY, 0d, 0d)
        };
        if (startDisabled) variable.IsEnabled = false;
        if (startCollapsed) variable.Visibility = Visibility.Collapsed;

        if (column > 0) Grid.SetColumn(variable, column);
        if (row > 0) Grid.SetRow(variable, row);
        children.Add(variable);
    }
    protected void CreateEditorButton<TEditor, TEditorWindow>(ref TEditor variable, ref Button buttonVariable, string name, string text, double posX, double posY, double width, bool startDisabled = false, bool startCollapsed = false, int column = 0, int row = 0) where TEditor : Editor<TEditorWindow>, new() where TEditorWindow : Window, new()
    {
        TEditor a = new TEditor();
        buttonVariable = new Button()
        {
            Name = name,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Content = text,
            Width = width,
            Margin = new Thickness(xShift + posX, yShift + posY, 0d, 0d)
        };
        variable = a;
        buttonVariable.Click += (s, e) => a.Show();
        if (startDisabled) buttonVariable.IsEnabled = false;
        if (startCollapsed) buttonVariable.Visibility = Visibility.Collapsed;

        if (column > 0) Grid.SetColumn(buttonVariable, column);
        if (row > 0) Grid.SetRow(buttonVariable, row);
        children.Add(buttonVariable);
    }
    protected void CreateComboBox(ref ComboBox variable, string name, double posX, double posY, double width, bool startDisabled = false, bool startCollapsed = false, bool dropdownOnly = true, int column = 0, int row = 0)
    {
        variable = new ComboBox()
        {
            Name = name,
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(xShift + posX, yShift + posY, 0d, 0d),
            VerticalAlignment = VerticalAlignment.Top,
            Width = width,
            IsEditable = !dropdownOnly,
            SelectedIndex = dropdownOnly ? 0 : -1
        };
        if (startDisabled) variable.IsEnabled = false;
        if (startCollapsed) variable.Visibility = Visibility.Collapsed;

        if (column > 0) Grid.SetColumn(variable, column);
        if (row > 0) Grid.SetRow(variable, row);
        children.Add(variable);
    }
    protected void CreateComboBox<TEnum>(ref ComboBox variable, string name, double posX, double posY, double width, TEnum? excluded = null, bool startDisabled = false, bool startCollapsed = false, bool useProperCase = true, bool dropdownOnly = true, int column = 0, int row = 0) where TEnum : struct
    {
        variable = new ComboBox()
        {
            Name = name,
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(xShift + posX, yShift + posY, 0d, 0d),
            VerticalAlignment = VerticalAlignment.Top,
            Width = width,
            IsEditable = !dropdownOnly,
            SelectedIndex = dropdownOnly ? 0 : -1
        };

        if (startDisabled) variable.IsEnabled = false;
        if (startCollapsed) variable.Visibility = Visibility.Collapsed;

        TEnum[] values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().OrderBy(x => (int)x.ToString()[0]).ToArray();

        if (excluded.HasValue)
        {
            if (useProperCase)
            {
                for (int i = 0; i < values.Length; i++)
                    if (!Equals(values[i], excluded.Value))
                        variable.Items.Add(new ComboBoxItem() { Content = ToProperCase(values[i].ToString()) });
            }
            else
            {
                for (int i = 0; i < values.Length; i++)
                    if (!Equals(values[i], excluded.Value))
                        variable.Items.Add(new ComboBoxItem() { Content = values[i].ToString() });
            }
        }
        else if (useProperCase)
        {
            for (int i = 0; i < values.Length; i++)
                variable.Items.Add(new ComboBoxItem() { Content = ToProperCase(values[i].ToString()) });
        }
        else
        {
            for (int i = 0; i < values.Length; i++)
                variable.Items.Add(new ComboBoxItem() { Content = values[i].ToString() });
        }

        if (column > 0) Grid.SetColumn(variable, column);
        if (row > 0) Grid.SetRow(variable, row);
        children.Add(variable);
    }
    protected void CreateCustom(UIElement element, int column = 0, int row = 0)
    {
        if (column > 0) Grid.SetColumn(element, column);
        if (row > 0) Grid.SetRow(element, row);
        children.Add(element);
    }
    public abstract void Populate(AssetFile file, bool isNewItem);
    public abstract void GetStringPairs(List<StringPair> refStringPairs, List<StringPair> refLocalStringPairs);
    private int _x = -1;
    public int X
    {
        get => _x;
        set
        {
            _x = value;
            Grid.SetColumn(grid, _x);
        }
    }
    private int _y = -1;
    public int Y
    {
        get => _y;
        set
        {
            _y = value;
            Grid.SetRow(grid, _y);
        }
    }
    public static void SetOrAddKey(string key, string? value, List<StringPair> refStringPairs)
    {
        for (int i = 0; i < refStringPairs.Count; i++)
        {
            if (refStringPairs[i].key == key)
            {
                if (refStringPairs[i].value != value)
                    refStringPairs[i].SetValue(value);
                return;
            }
        }
    }
    public static void SelectEnum<TEnum>(TEnum @enum, ComboBox box) where TEnum : struct
    {
        string category = @enum.ToString();
        int i2 = -1;
        foreach (string? val in box.Items.OfType<ComboBoxItem>().Select(x => x.Content).OfType<string>())
        {
            i2++;
            if (val.Equals(category, StringComparison.OrdinalIgnoreCase))
            {
                box.SelectedIndex = i2;
                break;
            }
        }
    }
    public static string ToProperCase(string input)
    {
        if (string.IsNullOrEmpty(input) || input == "NPC") return input;
        char[] c = input.ToCharArray();
        for (int i = 0; i < c.Length; i++)
        {
            char current = c[i];
            char p;
            if (i == 0 || (p = c[i - 1]) == ' ' || p == '.' || p == ',' || p == '/' || p == '\\' || p == '_' || p == ':' || p == '\t' || p == '\n' || p == '\r')
                c[i] = char.ToUpperInvariant(current);
            else 
                c[i] = char.ToLowerInvariant(current);
        }
        return new string(c);
    }
}

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
            EUseableType type = MainWindow.Instance!.loadedItem!.Value.GetEnumType("Useable", EUseableType.UNKNOWN);
            if (type != EUseableType.UNKNOWN)
                SelectEnum(type, useableTypeSelection);
            else if (Assets.defaultUseableTypes.TryGetValue(MainWindow.Instance!.loadedItem!.Value.types[0], out type))
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
                if (MainWindow.Instance!.loadedItem.HasValue && ItemLoader.SelectedCategory == MainWindow.Instance!.loadedItem.Value.category && MainWindow.Instance!.loadedItem.Value.types.Length > 0)
                {
                    SelectEnum(MainWindow.Instance!.loadedItem.Value.types[0], typeSelection);
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
        if (loaded.HasValue && file.file.FullName == loaded.Value.file.FullName) return;
        loaded = file;
        idTxt.Text = file.ID.ToString(AssetFile.info);
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
            sizeXTxt.Text = sizeX.ToString(AssetFile.info);
            sizeYTxt.Text = sizeY.ToString(AssetFile.info);
            sizeZTxt.Text = sizeZ.ToString(AssetFile.info);
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
public class ItemPanel : Panel
{
    readonly TextBlock titleLbl;
    readonly TextBlock amountLbl;
    readonly TextBox amountTxt;
    readonly TextBlock countLbl;
    readonly TextBox countTxtMin;
    readonly TextBlock countToLbl;
    readonly TextBox countTxtMax;
    readonly TextBlock qualityLbl;
    readonly TextBox qualityTxtMin;
    readonly TextBlock qualityToLbl;
    readonly TextBox qualityTxtMax;

    readonly CheckBox backwardsCheckbox;
    readonly CheckBox verifyHashCheckbox;
    readonly CheckBox canEquipCheckbox;
    readonly CheckBox canUseUnderwater;
    readonly CheckBox isProCheckbox;

    readonly TextBlock blueprintsLbl;
    readonly BlueprintEditor blueprintEditor;
    readonly Button blueprintsButton;
    private const string blueprintsButtonText = "{0} Blueprints (Edit)";
#pragma warning disable CS8601 // Possible null reference assignment.
    public ItemPanel() : base(EAssetCategory.ITEM)
    {
        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1d, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1d, GridUnitType.Star) });

        CreateLabel(ref titleLbl, nameof(titleLbl), "Item Panel", 10d, 10d);
        CreateLabel(ref amountLbl, nameof(amountLbl), "Amount:", 10d, 31d);
        CreateTextBox(ref amountTxt, nameof(amountTxt), 113d, 20d, 65d);
        CreateLabel(ref countLbl, nameof(countLbl), "Count:", 10d, 54d);
        CreateTextBox(ref countTxtMin, nameof(countTxtMin), 113d, 52d, 65d);
        CreateLabel(ref countToLbl, nameof(countToLbl), "-", 183d, 52d);
        CreateTextBox(ref countTxtMax, nameof(countTxtMax), 193d, 52d, 65d);
        CreateLabel(ref qualityLbl, nameof(qualityLbl), "Quality:", 10d, 76d);
        CreateTextBox(ref qualityTxtMin, nameof(qualityTxtMin), 113d, 75d, 65d);
        CreateLabel(ref qualityToLbl, nameof(qualityToLbl), "-", 183d, 75d);
        CreateTextBox(ref qualityTxtMax, nameof(qualityTxtMax), 193d, 75d, 65d);
        CreateCheckBox(ref backwardsCheckbox, nameof(backwardsCheckbox), "Render Backwards", 113d, 98d);
        CreateCheckBox(ref verifyHashCheckbox, nameof(verifyHashCheckbox), "Verify Hash", 113d, 118d);
        CreateCheckBox(ref canEquipCheckbox, nameof(canEquipCheckbox), "Can Player Equip", 113d, 138d);
        CreateCheckBox(ref canUseUnderwater, nameof(canUseUnderwater), "Can Use Underwater", 113d, 158d);
        CreateCheckBox(ref isProCheckbox, nameof(isProCheckbox), "Gold Item", 113d, 178d);
        isProCheckbox.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(226, 208, 13));

        CreateLabel(ref blueprintsLbl, nameof(blueprintsLbl), "Blueprints:", 10d, 31d, column: 1);
        CreateEditorButton<BlueprintEditor, BlueprintEditorWindow>(ref blueprintEditor, ref blueprintsButton, nameof(blueprintsButton), string.Format(blueprintsButtonText, 0), 113d, 29d, 215d, column: 1);

        AddElementsToGrid();
    }
#pragma warning restore CS8601 // Possible null reference assignment.

    public override void GetStringPairs(List<StringPair> refStringPairs, List<StringPair> refLocalStringPairs)
    {

    }
    public override void Populate(AssetFile file, bool isNewItem)
    {
        amountTxt.Text = file.GetIntegerTypeClampTo1("Amount").ToString();
        countTxtMin.Text = file.GetIntegerTypeClampTo1("Count_Min").ToString();
        countTxtMax.Text = file.GetIntegerTypeClampTo1("Count_Max").ToString();
        qualityTxtMin.Text = file.GetIntegerTypeClampTo1("Quality_Min").ToString();
        qualityTxtMax.Text = file.GetIntegerTypeClampTo1("Quality_Max").ToString();
        backwardsCheckbox.IsChecked = file.HasProperty("Backward");
        verifyHashCheckbox.IsChecked = file.HasProperty("Should_Verify_Hash");
        canEquipCheckbox.IsChecked = file.GetBooleanType("Can_Player_Equip", file.GetEnumType("Useable", EUseableType.UNKNOWN) != EUseableType.UNKNOWN);
        canUseUnderwater.IsChecked = file.GetBooleanType("Can_Use_Underwater", file.GetEnumType<ESlotType>("Slot", default) != ESlotType.PRIMARY);
        EAssetType type = file.types[0];
        bool goldEnabled = type switch
        {
            EAssetType.Shirt => true,
            EAssetType.Pants => true,
            EAssetType.Hat => true,
            EAssetType.Backpack => true,
            EAssetType.Vest => true,
            EAssetType.Mask => true,
            EAssetType.Glasses => true,
            EAssetType.Key => true,
            EAssetType.Box => true,
            _ => false
        };
        isProCheckbox.IsEnabled = goldEnabled;
        isProCheckbox.Visibility = goldEnabled ? Visibility.Visible : Visibility.Collapsed;
        isProCheckbox.IsChecked = file.HasProperty("Is_Pro");
        int bps = file.GetIntegerTypeClampTo0("Blueprints");
        blueprintsButton.Content = string.Format(blueprintsButtonText, bps);
        blueprintEditor.LoadWindow(file, this);
    }
}

public class GunPanel : Panel
{

    public GunPanel() : base(EAssetCategory.ITEM, EAssetType.Gun)
    {

        AddElementsToGrid();
    }

    public override void GetStringPairs(List<StringPair> refStringPairs, List<StringPair> refLocalStringPairs)
    {

    }
    public override void Populate(AssetFile file, bool isNewItem)
    {

    }
}