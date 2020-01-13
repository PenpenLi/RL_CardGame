using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BattleStateVisionController : MonoBehaviour
{
    public List<Transform> Unit_All_List;
    public List<Transform> Unit_ST_State_List;
    public List<Transform> Unit_Standard_State_List;
    public Transform Unit_Die_State;
    [ContextMenu("BUILD")]
    public void BUildList()
    {
        Unit_All_List = transform.GetComponentsInChildren<Transform>().ToList()
            .FindAll(x=>x!=this.transform);
        Unit_ST_State_List = Unit_All_List.FindAll(x => x.GetSiblingIndex() < (int)StateTag.End);
        Unit_Standard_State_List = Unit_All_List.FindAll(x => x.GetSiblingIndex() >= (int)StateTag.End);
    }
}
