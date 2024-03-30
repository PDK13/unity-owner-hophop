public interface IBodySwitch
{
    void ISwitchIdentity(string Identity, bool State);

    void ISwitchState(bool State);

    void ISwitchRevert();
}
