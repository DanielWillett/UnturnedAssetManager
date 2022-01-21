using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UAM;

public abstract class Editor<T> where T : Window, new()
{
    protected readonly T _child;
    public T Window => _child;

    public Editor()
    {
        _child = new T();
        _child.Closing += OnWindowClosed;
    }

    private void OnWindowClosed(object sender, System.ComponentModel.CancelEventArgs e)
    {
        e.Cancel = true;
        _child.Hide();
    }
    public void Show() => _child.Show();
    public void Hide() => _child.Hide();
    public abstract void LoadWindow(AssetFile file, Panel panel);

}
public class BlueprintEditor : Editor<BlueprintEditorWindow>
{
    public List<BlueprintData> Blueprints;
    public List<ActionData> Actions;
    public BlueprintEditor() : base()
    {

    }
    public override void LoadWindow(AssetFile file, Panel panel)
    {

        int totalBlueprints = file.GetIntegerTypeClampTo0("Blueprints");
        int totalActions = file.GetIntegerTypeClampTo0("Actions");

        Blueprints = new List<BlueprintData>(totalBlueprints);
        Actions = new List<ActionData>(totalActions);

        for (int i = 0; i < totalBlueprints; i++)
            Blueprints.Add(new BlueprintData(file, i));

        for (int i = 0; i < totalActions; i++)
            Actions.Add(new ActionData(file, i, file.ID));

        _child.Load(file, this);

        Debug.WriteLine($"{Blueprints.Count} blueprints and {Actions.Count} actions.");
    }
}