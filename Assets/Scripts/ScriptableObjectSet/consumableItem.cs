using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Consumable",menuName ="Wun/道具/新材料")]
public class consumableItem : ItemBase,ItemBase.IUsable
{
    [SerializeField]
#if UNITY_EDITOR
    [EnumMemberNames("英雄经验","英雄星级","英雄技能","好感度","金币兑换物"
        ,"装备经验","装备校准","装备获取箱","药剂","戏法","暗器","女神经验"
        ,"女神碎片","解锁器","宝物","原料","其他","end")]
#endif
    private SDConstants.MaterialType materialType;
    public SDConstants.MaterialType MaterialType
    {
        get { return materialType; }
        set { materialType = value; }
    }

    [SerializeField]
    private SDConstants.ConsumableType consumableType;
    public SDConstants.ConsumableType ConsumableType
    {
        get { return consumableType; }
        set { consumableType = value; }
    }

    [SerializeField]
    private string data;
    public string Data { get { return data; } }
    public bool isProp
    {
        get 
        {
            return ConsumableType == SDConstants.ConsumableType.prop;
        }
    }
    [Space]
#if UNITY_EDITOR
    [ConditionalHide("ConsumableType", (int)SDConstants.ConsumableType.prop, true)]
#endif
    public SDConstants.AOEType AOE;
#if UNITY_EDITOR
    [ConditionalHide("ConsumableType", (int)SDConstants.ConsumableType.prop, true)]
#endif
    public string AIM;


    public consumableItem()
    {
        ItemType = SDConstants.ItemType.Consumable;
    }

    public void OnUse()
    {
        Debug.Log("UseMaterial " + NAME);
    }



    //
    [Space]
    public int buyPrice_coin;
    public int buyPrice_diamond;
    public int minLv;
    public int maxLv;
    public int exchangeType;
    public int weight;
}