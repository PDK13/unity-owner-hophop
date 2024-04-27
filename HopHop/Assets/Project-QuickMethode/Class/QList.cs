using System.Collections.Generic;
using UnityEngine;

public class QList
{
    public static bool SetSwap<T>(List<T> List, int IndexA, int IndexB)
    {
        IndexA = Mathf.Clamp(IndexA, 0, List.Count - 1);
        IndexB = Mathf.Clamp(IndexB, 0, List.Count - 1);

        if (IndexA == IndexB)
            return false;

        T Temp = List[IndexA];
        List[IndexA] = List[IndexB];
        List[IndexB] = Temp;

        return true;
    }
}