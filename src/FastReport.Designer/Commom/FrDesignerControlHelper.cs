using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
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
using FastReport.Designer.Views;
using FastReport.Utils;
using Material.Icons;
using Material.Icons.Avalonia;
using ReactiveUI;
using CheckBox = Avalonia.Controls.CheckBox;
using Control = System.Windows.Forms.Control;

namespace FastReport.Designer.Commom;

internal sealed class FrDesignerMenuHelper
{
    #region Fields

    private readonly DesignerControl designer;
    private readonly AvaloniaDesignerControl control;
    private Dictionary<int, MaterialIconKind> insertIcons = [];

    #endregion Fields
    
    #region Constructors

    public FrDesignerMenuHelper(AvaloniaDesignerControl designerControl)
    {
        control = designerControl;
        designer = designerControl.InnerDesigner;
        Initialize();
    }

    #endregion Constructors

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
        var menu = new MenuItem { Header = "Arquivo" };
        menu.Items.Add(new MenuItem
        {
            Header = "Novo...",
            Command = control.cmdNew,
            Icon =  new MaterialIcon
            {
                Kind = MaterialIconKind.FileDocument
            }
        });
        
        menu.Items.Add(new MenuItem
        {
            Header = "Abrir...",
            HotKey = KeyGesture.Parse("Ctrl+O"),
            Command = control.cmdOpen,
            Icon =  new MaterialIcon
            {
                Kind = MaterialIconKind.FolderOpen
            }
        });
        
        menu.Items.Add(new MenuItem
        {
            Header = "Abrir Pagina...",
            Command = control.cmdOpenPage,
            Icon =  new MaterialIcon
            {
                Kind = MaterialIconKind.FileDownload
            }
        });
        
        menu.Items.Add(new MenuItem
        {
            Header = "Abrir da Nuvem...",
            Command = control.cmdOpenViaCloud,
            Icon =  new MaterialIcon
            {
                Kind = MaterialIconKind.CloudDownload
            }
        });

        menu.Items.Add(new Separator());

        var recentFilesMenu = new MenuItem { Header = "Arquivos Recentes" };
        var recentFiles = designer.RecentFiles();
        foreach (var recentFile in recentFiles)
        {
            var item =new MenuItem
            {
                Header = recentFile,
                Command = ReactiveCommand.Create(() =>
                {
                    designer.cmdRecentFiles.LoadFile(recentFile);
                })
            };

            if (designer.cmdRecentFiles.IsCloudFile(recentFile))
            {
                item.Icon = new MaterialIcon
                {
                    Kind = MaterialIconKind.Cloud
                };
            }

            recentFilesMenu.Items.Add(item);
        }

        recentFilesMenu.IsEnabled = recentFilesMenu.Items.Count > 0;
        menu.Items.Add(recentFilesMenu);
        
        menu.Items.Add(new Separator());
        
        menu.Items.Add(new MenuItem
        {
            Header = "Salvar...",
            HotKey = KeyGesture.Parse("Ctrl+S"),
            Command = control.cmdSave,
            Icon =  new MaterialIcon
            {
                Kind = MaterialIconKind.ContentSave
            }
        });
        
        menu.Items.Add(new MenuItem
        {
            Header = "Salvar como...",
            HotKey = KeyGesture.Parse("Ctrl+Alt+S"),
            Command = control.cmdSaveAs,
            Icon =  new MaterialIcon
            {
                Kind = MaterialIconKind.ContentSaveEdit
            }
        });
        
        menu.Items.Add(new MenuItem
        {
            Header = "Salvar na Nuvem...",
            HotKey = KeyGesture.Parse("Ctrl+Shift+S"),
            Command = control.cmdSaveToCloud,
            Icon =  new MaterialIcon
            {
                Kind = MaterialIconKind.CloudUpload
            }
        });
        
        menu.Items.Add(new MenuItem
        {
            Header = "Salvar com dados...",
            Command = control.cmdSaveWithRandomData,
            Icon =  new MaterialIcon
            {
                Kind = MaterialIconKind.ContentSavePlus
            }
        });
        
        menu.Items.Add(new Separator());
        
        menu.Items.Add(new MenuItem
        {
            Header = "Configuração da Página...",
            Command = control.cmdPageSetup,
            Icon =  new MaterialIcon
            {
                Kind = MaterialIconKind.FileCog
            }
        });
        
        menu.Items.Add(new MenuItem
        {
            Header = "Pré-Visualizar...",
            Command = control.cmdPreview,
            HotKey = KeyGesture.Parse("Ctrl+P"),
            Icon =  new MaterialIcon
            {
                Kind = MaterialIconKind.PrinterEye
            }
        });
        
        menu.Items.Add(new Separator());
        
        menu.Items.Add(new MenuItem
        {
            Header = "Selecionar Idioma",
            Command = control.cmdSelectLanguage,
            Icon =  new MaterialIcon
            {
                Kind = MaterialIconKind.PrinterEye
            }
        });
        
        menu.Items.Add(new Separator());
        
        menu.Items.Add(new MenuItem
        {
            Header = "Sair...",
            Command = finishAppCmd,
            Icon =  new MaterialIcon
            {
                Kind = MaterialIconKind.Close
            }
        });

        return menu;
    }

    private MenuItem GenerateEditMenu()
    {
        var menu = new MenuItem { Header = "Editar" };
        menu.Items.Add(new MenuItem
        {
            Header = "Desfazer",
            HotKey = KeyGesture.Parse("Ctrl+Z"),
            Command = control.cmdUndo,
            Icon =  new MaterialIcon
            {
                Kind = MaterialIconKind.Undo
            }
        });
        menu.Items.Add(new MenuItem
        {
            Header = "Refazer",
            HotKey = KeyGesture.Parse("Ctrl+Y"),
            Command = control.cmdRedo,
            Icon =  new MaterialIcon
            {
                Kind = MaterialIconKind.Redo
            }
        });
        menu.Items.Add(new Separator());
        menu.Items.Add(new MenuItem
        {
            Header = "Recortar",
            HotKey = KeyGesture.Parse("Ctrl+X"),
            Command = control.cmdCut,
            Icon =  new MaterialIcon
            {
                Kind = MaterialIconKind.ContentCut
            }
        });
        menu.Items.Add(new MenuItem
        {
            Header = "Copiar",
            HotKey = KeyGesture.Parse("Ctrl+C"),
            Command = control.cmdCopy,
            Icon =  new MaterialIcon
            {
                Kind = MaterialIconKind.ContentCopy
            }
        });
        menu.Items.Add(new MenuItem
        {
            Header = "Colar",
            HotKey = KeyGesture.Parse("Ctrl+V"),
            Command = control.cmdPaste,
            Icon =  new MaterialIcon
            {
                Kind = MaterialIconKind.ContentPaste
            }
        });
        menu.Items.Add(new Separator());
        menu.Items.Add(new MenuItem
        {
            Header = "Nova Página",
            HotKey = KeyGesture.Parse("Ctrl+N"),
            Command = control.cmdNewPage,
            Icon =  new MaterialIcon
            {
                Kind = MaterialIconKind.FileDocumentAdd
            }
        });
        menu.Items.Add(new MenuItem
        {
            Header = "Copiar Página",
            Command = control.cmdCopyPage,
            Icon =  new MaterialIcon
            {
                Kind = MaterialIconKind.FileDocumentMultiple
            }
        });
        menu.Items.Add(new MenuItem
        {
            Header = "Apagar Página",
            Command = control.cmdDeletePage,
            Icon =  new MaterialIcon
            {
                Kind = MaterialIconKind.FileDocumentDelete
            }
        });
        menu.Items.Add(new Separator());
        menu.Items.Add(new MenuItem
        {
            Header = "Selecionar tudo",
            HotKey = KeyGesture.Parse("Ctrl+A"),
            Command = control.cmdSelectAll,
            Icon =  new MaterialIcon
            {
                Kind = MaterialIconKind.SelectAll
            }
        });
        menu.Items.Add(new Separator());
        menu.Items.Add(new MenuItem
        {
            Header = "Agrupar",
            Command = control.cmdGroup,
            Icon =  new MaterialIcon
            {
                Kind = MaterialIconKind.Group
            }
        });
        menu.Items.Add(new MenuItem
        {
            Header = "Desagrupar",
            Command = control.cmdUngroup,
            Icon =  new MaterialIcon
            {
                Kind = MaterialIconKind.Ungroup
            }
        });
        menu.Items.Add(new Separator());
        menu.Items.Add(new MenuItem
        {
            Header = "Localizar...",
            HotKey = KeyGesture.Parse("Ctrl+F"),
            Command = control.cmdFind,
            Icon =  new MaterialIcon
            {
                Kind = MaterialIconKind.FindInPage
            }
        });
        menu.Items.Add(new MenuItem
        {
            Header = "Substituir...",
            HotKey = KeyGesture.Parse("Ctrl+H"),
            Command = control.cmdReplace,
            Icon =  new MaterialIcon
            {
                Kind = MaterialIconKind.FindReplace
            }
        });

        return menu;
    }
    
    private MenuItem GenerateViewMenu(ICommand pluginCmd)
    {
        var menu = new MenuItem { Header = "Visualizar" };
        var barraFerramenta = new MenuItem { Header = "Barra de Ferramenta" };
        menu.Items.Add(barraFerramenta);

        foreach (var item in designer.Plugins.Cast<IDesignerPlugin>()
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
                designer.LayoutToolbars();
            });
            
            barraFerramenta.Items.Add(menuItem);
        }
        
        menu.Items.Add(new MenuItem
        {
            Header = "Mensagens",
            Icon =  new MaterialIcon
            {
                Kind = MaterialIconKind.MessageNotification
            },
            Command  = ReactiveCommand.Create(() =>
            {
                designer.MainMenu.miViewMessages.ToggleChecked();
            })
        });

        menu.Items.Add(new Separator());

        int[] checkeds = [4, 5];
        for (var i = 4; i < 7; i++)
        {
            var dropItem = designer.MainMenu.miView.DropDownItems[i];
            menu.Items.Add(CreateCheckedItem(dropItem.Text, dropItem, checkeds.Contains(i)));
        }
        
        menu.Items.Add(new MenuItem
        {
            Header = designer.MainMenu.miView.DropDownItems[7].Text,
            Command  = ReactiveCommand.Create(() =>
            {
                designer.MainMenu.miView.DropDownItems[7].PerformClick();
            })
        });
        
        menu.Items.Add(new MenuItem
        {
            Header = designer.MainMenu.miView.DropDownItems[8].Text,
            Command = ReactiveCommand.Create(() =>
            {
                designer.MainMenu.miView.DropDownItems[8].PerformClick();
            })
        });
        
        menu.Items.Add(new Separator());
        
        var unidadeMenu = new MenuItem { Header = "Unidades" };
        menu.Items.Add(unidadeMenu);

        var defaultUnit = Config.Root.FindItem("Designer").FindItem("Report").GetProp("Units");
        var unitName = Res.TryGet($"Forms,ReportPageOptions,{defaultUnit}");
        
        var unidadeToolbar = (ToolStripMenuItem)designer.MainMenu.miView.DropDownItems[10];
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
                Icon =  checkBox
            };

            menuItem.Command = ReactiveCommand.Create(() =>
            {
                checkBox.IsChecked = !checkBox.IsChecked;

                foreach (var undMenu in unidadeMenu.Items.Cast<MenuItem>())
                {
                    if (undMenu == menuItem) continue;
                    
                    ((CheckBox)undMenu.Icon!).IsChecked = !checkBox.IsChecked;
                }

                item.PerformClick();
            });

            unidadeMenu.Items.Add(menuItem);
        }
        
        menu.Items.Add(new Separator());
        menu.Items.Add(new MenuItem
        {
            Header = "Plugin Manager",
            Icon =  new MaterialIcon
            {
                Kind = MaterialIconKind.Plugin
            },
            Command = pluginCmd
        });
        menu.Items.Add(new MenuItem
        {
            Header = "Opções",
            Icon =  new MaterialIcon
            {
                Kind = MaterialIconKind.Cogs
            },
            Command  = control.cmdOptions
        });

        return menu;
    }
    
    private MenuItem GenerateInsertMenu()
    {
        var insertCommand = ReactiveCommand.Create<ObjectInfo>(item =>
        {
            designer.InsertObject(item, InsertFrom.NewObject);
            
        }, designer.cmdInsert.WhenAnyValue(x => x.Enabled));
        
        var menu = new MenuItem { Header = "Inserir" };
        var item = RegisteredObjects.FindObject(typeof(ReportPage));
        GenerateInsertMenu(menu, insertCommand, item.Items);

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
        var menu = new MenuItem { Header = "Relatório" };

        var rptToolMenu = designer.MainMenu.miReport;
        
        menu.Items.Add(CreateCheckedItem("Título do Relatório", rptToolMenu.DropDownItems[0], true));
        menu.Items.Add(CreateCheckedItem("Sumário do Relatório", rptToolMenu.DropDownItems[1], false));
        menu.Items.Add(CreateCheckedItem("Cabeçalho de Página", rptToolMenu.DropDownItems[2], true));
        menu.Items.Add(CreateCheckedItem("Rodapé de Página", rptToolMenu.DropDownItems[3], true));
        menu.Items.Add(CreateCheckedItem("Cabeçalho de Coluna", rptToolMenu.DropDownItems[4], false));
        menu.Items.Add(CreateCheckedItem("Rodapé de Coluna", rptToolMenu.DropDownItems[5], false));
        menu.Items.Add(CreateCheckedItem("Deslocamento", rptToolMenu.DropDownItems[6], false));
        menu.Items.Add(new Separator());
        menu.Items.Add(new MenuItem
        {
            Header = "Configurar Bandas...",
            Icon =  new MaterialIcon
            {
                Kind = MaterialIconKind.FormatListBulleted
            },
            Command = ReactiveCommand.Create(() =>
            {
                rptToolMenu.DropDownItems[8].PerformClick();
            })
        });
        menu.Items.Add(new MenuItem
        {
            Header = "Estilos...",
            Icon =  new MaterialIcon
            {
                Kind = MaterialIconKind.Style
            },
            Command = ReactiveCommand.Create(() =>
            {
                rptToolMenu.DropDownItems[9].PerformClick();
            })
        });
        menu.Items.Add(new Separator());
        menu.Items.Add(new MenuItem
        {
            Header = "Validar Relatório",
            Icon =  new MaterialIcon
            {
                Kind = MaterialIconKind.FileDocumentCheck
            },
            Command = control.cmdReportValidation
        });
        menu.Items.Add(new MenuItem
        {
            Header = "Opções...",
            Icon =  new MaterialIcon
            {
                Kind = MaterialIconKind.FileDocumentEdit
            },
            Command = control.cmdReportSettings
        });
        
        return menu;
    }
    
    private MenuItem GenerateDataMenu()
    {
        var menu = new MenuItem { Header = "Dados" };
        menu.Items.Add(new MenuItem
        {
            Header = "Escolher Dados do Relatório",
            Icon =  new MaterialIcon
            {
                Kind = MaterialIconKind.DatabaseSettings
            },
            Command = control.cmdChooseData
        });
        menu.Items.Add(new MenuItem
        {
            Header = "Adicionar fonte de dados",
            Icon =  new MaterialIcon
            {
                Kind = MaterialIconKind.DatabaseAdd
            },
            Command = control.cmdAddData
        });

        return menu;
    }
    
    private MenuItem GenerateHelpMenu()
    {
        var menu = new MenuItem { Header = "Ajuda" };
        menu.Items.Add(new MenuItem
        {
            Header = "Conteúdo de Ajuda...",
            HotKey = KeyGesture.Parse("F1"),
            Command = control.cmdHelpContents,
            Icon =  new MaterialIcon
            {
                Kind = MaterialIconKind.HelpCircle
            }
        });
        
        menu.Items.Add(new MenuItem
        {
            Header = "Conta...",
            Command = control.cmdAccount,
            Icon =  new MaterialIcon
            {
                Kind = MaterialIconKind.Account
            }
        });
        
        menu.Items.Add(new MenuItem
        {
            Header = "Sobre...",
            Command = control.cmdAbout,
            Icon =  new MaterialIcon
            {
                Kind = MaterialIconKind.About
            }
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
