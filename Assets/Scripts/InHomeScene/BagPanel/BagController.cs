using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

public class BagController : MonoBehaviour
{
    //public Transform BagSlot;
    public ScrollRect scrollRect;
    public List<OneBagSlot> allSlots;
    public HEWPageController page;
    public int currentSlotIndex;
    public enum useType
    {
        change, use, end,
    }
    useType _cut = useType.change;
    public useType currentUseType
    {
        get { return _cut; }
        set
        {
            if (_cut != value)
            {
                _cut = value;

            }
        }
    }
    [Header("CurrentProp")]
    public string specialStr;
    public string target;
    public string range;
    public BattleManager BM;


    public void InitBag(useType use_type)
    {
        currentUseType = use_type;
        List<GDEItemData> list = SDDataManager.Instance.PlayerData.propsTeam;
        if (list.Count < SDConstants.BagStartVolime)//初始化
        {
            SDDataManager.Instance.PlayerData.propsTeam.Clear();
            SDDataManager.Instance.PlayerData.Set_propsTeam();
            GDEItemData a = new GDEItemData(GDEItemKeys.Item_MaterialEmpty)
            {
                id = string.Empty,
                num = 0,
                index=0,
            };
            SDDataManager.Instance.PlayerData.propsTeam.Add(a);
            SDDataManager.Instance.PlayerData.Set_propsTeam();
            GDEItemData a1 = new GDEItemData(GDEItemKeys.Item_MaterialEmpty)
            {
                id = string.Empty,
                num = 0,
                index = 1,
            };
            SDDataManager.Instance.PlayerData.propsTeam.Add(a1);
            SDDataManager.Instance.PlayerData.Set_propsTeam();
            list = SDDataManager.Instance.PlayerData.propsTeam;
        }
        //
        allSlots.Clear();
        for (int i = 0; i < scrollRect.content.childCount; i++)
        {
            OneBagSlot bs = scrollRect.content.GetChild(i).GetComponent<OneBagSlot>();
            if (i < list.Count)//已解锁位置
            {
                bs.initSlot(currentUseType, list[i]);
            }
            else
            {
                bs.initSlot(useType.end
                    , new GDEItemData(GDEItemKeys.Item_MaterialEmpty)
                    {
                        id = string.Empty,
                        num = 0
                    });
            }
            bs.index = i;
            allSlots.Add(bs);
        }
    }

    public void item_change(OneBagSlot slot)
    {
        if(slot.currentUseType == useType.end)//该槽位未解锁
        {
            return;
        }
        initPropList();
        currentSlotIndex = slot.index;
    }
    public void initPropList()
    {
        SDGameManager.Instance.stockUseType = SDConstants.StockUseType.work;
        page.ItemsInit(SDConstants.ItemType.Prop);
        foreach (SingleItem s in page.items)
        {
            s.bagController = this;
        }
    }
    public void selectPropToChangeCurrentSlot(string id)
    {
        string oldPropId = string.Empty;
        for(int i = 0; i < SDDataManager.Instance.PlayerData.propsTeam.Count;i++)
        {
            GDEItemData P = SDDataManager.Instance.PlayerData.propsTeam[i];
            if (P.index == currentSlotIndex)
            {
                if(P.id == id)
                {
                    return;
                }
                oldPropId = P.id;
                P.id = id;
                int volume = SDDataManager.Instance.propTakenVolume(id);
                int allNum = SDDataManager.Instance.getPropOwned(id).num;
                if (allNum >= volume) P.num = volume;
                else P.num = allNum;
                SDDataManager.Instance.PlayerData.propsTeam[i] = P;
                SDDataManager.Instance.PlayerData.Set_propsTeam();
            }
        }
        if (string.IsNullOrEmpty(oldPropId))
        {
            for (int i = 0; i < SDDataManager.Instance.PlayerData.propsTeam.Count; i++)
            {
                GDEItemData P = SDDataManager.Instance.PlayerData.propsTeam[i];
                if (P.index != currentSlotIndex)
                {
                    if (P.id == id)//旧位置
                    {
                        P.id = oldPropId;
                        int volume = SDDataManager.Instance.propTakenVolume(oldPropId);
                        int allNum = SDDataManager.Instance.getPropOwned(oldPropId).num;
                        if (allNum >= volume) P.num = volume;
                        else P.num = allNum;
                        SDDataManager.Instance.PlayerData.propsTeam[i] = P;
                        SDDataManager.Instance.PlayerData.Set_propsTeam();
                    }
                }
            }
        }
        initPropList();
        InitBag(currentUseType);
    }


    public void item_use(OneBagSlot slot)
    {
        GDEItemData P = SDDataManager.Instance.PlayerData.propsTeam[currentSlotIndex];
        if (P!=null && string.IsNullOrEmpty(P.id))
        {
            ROPropData Prop = SDDataManager.Instance.getPropDataById(P.id);
            specialStr = Prop.specialStr;
            target = Prop.target;
            SDGameManager.Instance.isUsingProp = true;
            parseSpecialStr();
        }
    }

    public void parseSpecialStr()
    {
        string[] strings = specialStr.Split('|');
        //
        for(int i = 0; i < strings.Length; i++)
        {
            string[] tmp = strings[i].Split(':');
            BM.PropFunictionName = tmp[0];
            string[] paramsStr = tmp[1].Split(',');
            BM.param0 = SDDataManager.Instance.getInteger(paramsStr[0]);
            if (paramsStr.Length > 1) BM.param1 = SDDataManager.Instance.getInteger(paramsStr[1]);
        }
        

    }
}
