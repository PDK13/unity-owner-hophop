public interface IBodyPhysic
{
    bool IControl(IsometricVector Dir);

    void IMove(bool State, IsometricVector Dir);

    void IForce(bool State, IsometricVector Dir);

    void IGravity(bool State);

    void IPush(bool State, IsometricVector Dir, IsometricVector From);
}