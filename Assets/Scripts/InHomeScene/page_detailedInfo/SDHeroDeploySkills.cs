using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

public class SDHeroDeploySkills : MonoBehaviour
{
    public SDHeroDetail heroDetail;
    public HeroDetailPanel HDP;
    public string currentSkillId;
    [HideInInspector]
    public OneSkill currentSkill;
    [HideInInspector]
    public int SSI_Id;
    public SDSkillSelect skillSelect;
    //public Transform ModelAndSkilllistPanel;
    [Header("技能描述")]
    public Text skill_limit;
    public Text skill_name;
    public Text skill_tag;
    public Text skill_basedata;
    public Text skill_statelist;
    public Text skill_desc;

    public void initHeroSkillListPanel()
    {
        skillSelect.initHeroAllSkills();
        bool flag = false;
        for(int i = 0; i < skillSelect.skillList.Count; i++)
        {

        }
        if (!flag)
        {
            skillSelect.skillList[0].isSelected = true;
            currentSkillId = skillSelect.skillList[0].ItemId;
            currentSkill 
                = SDDataManager.Instance.getOwnedSkillById(currentSkillId, heroDetail.Hashcode);
        }
        refreshSkillDetail();
    }
    public void SelectSkill(RTSingleSkillItem skillItem)
    {
        if (currentSkillId == skillItem.ItemId) return;
        //
        SSI_Id = skillSelect.skillList.IndexOf(skillItem);
        for(int i = 0; i < skillSelect.skillList.Count; i++)
        {
            skillSelect.skillList[i].isSelected = false;
        }
        skillItem.isSelected = true;
        currentSkillId = skillItem.ItemId;
        currentSkill = SDDataManager.Instance.getOwnedSkillById
            (currentSkillId, heroDetail.Hashcode);
        refreshSkillDetail();
    }
    public void refreshSkillDetail()
    {
        #region allSkills
        if (currentSkill != null)
        {
            SkillFunction function 
                = HDP.skillDetailList.AllSkillList[currentSkill.SkillFunctionID]
                .GetComponent<SkillFunction>();
            if (currentSkill.UseAppointedPrefab)
            {
                function = currentSkill.SkillPrefab.GetComponent<SkillFunction>();
            }
            //
            skill_limit.text = SDGameManager.T
                (currentSkill.isOmegaSkill ? "绝招" : "普通");
            skill_name.text = currentSkill.SkillName + "·Lv " + currentSkill.lv;
            skill_basedata.text 
                = (function.CritR != 0 ? string.Format("基础暴击修正 {0:D}", function.CritR) : "")
            + (function.AccuracyR != 0 ? string.Format("·基础精度修正 {0:D}", function.AccuracyR) : "")
            + (function.ExpectR != 0 ? string.Format("·基础期望修正 {0:D}", function.ExpectR) : "");
            skill_desc.text = currentSkill.Desc;
        }
        #endregion
        #region equipedSkills
        heroDetail.readHeroSkills();
        #endregion
    }
    public void btnToImprove()
    {
        HDP.BtnToHeroImprove();
    }
}
