using Avalonia;
using Avalonia.Controls;
using AvaloniaEdit.Utils;

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

        TextProperty.Changed.Subscribe(value =>
        {
            CaptionHeader.Text = value.NewValue.GetValueOrDefault();
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
