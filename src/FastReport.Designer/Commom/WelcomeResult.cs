using FastReport.Wizards;

namespace FastReport.Designer.Commom;

public class WelcomeResult
{
    public WelcomeType Tipo { get; set; }
    
    public WizardBase? Wizard { get; set; }

    public string? File { get; set; }
}