using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using FastReport.Utils;
using ReactiveUI;

namespace FastReport.Designer.Extensions;

internal static class DesignerControlExtensions
{
    public static void InsertObject(this Design.Designer designer, ObjectInfo info, InsertFrom source)
    {
        var method = typeof(Design.Designer).GetMethod("InsertObject", 
            BindingFlags.Instance  | BindingFlags.NonPublic, [typeof(ObjectInfo), typeof(InsertFrom)]);
        method?.Invoke(designer, [info, source]);
    }
    
    public static void LayoutToolbars(this Design.Designer designer)
    {
        var method = typeof(Design.Designer).GetMethod("LayoutToolbars", BindingFlags.Instance  | BindingFlags.NonPublic);
        method?.Invoke(designer, null);
    }
    
    public static IEnumerable<string> RecentFiles(this Design.Designer designer)
    {
        var property = typeof(Design.Designer).GetProperty("RecentFiles", BindingFlags.Instance  | BindingFlags.NonPublic);
        var files = (List<string>?)property?.GetValue(designer);
        if(files == null) throw new InvalidOperationException();
        
        return files;
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
        ((ToolStripMenuItem?)item)?.ToggleChecked();
    }
    
    public static void ToggleChecked(this ToolStripMenuItem item)
    {
        item.Checked = !item.Checked;
        item.PerformClick();
    }
}