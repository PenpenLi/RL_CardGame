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
    }


    public int stateCondition = 0;

    public int LastTime = 0;
    /// <summary>
    /// 三项修改值
    /// </summary>
    public StateEffectStorage Storage = new StateEffectStorage();
    /// <summary>
    /// 伤害值
    /// </summary>
    public int ExtraDmg;
    public void Clear(bool ResetCondition = false)
    {
        Storage.ResetSelf();
        ExtraDmg = 0;
        if (ResetCondition) stateCondition = 0;
    }
}