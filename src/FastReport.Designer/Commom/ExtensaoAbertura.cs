using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FastReport.Designer.Commom;

/// <summary>
/// --extensao &lt;dll&gt; [argumentos...]: o programa que chama o Designer entrega o relatório já pronto,
/// com dados registrados e parâmetros, como o Report.Design() faz dentro de uma aplicação. A dll
/// precisa ter um método <c>public static void AbrirRelatorio(string[] argumentos, Report report)</c>,
/// que recebe os argumentos que vêm depois dela e preenche o report.
/// </summary>
public static class ExtensaoAbertura
{
    #region Methods

    public static void Abrir(string caminho, string[] argumentos, Report report)
    {
        // LoadFrom: as dependências da extensão são procuradas na pasta dela.
        var assembly = Assembly.LoadFrom(Path.GetFullPath(caminho));
        var metodo = assembly.GetExportedTypes()
                         .Select(x => x.GetMethod("AbrirRelatorio", BindingFlags.Public | BindingFlags.Static,
                             [typeof(string[]), typeof(Report)]))
                         .FirstOrDefault(x => x != null)
                     ?? throw new MissingMethodException(
                         $"{caminho} não tem um método public static void AbrirRelatorio(string[], Report).");

        try
        {
            metodo.Invoke(null, [argumentos, report]);
        }
        catch (TargetInvocationException e) when (e.InnerException != null)
        {
            throw e.InnerException;
        }
    }

    #endregion Methods
}
