public interface IBodyBullet
{
    void IInit(IsometricVector Dir, int Speed);

    bool IHit(IsometricBlock Target); //Destroy when Hit after check Target

    void IHit(); //Destroy when Hit
}