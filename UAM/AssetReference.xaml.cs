using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UAM;
/// <summary>
/// Interaction logic for AssetReference.xaml
/// </summary>
public partial class AssetReference : UserControl
{
    public EAssetCategory Category
    {
        get => (EAssetCategory)GetValue(CategoryProperty);
        set => SetValue(CategoryProperty, value);
    }
    public static readonly DependencyProperty CategoryProperty =
        DependencyProperty.Register("Category", typeof(EAssetCategory), typeof(AssetReference), new PropertyMetadata(EAssetCategory.ITEM));
    public EAssetType TypeFilter
    {
        get => (EAssetType)GetValue(TypeFilterProperty);
        set => SetValue(TypeFilterProperty, value);
    }
    public static readonly DependencyProperty TypeFilterProperty =
        DependencyProperty.Register("TypeFilter", typeof(EAssetType), typeof(AssetReference), new PropertyMetadata(EAssetType.UNKNOWN));
    private static Brush? _selectedBrush = null;
    private static Brush? _unselectedBrush = null;
    public AssetFile? SelectedFile
    {
        get => selectedItemIndex > -1 && MainWindow.Instance!.files.Count > selectedItemIndex
            ? MainWindow.Instance.files[selectedItemIndex]
            : default;
        set
        {
            if (value != default && selectedItemIndex > -1 && MainWindow.Instance!.files.Count > selectedItemIndex && MainWindow.Instance.files[selectedItemIndex].guid == value.guid)
                MainWindow.Instance.files[selectedItemIndex] = value;
        }
    }
    public int SelectedFileIndex => selectedItemIndex > -1 && MainWindow.Instance!.files.Count > selectedItemIndex ? selectedItemIndex : -1;
    public ESearchMethod SearchMethod
    {
        get => (ESearchMethod)GetValue(SearchMethodProperty);
        set
        {
            if (_selectedBrush == null && TryFindResource("SelectedButtonColor") is Color br)
                _selectedBrush = new SolidColorBrush(br);
            if (_unselectedBrush == null && TryFindResource("DeselectedButtonColor") is Color br2)
                _unselectedBrush = new SolidColorBrush(br2);

            if (value == ESearchMethod.NAME)
            {
                btnName.Background = _selectedBrush;
                btnGUID.Background = _unselectedBrush;
                btnUInt16.Background = _unselectedBrush;
            }
            else if (value == ESearchMethod.GUID)
            {
                btnName.Background = _unselectedBrush;
                btnGUID.Background = _selectedBrush;
                btnUInt16.Background = _unselectedBrush;
            }
            else if (value == ESearchMethod.UINT16)
            {
                btnName.Background = _unselectedBrush;
                btnGUID.Background = _unselectedBrush;
                btnUInt16.Background = _selectedBrush;
            }
            else
            {
                btnName.Background = _unselectedBrush;
                btnGUID.Background = _unselectedBrush;
                btnUInt16.Background = _unselectedBrush;
            }

            SetValue(SearchMethodProperty, value);
            UpdateSearch();
        }
    }
    public static readonly DependencyProperty SearchMethodProperty =
        DependencyProperty.Register("SearchMethod", typeof(ESearchMethod), typeof(AssetReference), new PropertyMetadata(ESearchMethod.NAME));
    public string Value
    {
        get => txtEntryBox.Text;
        set
        {
            txtEntryBox.Text = value;
            UpdateSearch();
        }
    }
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register("Value", typeof(string), typeof(AssetReference), new PropertyMetadata(string.Empty));


    public int SelectedIndex
    {
        get => (int)GetValue(SelectedIndexProperty);
        set
        {
            if (value >= fileIndexes.Count)
                value = fileIndexes.Count - 1;
            else if (value < -1)
                value = -1;
            if (value == -1)
                selectedItemIndex = -1;
            else
                selectedItemIndex = fileIndexes[value];
            SetValue(SelectedIndexProperty, value);
        }
    }
    public static readonly DependencyProperty SelectedIndexProperty =
        DependencyProperty.Register("SelectedIndex", typeof(int), typeof(AssetReference), new PropertyMetadata(-1));

    public AssetReference()
    {
        InitializeComponent();
    }
    public List<int> fileIndexes = new List<int>();
    public int selectedItemIndex = -1;
    public void UpdateSearch()
    {
        string text = txtEntryBox.Text;
        fileIndexes.Clear();
        EAssetCategory category = Category;
        EAssetType type = TypeFilter;
        switch (SearchMethod)
        {
            case ESearchMethod.NAME:
                for (int i = 0; i < MainWindow.Instance!.files.Count; i++)
                {
                    AssetFile file = MainWindow.Instance!.files[i];
                    if (type != EAssetType.UNKNOWN && (file.types.Length == 0 || file.types[0] != type)) continue;
                    if (category != EAssetCategory.NONE && file.category != category) continue;
                    if (file.Name.IndexOf(text, StringComparison.OrdinalIgnoreCase) != -1)
                        fileIndexes.Add(i);
                }
                fileIndexes.Sort((a, b) => MainWindow.Instance!.files[b].Name.Length.CompareTo(MainWindow.Instance!.files[a].Name.Length));
                if (fileIndexes.Count > 20)
                    fileIndexes.RemoveRange(20, fileIndexes.Count - 20);
                break;
            case ESearchMethod.GUID:
                if (Guid.TryParse(txtEntryBox.Text, out Guid guid))
                {
                    for (int i = 0; i < MainWindow.Instance!.files.Count; i++)
                    {
                        AssetFile file = MainWindow.Instance!.files[i];
                        if (type != EAssetType.UNKNOWN && (file.types.Length == 0 || file.types[0] != type)) continue;
                        if (category != EAssetCategory.NONE && file.category != category) continue;
                        if (file.guid == guid)
                        {
                            fileIndexes.Add(i);
                            break;
                        }
                    }
                }
                break;
            case ESearchMethod.UINT16:
                if (ushort.TryParse(txtEntryBox.Text, System.Globalization.NumberStyles.Any, AssetFile.Locale, out ushort id))
                {
                    for (int i = 0; i < MainWindow.Instance!.files.Count; i++)
                    {
                        AssetFile file = MainWindow.Instance!.files[i];
                        if (type != EAssetType.UNKNOWN && (file.types.Length == 0 || file.types[0] != type)) continue;
                        if (category != EAssetCategory.NONE && file.category != category) continue;
                        if (file.ID == id)
                        {
                            fileIndexes.Add(i);
                            if (category != EAssetCategory.NONE)
                                break;
                        }
                    }
                }
                break;
        }
        for (int i = 0; i < fileIndexes.Count; i++)
        {
            if (selectedItemIndex == fileIndexes[i])
            {
                SelectedIndex = i;
                goto disp;
            }
        }
        if (SelectedIndex >= fileIndexes.Count)
        {
            if (fileIndexes.Count == 0)
                SelectedIndex = -1;
            else 
                SelectedIndex = fileIndexes.Count - 1;
        }
        disp:
        DisplaySearch();
    }
    public void SelectItemIndex(int index)
    {
        if (index == -1)
        {
            fileIndexes.Clear();
            selectedItemIndex = -1;
        }
        if (MainWindow.Instance!.files.Count > index)
        {
            fileIndexes.Clear();
            fileIndexes.Add(index);

            if (_selectedBrush == null && TryFindResource("SelectedButtonColor") is Color br)
                _selectedBrush = new SolidColorBrush(br);
            if (_unselectedBrush == null && TryFindResource("DeselectedButtonColor") is Color br2)
                _unselectedBrush = new SolidColorBrush(br2);
            btnName.Background = _unselectedBrush;
            btnGUID.Background = _selectedBrush;
            btnUInt16.Background = _unselectedBrush;
            selectedItemIndex = index;
            SetValue(SearchMethodProperty, ESearchMethod.GUID);

            DisplaySearch();
        }
    }
    private void DisplaySearch()
    {
        if (_selectedBrush == null && TryFindResource("SelectedButtonColor") is Color br)
            _selectedBrush = new SolidColorBrush(br);
        if (_unselectedBrush == null && TryFindResource("DeselectedButtonColor") is Color br2)
            _unselectedBrush = new SolidColorBrush(br2);
        OptionsStack.Children.Clear();
        for (int i = 0; i < fileIndexes.Count; i++)
        {
            int j = fileIndexes[i];
            if (j != -1 && MainWindow.Instance!.files.Count > j)
            {
                Button button = new Button()
                {
                    Background = i == 0 ? _selectedBrush : _unselectedBrush,
                    Tag = i,
                    Content = MainWindow.Instance!.files[j].Name
                };
                button.Click += OptionClick;
                OptionsStack.Children.Add(button);
            }
        }
    }
    public void UpdateSelectedIndex()
    {
        int i2 = 0;
        int selind = SelectedIndex;
        for (int i = 0; i < OptionsStack.Children.Count; i++)
        {
            if (OptionsStack.Children[i] is not Button btn) continue;
            if (i2 == selind)
            {
                btn.Background = _selectedBrush;
            }
            else
                btn.Background = _unselectedBrush;
            i2++;
        }
    }
    private void OptionClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is int index)
        {
            int i = fileIndexes[index];
            if (MainWindow.Instance!.files.Count > i)
            {
                SelectedIndex = index;
                UpdateSelectedIndex();
            }
        }
    }

    private void OnSelectorClicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            ESearchMethod old = SearchMethod;
            if (button == btnGUID)
            {
                if (old == ESearchMethod.GUID) return;
                if (selectedItemIndex > -1 && selectedItemIndex < MainWindow.Instance!.files.Count)
                {
                    AssetFile file = MainWindow.Instance!.files[selectedItemIndex];
                    txtEntryBox.Text = file.guid.ToString("N");
                }
                SearchMethod = ESearchMethod.GUID;

            }
            else if (button == btnName)
            {
                if (old == ESearchMethod.NAME) return;
                if (selectedItemIndex > -1 && selectedItemIndex < MainWindow.Instance!.files.Count)
                {
                    AssetFile file = MainWindow.Instance!.files[selectedItemIndex];
                    txtEntryBox.Text = file.Name;
                }
                SearchMethod = ESearchMethod.NAME;
            }
            else if (button == btnUInt16)
            {
                if (old == ESearchMethod.UINT16) return;
                if (selectedItemIndex > -1 && selectedItemIndex < MainWindow.Instance!.files.Count)
                {
                    AssetFile file = MainWindow.Instance!.files[selectedItemIndex];
                    txtEntryBox.Text = file.ID.ToString(AssetFile.Locale);
                }
                SearchMethod = ESearchMethod.UINT16;

            }
        }
    }

    private void txtEntryBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateSearch();
    }

    private void LeftScrollButtonClicked(object sender, RoutedEventArgs e)
    {
        Debug.WriteLine(ScrollViewerOptions.HorizontalOffset);
        ScrollViewerOptions.ScrollToHorizontalOffset(Math.Max(0d, ScrollViewerOptions.HorizontalOffset - ScrollViewerOptions.ScrollableWidth / 15d));
        Debug.WriteLine(ScrollViewerOptions.HorizontalOffset);
    }

    private void RightScrollButtonClicked(object sender, RoutedEventArgs e)
    {
        Debug.WriteLine(ScrollViewerOptions.HorizontalOffset);
        ScrollViewerOptions.ScrollToHorizontalOffset(Math.Min(ScrollViewerOptions.ScrollableWidth, ScrollViewerOptions.HorizontalOffset + ScrollViewerOptions.ScrollableWidth / 15d));
        Debug.WriteLine(ScrollViewerOptions.HorizontalOffset);
    }

    private void OnScrollScrollViewer(object sender, MouseWheelEventArgs e)
    {
        e.Handled = true;
        if (e.Delta > 0)
        {
            ScrollViewerOptions.ScrollToHorizontalOffset(Math.Min(ScrollViewerOptions.ScrollableWidth, ScrollViewerOptions.HorizontalOffset + ScrollViewerOptions.ScrollableWidth / 15d));
        }
        else if (e.Delta < 0)
        {
            ScrollViewerOptions.ScrollToHorizontalOffset(Math.Max(0d, ScrollViewerOptions.HorizontalOffset - ScrollViewerOptions.ScrollableWidth / 15d));
        }
    }
}

public enum ESearchMethod
{
    NAME,
    GUID,
    UINT16
}

public struct Test
{
    public int t1;
    public int t2;
    public int t3;
}