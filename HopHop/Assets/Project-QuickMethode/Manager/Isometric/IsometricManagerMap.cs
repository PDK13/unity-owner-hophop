using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class IsometricManagerMap
{
    public const string NAME_CURSON = "iso-curson";
    public const string NAME_ROOM = "iso-room-";

    private IsometricManager m_manager;

    [SerializeField] private string m_name;
    [SerializeField] private Transform m_root;

    public string Name
    {
        get => m_name;
        set { m_name = value; m_root.name = NameFixed; }
    }

    public string NameFixed => string.Format("{0}{1}", NAME_ROOM, m_name);

    public Transform Root => m_root;

    public bool Active
    {
        get => m_root.gameObject.activeInHierarchy;
        set => m_root.gameObject.SetActive(value);
    }

    public Action onCreate;
    public Action onRemove;

    public List<string> Command = new List<string>();
    public List<IsometricDataRoomPosH> PosH = new List<IsometricDataRoomPosH>();
    public List<IsometricDataRoomTag> Tag = new List<IsometricDataRoomTag>();
    public List<IsometricDataRoomIdentity> Identity = new List<IsometricDataRoomIdentity>();

    public bool Emty => Tag == null ? true : Tag.Count == 0;

    public IsometricManagerMap(IsometricManager Manager)
    {
        if (Manager == null)
        {
            Debug.Log("Manager can't be null!");
            return;
        }
        m_manager = Manager;
    }

    #region ======================================================================== Refresh

    public void SetRefresh()
    {
        SetRefreshPosH();
        SetRefreshTag();
        SetRefreshIdentity();
    }

    private void SetRefreshPosH()
    {
        int Index = 0;
        while (Index < PosH.Count)
        {
            PosH[Index].SetRefresh();
            if (PosH[Index].Block.Count == 0)
                PosH.RemoveAt(Index);
            else
                Index++;
        }
    }

    private void SetRefreshTag()
    {
        int Index = 0;
        while (Index < Tag.Count)
        {
            Tag[Index].SetRefresh();
            if (Tag[Index].Block.Count == 0)
                Tag.RemoveAt(Index);
            else
                Index++;
        }
    }

    private void SetRefreshIdentity()
    {
        int Index = 0;
        while (Index < Identity.Count)
        {
            Identity[Index].SetRefresh();
            if (Identity[Index].Block.Count == 0)
                Identity.RemoveAt(Index);
            else
                Index++;
        }
    }

    #endregion

    #region ======================================================================== Init

    public void SetInit()
    {
        if (m_root != null)
            return;
        //
        m_root = new GameObject(NameFixed).transform;
        m_root.transform.parent = m_manager.transform;
    }

    public void SetInit(string Name)
    {
        if (m_manager == null)
            return;
        //
        m_name = Name;
        //
        m_root = new GameObject(NameFixed).transform;
        m_root.transform.parent = m_manager.transform;
    }

    public void SetInit(Transform Root)
    {
        if (m_manager == null)
            return;
        //
        m_name = Root.name.Replace(NAME_ROOM, "");
        //
        m_root = Root;
    }

    #endregion

    #region ======================================================================== Block

    #region Block Create

    public IsometricBlock SetBlockCreate(IsometricVector BlockPos, GameObject BlockPrefab, bool BlockLastData, string BlockIdentity = "")
    {
        if (BlockPrefab == null)
        {
            Debug.LogWarningFormat("Block {0} not found!", BlockPos.Encypt);
            return null;
        }

        if (BlockPrefab.GetComponent<IsometricBlock>() == null)
        {
            Debug.LogWarningFormat("Block {0} {1} not found IsometricBlock!", BlockPos.Encypt, BlockPrefab.name);
            return null;
        }

        //Create
        GameObject BlockObject = QGameObject.SetCreate(BlockPrefab);

        //Block
        IsometricBlock Block = QComponent.GetComponent<IsometricBlock>(BlockObject);
        Block.WorldManager = m_manager;
        Block.Pos = BlockPos.Fixed;
        Block.PosPrimary = BlockPos.Fixed;

        //Data
        if (BlockLastData)
        {
            IsometricBlock BlockExist = GetBlockCurrent(BlockPos); //Check old Block exist in current Pos for save it's data to new Block!
            if (BlockExist != null)
            {
                foreach (var DataExist in BlockExist.GetComponents<IsometricDataInit>())
                {
                    var DataNew = Block.AddComponent<IsometricDataInit>();
                    DataNew.SetValue(DataExist);
                }
                foreach (var DataExist in BlockExist.GetComponents<IsometricDataMove>())
                {
                    var DataNew = Block.AddComponent<IsometricDataMove>();
                    DataNew.SetValue(DataExist);
                }
                foreach (var DataExist in BlockExist.GetComponents<IsometricDataAction>())
                {
                    var DataNew = Block.AddComponent<IsometricDataAction>();
                    DataNew.SetValue(DataExist);
                }
                foreach (var DataExist in BlockExist.GetComponents<IsometricDataTeleport>())
                {
                    var DataNew = Block.AddComponent<IsometricDataTeleport>();
                    DataNew.SetValue(DataExist);
                }
            }
        }

        //Remove
        if (Block.PosType == IsometricPosType.Free && Application.isPlaying)
        {
            //When in playing, FREE Block's Pos Primary will not be track, so just can be find by it own Tag!
        }
        else
        {
            //Delete
            SetBlockRemovePrimary(BlockPos.Fixed);

            //World
            int IndexPosH = GetWorldIndexPosH(BlockPos.HInt);
            if (IndexPosH == -1)
            {
                PosH.Add(new IsometricDataRoomPosH(BlockPos.Fixed.HInt, new List<IsometricBlock>()));
                IndexPosH = PosH.Count - 1;
                PosH[IndexPosH].Block.Add(Block);
            }
            else
            {
                PosH[IndexPosH].Block.Add(Block);
            }
        }

        //Tag
        List<string> TagFind = Block.GetComponent<IsometricBlock>().Tag;
        if (TagFind.Count == 0)
        {
            //None Tag!
            int TagIndex = GetWorldIndexTag("");
            if (TagIndex == -1)
            {
                Tag.Add(new IsometricDataRoomTag("", new List<IsometricBlock>()));
                TagIndex = Tag.Count - 1;
                Tag[TagIndex].Block.Add(Block);
            }
            else
            {
                Tag[TagIndex].Block.Add(Block);
            }
        }
        else
        {
            //Got Tag!
            foreach (string TagCheck in TagFind)
            {
                int TagIndex = GetWorldIndexTag(TagCheck);
                if (TagIndex == -1)
                {
                    Tag.Add(new IsometricDataRoomTag(TagCheck));
                    TagIndex = Tag.Count - 1;
                    Tag[TagIndex].Block.Add(Block);
                }
                else
                {
                    Tag[TagIndex].Block.Add(Block);
                }
            }
        }

        //Identity
        if (!string.IsNullOrEmpty(BlockIdentity))
        {
            //Replace Identity of Block
            if (!string.IsNullOrEmpty(Block.Identity))
                Debug.LogFormat("Block {0} with identity {1} replaced with {2}", Block.Name, Block.Identity, BlockIdentity);
            Block.SetIdentity(BlockIdentity, false);
        }

        if (!string.IsNullOrEmpty(Block.Identity))
        {
            //Add Identity of Block to list
            int IdentityIndex = GetWorldIndexIdentity(Block.Identity);
            if (IdentityIndex == -1)
            {
                Identity.Add(new IsometricDataRoomIdentity(Block.Identity, new List<IsometricBlock>()));
                IdentityIndex = Identity.Count - 1;
                Identity[IdentityIndex].Block.Add(Block);
            }
            else
            {
                Identity[IdentityIndex].Block.Add(Block);
            }
        }

        //Scene
        Transform ParentPosH = m_root.Find(GetWorldNamePosH(BlockPos.Fixed));
        if (ParentPosH != null)
        {
            Block.transform.SetParent(ParentPosH, true);
        }
        else
        {
            ParentPosH = QGameObject.SetCreate(GetWorldNamePosH(BlockPos.Fixed), m_root).transform;
            Block.transform.SetParent(ParentPosH, true);
        }

        return Block;
    }

    #endregion

    #region Block Update

    public string SetBlockUpdateIdentity(IsometricBlock Block, string BlockIdentity)
    {
        if (!string.IsNullOrEmpty(Block.Identity))
        {
            //Remove Block out of Identity list
            int IdentityIndex = GetWorldIndexIdentity(Block.Identity);
            if (IdentityIndex != -1)
            {
                Identity[IdentityIndex].Block.Remove(Block);
                if (Identity[IdentityIndex].Block.Count == 0)
                    Identity.RemoveAt(IdentityIndex);
            }
        }

        if (!string.IsNullOrEmpty(BlockIdentity))
        {
            //Replace Identity of Block
            if (!string.IsNullOrEmpty(Block.Identity))
                Debug.LogFormat("Block {0} with identity {1} replaced with {2}", Block.Name, Block.Identity, BlockIdentity);
            Block.SetIdentity(BlockIdentity, false);
        }

        if (!string.IsNullOrEmpty(Block.Identity))
        {
            //Add Identity of Block to list
            int IdentityIndex = GetWorldIndexIdentity(Block.Identity);
            if (IdentityIndex == -1)
            {
                Identity.Add(new IsometricDataRoomIdentity(Block.Identity, new List<IsometricBlock>()));
                IdentityIndex = Identity.Count - 1;
                Identity[IdentityIndex].Block.Add(Block);
            }
            else
            {
                Identity[IdentityIndex].Block.Add(Block);
            }
        }

        return BlockIdentity;
    }

    #endregion

    #region Block Get

    public IsometricBlock GetBlockPrimary(IsometricVector Pos)
    {
        //World
        int IndexPosH = GetWorldIndexPosH(Pos.Fixed.HInt);
        if (IndexPosH == -1)
        {
            return null;
        }

        for (int i = 0; i < PosH[IndexPosH].Block.Count; i++)
        {
            if (PosH[IndexPosH].Block[i].PosPrimary != Pos.Fixed)
            {
                continue;
            }

            return PosH[IndexPosH].Block[i];
        }

        return null;
    }

    public IsometricBlock GetBlockCurrent(IsometricVector Pos, params string[] Tag)
    {
        if (Tag.Length > 0)
        {
            //Find all Block with know tag - More Quickly!!
            foreach (string TagFind in Tag)
            {
                int TagIndex = GetWorldIndexTag(TagFind);
                if (TagIndex == -1)
                    //Not exist Tag in Tag List!
                    continue;

                for (int BlockIndex = 0; BlockIndex < this.Tag[TagIndex].Block.Count; BlockIndex++)
                {
                    if (this.Tag[TagIndex].Block[BlockIndex].Pos.Fixed != Pos.Fixed)
                        continue;

                    return this.Tag[TagIndex].Block[BlockIndex];
                }
            }
        }
        else
        {
            //Find all block with unknow tag - More slower!! (But always found Block)
            foreach (IsometricDataRoomTag TagCheck in this.Tag)
            {
                foreach (IsometricBlock BlockCheck in TagCheck.Block)
                {
                    if (BlockCheck.Pos.Fixed != Pos.Fixed)
                        continue;

                    return BlockCheck;
                }
            }
        }

        return null;
    }

    public List<IsometricBlock> GetBlockCurrentAll(IsometricVector Pos, params string[] Tag)
    {
        List<IsometricBlock> List = new List<IsometricBlock>();

        if (Tag.Length > 0)
        {
            //Find all Block with know tag - More Quickly!!
            foreach (string TagFind in Tag)
            {
                int TagIndex = GetWorldIndexTag(TagFind);
                if (TagIndex == -1)
                    //Not exist Tag in Tag List!
                    continue;

                for (int BlockIndex = 0; BlockIndex < this.Tag[TagIndex].Block.Count; BlockIndex++)
                {
                    if (this.Tag[TagIndex].Block[BlockIndex].Pos.Fixed != Pos.Fixed)
                    {
                        continue;
                    }

                    if (!List.Contains(this.Tag[TagIndex].Block[BlockIndex]))
                        List.Add(this.Tag[TagIndex].Block[BlockIndex]);
                }
            }
        }
        else
        {
            //Find all block with unknow tag - More slower!! (But always found Block)
            foreach (IsometricDataRoomTag TagCheck in this.Tag)
            {
                foreach (IsometricBlock BlockCheck in TagCheck.Block)
                {
                    if (BlockCheck.Pos.Fixed != Pos.Fixed)
                        continue;

                    if (!List.Contains(BlockCheck))
                        List.Add(BlockCheck);
                }
            }
        }

        return List;
    }

    public List<IsometricBlock> GetBlockByTag(string Tag)
    {
        foreach (IsometricDataRoomTag Check in this.Tag)
        {
            if (Check.Tag != Tag)
                continue;
            return Check.Block;
        }
        return null;
    }

    public List<IsometricBlock> GetBlockByIdentity(string Identity)
    {
        foreach (IsometricDataRoomIdentity Check in this.Identity)
        {
            if (Check.Identity != Identity)
                continue;
            return Check.Block;
        }
        return null;
    }

    #endregion

    #region Block Remove

    public void SetBlockRemovePrimary(IsometricVector Pos, float Delay = 0)
    {
        //World
        int IndexPosH = GetWorldIndexPosH(Pos.Fixed.HInt);
        if (IndexPosH == -1)
            return;

        for (int i = 0; i < PosH[IndexPosH].Block.Count; i++)
        {
            if (PosH[IndexPosH].Block[i].PosPrimary != Pos.Fixed)
                continue;

            IsometricBlock Block = PosH[IndexPosH].Block[i];

            //World
            PosH[IndexPosH].Block.Remove(Block);
            if (PosH[IndexPosH].Block.Count == 0)
                PosH.RemoveAt(IndexPosH);

            //Tag
            foreach (string TagCheck in Block.Tag)
            {
                int TagIndex = GetWorldIndexTag(TagCheck);
                if (TagIndex != -1)
                {
                    Tag[TagIndex].Block.Remove(Block);
                    if (Tag[TagIndex].Block.Count == 0)
                        Tag.RemoveAt(TagIndex);
                }
            }

            //Identity
            int IdentityIndex = GetWorldIndexIdentity(Block.Identity);
            if (IdentityIndex != -1)
            {
                Identity[IdentityIndex].Block.Remove(Block);
                if (Identity[IdentityIndex].Block.Count == 0)
                    Identity.RemoveAt(IdentityIndex);
            }

            //Scene
            if (Application.isEditor && !Application.isPlaying)
                GameObject.DestroyImmediate(Block.gameObject);
            else
                GameObject.Destroy(Block.gameObject, Delay);

            break;
        }
    }

    public void SetBlockRemoveInstant(IsometricBlock Block, float Delay)
    {
        if (Block.PosType == IsometricPosType.Track)
        {
            //World
            PosH[GetWorldIndexPosH(Block.Pos.HInt)].Block.Remove(Block);
        }

        //Tag
        foreach (string TagCheck in Block.Tag)
        {
            int TagIndex = GetWorldIndexTag(TagCheck);
            if (TagIndex != -1)
            {
                Tag[TagIndex].Block.Remove(Block);
                if (Tag[TagIndex].Block.Count == 0)
                    Tag.RemoveAt(TagIndex);
            }
        }

        //Identity
        int IdentityIndex = GetWorldIndexIdentity(Block.Identity);
        if (IdentityIndex != -1)
        {
            Identity[IdentityIndex].Block.Remove(Block);
            if (Identity[IdentityIndex].Block.Count == 0)
                Identity.RemoveAt(IdentityIndex);
        }

        //Scene
        if (Application.isEditor && !Application.isPlaying)
            GameObject.DestroyImmediate(Block.gameObject);
        else
            GameObject.Destroy(Block.gameObject, Delay);
    }

    #endregion

    #endregion

    #region ======================================================================== World

    #region World Read

    public bool SetWorldRead()
    {
        if (m_root == null)
            return false;
        //
        //Clear Current World!!
        SetWorldRemove();

        //Store Block(s) Found!!
        List<IsometricBlock> BlockFound = m_root.GetComponentsInChildren<IsometricBlock>().ToList();
        GameObject BlockStore = QGameObject.SetCreate("BlockStore");
        foreach (IsometricBlock Block in BlockFound)
        {
            if (Block.gameObject.name == NAME_CURSON)
            {
                continue;
            }

            Block.transform.SetParent(BlockStore.transform);
        }
        //
        //Remove All GameObject!!
        for (int i = m_root.transform.childCount - 1; i >= 0; i--)
        {
#if UNITY_EDITOR
            if (m_root.GetChild(i).gameObject.name == NAME_CURSON)
            {
                continue;
            }
#endif
            if (m_root.GetChild(i).GetComponent<Camera>() != null)
            {
                continue;
            }

            if (Application.isEditor && !Application.isPlaying)
            {
                GameObject.DestroyImmediate(m_root.GetChild(i).gameObject);
            }
            else
            {
                GameObject.Destroy(m_root.GetChild(i).gameObject);
            }
        }
        //
        //Add Block(s) Found!!
        foreach (IsometricBlock Block in BlockFound)
        {
            if (Block.gameObject.name == NAME_CURSON)
            {
                continue;
            }

            SetWorldReadBlock(Block);
        }
        //
        //Destroy Block(s) Store!!
        if (Application.isEditor && !Application.isPlaying)
        {
            GameObject.DestroyImmediate(BlockStore);
        }
        else
        {
            GameObject.Destroy(BlockStore);
        }
        //
        onCreate?.Invoke();
        //
        return true;
    }

    public void SetWorldReadBlock(IsometricBlock Block)
    {
        if (m_root == null)
        {
            Debug.LogFormat("Room not exist for excute command!");
            return;
        }
        //
        Block.WorldManager = m_manager;
        Block.PosPrimary = Block.Pos;

        //World
        int IndexPosH = GetWorldIndexPosH(Block.Pos.HInt);
        if (IndexPosH == -1)
        {
            PosH.Add(new IsometricDataRoomPosH(Block.Pos.HInt));
            IndexPosH = PosH.Count - 1;
            PosH[IndexPosH].Block.Add(Block);
        }
        else
        {
            PosH[IndexPosH].Block.Add(Block);
        }

        //Tag
        foreach (string TagCheck in Block.GetComponent<IsometricBlock>().Tag)
        {
            int TagIndex = GetWorldIndexTag(TagCheck);
            if (TagIndex == -1)
            {
                Tag.Add(new IsometricDataRoomTag(TagCheck));
                IndexPosH = Tag.Count - 1;
                Tag[IndexPosH].Block.Add(Block);
            }
            else
            {
                Tag[TagIndex].Block.Add(Block);
            }
        }

        //Identity
        int IdentityIndex = GetWorldIndexIdentity(Block.Identity);
        if (IdentityIndex == -1)
        {
            Identity.Add(new IsometricDataRoomIdentity(Block.Identity));
            IndexPosH = Identity.Count - 1;
            Identity[IndexPosH].Block.Add(Block);
        }
        else
        {
            Identity[IdentityIndex].Block.Add(Block);
        }

        //Scene
        Transform ParentPosH = m_root.Find(GetWorldNamePosH(Block.Pos));
        if (ParentPosH != null)
        {
            Block.transform.SetParent(ParentPosH, true);
        }
        else
        {
            ParentPosH = QGameObject.SetCreate(GetWorldNamePosH(Block.Pos), m_root).transform;
            Block.transform.SetParent(ParentPosH, true);
        }
    }

    #endregion

    #region World Remove

    public void SetWorldRemove(bool Full = false)
    {
        //PosH
        for (int i = PosH.Count - 1; i >= 0; i--)
        {
            for (int j = PosH[i].Block.Count - 1; j >= 0; j--)
            {
                IsometricBlock Block = PosH[i].Block[j];
                if (Block == null)
                    continue;

                if (Application.isEditor && !Application.isPlaying)
                    GameObject.DestroyImmediate(Block.gameObject);
                else
                    GameObject.Destroy(Block.gameObject);
            }
        }
        PosH.Clear();

        //Tag
        for (int i = Tag.Count - 1; i >= 0; i--)
        {
            for (int j = Tag[i].Block.Count - 1; j >= 0; j--)
            {
                IsometricBlock Block = Tag[i].Block[j];

                if (Block == null)
                    continue;

                if (Application.isEditor && !Application.isPlaying)
                    GameObject.DestroyImmediate(Block.gameObject);
                else
                    GameObject.Destroy(Block.gameObject);
            }
        }
        Tag.Clear();

        //Identity
        for (int i = Identity.Count - 1; i >= 0; i--)
        {
            for (int j = Identity[i].Block.Count - 1; j >= 0; j--)
            {
                IsometricBlock Block = Identity[i].Block[j];

                if (Block == null)
                    continue;

                if (Application.isEditor && !Application.isPlaying)
                    GameObject.DestroyImmediate(Block.gameObject);
                else
                    GameObject.Destroy(Block.gameObject);
            }
        }
        Identity.Clear();

        if (Full)
        {
            //Remove All GameObject!!
            for (int i = m_root.childCount - 1; i >= 0; i--)
            {
#if UNITY_EDITOR
                if (m_root.GetChild(i).gameObject.name == NAME_CURSON)
                    continue;
#endif
                if (m_root.GetChild(i).GetComponent<Camera>() != null)
                    continue;

                if (Application.isEditor && !Application.isPlaying)
                    GameObject.DestroyImmediate(m_root.GetChild(i).gameObject);
                else
                    GameObject.Destroy(m_root.GetChild(i).gameObject);
            }
        }

        onRemove?.Invoke();
    }

    #endregion

    #region World Progess

    //World

    private int GetWorldIndexPosH(int PosH)
    {
        if (m_root == null)
        {
            Debug.LogFormat("Room not exist for excute command!");
            return 0;
        }
        //
        for (int i = 0; i < this.PosH.Count; i++)
        {
            if (this.PosH[i].PosH != PosH)
            {
                continue;
            }

            return i;
        }
        return -1;
    }

    private string GetWorldNamePosH(IsometricVector Pos)
    {
        if (m_root == null)
        {
            Debug.LogFormat("Room not exist for excute command!");
            return "";
        }
        //
        return Pos.HInt.ToString();
    }

    public void SetWorldIndexPosHOrder()
    {
        if (m_root == null)
        {
            Debug.LogFormat("Room not exist for excute command!");
            return;
        }
        //
        PosH = PosH.OrderByDescending(h => h.PosH).ToList();
        for (int i = 0; i < PosH.Count; i++)
            PosH[i] = new IsometricDataRoomPosH(PosH[i].PosH, PosH[i].Block.OrderByDescending(a => a.Pos.X).OrderByDescending(b => b.Pos.Y).ToList());
    }

    //Tag

    private int GetWorldIndexTag(string Tag)
    {
        if (m_root == null)
        {
            Debug.LogFormat("Room not exist for excute command!");
            return 0;
        }
        //
        for (int i = 0; i < this.Tag.Count; i++)
        {
            if (this.Tag[i].Tag != Tag)
                continue;
            return i;
        }
        return -1;
    }

    //Identity

    private int GetWorldIndexIdentity(string Identity)
    {
        if (m_root == null)
        {
            Debug.LogFormat("Room not exist for excute command!");
            return 0;
        }
        //
        for (int i = 0; i < this.Identity.Count; i++)
        {
            if (this.Identity[i].Identity != Identity)
                continue;
            return i;
        }
        return -1;
    }

    #endregion

    #endregion

    #region ======================================================================== Editor

    public bool SetEditorMask(IsometricVector PosCurrent, IsometricVector? PosFocus, Color ColorMask, Color ColorUnMask, Color ColorCentre, Color ColorFocus)
    {
        if (m_root == null)
        {
            Debug.LogFormat("Room not exist for excute command!");
            return false;
        }
        //
        bool CentreFound = false;
        for (int i = 0; i < PosH.Count; i++)
        {
            for (int j = 0; j < PosH[i].Block.Count; j++)
            {
                IsometricBlock Block = PosH[i].Block[j].GetComponent<IsometricBlock>();
                if (Block == null)
                    continue;
                //
                if (PosH[i].Block[j].Pos == PosCurrent)
                {
                    CentreFound = true;
                    Block.SetSpriteColor(ColorCentre, 1f);
                }
                else
                if (PosFocus != null ? PosH[i].Block[j].Pos == PosFocus : false)
                    Block.SetSpriteColor(ColorFocus, 1f);
                else
                if (PosH[i].Block[j].Pos.X == PosCurrent.X || PosH[i].Block[j].Pos.Y == PosCurrent.Y)
                    Block.SetSpriteColor(ColorMask, 1f);
                else
                    Block.SetSpriteColor(ColorUnMask, 1f);
            }
        }
        //
        return CentreFound;
    }

    public void SetEditorHidden(int FromH, float UnMask)
    {
        if (m_root == null)
        {
            Debug.LogFormat("Room not exist for excute command!");
            return;
        }
        //
        for (int i = 0; i < PosH.Count; i++)
        {
            for (int j = 0; j < PosH[i].Block.Count; j++)
            {
                IsometricBlock Block = PosH[i].Block[j].GetComponent<IsometricBlock>();
                if (Block == null)
                    continue;
                //
                if (PosH[i].Block[j].Pos.H > FromH)
                    Block.SetSpriteAlpha(UnMask);
                else
                    Block.SetSpriteAlpha(1f);
            }
        }
    }

    #endregion
}

[Serializable]
public class IsometricDataRoomPosH
{
    public int PosH;
    public List<IsometricBlock> Block;

    public IsometricDataRoomPosH(int PosH)
    {
        this.PosH = PosH;
        Block = new List<IsometricBlock>();
    }

    public IsometricDataRoomPosH(int PosH, List<IsometricBlock> Block)
    {
        this.PosH = PosH;
        this.Block = Block;
    }

    public void SetRefresh()
    {
        Block = Block.Where(x => x != null).ToList();
    }
}

[Serializable]
public class IsometricDataRoomTag
{
    public string Tag;
    public List<IsometricBlock> Block;

    public IsometricDataRoomTag(string Tag)
    {
        this.Tag = Tag;
        Block = new List<IsometricBlock>();
    }

    public IsometricDataRoomTag(string Tag, List<IsometricBlock> Block)
    {
        this.Tag = Tag;
        this.Block = Block;
    }

    public void SetRefresh()
    {
        Block = Block.Where(x => x != null).ToList();
    }
}

[Serializable]
public class IsometricDataRoomIdentity
{
    public string Identity;
    public List<IsometricBlock> Block;

    public IsometricDataRoomIdentity(string Identity)
    {
        this.Identity = Identity;
        Block = new List<IsometricBlock>();
    }

    public IsometricDataRoomIdentity(string Identity, List<IsometricBlock> Block)
    {
        this.Identity = Identity;
        this.Block = Block;
    }

    public void SetRefresh()
    {
        Block = Block.Where(x => x != null).ToList();
    }
}