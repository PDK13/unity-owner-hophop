public interface IBodyStatic
{
    bool IControl();

    bool IControl(IsometricVector Dir);

    void IMove(bool State, IsometricVector Dir);
}