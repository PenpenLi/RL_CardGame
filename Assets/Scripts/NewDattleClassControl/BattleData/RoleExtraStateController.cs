using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoleExtraStateController : MonoBehaviour
{
    public BattleRoleData unit 
    {
        get { return GetComponent<BattleRoleData>(); }
    }
    public enum ExtraStateTag 
    {
        intervene,
        reflect,

    }
    //[Header("intervene_state")]
    public ExtraStateData interveneState;
    public ExtraStateData reflectState;
    public void checkExtraStates()
    {
        //援护状态设置
        if (interveneState.LastTime > 0)
        {
            interveneState.LastTime--;
            if (interveneState.stateUnit != null)
            {
                interveneState.stateUnit.gameObject.SetActive(true);
                if (interveneState.isBuff) { interveneState.stateUnit.sprite = interveneState.buffImg; }
                else { interveneState.stateUnit.sprite = interveneState.debuffImg; }
            }
            unit.ThisBasicRoleProperty().DmgReduction = interveneState.changeData;
        }
        else 
        {
            interveneState.stateUnit?.gameObject.SetActive(false);
            unit.ThisBasicRoleProperty().DmgReduction = 0;
            interveneState.changeData = 0;
        }

        //反击状态设置
        if(reflectState.LastTime > 0)
        {
            reflectState.LastTime--;
            if (reflectState.stateUnit != null)
            {
                reflectState.stateUnit.gameObject.SetActive(true);
                if (reflectState.isBuff) { reflectState.stateUnit.sprite = reflectState.buffImg; }
                else { reflectState.stateUnit.sprite = reflectState.debuffImg; }
            }
            unit.ThisBasicRoleProperty().DmgReflection = reflectState.changeData;
        }
        else
        {
            reflectState.stateUnit?.gameObject.SetActive(false);
            unit.ThisBasicRoleProperty().DmgReflection = 0;
            reflectState.changeData = 0;
        }
    }
    public void initInterveneState(int changeData, int lasttime, bool isBuff = true)
    {
        interveneState.changeData = changeData;
        interveneState.LastTime = lasttime;
        if (isBuff)
        {
            if (changeData < 0) { interveneState.isBuff = false; }
            else { interveneState.isBuff = true; }
        }
        else { interveneState.isBuff = false; }
    }
    public void initReflectState(int changeData, int lasttime, bool isBuff = true)
    {
        reflectState.changeData = changeData;
        reflectState.LastTime = lasttime;
        if (isBuff)
        {
            if (changeData < 0) { reflectState.isBuff = false; }
            else { reflectState.isBuff = true; }
        }
        else { reflectState.isBuff = false; }
    }
}


[System.Serializable]
public class ExtraStateData
{
    public Image stateUnit;
    public int LastTime;
    public bool isBuff;
    //public bool keepChanging = false;
    public Sprite buffImg;
    public Sprite debuffImg;
    public int changeData;
}