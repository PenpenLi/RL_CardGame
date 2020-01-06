using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDataEditor;
using System.Linq;

/// <summary>
/// 材料效果
/// </summary>
public class MaterialResolveList : MonoBehaviour
{


    #region equip_reap
    public bool Use__equip_reap(string equipReapItem_id ,out string equipId)
    {
        equipId = string.Empty;
        if (SDDataManager.Instance.getConsumableNum(equipReapItem_id) <= 0) return false;
        bool flag = SDDataManager.Instance.checkIfHaveOpKey(SDConstants.MaterialType.equip_reap
            ,out string keyId);
        if (!flag) return false;
        consumableItem item = SDDataManager.Instance.getConsumableById(equipReapItem_id);
        if (!item) return false;
        string str = item.SpecialStr;
        string[] strings = str.Split('_');
        EquipPosition pos = ROHelp.EQUIP_POS(strings[0]);
        int rank = GetArmorRank(strings[1]);
        equipId = ReapNormalEquip(pos, rank);
        SDDataManager.Instance.consumeConsumable(keyId,out int r);
        return true;
    }
    int GetArmorRank(string s)
    {
        if (s == "1") return 1;
        else if (s == "2") return 2;
        else if (s == "3") return 3;
        else if (s == "4") return 4;
        else if (s == "0") return 0;
        else return -1;
    }
    /// <summary>
    /// 被封印类道具生效后结果
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="type"></param>
    /// <param name="Num"></param>
    /// <returns></returns>
    public string ReapNormalEquip(EquipPosition pos,int type)
    {
        List<EquipItem> equips = SDDataManager.Instance.AllEquipList;
        List<EquipItem> all = equips.FindAll(x => x.EquipPos == pos);
        if (pos == EquipPosition.End) all = equips;

        if(type>0)
            all = all.FindAll(x => x.ArmorRank.Index == type);

        float[] r = new float[] { 6, 3.5f, 0.46f, 0.04f };
        int level = RandomIntger.Choose(r);
        all = all.FindAll(x => x.LEVEL == level);
        int randomNum = UnityEngine.Random.Range(0, all.Count);
        EquipItem result = all[randomNum];
        return result.ID;
    }
    #endregion

}
