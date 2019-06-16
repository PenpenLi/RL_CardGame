using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class NewItemManager:MonoBehaviour
{
    ItemDataEntry _data = new ItemDataEntry();
    ItemDataEntry data
    {
        get
        {
            return _data;
        }
        set
        {
            _data = value;
        }
    }
    private static NewItemManager _instance;
    public static NewItemManager Instance
    {
        get
        {
            if (_instance == null) _instance = new NewItemManager();
            return _instance;
        }
    }
    [HideInInspector]
    public List<ItemData> ItemList;

    private void Start()
    {
        if (LoadFromFile() != null)
        {
            data = LoadFromFile();
        }
    }
    ItemDataEntry LoadFromFile()
    {
        TextAsset ta = Resources.Load<TextAsset>("Jsons/AllItems");
        if (ta == null)
        {
            return null;
        }
        if (ta.text.Length > 0)
        {
            return JsonUtility.FromJson<ItemDataEntry>(ta.text);
        }
        return null;
    }

    public void ReadData()
    {
        if(ItemList == null)
        {
            ItemList = new List<ItemData>();
        }

        if (LoadFromFile() != null)
        {
            data = LoadFromFile();
        }

        for (int i = 0; i < data.ItemDatas.Length; i++)
        {
            int ItemID = data.ItemDatas[i].item_ID;
            string ItemName = data.ItemDatas[i].item_name;
            string ItemDesc = data.ItemDatas[i].item_description;
            int type = (int)data.ItemDatas[i].item_type;
            int kind = data.ItemDatas[i].item_kind;
            string icon = data.ItemDatas[i].item_icon;
            string bgicon = data.ItemDatas[i].item_bgicon;
            int count = data.ItemDatas[i].item_count;
            int qual = data.ItemDatas[i].item_quality;
            int opera = data.ItemDatas[i].item_operation;
            ItemData a = new ItemData(ItemID,ItemName,ItemDesc,type,kind,icon,bgicon,count,qual,opera);
            ItemList.Add(a);
            Debug.Log(ItemID + " " + ItemName + " " + ItemDesc + " " + type + " " + kind + " " 
                + icon + " " + bgicon + " " + count + " " + qual + " " + opera);
        }
    }
    /*
    [ContextMenu("TestLoadConfig")]
    public void TestLeadConfig()
    {
        NewItemManager.Instance.ReadData();
    }
    */
}