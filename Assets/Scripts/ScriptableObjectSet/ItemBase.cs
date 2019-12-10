using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ItemBase :ScriptableObject
{
    [SerializeField]
    private string _ID;
    public string ID { get { return _ID; } }

    [SerializeField]
    private string _NAME;
    public string NAME { get { return _NAME; } }

    [SerializeField,TextArea]
    private string _DESC;
    public string DESC { get { return _DESC; } }

    [SerializeField,ReadOnly]
    private SDConstants.ItemType itemtype;
    public SDConstants.ItemType ItemType
    {
        get { return itemtype; }
        protected set { itemtype = value; }
    }

    [SerializeField]
    private int _LEVEL;
    public int LEVEL { get { return _LEVEL; } }

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
    private bool SellAble
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
    private int maxDurability=100;//耐久度
    public int MaxDurability
    {
        get { return maxDurability; }
    }


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