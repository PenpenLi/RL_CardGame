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
        selfRALChange = RoleAttributeList.zero;
        otherRALChange = RoleAttributeList.zero;
        BM.resetHTED();
        BM.resetETHD();
        //
        CheckRaceEffect();
        //对玩家队伍效果
        for(int i =0;i < BM.Remaining_SRL.Count; i++)
        {
            BM.Remaining_SRL[i].getBCOA(selfBarChartEffect);
            if (selfRALChange.HaveData)
            {
                BM.Remaining_SRL[i].ThisBasicRoleProperty()
                    ._role.extraRALChangeData.Add(selfRALChange);
            }
        }

        //对敌对队伍效果
        for(int i = 0; i < BM.Remaining_ORL.Count; i++)
        {
            BM.Remaining_ORL[i].getBCOA(otherBarChartEffect);
            if (otherRALChange.HaveData)
            {
                BM.Remaining_ORL[i].ThisBasicRoleProperty()
                    ._role.extraRALChangeData.Add(otherRALChange);
            }
        }
    } 


    public static int setNormalDmgByPlayerData(int oldDmg,SkillKind kind
        , BattleRoleData target)
    {
        if (target.IsEnemy)
        {
            List<GDEtownBuildingData> all
                = SDDataManager.Instance.PlayerData.buildingsOwned;
            foreach(GDEtownBuildingData b in all)
            {
                if(b.id == "BUILD#"+16 && kind == SkillKind.Physical)
                {
                    int lv = b.level;
                    int dmg = (int)(oldDmg * (1 + lv * SDConstants.BuffBuildingImproveFigureByLv));
                    return dmg;
                }
                else if(b.id == "BUILD#"+17 && kind == SkillKind.Elemental)
                {
                    int lv = b.level;
                    int dmg = (int)(oldDmg * (1 + lv * SDConstants.BuffBuildingImproveFigureByLv));
                    return dmg;
                }
            }
            return oldDmg;
        }
        else
        {
            return oldDmg;
        }
    }


    public void CheckRaceEffect()
    {
        #region heroSide
        List<BattleRoleData> humans = BM.Remaining_SRL.FindAll(x =>
        {
            return x.HeroProperty._hero._heroRace == Race.Human;
        });
        if (humans.Count > 1)
        {
            int count = humans.Count;
            selfRALChange.Add(5 * count, AttributeData.AT);
            selfRALChange.Add(5 * count, AttributeData.MT);
            if (count % 2 == 0)
            {
                BM.heroToEnemyDmg_event+=(x)=>
                {
                    return (int)(x * AllRandomSetClass.SimplePercentToDecimal(100 + count / 2 * 5));
                };
            }
        }
        //
        List<BattleRoleData> elfs = BM.Remaining_SRL.FindAll(x =>
        {
            return x.HeroProperty._hero._heroRace == Race.Elf;
        });
        if (elfs.Count > 1)
        {
            int count = elfs.Count;
            selfRALChange.Add(5 * count, AttributeData.AT);
            selfRALChange.Add(5 * count, AttributeData.MT);
            if (count % 2 == 0)
            {
                selfRALChange.Add(count, AttributeData.Crit);
                selfRALChange.Add(count, AttributeData.Evo);
            }
        }
        //
        List<BattleRoleData> dragonborns = BM.Remaining_SRL.FindAll(x =>
        {
            return x.HeroProperty._hero._heroRace == Race.Dragonborn;
        });
        if (dragonborns.Count > 1)
        {
            int count = dragonborns.Count;
            selfRALChange.Add(5 * count, AttributeData.AT);
            selfRALChange.Add(5 * count, AttributeData.MT);
            if (count % 2 == 0)
            {
                BM.enemyToHeroDmg_event += (x) =>
                {
                    return (int)(x * AllRandomSetClass.SimplePercentToDecimal(100 - count / 2 * 5));
                };
            }
        }
        #endregion
        #region enemySide
        //for(int i=0;i<)
        #endregion
    }


}