using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using AvaloniaEdit.Utils;
using DynamicData.Binding;

namespace FastReport.Designer.Controls;

public partial class DefaultCaption : UserControl
{
    #region Avalonia Properties

    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<DefaultCaption, string>(nameof(Text), defaultValue: "");

    #endregion Avalonia Properties

    #region Constructors

    public DefaultCaption()
    {
        InitializeComponent();
        
        this.WhenValueChanged(x => x.Text).Subscribe(value =>
        {
            CaptionHeader.Text = value;
        });
    }

    #endregion Constructors

    #region Properties

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    #endregion Properties
}
