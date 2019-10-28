using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

public class BattleGroupStateController : MonoBehaviour
{
    BattleManager _bm;
    public BattleManager BM
    { get { if (_bm == null) _bm = GetComponent<BattleManager>();
            return _bm;
        } }
    public int currentGoddessId;
    //
    #region groupState
    [HideInInspector]
    public RoleBarChart selfBarChartEffect;
    [HideInInspector]
    public RoleAttributeList selfRALChange;
    public Image selfGroupStateImg;
    [HideInInspector]
    public RoleBarChart otherBarChartEffect;
    [HideInInspector]
    public RoleAttributeList otherRALChange;
    public Image otherGroupStateImg;
    #endregion
    public void checkAllGroupState()
    {
        //对玩家队伍效果
        for(int i =0;i < BM.Remaining_SRL.Count; i++)
        {
            BM.Remaining_SRL[i].getBCOA(selfBarChartEffect);
            if (selfRALChange.HaveData)
            {
                BM.Remaining_SRL[i].ThisBasicRoleProperty()._role.groupStateRAL = selfRALChange;
            }
            else
            {
                BM.Remaining_SRL[i].ThisBasicRoleProperty()._role.groupStateRAL = RoleAttributeList.zero;
            }
        }

        //对敌对队伍效果
        for(int i = 0; i < BM.Remaining_ORL.Count; i++)
        {
            BM.Remaining_ORL[i].getBCOA(otherBarChartEffect);
            if (otherRALChange.HaveData)
            {
                BM.Remaining_ORL[i].ThisBasicRoleProperty()._role.groupStateRAL = otherRALChange;
            }
            else
            {
                BM.Remaining_ORL[i].ThisBasicRoleProperty()._role.groupStateRAL = RoleAttributeList.zero;
            }
        }
    } 
}