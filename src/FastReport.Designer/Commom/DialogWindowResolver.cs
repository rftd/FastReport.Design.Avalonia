using ActiproSoftware.UI.Avalonia.Controls;
using Avalonia.Controls;
using Avalonia.Data;
using Caramelo.MvvmApp.Avalonia.Commom;
using FastReport.Designer.Controls;

namespace FastReport.Designer.Commom;

public sealed class DialogWindowResolver : IDialogWindowResolver
{
    public Window CreateWindow(Control control)
    {
        var caption = new DefaultCaption();
        var titleBar = new ChromedTitleBar
        {
            LeftContent = caption,
            IsMaximizeButtonAllowed = false,
            IsMinimizeButtonAllowed = false
        };
        DockPanel.SetDock(titleBar, Dock.Top);

        var window = new Window
        {
            SystemDecorations = SystemDecorations.None,
            Content = new DockPanel
            {
                LastChildFill = true,
                Children = { titleBar, control }
            },
            SizeToContent = SizeToContent.WidthAndHeight,
            CanResize = false
        };

        caption.Bind(DefaultCaption.TitleProperty, new Binding("Title") { Source = window });
        return window;
    }
}