﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using AvaloniaEdit.Utils;
using Caramelo.MvvmApp.Avalonia.Controls;
using FastReport.Designer.Commom;
using FastReport.Designer.ViewModels;
using FastReport.Utils;
using FastReport.Wizards;
using Material.Icons;
using Material.Icons.Avalonia;
using ReactiveMarbles.ObservableEvents;

namespace FastReport.Designer.Views;

public partial class WelcomDialogView : MvvmWindow<WelcomeDialogViewModel>
{
    public WelcomDialogView()
    {
        InitializeComponent();

        this.Events().Loaded.Subscribe(_ =>
        { foreach (var recentFile in ViewModel.RecentFiles)
            {
                RecentFiles.Children.Add(new Button
                {
                    BorderThickness = Thickness.Parse("0"),
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    Command = ViewModel.RecentReportCommand,
                    CommandParameter = recentFile,
                    Content = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Children =
                        {
                            new MaterialIcon {Kind = MaterialIconKind.FileDocumentBox},
                            new TextBlock {Text = Path.GetFileName(recentFile)}
                        }
                    }
                });
            }
        });
    }
}