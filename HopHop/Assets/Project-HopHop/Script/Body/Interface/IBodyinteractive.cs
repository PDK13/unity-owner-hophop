public interface IBodyInteractive
{
    bool IInteractive();
}

public interface IBodyInteractiveActive
{
    bool IInteractive(IsometricVector Dir);
}