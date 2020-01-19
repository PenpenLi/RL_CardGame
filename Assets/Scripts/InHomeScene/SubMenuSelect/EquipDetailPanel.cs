using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;
using DG.Tweening;

public class EquipDetailPanel : MonoBehaviour
{
    public SDEquipDetail equipDetail;
    public bool isClicking;
    private float clickEndTime = 0.3f;
    public Button[] AllBtns = new Button[3];
    [Header("INFOR")]
    public Transform DescPanel;
    public Text equipDiscText;
    [Header("LVUP")]
    public PerConsumableVision[] AllLvupArray = new PerConsumableVision[3];
    [System.Serializable]
    public class PerConsumableVision
    {
        public consumableItem Item;
        public Text BtnText;
        public Button Btn;
        public Image Icon;
        public int Number;
        public Text ItemDetailText;
        public void initThisItem()
        {
            BtnText.text = SDGameManager.T(Item.Data);
            Icon.sprite = Item.IconFromAtlas;
            refreshThisItemDetail();
        }
        public void refreshThisItemDetail()
        {
            Number = SDDataManager.Instance.getConsumableNum(Item.ID);
            ItemDetailText.text = "= " + Number;
        }
    }
    public int lvupTypeIndex;
    private float btnAnimTime = 0.2f;
    [Header("FIX")]
    public consumableItem ItemForFix;
    public Image Icon_Fix;
    public Text Text_Fix;
    [Space]
    public Transform EmptyPanel;
    void refreshPanel()
    {
        refreshPanel_infor();
        refreshPanel_lvup();
        refreshPanel_fix();
    }
    IEnumerator IEClickEnd()
    {
        yield return new WaitForSeconds(clickEndTime);
        isClicking = false;
    }
    #region _infor
    public void btnToInfor()
    {
        if (!isClicking)
        {
            isClicking = true;
            if (!DescPanel.gameObject.activeSelf)
            {
                UIEffectManager.Instance.showAnimFadeIn(DescPanel);
            }
            else
            {
                UIEffectManager.Instance.hideAnimFadeOut(DescPanel);
            }
            StartCoroutine(IEClickEnd());
            refreshPanel_infor();
            int hc = equipDetail.equipHashcode;
            equipDetail.initEquipDetailVision(hc);
        }
    }
    void refreshPanel_infor()
    {
        EquipItem item = SDDataManager.Instance.GetEquipItemById(equipDetail.equipId);
        if (item == null) return;
        equipDiscText.text = item.DESC;
    }
    #endregion

    #region _lvup
    public void btnToLvUp()
    {
        if (!isClicking)
        {
            isClicking = true;
            //
            consumableItem item = AllLvupArray[lvupTypeIndex].Item;
            int ss = SDDataManager.Instance.getInteger(item.SpecialStr);
            GDEEquipmentData equip = SDDataManager.Instance.getEquipmentByHashcode
                (equipDetail.equipHashcode);
            if (equip == null)
            {
                StartCoroutine(IEClickEnd());
                return;
            }

            bool flag = SDDataManager.Instance.consumeConsumable
                (item.ID,out int residue, SDConstants.MinExpPerLevel);
            if (!flag)
            {
                StartCoroutine(IEClickEnd());return;
            }

            int currentLv = equip.lv;
            float rate = 1;
            float change = ss * 1f / 100;
            if (currentLv < SDConstants.equipMaxPreferLv)
            {
                for(int i = 0; i < currentLv; i++)
                {
                    rate *= change;
                }
            }
            else
            {
                rate = 0.5f;
                change *= 0.5f;
                for (int i = 0; i < currentLv; i++)
                {
                    rate *= change;
                }
            }
            float r = UnityEngine.Random.Range(0, 1);
            if (r < rate)
            {
                Debug.Log("装备升级成功");
                SDDataManager.Instance.LvupEquipByHashcode(equipDetail.equipHashcode);
            }
            else
            {
                Debug.Log("装备升级失败");
            }
            refreshPanel_lvup();
            int hc = equipDetail.equipHashcode;
            equipDetail.initEquipDetailVision(hc);
            StartCoroutine(IEClickEnd());
        }
    }

    void refreshPanel_lvup()
    {
        for(int I = 0; I < AllLvupArray.Length; I++)
        {
            if (I == lvupTypeIndex)
            {
                AllLvupArray[I].refreshThisItemDetail();
                AllLvupArray[I].Btn.transform.localScale = Vector3.zero;
            }
            else
            {
                AllLvupArray[I].Btn.transform.localScale = Vector3.one;
            }
        }
        consumableItem item = AllLvupArray[lvupTypeIndex].Item;
        int num = AllLvupArray[lvupTypeIndex].Number;
        if (num >= SDConstants.MinExpPerLevel)
        {
            AllBtns[1].interactable = true;
        }
        else AllBtns[1].interactable = false;
    }
    public void btn_changeLvupType(int index)
    {
        if (index >= AllLvupArray.Length || index < 0) return;
        lvupTypeIndex = index;
        refreshPanel_lvup();
    }
    #endregion

    #region _fix
    public void btnToFix()
    {
        if (!isClicking)
        {
            isClicking = true;
            int num = SDDataManager.Instance.getConsumableNum(ItemForFix.ID);
            GDEEquipmentData equip = SDDataManager.Instance.getEquipmentByHashcode
                (equipDetail.equipHashcode);
            if (num > 0)
            {
                bool flag =SDDataManager.Instance.PromoteEquipQuality
                    (equipDetail.equipHashcode, 1);
                if (flag)
                {
                    SDDataManager.Instance.consumeConsumable(ItemForFix.ID, out int reduice, 1);
                    refreshPanel_fix();
                    int hc = equipDetail.equipHashcode;
                    equipDetail.initEquipDetailVision(hc);
                }
            }
            StartCoroutine(IEClickEnd());
        }
    }
    void refreshPanel_fix()
    {
        int num = SDDataManager.Instance.getConsumableNum(ItemForFix.ID);
        GDEEquipmentData equip = SDDataManager.Instance.getEquipmentByHashcode
            (equipDetail.equipHashcode);
        if (equip == null)
        {
            AllBtns[2].interactable = false;return;
        }
        if(equip.quality<SDConstants.equipMaxQuality && num > 0)
        {
            AllBtns[2].interactable = true;
        }
        else
        {
            AllBtns[2].interactable = false;
        }
    }
    #endregion

    public void whenOpenThisPanel()
    {
        foreach(PerConsumableVision v in AllLvupArray)
        {
            v.initThisItem();
        }
        EmptyPanel.gameObject.SetActive(true);
        isClicking = false;
        refreshPanel();
    }
    public void whenCloseThisPanel()
    {
        //equipImprove.CloseThisPanel();
    }
}
