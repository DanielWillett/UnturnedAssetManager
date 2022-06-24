using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UAM.Panels;

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
        isProCheckbox.Foreground = new SolidColorBrush(Color.FromRgb(226, 208, 13));

        CreateLabel(ref blueprintsLbl, nameof(blueprintsLbl), "Blueprints:", 10d, 31d, column: 1);
        CreateEditorButton<BlueprintEditor, BlueprintEditorWindow>(ref blueprintEditor, ref blueprintsButton, nameof(blueprintsButton), string.Format(blueprintsButtonText, 0), 113d, 29d, 215d, column: 1);

        AddElementsToGrid();
    }
#pragma warning restore CS8601 // Possible null reference assignment.

    public override void GetStringPairs(List<StringPair> refStringPairs, List<StringPair> refLocalStringPairs)
    {
        if (amountTxt.Text.Length > 0 && int.TryParse(amountTxt.Text, NumberStyles.Number, AssetFile.Locale, out int res))
            SetOrAddKey("Amount", res, refStringPairs);
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