using System;
using System.Reflection;
using System.Windows.Forms;
using FastReport.Design;
using FastReport.Designer.Commom;
using ReactiveUI;

namespace FastReport.Designer.Extensions;

public static class DesignerControlExtensions
{
    public static FrDesignerControlHelper GetHelper(this AvaloniaDesignerControl control)
    {
        return new FrDesignerControlHelper(control.InnerDesigner);
    }

    public static IObservable<bool> ToCheckedObservable(this ToolStripItem item)
    {
        var itemMenu = (ToolStripMenuItem) item;
        var field = typeof(ToolStripMenuItem).GetField("menuItem", BindingFlags.Instance | BindingFlags.NonPublic);
        if (field == null) throw new Exception();
        
        var internalItem = (CustomControls.ToolStripMenuItem?)field.GetValue(itemMenu);
        return internalItem.WhenAnyValue(x => x.IsChecked);
    }
    
    public static void SetChecked(this ToolStripItem item, bool check)
    {
        var itemMenu = (ToolStripMenuItem) item;
        itemMenu.Checked = check;
    }
    
    public static bool GetChecked(this ToolStripItem item)
    {
        var itemMenu = (ToolStripMenuItem) item;
        return itemMenu.Checked;
    }
    
    public static void ToggleChecked(this ToolStripItem item)
    {
        var itemMenu = (ToolStripMenuItem) item;
        itemMenu.Checked = !itemMenu.Checked;
        itemMenu.PerformClick();
    }
}