using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDataEditor;
using System.Linq;

public class test_runeAd : MonoBehaviour
{
    public int HC;
    [Space]
    public List<RuneItem> Alls;
    private void Start()
    {
        Alls = SDDataManager.Instance.AllRuneList;
    }

    [ContextMenu("AddOneRune")]
    public void AddRune()
    {
        SDDataManager.Instance.AddRune(Alls[HC].ID);
    }

    [ContextMenu("AddAllRune")]
    public void AddRuneAll()
    {
        foreach(RuneItem i in Alls)
        {
            SDDataManager.Instance.AddRune(i.ID);
        }
    }

    [ContextMenu("Add_All_Consumable_5")]
    public void Test_AddConsumableItems()
    {
        List<GDEItemData> alls = SDDataManager.Instance.PlayerData.consumables;
        if (alls.Count < 10)
        {
            List<consumableItem> all = SDDataManager.Instance.AllConsumableList;
            all = all.FindAll(x => x.IconFromAtlas != null);
            List<int> enables = RandomIntger.NumListReturn(5, all.Count);
            string s = "";
            for (int i = 0; i < enables.Count; i++)
            {
                SDDataManager.Instance.addConsumable(all[enables[i]].ID, 5);
                s += all[enables[i]].name + "| ";
            }
            Debug.Log(s);
        }
    }

    [ContextMenu("Show_Hero_Skills")]
    public void ReadHeroSkills()
    {
        GDEHeroData D = SDDataManager.Instance.PlayerData.herosOwned[0];
        int hashcode = D.hashCode;
        Debug.Log("HC: " + hashcode);
        GDEHeroData h = SDDataManager.Instance.getHeroByHashcode(hashcode);
        List<string> w = h.skillsOwned.Select(x =>
        {
            return x.Id + "__" + x.Lv;
        }).ToList();
        string W = "";
        foreach (string ww in w)
        {
            W += ww + "/";
        }
        Debug.Log("FisrtHeroSkills: " + W);
    }

    [ContextMenu("AddAllEquips")]
    public void AddEquips()
    {
        List<GDEEquipmentData> _gdes = SDDataManager.Instance.getAllOwnedEquips();
        if (_gdes.Count < 4)
        {
            List<EquipItem> all = SDDataManager.Instance.AllEquipList;
            all = all.FindAll(x => x.IconFromAtlas != null
            && x.WeaponRace.WeaponClass == SDConstants.WeaponClass.Claymore);
            List<int> enables = RandomIntger.NumListReturn(5, all.Count);
            for (int i = 0; i < all.Count; i++)
            {
                if (enables.Contains(i))
                    SDDataManager.Instance.addEquip(all[i].ID);
            }
            List<EquipItem> allcs = all.FindAll(x =>
            x.WeaponRace.WeaponClass == SDConstants.WeaponClass.Claymore);
            EquipItem _item = allcs[Random.Range(0, allcs.Count)];
            SDDataManager.Instance.addEquip(_item.ID);

            List<GDEEquipmentData> gdes = SDDataManager.Instance.getAllOwnedEquips();
            int[] posLists = new int[(int)EquipPosition.End];
            for (int i = 0; i < gdes.Count; i++)
            {
                string id = gdes[i].id;
                EquipItem item = SDDataManager.Instance.GetEquipItemById(id);
                EquipPosition pos = item.EquipPos;
                posLists[(int)pos]++;
            }
            string S = "";
            for (int i = 0; i < posLists.Length; i++)
            {
                S += ((EquipPosition)i).ToString().ToUpper() + ": " + posLists[i] + "/ ";
            }
            Debug.Log(S);
        }
    }


    [ContextMenu("Add_All_Heros_Fatigue")]
    public void AddAllHerosFatigue()
    {
        List<int> AllHcs = SDDataManager.Instance.PlayerData.herosOwned
            .Select(x => x.hashCode).ToList();
        foreach(int hc in AllHcs)
        {
            SDDataManager.Instance.addHeroFatigue(50, hc);
        }
    }


    [ContextMenu("Add_All_Exp_Consumables_5")]
    public void AddExpConsumables()
    {
        List<consumableItem> all = SDDataManager.Instance.AllConsumableList.FindAll
            (x => x.MaterialType == SDConstants.MaterialType.exp);
        foreach(var item in all)
        {
            SDDataManager.Instance.addConsumable(item.ID, 5);
        }
    }
}
