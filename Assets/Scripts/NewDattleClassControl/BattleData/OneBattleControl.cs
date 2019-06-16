using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneBattleControl : MonoBehaviour
{
    public OneRoleClassData[] AllSelfRoleList;
    public float[] SRL_SV;
    public OneRoleClassData[] AllOtherRoleList;
    public float[] ORL_SV;
    public float MinBTime;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }


    #region 构建行动条
    public int MaxSpeed;
    public void BuildBehaveValueBase()
    {
        SRL_SV = new float[AllSelfRoleList.Length];
        ORL_SV = new float[AllOtherRoleList.Length];
        MaxSpeed = Mathf.Max(PerListMaxSpeed(AllSelfRoleList), PerListMaxSpeed(AllOtherRoleList));
    }
    public int PerListMaxSpeed(OneRoleClassData[] list)
    {
        int ms = 0;
        for (int i = 0; i < list.Length; i++)
        {
           ms = Mathf.Max(ms, list[i].ThisRoleAttributes.Speed);
        }
        return ms;
    }
    public float PerRoleBehaveValue(OneRoleClassData PerRole,float BV)
    {
        BV += (PerRole.ThisRoleAttributes.Speed *1f)/ MaxSpeed * Time.deltaTime;
        return BV;
    }

    //
    [HideInInspector]
    public OneRoleClassData CurrentRole;
    public int BattleBVPause;
    public bool CheckPerListBehaveValue(OneRoleClassData[] List,float[] SV)
    {
        bool t = false;
        for(int i = 0; i < List.Length; i++)
        {
            if (i < SV.Length)
            {
                //SV[i] = PerRoleBehaveValue(List[i], SV[i]);
                if (SV[i] >= MinBTime)
                {
                    SV[i] = 0;
                    t = true;
                    CurrentRole = List[i];
                }
            }
        }
        return t;
    }

    //Update
    public void BehaveValueDoing()
    {
        if (BattleBVPause == 0)
        {
            if (CheckPerListBehaveValue(AllSelfRoleList, SRL_SV))
            {
                BattleBVPause = 1;//玩家队伍有人开始行动
            }
            else if (CheckPerListBehaveValue(AllOtherRoleList, ORL_SV))
            {
                BattleBVPause = -1;//敌方队伍有人开始行动
            }
            else
            {
                for (int i = 0; i < AllOtherRoleList.Length; i++)
                {
                    if (i < ORL_SV.Length) { ORL_SV[i] = PerRoleBehaveValue(AllOtherRoleList[i], ORL_SV[i]); }
                }
                //
                for (int i = 0; i < AllSelfRoleList.Length; i++)
                {
                    if (i < SRL_SV.Length) { SRL_SV[i] = PerRoleBehaveValue(AllSelfRoleList[i], SRL_SV[i]); }
                }
            }
        }
        else
        {
            if(BattleBVPause == 1)//我方行动
            {

            }
            else if(BattleBVPause == -1)//敌方行动
            {
                
            }
        }       
    }
    #endregion
}
