using System;
using Avalonia.Controls;

namespace FastReport.Designer.Extensions;

internal static class MenuItemExtensions
{
    public static void AddItem(this MenuItem menu, Action<MenuItem> configure)
    {
        var subMneu = new MenuItem();
        menu.Items.Add(subMneu);
        configure.Invoke(subMneu);
    }
    
    public static void AddSeparator(this MenuItem menu) => menu.Items.Add(new Separator());
}