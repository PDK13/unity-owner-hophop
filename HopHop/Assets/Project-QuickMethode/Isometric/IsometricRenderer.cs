using IsometricMethode;
using QuickMethode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IsometricBlock))]
[RequireComponent(typeof(SpriteRenderer))]
public class IsometricRenderer : MonoBehaviour
{
    #region Varible: Renderer Manager

    [Header("XY")]
    [SerializeField] private List<Sprite> m_spriteJoin; //Block(s) will chain-join each other!

    [Header("H")]
    [SerializeField] private List<IsometricBlock> m_blockTop; //Block(s) will be auto generated after this Block Bottom Root!

    #endregion

    #region ================================================================== Sprite Join

    public void SetSpriteJoin(IsoVector PosPrimary)
    {
        if (m_spriteJoin == null)
            return;

        if (m_spriteJoin.Count <= 1)
            return;

        //Index = (|X| + |Y|) % Count
        GetComponent<SpriteRenderer>().sprite = m_spriteJoin[(Mathf.Abs((int)PosPrimary.X) + Mathf.Abs((int)PosPrimary.Y)) % m_spriteJoin.Count];
    }

    public void SetSpriteAlpha(float Alpha)
    {
        Color Color = GetComponent<SpriteRenderer>().color;
        QColor.SetColor(ref Color, Alpha);
        GetComponent<SpriteRenderer>().color = Color;
    }

    public void SetSpriteColor(Color Color, float Alpha = 1)
    {
        GetComponent<SpriteRenderer>().color = Color;
        SetSpriteAlpha(Alpha);
    }

    #endregion
}
