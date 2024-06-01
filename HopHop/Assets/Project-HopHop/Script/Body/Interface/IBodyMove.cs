public interface IBodyMove
{
    void IMove(IsometricVector Dir); //Active

    void IMoveIdentity(string Identity, IsometricVector Dir); //Check
}