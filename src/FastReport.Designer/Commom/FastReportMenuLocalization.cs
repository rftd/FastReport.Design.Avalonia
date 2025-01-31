using System;
using System.Linq.Expressions;
using System.Reactive.Subjects;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using FastReport.Utils;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace FastReport.Designer.Commom;

public sealed partial class FastReportMenuLocalization : ReactiveObject
{
    #region Constructors

    public FastReportMenuLocalization()
    {
        Update();
    }

    #endregion Constructors
    
    #region Properties

    [Reactive]
    public partial string File { get; set; }

    [Reactive]
    public partial string FileNew { get; set; }

    [Reactive]
    public partial string FileOpen { get; set; }
    
    [Reactive]
    public partial string FileOpenPage { get; set; }
    
    [Reactive]
    public partial string FileOpenCloud { get; set; }
    
    [Reactive]
    public partial string FileRecentFiles { get; set; }

    [Reactive]
    public partial string FileClose { get; set; }

    [Reactive]
    public partial string FileSave { get; set; }

    [Reactive]
    public partial string FileSaveAs { get; set; }

    [Reactive]
    public partial string FileSaveWithRandomData { get; set; }
    
    [Reactive]
    public partial string FileSaveAll { get; set; }

    [Reactive]
    public partial string FileSaveToCloud { get; set; }
    
    [Reactive]
    public partial string FilePageSetup { get; set; }

    [Reactive]
    public partial string FilePrinterSetup { get; set; }

    [Reactive]
    public partial string FilePreview { get; set; }

    [Reactive]
    public partial string FilePreviewCloud { get; set; }
    
    [Reactive]
    public partial string FileSelectLanguage { get; set; }

    [Reactive]
    public partial string FileExit { get; set; }

    [Reactive]
    public partial string Edit { get; set; }

    [Reactive]
    public partial string EditUndo { get; set; }

    [Reactive]
    public partial string EditRedo { get; set; }

    [Reactive]
    public partial string EditCut { get; set; }

    [Reactive]
    public partial string EditCopy { get; set; }

    [Reactive]
    public partial string EditPaste { get; set; }

    [Reactive]
    public partial string EditDelete { get; set; }

    [Reactive]
    public partial string EditNewPage { get; set; }

    [Reactive]
    public partial string EditCopyPage { get; set; }

    [Reactive]
    public partial string EditDeletePage { get; set; }

    [Reactive]
    public partial string EditSelectAll { get; set; }

    [Reactive]
    public partial string EditGroup { get; set; }

    [Reactive]
    public partial string EditUngroup { get; set; }

    [Reactive]
    public partial string EditFind { get; set; }

    [Reactive]
    public partial string EditReplace { get; set; }

    [Reactive]
    public partial string Insert { get; set; }

    [Reactive]
    public partial string Report { get; set; }

    [Reactive]
    public partial string ReportValidation { get; set; }

    [Reactive]
    public partial string ReportOptions { get; set; }

    [Reactive]
    public partial string Data { get; set; }

    [Reactive]
    public partial string DataChoose { get; set; }

    [Reactive]
    public partial string DataAdd { get; set; }

    [Reactive]
    public partial string DataShowData { get; set; }
    
    [Reactive]
    public partial string View { get; set; }

    [Reactive]
    public partial string ViewToolbars { get; set; }

    [Reactive]
    public partial string ViewStartPage { get; set; }

    [Reactive]
    public partial string ViewMessages { get; set; }

    [Reactive]
    public partial string ViewOptions { get; set; }

    [Reactive]
    public partial string Window { get; set; }

    [Reactive]
    public partial string WindowCloseAll { get; set; }

    [Reactive]
    public partial string Help { get; set; }

    [Reactive]
    public partial string HelpContents { get; set; }

    [Reactive]
    public partial string HelpAbout { get; set; }

    [Reactive]
    public partial string HelpAccount { get; set; }

    #endregion Properties
    
    #region Methods

    public void BindLocalization(MenuItem menuItem, Expression<Func<FastReportMenuLocalization, string>> field)
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