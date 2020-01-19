using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
//using MoreMountains.Tools;


public class QuicklySelectSkillTarget : MonoBehaviour
{
    public BattleManager BM;
    //public int actStep;
    bool clickUnable;
    public float MinClickInterval = 0.15f;
    public Transform RoleDetailPanel;
    public int WhenPointDownIndex()
    {
        if (!BM) BM = GetComponentInParent<BattleManager>();
        Ray targetChooseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector2 origin = new Vector2(targetChooseRay.origin.x, targetChooseRay.origin.y);
        int index = 0;
        for (int i = 0; i < BM.All_Array.Count; i++)
        {
            float dis0 = Vector2.Distance(BM.All_Array[index].transform.position, origin);
            float dis1 = Vector2.Distance(BM.All_Array[i].transform.position, origin);
            if (dis0 > dis1)
            {
                index = i;
            }
            BM.All_Array[i].unit_model
                .GetComponentInChildren<SpriteRenderer>().color = Color.white;
        }
        return index;
    }
    public void ConfirmTarget(int index)
    {
        if (!BM) BM = GetComponentInParent<BattleManager>();

        BM.All_Array[index].unit_model
            .GetComponentInChildren<SpriteRenderer>().color = Color.blue;
    }
    public void ResetTarget()
    {
        if (!BM) BM = GetComponentInParent<BattleManager>();

        for (int i = 0; i < BM.All_Array.Count; i++)
        {
            //BM.All_Array[i].unit_character_model
                //.GetComponentInChildren<SpriteRenderer>().color = Color.white;
        }
    }


    public void OnClick()
    {
        if (!BM) BM = GetComponentInParent<BattleManager>();
        if (!clickUnable)
        {
            Debug.Log("Select Target Clicking");
            clickUnable = true;
            BM.ConfirmSkillAndTarget();
            StartCoroutine(IEBtnRecover());
        }
        else
        {
            //显示角色详细信息

        }

    }
    public IEnumerator IEBtnRecover()
    {
        yield return new WaitForSeconds(MinClickInterval);
        clickUnable = false;
    }
}
