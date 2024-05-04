using System.ComponentModel;

namespace ScumDB.Models.Enums;

public enum PurgeType
{
    [Description("Only removes unassigned models (i.e without OwnerId)")]
    Soft,
    
    [Description("Removes everything according to the filter, makes no distinction")]
    Hard
}