using System;
using System.Linq.Expressions;
using System.Reactive.Subjects;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using FastReport.Utils;
using ReactiveUI;

namespace FastReport.Designer.Commom;

public sealed class FrMenuLocalization : ReactiveObject
{
    #region Fields

    private string file = string.Empty;
    private string fileNew = string.Empty;
    private string fileOpen = string.Empty;
    private string fileOpenPage = string.Empty;
    private string fileOpenCloud = string.Empty;
    private string fileRecentFiles = string.Empty;
    private string fileClose = string.Empty;
    private string fileSave = string.Empty;
    private string fileSaveAs = string.Empty;
    private string fileSaveWithRandomData = string.Empty;
    private string fileSaveAll = string.Empty;
    private string fileSaveToCloud = string.Empty;
    private string filePageSetup = string.Empty;
    private string filePrinterSetup = string.Empty;
    private string filePreview = string.Empty;
    private string filePreviewCloud = string.Empty;
    private string fileSelectLanguage = string.Empty;
    private string fileExit = string.Empty;
    private string edit = string.Empty;
    private string editUndo = string.Empty;
    private string editRedo = string.Empty;
    private string editCut = string.Empty;
    private string editCopy = string.Empty;
    private string editPaste = string.Empty;
    private string editDelete = string.Empty;
    private string editNewPage = string.Empty;
    private string editCopyPage = string.Empty;
    private string editDeletePage = string.Empty;
    private string editSelectAll = string.Empty;
    private string editGroup = string.Empty;
    private string editUngroup = string.Empty;
    private string editFind = string.Empty;
    private string editReplace = string.Empty;
    private string insert = string.Empty;
    private string report = string.Empty;
    private string reportValidation = string.Empty;
    private string reportOptions = string.Empty;
    private string data = string.Empty;
    private string dataChoose = string.Empty;
    private string dataAdd = string.Empty;
    private string dataShowData = string.Empty;
    private string view = string.Empty;
    private string viewToolbars = string.Empty;
    private string viewStartPage = string.Empty;
    private string viewMessages = string.Empty;
    private string viewOptions = string.Empty;
    private string window = string.Empty;
    private string windowCloseAll = string.Empty;
    private string help = string.Empty;
    private string helpContents = string.Empty;
    private string helpAbout = string.Empty;
    private string helpAccount = string.Empty;

    #endregion Fields

    #region Constructors

    public FrMenuLocalization()
    {
        Update();
    }

    #endregion Constructors
    
    #region Properties

    public string File
    {
        get => file;
        private set => this.RaiseAndSetIfChanged(ref file, value);
    }

    public string FileNew
    {
        get => fileNew;
        private set => this.RaiseAndSetIfChanged(ref fileNew, value);
    }

    public string FileOpen
    {
        get => fileOpen;
        private set => this.RaiseAndSetIfChanged(ref fileOpen, value);
    }
    
    public string FileOpenPage
    {
        get => fileOpenPage;
        private set => this.RaiseAndSetIfChanged(ref fileOpenPage, value);
    }
    
    public string FileOpenCloud
    {
        get => fileOpenCloud;
        private set => this.RaiseAndSetIfChanged(ref fileOpenCloud, value);
    }
    
    public string FileRecentFiles
    {
        get => fileRecentFiles;
        private set => this.RaiseAndSetIfChanged(ref fileRecentFiles, value);
    }

    public string FileClose
    {
        get => fileClose;
        private set => this.RaiseAndSetIfChanged(ref fileClose, value);
    }

    public string FileSave
    {
        get => fileSave;
        private set => this.RaiseAndSetIfChanged(ref fileSave, value);
    }

    public string FileSaveAs
    {
        get => fileSaveAs;
        private set => this.RaiseAndSetIfChanged(ref fileSaveAs, value);
    }

    public string FileSaveWithRandomData
    {
        get => fileSaveWithRandomData;
        private set => this.RaiseAndSetIfChanged(ref fileSaveWithRandomData, value);
    }
    
    public string FileSaveAll
    {
        get => fileSaveAll;
        private set => this.RaiseAndSetIfChanged(ref fileSaveAll, value);
    }

    public string FileSaveToCloud
    {
        get => fileSaveToCloud;
        private set => this.RaiseAndSetIfChanged(ref fileSaveToCloud, value);
    }
    
    public string FilePageSetup
    {
        get => filePageSetup;
        private set => this.RaiseAndSetIfChanged(ref filePageSetup, value);
    }

    public string FilePrinterSetup
    {
        get => filePrinterSetup;
        private set => this.RaiseAndSetIfChanged(ref filePrinterSetup, value);
    }

    public string FilePreview
    {
        get => filePreview;
        private set => this.RaiseAndSetIfChanged(ref filePreview, value);
    }

    public string FilePreviewCloud
    {
        get => filePreviewCloud;
        private set => this.RaiseAndSetIfChanged(ref filePreviewCloud, value);
    }
    
    public string FileSelectLanguage
    {
        get => fileSelectLanguage;
        private set => this.RaiseAndSetIfChanged(ref fileSelectLanguage, value);
    }

    public string FileExit
    {
        get => fileExit;
        private set => this.RaiseAndSetIfChanged(ref fileExit, value);
    }

    public string Edit
    {
        get => edit;
        private set => this.RaiseAndSetIfChanged(ref edit, value);
    }

    public string EditUndo
    {
        get => editUndo;
        private set => this.RaiseAndSetIfChanged(ref editUndo, value);
    }

    public string EditRedo
    {
        get => editRedo;
        private set => this.RaiseAndSetIfChanged(ref editRedo, value);
    }

    public string EditCut
    {
        get => editCut;
        private set => this.RaiseAndSetIfChanged(ref editCut, value);
    }

    public string EditCopy
    {
        get => editCopy;
        private set => this.RaiseAndSetIfChanged(ref editCopy, value);
    }

    public string EditPaste
    {
        get => editPaste;
        private set => this.RaiseAndSetIfChanged(ref editPaste, value);
    }

    public string EditDelete
    {
        get => editDelete;
        private set => this.RaiseAndSetIfChanged(ref editDelete, value);
    }

    public string EditNewPage
    {
        get => editNewPage;
        private set => this.RaiseAndSetIfChanged(ref editNewPage, value);
    }

    public string EditCopyPage
    {
        get => editCopyPage;
        private set => this.RaiseAndSetIfChanged(ref editCopyPage, value);
    }

    public string EditDeletePage
    {
        get => editDeletePage;
        private set => this.RaiseAndSetIfChanged(ref editDeletePage, value);
    }

    public string EditSelectAll
    {
        get => editSelectAll;
        private set => this.RaiseAndSetIfChanged(ref editSelectAll, value);
    }

    public string EditGroup
    {
        get => editGroup;
        private set => this.RaiseAndSetIfChanged(ref editGroup, value);
    }

    public string EditUngroup
    {
        get => editUngroup;
        private set => this.RaiseAndSetIfChanged(ref editUngroup, value);
    }

    public string EditFind
    {
        get => editFind;
        private set => this.RaiseAndSetIfChanged(ref editFind, value);
    }

    public string EditReplace
    {
        get => editReplace;
        private set => this.RaiseAndSetIfChanged(ref editReplace, value);
    }

    public string Insert
    {
        get => insert;
        private set => this.RaiseAndSetIfChanged(ref insert, value);
    }

    public string Report
    {
        get => report;
        private set => this.RaiseAndSetIfChanged(ref report, value);
    }

    public string ReportValidation
    {
        get => reportValidation;
        private set => this.RaiseAndSetIfChanged(ref reportValidation, value);
    }

    public string ReportOptions
    {
        get => reportOptions;
        private set => this.RaiseAndSetIfChanged(ref reportOptions, value);
    }

    public string Data
    {
        get => data;
        private set => this.RaiseAndSetIfChanged(ref data, value);
    }

    public string DataChoose
    {
        get => dataChoose;
        private set => this.RaiseAndSetIfChanged(ref dataChoose, value);
    }

    public string DataAdd
    {
        get => dataAdd;
        private set => this.RaiseAndSetIfChanged(ref dataAdd, value);
    }

    public string DataShowData
    {
        get => dataShowData;
        private set => this.RaiseAndSetIfChanged(ref dataShowData, value);
    }

    public string View
    {
        get => view;
        private set => this.RaiseAndSetIfChanged(ref view, value);
    }

    public string ViewToolbars
    {
        get => viewToolbars;
        private set => this.RaiseAndSetIfChanged(ref viewToolbars, value);
    }

    public string ViewStartPage
    {
        get => viewStartPage;
        private set => this.RaiseAndSetIfChanged(ref viewStartPage, value);
    }

    public string ViewMessages
    {
        get => viewMessages;
        private set => this.RaiseAndSetIfChanged(ref viewMessages, value);
    }

    public string ViewOptions
    {
        get => viewOptions;
        private set => this.RaiseAndSetIfChanged(ref viewOptions, value);
    }

    public string Window
    {
        get => window;
        private set => this.RaiseAndSetIfChanged(ref window, value);
    }

    public string WindowCloseAll
    {
        get => windowCloseAll;
        private set => this.RaiseAndSetIfChanged(ref windowCloseAll, value);
    }

    public string Help
    {
        get => help;
        private set => this.RaiseAndSetIfChanged(ref help, value);
    }

    public string HelpContents
    {
        get => helpContents;
        private set => this.RaiseAndSetIfChanged(ref helpContents, value);
    }

    public string HelpAbout
    {
        get => helpAbout;
        private set => this.RaiseAndSetIfChanged(ref helpAbout, value);
    }

    public string HelpAccount
    {
        get => helpAccount;
        private set => this.RaiseAndSetIfChanged(ref helpAccount, value);
    }

    #endregion Properties
    
    #region Methods

    public void BindLocalization(MenuItem menuItem, Expression<Func<FrMenuLocalization, string>> field)
    {
        var source = new Subject<object?>();
        menuItem.Bind(HeaderedSelectingItemsControl.HeaderProperty, source);
        this.WhenAnyValue(field).Subscribe(s => source.OnNext(s));
    }
    
    public void Update()
    {
        var myRes = new MyRes("Designer,Menu");
        
        File = myRes.Get("File");
        FileNew = myRes.Get("File,New");
        FileOpen = myRes.Get("File,Open");
        FileOpenPage = myRes.Get("File,OpenPage");
        FileOpenCloud = myRes.Get("File,OpenViaCloud");
        FileRecentFiles = myRes.Get("File,RecentFiles");
        FileClose = myRes.Get("File,Close");
        FileSave = myRes.Get("File,Save");
        FileSaveAs = myRes.Get("File,SaveAs");
        FileSaveWithRandomData = myRes.Get("File,SaveWithRandomData");
        FileSaveAll = myRes.Get("File,SaveAll");
        FileSaveToCloud = myRes.Get("File,SaveToCloud");
        FilePageSetup = myRes.Get("File,PageSetup");
        FilePrinterSetup = myRes.Get("File,PrinterSetup");
        FilePreview = myRes.Get("File,Preview");
        FilePreviewCloud = myRes.Get("File,PreviewCloud");
        FileSelectLanguage = myRes.Get("File,SelectLanguage");
        FileExit = myRes.Get("File,Exit");
        
        
        Edit = myRes.Get("Edit");
        EditUndo = myRes.Get("Edit,Undo");
        EditRedo = myRes.Get("Edit,Redo");
        EditCut = myRes.Get("Edit,Cut");
        EditCopy = myRes.Get("Edit,Copy");
        EditPaste = myRes.Get("Edit,Paste");
        EditDelete = myRes.Get("Edit,Delete");
        EditNewPage = myRes.Get("Edit,NewPage");
        EditCopyPage = myRes.Get("Edit,CopyPage");
        EditDeletePage = myRes.Get("Edit,DeletePage");
        EditSelectAll = myRes.Get("Edit,SelectAll");
        EditGroup = myRes.Get("Edit,Group");
        EditUngroup = myRes.Get("Edit,Ungroup");
        EditFind = myRes.Get("Edit,Find");
        EditReplace = myRes.Get("Edit,Replace");
        
        Insert = myRes.Get("Insert");
        Report = myRes.Get("Report");
        ReportValidation = myRes.Get("Report,Validate");
        ReportOptions = myRes.Get("Report,Options");
        Data = myRes.Get("Data");
        DataChoose = myRes.Get("Data,Choose");
        DataAdd = myRes.Get("Data,Add");
        DataShowData = myRes.Get("Data,ShowData");
        View = myRes.Get("View");
        ViewToolbars = myRes.Get("View,Toolbars");
        ViewStartPage = myRes.Get("View,StartPage");
        ViewMessages = Res.Get("Designer,ToolWindow,Messages");
        ViewOptions = myRes.Get("View,Options");
        Window = myRes.Get("Window");
        WindowCloseAll = myRes.Get("Window,CloseAll");
        Help = myRes.Get("Help");
        HelpContents = myRes.Get("Help,Contents");
        HelpAbout = myRes.Get("Help,About");
        HelpAccount = myRes.Get("Help,Account");
    }

    #endregion Methods
}