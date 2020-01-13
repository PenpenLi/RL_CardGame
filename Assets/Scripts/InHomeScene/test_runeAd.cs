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
        List<consumableItem> all = SDDataManager.Instance.AllConsumableList;
        for (int i = 0; i < all.Count; i++)
        {
            SDDataManager.Instance.addConsumable(all[i].ID, 5);
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
        List<EquipItem> all = SDDataManager.Instance.AllEquipList;
        for(int i = 0; i < all.Count; i++)
        {
            SDDataManager.Instance.addEquip(all[i].ID);
        }
        List<GDEEquipmentData> gdes = SDDataManager.Instance.getAllOwnedEquips();
        int[] posLists = new int[(int)EquipPosition.End];
        for(int i = 0; i < gdes.Count; i++)
        {
            string id = gdes[i].id;
            EquipItem item = SDDataManager.Instance.GetEquipItemById(id);
            EquipPosition pos = item.EquipPos;
            posLists[(int)pos]++;
        }
        string S = "";
        for(int i = 0; i < posLists.Length; i++)
        {
            S += ((EquipPosition)i).ToString().ToUpper() + ": " + posLists[i] + "/ ";
        }
        Debug.Log(S);
    }
}
