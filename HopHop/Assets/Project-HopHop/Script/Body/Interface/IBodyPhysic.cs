public interface IBodyPhysic
{
    void IMoveForce(bool State, IsometricVector Dir);

    void IForce(bool State, IsometricVector Dir);

    bool IMove(IsometricVector Dir);

    void IGravity(bool State);

    void IPush(bool State, IsometricVector Dir, IsometricVector From);
}