public interface IBodySwitch
{
    void ISwitchIdentity(string Identity, bool State); //Check!

    void ISwitch(bool State); //Active!

    void ISwitch();
}