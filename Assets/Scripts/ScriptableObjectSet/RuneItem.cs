using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Rune",menuName ="Wun/道具/新如尼道具")]
public class RuneItem : ItemBase,ItemBase.IUsable
{
    [SerializeField,Range(0,2)]
    private int quality;
    public int Quality
    {
        get { return quality; }
    }

    [SerializeField]
    [EnumMemberNames("敏捷","耐力","恢复","统帅","无倾向")]
    private GoddessAttiType _AttiType;
    public GoddessAttiType AttiType
    {
        get { return _AttiType; }
    }

    [SerializeField]
    private GoddessAttritube _Atti;
    public GoddessAttritube Atti { get { return _Atti; } }

    public RuneItem()
    {
        ItemType = SDConstants.ItemType.Rune;
    }
    public void OnUse()
    {
        Debug.Log("UseRune");
    }
}
