using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Caramelo.MvvmApp.Avalonia;
using FastReport.Utils;
using FastReport.Designer.ViewModels;

namespace FastReport.Designer;

public partial class App : MvvmApplication<AppBootstrapperViewModel>
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        Window.WindowOpenedEvent.AddClassHandler(typeof(Window), CentralizarJanela);

        EsquecerPosicoesSalvas();
        Window.WindowClosedEvent.AddClassHandler(typeof(Window),
            (_, _) => Dispatcher.UIThread.Post(EsquecerPosicoesSalvas));
    }

    /// <summary>
    /// O FastReport grava a posição das janelas dele (Forms,PreviewForm...) ao fechar e, ao abrir de
    /// novo, a restaura antes de a janela aparecer, com Screens.ScreenFromWindow — que devolve null
    /// quando o (0,0) não está em monitor nenhum (monitores desalinhados), e o Preview quebrava com
    /// NullReferenceException. Sem o "Left" ele não restaura nada, e a janela abre centralizada.
    /// O Post deixa o FastReport gravar primeiro.
    /// </summary>
    private static void EsquecerPosicoesSalvas()
    {
        var indice = Config.Root.Find("Forms");
        if (indice < 0) return;

        var forms = Config.Root[indice];
        for (var i = 0; i < forms.Count; i++)
            forms[i].RemoveProp("Left");
    }

    /// <summary>
    /// Centraliza todo diálogo no monitor da janela dona (ou da principal). O CenterOwner não serve:
    /// a principal abre maximizada e o diálogo de boas-vindas aparece antes de o gerenciador de
    /// janelas terminar de posicioná-la, então ele caía num canto do outro monitor; e as janelas do
    /// DialogWindowResolver nem tinham posição inicial.
    /// </summary>
    private static void CentralizarJanela(object? sender, RoutedEventArgs e)
    {
        if (sender is not Window janela) return;
        if (Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop) return;

        // A principal se posiciona sozinha; antes dela existir só abre a splash, que é CenterScreen.
        var referencia = janela.Owner as Window ?? desktop.MainWindow;
        if (referencia == null || ReferenceEquals(janela, desktop.MainWindow)) return;

        // Depois do layout, para o tamanho das janelas com SizeToContent já estar calculado.
        Dispatcher.UIThread.Post(() =>
        {
            var tela = janela.Screens.ScreenFromWindow(referencia) ?? janela.Screens.Primary;
            if (tela == null) return;

            var area = tela.WorkingArea;
            var tamanho = PixelSize.FromSize(janela.FrameSize ?? janela.ClientSize, tela.Scaling);

            janela.Position = new PixelPoint(
                area.X + Math.Max(0, (area.Width - tamanho.Width) / 2),
                area.Y + Math.Max(0, (area.Height - tamanho.Height) / 2));
        }, DispatcherPriority.Loaded);
    }
}
