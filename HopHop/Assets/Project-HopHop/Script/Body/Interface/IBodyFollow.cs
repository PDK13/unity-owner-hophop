public interface IBodyFollow
{
    void IFollow(IsometricVector Dir); //Active

    void IFollowIdentity(string Identity, IsometricVector Dir); //Check
}