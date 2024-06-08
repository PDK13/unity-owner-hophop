public interface IBodyPhysic
{
    bool IControl();

    bool IControl(IsometricVector Dir);

    void IMove(bool State, IsometricVector Dir);

    void IMoveForce(bool State, IsometricVector Dir);

    void IForce(bool State, IsometricVector Dir, IsometricVector From);

    void IGravity(bool State);

    void IPush(bool State, IsometricVector Dir, IsometricVector From);

    void ICollide(IsometricVector Dir);
}