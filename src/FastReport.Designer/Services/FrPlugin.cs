namespace FastReport.Designer.Services;

public class FrPlugin
{
    #region Constructors

    internal FrPlugin(string id, string name, string description, string version)
    {
        Id = id;
        Name = name;
        Description = description;
        Version = version;
    }

    #endregion Constructors

    #region Properties

    public string Id { get; }
    
    public string Name { get; }
    
    public string Description { get; } 
    
    public string Version { get; } 

    #endregion Properties
}