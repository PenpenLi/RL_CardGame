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
    public Image UnlockedImg;
    public bool isUnlocked;
    public bool isDeployed;
    public SDConstants.deployType deploy = SDConstants.deployType.skill;
    public bool isShowSkillDetail;
    public void BtnTapped()
    {
        if(deploy == SDConstants.deployType.skill)
        {
            if (!isShowSkillDetail)
                chooseSkillToDeploy();
            else chooseSkillToShowDetail();
        }
    }
    public void chooseSkillToDeploy()
    {
        SDSkillSelect SS = GetComponentInParent<SDSkillSelect>();

    }
    public void chooseSkillToShowDetail()
    {

    }
    public void initSkillItem(OneSkill baseskilldata)
    {
        deploy = SDConstants.deployType.skill;
        ItemId = baseskilldata.skillId;
        itemNameText.text = baseskilldata.SkillName;
        isUnlocked = baseskilldata.isUnlocked;
        if (isUnlocked)
        {
            starVision.StarNum = baseskilldata.lv + 1;
            UnlockedImg.gameObject.SetActive(false);
        }
        else
        {
            starVision.StarNum = 0;
            UnlockedImg.gameObject.SetActive(true);
        }
    }
}
