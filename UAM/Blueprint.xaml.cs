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
        AddConditionButton.Click += OnAddConditionClicked;
        AddRewardButton.Click += OnAddRewardClicked;
    }
    
    private void OnAddSupplyClicked(object sender, RoutedEventArgs e)
    {
        AddSupply(-1, supplyControls.Count);
    }
    private void OnAddOutputClicked(object sender, RoutedEventArgs e)
    {
        AddOutput(-1, outputControls.Count);
    }
    private void OnAddConditionClicked(object sender, RoutedEventArgs e)
    {
        AddCondition(-1, conditionControls.Count);
    }
    private void OnAddRewardClicked(object sender, RoutedEventArgs e)
    {
        AddReward(-1, conditionControls.Count);
    }

    private static readonly EItemOrigin[]       ORIGIN_VALUES       = Enum.GetValues(typeof(EItemOrigin))       .OfType<EItemOrigin>()      .OrderBy(x => (int)x.ToString()[0]).ToArray();
    private static readonly ENPCLogicType[]     LOGIC_VALUES        = Enum.GetValues(typeof(ENPCLogicType))     .OfType<ENPCLogicType>()    .OrderBy(x => (int)x.ToString()[0]).ToArray();
    private static readonly ENPCConditionType[] CONDITIONS_VALUES   = Enum.GetValues(typeof(ENPCConditionType)) .OfType<ENPCConditionType>().OrderBy(x => (int)x.ToString()[0]).ToArray();
    private static readonly ENPCRewardType[]    REWARD_VALUES       = Enum.GetValues(typeof(ENPCRewardType))    .OfType<ENPCRewardType>()   .OrderBy(x => (int)x.ToString()[0]).ToArray();

    private BlueprintData data;

    private static readonly GridLength smlLen   = new GridLength(50d, GridUnitType.Pixel);
    private static readonly GridLength lrgLen   = new GridLength(260d, GridUnitType.Pixel);
    private static readonly GridLength strLen   = new GridLength(1d, GridUnitType.Star);
    private static readonly GridLength valGrid1 = new GridLength(2d, GridUnitType.Star);
    private static readonly GridLength valGrid2 = new GridLength(3d, GridUnitType.Star);

    private readonly List<SupplyControls> supplyControls        = new List<SupplyControls>(4);
    private readonly List<OutputControls> outputControls        = new List<OutputControls>(4);
    private readonly List<ConditionControls> conditionControls  = new List<ConditionControls>(4);
    private readonly List<RewardControls> rewardControls        = new List<RewardControls>(4);

    private struct SupplyControls
    {
        public int index;
        public int supplyIndex;
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
        public int outputIndex;
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
    private struct ConditionControls
    {
        public int index;
        public int conditionIndex;
        public StackPanel leftHost;
        public Button removeButton;
        public TextBlock number;
        public Grid valuesGrid;
        public TextBlock typeLbl;
        public TextBlock localKeyLbl;
        public TextBlock shouldResetLbl;
        public TextBlock logicTypeLbl;
        public TextBox localKeyTxt;
        public CheckBox shouldResetChk;
        public ComboBox typeCbo;
        public ComboBox logicTypeCbo;
        public List<FrameworkElement> extra;
        public BlueprintData.NPCCondition.NPCConditionData? conditionDataPending;
    }
    private struct RewardControls
    {
        public int index;
        public int rewardIndex;
        public StackPanel leftHost;
        public Button removeButton;
        public TextBlock number;
        public Grid valuesGrid;
        public TextBlock typeLbl;
        public TextBlock localKeyLbl;
        public TextBox localKeyTxt;
        public ComboBox typeCbo;
        public List<FrameworkElement> extra;
    }
    public void LoadBlueprint(BlueprintData data, int index)
    {
        this.data = data;
        Title.Text = "Blueprint #" + (index + 1).ToString();
        SuppliesTabGrid.RowDefinitions.Clear();
        for (int i = 0; i < this.data.Supplies.Count; i++)
            AddSupply(i, i);
        SuppliesTabGrid.RowDefinitions.Add(new RowDefinition() { Height = strLen });

        OutputsTabGrid.RowDefinitions.Clear();
        for (int i = 0; i < this.data.Outputs.Count; i++)
            AddOutput(i, i);
        OutputsTabGrid.RowDefinitions.Add(new RowDefinition() { Height = strLen });

        ConditionsTabGrid.RowDefinitions.Clear();
        for (int i = 0; i < this.data.Conditions.Count; i++)
            AddCondition(i, i);
        ConditionsTabGrid.RowDefinitions.Add(new RowDefinition() { Height = strLen });
    }

    private const string CRITICAL_TOOLTIP = "Marks this supply as required for the recipe to show up.";

    private void AddSupply(int supplyIndex, int index)
    {
        SuppliesTabGrid.RowDefinitions.Insert(0, new RowDefinition() { Height = smlLen });
        Grid.SetRow(AddSupplyButton, this.supplyControls.Count + 1);
        SupplyControls supplyControls = new SupplyControls() { index = index, supplyIndex = -1 };
        BlueprintData.SupplyData supplyData = default;
        if (supplyIndex > -1 && supplyIndex < this.data.Supplies.Count)
            supplyData = this.data.Supplies[supplyIndex];
        supplyControls.leftHost = new StackPanel()
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
        };
        Grid.SetRow(supplyControls.leftHost, index);
        Grid.SetColumn(supplyControls.leftHost, 0);
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
        bool isVal = supplyData.ID != 0;
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
            supplyControls.idTxt.Text = supplyData.ID.ToString(AssetFile.Locale);
            supplyControls.quantityTxt.Text = supplyData.Amount.ToString(AssetFile.Locale);
            supplyControls.criticalChk.IsChecked = supplyData.Critical;
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
    private void AddOutput(int outputIndex, int index)
    {
        OutputsTabGrid.RowDefinitions.Insert(0, new RowDefinition() { Height = smlLen });
        Grid.SetRow(AddOutputButton, this.outputControls.Count + 1);
        OutputControls outputControls = new OutputControls() { index = index, outputIndex = outputIndex };

        BlueprintData.OutputData outputData = default;
        if (outputIndex > -1 && outputIndex < this.data.Outputs.Count)
            outputData = this.data.Outputs[outputIndex];

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
        bool isVal = outputData.ID != 0;
        if (isVal)
        {
            outputControls.idTxt.Text = outputData.ID.ToString(AssetFile.Locale);
            outputControls.quantityTxt.Text = outputData.Amount.ToString(AssetFile.Locale);
            Panel.SelectEnum(outputData.Origin, outputControls.originCbo);
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

    private const string RESET_TOOLTIP = "Allows the condition to be reset after reaching it.";
    private void AddCondition(int conditionIndex, int index)
    {
        ConditionsTabGrid.RowDefinitions.Insert(0, new RowDefinition() { Height = lrgLen });
        Grid.SetRow(AddConditionButton, this.conditionControls.Count + 1);
        ConditionControls conditionControls = new ConditionControls() { index = index, conditionIndex = conditionIndex, extra = new List<FrameworkElement>(4) };

        BlueprintData.NPCCondition conditionData = default;
        if (conditionIndex > -1 && conditionIndex < this.data.Conditions.Count)
            conditionData = this.data.Conditions[conditionIndex];
        else
            conditionData.ConditionType = ENPCConditionType.NONE;

        conditionControls.leftHost = new StackPanel()
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
        };
        Grid.SetRow(conditionControls.leftHost, index);
        Grid.SetColumn(conditionControls.leftHost, 0);
        conditionControls.removeButton = new Button()
        {
            Content = "Remove",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0d, 0d, 10d, 0d),
            BorderBrush = Brushes.Red
        };
        conditionControls.removeButton.Click += OnRemoveConditionButtonClicked;
        conditionControls.leftHost.Children.Add(conditionControls.removeButton);
        conditionControls.number = new TextBlock()
        {
            Text = "#" + (index + 1).ToString(),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        conditionControls.leftHost.Children.Add(conditionControls.number);
        ConditionsTabGrid.Children.Add(conditionControls.leftHost);
        conditionControls.valuesGrid = new Grid()
        {
            VerticalAlignment = VerticalAlignment.Top
        };
        conditionControls.valuesGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = valGrid1 });
        conditionControls.valuesGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = valGrid2 });
        conditionControls.valuesGrid.RowDefinitions.Add(new RowDefinition() { Height = strLen });
        conditionControls.valuesGrid.RowDefinitions.Add(new RowDefinition() { Height = strLen });
        conditionControls.valuesGrid.RowDefinitions.Add(new RowDefinition() { Height = strLen });
        conditionControls.valuesGrid.RowDefinitions.Add(new RowDefinition() { Height = strLen });
        Grid.SetColumn(conditionControls.valuesGrid, 1);
        Grid.SetRow(conditionControls.valuesGrid, index);
        conditionControls.typeLbl = new TextBlock()
        {
            Text = "Type",
            FontSize = 10
        };
        conditionControls.valuesGrid.Children.Add(conditionControls.typeLbl);
        conditionControls.localKeyLbl = new TextBlock()
        {
            Text = "Quantity",
            FontSize = 10
        };
        Grid.SetRow(conditionControls.localKeyLbl, 1);
        conditionControls.valuesGrid.Children.Add(conditionControls.localKeyLbl);
        conditionControls.shouldResetLbl = new TextBlock()
        {
            Text = "Is Critical",
            FontSize = 10,
            ToolTip = RESET_TOOLTIP
        };
        Grid.SetRow(conditionControls.shouldResetLbl, 2);
        conditionControls.valuesGrid.Children.Add(conditionControls.shouldResetLbl);
        conditionControls.logicTypeLbl = new TextBlock()
        {
            Text = "Logic Type",
            FontSize = 10
        };
        Grid.SetRow(conditionControls.logicTypeLbl, 3);
        conditionControls.valuesGrid.Children.Add(conditionControls.logicTypeLbl);
        bool isVal = conditionData.ConditionType != ENPCConditionType.NONE;
        conditionControls.typeCbo = new ComboBox();
        for (int i = 0; i < CONDITIONS_VALUES.Length; i++)
        {
            conditionControls.typeCbo.Items.Add(new ComboBoxItem()
            {
                Content = Panel.ToProperCase(CONDITIONS_VALUES[i].ToString())
            });
        }
        conditionControls.typeCbo.SelectionChanged += OnConditionTypeChanged;
        Grid.SetColumn(conditionControls.typeCbo, 1);
        conditionControls.valuesGrid.Children.Add(conditionControls.typeCbo);
        conditionControls.localKeyTxt = new TextBox();
        Grid.SetColumn(conditionControls.localKeyTxt, 1);
        Grid.SetRow(conditionControls.localKeyTxt, 1);
        conditionControls.valuesGrid.Children.Add(conditionControls.localKeyTxt);
        conditionControls.shouldResetChk = new CheckBox()
        {
            ToolTip = RESET_TOOLTIP
        };
        Grid.SetColumn(conditionControls.shouldResetChk, 1);
        Grid.SetRow(conditionControls.shouldResetChk, 2);
        conditionControls.valuesGrid.Children.Add(conditionControls.shouldResetChk);
        conditionControls.logicTypeCbo = new ComboBox();
        for (int i = 0; i < LOGIC_VALUES.Length; i++)
        {
            conditionControls.logicTypeCbo.Items.Add(new ComboBoxItem()
            {
                Content = Panel.ToProperCase(LOGIC_VALUES[i].ToString())
            });
        }
        Grid.SetColumn(conditionControls.logicTypeCbo, 1);
        Grid.SetRow(conditionControls.logicTypeCbo, 3);
        conditionControls.valuesGrid.Children.Add(conditionControls.logicTypeCbo);
        if (isVal)
        {
            Panel.SelectEnum(conditionData.ConditionType, conditionControls.typeCbo);
            conditionControls.shouldResetChk.IsChecked = conditionData.ShouldReset;
            Panel.SelectEnum(conditionData.LogicType, conditionControls.logicTypeCbo);
            conditionControls.localKeyTxt.Text = conditionData.LocalText ?? string.Empty;
        }
        ConditionsTabGrid.Children.Add(conditionControls.valuesGrid);
        this.conditionControls.Add(conditionControls);
    }
    public void OnConditionTypeChanged(object sender, SelectionChangedEventArgs args)
    {
        if (sender is ComboBox typeCbo)
        {
            for (int i = 0; i < conditionControls.Count; i++)
            {
                if (conditionControls[i].typeCbo == typeCbo)
                {
                    OnConditionTypeChanged(i);
                    return;
                }
            }
        }
    }

    private void OnConditionTypeChanged(int index)
    {
        ConditionControls ctrls = conditionControls[index];
        for (int i = ctrls.extra.Count - 1; i >= 0; i--)
        {
            ctrls.valuesGrid.Children.Remove(ctrls.extra[i]);
            ctrls.extra.RemoveAt(i);
        }

        ENPCConditionType newtype = Panel.GetSelectedEnum<ENPCConditionType>(ctrls.typeCbo);
        int row = 4;
        int ct = ctrls.valuesGrid.RowDefinitions.Count;
        for (; ct > 4; ct--)
            ctrls.valuesGrid.RowDefinitions.RemoveAt(ct - 1);
        if (ctrls.conditionIndex > -1 && ctrls.conditionIndex < data.Conditions.Count && data.Conditions[ctrls.conditionIndex].ConditionType == newtype && data.Conditions[ctrls.conditionIndex].Condition != null)
        {
            data.Conditions[ctrls.conditionIndex].Condition!.AddChildren(ctrls.valuesGrid, ref row, ctrls.extra);
        }
        else
        {
            ctrls.conditionDataPending = newtype switch
                {
                    ENPCConditionType.EXPERIENCE => new BlueprintData.NPCCondition.NPCExperienceConditionData(),
                    ENPCConditionType.REPUTATION => new BlueprintData.NPCCondition.NPCReputationConditionData(),
                    ENPCConditionType.FLAG_BOOL => new BlueprintData.NPCCondition.NPCBoolFlagConditionData(),
                    ENPCConditionType.FLAG_SHORT => new BlueprintData.NPCCondition.NPCShortFlagConditionData(),
                    ENPCConditionType.QUEST => new BlueprintData.NPCCondition.NPCQuestConditionData(),
                    ENPCConditionType.SKILLSET => new BlueprintData.NPCCondition.NPCSkillsetConditionData(),
                    ENPCConditionType.ITEM => new BlueprintData.NPCCondition.NPCItemConditionData(),
                    ENPCConditionType.KILLS_ZOMBIE => new BlueprintData.NPCCondition.NPCZombieKillsConditionData(),
                    ENPCConditionType.KILLS_HORDE => new BlueprintData.NPCCondition.NPCHordeKillsConditionData(),
                    ENPCConditionType.KILLS_ANIMAL => new BlueprintData.NPCCondition.NPCAnimalKillsConditionData(),
                    ENPCConditionType.COMPARE_FLAGS => new BlueprintData.NPCCondition.NPCCompareFlagsConditionData(),
                    ENPCConditionType.TIME_OF_DAY => new BlueprintData.NPCCondition.NPCTimeOfDayConditionData(),
                    ENPCConditionType.PLAYER_LIFE_HEALTH => new BlueprintData.NPCCondition.NPCPlayerLifeHealthConditionData(),
                    ENPCConditionType.PLAYER_LIFE_FOOD => new BlueprintData.NPCCondition.NPCPlayerLifeFoodConditionData(),
                    ENPCConditionType.PLAYER_LIFE_WATER => new BlueprintData.NPCCondition.NPCPlayerLifeWaterConditionData(),
                    ENPCConditionType.PLAYER_LIFE_VIRUS => new BlueprintData.NPCCondition.NPCPlayerLifeVirusConditionData(),
                    ENPCConditionType.HOLIDAY => new BlueprintData.NPCCondition.NPCHolidayConditionData(),
                    ENPCConditionType.KILLS_PLAYER => new BlueprintData.NPCCondition.NPCPlayerKillsConditionData(),
                    ENPCConditionType.KILLS_OBJECT => new BlueprintData.NPCCondition.NPCObjectKillsConditionData(),
                    ENPCConditionType.CURRENCY => new BlueprintData.NPCCondition.NPCCurrencyConditionData(),
                    ENPCConditionType.KILLS_TREE => new BlueprintData.NPCCondition.NPCTreeKillsConditionData(),
                    ENPCConditionType.WEATHER_STATUS => new BlueprintData.NPCCondition.NPCTreeKillsConditionData(),
                    ENPCConditionType.WEATHER_BLEND_ALPHA => new BlueprintData.NPCCondition.NPCTreeKillsConditionData(),
                    _ => null
                };
            ctrls.conditionDataPending?.AddChildren(ctrls.valuesGrid, ref row, ctrls.extra);
        }
        conditionControls[index] = ctrls;
    }
    public void OnRemoveConditionButtonClicked(object sender, RoutedEventArgs args)
    {
        if (sender is Button button)
        {
            for (int i = 0; i < conditionControls.Count; i++)
            {
                if (conditionControls[i].removeButton == button)
                {
                    RemoveCondition(conditionControls[i]);
                    return;
                }
            }
        }
    }
    private void RemoveCondition(ConditionControls controls)
    {
        ConditionsTabGrid.Children.Remove(controls.leftHost);
        foreach (UIElement element in controls.extra)
            controls.valuesGrid.Children.Remove(element);
        ConditionsTabGrid.Children.Remove(controls.valuesGrid);
        ConditionsTabGrid.RowDefinitions.RemoveAt(0);
        for (int i = controls.index + 1; i < conditionControls.Count; i++)
        {
            ConditionControls ctrls = conditionControls[i];
            ctrls.index -= 1;
            Grid.SetRow(ctrls.leftHost, ctrls.index);
            Grid.SetRow(ctrls.valuesGrid, ctrls.index);
            ctrls.number.Text = "#" + ctrls.index.ToString();
            conditionControls[i] = ctrls;
        }
        conditionControls.RemoveAt(controls.index);
        Grid.SetRow(AddConditionButton, this.conditionControls.Count);
    }

    private void AddReward(int rewardIndex, int index)
    {
        RewardsTabGrid.RowDefinitions.Insert(0, new RowDefinition() { Height = smlLen });
        Grid.SetRow(AddSupplyButton, this.supplyControls.Count + 1);
        RewardControls rewardControls = new RewardControls() { index = index, rewardIndex = rewardIndex, extra = new List<FrameworkElement>(4) };

        BlueprintData.NPCReward rewardData = default;
        if (rewardIndex > -1 && rewardIndex < this.data.Rewards.Count)
            rewardData = this.data.Rewards[rewardIndex];
        else
            rewardData.RewardType = ENPCRewardType.NONE;

        rewardControls.leftHost = new StackPanel()
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
        };
        Grid.SetRow(rewardControls.leftHost, index);
        Grid.SetColumn(rewardControls.leftHost, 0);
        rewardControls.removeButton = new Button()
        {
            Content = "Remove",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0d, 0d, 10d, 0d),
            BorderBrush = Brushes.Red
        };
        rewardControls.removeButton.Click += OnRemoveSupplyButtonClicked;
        rewardControls.leftHost.Children.Add(rewardControls.removeButton);
        rewardControls.number = new TextBlock()
        {
            Text = "#" + (index + 1).ToString(),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        rewardControls.leftHost.Children.Add(rewardControls.number);
        RewardsTabGrid.Children.Add(rewardControls.leftHost);
        rewardControls.valuesGrid = new Grid();
        rewardControls.valuesGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = valGrid1 });
        rewardControls.valuesGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = valGrid2 });
        rewardControls.valuesGrid.RowDefinitions.Add(new RowDefinition() { Height = strLen });
        rewardControls.valuesGrid.RowDefinitions.Add(new RowDefinition() { Height = strLen });
        rewardControls.valuesGrid.RowDefinitions.Add(new RowDefinition() { Height = strLen });
        rewardControls.valuesGrid.RowDefinitions.Add(new RowDefinition() { Height = strLen });
        Grid.SetColumn(rewardControls.valuesGrid, 1);
        Grid.SetRow(rewardControls.valuesGrid, index);
        rewardControls.typeLbl = new TextBlock()
        {
            Text = "Type",
            FontSize = 10
        };
        rewardControls.valuesGrid.Children.Add(rewardControls.typeLbl);
        rewardControls.localKeyLbl = new TextBlock()
        {
            Text = "Local Key",
            FontSize = 10
        };
        Grid.SetRow(rewardControls.localKeyLbl, 1);
        rewardControls.valuesGrid.Children.Add(rewardControls.localKeyLbl);
        bool isVal = rewardData.RewardType != ENPCRewardType.NONE;
        rewardControls.typeCbo = new ComboBox();
        for (int i = 0; i < REWARD_VALUES.Length; i++)
        {
            rewardControls.typeCbo.Items.Add(new ComboBoxItem()
            {
                Content = Panel.ToProperCase(REWARD_VALUES[i].ToString())
            });
        }
        Grid.SetColumn(rewardControls.typeCbo, 1);
        rewardControls.valuesGrid.Children.Add(rewardControls.typeCbo);
        rewardControls.localKeyTxt = new TextBox();
        Grid.SetColumn(rewardControls.localKeyTxt, 1);
        Grid.SetRow(rewardControls.localKeyTxt, 1);
        rewardControls.valuesGrid.Children.Add(rewardControls.localKeyTxt);
        if (isVal)
        {
            Panel.SelectEnum(rewardData.RewardType, rewardControls.typeCbo);
            rewardControls.localKeyTxt.Text = rewardData.LocalText ?? string.Empty;
        }
        RewardsTabGrid.Children.Add(rewardControls.valuesGrid);
        this.rewardControls.Add(rewardControls);
    }

    public void OnRemoveRewardButtonClicked(object sender, RoutedEventArgs args)
    {
        if (sender is Button button)
        {
            for (int i = 0; i < rewardControls.Count; i++)
            {
                if (rewardControls[i].removeButton == button)
                {
                    RemoveReward(rewardControls[i]);
                    return;
                }
            }
        }
    }
    private void RemoveReward(RewardControls controls)
    {
        ConditionsTabGrid.Children.Remove(controls.leftHost);
        foreach (UIElement element in controls.extra)
            controls.valuesGrid.Children.Remove(element);
        ConditionsTabGrid.Children.Remove(controls.valuesGrid);
        ConditionsTabGrid.RowDefinitions.RemoveAt(0);
        for (int i = controls.index + 1; i < rewardControls.Count; i++)
        {
            RewardControls ctrls = rewardControls[i];
            ctrls.index -= 1;
            Grid.SetRow(ctrls.leftHost, ctrls.index);
            Grid.SetRow(ctrls.valuesGrid, ctrls.index);
            ctrls.number.Text = "#" + ctrls.index.ToString();
            rewardControls[i] = ctrls;
        }

        rewardControls.RemoveAt(controls.index);
        Grid.SetRow(AddRewardButton, this.rewardControls.Count);
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
            Panel.SetOrAddKey(prefix + "_ID", ID.ToString(AssetFile.Locale), pairs);
            Panel.SetOrAddKey(prefix + "_Amount", ID.ToString(AssetFile.Locale), pairs);
            Panel.SetOrAddFlag(prefix + "_Critical", Critical, pairs);
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
            Panel.SetOrAddKey(prefix + "_ID", ID.ToString(AssetFile.Locale), pairs);
            Panel.SetOrAddKey(prefix + "_Amount", ID.ToString(AssetFile.Locale), pairs);
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
            Panel.SetOrAddKey(prefix, ID.ToString(AssetFile.Locale), pairs);
            Panel.SetOrAddFlag(prefix + "_Critical", Critical, pairs);
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

        public abstract class NPCConditionData
        {
            public NPCConditionData()
            {

            }
            public abstract void AddChildren(Grid parent, ref int row, List<FrameworkElement> childrenList);
            public abstract void WriteData(string prefix, List<StringPair> pairs);
            public abstract void FindData(List<FrameworkElement> children);
            protected void AddTextBoxAndLabel(Grid parent, ref int row, List<FrameworkElement> childrenList, string labeltext, string name, string? @default = null)
            {
                TextBlock label = new TextBlock()
                {
                    Text = labeltext,
                    FontSize = 10,
                    Name = "lbl_" + name
                };
                TextBox tb = new TextBox()
                {
                    Name = name
                };
                if (@default != null)
                    tb.Text = @default;
                parent.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1d, GridUnitType.Star) });
                Grid.SetRow(label, row);
                Grid.SetRow(tb, row);
                Grid.SetColumn(tb, 1);
                parent.Children.Add(label);
                parent.Children.Add(tb);
                childrenList.Add(label);
                childrenList.Add(tb);
                row++;
            }
            protected void AddAssetReferenceGUID(Grid parent, ref int row, List<FrameworkElement> childrenList, string labeltext, string name, EAssetCategory category, EAssetType type = EAssetType.UNKNOWN, Guid? @default = null)
            {
                TextBlock label = new TextBlock()
                {
                    Text = labeltext,
                    FontSize = 10,
                    Name = "lbl_" + name
                };
                int index = -1;
                if (@default.HasValue)
                {
                    for (int i = 0; i < MainWindow.Instance!.files.Count; i++)
                    {
                        if (MainWindow.Instance!.files[i].guid == @default.Value)
                        {
                            index = i;
                            break;
                        }
                    }
                }
                AssetReference tb = new AssetReference()
                {
                    Name = name,
                    Category = category,
                    TypeFilter = type
                };
                tb.SelectItemIndex(index);
                parent.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(58d, GridUnitType.Pixel) });
                Grid.SetRow(label, row);
                Grid.SetRow(tb, row);
                Grid.SetColumn(tb, 1);
                parent.Children.Add(label);
                parent.Children.Add(tb);
                childrenList.Add(label);
                childrenList.Add(tb);
                row++;
            }
            protected void AddAssetReferenceID(Grid parent, ref int row, List<FrameworkElement> childrenList, string labeltext, string name, EAssetCategory category, EAssetType type = EAssetType.UNKNOWN, ushort? @default = null)
            {
                TextBlock label = new TextBlock()
                {
                    Text = labeltext,
                    FontSize = 10,
                    Name = "lbl_" + name
                };
                int index = -1;
                if (@default.HasValue)
                {
                    for (int i = 0; i < MainWindow.Instance!.files.Count; i++)
                    {
                        AssetFile file = MainWindow.Instance!.files[i];
                        if (file.category == category && file.ID == @default.Value)
                        {
                            index = i;
                            break;
                        }
                    }
                }
                AssetReference tb = new AssetReference()
                {
                    Name = name,
                    Category = category,
                    TypeFilter = type
                };
                tb.SelectItemIndex(index);
                parent.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(58d, GridUnitType.Pixel) });
                Grid.SetRow(label, row);
                Grid.SetRow(tb, row);
                Grid.SetColumn(tb, 1);
                parent.Children.Add(label);
                parent.Children.Add(tb);
                childrenList.Add(label);
                childrenList.Add(tb);
                row++;
            }
            protected void AddCheckBoxAndLabel(Grid parent, ref int row, List<FrameworkElement> childrenList, string labeltext, string name, bool @default = false)
            {
                TextBlock label = new TextBlock()
                {
                    Text = labeltext,
                    FontSize = 10,
                    Name = "lbl_" + name
                };
                CheckBox tb = new CheckBox()
                {
                    Name = name,
                    IsChecked = @default
                };
                parent.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1d, GridUnitType.Star) });
                Grid.SetRow(label, row);
                Grid.SetRow(tb, row);
                Grid.SetColumn(tb, 1);
                parent.Children.Add(label);
                parent.Children.Add(tb);
                childrenList.Add(label);
                childrenList.Add(tb);
                row++;
            }
            protected void AddEnumDropdown<TEnum>(Grid parent, ref int row, List<FrameworkElement> childrenList, string labeltext, string name, TEnum? excluded = null, TEnum? @default = null) where TEnum : struct
            {
                TextBlock label = new TextBlock()
                {
                    Text = labeltext,
                    FontSize = 10,
                    Name = "lbl_" + name
                };
                ComboBox tb = new ComboBox()
                {
                    Name = name,
                    IsEditable = false,
                };
                parent.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1d, GridUnitType.Star) });
                TEnum[] values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().OrderBy(x => (int)x.ToString()[0]).ToArray();
                int selected = -1;
                if (excluded.HasValue)
                {
                    if (@default.HasValue)
                    {
                        for (int i = 0; i < values.Length; i++)
                        {
                            if (!Equals(values[i], excluded.Value))
                            {
                                if (selected == -1 && Equals(@default.Value, values[i]))
                                    selected = i;
                                tb.Items.Add(new ComboBoxItem() { Content = Panel.ToProperCase(values[i].ToString()) });
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < values.Length; i++)
                            if (!Equals(values[i], excluded.Value))
                                tb.Items.Add(new ComboBoxItem() { Content = Panel.ToProperCase(values[i].ToString()) });
                    }
                }
                else if (@default.HasValue)
                {
                    for (int i = 0; i < values.Length; i++)
                    {
                        if (selected == -1 && Equals(@default.Value, values[i]))
                            selected = i;
                        tb.Items.Add(new ComboBoxItem() { Content = Panel.ToProperCase(values[i].ToString()) });
                    }
                }
                else
                {
                    for (int i = 0; i < values.Length; i++)
                        tb.Items.Add(new ComboBoxItem() { Content = Panel.ToProperCase(values[i].ToString()) });
                }
                Grid.SetRow(label, row);
                Grid.SetRow(tb, row);
                Grid.SetColumn(tb, 1);
                parent.Children.Add(label);
                parent.Children.Add(tb);
                childrenList.Add(label);
                childrenList.Add(tb);
                row++;
            }
            protected ushort FindUInt16(string name, List<FrameworkElement> children, ushort save)
            {
                for (int i = 0; i < children.Count; i++)
                {
                    FrameworkElement child = children[i];
                    if (child.Name.Equals(name, StringComparison.Ordinal))
                    {
                        if (child is TextBox box)
                        {
                            if (box.Text != null && ushort.TryParse(box.Text, System.Globalization.NumberStyles.Any, AssetFile.Locale, out ushort rtn))
                                return rtn;
                            else
                                box.Text = save.ToString(AssetFile.Locale);
                        }
                        return save;
                    }
                }
                return save;
            }
            protected Guid FindAsset(string name, List<FrameworkElement> children, Guid save)
            {
                for (int i = 0; i < children.Count; i++)
                {
                    FrameworkElement child = children[i];
                    if (child.Name.Equals(name, StringComparison.Ordinal))
                    {
                        if (child is AssetReference box)
                        {
                            int sfi = box.SelectedFileIndex;
                            if (sfi == -1) return save;
                            else
                                return MainWindow.Instance!.files[sfi].guid;
                        }
                        return save;
                    }
                }
                return save;
            }
            protected ushort FindAsset(string name, List<FrameworkElement> children, ushort save)
            {
                for (int i = 0; i < children.Count; i++)
                {
                    FrameworkElement child = children[i];
                    if (child.Name.Equals(name, StringComparison.Ordinal))
                    {
                        if (child is AssetReference box)
                        {
                            int sfi = box.SelectedFileIndex;
                            if (sfi == -1) return save;
                            else
                                return MainWindow.Instance!.files[sfi].ID;
                        }
                        return save;
                    }
                }
                return save;
            }
            protected AssetFile? FindAsset(string name, List<FrameworkElement> children)
            {
                for (int i = 0; i < children.Count; i++)
                {
                    FrameworkElement child = children[i];
                    if (child.Name.Equals(name, StringComparison.Ordinal))
                    {
                        if (child is AssetReference box)
                        {
                            int sfi = box.SelectedFileIndex;
                            if (sfi == -1) return default;
                            else
                            {
                                return MainWindow.Instance!.files[sfi];
                            }
                        }
                        return default;
                    }
                }
                return default;
            }
            public static uint FindUInt32(string name, List<FrameworkElement> children, uint save)
            {
                for (int i = 0; i < children.Count; i++)
                {
                    FrameworkElement child = children[i];
                    if (child.Name.Equals(name, StringComparison.Ordinal))
                    {
                        if (child is TextBox box)
                        {
                            if (box.Text != null && uint.TryParse(box.Text, System.Globalization.NumberStyles.Any, AssetFile.Locale, out uint rtn))
                                return rtn;
                            else
                                box.Text = save.ToString(AssetFile.Locale);
                        }
                        return save;
                    }
                }
                return save;
            }
            public static int FindInt32(string name, List<FrameworkElement> children, int save)
            {
                for (int i = 0; i < children.Count; i++)
                {
                    FrameworkElement child = children[i];
                    if (child.Name.Equals(name, StringComparison.Ordinal))
                    {
                        if (child is TextBox box)
                        {
                            if (box.Text != null && int.TryParse(box.Text, System.Globalization.NumberStyles.Any, AssetFile.Locale, out int rtn))
                                return rtn;
                            else
                                box.Text = save.ToString(AssetFile.Locale);
                        }
                        return save;
                    }
                }
                return save;
            }
            public static byte FindUInt8(string name, List<FrameworkElement> children, byte save)
            {
                for (int i = 0; i < children.Count; i++)
                {
                    FrameworkElement child = children[i];
                    if (child.Name.Equals(name, StringComparison.Ordinal))
                    {
                        if (child is TextBox box)
                        {
                            if (box.Text != null && byte.TryParse(box.Text, System.Globalization.NumberStyles.Any, AssetFile.Locale, out byte rtn))
                                return rtn;
                            else
                                box.Text = save.ToString(AssetFile.Locale);
                        }
                        return save;
                    }
                }
                return save;
            }
            public static short FindInt16(string name, List<FrameworkElement> children, short save)
            {
                for (int i = 0; i < children.Count; i++)
                {
                    FrameworkElement child = children[i];
                    if (child.Name.Equals(name, StringComparison.Ordinal))
                    {
                        if (child is TextBox box)
                        {
                            if (box.Text != null && short.TryParse(box.Text, System.Globalization.NumberStyles.Any, AssetFile.Locale, out short rtn))
                                return rtn;
                            else
                                box.Text = save.ToString(AssetFile.Locale);
                        }
                        return save;
                    }
                }
                return save;
            }
            public static float FindFloat(string name, List<FrameworkElement> children, float save)
            {
                for (int i = 0; i < children.Count; i++)
                {
                    FrameworkElement child = children[i];
                    if (child.Name.Equals(name, StringComparison.Ordinal))
                    {
                        if (child is TextBox box)
                        {
                            if (box.Text != null && float.TryParse(box.Text, System.Globalization.NumberStyles.Any, AssetFile.Locale, out float rtn))
                                return rtn;
                            else
                                box.Text = save.ToString(AssetFile.Locale);
                        }
                        return save;
                    }
                }
                return save;
            }
            public static bool FindBool(string name, List<FrameworkElement> children, bool save)
            {
                for (int i = 0; i < children.Count; i++)
                {
                    FrameworkElement child = children[i];
                    if (child.Name.Equals(name, StringComparison.Ordinal))
                    {
                        if (child is CheckBox box)
                        {
                            if (box.IsChecked.HasValue)
                                return box.IsChecked.Value;
                            else
                                box.IsChecked = save;
                        }
                        return save;
                    }
                }
                return save;
            }
            public static TEnum FindEnum<TEnum>(string name, List<FrameworkElement> children, TEnum save) where TEnum : struct
            {
                for (int i = 0; i < children.Count; i++)
                {
                    FrameworkElement child = children[i];
                    if (child.Name.Equals(name, StringComparison.Ordinal))
                    {
                        if (child is ComboBox box)
                        {
                            if (box.SelectedIndex == -1) return save;
                            if (box.SelectedItem is ComboBoxItem item && item.Content is string value && Enum.TryParse(value, true, out TEnum val))
                                return val;
                            else
                                Panel.SelectEnum(save, box);
                        }
                        return save;
                    }
                }
                return save;
            }
            public static string FindString(string name, List<FrameworkElement> children, string save)
            {
                for (int i = 0; i < children.Count; i++)
                {
                    FrameworkElement child = children[i];
                    if (child.Name.Equals(name, StringComparison.Ordinal))
                    {
                        if (child is TextBox box)
                        {
                            if (box.Text != null)
                                return box.Text;
                            else
                                box.Text = save;
                        }
                        return save;
                    }
                }
                return save;
            }
        }
        public class NPCExperienceConditionData : NPCConditionData
        {
            public uint Experience;
            public NPCExperienceConditionData(AssetFile file, string prefix)
            {
                Experience = file.GetUIntegerType(prefix + "_Value");
            }
            public NPCExperienceConditionData() {}
            public override void AddChildren(Grid parent, ref int row, List<FrameworkElement> childrenList)
            {
                AddTextBoxAndLabel(parent, ref row, childrenList, "Experience:", "experienceConditionExp", Experience.ToString(AssetFile.Locale));
            }
            public override void WriteData(string prefix, List<StringPair> pairs)
            {
                Panel.SetOrAddKey(prefix + "_Value", Experience, pairs);
            }
            public override void FindData(List<FrameworkElement> children)
            {
                Experience = FindUInt32("experienceConditionExp", children, Experience);
            }
        }
        public class NPCReputationConditionData : NPCConditionData
        {
            public uint Reputation;
            public NPCReputationConditionData(AssetFile file, string prefix)
            {
                Reputation = file.GetUIntegerType(prefix + "_Value");
            }
            public NPCReputationConditionData() { }
            public override void AddChildren(Grid parent, ref int row, List<FrameworkElement> childrenList)
            {
                AddTextBoxAndLabel(parent, ref row, childrenList, "Reputation:", "reputationConditionRep", Reputation.ToString(AssetFile.Locale));
            }
            public override void WriteData(string prefix, List<StringPair> pairs)
            {
                Panel.SetOrAddKey(prefix + "_Value", Reputation, pairs);
            }
            public override void FindData(List<FrameworkElement> children)
            {
                Reputation = FindUInt32("reputationConditionRep", children, Reputation);
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
            public NPCFlagConditionData() { }
            public override void AddChildren(Grid parent, ref int row, List<FrameworkElement> childrenList)
            {
                AddTextBoxAndLabel(parent, ref row, childrenList, "Flag ID:", "flagConditionID", FlagID.ToString(AssetFile.Locale));
                AddCheckBoxAndLabel(parent, ref row, childrenList, "Allow Unset:", "flagConditionAllowUnset", AllowUnset);
            }
            public override void WriteData(string prefix, List<StringPair> pairs)
            {
                Panel.SetOrAddKey(prefix + "_ID", FlagID, pairs);
                Panel.SetOrAddFlag(prefix + "_Allow_Unset", AllowUnset, pairs);
            }
            public override void FindData(List<FrameworkElement> children)
            {
                FlagID = FindUInt16("flagConditionID", children, FlagID);
                AllowUnset = FindBool("flagConditionAllowUnset", children, AllowUnset);
            }
        }
        public class NPCBoolFlagConditionData : NPCFlagConditionData
        {
            public bool Value;
            public NPCBoolFlagConditionData(AssetFile file, string prefix) : base (file, prefix)
            {
                Value = file.GetBooleanType(prefix + "_Value");
            }
            public NPCBoolFlagConditionData() { }
            public override void AddChildren(Grid parent, ref int row, List<FrameworkElement> childrenList)
            {
                base.AddChildren(parent, ref row, childrenList);
                AddCheckBoxAndLabel(parent, ref row, childrenList, "Value:", "flagBoolConditionValue", Value);
            }
            public override void WriteData(string prefix, List<StringPair> pairs)
            {
                base.WriteData(prefix, pairs);
                Panel.SetOrAddKey(prefix + "_Value", Value, pairs);
            }
            public override void FindData(List<FrameworkElement> children)
            {
                base.FindData(children);
                Value = FindBool("flagBoolConditionValue", children, Value);
            }
        }
        public class NPCShortFlagConditionData : NPCFlagConditionData
        {
            public short Value;
            public NPCShortFlagConditionData(AssetFile file, string prefix) : base (file, prefix)
            {
                Value = file.GetShortType(prefix + "_Value");
            }
            public NPCShortFlagConditionData() { }
            public override void AddChildren(Grid parent, ref int row, List<FrameworkElement> childrenList)
            {
                base.AddChildren(parent, ref row, childrenList);
                AddTextBoxAndLabel(parent, ref row, childrenList, "Value:", "flagShortConditionValue", Value.ToString(AssetFile.Locale));
            }
            public override void WriteData(string prefix, List<StringPair> pairs)
            {
                base.WriteData(prefix, pairs);
                Panel.SetOrAddKey(prefix + "_Value", Value, pairs);
            }
            public override void FindData(List<FrameworkElement> children)
            {
                base.FindData(children);
                Value = FindInt16("flagBoolConditionValue", children, Value);
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
            public NPCQuestConditionData() { }
            public override void AddChildren(Grid parent, ref int row, List<FrameworkElement> childrenList)
            {
                AddAssetReferenceID(parent, ref row, childrenList, "Quest ID:", "questConditionID", EAssetCategory.NPC, EAssetType.Quest, QuestID);
                AddEnumDropdown<ENPCQuestStatus>(parent, ref row, childrenList, "Quest Status:", "questConditionStatus", null, QuestStatus);
                AddCheckBoxAndLabel(parent, ref row, childrenList, "Ignore NPC:", "questConditionIgnoreNpc", IgnoreNPC);
            }
            public override void WriteData(string prefix, List<StringPair> pairs)
            {
                Panel.SetOrAddKey(prefix + "_ID", QuestID, pairs);
                Panel.SetOrAddEnumKey(prefix + "_Status", QuestStatus, pairs);
                Panel.SetOrAddFlag(prefix + "_Ignore_NPC", IgnoreNPC, pairs);
            }
            public override void FindData(List<FrameworkElement> children)
            {
                QuestID = FindAsset("questConditionID", children, QuestID);
                QuestStatus = FindEnum("questConditionStatus", children, QuestStatus);
                IgnoreNPC = FindBool("questConditionIgnoreNpc", children, IgnoreNPC);
            }
        }
        public class NPCSkillsetConditionData : NPCConditionData
        {
            public EPlayerSkillset Skillset;
            public NPCSkillsetConditionData(AssetFile file, string prefix)
            {
                Skillset = file.GetEnumType<EPlayerSkillset>(prefix + "_Value");
            }
            public NPCSkillsetConditionData() { }
            public override void AddChildren(Grid parent, ref int row, List<FrameworkElement> childrenList)
            {
                AddEnumDropdown<EPlayerSkillset>(parent, ref row, childrenList, "Skillset:", "skillsetConditionSkillset", null, Skillset);
            }
            public override void WriteData(string prefix, List<StringPair> pairs)
            {
                Panel.SetOrAddEnumKey(prefix + "_Value", Skillset, pairs);
            }
            public override void FindData(List<FrameworkElement> children)
            {
                Skillset = FindEnum("skillsetConditionSkillset", children, Skillset);
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
            public NPCItemConditionData() { }
            public override void AddChildren(Grid parent, ref int row, List<FrameworkElement> childrenList)
            {
                if (ItemID > 0 && ItemGUID == Guid.Empty)
                {
                    AssetFile file = MainWindow.Instance!.files.FirstOrDefault(x => x.ID == ItemID && x.category == EAssetCategory.ITEM);
                    if (file.guid != Guid.Empty)
                    {
                        ItemGUID = file.guid;
                    }
                }

                AddAssetReferenceGUID(parent, ref row, childrenList, "Item GUID:", "itemConditionGuid", EAssetCategory.ITEM, EAssetType.UNKNOWN, ItemGUID);
                AddTextBoxAndLabel(parent, ref row, childrenList, "Item Amount:", "itemConditionAmount", Amount.ToString(AssetFile.Locale));
            }
            public override void WriteData(string prefix, List<StringPair> pairs)
            {
                if (ItemGUID != Guid.Empty)
                    Panel.SetOrAddKey(prefix + "_ID", ItemGUID, pairs);
                else
                    Panel.SetOrAddKey(prefix + "_ID", ItemID, pairs);
                Panel.SetOrAddKey(prefix + "_Amount", Amount, pairs);
            }
            public override void FindData(List<FrameworkElement> children)
            {
                AssetFile? file = FindAsset("itemConditionGuid", children);
                if (file is not null)
                {
                    ItemID = file.ID;
                    ItemGUID = file.guid;
                }
                Amount = FindUInt16("itemConditionAmount", children, Amount);
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
            public NPCZombieKillsConditionData() { }
            public override void AddChildren(Grid parent, ref int row, List<FrameworkElement> childrenList)
            {
                AddTextBoxAndLabel(parent, ref row, childrenList, "Zombie ID:", "zombieKillsConditionID", ID.ToString(AssetFile.Locale));
                AddTextBoxAndLabel(parent, ref row, childrenList, "Kill Count:", "zombieKillsConditionValue", Value.ToString(AssetFile.Locale));
                AddCheckBoxAndLabel(parent, ref row, childrenList, "Should Spawn", "zombieKillsConditionSpawn", Spawn);
                AddTextBoxAndLabel(parent, ref row, childrenList, "Spawn Quantity:", "zombieKillsConditionSpawnQuantity", SpawnQuantity.ToString(AssetFile.Locale));
                AddEnumDropdown<EZombieSpeciality>(parent, ref row, childrenList, "Zombie Type:", "zombieKillsConditionZombieType", null, ZombieType);
                AddTextBoxAndLabel(parent, ref row, childrenList, "Nav ID:", "zombieKillsConditionNavID", NavID.ToString(AssetFile.Locale));
                AddTextBoxAndLabel(parent, ref row, childrenList, "Max Radius:", "zombieKillsConditionMaxRadius", MaxRadius.ToString(AssetFile.Locale));
                AddTextBoxAndLabel(parent, ref row, childrenList, "Min Radius:", "zombieKillsConditionMinRadius", MinRadius.ToString(AssetFile.Locale));
            }
            public override void WriteData(string prefix, List<StringPair> pairs)
            {
                Panel.SetOrAddKey(prefix + "_ID", ID, pairs);
                Panel.SetOrAddKey(prefix + "_Value", Value, pairs);
                Panel.SetOrAddEnumKey(prefix + "_Zombie", ZombieType, pairs);
                Panel.SetOrAddKey(prefix + "_Spawn_Quantity", SpawnQuantity, pairs);
                Panel.SetOrAddKey(prefix + "_Nav", NavID, pairs);
                Panel.SetOrAddKey(prefix + "_Radius", MaxRadius, pairs);
                Panel.SetOrAddKey(prefix + "_MinRadius", MinRadius, pairs);
                Panel.SetOrAddFlag(prefix + "_Spawn", Spawn, pairs);
            }
            public override void FindData(List<FrameworkElement> children)
            {
                ID = FindUInt16("zombieKillsConditionID", children, ID);
                Value = FindInt16("zombieKillsConditionValue", children, Value);
                ZombieType = FindEnum("zombieKillsConditionZombieType", children, ZombieType);
                Spawn = FindBool("zombieKillsConditionSpawn", children, Spawn);
                SpawnQuantity = FindInt32("zombieKillsConditionSpawnQuantity", children, SpawnQuantity);
                NavID = FindUInt8("zombieKillsConditionNavID", children, NavID);
                MaxRadius = FindFloat("zombieKillsConditionMaxRadius", children, MaxRadius);
                MinRadius = FindFloat("zombieKillsConditionMinRadius", children, MinRadius);
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
            public NPCHordeKillsConditionData() { }
            public override void AddChildren(Grid parent, ref int row, List<FrameworkElement> childrenList)
            {
                AddTextBoxAndLabel(parent, ref row, childrenList, "Horde ID:", "hordeKillsConditionID", ID.ToString(AssetFile.Locale));
                AddTextBoxAndLabel(parent, ref row, childrenList, "Kill Count:", "hordeKillsConditionValue", Value.ToString(AssetFile.Locale));
                AddTextBoxAndLabel(parent, ref row, childrenList, "Nav ID:", "hordeKillsConditionNavID", NavID.ToString(AssetFile.Locale));
            }
            public override void WriteData(string prefix, List<StringPair> pairs)
            {
                Panel.SetOrAddKey(prefix + "_ID", ID, pairs);
                Panel.SetOrAddKey(prefix + "_Value", Value, pairs);
                Panel.SetOrAddKey(prefix + "_Nav", NavID, pairs);
            }
            public override void FindData(List<FrameworkElement> children)
            {
                ID = FindUInt16("hordeKillsConditionID", children, ID);
                Value = FindInt16("hordeKillsConditionValue", children, Value);
                NavID = FindUInt8("hordeKillsConditionNavID", children, NavID);
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
            public NPCAnimalKillsConditionData() { }
            public override void AddChildren(Grid parent, ref int row, List<FrameworkElement> childrenList)
            {
                AddTextBoxAndLabel(parent, ref row, childrenList, "Condition ID:", "animalKillsConditionID", ID.ToString(AssetFile.Locale));
                AddTextBoxAndLabel(parent, ref row, childrenList, "Kill Count:", "animalKillsConditionValue", Value.ToString(AssetFile.Locale));
                AddAssetReferenceID(parent, ref row, childrenList, "Animal ID:", "animalKillsConditionAnimalID", EAssetCategory.ANIMAL, EAssetType.Animal, Animal);
            }
            public override void WriteData(string prefix, List<StringPair> pairs)
            {
                Panel.SetOrAddKey(prefix + "_ID", ID, pairs);
                Panel.SetOrAddKey(prefix + "_Value", Value, pairs);
                Panel.SetOrAddKey(prefix + "_Animal", Animal, pairs);
            }
            public override void FindData(List<FrameworkElement> children)
            {
                ID = FindUInt16("animalKillsConditionID", children, ID);
                Value = FindInt16("animalKillsConditionValue", children, Value);
                Animal = FindAsset("animalKillsConditionAnimalID", children, Animal);
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
            public NPCCompareFlagsConditionData() { }
            public override void AddChildren(Grid parent, ref int row, List<FrameworkElement> childrenList)
            {
                AddTextBoxAndLabel(parent, ref row, childrenList, "Flag 1:", "compareFlagsA", A.ToString(AssetFile.Locale));
                AddCheckBoxAndLabel(parent, ref row, childrenList, "  Allow Unset:", "compareFlagsAAllowUnset", AllowAUnset);
                AddTextBoxAndLabel(parent, ref row, childrenList, "Flag 2:", "compareFlagsB", B.ToString(AssetFile.Locale));
                AddCheckBoxAndLabel(parent, ref row, childrenList, "  Allow Unset:", "compareFlagsBAllowUnset", AllowBUnset);
            }
            public override void WriteData(string prefix, List<StringPair> pairs)
            {
                Panel.SetOrAddKey(prefix + "_A_ID", A, pairs);
                Panel.SetOrAddFlag(prefix + "_Allow_A_Unset", AllowAUnset, pairs);
                Panel.SetOrAddKey(prefix + "_B_ID", B, pairs);
                Panel.SetOrAddFlag(prefix + "_Allow_B_Unset", AllowBUnset, pairs);
            }
            public override void FindData(List<FrameworkElement> children)
            {
                A = FindUInt16("compareFlagsA", children, A);
                B = FindUInt16("compareFlagsB", children, B);
                AllowAUnset = FindBool("compareFlagsAAllowUnset", children, AllowAUnset);
                AllowBUnset = FindBool("compareFlagsBAllowUnset", children, AllowBUnset);
            }
        }
        public class NPCTimeOfDayConditionData : NPCConditionData
        {
            public int TimeOfDaySeconds;
            public NPCTimeOfDayConditionData(AssetFile file, string prefix)
            {
                TimeOfDaySeconds = file.GetIntegerType(prefix + "_Second");
            }
            public NPCTimeOfDayConditionData() { }
            public override void AddChildren(Grid parent, ref int row, List<FrameworkElement> childrenList)
            {
                AddTextBoxAndLabel(parent, ref row, childrenList, "TOD Seconds:", "timeConditionTimeOfDay", TimeOfDaySeconds.ToString(AssetFile.Locale));
            }
            public override void WriteData(string prefix, List<StringPair> pairs)
            {
                Panel.SetOrAddKey(prefix + "_Second", TimeOfDaySeconds, pairs);
            }
            public override void FindData(List<FrameworkElement> children)
            {
                TimeOfDaySeconds = FindInt32("timeConditionTimeOfDay", children, TimeOfDaySeconds);
            }
        }
        public class NPCPlayerLifeHealthConditionData : NPCConditionData
        {
            public int Value;
            public NPCPlayerLifeHealthConditionData(AssetFile file, string prefix)
            {
                Value = file.GetIntegerType(prefix + "_Value");
            }
            public NPCPlayerLifeHealthConditionData() { }
            public override void AddChildren(Grid parent, ref int row, List<FrameworkElement> childrenList)
            {
                AddTextBoxAndLabel(parent, ref row, childrenList, "Health Value:", "healthConditionValue", Value.ToString(AssetFile.Locale));
            }
            public override void WriteData(string prefix, List<StringPair> pairs)
            {
                Panel.SetOrAddKey(prefix + "_Value", Value, pairs);
            }
            public override void FindData(List<FrameworkElement> children)
            {
                Value = FindInt32("healthConditionValue", children, Value);
            }
        }
        public class NPCPlayerLifeFoodConditionData : NPCConditionData
        {
            public int Value;
            public NPCPlayerLifeFoodConditionData(AssetFile file, string prefix)
            {
                Value = file.GetIntegerType(prefix + "_Value");
            }
            public NPCPlayerLifeFoodConditionData() { }
            public override void AddChildren(Grid parent, ref int row, List<FrameworkElement> childrenList)
            {
                AddTextBoxAndLabel(parent, ref row, childrenList, "Food Value:", "foodConditionValue", Value.ToString(AssetFile.Locale));
            }
            public override void WriteData(string prefix, List<StringPair> pairs)
            {
                Panel.SetOrAddKey(prefix + "_Value", Value, pairs);
            }
            public override void FindData(List<FrameworkElement> children)
            {
                Value = FindInt32("foodConditionValue", children, Value);
            }
        }
        public class NPCPlayerLifeWaterConditionData : NPCConditionData
        {
            public int Value;
            public NPCPlayerLifeWaterConditionData(AssetFile file, string prefix)
            {
                Value = file.GetIntegerType(prefix + "_Value");
            }
            public NPCPlayerLifeWaterConditionData() { }
            public override void AddChildren(Grid parent, ref int row, List<FrameworkElement> childrenList)
            {
                AddTextBoxAndLabel(parent, ref row, childrenList, "Water Value:", "waterConditionValue", Value.ToString(AssetFile.Locale));
            }
            public override void WriteData(string prefix, List<StringPair> pairs)
            {
                Panel.SetOrAddKey(prefix + "_Value", Value, pairs);
            }
            public override void FindData(List<FrameworkElement> children)
            {
                Value = FindInt32("waterConditionValue", children, Value);
            }
        }
        public class NPCPlayerLifeVirusConditionData : NPCConditionData
        {
            public int Value;
            public NPCPlayerLifeVirusConditionData(AssetFile file, string prefix)
            {
                Value = file.GetIntegerType(prefix + "_Value");
            }
            public NPCPlayerLifeVirusConditionData() { }
            public override void AddChildren(Grid parent, ref int row, List<FrameworkElement> childrenList)
            {
                AddTextBoxAndLabel(parent, ref row, childrenList, "Virus Value:", "virusConditionValue", Value.ToString(AssetFile.Locale));
            }
            public override void WriteData(string prefix, List<StringPair> pairs)
            {
                Panel.SetOrAddKey(prefix + "_Value", Value, pairs);
            }
            public override void FindData(List<FrameworkElement> children)
            {
                Value = FindInt32("virusConditionValue", children, Value);
            }
        }
        public class NPCHolidayConditionData : NPCConditionData
        {
            public ENPCHoliday Holiday;
            public NPCHolidayConditionData(AssetFile file, string prefix)
            {
                Holiday = file.GetEnumType(prefix + "_Value", ENPCHoliday.NONE);
            }
            public NPCHolidayConditionData() { }
            public override void AddChildren(Grid parent, ref int row, List<FrameworkElement> childrenList)
            {
                AddEnumDropdown<ENPCHoliday>(parent, ref row, childrenList, "Holiday:", "holidayConditionHoliday", null, Holiday);
            }
            public override void WriteData(string prefix, List<StringPair> pairs)
            {
                Panel.SetOrAddEnumKey(prefix + "_Value", Holiday, pairs);
            }
            public override void FindData(List<FrameworkElement> children)
            {
                Holiday = FindEnum("holidayConditionHoliday", children, Holiday);
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
            public NPCPlayerKillsConditionData() { }
            public override void AddChildren(Grid parent, ref int row, List<FrameworkElement> childrenList)
            {
                AddTextBoxAndLabel(parent, ref row, childrenList, "ID:", "playerKillsConditionID", ID.ToString(AssetFile.Locale));
                AddTextBoxAndLabel(parent, ref row, childrenList, "Threshold:", "playerKillsConditionValue", Threshold.ToString(AssetFile.Locale));
            }
            public override void WriteData(string prefix, List<StringPair> pairs)
            {
                Panel.SetOrAddKey(prefix + "_ID", ID, pairs);
                Panel.SetOrAddKey(prefix + "_Value", Threshold, pairs);
            }
            public override void FindData(List<FrameworkElement> children)
            {
                ID = FindUInt16("playerKillsConditionID", children, ID);
                Threshold = FindInt16("playerKillsConditionValue", children, Threshold);
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
            public NPCObjectKillsConditionData() { }
            public override void AddChildren(Grid parent, ref int row, List<FrameworkElement> childrenList)
            {
                AddTextBoxAndLabel(parent, ref row, childrenList, "ID:", "objectKillsConditionID", ID.ToString(AssetFile.Locale));
                AddTextBoxAndLabel(parent, ref row, childrenList, "Threshold:", "objectKillsConditionValue", Threshold.ToString(AssetFile.Locale));
                AddAssetReferenceGUID(parent, ref row, childrenList, "Object GUID:", "objectKillsConditionGuid", EAssetCategory.OBJECT, EAssetType.UNKNOWN, ObjectGUID);
                AddTextBoxAndLabel(parent, ref row, childrenList, "Nav:", "objectKillsConditionNav", Nav.ToString(AssetFile.Locale));
            }
            public override void WriteData(string prefix, List<StringPair> pairs)
            {
                Panel.SetOrAddKey(prefix + "_ID", ID, pairs);
                Panel.SetOrAddKey(prefix + "_Value", Threshold, pairs);
                Panel.SetOrAddKey(prefix + "_Value", ObjectGUID, pairs);
                Panel.SetOrAddKey(prefix + "_Nav", Nav, pairs);
            }
            public override void FindData(List<FrameworkElement> children)
            {
                ID = FindUInt16("objectKillsConditionID", children, ID);
                Threshold = FindInt16("objectKillsConditionValue", children, Threshold);
                ObjectGUID = FindAsset("objectKillsConditionGuid", children, ObjectGUID);
                Nav = FindUInt8("objectKillsConditionValue", children, Nav);
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
            public NPCCurrencyConditionData() { }
            public override void AddChildren(Grid parent, ref int row, List<FrameworkElement> childrenList)
            {
                AddAssetReferenceGUID(parent, ref row, childrenList, "Currency GUID:", "currencyConditionGuid", EAssetCategory.ITEM, EAssetType.Supply, CurrencyItemGUID);
                AddTextBoxAndLabel(parent, ref row, childrenList, "Threshold:", "currencyConditionValue", Threshold.ToString(AssetFile.Locale));
            }
            public override void WriteData(string prefix, List<StringPair> pairs)
            {
                Panel.SetOrAddKey(prefix + "_GUID", CurrencyItemGUID, pairs);
                Panel.SetOrAddKey(prefix + "_Value", Threshold, pairs);
            }
            public override void FindData(List<FrameworkElement> children)
            {
                CurrencyItemGUID = FindAsset("currencyConditionGuid", children, CurrencyItemGUID);
                Threshold = FindUInt32("currencyConditionValue", children, Threshold);
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
            public NPCTreeKillsConditionData() { }
            public override void AddChildren(Grid parent, ref int row, List<FrameworkElement> childrenList)
            {
                AddTextBoxAndLabel(parent, ref row, childrenList, "ID:", "treeKillsConditionID", ID.ToString(AssetFile.Locale));
                AddTextBoxAndLabel(parent, ref row, childrenList, "Tree GUID:", "treeKillsConditionGuid", TreeGUID.ToString("N"));
                AddTextBoxAndLabel(parent, ref row, childrenList, "Threshold:", "treeKillsConditionValue", Threshold.ToString(AssetFile.Locale));
            }
            public override void WriteData(string prefix, List<StringPair> pairs)
            {
                Panel.SetOrAddKey(prefix + "_ID", ID, pairs);
                Panel.SetOrAddKey(prefix + "_Value", Threshold, pairs);
                Panel.SetOrAddKey(prefix + "_Tree", TreeGUID, pairs);
            }
            public override void FindData(List<FrameworkElement> children)
            {
                if (Guid.TryParse(FindString("treeKillsConditionGuid", children, TreeGUID.ToString("N")), out Guid n))
                    TreeGUID = n;
                ID = FindUInt16("treeKillsConditionID", children, ID);
                Threshold = FindInt16("treeKillsConditionValue", children, Threshold);
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
            public NPCWeatherStatusConditionData() { }
            public override void AddChildren(Grid parent, ref int row, List<FrameworkElement> childrenList)
            {
                AddTextBoxAndLabel(parent, ref row, childrenList, "Weather GUID:", "weatherStatusConditionGuid", WeatherGUID.ToString("N"));
                AddEnumDropdown<ENPCWeatherStatus>(parent, ref row, childrenList, "Weather GUID:", "weatherStatusConditionType", null, WeatherType);
            }
            public override void WriteData(string prefix, List<StringPair> pairs)
            {
                Panel.SetOrAddKey(prefix + "_GUID", WeatherGUID, pairs);
                Panel.SetOrAddEnumKey(prefix + "_Value", WeatherType, pairs);
            }
            public override void FindData(List<FrameworkElement> children)
            {
                if (Guid.TryParse(FindString("weatherStatusConditionGuid", children, WeatherGUID.ToString("N")), out Guid n))
                    WeatherGUID = n;
                WeatherType = FindEnum("weatherStatusConditionType", children, WeatherType);
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
            public NPCWeatherBlendAlphaConditionData() { }
            public override void AddChildren(Grid parent, ref int row, List<FrameworkElement> childrenList)
            {
                AddTextBoxAndLabel(parent, ref row, childrenList, "Weather GUID:", "weatherBlendConditionGuid", WeatherGUID.ToString("N"));
                AddTextBoxAndLabel(parent, ref row, childrenList, "Lerp Alpha:", "weatherBlendConditionLerp", Threshold.ToString(AssetFile.Locale));
            }
            public override void WriteData(string prefix, List<StringPair> pairs)
            {
                Panel.SetOrAddKey(prefix + "_GUID", WeatherGUID, pairs);
                Panel.SetOrAddKey(prefix + "_Value", Threshold, pairs);
            }
            public override void FindData(List<FrameworkElement> children)
            {
                if (Guid.TryParse(FindString("weatherBlendConditionGuid", children, WeatherGUID.ToString("N")), out Guid n))
                    WeatherGUID = n;
                Threshold = FindFloat("weatherBlendConditionLerp", children, Threshold);
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