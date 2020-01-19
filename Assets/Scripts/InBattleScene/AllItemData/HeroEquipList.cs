using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroEquipList : MonoBehaviour
{
    public bool isClicking;
    public OneEquipVision helmetD;
    public Helmet _helmet;
    //
    public OneEquipVision breastplateD;
    public Breastplate _breastplate;
    //
    public OneEquipVision gardebrasD;
    public Gardebras _gardebras;
    //
    public OneEquipVision leggingD;
    public Legging _legging;
    //
    public OneEquipVision jewelry0D;
    public Jewelry _jewelry0;
    //
    public OneEquipVision jewelry1D;
    public Jewelry _jewelry1;
    //
    public OneEquipVision weaponD;
    public SDWeapon _weapon;

    //
    public SDHeroDetail HD;
    public SDEquipSelect ES;
    public HeroDetailPanel HDP;
    //
    public EquipPosition CurrentSearchingPos = EquipPosition.End;
    public bool isSecondPos;
    public OneEquipVision EquipVision(EquipPosition pos, bool isSecondJewelry = false)
    {
        switch (pos)
        {
            case EquipPosition.Head:
                return helmetD;
            case EquipPosition.Breast:
                return breastplateD;
            case EquipPosition.Arm:
                return gardebrasD;
            case EquipPosition.Leg:
                return leggingD;
            case EquipPosition.Finger:
                if (!isSecondJewelry) return jewelry0D;
                else return jewelry1D;
            case EquipPosition.Hand:
                return weaponD;
            default: return helmetD;
        }
    }

    private void Awake()
    {
        BuildEquipListBase();
    }
    public void BuildEquipListBase()
    {
        if (_helmet == null && helmetD != null)
            _helmet = helmetD.gameObject.AddComponent<Helmet>();
        if (_breastplate == null && breastplateD != null)
            _breastplate = breastplateD.gameObject.AddComponent<Breastplate>();
        if (_gardebras == null && gardebrasD != null)
            _gardebras = gardebrasD.gameObject.AddComponent<Gardebras>();
        if (_legging == null && leggingD != null)
            _legging = leggingD.gameObject.AddComponent<Legging>();
        if (_jewelry0 == null && jewelry0D != null)
            _jewelry0 = jewelry0D.gameObject.AddComponent<Jewelry>();
        if (_jewelry1 == null && jewelry1D != null)
            _jewelry1 = jewelry1D.gameObject.AddComponent<Jewelry>();
        if (_weapon == null && weaponD != null)
            _weapon = weaponD.gameObject.AddComponent<SDWeapon>();
    }
    public void equipBtnTapped(EquipPosition pos,bool isSecondJewelry = false)
    {
        if (isClicking) return;
        else
        {
            isClicking = true;
        }
        bool flag = false;
        if (CurrentSearchingPos != pos)
        {
            flag = true;
        }
        else
        {
            if(pos == EquipPosition.Finger && isSecondPos != isSecondJewelry)
            {
                flag = true;
            }
            else
            {
                flag = false;
            }
        }
        if (flag)
        {
            CurrentSearchingPos = pos;
            isSecondPos = isSecondJewelry;
            if(HDP.CurrentRDSubType != HeroDetailPanel.RoleDetailSubType.heroEquip)
            {
                HDP.CurrentRDSubType = HeroDetailPanel.RoleDetailSubType.heroEquip;
                HDP.historyAdd(HDP.CurrentRDSubType);
            }
            ES.initPosEquipSelectPanel(pos, isSecondPos);
            UIEffectManager.Instance.showAnimFadeIn(ES.transform);
        }
        else
        {
            UIEffectManager.Instance.hideAnimFadeOut(ES.transform);
            CurrentSearchingPos = EquipPosition.End;
            //
            if(HDP.CurrentRDSubType == HeroDetailPanel.RoleDetailSubType.heroEquip)
                HDP.commonBackAction();
        }
        Invoke("BtnTappedEnd", 0.25f);
    }
    void BtnTappedEnd()
    {
        isClicking = false;
    }
    void refreshAllEquipVisions()
    {

    }
}
