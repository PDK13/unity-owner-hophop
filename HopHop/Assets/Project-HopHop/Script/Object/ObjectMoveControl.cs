using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMoveControl : MonoBehaviour
{
    [SerializeField] private string m_keyStart = GameKey.OBJECT;
    [SerializeField] private string m_keyEnd = GameKey.OBJECT;

    private IsoDataBlockMove m_dataMove;

    private IsometricBlock m_block;

    private void Awake()
    {
        m_block = GetComponent<IsometricBlock>();

        GameEvent.onTriggerStart += SetTriggerStart;
    }

    private void Start()
    {
        foreach (var MoveCheck in m_block.Data.MoveData)
        {
            if (MoveCheck.KeyStart != m_keyStart || MoveCheck.KeyStart != m_keyEnd)
                continue;
            m_dataMove = MoveCheck;
            GameData.m_objectControlCount++;
            return;
        }
    }

    private void OnDestroy()
    {
        GameEvent.onTriggerStart -= SetTriggerStart;

        if (m_dataMove != null)
            GameData.m_objectControlCount--;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L) && m_dataMove != null)
            Destroy(this.gameObject);
    }

    private void SetTriggerStart(string Key)
    {
        if (Key == m_keyStart && m_dataMove != null)
        {
            SetMove(m_dataMove.Data[m_dataMove.Index].Dir, m_dataMove.Data[m_dataMove.Index].Length, m_dataMove.Dir == -1);
            m_dataMove.Index += m_dataMove.Dir;
            if (m_dataMove.Loop && (m_dataMove.Index < 0 || m_dataMove.Index > m_dataMove.Data.Count - 1))
            {
                m_dataMove.Dir *= -1;
                m_dataMove.Index += m_dataMove.Dir;
            }
            return;
        }

        foreach (var MoveCheck in m_block.Data.MoveData)
        {
            if (Key != MoveCheck.KeyStart)
                continue;
            SetMove(MoveCheck.Data[MoveCheck.Index].Dir, MoveCheck.Data[MoveCheck.Index].Length, MoveCheck.Dir == -1);
            MoveCheck.Index += m_dataMove.Dir;
            if (MoveCheck.Loop && (MoveCheck.Index < 0 || MoveCheck.Index > MoveCheck.Data.Count - 1))
            {
                MoveCheck.Dir *= -1;
                MoveCheck.Index += m_dataMove.Dir;
            }
        }
    }

    private void SetMove(IsoDir Dir, int Length, bool Revert)
    {
        Vector3 Pos = new Vector3(m_block.Pos.X, m_block.Pos.Y, m_block.Pos.H);
        DOTween.To(() => Pos, x => Pos = x, Pos + IsoVector.GetVectorDir(Dir, Revert) * Length, GameData.TimeMove).SetEase(Ease.Linear).OnUpdate(() =>
        {
            m_block.Pos = new IsoVector(Pos);
        }).OnComplete(() =>
        {
            GameEvent.SetTriggerEnd(GameKey.OBJECT);
        });
    } //Move!!
}