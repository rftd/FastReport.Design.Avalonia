using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Forms;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using FastReport.Design;
using FastReport.Design.StandardDesigner;
using FastReport.Design.Toolbars;
using FastReport.Designer.Extensions;
using FastReport.Designer.ViewModels;
using FastReport.Utils;
using Material.Icons;
using Material.Icons.Avalonia;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;
using CheckBox = Avalonia.Controls.CheckBox;

namespace FastReport.Designer.Commom;

public sealed class FrDesignerMenuHelper
{
    #region Fields
    
    private readonly Dictionary<int, MaterialIconKind> insertIcons = [];

    #endregion Fields
    
    #region Constructors

    public FrDesignerMenuHelper(FrMenuLocalization localization)
    {
        Localization = localization;
        Initialize();
    }

    #endregion Constructors

    #region Properties

    public FrMenuLocalization Localization { get; }

    public AvaloniaDesignerControl AvaloniaDesigner { get; set; } = null!;

    public DesignerControl Designer => AvaloniaDesigner.InnerDesigner;

    #endregion Properties

    #region Methods

    private void Initialize()
    {
        insertIcons.Add(102, MaterialIconKind.CapsLock);
        insertIcons.Add(103, MaterialIconKind.Image);
        insertIcons.Add(104, MaterialIconKind.TextBoxMultiple);
        insertIcons.Add(144, MaterialIconKind.TextBoxOutline);
        insertIcons.Add(127, MaterialIconKind.Table);
        insertIcons.Add(124, MaterialIconKind.CheckboxMarked);
        insertIcons.Add(129, MaterialIconKind.Counter);
        insertIcons.Add(121, MaterialIconKind.FormatTextbox);
        insertIcons.Add(246, MaterialIconKind.LanguageHtml5);
        insertIcons.Add(265, MaterialIconKind.IdCard);
        insertIcons.Add(153, MaterialIconKind.Map);
        insertIcons.Add(251, MaterialIconKind.Signature);
        insertIcons.Add(125, MaterialIconKind.ChartBar);
        insertIcons.Add(130, MaterialIconKind.ChartTimelineVariant);
        insertIcons.Add(249, MaterialIconKind.Svg);
        insertIcons.Add(126, MaterialIconKind.Text);
        
        insertIcons.Add(105, MaterialIconKind.Minus);
        insertIcons.Add(107, MaterialIconKind.VectorLine);
        insertIcons.Add(150, MaterialIconKind.VectorLine);
        insertIcons.Add(151, MaterialIconKind.VectorLine);
        insertIcons.Add(152, MaterialIconKind.VectorLine);
        insertIcons.Add(108, MaterialIconKind.SquareOutline);
        insertIcons.Add(109, MaterialIconKind.SquareRoundedOutline);
        insertIcons.Add(110, MaterialIconKind.EllipseOutline);
        insertIcons.Add(111, MaterialIconKind.TriangleOutline);
        insertIcons.Add(131, MaterialIconKind.RhombusOutline);
        insertIcons.Add(240, MaterialIconKind.VectorPolyline);
        insertIcons.Add(241, MaterialIconKind.ShapePolygonAdd);
        insertIcons.Add(242, MaterialIconKind.PentagonOutline);
        insertIcons.Add(243, MaterialIconKind.HexagonOutline);
        insertIcons.Add(244, MaterialIconKind.OctagonOutline);
        insertIcons.Add(245, MaterialIconKind.OctagonOutline);
        
        insertIcons.Add(142, MaterialIconKind.Matrix);
        
        insertIcons.Add(149, MaterialIconKind.Qrcode);
        insertIcons.Add(123, MaterialIconKind.Barcode);
        
        insertIcons.Add(140, MaterialIconKind.Gauge);
    }

    public void GenerateMenu(Menu menu, AppBootstrapperViewModel model)
    {
        menu.Items.Clear();
        menu.Items.Add(GenerateFileMenu(model.FinishAppCommand));
        menu.Items.Add(GenerateEditMenu());
        menu.Items.Add(GenerateViewMenu(model.PluginManagerCommand));
        menu.Items.Add(GenerateInsertMenu());
        menu.Items.Add(GenerateReportMenu());
        menu.Items.Add(GenerateDataMenu());
        menu.Items.Add(GenerateHelpMenu());
    }

    private MenuItem GenerateFileMenu(ICommand finishAppCmd)
    {
        var menu = new MenuItem { Header = Localization.File };
        Localization.BindLocalization(menu, x => x.File);
        menu.AddItem(subMenu =>
        {
            subMenu.Header = Localization.FileNew;
            subMenu.Command = AvaloniaDesigner.cmdNew;
            subMenu.Icon = new MaterialIcon
            {
                Kind = MaterialIconKind.FileDocument
            };
            
            Localization.BindLocalization(subMenu, x => x.FileNew);
        });
        
        menu.AddItem(subMenu =>
        {
            subMenu.Header = Localization.FileOpen;
            subMenu.HotKey = KeyGesture.Parse("Ctrl+O");
            subMenu.Command = AvaloniaDesigner.cmdOpen;
            subMenu.Icon = new MaterialIcon
            {
                Kind = MaterialIconKind.FolderOpen
            };
                
            Localization.BindLocalization(subMenu, x => x.FileOpen);
        });
        
        menu.AddItem(subMenu =>
        {
            subMenu.Header = Localization.FileOpenPage;
            subMenu.Command = AvaloniaDesigner.cmdOpenPage;
            subMenu.Icon = new MaterialIcon
            {
                Kind = MaterialIconKind.FileDownload
            };
            Localization.BindLocalization(subMenu, x => x.FileOpenPage);
        });
        
        menu.AddItem(subMenu =>
        {
            subMenu.Header = Localization.FileOpenCloud;
            subMenu.Command = AvaloniaDesigner.cmdOpenViaCloud;
            subMenu.Icon = new MaterialIcon
            {
                Kind = MaterialIconKind.CloudDownload
            };
            Localization.BindLocalization(subMenu, x => x.FileOpenCloud);
        });

        menu.AddSeparator();

        menu.AddItem(subMenu =>
        {
            subMenu.Header = Localization.FileRecentFiles;
            Localization.BindLocalization(subMenu, x => x.FileRecentFiles);
            
            menu.Events().SubmenuOpened.Subscribe(_ =>
            {
                subMenu.Items.Clear();
            
                var recentFiles = Designer.RecentFiles();
                foreach (var recentFile in recentFiles)
                {
                    var item =new MenuItem
                    {
                        Header = recentFile,
                        Command = ReactiveCommand.Create(() =>
                        {
                            Designer.cmdRecentFiles.LoadFile(recentFile);
                        })
                    };

                    if (Designer.cmdRecentFiles.IsCloudFile(recentFile))
                    {
                        item.Icon = new MaterialIcon
                        {
                            Kind = MaterialIconKind.Cloud
                        };
                    }

                    subMenu.Items.Add(item);
                }

                subMenu.IsEnabled = subMenu.Items.Count > 0;
            });
        });
        
        menu.AddSeparator();
        
        menu.AddItem(subMenu =>
        {
            subMenu.Header = Localization.FileSave;
            subMenu.HotKey = KeyGesture.Parse("Ctrl+S");
            subMenu.Command = AvaloniaDesigner.cmdSave;
            subMenu.Icon = new MaterialIcon
            {
                Kind = MaterialIconKind.ContentSave
            };
            Localization.BindLocalization(subMenu, x => x.FileSave);
        });
        
        menu.AddItem(subMenu =>
        {
            subMenu.Header = Localization.FileSaveAs;
            subMenu.HotKey = KeyGesture.Parse("Ctrl+Alt+S");
            subMenu.Command = AvaloniaDesigner.cmdSaveAs;
            subMenu.Icon = new MaterialIcon
            {
                Kind = MaterialIconKind.ContentSaveEdit
            };
            Localization.BindLocalization(subMenu, x => x.FileSaveAs);
        });
        
        menu.AddItem(subMenu =>
        {
            subMenu.Header = Localization.FileSaveToCloud;
            subMenu.HotKey = KeyGesture.Parse("Ctrl+Shift+S");
            subMenu.Command = AvaloniaDesigner.cmdSaveToCloud;
            subMenu.Icon = new MaterialIcon
            {
                Kind = MaterialIconKind.CloudUpload
            };
            Localization.BindLocalization(subMenu, x => x.FileSaveToCloud);
        });
        
        menu.AddItem(subMenu =>
        {
            subMenu.Header = Localization.FileSaveWithRandomData;
            subMenu.Command = AvaloniaDesigner.cmdSaveWithRandomData;
            subMenu.Icon = new MaterialIcon
            {
                Kind = MaterialIconKind.ContentSavePlus
            };
            Localization.BindLocalization(subMenu, x => x.FileSaveWithRandomData);
        });
        
        menu.AddSeparator();
        
        menu.AddItem(subMenu =>
        {
            subMenu.Header = Localization.FilePageSetup;
            subMenu.Command = AvaloniaDesigner.cmdPageSetup;
            subMenu.Icon = new MaterialIcon
            {
                Kind = MaterialIconKind.FileCog
            };
            Localization.BindLocalization(subMenu, x => x.FilePageSetup);
        });
        
        menu.AddItem(subMenu =>
        {
            subMenu.Header = Localization.FilePreview;
            subMenu.Command = AvaloniaDesigner.cmdPreview;
            subMenu.HotKey = KeyGesture.Parse("Ctrl+P");
            subMenu.Icon = new MaterialIcon
            {
                Kind = MaterialIconKind.PrinterEye
            };
            Localization.BindLocalization(subMenu, x => x.FilePreview);
        });
        
        menu.AddSeparator();
        
        menu.AddItem(subMenu =>
        {
            subMenu.Header = Localization.FileSelectLanguage;
            subMenu.Command = ReactiveCommand.Create(() =>
            {
                AvaloniaDesigner.cmdSelectLanguage.Execute(null);
                Localization.Update();
            });
            subMenu.Icon = new MaterialIcon
            {
                Kind = MaterialIconKind.PrinterEye
            };
            Localization.BindLocalization(subMenu, x => x.FileSelectLanguage);
        });
        
        menu.AddSeparator();
        
        menu.AddItem(subMenu =>
        {
            subMenu.Header = Localization.FileExit;
            subMenu.Command = finishAppCmd;
            subMenu.Icon = new MaterialIcon
            {
                Kind = MaterialIconKind.Close
            };
            Localization.BindLocalization(subMenu, x => x.FileExit);
        });

        return menu;
    }

    private MenuItem GenerateEditMenu()
    {
        var menu = new MenuItem { Header = Localization.Edit };
        Localization.BindLocalization(menu, x => x.Edit);
        
        menu.AddItem(subMenu =>
        {
            subMenu.Header = Localization.EditUndo;
            subMenu.HotKey = KeyGesture.Parse("Ctrl+Z");
            subMenu.Command = AvaloniaDesigner.cmdUndo;
            subMenu.Icon = new MaterialIcon
            {
                Kind = MaterialIconKind.Undo
            };
            Localization.BindLocalization(subMenu, x => x.EditUndo);
        });
        menu.AddItem(subMenu =>
        {
            subMenu.Header = Localization.EditRedo;
            subMenu.HotKey = KeyGesture.Parse("Ctrl+Y");
            subMenu.Command = AvaloniaDesigner.cmdRedo;
            subMenu.Icon = new MaterialIcon
            {
                Kind = MaterialIconKind.Redo
            };
            Localization.BindLocalization(subMenu, x => x.EditRedo);
        });
        menu.AddSeparator();
        menu.AddItem(subMenu =>
        {
            subMenu.Header = Localization.EditCut;
            subMenu.HotKey = KeyGesture.Parse("Ctrl+X");
            subMenu.Command = AvaloniaDesigner.cmdCut;
            subMenu.Icon = new MaterialIcon
            {
                Kind = MaterialIconKind.ContentCut
            };
            Localization.BindLocalization(subMenu, x => x.EditCut);
        });
        menu.AddItem(subMenu =>
        {
            subMenu.Header = Localization.EditCopy;
            subMenu.HotKey = KeyGesture.Parse("Ctrl+C");
            subMenu.Command = AvaloniaDesigner.cmdCopy;
            subMenu.Icon = new MaterialIcon
            {
                Kind = MaterialIconKind.ContentCopy
            };
            Localization.BindLocalization(subMenu, x => x.EditCopy);
        });
        menu.AddItem(subMenu =>
        {
            subMenu.Header = Localization.EditPaste;
            subMenu.HotKey = KeyGesture.Parse("Ctrl+V");
            subMenu.Command = AvaloniaDesigner.cmdPaste;
            subMenu.Icon = new MaterialIcon
            {
                Kind = MaterialIconKind.ContentPaste
            };
            Localization.BindLocalization(subMenu, x => x.EditPaste);
        });
        menu.AddSeparator();
        menu.AddItem(subMenu =>
        {
            subMenu.Header = Localization.EditNewPage;
            subMenu.HotKey = KeyGesture.Parse("Ctrl+N");
            subMenu.Command = AvaloniaDesigner.cmdNewPage;
            subMenu.Icon = new MaterialIcon
            {
                Kind = MaterialIconKind.FileDocumentAdd
            };
            Localization.BindLocalization(subMenu, x => x.EditNewPage);
        });
        menu.AddItem(subMenu =>
        {
            subMenu.Header = Localization.EditCopyPage;
            subMenu.Command = AvaloniaDesigner.cmdCopyPage;
            subMenu.Icon = new MaterialIcon
            {
                Kind = MaterialIconKind.FileDocumentMultiple
            };
            Localization.BindLocalization(subMenu, x => x.EditCopyPage);
        });
        menu.AddItem(subMenu =>
        {
            subMenu.Header = Localization.EditDeletePage;
            subMenu.Command = AvaloniaDesigner.cmdDeletePage;
            subMenu.Icon = new MaterialIcon
            {
                Kind = MaterialIconKind.FileDocumentDelete
            };
            Localization.BindLocalization(subMenu, x => x.EditDeletePage);
        });
        menu.AddSeparator();
        menu.AddItem(subMenu =>
        {
            subMenu.Header = Localization.EditSelectAll;
            subMenu.HotKey = KeyGesture.Parse("Ctrl+A");
            subMenu.Command = AvaloniaDesigner.cmdSelectAll;
            subMenu.Icon = new MaterialIcon
            {
                Kind = MaterialIconKind.SelectAll
            };
            Localization.BindLocalization(subMenu, x => x.EditSelectAll);
        });
        menu.AddSeparator();
        menu.AddItem(subMenu =>
        {
            subMenu.Header = Localization.EditGroup;
            subMenu.Command = AvaloniaDesigner.cmdGroup;
            subMenu.Icon = new MaterialIcon
            {
                Kind = MaterialIconKind.Group
            };
            Localization.BindLocalization(subMenu, x => x.EditGroup);
        });
        menu.AddItem(subMenu =>
        {
            subMenu.Header = Localization.EditUngroup;
            subMenu.Command = AvaloniaDesigner.cmdUngroup;
            subMenu.Icon = new MaterialIcon
            {
                Kind = MaterialIconKind.Ungroup
            };
            Localization.BindLocalization(subMenu, x => x.EditUngroup);
        });
        menu.AddSeparator();
        menu.AddItem(subMenu =>
        {
            subMenu.Header = Localization.EditFind;
            subMenu.HotKey = KeyGesture.Parse("Ctrl+F");
            subMenu.Command = AvaloniaDesigner.cmdFind;
            subMenu.Icon = new MaterialIcon
            {
                Kind = MaterialIconKind.FindInPage
            };
            Localization.BindLocalization(subMenu, x => x.EditFind);
        });
        menu.AddItem(subMenu =>
        {
            subMenu.Header = Localization.EditReplace;
            subMenu.HotKey = KeyGesture.Parse("Ctrl+H");
            subMenu.Command = AvaloniaDesigner.cmdReplace;
            subMenu.Icon = new MaterialIcon
            {
                Kind = MaterialIconKind.FindReplace
            };
            Localization.BindLocalization(subMenu, x => x.EditReplace);
        });

        return menu;
    }

    private MenuItem GenerateViewMenu(ICommand pluginCmd)
    {
        var menu = new MenuItem { Header = Localization.View };
        Localization.BindLocalization(menu, x => x.View);

        var barraFerramenta = new MenuItem { Header = Localization.ViewToolbars };
        Localization.BindLocalization(barraFerramenta, x => x.ViewToolbars);
        menu.Items.Add(barraFerramenta);

        barraFerramenta.Events().GotFocus.Subscribe(_ =>
        {
            barraFerramenta.Items.Clear();
            foreach (var item in Designer.Plugins.Cast<IDesignerPlugin>()
                         .Where(x => x is DesignerToolbarBase).Cast<DesignerToolbarBase>())
            {
                var menuItem = new MenuItem { Header = item.Text };
                var checkBox = new CheckBox
                {
                    BorderThickness = Thickness.Parse("0"),
                    IsHitTestVisible = false,
                    IsChecked = item.Visible
                };
                menuItem.Icon = checkBox;

                menuItem.Command = ReactiveCommand.Create(() =>
                {
                    item.Visible = !item.Visible;
                    checkBox.IsChecked = item.Visible;
                    Designer.LayoutToolbars();
                });

                barraFerramenta.Items.Add(menuItem);
            }
        });

        var mensagem = Config.Root.FindItem("Designer").FindItem("MessagesWindow").GetProp("Visible");
        Designer.MainMenu.miViewMessages.SetChecked(mensagem == "1");
        Designer.MainMenu.miViewMessages.ToCheckedObservable()
            .Subscribe(x => Designer.MainMenu.miViewMessages.SetChecked(x));
        menu.AddItem(subMenu =>
        {
            subMenu.Header = Localization.ViewMessages;
            subMenu.Icon = new MaterialIcon
            {
                Kind = MaterialIconKind.MessageNotification
            };
            subMenu.Command = ReactiveCommand.Create(() => { Designer.MainMenu.miViewMessages.ToggleChecked(); });

            Localization.BindLocalization(subMenu, x => x.ViewMessages);
        });

        menu.AddSeparator();

        int[] checkeds = [4, 5];
        for (var i = 4; i < 7; i++)
        {
            var dropItem = Designer.MainMenu.miView.DropDownItems[i];
            menu.Items.Add(CreateCheckedItem(dropItem.Text, dropItem, checkeds.Contains(i)));
        }

        menu.Items.Add(new MenuItem
        {
            Header = Designer.MainMenu.miView.DropDownItems[7].Text,
            Command = ReactiveCommand.Create(() => { Designer.MainMenu.miView.DropDownItems[7].PerformClick(); })
        });

        menu.Items.Add(new MenuItem
        {
            Header = Designer.MainMenu.miView.DropDownItems[8].Text,
            Command = ReactiveCommand.Create(() => { Designer.MainMenu.miView.DropDownItems[8].PerformClick(); })
        });

        menu.AddSeparator();
        
        var unidadeMenu = new MenuItem();
        menu.Items.Add(unidadeMenu);
        
        menu.AddSeparator();
        menu.AddItem(subMenu =>
        {
            subMenu.Header = "Plugin Manager";
            subMenu.Icon = new MaterialIcon
            {
                Kind = MaterialIconKind.Plugin
            };
            subMenu.Command = pluginCmd;
        });
        menu.AddItem(subMenu =>
        {
            subMenu.Header = Localization.ViewOptions;
            subMenu.Icon = new MaterialIcon
            {
                Kind = MaterialIconKind.Cogs
            };
            subMenu.Command = AvaloniaDesigner.cmdOptions;
            Localization.BindLocalization(subMenu, x => x.ViewOptions);
        });
        
        menu.Events().SubmenuOpened.Subscribe(_ =>
        {
            for (var i = 4; i < 9; i++)
                ((MenuItem)menu.Items[i-1]).Header = Designer.MainMenu.miView.DropDownItems[i].Text;
            
            var unidadeToolbar = (ToolStripMenuItem)Designer.MainMenu.miView.DropDownItems[10];

            unidadeMenu.Header = unidadeToolbar.Text;
            unidadeMenu.Items.Clear();
            
            var defaultUnit = Config.Root.FindItem("Designer").FindItem("Report").GetProp("Units");
            var unitName = Res.TryGet($"Forms,ReportPageOptions,{defaultUnit}");
            foreach (var item in unidadeToolbar.DropDownItems.Cast<ToolStripMenuItem>())
            {
                var checkBox = new CheckBox
                {
                    BorderThickness = Thickness.Parse("0"),
                    IsHitTestVisible = false,
                    IsChecked = unitName == item.Text
                };
                var menuItem = new MenuItem
                {
                    Header = item.Text,
                    Icon = checkBox
                };

                menuItem.Command = ReactiveCommand.Create(() =>
                {
                    checkBox.IsChecked = !checkBox.IsChecked;

                    foreach (var undMenu in unidadeMenu.Items.Cast<MenuItem>())
                    {
                        if (undMenu == menuItem) continue;
                        if (undMenu.Icon is not CheckBox mCheck) continue;

                        mCheck.IsChecked = !checkBox.IsChecked;
                    }

                    item.PerformClick();
                });

                unidadeMenu.Items.Add(menuItem);
            }
        });
        
        return menu;
    }

    private MenuItem GenerateInsertMenu()
    {
        var insertCommand = ReactiveCommand.Create<ObjectInfo>(
            item => { Designer.InsertObject(item, InsertFrom.NewObject); },
            Designer.cmdInsert.WhenAnyValue(x => x.Enabled));

        var menu = new MenuItem { Header = Localization.Insert };
        Localization.BindLocalization(menu, x => x.Insert);

        menu.Events().GotFocus.Subscribe(_ =>
        {
            menu.Items.Clear();
            var item = RegisteredObjects.FindObject(typeof(ReportPage));
            GenerateInsertMenu(menu, insertCommand, item.Items);
        });

        return menu;
    }

    private void GenerateInsertMenu(MenuItem menuItem, ICommand command, IEnumerable<ObjectInfo> itens)
    {
        foreach (var item in itens)
        {
            var menu = new MenuItem
            {
                Header = Res.TryGet(item.Text)
            };

            if (item.Items.Count > 0)
            {
                GenerateInsertMenu(menu, command, item.Items);
            }
            else
            {
                menu.Command = command;
                menu.CommandParameter = item;

                if (insertIcons.TryGetValue(item.ImageIndex, out var icon))
                {
                    menu.Icon = new MaterialIcon
                    {
                        Kind = icon
                    };
                }
            }

            menuItem.Items.Add(menu);
        }
    }

    private MenuItem GenerateReportMenu()
    {
        var menu = new MenuItem { Header = Localization.Report };
        Localization.BindLocalization(menu, x => x.Report);

        var rptToolMenu = Designer.MainMenu.miReport;
        
        menu.Items.Add(CreateCheckedItem(rptToolMenu.DropDownItems[0].Text, rptToolMenu.DropDownItems[0], true));
        menu.Items.Add(CreateCheckedItem(rptToolMenu.DropDownItems[1].Text, rptToolMenu.DropDownItems[1], false));
        menu.Items.Add(CreateCheckedItem(rptToolMenu.DropDownItems[2].Text, rptToolMenu.DropDownItems[2], true));
        menu.Items.Add(CreateCheckedItem(rptToolMenu.DropDownItems[3].Text, rptToolMenu.DropDownItems[3], true));
        menu.Items.Add(CreateCheckedItem(rptToolMenu.DropDownItems[4].Text, rptToolMenu.DropDownItems[4], false));
        menu.Items.Add(CreateCheckedItem(rptToolMenu.DropDownItems[5].Text, rptToolMenu.DropDownItems[5], false));
        menu.Items.Add(CreateCheckedItem(rptToolMenu.DropDownItems[6].Text, rptToolMenu.DropDownItems[6], false));
        
        menu.AddSeparator();
        menu.AddItem(subMenu =>
        {
            subMenu.Header = rptToolMenu.DropDownItems[8].Text;
            subMenu.Icon = new MaterialIcon
            {
                Kind = MaterialIconKind.FormatListBulleted
            };
            subMenu.Command = ReactiveCommand.Create(() =>
            {
                rptToolMenu.DropDownItems[8].PerformClick();
            });
        });
        menu.AddItem(subMenu =>
        {
            subMenu.Header = rptToolMenu.DropDownItems[9].Text;
            subMenu.Icon = new MaterialIcon
            {
                Kind = MaterialIconKind.Style
            };
            subMenu.Command = ReactiveCommand.Create(() =>
            {
                rptToolMenu.DropDownItems[9].PerformClick();
            });
        });
        menu.AddSeparator();
        menu.AddItem(subMenu =>
        {
            subMenu.Header = Localization.ReportValidation;
            subMenu.Icon = new MaterialIcon
            {
                Kind = MaterialIconKind.FileDocumentCheck
            };
            subMenu.Command = AvaloniaDesigner.cmdReportValidation;
            Localization.BindLocalization(subMenu, x => x.ReportValidation);
        });
        menu.AddItem(subMenu =>
        {
            subMenu.Header = Localization.ReportOptions;
            subMenu.Icon = new MaterialIcon
            {
                Kind = MaterialIconKind.FileDocumentEdit
            };
            subMenu.Command = AvaloniaDesigner.cmdReportSettings;
            Localization.BindLocalization(subMenu, x => x.ReportOptions);
        });
        
        menu.Events().SubmenuOpened.Subscribe(x =>
        {
            for (var i = 0; i < 7; i++)
                ((MenuItem)menu.Items[i]).Header = rptToolMenu.DropDownItems[i].Text;

            ((MenuItem)menu.Items[8]).Header = rptToolMenu.DropDownItems[8].Text;
            ((MenuItem)menu.Items[9]).Header = rptToolMenu.DropDownItems[9].Text;
        });
        
        return menu;
    }
    
    private MenuItem GenerateDataMenu()
    {
        var menu = new MenuItem { Header = Localization.Data };
        Localization.BindLocalization(menu, x => x.Data);
        menu.AddItem(subMenu =>
        {
            subMenu.Header = Localization.DataChoose;
            subMenu.Icon = new MaterialIcon
            {
                Kind = MaterialIconKind.DatabaseSettings
            };
            subMenu.Command = AvaloniaDesigner.cmdChooseData;
            Localization.BindLocalization(subMenu, x => x.DataChoose);
        });
        menu.AddItem(subMenu =>
        {
            subMenu.Header = Localization.DataAdd;
            subMenu.Icon = new MaterialIcon
            {
                Kind = MaterialIconKind.DatabaseAdd
            };
            subMenu.Command = AvaloniaDesigner.cmdAddData;
            Localization.BindLocalization(subMenu, x => x.DataAdd);
        });

        return menu;
    }
    
    private MenuItem GenerateHelpMenu()
    {
        var menu = new MenuItem { Header = Localization.Help };
        Localization.BindLocalization(menu, x => x.Help);
        
        menu.AddItem(subMenu =>
        {
            subMenu.Header = Localization.HelpContents;
            subMenu.HotKey = KeyGesture.Parse("F1");
            subMenu.Command = AvaloniaDesigner.cmdHelpContents;
            subMenu.Icon = new MaterialIcon
            {
                Kind = MaterialIconKind.HelpCircle
            };
            Localization.BindLocalization(subMenu, x => x.HelpContents);
        });
        
        menu.AddItem(subMenu =>
        {
            subMenu.Header = Localization.HelpAccount;
            subMenu.Command = AvaloniaDesigner.cmdAccount;
            subMenu.Icon = new MaterialIcon
            {
                Kind = MaterialIconKind.Account
            };
            Localization.BindLocalization(subMenu, x => x.HelpAccount);
        });
        
        menu.AddItem(subMenu =>
        {
            subMenu.Header = Localization.HelpAbout;
            subMenu.Command = AvaloniaDesigner.cmdAbout;
            subMenu.Icon = new MaterialIcon
            {
                Kind = MaterialIconKind.About
            };
            Localization.BindLocalization(subMenu, x => x.HelpAbout);
        });

        return menu;
    }
    
    private static MenuItem CreateCheckedItem(string header, ToolStripItem item, bool defaultValue)
    {
        var menuItem = new MenuItem
        {
            Header = header,
            Command = ReactiveCommand.Create(item.ToggleChecked)
        };
        var checkBox = new CheckBox
        {
            BorderThickness = Thickness.Parse("0"),
            IsHitTestVisible = false
        };
        menuItem.Icon = checkBox;
        
        item.SetChecked(defaultValue);
        item.ToCheckedObservable().Subscribe(x => checkBox.IsChecked = x);

        return menuItem;
    }
    
    #endregion Methods
}