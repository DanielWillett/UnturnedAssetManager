using System;
using System.Collections.Generic;
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

namespace UAM;
/// <summary>
/// Interaction logic for UserControl1.xaml
/// </summary>
public partial class Blueprint : UserControl
{
    public Blueprint()
    {
        InitializeComponent();
        AddSupplyButton.Click += OnAddSupplyClicked;
        AddOutputButton.Click += OnAddOutputClicked;
    }
    
    private void OnAddSupplyClicked(object sender, RoutedEventArgs e)
    {
        AddSupply(EMPTY_SUPPLY_DATA, supplyControls.Count);
    }
    private void OnAddOutputClicked(object sender, RoutedEventArgs e)
    {
        AddOutput(EMPTY_OUTPUT_DATA, outputControls.Count);
    }

    private static readonly EItemOrigin[]       ORIGIN_VALUES       = Enum.GetValues(typeof(EItemOrigin)).OfType<EItemOrigin>().OrderBy(x => (int)x.ToString()[0]).ToArray();
    private static readonly ENPCLogicType[]     LOGIC_VALUES        = Enum.GetValues(typeof(ENPCLogicType)).OfType<ENPCLogicType>().OrderBy(x => (int)x.ToString()[0]).ToArray();
    private static readonly ENPCConditionType[] CONDITIONS_VALUES   = Enum.GetValues(typeof(ENPCConditionType)).OfType<ENPCConditionType>().OrderBy(x => (int)x.ToString()[0]).ToArray();
    private static readonly ENPCRewardType[]    REWARD_VALUES       = Enum.GetValues(typeof(ENPCRewardType)).OfType<ENPCRewardType>().OrderBy(x => (int)x.ToString()[0]).ToArray();

    private static readonly BlueprintData.SupplyData    EMPTY_SUPPLY_DATA     = new BlueprintData.SupplyData(0, 0, false);
    private static readonly BlueprintData.OutputData    EMPTY_OUTPUT_DATA     = new BlueprintData.OutputData(0, 0, EItemOrigin.CRAFT);
    private static readonly BlueprintData.NPCCondition  EMPTY_CONDITION_DATA  = new BlueprintData.NPCCondition(ENPCConditionType.NONE, null, true, ENPCLogicType.NONE, null);
    private static readonly BlueprintData.NPCReward     EMPTY_REWARD_DATA     = new BlueprintData.NPCReward(ENPCRewardType.NONE, null, null);

    private static readonly GridLength smlLen   = new GridLength(50d, GridUnitType.Pixel);
    private static readonly GridLength lrgLen   = new GridLength(130d, GridUnitType.Pixel);
    private static readonly GridLength strLen   = new GridLength(1d, GridUnitType.Star);
    private static readonly GridLength valGrid1 = new GridLength(2d, GridUnitType.Star);
    private static readonly GridLength valGrid2 = new GridLength(3d, GridUnitType.Star);

    private readonly List<SupplyControls> supplyControls = new List<SupplyControls>();
    private readonly List<OutputControls> outputControls = new List<OutputControls>();

    private struct SupplyControls
    {
        public int index;
        public StackPanel leftHost;
        public Button removeButton;
        public TextBlock number;
        public Grid valuesGrid;
        public TextBlock idLbl;
        public TextBlock quantityLbl;
        public TextBlock isCriticalLbl;
        public TextBox idTxt;
        public TextBox quantityTxt;
        public CheckBox criticalChk;
    }
    private struct OutputControls
    {
        public int index;
        public StackPanel leftHost;
        public Button removeButton;
        public TextBlock number;
        public Grid valuesGrid;
        public TextBlock idLbl;
        public TextBlock quantityLbl;
        public TextBlock originLbl;
        public TextBox idTxt;
        public TextBox quantityTxt;
        public ComboBox originCbo;
    }

    private const string CRITICAL_TOOLTIP = "Marks this supply as required for the recipe to show up.";
    private void AddSupply(BlueprintData.SupplyData data, int index)
    {
        SuppliesTabGrid.RowDefinitions.Insert(0, new RowDefinition() { Height = smlLen });
        Grid.SetRow(AddSupplyButton, this.supplyControls.Count + 1);
        SupplyControls supplyControls = new SupplyControls() { index = index };
        supplyControls.leftHost = new StackPanel()
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
        };
        Grid.SetRow(supplyControls.leftHost, index);
        supplyControls.removeButton = new Button()
        {
            Content = "Remove",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0d, 0d, 10d, 0d),
            BorderBrush = Brushes.Red
        };
        supplyControls.removeButton.Click += OnRemoveSupplyButtonClicked;
        supplyControls.leftHost.Children.Add(supplyControls.removeButton);
        supplyControls.number = new TextBlock()
        {
            Text = "#" + (index + 1).ToString(),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        supplyControls.leftHost.Children.Add(supplyControls.number);
        SuppliesTabGrid.Children.Add(supplyControls.leftHost);
        supplyControls.valuesGrid = new Grid();
        supplyControls.valuesGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = valGrid1 });
        supplyControls.valuesGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = valGrid2 });
        supplyControls.valuesGrid.RowDefinitions.Add(new RowDefinition() { Height = strLen });
        supplyControls.valuesGrid.RowDefinitions.Add(new RowDefinition() { Height = strLen });
        supplyControls.valuesGrid.RowDefinitions.Add(new RowDefinition() { Height = strLen });
        Grid.SetColumnSpan(supplyControls.valuesGrid, 1);
        Grid.SetColumn(supplyControls.valuesGrid, 1);
        Grid.SetRow(supplyControls.valuesGrid, index);
        supplyControls.idLbl = new TextBlock()
        {
            Text = "Item ID",
            FontSize = 10
        };
        supplyControls.valuesGrid.Children.Add(supplyControls.idLbl);
        supplyControls.quantityLbl = new TextBlock()
        {
            Text = "Quantity",
            FontSize = 10
        };
        Grid.SetRow(supplyControls.quantityLbl, 1);
        supplyControls.valuesGrid.Children.Add(supplyControls.quantityLbl);
        supplyControls.isCriticalLbl = new TextBlock()
        {
            Text = "Is Critical",
            FontSize = 10,
            ToolTip = CRITICAL_TOOLTIP
        };
        Grid.SetRow(supplyControls.isCriticalLbl, 2);
        supplyControls.valuesGrid.Children.Add(supplyControls.isCriticalLbl);
        bool isVal = data.ID != 0;
        supplyControls.idTxt = new TextBox();
        Grid.SetColumn(supplyControls.idTxt, 1);
        supplyControls.valuesGrid.Children.Add(supplyControls.idTxt);
        supplyControls.quantityTxt = new TextBox();
        Grid.SetColumn(supplyControls.quantityTxt, 1);
        Grid.SetRow(supplyControls.quantityTxt, 1);
        supplyControls.valuesGrid.Children.Add(supplyControls.quantityTxt);
        supplyControls.criticalChk = new CheckBox()
        {
            ToolTip = CRITICAL_TOOLTIP
        };
        Grid.SetColumn(supplyControls.criticalChk, 1);
        Grid.SetRow(supplyControls.criticalChk, 2);
        if (isVal)
        {
            supplyControls.idTxt.Text = data.ID.ToString(AssetFile.info);
            supplyControls.quantityTxt.Text = data.Amount.ToString(AssetFile.info);
            supplyControls.criticalChk.IsChecked = data.Critical;
        }
        supplyControls.valuesGrid.Children.Add(supplyControls.criticalChk);
        SuppliesTabGrid.Children.Add(supplyControls.valuesGrid);
        this.supplyControls.Add(supplyControls);
    }
    private void RemoveSupply(SupplyControls controls)
    {
        SuppliesTabGrid.Children.Remove(controls.leftHost);
        SuppliesTabGrid.Children.Remove(controls.valuesGrid);
        SuppliesTabGrid.RowDefinitions.RemoveAt(0);
        for (int i = controls.index + 1; i < supplyControls.Count; i++)
        {
            SupplyControls ctrls = supplyControls[i];
            ctrls.index -= 1;
            Grid.SetRow(ctrls.leftHost, ctrls.index);
            Grid.SetRow(ctrls.valuesGrid, ctrls.index);
            ctrls.number.Text = "#" + ctrls.index.ToString();
            supplyControls[i] = ctrls;
        }

        supplyControls.RemoveAt(controls.index);
        Grid.SetRow(AddSupplyButton, this.supplyControls.Count);
    }
    public void OnRemoveSupplyButtonClicked(object sender, RoutedEventArgs args)
    {
        if (sender is Button button)
        {
            for (int i = 0; i < supplyControls.Count; i++)
            {
                if (supplyControls[i].removeButton == button)
                {
                    RemoveSupply(supplyControls[i]);
                    return;
                }
            }
        }
    }
    private void AddOutput(BlueprintData.OutputData data, int index)
    {
        OutputsTabGrid.RowDefinitions.Insert(0, new RowDefinition() { Height = smlLen });
        Grid.SetRow(AddOutputButton, this.outputControls.Count + 1);
        OutputControls outputControls = new OutputControls() { index = index };
        outputControls.leftHost = new StackPanel()
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
        };
        Grid.SetRow(outputControls.leftHost, index);
        outputControls.removeButton = new Button()
        {
            Content = "Remove",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0d, 0d, 10d, 0d),
            BorderBrush = Brushes.Red
        };
        outputControls.removeButton.Click += OnRemoveOutputButtonClicked;
        outputControls.leftHost.Children.Add(outputControls.removeButton);
        outputControls.number = new TextBlock()
        {
            Text = "#" + (index + 1).ToString(),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        outputControls.leftHost.Children.Add(outputControls.number);
        OutputsTabGrid.Children.Add(outputControls.leftHost);
        outputControls.valuesGrid = new Grid();
        outputControls.valuesGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = valGrid1 });
        outputControls.valuesGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = valGrid2 });
        outputControls.valuesGrid.RowDefinitions.Add(new RowDefinition() { Height = strLen });
        outputControls.valuesGrid.RowDefinitions.Add(new RowDefinition() { Height = strLen });
        outputControls.valuesGrid.RowDefinitions.Add(new RowDefinition() { Height = strLen });
        Grid.SetColumn(outputControls.valuesGrid, 1);
        Grid.SetRow(outputControls.valuesGrid, index);
        outputControls.idLbl = new TextBlock()
        {
            Text = "Item ID",
            FontSize = 10
        };
        outputControls.valuesGrid.Children.Add(outputControls.idLbl);
        outputControls.quantityLbl = new TextBlock()
        {
            Text = "Quantity",
            FontSize = 10
        };
        Grid.SetRow(outputControls.quantityLbl, 1);
        outputControls.valuesGrid.Children.Add(outputControls.quantityLbl);
        outputControls.originLbl = new TextBlock()
        {
            Text = "Origin",
            FontSize = 10
        };
        Grid.SetRow(outputControls.originLbl, 2);
        outputControls.valuesGrid.Children.Add(outputControls.originLbl);
        outputControls.idTxt = new TextBox();
        Grid.SetColumn(outputControls.idTxt, 1);
        outputControls.valuesGrid.Children.Add(outputControls.idTxt);
        outputControls.quantityTxt = new TextBox();
        Grid.SetColumn(outputControls.quantityTxt, 1);
        Grid.SetRow(outputControls.quantityTxt, 1);
        outputControls.valuesGrid.Children.Add(outputControls.quantityTxt);
        int craftIndex = 0;
        for (; craftIndex < ORIGIN_VALUES.Length; craftIndex++)
            if (ORIGIN_VALUES[craftIndex] == EItemOrigin.CRAFT) break;
        if (craftIndex >= ORIGIN_VALUES.Length)
            craftIndex = -1;
        outputControls.originCbo = new ComboBox()
        {
            IsEditable = false
        };
        for (int i = 0; i < ORIGIN_VALUES.Length; i++)
        {
            outputControls.originCbo.Items.Add(new ComboBoxItem()
            {
                Content = Panel.ToProperCase(ORIGIN_VALUES[i].ToString())
            });
        }
        outputControls.originCbo.SelectedIndex = craftIndex;
        Grid.SetColumn(outputControls.originCbo, 1);
        Grid.SetRow(outputControls.originCbo, 2);
        bool isVal = data.ID != 0;
        if (isVal)
        {
            outputControls.idTxt.Text = data.ID.ToString(AssetFile.info);
            outputControls.quantityTxt.Text = data.Amount.ToString(AssetFile.info);
            Panel.SelectEnum(data.Origin, outputControls.originCbo);
        }
        outputControls.valuesGrid.Children.Add(outputControls.originCbo);
        OutputsTabGrid.Children.Add(outputControls.valuesGrid);
        this.outputControls.Add(outputControls);
    }
    private void RemoveOutput(OutputControls controls)
    {
        OutputsTabGrid.Children.Remove(controls.leftHost);
        OutputsTabGrid.Children.Remove(controls.valuesGrid);
        OutputsTabGrid.RowDefinitions.RemoveAt(0);
        for (int i = controls.index + 1; i < outputControls.Count; i++)
        {
            OutputControls ctrls = outputControls[i];
            ctrls.index -= 1;
            Grid.SetRow(ctrls.leftHost, ctrls.index);
            Grid.SetRow(ctrls.valuesGrid, ctrls.index);
            ctrls.number.Text = "#" + ctrls.index.ToString();
            outputControls[i] = ctrls;
        }
        outputControls.RemoveAt(controls.index);
        Grid.SetRow(AddOutputButton, this.outputControls.Count);
    }
    public void OnRemoveOutputButtonClicked(object sender, RoutedEventArgs args)
    {
        if (sender is Button button)
        {
            for (int i = 0; i < outputControls.Count; i++)
            {
                if (outputControls[i].removeButton == button)
                {
                    RemoveOutput(outputControls[i]);
                    return;
                }
            }
        }
    }
    public void LoadBlueprint(BlueprintData data, int index)
    {
        Title.Text = "Blueprint #" + (index + 1).ToString();
        SuppliesTabGrid.ColumnDefinitions.Clear();
        for (int i = 0; i < data.Supplies.Count; i++)
            AddSupply(data.Supplies[i], i);
        SuppliesTabGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = strLen });

        OutputsTabGrid.ColumnDefinitions.Clear();
        for (int i = 0; i < data.Outputs.Count; i++)
            AddOutput(data.Outputs[i], i);
        OutputsTabGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = strLen });
    }

}
public struct BlueprintData
{
    public EBlueprintType BlueprintType;
    public List<SupplyData> Supplies;
    public List<OutputData> Outputs;
    public List<NPCCondition> Conditions;
    public List<NPCReward> Rewards;
    public ToolData Tool;
    public ushort CraftEffectID;
    public byte Level;
    public EBlueprintSkill Skill;
    public bool TransferState;
    public string? Map;
    public BlueprintData(EBlueprintType type, SupplyData[] supplies, OutputData[] outputs, NPCCondition[] conditions, NPCReward[] rewards, ToolData tool, ushort buildEffect, byte level, EBlueprintSkill skill, bool transferState, string map)
    {
        BlueprintType = type;
        Outputs = new List<OutputData>(outputs);
        Supplies = new List<SupplyData>(supplies);
        Conditions = new List<NPCCondition>(conditions);
        Rewards = new List<NPCReward>(rewards);
        Tool = tool;
        CraftEffectID = buildEffect;
        Level = level;
        Skill = skill;
        TransferState = transferState;
        Map = map;
    }
    public BlueprintData(AssetFile file, int blueprintIndex)
    {
        string prefix = "Blueprint_" + blueprintIndex.ToString();
        BlueprintType = file.GetEnumType<EBlueprintType>(prefix + "_Type", default);
        int amt = file.GetIntegerTypeClampTo1(prefix + "_Supplies");
        Supplies = new List<SupplyData>(amt);
        for (int i = 0; i < amt; i++)
            Supplies.Add(new SupplyData(file, blueprintIndex, i));
        amt = file.GetIntegerTypeClampTo0(prefix + "_Outputs");
        if (amt > 0)
        {
            Outputs = new List<OutputData>(amt);
            for (int i = 0; i < amt; i++)
                Outputs.Add(new OutputData(file, blueprintIndex, i));
        }
        else
        {
            ushort id = file.GetUShortType(prefix + "_Product", file.ID);
            byte amt2 = file.GetByteType(prefix + "_Products", 1);
            EItemOrigin origin = file.GetEnumType(prefix + "_Origin", EItemOrigin.CRAFT);
            if (amt2 < 1) amt2 = 1;
            Outputs = new List<OutputData>(1) { new OutputData(id, amt2, origin) };
        }
        amt = file.GetIntegerTypeClampTo0(prefix + "_Conditions");
        Conditions = new List<NPCCondition>(amt);
        for (int i = 0; i < amt; i++)
            Conditions.Add(new NPCCondition(file, blueprintIndex, i));
        amt = file.GetIntegerTypeClampTo0(prefix + "_Rewards");
        Rewards = new List<NPCReward>(amt);
        for (int i = 0; i < amt; i++)
            Rewards.Add(new NPCReward(file, blueprintIndex, i));
        Tool = new ToolData(file, blueprintIndex);
        Level = (byte)file.GetIntegerTypeClampTo0(prefix + "_Level");
        Skill = file.GetEnumType(prefix + "_Skill", EBlueprintSkill.NONE);
        CraftEffectID = (ushort)file.GetIntegerTypeClampTo0(prefix + "_Build");
        TransferState = file.HasProperty(prefix + "_Transfer_State");
        Map = file.GetProperty(prefix + "_Map");
        
    }
    public struct SupplyData
    {
        public ushort ID;
        public int Amount;
        public bool Critical;
        public SupplyData(ushort id, int amount, bool isCritical)
        {
            this.ID = id;
            this.Amount = amount;
            this.Critical = isCritical;
        }
        public SupplyData(AssetFile file, int blueprintIndex, int supplyIndex)
        {
            string prefix = $"Blueprint_{blueprintIndex}_Supply_{supplyIndex}";
            this.ID = file.GetUShortType(prefix + "_ID", 0);
            this.Amount = file.GetIntegerTypeClampTo1(prefix + "_Amount");
            this.Critical = file.HasProperty(prefix + "_Critical");
        }
        public void WriteSupply(List<StringPair> pairs, int blueprintIndex, int supplyIndex)
        {
            string prefix = $"Blueprint_{blueprintIndex}_Supply_{supplyIndex}";
            Panel.SetOrAddKey(prefix + "_ID", ID.ToString(AssetFile.info), pairs);
            Panel.SetOrAddKey(prefix + "_Amount", ID.ToString(AssetFile.info), pairs);
            if (Critical) Panel.SetOrAddKey(prefix + "_Critical", null, pairs);
        }
        public override string ToString() => $"Supply: ID " + ID.ToString() + ", Amount " + Amount.ToString() + (Critical ? " (Critical Item)" : string.Empty);
    }
    public struct OutputData
    {
        public ushort ID;
        public int Amount;
        public EItemOrigin Origin;
        public OutputData(ushort id, int amount, EItemOrigin origin)
        {
            this.ID = id;
            this.Amount = amount;
            this.Origin = origin;
        }
        public OutputData(AssetFile file, int blueprintIndex, int outputIndex)
        {
            string prefix = $"Blueprint_{blueprintIndex}_Output_{outputIndex}";
            this.ID = file.GetUShortType(prefix + "_ID", 0);
            this.Amount = file.GetIntegerTypeClampTo1(prefix + "_Amount");
            this.Origin = file.GetEnumType(prefix + "_Origin", EItemOrigin.CRAFT);
        }
        public static OutputData ReadAsProduct(AssetFile file, int blueprintIndex)
        {
            string prefix = $"Blueprint_{blueprintIndex}_";
            ushort ID = file.GetUShortType(prefix + "_Product", 0);
            int Amount = file.GetIntegerTypeClampTo1(prefix + "_Products");
            EItemOrigin Origin = file.GetEnumType(prefix + "_Origin", EItemOrigin.CRAFT);
            return new OutputData(ID, Amount, Origin);
        }
        public void WriteSupply(List<StringPair> pairs, int blueprintIndex, int outputIndex)
        {
            string prefix = $"Blueprint_{blueprintIndex}_Output_{outputIndex}";
            Panel.SetOrAddKey(prefix + "_ID", ID.ToString(AssetFile.info), pairs);
            Panel.SetOrAddKey(prefix + "_Amount", ID.ToString(AssetFile.info), pairs);
        }
        public override string ToString() => $"Output: ID " + ID.ToString() + ", Amount " + Amount.ToString() + (Origin != EItemOrigin.CRAFT ? "Origin: " + Origin : string.Empty);
    }
    public struct ToolData
    {
        public ushort ID;
        public bool Critical;
        public ToolData(ushort id, bool isCritical)
        {
            this.ID = id;
            this.Critical = isCritical;
        }
        public ToolData(AssetFile file, int blueprintIndex)
        {
            string prefix = $"Blueprint_{blueprintIndex}_Tool";
            this.ID = file.GetUShortType(prefix, 0);
            this.Critical = file.HasProperty(prefix + "_Critical");
        }
        public void WriteSupply(List<StringPair> pairs, int blueprintIndex)
        {
            string prefix = $"Blueprint_{blueprintIndex}_Tool";
            Panel.SetOrAddKey(prefix, ID.ToString(AssetFile.info), pairs);
            if (Critical) Panel.SetOrAddKey(prefix + "_Critical", null, pairs);
        }
        public override string ToString() => ID == 0 ? "No Tool" : $"Tool: ID " + ID.ToString() + (Critical ? " (Critical Item)" : string.Empty);
    }
    public struct NPCCondition
    {
        public ENPCConditionType ConditionType;
        public string? LocalText;
        public bool ShouldReset;
        public ENPCLogicType LogicType;
        public NPCConditionData? Condition;

        public NPCCondition(ENPCConditionType conditionType, string? localText, bool newShouldReset, ENPCLogicType logicType, NPCConditionData? condition)
        {
            ConditionType = conditionType;
            LocalText = localText;
            ShouldReset = newShouldReset;
            LogicType = logicType;
            Condition = condition;
        }

        public NPCCondition(AssetFile file, int blueprintIndex, int conditionIndex)
        {
            string prefix = $"Blueprint_{blueprintIndex}_Condition_{conditionIndex}";
            ConditionType = file.GetEnumType(prefix + "_Type", ENPCConditionType.NONE);
            if (ConditionType == ENPCConditionType.NONE)
            {
                MessageBox.Show("Unknown or missing condition type " + (file.GetProperty(prefix + "_Type") ?? "null") + " for Blueprint " + blueprintIndex.ToString() + " NPC Condition " + conditionIndex.ToString());
                LocalText = null;
                ShouldReset = false;
                LogicType = ENPCLogicType.NONE;
                Condition = null;
            }
            else
            {
                LocalText = file.GetLocalProperty(prefix);
                ShouldReset = file.HasProperty(prefix + "_Reset");
                LogicType = file.GetEnumType<ENPCLogicType>(prefix + "_Logic", default);
                Condition = ConditionType switch
                {   
                    ENPCConditionType.EXPERIENCE =>             new NPCExperienceConditionData(file, prefix),
                    ENPCConditionType.REPUTATION =>             new NPCReputationConditionData(file, prefix),
                    ENPCConditionType.FLAG_BOOL =>              new NPCBoolFlagConditionData(file, prefix),
                    ENPCConditionType.FLAG_SHORT =>             new NPCShortFlagConditionData(file, prefix),
                    ENPCConditionType.QUEST =>                  new NPCQuestConditionData(file, prefix),
                    ENPCConditionType.SKILLSET =>               new NPCSkillsetConditionData(file, prefix),
                    ENPCConditionType.ITEM =>                   new NPCItemConditionData(file, prefix),
                    ENPCConditionType.KILLS_ZOMBIE =>           new NPCZombieKillsConditionData(file, prefix),
                    ENPCConditionType.KILLS_HORDE =>            new NPCHordeKillsConditionData(file, prefix),
                    ENPCConditionType.KILLS_ANIMAL =>           new NPCAnimalKillsConditionData(file, prefix),
                    ENPCConditionType.COMPARE_FLAGS =>          new NPCCompareFlagsConditionData(file, prefix),
                    ENPCConditionType.TIME_OF_DAY =>            new NPCTimeOfDayConditionData(file, prefix),
                    ENPCConditionType.PLAYER_LIFE_HEALTH =>     new NPCPlayerLifeHealthConditionData(file, prefix),
                    ENPCConditionType.PLAYER_LIFE_FOOD =>       new NPCPlayerLifeFoodConditionData(file, prefix),
                    ENPCConditionType.PLAYER_LIFE_WATER =>      new NPCPlayerLifeWaterConditionData(file, prefix),
                    ENPCConditionType.PLAYER_LIFE_VIRUS =>      new NPCPlayerLifeVirusConditionData(file, prefix),
                    ENPCConditionType.HOLIDAY =>                new NPCHolidayConditionData(file, prefix),
                    ENPCConditionType.KILLS_PLAYER =>           new NPCPlayerKillsConditionData(file, prefix),
                    ENPCConditionType.KILLS_OBJECT =>           new NPCObjectKillsConditionData(file, prefix),
                    ENPCConditionType.CURRENCY =>               new NPCCurrencyConditionData(file, prefix),
                    ENPCConditionType.KILLS_TREE =>             new NPCTreeKillsConditionData(file, prefix),
                    ENPCConditionType.WEATHER_STATUS =>         new NPCTreeKillsConditionData(file, prefix),
                    ENPCConditionType.WEATHER_BLEND_ALPHA =>    new NPCTreeKillsConditionData(file, prefix),
                    _ =>                                        null
                };
            }
        }
        #region CONDITIONS
        public abstract class NPCConditionData { }
        public class NPCExperienceConditionData : NPCConditionData
        {
            public uint Experience;
            public NPCExperienceConditionData(AssetFile file, string prefix)
            {
                Experience = file.GetUIntegerType(prefix + "_Value");
            }
        }
        public class NPCReputationConditionData : NPCConditionData
        {
            public uint Reputation;
            public NPCReputationConditionData(AssetFile file, string prefix)
            {
                Reputation = file.GetUIntegerType(prefix + "_Value");
            }
        }
        public class NPCFlagConditionData : NPCConditionData
        {
            public ushort FlagID;
            public bool AllowUnset;
            public NPCFlagConditionData(AssetFile file, string prefix)
            {
                FlagID = file.GetUShortType(prefix + "_ID");
                AllowUnset = file.HasProperty(prefix + "_Allow_Unset");
            }
        }
        public class NPCBoolFlagConditionData : NPCFlagConditionData
        {
            public bool Value;
            public NPCBoolFlagConditionData(AssetFile file, string prefix) : base (file, prefix)
            {
                Value = file.GetBooleanType(prefix + "_Value");
            }
        }
        public class NPCShortFlagConditionData : NPCFlagConditionData
        {
            public short Value;
            public NPCShortFlagConditionData(AssetFile file, string prefix) : base (file, prefix)
            {
                Value = file.GetShortType(prefix + "_Value");
            }
        }
        public class NPCQuestConditionData : NPCConditionData
        {
            public ushort QuestID;
            public ENPCQuestStatus QuestStatus;
            public bool IgnoreNPC;
            public NPCQuestConditionData(AssetFile file, string prefix)
            {
                QuestID = file.GetUShortType(prefix + "_ID");
                QuestStatus = file.GetEnumType<ENPCQuestStatus>(prefix + "_Status");
                IgnoreNPC = file.HasProperty(prefix + "_Ignore_NPC");
            }
        }
        public class NPCSkillsetConditionData : NPCConditionData
        {
            public EPlayerSkillset Skillset;
            public NPCSkillsetConditionData(AssetFile file, string prefix)
            {
                Skillset = file.GetEnumType<EPlayerSkillset>(prefix + "_Value");
            }
        }
        public class NPCItemConditionData : NPCConditionData
        {
            public Guid ItemGUID;
            public ushort ItemID;
            public ushort Amount;
            public NPCItemConditionData(AssetFile file, string prefix)
            {
                string k = prefix + "_ID";
                ItemGUID = file.GetGUIDType(k);
                ItemID = file.GetUShortType(k);
                Amount = file.GetUShortType(prefix + "_Amount");
            }
        }
        public class NPCZombieKillsConditionData : NPCConditionData
        {
            public ushort ID;
            public short Value;
            public EZombieSpeciality ZombieType;
            public bool Spawn;
            public int SpawnQuantity;
            public byte NavID;
            public float MaxRadius;
            public float MinRadius;
            public NPCZombieKillsConditionData(AssetFile file, string prefix)
            {
                ID = file.GetUShortType(prefix + "_ID");
                Value = file.GetShortType(prefix + "_Value");
                ZombieType = file.GetEnumType(prefix + "_Zombie", EZombieSpeciality.NONE);
                SpawnQuantity = file.GetIntegerType(prefix + "_Spawn_Quantity");
                NavID = file.GetByteType(prefix + "_Nav", byte.MaxValue);
                MaxRadius = file.GetFloatType(prefix + "_Radius", 512f);
                MinRadius = file.GetFloatType(prefix + "_MinRadius", 512f);
                Spawn = file.HasProperty(prefix + "_Spawn");
            }
        }
        public class NPCHordeKillsConditionData : NPCConditionData
        {
            public ushort ID;
            public short Value;
            public byte NavID;
            public NPCHordeKillsConditionData(AssetFile file, string prefix)
            {
                ID = file.GetUShortType(prefix + "_ID");
                Value = file.GetShortType(prefix + "_Value");
                NavID = file.GetByteType(prefix + "_Nav");
            }
        }
        public class NPCAnimalKillsConditionData : NPCConditionData
        {
            public ushort ID;
            public short Value;
            public ushort Animal;
            public NPCAnimalKillsConditionData(AssetFile file, string prefix)
            {
                ID = file.GetUShortType(prefix + "_ID");
                Value = file.GetShortType(prefix + "_Value");
                Animal = file.GetByteType(prefix + "_Animal");
            }
        }
        public class NPCCompareFlagsConditionData : NPCConditionData
        {
            public ushort A;
            public ushort B;
            public bool AllowAUnset;
            public bool AllowBUnset;
            public NPCCompareFlagsConditionData(AssetFile file, string prefix)
            {
                A = file.GetUShortType(prefix + "_A_ID");
                B = file.GetUShortType(prefix + "_B_ID");
                AllowAUnset = file.HasProperty(prefix + "_Allow_A_Unset");
                AllowBUnset = file.HasProperty(prefix + "_Allow_B_Unset");
            }
        }
        public class NPCTimeOfDayConditionData : NPCConditionData
        {
            public int TimeOfDaySeconds;
            public NPCTimeOfDayConditionData(AssetFile file, string prefix)
            {
                TimeOfDaySeconds = file.GetIntegerType(prefix + "_Second");
            }
        }
        public class NPCPlayerLifeHealthConditionData : NPCConditionData
        {
            public int Value;
            public NPCPlayerLifeHealthConditionData(AssetFile file, string prefix)
            {
                Value = file.GetIntegerType(prefix + "_Value");
            }
        }
        public class NPCPlayerLifeFoodConditionData : NPCConditionData
        {
            public int Value;
            public NPCPlayerLifeFoodConditionData(AssetFile file, string prefix)
            {
                Value = file.GetIntegerType(prefix + "_Value");
            }
        }
        public class NPCPlayerLifeWaterConditionData : NPCConditionData
        {
            public int Value;
            public NPCPlayerLifeWaterConditionData(AssetFile file, string prefix)
            {
                Value = file.GetIntegerType(prefix + "_Value");
            }
        }
        public class NPCPlayerLifeVirusConditionData : NPCConditionData
        {
            public int Value;
            public NPCPlayerLifeVirusConditionData(AssetFile file, string prefix)
            {
                Value = file.GetIntegerType(prefix + "_Value");
            }
        }
        public class NPCHolidayConditionData : NPCConditionData
        {
            public ENPCHoliday Holiday;
            public NPCHolidayConditionData(AssetFile file, string prefix)
            {
                Holiday = file.GetEnumType(prefix + "_Value", ENPCHoliday.NONE);
            }
        }
        public class NPCPlayerKillsConditionData : NPCConditionData
        {
            public ushort ID;
            public short Threshold;
            public NPCPlayerKillsConditionData(AssetFile file, string prefix)
            {
                ID = file.GetUShortType(prefix + "_ID");
                Threshold = file.GetShortType(prefix + "_Value");
            }
        }
        public class NPCObjectKillsConditionData : NPCConditionData
        {
            public ushort ID;
            public short Threshold;
            public Guid ObjectGUID;
            public byte Nav;
            public NPCObjectKillsConditionData(AssetFile file, string prefix)
            {
                ID = file.GetUShortType(prefix + "_ID");
                Threshold = file.GetShortType(prefix + "_Value");
                ObjectGUID = file.GetGUIDType(prefix + "_Object");
                Nav = file.GetByteType(prefix + "_Nav", byte.MaxValue);
            }
        }
        public class NPCCurrencyConditionData : NPCConditionData
        {
            public Guid CurrencyItemGUID;
            public uint Threshold;
            public NPCCurrencyConditionData(AssetFile file, string prefix)
            {
                CurrencyItemGUID = file.GetGUIDType(prefix + "_GUID");
                Threshold = file.GetUIntegerType(prefix + "_Value");
            }
        }
        public class NPCTreeKillsConditionData : NPCConditionData
        {
            public ushort ID;
            public Guid TreeGUID;
            public short Threshold;
            public NPCTreeKillsConditionData(AssetFile file, string prefix)
            {
                ID = file.GetUShortType(prefix + "_ID");
                Threshold = file.GetShortType(prefix + "_Value");
                TreeGUID = file.GetGUIDType(prefix + "_Tree");
            }
        }
        public class NPCWeatherStatusConditionData : NPCConditionData
        {
            public Guid WeatherGUID;
            public ENPCWeatherStatus WeatherType;
            public NPCWeatherStatusConditionData(AssetFile file, string prefix)
            {
                WeatherGUID = file.GetGUIDType(prefix + "_GUID");
                WeatherType = file.GetEnumType(prefix + "_Value", ENPCWeatherStatus.Active);
            }
        }
        public class NPCWeatherBlendAlphaConditionData : NPCConditionData
        {
            public Guid WeatherGUID;
            public float Threshold;
            public NPCWeatherBlendAlphaConditionData(AssetFile file, string prefix)
            {
                WeatherGUID = file.GetGUIDType(prefix + "_GUID");
                Threshold = file.GetFloatType(prefix + "_Value");
            }
        }
        #endregion

        public override string ToString() => $"NPC {ConditionType} logical {LogicType} condition (" + (LocalText ?? "null") + ").";
    }
    public struct NPCReward
    {
        public ENPCRewardType RewardType;
        public string? LocalText;
        public NPCRewardData? Reward;

        public NPCReward(AssetFile file, int blueprintIndex, int rewardIndex)
        {
            string prefix = $"Blueprint_{blueprintIndex}_Reward_{rewardIndex}";
            RewardType = file.GetEnumType(prefix + "_Type", ENPCRewardType.NONE);
            if (RewardType == ENPCRewardType.NONE)
            {
                MessageBox.Show("Unknown or missing reward type " + (file.GetProperty(prefix + "_Type") ?? "null") + " for Blueprint " + blueprintIndex.ToString() + " NPC Reward " + rewardIndex.ToString());
                LocalText = null;
                Reward = null;
            }
            else
            {
                LocalText = file.GetLocalProperty(prefix);
                Reward = RewardType switch
                {
                    ENPCRewardType.EXPERIENCE => new NPCExperienceRewardData(file, prefix),
                    ENPCRewardType.REPUTATION => new NPCReputationRewardData(file, prefix),
                    ENPCRewardType.FLAG_BOOL => new NPCBoolFlagRewardData(file, prefix),
                    ENPCRewardType.FLAG_SHORT => new NPCShortFlagRewardData(file, prefix),
                    ENPCRewardType.FLAG_SHORT_RANDOM => new NPCRandomShortFlagRewardData(file, prefix),
                    ENPCRewardType.QUEST => new NPCQuestRewardData(file, prefix),
                    ENPCRewardType.ITEM => new NPCItemRewardData(file, prefix),
                    ENPCRewardType.ITEM_RANDOM => new NPCRandomItemRewardData(file, prefix),
                    ENPCRewardType.ACHIEVEMENT => new NPCAchievementRewardData(file, prefix),
                    ENPCRewardType.VEHICLE => new NPCVehicleRewardData(file, prefix),
                    ENPCRewardType.TELEPORT => new NPCTeleportRewardData(file, prefix),
                    ENPCRewardType.EVENT => new NPCEventRewardData(file, prefix),
                    ENPCRewardType.FLAG_MATH => new NPCFlagMathRewardData(file, prefix),
                    ENPCRewardType.CURRENCY => new NPCCurrencyRewardData(file, prefix),
                    ENPCRewardType.HINT => new NPCHintRewardData(file, prefix),
                    _ => null
                };
            }
        }

        public NPCReward(ENPCRewardType rewardType, string? localText, NPCRewardData? reward)
        {
            RewardType = rewardType;
            LocalText = localText;
            Reward = reward;
        }
        #region REWARDS
        public abstract class NPCRewardData { }
        public class NPCExperienceRewardData : NPCRewardData
        {
            public uint Value;
            public NPCExperienceRewardData(AssetFile file, string prefix)
            {
                Value = file.GetUIntegerType(prefix + "_Value");
            }
        }
        public class NPCReputationRewardData : NPCRewardData
        {
            public uint Value;
            public NPCReputationRewardData(AssetFile file, string prefix)
            {
                Value = file.GetUIntegerType(prefix + "_Value");
            }
        }
        public class NPCBoolFlagRewardData : NPCRewardData
        {
            public ushort ID;
            public bool Value;
            public NPCBoolFlagRewardData(AssetFile file, string prefix)
            {
                ID = file.GetUShortType(prefix + "_ID");
                Value = file.GetBooleanType(prefix + "_Value");
            }
        }
        public class NPCShortFlagRewardData : NPCRewardData
        {
            public ushort ID;
            public short Value;
            public ENPCModificationType ModificationType;
            public NPCShortFlagRewardData(AssetFile file, string prefix)
            {
                ID = file.GetUShortType(prefix + "_ID");
                Value = file.GetShortType(prefix + "_Value");
                ModificationType = file.GetEnumType(prefix + "_Modification", ENPCModificationType.NONE);
            }
        }
        public class NPCRandomShortFlagRewardData : NPCRewardData
        {
            public ushort ID;
            public short MinValue;
            public short MaxValue;
            public ENPCModificationType ModificationType;
            public NPCRandomShortFlagRewardData(AssetFile file, string prefix)
            {
                ID = file.GetUShortType(prefix + "_ID");
                MinValue = file.GetShortType(prefix + "_Min_Value");
                MaxValue = file.GetShortType(prefix + "_Max_Value");
                ModificationType = file.GetEnumType(prefix + "_Modification", ENPCModificationType.NONE);
            }
        }
        public class NPCQuestRewardData : NPCRewardData
        {
            public ushort ID;
            public NPCQuestRewardData(AssetFile file, string prefix)
            {
                ID = file.GetUShortType(prefix + "_ID");
            }
        }
        public class NPCItemRewardData : NPCRewardData
        {
            public ushort ItemID;
            public Guid ItemGUID;
            public byte Quantity;
            public bool ShouldAutoEquip;
            public ushort Sight;
            public ushort Tactical;
            public ushort Grip;
            public ushort Barrel;
            public ushort Magazine;
            public ushort Ammo;
            public NPCItemRewardData(AssetFile file, string prefix)
            {
                ItemID = file.GetUShortType(prefix + "_ID");
                ItemGUID = file.GetGUIDType(prefix + "_ID");
                ShouldAutoEquip = file.GetBooleanType(prefix + "_Auto_Equip");
                Quantity = file.GetByteType(prefix + "_Amount");

                Sight = file.GetUShortType(prefix + "_Sight");
                Tactical = file.GetUShortType(prefix + "_Tactical");
                Grip = file.GetUShortType(prefix + "_Grip");
                Barrel = file.GetUShortType(prefix + "_Barrel");
                Magazine = file.GetUShortType(prefix + "_Magazine");

                Ammo = file.GetByteType(prefix + "_Ammo");
            }
        }
        public class NPCRandomItemRewardData : NPCRewardData
        {
            public ushort SpawnID;
            public byte Amount;
            public bool ShouldAutoEquip;
            public NPCRandomItemRewardData(AssetFile file, string prefix)
            {
                SpawnID = file.GetUShortType(prefix + "_ID");
                Amount = file.GetByteType(prefix + "_Amount");
                ShouldAutoEquip = file.GetBooleanType(prefix + "_Auto_Equip");
            }
        }
        public class NPCAchievementRewardData : NPCRewardData
        {
            public string? AchievementID;
            public NPCAchievementRewardData(AssetFile file, string prefix)
            {
                AchievementID = file.GetProperty(prefix + "_ID");
            }
        }
        public class NPCVehicleRewardData : NPCRewardData
        {
            public ushort VehicleID;
            public string? Spawnpoint;
            public NPCVehicleRewardData(AssetFile file, string prefix)
            {
                VehicleID = file.GetUShortType(prefix + "_ID");
                Spawnpoint = file.GetProperty(prefix + "_Spawnpoint");
            }
        }
        public class NPCTeleportRewardData : NPCRewardData
        {
            public string? Spawnpoint;
            public NPCTeleportRewardData(AssetFile file, string prefix)
            {
                Spawnpoint = file.GetProperty(prefix + "_Spawnpoint");
            }
        }
        public class NPCEventRewardData : NPCRewardData
        {
            public string? EventID;
            public NPCEventRewardData(AssetFile file, string prefix)
            {
                EventID = file.GetProperty(prefix + "_ID");
            }
        }
        public class NPCFlagMathRewardData : NPCRewardData
        {
            public ushort A;
            public ushort B;
            public ENPCOperationType OperationType;
            public NPCFlagMathRewardData(AssetFile file, string prefix)
            {
                A = file.GetUShortType(prefix + "_A_ID");
                B = file.GetUShortType(prefix + "_B_ID");
                OperationType = file.GetEnumType(prefix + "_Operation", ENPCOperationType.NONE);
            }
        }
        public class NPCCurrencyRewardData : NPCRewardData
        {
            public Guid CurrencyItemGUID;
            public uint Amount;
            public NPCCurrencyRewardData(AssetFile file, string prefix)
            {
                CurrencyItemGUID = file.GetGUIDType(prefix + "_GUID");
                Amount = file.GetUIntegerType(prefix + "_Value");
            }
        }
        public class NPCHintRewardData : NPCRewardData
        {
            public string? Text;
            public float Duration;
            public NPCHintRewardData(AssetFile file, string prefix)
            {
                Text = file.GetProperty(prefix + "_Text");
                Duration = file.GetFloatType(prefix + "_Duration", 2f);
            }
        }
        #endregion
        public override string ToString() => $"NPC {RewardType} reward (" + (LocalText ?? "null") + ").";
    }

    public override string ToString() => $"{BlueprintType} Blueprint, {Supplies.Count} supplies, " +
                                         $"{Outputs.Count} outputs{(Conditions.Count > 0 || Rewards.Count > 0 ? $", {Conditions.Count} conditions, {Rewards.Count} rewards" : string.Empty)}. " +
                                         (Tool.ID == 0 ? "No tool" : ("Tool: " + Tool.ToString())) + ", Level: " + Level.ToString() + ", Skill: " + Skill.ToString() + 
                                         ", KeepState: " + TransferState.ToString() + (Map == null ? string.Empty : ", Only works on " + Map);
}
public struct ActionData
{
    public EActionType ActionType;
    public List<ActionBlueprint> Blueprints;
    public string? LocalizedKey;
    public string? ExplicitText;
    public string? ExplicitTooltip;
    public ushort SourceItem;
    public ActionData(EActionType actionType, List<ActionBlueprint> blueprints, string? localizedKey, string? explicitText, string? explicitTooltip, ushort sourceItem)
    {
        ActionType = actionType;
        Blueprints = blueprints;
        LocalizedKey = localizedKey;
        ExplicitText = explicitText;
        ExplicitTooltip = explicitTooltip;
        SourceItem = sourceItem;
    }
    public ActionData(AssetFile file, int actionIndex, ushort itemID)
    {
        string prefix = $"Action_{actionIndex}_";
        ActionType = file.GetEnumType(prefix + "Type", EActionType.BLUEPRINT);
        int length = file.GetIntegerTypeClampTo1(prefix + "Blueprints");
        Blueprints = new List<ActionBlueprint>(length);
        for (int i = 0; i < length; i++)
            Blueprints.Add(new ActionBlueprint(file, actionIndex, i));
        LocalizedKey = file.GetProperty(prefix + "Key");
        if (string.IsNullOrEmpty(LocalizedKey))
        {
            string key1 = prefix + "Text";
            string key2 = prefix + "Tooltip";
            ExplicitText = file.GetLocalProperty(key1) ?? file.GetProperty(key1);
            ExplicitTooltip = file.GetLocalProperty(key2) ?? file.GetProperty(key2);
        }
        else
        {
            ExplicitText = null;
            ExplicitTooltip = null;
        }

        SourceItem = file.GetUShortType(prefix + "Source", itemID);
    }
    public struct ActionBlueprint
    {
        public byte Index;
        public bool Link;
        public ActionBlueprint(byte index, bool link)
        {
            Index = index;
            Link = link;
        }
        public ActionBlueprint(AssetFile file, int actionIndex, int blueprintIndex)
        {
            string prefix = $"Action_{actionIndex}_Blueprint_{blueprintIndex}_";
            Index = file.GetByteType(prefix + "Index");
            Link = file.HasProperty(prefix + "Link");
        }

        public override string ToString() => $"Action Blueprint Link to index " + Index + (Link ? ". Crafts on click." : string.Empty);
    }

    public override string ToString() => $"{ActionType} Action, {Blueprints.Count} blueprints, " +
                                         (string.IsNullOrEmpty(LocalizedKey)
                                             ? $"Null key, text: {ExplicitText ?? "null"}, tooltip: {ExplicitText ?? "null"})"
                                             : ("Key: " + LocalizedKey)) +
                                         $", SourceItem: " + SourceItem.ToString();
}