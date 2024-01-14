public interface IBodyTurn
{
    bool ITurnActive { get; set; }

    void IOnTurn(int Turn);

    void IOnStep(string Name);
}