using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
