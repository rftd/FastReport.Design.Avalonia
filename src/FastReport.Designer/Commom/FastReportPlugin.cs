﻿using System;
using ReactiveUI;

namespace FastReport.Designer.Commom;

public class FastReportPlugin : ReactiveObject, IComparable<FastReportPlugin>, IEquatable<FastReportPlugin>
{
    #region Constructors

    internal FastReportPlugin(string id, string name, string description, string version)
    {
        Id = id;
        Name = name;
        Description = description;
        Version = version;
    }

    #endregion Constructors

    #region Properties

    public string Id { get; }

    public string NameVersion => $"{Name} - {Version}";
    
    public string Name { get; }
    
    public string Description { get; } 
    
    public string Version { get; } 

    #endregion Properties

    #region Methods

    public bool Equals(FastReportPlugin? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id && Version == other.Version;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((FastReportPlugin) obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Version);
    }

    public int CompareTo(FastReportPlugin? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        var idComparison = string.Compare(Id, other.Id, StringComparison.Ordinal);
        return idComparison != 0 ? idComparison : string.Compare(Version, other.Version, StringComparison.Ordinal);
    }

    #endregion Methods
}