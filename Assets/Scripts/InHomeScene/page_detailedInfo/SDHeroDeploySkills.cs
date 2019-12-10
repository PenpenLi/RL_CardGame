using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

public class SDHeroDeploySkills : MonoBehaviour
{
    public SDHeroDetail heroDetail;
    public HeroDetailPanel HDP;
    public int currentSkillId;
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
    [Header("交互用按钮")]
    public Button btn_deploy;
    public Button btn_improve;

    public void initHeroSkillListPanel()
    {
        skillSelect.initHeroAllSkills();
        bool flag = false;
        for(int i = 0; i < skillSelect.skillList.Count; i++)
        {
            if (skillSelect.skillList[i].isDeployed)
            {
                flag = true;
                skillSelect.skillList[i].isSelected = true;
                currentSkillId = skillSelect.skillList[i].ItemId;
                currentSkill 
                    = SDDataManager.Instance.getOwnedSkillById(currentSkillId, heroDetail.Hashcode);
                break;
            }
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
        if (skillItem.isLocked) return;
        if (currentSkillId == skillItem.ItemId) return;
        //
        SSI_Id = skillSelect.skillList.IndexOf(skillItem);
        for(int i = 0; i < skillSelect.skillList.Count; i++)
        {
            skillSelect.skillList[i].isSelected = false;
        }
        skillItem.isSelected = true;
        currentSkillId = skillItem.ItemId;
        currentSkill = SDDataManager.Instance.getOwnedSkillById(currentSkillId, heroDetail.Hashcode);
        refreshSkillDetail();
    }
    public void refreshSkillDetail()
    {
        #region allSkills
        bool deployEnable = false;
        bool improveEnable = false;
        if (currentSkill != null)
        {
            if (currentSkill.isUnlocked)//未解锁
            {
                deployEnable = true;
            }
            if (currentSkill.lv < SDConstants.SkillMaxGrade)//已达上限
            {
                improveEnable = true;
            }
            if (SDDataManager.Instance.ifDeployThisSkill(currentSkillId, heroDetail.Hashcode))//已装备
            {
                btn_deploy.GetComponentInChildren<Text>().text
                    = SDGameManager.T("解除");
            }
            else//未装备
            {
                btn_deploy.GetComponentInChildren<Text>().text
                    = SDGameManager.T("装备");
            }
            btn_deploy.gameObject.SetActive(deployEnable);
            btn_improve.gameObject.SetActive(improveEnable);
            //
            SkillFunction function 
                = HDP.skillDetailList.AllSkillList[currentSkill.SkillFunctionID]
                .GetComponent<SkillFunction>();
            //
            skill_name.text = currentSkill.SkillName + "·Lv " + currentSkill.lv;
            skill_basedata.text 
                = (function.CritR != 0 ? string.Format("基础暴击修正 {0:D}", function.CritR) : "")
            + (function.AccuracyR != 0 ? string.Format("·基础精度修正 {0:D}", function.AccuracyR) : "")
            + (function.ExpectR != 0 ? string.Format("·基础期望修正 {0:D}", function.ExpectR) : "");
            skill_desc.text = currentSkill.Desc;
        }
        #endregion
        #region equipedSkills
        HDP.readHeroEquipedSkills(heroDetail.Hashcode);
        #endregion
    }

    public void DeploySkill( int skillPos )
    {
        int heroHC = heroDetail.Hashcode;
        RTSingleSkillItem si = skillSelect.skillList[SSI_Id];
        if (si.isSelected)//基础条件
        {
            if (si.isDeployed)//已经被装备
            {
                SDDataManager.Instance.UnDeploySkillById(si.ItemId, heroHC);
                si.isDeployed = false;
            }
            else if(!si.isLocked)//未被装备同时已经解锁
            {
                SDDataManager.Instance.changeEquipedSkill(si.ItemId, skillPos, heroHC);
                si.isDeployed = true;
            }
            currentSkill
                = SDDataManager.Instance.getOwnedSkillById(currentSkillId, heroDetail.Hashcode);
            refreshSkillDetail();
        }
    }

    public void btnToDeploy()
    {
        int heroHC = heroDetail.Hashcode;
        GDEHeroData hero = SDDataManager.Instance.getHeroByHashcode(heroHC);
        RTSingleSkillItem si = skillSelect.skillList[SSI_Id];
        if (si.isSelected)
        {
            if (si.isDeployed)//已经被装备
            {
                SDDataManager.Instance.UnDeploySkillById(si.ItemId, heroHC);
                si.isDeployed = false;
                refreshSkillDetail();
            }
            else if (!si.isLocked)//未被装备同时已经解锁
            {
                if (currentSkill.isOmegaSkill)
                {
                    if(hero.skillOmegaId == 0)//槽位为空
                    {
                        SDDataManager.Instance.changeEquipedSkill(currentSkillId, 2, heroHC);
                        si.isDeployed = true;
                        refreshSkillDetail();
                    }
                    else
                    {
                        Debug.Log("无法装备:绝招槽已被占用");
                    }
                }
                else
                {
                    if (!SDDataManager.Instance.checkHeroEnableSkill1ById(hero.id))
                    {
                        if (hero.skill0Id > 0)
                        {
                            Debug.Log("无法装备:唯一普通技能槽已被占用");
                        }
                        else
                        {
                            SDDataManager.Instance.changeEquipedSkill(currentSkillId, 0, heroHC);
                            si.isDeployed = true;
                            refreshSkillDetail();
                        }
                    }
                    else
                    {
                        if(hero.skill0Id == 0)
                        {
                            SDDataManager.Instance.changeEquipedSkill(currentSkillId, 0, heroHC);
                            si.isDeployed = true;
                            refreshSkillDetail();
                            return;
                        }
                        if(hero.skill1Id == 0)
                        {
                            SDDataManager.Instance.changeEquipedSkill(currentSkillId, 1, heroHC);
                            si.isDeployed = true;
                            refreshSkillDetail();
                            return;
                        }
                        Debug.Log("无法装备:所有普通技能槽均已被占用");
                    }
                }
            }
        }
    }
    public void btnToImprove()
    {
        HDP.BtnToHeroImprove();
    }
}
