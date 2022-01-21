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
using System.Windows.Shapes;

namespace UAM;
/// <summary>
/// Interaction logic for BlueprintEditor.xaml
/// </summary>
public partial class BlueprintEditorWindow : Window
{
    
    public BlueprintEditorWindow()
    {
        InitializeComponent();
    }
    public void Load(AssetFile file, BlueprintEditor parent)
    {
        StackPanel.Children.Clear();
        int i = 0;
        foreach (BlueprintData data in parent.Blueprints)
        {
            Blueprint bp = new Blueprint();
            bp.LoadBlueprint(data, i++);
            StackPanel.Children.Add(bp);
        }
        foreach (ActionData action in parent.Actions)
        {
            StackPanel.Children.Add(new TextBlock() { Text = action.ToString() });
            foreach (ActionData.ActionBlueprint blueprint in action.Blueprints)
            {
                StackPanel.Children.Add(new TextBlock() { Text = blueprint.ToString() });
            }
        }
    }
}
