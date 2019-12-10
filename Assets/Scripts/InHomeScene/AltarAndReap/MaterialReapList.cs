using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDataEditor;

/// <summary>
/// 材料效果
/// </summary>
public class MaterialReapList : MonoBehaviour
{
    public List<GDEEquipmentData> ReapNormalEquip(EquipPosition pos,int type, int Num = 1)
    {
        List<Dictionary<string, string>> armors = SDDataManager.Instance.ReadFromCSV("equip");
        List<Dictionary<string, string>> weaps = SDDataManager.Instance.ReadFromCSV("weapon");
        List<Dictionary<string, string>> jewes = SDDataManager.Instance.ReadFromCSV("jewelry");
        List<Dictionary<string, string>> all = new List<Dictionary<string, string>>();

        if (pos == EquipPosition.End)
        {
            for(int i = 0; i < armors.Count; i++)
            {
                all.Add(armors[i]);
            }
            for(int i = 0; i < weaps.Count; i++)
            {
                all.Add(weaps[i]);
            }
            for(int i = 0; i < jewes.Count; i++)
            {
                all.Add(jewes[i]);
            }


        }
        else if(pos != EquipPosition.Hand && pos != EquipPosition.Finger)
        {
            for (int i = 0; i < armors.Count; i++)
            {
                if(SDDataManager.Instance.getInteger(armors[i]["pos"]) 
                    == (int)pos)
                {
                    all.Add(armors[i]);
                }
            }

        }
        else if(pos == EquipPosition.Hand)
        {
            for (int i = 0; i < weaps.Count; i++)
            {
                all.Add(weaps[i]);
            }

        }
        else if(pos == EquipPosition.Finger)
        {
            for (int i = 0; i < jewes.Count; i++)
            {
                all.Add(jewes[i]);
            }

        }

        if (type >= 0)
        {
            for (int i = 0; i < all.Count; i++)
            {
                int _type = SDDataManager.Instance.getInteger
                    (all[i]["type"]);
                if(_type != type)
                {
                    all.RemoveAt(i);
                }
            }
        }
        List<GDEEquipmentData> result = new List<GDEEquipmentData>();
        for(int i = 0; i < Num; i++)
        {
            int randomNum = UnityEngine.Random.Range(0, all.Count);
            string equipId = (all[randomNum]["id"]);
            GDEEquipmentData E = new GDEEquipmentData(GDEItemKeys.Equipment_EquipEmpty)
            {
                id = equipId,
                locked = false,
                equipType = 0,
                index = 0,
                num = 1,
                OwnerHashcode = 0,
                exp = 0,
                quality = UnityEngine.Random.Range(0, SDConstants.equipMaxQuality),
                initialQuality = UnityEngine.Random.Range(0, 1),
            };
            result.Add(E);
        }

        return result;
    }


}
