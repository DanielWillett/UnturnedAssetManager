using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

#nullable enable

namespace UAM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow? Instance;
        private CommonOpenFileDialog dlg;
        private readonly DirectoryInfo?[] selectedDirectories = new DirectoryInfo[4];
        private int paths = 0;
        public AssetTreeViewItem? selectedItem = null;
        public AssetFile? loadedItem = null;
        public List<AssetFile> files = new List<AssetFile>(1024);

        public MainWindow()
        {
            Instance = this;
            InitializeComponent();
            string? def = Environment.GetEnvironmentVariable("PROGRAMFILES(X86)") +
                         "\\Steam\\steamapps\\common\\Unturned\\Bundles";
            if (!Directory.Exists(def!))
                def = null;
            dlg = new CommonOpenFileDialog("Select Folder")
            {
                AddToMostRecentlyUsedList = true,
                AllowNonFileSystemItems = false,
                AllowPropertyEditing = false,
                DefaultDirectory = def,
                EnsurePathExists = true,
                EnsureFileExists = true,
                EnsureReadOnly = true,
                IsFolderPicker = true,
                NavigateToShortcut = false,
                Title = "Select Folder"
            };
            PreviewPanel.Visibility = Visibility.Collapsed;
            this.Closing += OnClosing;
        }
        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void OnBrowseButtonClicked(object sender, RoutedEventArgs e)
        {
            CommonFileDialogResult result = dlg.ShowDialog();
            if (result != CommonFileDialogResult.Ok)
                return;
            if (Directory.Exists(dlg.FileName) && paths < 3)
            {
                for (int i = 0; i < selectedDirectories.Length; i++)
                {
                    if (selectedDirectories[i] != null && (selectedDirectories[i]!.FullName == dlg.FileName || dlg.FileName.Contains(selectedDirectories[i]!.FullName)))
                        return;
                }
                for (int i = 0; i < selectedDirectories.Length; i++)
                {
                    if (selectedDirectories[i] != null &&
                        selectedDirectories[i]!.FullName.Contains(dlg.FileName))
                    {
                        selectedDirectories[i] = new DirectoryInfo(dlg.FileName);
                        goto apply;
                    }
                }
                paths++;
                selectedDirectories[3] = selectedDirectories[2];
                selectedDirectories[2] = selectedDirectories[1];
                selectedDirectories[1] = selectedDirectories[0];
                selectedDirectories[0] = new DirectoryInfo(dlg.FileName);
                apply:
                if (selectedDirectories[3] != null)
                    DirPath3.Text = selectedDirectories[3]!.FullName;
                if (selectedDirectories[2] != null)
                    DirPath2.Text = selectedDirectories[2]!.FullName;
                if (selectedDirectories[1] != null)
                    DirPath1.Text = selectedDirectories[1]!.FullName;
                if (selectedDirectories[0] != null)
                    BrowseTextBox.Text = selectedDirectories[0]!.FullName;
            }
        }
        private void OnClearButtonClicked(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 4; i++)
                selectedDirectories[i] = null;
            DirPath3.Text = string.Empty;
            DirPath2.Text = string.Empty;
            DirPath1.Text = string.Empty;
            BrowseTextBox.Text = string.Empty;
            paths = 0;
        }
        private void Worker()
        {
            files.Clear();
            string[][] list = new string[4][];
            int totalFiles = 0;
            for (int i = 0; i < selectedDirectories.Length; i++)
            {
                if (selectedDirectories[i] == null) break;
                ReportProgress(0.025f * i, "Gathering files from " + selectedDirectories[i]!.Name);
                try
                {
                    list[i] = Directory.GetFileSystemEntries(selectedDirectories[i]!.FullName, "*.dat", SearchOption.AllDirectories);
                    totalFiles += list[i].Length;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    continue;
                }
            }

            float totalFilesf = (float)totalFiles;
            int processedFiles = 0;
            for (int i = 0; i < selectedDirectories.Length; i++)
            {
                if (selectedDirectories[i] == null) break;
                int fileCount = list[i].Length;
                for (int j = 0; j < fileCount; j++)
                {
                    processedFiles++;
                    System.IO.FileInfo file = new System.IO.FileInfo(list[i][j]);
                    if (!file.Exists || file.Name.Length < 5) continue;
                    bool exists = false;
                    string filename = file.FullName;
                    for (int k = 0; k < this.files.Count; k++)
                    {
                        if (this.files[k].file.FullName == filename)
                            exists = true;
                    }

                    if (exists) continue;
                    DirectoryInfo dir = file.Directory;
                    if (file.Name.Substring(0, file.Name.Length - 4) == dir.Name)
                    {
                        System.IO.FileInfo[] files = dir.GetFiles("*.dat");
                        System.IO.FileInfo? local = null;
                        for (int k = 0; k < files.Length; k++)
                        {
                            if (files[k].Name.StartsWith("English"))
                            {
                                local = files[k];
                            }
                        }
                        if (processedFiles % 25 == 0)
                            ReportProgress((processedFiles / totalFilesf) * 0.85f + 0.1f, "Reading file " + file.Name);

                        AssetFile f = new AssetFile(file, local);
                        if (f.initSuccess)
                            this.files.Add(f);
                    }
                }
            }

            Debug.WriteLine("Found " + this.files.Count + " valid files.");
            ReportProgress(0.95f, "Loading");
            Dispatcher.Invoke(LoadItemsToTreeView);
        }
        private void OnLoadButtonClicked(object sender, RoutedEventArgs e)
        {
            if (selectedDirectories[0] != null)
                Task.Run(Worker);
        }
        private volatile float amtPending;
        private volatile string? statusPending;
        private void ReportProgress(float amt, string? status = null)
        {
            amtPending = amt;
            statusPending = status;
            Dispatcher.Invoke(IntlReportProgress);
        }
        private void IntlReportProgress()
        {
            LoadProgressBar.Value = amtPending;
            if (statusPending != null)
                LoadingStatus.Text = statusPending;
        }
        private void OnGameButtonClicked(object sender, RoutedEventArgs e)
        {
            OnClearButtonClicked(sender, e);
            string pg86 = Environment.GetEnvironmentVariable("PROGRAMFILES(X86)");
            selectedDirectories[0] = new DirectoryInfo(pg86 + "\\Steam\\steamapps\\common\\Unturned\\Bundles");
            selectedDirectories[1] = new DirectoryInfo(pg86 + "\\Steam\\steamapps\\workshop\\content\\304930");
            BrowseTextBox.Text = selectedDirectories[0]!.FullName;
            DirPath1.Text = selectedDirectories[1]!.FullName;
            OnLoadButtonClicked(sender, e);
        }
        private void LoadItemsToTreeView()
        {
            this.files.Sort((a, b) => a.ID.CompareTo(b.ID));
            TreeView.Items.Clear();
            Dictionary<EAssetType, TreeViewItem> cache = new Dictionary<EAssetType, TreeViewItem>(32);
            for (int i = 0; i < files.Count; i++)
            {
                AssetFile file = files[i];
                if (file.types.Length == 0 || file.types[0] == EAssetType.UNKNOWN) continue;
                EAssetType type = file.types[0];
                TreeViewItem? item = null;
                TreeViewItem? item2 = null;
                for (int j = file.types.Length - 1; j >= 0; j--)
                {
                    if (!cache.TryGetValue(file.types[j], out item))
                    {
                        item = new TreeViewItem()
                        {
                            Header = file.types[j].ToString(),
                            Focusable = false
                        };
                        cache.Add(file.types[j], item);
                        if (item2 == null)
                            TreeView.Items.Add(item);
                        else
                            item2.Items.Insert(0, item);
                    }
                    item2 = item;
                }

                item!.Items.Add(new AssetTreeViewItem(file));
            }

            foreach (TreeViewItem item in cache.Values)
            {
                int i = 0;
                foreach (TreeViewItem child in item.Items.OfType<TreeViewItem>().Where(x => x.Items.Count > 0))
                    i++;
                item.Items.IsLiveSorting = true;
                item.Header = item.Header + " (" + (item.Items.Count - i) + ")";
            }
            TreeView.Items.IsLiveSorting = true;
            amtPending = 0f;
            statusPending = "Idle";
            IntlReportProgress();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            selectedItem = TreeView.SelectedItem as AssetTreeViewItem;
        }

        private void LoadItemButtonClicked(object sender, RoutedEventArgs e)
        {
            if (selectedItem != null)
            {
                ItemLoader.LoadItem(selectedItem.AssetFile);
            }
        }

        private void OnGUIDSearchButtonClicked(object sender, RoutedEventArgs e)
        {
            if (Guid.TryParse(QueryTextBox.Text, out Guid guid))
            {
                for (int i = 0; i < files.Count; i++)
                {
                    if (files[i].guid == guid)
                    {
                        ItemLoader.LoadItem(files[i]);
                        return;
                    }
                }
            }
        }
        private void OnUInt16SearchButtonClicked(object sender, RoutedEventArgs e)
        {
            if ((UInt16SearchCategoryComboBox.SelectedItem as ComboBoxItem)?.Content is not string selection) return;

            if (Enum.TryParse(selection, false, out EAssetCategory category) && ushort.TryParse(QueryTextBox.Text, out ushort id))
            {
                for (int i = 0; i < files.Count; i++)
                {
                    AssetFile assetFile = files[i];
                    if (assetFile.category == category && assetFile.ID == id)
                    {
                        ItemLoader.LoadItem(files[i]);
                        return;
                    }
                }
            }
        }
        private void OnNameSearchButtonClicked(object sender, RoutedEventArgs e)
        {
            if (QueryTextBox.Text.Length < 3) return;
            List<AssetFile> matches = new List<AssetFile>();
            for (int i = 0; i < files.Count; i++)
            {
                AssetFile assetFile = files[i];
                if (assetFile.Name.IndexOf(QueryTextBox.Text, StringComparison.OrdinalIgnoreCase) != -1)
                {
                    matches.Add(assetFile);
                }
            }
            if (matches.Count == 0)
            {
                for (int i = 0; i < files.Count; i++)
                {
                    AssetFile assetFile = files[i];
                    string? ln = assetFile.GetLocalName();
                    if (ln == null) continue;
                    if (ln.IndexOf(QueryTextBox.Text, StringComparison.OrdinalIgnoreCase) != -1)
                    {
                        matches.Add(assetFile);
                    }
                }
                if (matches.Count == 0) return;
            }
            matches.Sort((a, b) => a.Name.Length.CompareTo(b.Name.Length));
            ItemLoader.LoadItem(matches[0]);
        }

        private void FileIcon_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (loadedItem.HasValue)
            {
                using Process fileopener = new Process();

                fileopener.StartInfo.FileName = "explorer";
                fileopener.StartInfo.Arguments = "\"" + loadedItem.Value.file + "\"";
                fileopener.Start();
            }
        }
    }
}
