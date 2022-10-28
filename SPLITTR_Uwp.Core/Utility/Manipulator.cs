namespace SPLITTR_Uwp.Core.Utility;

public class Manipulator : IStringManipulator
{

    public string GetUserInitial(string userName)
    {

        var names = userName.Split(' ');
        var initials = "";
        foreach (var name in names)
        {
            initials += name[0];
            if (initials.Length == 2)
            {
                break;
            }
        }
        return initials;
    }
}
