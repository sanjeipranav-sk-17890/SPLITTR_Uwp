using System;
using SQLite;

namespace SPLITTR_Uwp.Core.Models;

public class Group
{
    private string _groupName;

    [PrimaryKey, Unique]
    public string GroupUniqueId { get; set; }

    //Created Owner Id
    public string UserEmailId { get; set; }

    public virtual string GroupName
    {
        get => _groupName;
        set => _groupName = value;
    }

    public DateTime CreateDateTime { get; set; }



    public Group(string userEmailId, string groupUniqueId, string groupName, DateTime createDateTime)
    {
        UserEmailId = userEmailId;
        _groupName = groupName;
        CreateDateTime = createDateTime;
        GroupUniqueId = groupUniqueId;
    }

    public Group()
    {
        CreateDateTime = DateTime.Now;
        GroupUniqueId = GroupUniqueId = DateTime.Now.Ticks.ToString();
    }
}