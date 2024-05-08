public interface IBodyStatic
{
    bool IControl(IsometricVector Dir);

    void IMove(bool State, IsometricVector Dir);
}