using SQLite;

namespace SPLITTR_Uwp.Core.Models;

public class GroupToUser
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string UserEmailId { get; set; }

    public string GroupUniqueId { get; set; }

    public GroupToUser(string userEmailId, string groupUniqueId)
    {
        UserEmailId = userEmailId;
        GroupUniqueId = groupUniqueId;
    }

    public GroupToUser()
    {

    }

}