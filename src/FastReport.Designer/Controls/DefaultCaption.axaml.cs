using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using AvaloniaEdit.Utils;
using DynamicData.Binding;

namespace FastReport.Designer.Controls;

public partial class DefaultCaption : UserControl
{
    #region Avalonia Properties
    
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<DefaultCaption, string>(nameof(Title), defaultValue: "");

    #endregion Avalonia Properties

    #region Constructors
    
    public DefaultCaption()
    {
        InitializeComponent();
        
        this.WhenValueChanged(x => x.Title).Subscribe(value => CaptionHeader.Text = value);
    }

    #endregion Constructors

    #region Properties

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    #endregion Properties
}
