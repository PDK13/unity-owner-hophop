using UnityEngine;

public class BodyChild : MonoBehaviour
{
    public BodySquare Square { private set; get; } = null;

    private void Awake()
    {
        Square = GetComponentInChildren<BodySquare>();
    }
}