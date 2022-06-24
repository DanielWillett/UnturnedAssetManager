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
using UAM.Panels;

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
        if (MainWindow.Instance!.loadedItem != null) return;
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
            panel.Populate(MainWindow.Instance!.loadedItem!, isNewItem);
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
                new ItemPanel(),
                new GunPanel()
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
            StringPair r = refStringPairs[i];
            if (r.key.Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                if (r.value != value)
                {
                    r.value = value;
                    refStringPairs[i] = r;
                }
                return;
            }
        }

        refStringPairs.Add(new StringPair(key, value));
    }
    public static void SetOrAddKey(string key, int value, List<StringPair> refStringPairs)
    {
        string v = value.ToString(AssetFile.Locale);
        SetOrAddKey(key, v, refStringPairs);
    }
    public static void SetOrAddKey(string key, Guid value, List<StringPair> refStringPairs)
    {
        string v = value.ToString("N");
        SetOrAddKey(key, v, refStringPairs);
    }
    public static void SetOrAddKey(string key, short value, List<StringPair> refStringPairs)
    {
        string v = value.ToString(AssetFile.Locale);
        SetOrAddKey(key, v, refStringPairs);
    }
    public static void SetOrAddKey(string key, ushort value, List<StringPair> refStringPairs)
    {
        string v = value.ToString(AssetFile.Locale);
        SetOrAddKey(key, v, refStringPairs);
    }
    public static void SetOrAddEnumKey<TEnum>(string key, TEnum value, List<StringPair> refStringPairs) where TEnum : struct
    {
        string v = value.ToString();
        SetOrAddKey(key, v, refStringPairs);
    }
    public static void SetOrAddKey(string key, byte value, List<StringPair> refStringPairs)
    {
        string v = value.ToString(AssetFile.Locale);
        SetOrAddKey(key, v, refStringPairs);
    }
    public static void SetOrAddKey(string key, float value, List<StringPair> refStringPairs)
    {
        string v = value.ToString(AssetFile.Locale);
        SetOrAddKey(key, v, refStringPairs);
    }
    public static void SetOrAddKey(string key, TextBox box, List<StringPair> refStringPairs)
    {
        string text = box.Text;
        if (text.Length != 0)
        {
            SetOrAddKey(key, box.Text, refStringPairs);
        }
        else
        {
            SetOrAddFlag(key, false, refStringPairs);
        }
    }
    public static void SetOrAddIntKey(string key, TextBox box, List<StringPair> refStringPairs)
    {
        string text = box.Text;
        if (text.Length != 0 && int.TryParse(box.Text, System.Globalization.NumberStyles.Any, AssetFile.Locale, out int value))
        {
            string v = value.ToString(AssetFile.Locale);
            SetOrAddKey(key, v, refStringPairs);
        }
        else
        {
            SetOrAddFlag(key, false, refStringPairs);
        }
    }
    public static void SetOrAddFloatKey(string key, TextBox box, List<StringPair> refStringPairs)
    {
        string text = box.Text;
        if (text.Length != 0 && float.TryParse(box.Text, System.Globalization.NumberStyles.Any, AssetFile.Locale, out float value))
        {
            string v = value.ToString(AssetFile.Locale);
            SetOrAddKey(key, v, refStringPairs);
        }
        else
        {
            SetOrAddFlag(key, false, refStringPairs);
        }
    }
    public static void SetOrAddByteKey(string key, TextBox box, List<StringPair> refStringPairs)
    {
        string text = box.Text;
        if (text.Length != 0 && byte.TryParse(box.Text, System.Globalization.NumberStyles.Any, AssetFile.Locale, out byte value))
        {
            string v = value.ToString(AssetFile.Locale);
            SetOrAddKey(key, v, refStringPairs);
        }
        else
        {
            SetOrAddFlag(key, false, refStringPairs);
        }
    }
    public static void SetOrAddEnumKey<TEnum>(string key, TextBox box, List<StringPair> refStringPairs) where TEnum : unmanaged, Enum
    {
        string text = box.Text;
        if (text.Length != 0 && Enum.TryParse(box.Text, true, out TEnum value))
        {
            string v = value.ToString();
            SetOrAddKey(key, v, refStringPairs);
        }
        else
        {
            SetOrAddFlag(key, false, refStringPairs);
        }
    }
    public static void SetOrAddBooleanKey(string key, CheckBox box, List<StringPair> refStringPairs)
    {
        bool? check = box.IsChecked;
        if (!check.HasValue)
        {
            SetOrAddFlag(key, false, refStringPairs);
        }
        else
        {
            string v = check.Value ? "true" : "false";
            SetOrAddKey(key, v, refStringPairs);
        }
    }
    public static void SetOrAddAssetRefKey(string key, AssetReference reference, List<StringPair> refStringPairs, bool useUInt16 = false)
    {
        int index = reference.SelectedFileIndex;
        if (index == -1)
        {
            SetOrAddFlag(key, false, refStringPairs);
        }
        else
        {
            AssetFile file = MainWindow.Instance!.files[index];
            string v = useUInt16 ? file.ID.ToString(AssetFile.Locale) : file.guid.ToString("N");
            SetOrAddKey(key, v, refStringPairs);
        }
    }
    public static void SetOrAddBooleanFlag(string key, CheckBox box, List<StringPair> refStringPairs)
    {
        bool check = box.IsChecked ?? false;
        SetOrAddFlag(key, check, refStringPairs);
    }
    public static void SetOrAddKey(string key, bool value, List<StringPair> refStringPairs)
    {
        string v = value ? "true" : "false";
        SetOrAddKey(key, v, refStringPairs);
    }
    public static void SetOrAddFlag(string key, bool value, List<StringPair> refStringPairs)
    {
        for (int i = 0; i < refStringPairs.Count; i++)
        {
            StringPair r = refStringPairs[i];
            if (r.key.Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                if (!value)
                    refStringPairs.RemoveAt(i);
                return;
            }
        }
        if (value)
            refStringPairs.Add(new StringPair(key));
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
    public static TEnum GetSelectedEnum<TEnum>(ComboBox box) where TEnum : struct
    {
        if (box.SelectedItem is ComboBoxItem item && item.Content is string value && Enum.TryParse(value, true, out TEnum val))
            return val;
        else
            return default;
    }
    public static bool TryGetSelectedEnum<TEnum>(ComboBox box, out TEnum @enum) where TEnum : struct
    {
        if (box.SelectedItem is ComboBoxItem item && item.Content is string value && Enum.TryParse(value, true, out @enum))
            return true;
        @enum = default;
        return false;
    }
    public static string ToProperCase(string input)
    {
        if (string.IsNullOrEmpty(input) || input.Equals("NPC", StringComparison.Ordinal)) return input;
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


