using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;
using System.Linq;

public class SDSkillSelect : MonoBehaviour
{
    public SDHeroDetail heroDetail;
    public SkillDetailsList SDL
    {
        get { return GetComponent<SkillDetailsList>(); }
    }
    public Transform skillItem;
    //[HideInInspector]
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
    public void initHeroAllSkills()
    {
        int hashcode = heroDetail.Hashcode;

        currentAll = SDDataManager.Instance.getAllSkillsByHashcode(hashcode);
        currentAll.Sort(
        (x, y) =>
        {
            return x.islocked.CompareTo(y.islocked);
        });
        resetSkillList();



        for (int i = 0; i < currentAll.Count; i++)
        {
            Transform s = Instantiate(skillItem) as Transform;
            s.SetParent(rect.content);
            s.localScale = Vector3.one;
            s.gameObject.SetActive(true);
            RTSingleSkillItem _S = s.GetComponent<RTSingleSkillItem>();
            if (SDDataManager.Instance.ifDeployThisSkill
                (currentAll[i].skillId, heroDetail.Hashcode))
            {
                _S.isDeployed = true;
            }
            else _S.isDeployed = false;
            _S.initSkillItem(currentAll[i], heroDetail.Hashcode);//构建技能基础信息

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
