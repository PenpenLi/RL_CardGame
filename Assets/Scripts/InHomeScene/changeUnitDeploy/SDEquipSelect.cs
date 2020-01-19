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

    public Image TitleIcon;

    public Transform emptyEquipPanel;
    [Space]
    public Image equipIcon;
    public Image equipBgIcon;
    public Image equipFrameIcon;
    [Space]
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

    void refreshEquipIcon()
    {
        TitleIcon.sprite = SDDataManager.Instance.equipPosIcon(equipPos);
        TitleIcon.SetNativeSize();
    }
    public void initPosEquipSelectPanel(EquipPosition Pos, bool isSecondJPos=false)
    {
        careerType = heroDetail.careerIndex;
        equipPos = Pos;

        pageController.currentHeroHashcode = heroDetail.Hashcode;
        SDGameManager.Instance.stockUseType = SDConstants.StockUseType.work;
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
        refreshEquipIcon();
        //titleText.text = "Helmet";
        List<GDEEquipmentData> equips = SDDataManager.Instance.GetPosOwnedEquipsByCareer
            (EquipPosition.Head, heroDetail.ID);
        GDEEquipmentData helmet = SDDataManager.Instance.getHeroEquipHelmet(heroDetail.Hashcode);
        if(helmet == null || string.IsNullOrEmpty(helmet.id))
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
        pageController.ItemsInit(SDConstants.ItemType.Equip,EquipPosition.Head);
    }
    public void initBreastplateSelectPanel()
    {
        refreshEquipIcon();
        //titleText.text = "Breastplate";
        List<GDEEquipmentData> equips = SDDataManager.Instance.GetPosOwnedEquipsByCareer
            (EquipPosition.Breast,heroDetail.ID);
        GDEEquipmentData breastplate = SDDataManager.Instance.getHeroEquipBreastplate(heroDetail.Hashcode);
        if (breastplate == null || string.IsNullOrEmpty(breastplate.id))
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
        pageController.ItemsInit(SDConstants.ItemType.Equip,EquipPosition.Breast);
    }
    public void initGardebrasSelectPanel()
    {
        refreshEquipIcon();
        //titleText.text = "Gardebras";
        List<GDEEquipmentData> equips = SDDataManager.Instance.GetPosOwnedEquipsByCareer
            (EquipPosition.Arm, heroDetail.ID);
        GDEEquipmentData gardebras = SDDataManager.Instance.getHeroEquipGardebras(heroDetail.Hashcode);
        if (gardebras == null || string.IsNullOrEmpty(gardebras.id))
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
        pageController.ItemsInit(SDConstants.ItemType.Equip,EquipPosition.Arm);
    }
    public void initLeggingSelectPanel()
    {
        refreshEquipIcon();
        //titleText.text = "Legging";
        List<GDEEquipmentData> equips = SDDataManager.Instance.GetPosOwnedEquipsByCareer
            (EquipPosition.Leg, heroDetail.ID);
        GDEEquipmentData legging = SDDataManager.Instance.getHeroEquipLegging(heroDetail.Hashcode);
        if (legging == null || string.IsNullOrEmpty(legging.id))
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
        pageController.ItemsInit(SDConstants.ItemType.Equip,EquipPosition.Leg);
    }
    public void initJewelrySelectPanel()
    {
        refreshEquipIcon();
        //titleText.text = "Jewelry";
        List<GDEEquipmentData> equips = SDDataManager.Instance.GetPosOwnedEquipsByCareer
            (EquipPosition.Finger, heroDetail.ID);
        GDEEquipmentData jewelry = SDDataManager.Instance.getHeroEquipJewelry(heroDetail.Hashcode);
        if (jewelry == null || string.IsNullOrEmpty(jewelry.id))
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
        pageController.ItemsInit(SDConstants.ItemType.Equip,EquipPosition.Finger);
    }
    public void initWeaponSelectPanel()
    {
        refreshEquipIcon();
        //titleText.text = "Weapon";
        List<GDEEquipmentData> equips = SDDataManager.Instance.GetPosOwnedEquipsByCareer
            (EquipPosition.Hand, heroDetail.ID);
        GDEEquipmentData weapon = SDDataManager.Instance.getHeroWeapon(heroDetail.Hashcode);
        if (weapon == null || string.IsNullOrEmpty(weapon.id))
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
        pageController.ItemsInit(SDConstants.ItemType.Equip,EquipPosition.Hand);
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
                equipedItemLevel.text = SDGameManager.T("Lv.")
                    + equip.lv;           
                equipedItemBattleForce.text
                    = "" + SDDataManager.Instance.getEquipBattleForceByHashCode(equip.hashcode);
                //
                equipIcon.sprite = SDDataManager.Instance.GetEquipIconById(equip.id);
                EquipItem item = SDDataManager.Instance.GetEquipItemById(equip.id);
                equipFrameIcon.sprite = SDDataManager.Instance.baseFrameSpriteByRarity(item.LEVEL);
                equipBgIcon.sprite = SDDataManager.Instance.baseBgSpriteByRarity(item.LEVEL);
                //
                equipedItemBtn.gameObject.SetActive(true);
                if (equip.OwnerHashcode > 0)//判断是否已被装备
                {
                    bool flag = false;
                    if(equip.OwnerHashcode == heroDetail.Hashcode)//是当前角色装备
                    {
                        flag = true;
                        if (SDDataManager.Instance.getEquipPosById(equip.id) == 4)
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
        equipIcon.sprite = null;
        equipBgIcon.sprite = null;
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
