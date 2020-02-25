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
    public SDHeroDeploySkills HDS;
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

        currentAll = SDDataManager.Instance.getSkillListByHashcode(hashcode);
        resetSkillList();

        for (int i = 0; i < currentAll.Count; i++)
        {
            Transform s = Instantiate(skillItem) as Transform;
            s.SetParent(rect.content);
            s.localScale = Vector3.one;
            s.gameObject.SetActive(true);
            RTSingleSkillItem _S = s.GetComponent<RTSingleSkillItem>();
            _S.initSkillItem(currentAll[i], heroDetail.Hashcode);//构建技能基础信息

            skillList.Add(_S);
        }
    }
    public void refreshAllSkillsCondition(string currentSkillId)
    {
        if (HDS == null) return;
        HDS.currentSkillId = currentSkillId;
        HDS.currentSkill = SDDataManager.Instance.getOwnedSkillById
            (currentSkillId, heroDetail.Hashcode);
        HDS.refreshSkillDetail();
        foreach(RTSingleSkillItem S in skillList)
        {
            if (S.ItemId == currentSkillId) S.isSelected = true;
            else S.isSelected = false;
        }
    }
}
