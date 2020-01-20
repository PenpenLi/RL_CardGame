using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDataEditor;
public class GoddessManager : MonoBehaviour
{
    public bool haveGoddess;
#if UNITY_EDITOR
    [Range(0, 1), ReadOnly, SerializeField]
#endif
    private float charge;
    public float Charge
    {
        get { return charge; }
        set
        {
            charge = value;
        }
    }
    public GoddessProperty GP;
    [HideInInspector]
    public BattleManager BM;
    public bool GoddessFunctionCanUse;
    public void initCurrentGoddess(string id)
    {
        bool k = GP.InitGoddess(id);
        haveGoddess = k;
    }
    void whenCharging()
    {

    }
    public bool goddessCharging()
    {
        if (Charge < 1)
        {
            Charge += GP.chargeSpeed * Time.deltaTime;
            whenCharging();
            GoddessFunctionCanUse = false;
        }
        else
        {
            Charge = 1;
            GoddessSpreadMp();
            GoddessFunctionCanUse = true;
        }
        return GoddessFunctionCanUse;
    }
    public void GoddessSpreadMp()
    {
        for(int i = 0; i < BM.Remaining_SRL.Count; i++)
        {
            BattleRoleData unit = BM.Remaining_SRL[i];
            int val = GP.mpRegendNum;
            unit.MpController.addMp(val);
        }
    }
    public RoleAttributeList GetRALUpByGoddess(RoleAttributeList baseRAL)
    {
        RoleAttributeList ral = RoleAttributeList.zero;
        float rate =AllRandomSetClass.SimplePercentToDecimal(GP.heroAttiUp);
        ral.AT = (int)(baseRAL.AT * rate);
        ral.AD = (int)(baseRAL.AD * rate);
        ral.MT = (int)(baseRAL.MT * rate);
        ral.MD = (int)(baseRAL.MD * rate);
        return ral;
    }


    //
    public void GoddessBtnClicked()
    {
        if (haveGoddess)
        {
            if (GP.GSkill != null) GP.GSkill.StartSkill();
        }
    }
}
