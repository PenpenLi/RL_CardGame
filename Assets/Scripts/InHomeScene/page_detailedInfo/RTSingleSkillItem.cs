using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

public class RTSingleSkillItem : MonoBehaviour
{
    public Image itemImg;
    public Text itemNameText;
    public Image itemBgImg;
    public ItemStarVision starVision;
    public int ItemId;
    public Transform LockedPanel;
    public bool isLocked
    {
        get
        {
            if (LockedPanel && LockedPanel.gameObject.activeSelf) return true;
            return false;
        }
        set { LockedPanel.gameObject.SetActive(value); }
    }
    public Transform DeployPanel;
    public bool isDeployed
    {
        get
        {
            if (DeployPanel && DeployPanel.gameObject.activeSelf) return true;
            return false;
        }
        set { DeployPanel.gameObject.SetActive(value); }
    }
    public Transform SelectedPanel;
    public bool isSelected
    {
        get 
        { 
            if (SelectedPanel && SelectedPanel.gameObject.activeSelf) return true;
            return false;
        }
        set { SelectedPanel.gameObject.SetActive(value); }
    }

    public SDConstants.deployType deploy = SDConstants.deployType.skill;
    public int use_type;
    public void BtnTapped()
    {
        if(deploy == SDConstants.deployType.skill)
        {
            if (use_type == 1)
                chooseSkillToDeploy();
            else if(use_type == 2) 
                chooseSkillToShowDetail();
        }
    }
    public void chooseSkillToDeploy()
    {
        //SDSkillSelect SS = GetComponentInParent<SDSkillSelect>();
        SDHeroDeploySkills HDS = GetComponentInParent<SDHeroDeploySkills>();
        HDS.SelectSkill(this);
    }
    public void chooseSkillToShowDetail()
    {

    }
    public void initSkillItem(OneSkill baseskilldata , int heroHashcode = 0 )
    {
        deploy = SDConstants.deployType.skill;
        ItemId = baseskilldata.skillId;
        itemNameText.text = baseskilldata.SkillName;
        isLocked = !baseskilldata.isUnlocked;
        if (!isLocked)
        {
            starVision.StarNum = baseskilldata.lv;
            //
            if (heroHashcode > 0)
            {
                if (SDDataManager.Instance.ifDeployThisSkill
                    (ItemId, heroHashcode))
                {
                    isDeployed = true;
                }
            }
        }
        else
        {
            starVision.StarNum = -1;
        }
    }
}
