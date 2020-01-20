using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.U2D;

public class ItemBase : ScriptableObject
{
    #region 基本信息
    [SerializeField]
    private string _ID;
    public string ID { get { return _ID; } protected set { _ID = value; } }

    [SerializeField]
    private string _NAME;
    public string NAME { get { return _NAME; } protected set { _NAME = value; } }

    [SerializeField, TextArea]
    private string _DESC;
    public string DESC { get { return _DESC; } protected set { _DESC = value; } }

    [SerializeField]
    private SpriteAtlas _atlasFrom;
    public SpriteAtlas AtlasFrom
    {
        get { return _atlasFrom; }
        set { _atlasFrom = value; }
    }
    public Sprite IconFromAtlas
    {
        get
        {
            if (AtlasFrom == null) return null;
            else return AtlasFrom.GetSprite(ID);
        }
    }

#if UNITY_EDITOR
    [SerializeField, ReadOnly]
#endif
    private SDConstants.ItemType itemtype;
    public SDConstants.ItemType ItemType
    {
        get { return itemtype; }
        protected set { itemtype = value; }
    }

    [SerializeField]
    private int _LEVEL;
    public int LEVEL { get { return _LEVEL; } protected set { _LEVEL = value; } }

    public interface IUsable { void OnUse(); }

    [SerializeField]
    protected bool stackAble = true;//是否可堆叠
    public virtual bool StackAble
    {
        get { return stackAble; }
    }


    [SerializeField]
    protected bool discardAble = true;//是否可丢弃
    public virtual bool DiscardAble
    {
        get { return discardAble; }
    }

    [SerializeField]
    protected bool useable = true;//是否可用
    public bool UseAble
    {
        get { return useable; }
    }

    [SerializeField]
    protected bool sellAble;//是否可出售
    public bool SellAble
    {
        get { return sellAble; }
    }

    [SerializeField]
    protected bool inexhaustible;//用之不竭
    public virtual bool Inexhaustible
    {
        get { return inexhaustible; }
    }

    [SerializeField]
    private int maxDurability = 100;//耐久度
    public int MaxDurability
    {
        get { return maxDurability; }protected set { maxDurability = value; }
    }

    [SerializeField]
    private string specialStr;
    public string SpecialStr
    { get { return specialStr; } protected set { specialStr = value; } }
    public virtual void initData(string id,string name,string desc,int level
        ,bool stackable,bool discardable
        ,bool useable, bool sellable, bool inexhaustible,string specialStr
        , SDConstants.ItemType itemtype)
    {
        this.ID = id;this.NAME = name;this.DESC = desc;this.ItemType = itemtype;this.LEVEL = level;
        this.stackAble = stackable;this.discardAble = discardable;this.useable = useable;this.sellAble = sellable;
        this.SpecialStr = specialStr;
        this.inexhaustible = inexhaustible;
    }

    #endregion


    # region 身份验证
    public bool isConsumable
    {
        get { return this is consumableItem; }
    }
    public bool isRune
    {
        get { return this is RuneItem; }
    }
    public bool isEquip
    {
        get { return this is EquipItem; }
    }
    #endregion
}

#region 道具信息相关
[Serializable]
public class ItemInfo // 在此进行拓展，如强化、词缀、附魔
{
    [SerializeField]
    public ItemBase item;
    [SerializeField]
    private int amount;
    public int Amount { 
        get { return amount; }
        set 
        { 
            if (value < 0) amount = 0;
            else amount = value;
        }
    }

    [HideInInspector]
    public int indexInGrid;

    public string ItemID
    {
        get
        {
            if (item) return item.ID;
            else return string.Empty;
        }
    }
    public string ItemName
    {
        get
        {
            if (item) return item.NAME;
            else return string.Empty;
        }
    }

    public ItemInfo()
    {
        amount = 1;
    }
    public ItemInfo(ItemBase item,int amount = 1)
    {
        this.item = item;
        this.amount = amount;
    }
    public ItemInfo Cloned
    {
        get { return MemberwiseClone() as ItemInfo; }
    }

    public static implicit operator bool (ItemInfo self)
    {
        return self != null;
    }
}
#endregion