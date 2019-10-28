using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

/// <summary>
/// 武器选择页，点击武器后可查看属性变化
/// </summary>
public class SDEquipSelect : MonoBehaviour
{
    public HEWPageController pageController;
    public EquipPosition equipPos;
    public bool isSecondJewelryPos = false;

    public Text titleText;

    public Transform emptyEquipPanel;

    public Text equipedItemName;
    public Text equipedItemLevel;
    public Text equipedItemBattleForce;
    public Button equipedItemBtn;
    public Text equipItemBtnText;
    public Button equipedOrRemoveBtn;
    [HideInInspector]
    public int currentEquipHashcode;
    //public GDEEquipmentData CurrentEquipment;


    public SDHeroDetail heroDetail;
    public Job careerType;


    public void initPosEquipSelectPanel(EquipPosition Pos, bool isSecondJPos=false)
    {
        careerType = (Job)heroDetail.careerIndex;
        equipPos = Pos;

        if(Pos == EquipPosition.Head)
        {
            initHelmetSelectPanel();
        }
        else if(Pos == EquipPosition.Breast)
        {
            initBreastplateSelectPanel();
        }
        else if(Pos == EquipPosition.Arm)
        {
            initGardebrasSelectPanel();
        }
        else if(Pos == EquipPosition.Leg)
        {
            initLeggingSelectPanel();
        }
        else if(Pos == EquipPosition.Finger)
        {
            isSecondJewelryPos = isSecondJPos;
            initJewelrySelectPanel();
        }
        else if(Pos == EquipPosition.Hand)
        {
            initWeaponSelectPanel();
        }
    }
    public void initHelmetSelectPanel()
    {
        titleText.text = "Helmet";
        List<GDEEquipmentData> equips = SDDataManager.Instance.GetPosOwnedEquipsByCareer
            (EquipPosition.Head, careerType);
        GDEEquipmentData helmet = SDDataManager.Instance.getHeroEquipHelmet(heroDetail.Hashcode);
        if(helmet == null || helmet.equipId == 0)
        {
            equipedItemBtn.gameObject.SetActive(false);
            equipedItemName.text = "";
            equipedItemLevel.text = "";
            setEquipDetailPanelEmpty();
        }
        else
        {
            refreshSelectedEquipmentDetail(helmet.hashcode);
            //helmet.
        }
        pageController.jobType = careerType;
        pageController.ItemsInit(SDConstants.ItemType.Helmet);
    }
    public void initBreastplateSelectPanel()
    {
        titleText.text = "Breastplate";
        List<GDEEquipmentData> equips = SDDataManager.Instance.GetPosOwnedEquipsByCareer
            (EquipPosition.Breast, careerType);
        GDEEquipmentData breastplate = SDDataManager.Instance.getHeroEquipBreastplate(heroDetail.Hashcode);
        if (breastplate == null || breastplate.equipId == 0)
        {
            equipedItemBtn.gameObject.SetActive(false);
            equipedItemName.text = "";
            equipedItemLevel.text = "";
            setEquipDetailPanelEmpty();
        }
        else
        {
            refreshSelectedEquipmentDetail(breastplate.hashcode);
            //breastplate.
        }
        pageController.jobType = careerType;
        pageController.ItemsInit(SDConstants.ItemType.Breastplate);
    }
    public void initGardebrasSelectPanel()
    {
        titleText.text = "Gardebras";
        List<GDEEquipmentData> equips = SDDataManager.Instance.GetPosOwnedEquipsByCareer
            (EquipPosition.Arm, careerType);
        GDEEquipmentData gardebras = SDDataManager.Instance.getHeroEquipGardebras(heroDetail.Hashcode);
        if (gardebras == null || gardebras.equipId == 0)
        {
            equipedItemBtn.gameObject.SetActive(false);
            equipedItemName.text = "";
            equipedItemLevel.text = "";
            setEquipDetailPanelEmpty();
        }
        else
        {
            refreshSelectedEquipmentDetail(gardebras.hashcode);
            //gardebras.
        }
        pageController.jobType = careerType;
        pageController.ItemsInit(SDConstants.ItemType.Gardebras);
    }
    public void initLeggingSelectPanel()
    {
        titleText.text = "Legging";
        List<GDEEquipmentData> equips = SDDataManager.Instance.GetPosOwnedEquipsByCareer
            (EquipPosition.Leg, careerType);
        GDEEquipmentData legging = SDDataManager.Instance.getHeroEquipLegging(heroDetail.Hashcode);
        if (legging == null || legging.equipId == 0)
        {
            equipedItemBtn.gameObject.SetActive(false);
            equipedItemName.text = "";
            equipedItemLevel.text = "";
            setEquipDetailPanelEmpty();
        }
        else
        {
            refreshSelectedEquipmentDetail(legging.hashcode);
            //legging.
        }
        pageController.jobType = careerType;
        pageController.ItemsInit(SDConstants.ItemType.Legging);
    }
    public void initJewelrySelectPanel()
    {
        titleText.text = "Jewelry";
        List<GDEEquipmentData> equips = SDDataManager.Instance.GetPosOwnedEquipsByCareer
            (EquipPosition.Finger, careerType);
        GDEEquipmentData jewelry = SDDataManager.Instance.getHeroEquipJewelry(heroDetail.Hashcode);
        if (jewelry == null || jewelry.equipId == 0)
        {
            equipedItemBtn.gameObject.SetActive(false);
            equipedItemName.text = "";
            equipedItemLevel.text = "";
            setEquipDetailPanelEmpty();
        }
        else
        {
            refreshSelectedEquipmentDetail(jewelry.hashcode);
            //jewelry.
        }
        pageController.jobType = careerType;
        pageController.ItemsInit(SDConstants.ItemType.Jewelry);
    }
    public void initWeaponSelectPanel()
    {
        titleText.text = "Weapon";
        List<GDEEquipmentData> equips = SDDataManager.Instance.GetPosOwnedEquipsByCareer
            (EquipPosition.Hand, careerType);
        GDEEquipmentData weapon = SDDataManager.Instance.getHeroWeapon(heroDetail.Hashcode);
        if (weapon == null || weapon.equipId == 0)
        {
            equipedItemBtn.gameObject.SetActive(false);
            equipedItemName.text = "";
            equipedItemLevel.text = "";
            setEquipDetailPanelEmpty();
        }
        else
        {
            refreshSelectedEquipmentDetail(weapon.hashcode);
            //weapon.
        }
        pageController.jobType = careerType;
        pageController.ItemsInit(SDConstants.ItemType.Weapon);
    }
    public void refreshSelectedEquipmentDetail(int hashcode)
    {
        if (hashcode > 0)
        {
            GDEEquipmentData equip = SDDataManager.Instance.getEquipmentByHashcode(hashcode);
            
            if (equip != null)
            {
                currentEquipHashcode = equip.hashcode;
                equipedItemName.text 
                    = SDDataManager.Instance.getEquipNameByHashcode(currentEquipHashcode);
                equipedItemLevel.text = SDGameManager.T("Lv.") + (equip.equipLv + equip.upLv);           
                equipedItemBattleForce.text
                    = "" + SDDataManager.Instance.getBattleForceByHashCode(equip.hashcode);
                equipedItemBtn.gameObject.SetActive(true);
                if (equip.OwnerHashcode > 0)//判断是否已被装备
                {
                    bool flag = false;
                    if(equip.OwnerHashcode == heroDetail.Hashcode)//是当前角色装备
                    {
                        flag = true;
                        if (SDDataManager.Instance.getEquipPosById(equip.equipId) == 4)
                        {
                            if (!isSecondJewelryPos 
                                && SDDataManager.Instance.getHeroByHashcode(heroDetail.Hashcode)
                                .jewelry0.hashcode != equip.hashcode)
                            {
                                flag = false;
                            }
                            else if(isSecondJewelryPos
                                && SDDataManager.Instance.getHeroByHashcode(heroDetail.Hashcode)
                                .jewelry1.hashcode != equip.hashcode)
                            {
                                flag = false;
                            }
                        }
                    }
                    else
                    {
                        flag = false;
                    }
                    if (flag)
                    {
                        equipItemBtnText.text = SDGameManager.T("解除装备");
                    }
                    else
                    {
                        equipedItemBtn.gameObject.SetActive(false);
                        equipItemBtnText.text = SDGameManager.T("无法装备");
                    }
                }
                else
                {
                    equipItemBtnText.text = SDGameManager.T("装备");



                }                
            }         
        }
    }
    public void setEquipDetailPanelEmpty()
    {

    }
    public void refreshEmptyEquipPanel(bool state)
    {
        emptyEquipPanel?.gameObject.SetActive(state);
    }
    public void EquipBelowBtn_Tapped()
    {
        GDEEquipmentData e 
            = SDDataManager.Instance.getEquipmentByHashcode(currentEquipHashcode);
        int ownerHashcode = e.OwnerHashcode;
        if(ownerHashcode == heroDetail.Hashcode)//解除装备
        {
            SDDataManager.Instance.disrobeEquipment(heroDetail.Hashcode, equipPos,isSecondJewelryPos);
            heroDetail.initHeroDetailPanel(heroDetail.Hashcode);
            refreshSelectedEquipmentDetail(currentEquipHashcode);
        }
        else if (ownerHashcode > 0)//已被其他角色装备
        {

        }
        else//装备
        {
            SDDataManager.Instance.dressEquipment
                    (heroDetail.Hashcode, currentEquipHashcode, isSecondJewelryPos);
            heroDetail.initHeroDetailPanel(heroDetail.Hashcode);
            refreshSelectedEquipmentDetail(currentEquipHashcode);
        }
    }

}
