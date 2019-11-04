using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

public class SDSkillSelect : MonoBehaviour
{
    //SDHeroDetail hd;
    public SDHeroDetail heroDetail;
    public SkillDetailsList SDL
    {
        get { return GetComponent<SkillDetailsList>(); }
    }
    public Transform skillItem;
    [HideInInspector]
    public List<OneSkill> currentAll;
    public List<RTSingleSkillItem> skillList;
    public ScrollRect rect;
    public enum itemUseType
    {
        none,deploy,detail,
    }
    public itemUseType item_use_type = itemUseType.none;
    public void resetSkillList()
    {
        for(int i = 0; i < skillList.Count; i++)
        {
            Destroy(skillList[i].gameObject);
        }
        skillList.Clear();
        rect.horizontalNormalizedPosition = 0;
    }
    public void initHeroAllSkills(bool noSort = false)
    {
        Job career = heroDetail._hero._heroJob;
        Race race = heroDetail._hero._heroRace;
        List<OneSkill> all = SDDataManager.Instance.getAllSkillsByHashcode(heroDetail.Hashcode);
        currentAll = all;
        if (!noSort)
        {
            all.Sort(
                (x, y) =>
                {
                    return x.isUnlocked.CompareTo(!y.isUnlocked);
                });
        }
        resetSkillList();



        for (int i = 0; i < all.Count; i++)
        {
            Transform s = Instantiate(skillItem) as Transform;
            s.SetParent(rect.content);
            s.localScale = Vector3.one;
            s.gameObject.SetActive(true);
            RTSingleSkillItem _S = s.GetComponent<RTSingleSkillItem>();

            _S.initSkillItem(all[i], heroDetail.Hashcode);//构建技能基础信息
            if (SDDataManager.Instance.ifDeployThisSkill(all[i].skillId, heroDetail.Hashcode))
            {
                _S.isDeployed = true;
            }
            if(item_use_type == itemUseType.deploy)
            {
                _S.use_type = 1;
            }
            else if(item_use_type == itemUseType.detail)
            {
                _S.use_type = 2;
            }
            skillList.Add(_S);
        }
    }



    public void BtnToShowSkillDetail(int skillId)
    {

    }

}
