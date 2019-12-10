using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BRD_OneStateController 
{
    private StateTag _statetag;
    public StateTag stateTag
    {
        get { return _statetag; }
        set { _statetag = value; }
    }


    public int stateCondition = 0;

    public int LastTime = 0;
    public RoleAttributeList UniqueEffectInRA = new RoleAttributeList();
    //public RoleBarChart BCArray = new RoleBarChart();
    /// <summary>
    /// 伤害值
    /// </summary>
    public int ExtraDmg;
    public void Clear(bool ResetCondition = false)
    {
        UniqueEffectInRA = RoleAttributeList.zero;
        //BCArray.ResetSelf();
        ExtraDmg = 0;
        if (ResetCondition) stateCondition = 0;
    }
}