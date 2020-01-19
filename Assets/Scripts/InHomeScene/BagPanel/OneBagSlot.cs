using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

public class OneBagSlot : MonoBehaviour
{
    #region unlocked
    public Transform lockedPanel;
    bool _islocked = false;
    public bool Islocked
    {
        get { return _islocked; }
        set
        {
            if(_islocked != value)
            {
                _islocked = value;
                lockedPanel.gameObject.SetActive(_islocked);
            }
        }
    }
    #endregion
    #region empty
    public Transform emptyPanel;
    bool _isEmpty = false;
    public bool IsEmpty
    {
        get { return _isEmpty; }
        set
        {
            if(_isEmpty != value)
            {
                _isEmpty = value;
                emptyPanel.gameObject.SetActive(_isEmpty);
            }
        }
    }
    #endregion
    #region select
    public Transform selectPanel;
    bool _isSelected = false;
    public bool IsSelected
    {
        get { return _isSelected; }
        set 
        { 
            if (_isSelected != value)
            {
                _isSelected = value;
                selectPanel?.gameObject.SetActive(_isSelected);
            }   
        }
    }
    #endregion
    #region useType
    [SerializeField,ReadOnly]
    BagController.useType _cut = BagController.useType.change;
    public BagController.useType currentUseType
    {
        get { return _cut; }
        set
        {
            if(_cut != value)
            {
                _cut = value;

            }
        }
    }
    #endregion
    public Image propIcon;
    public Image propBgIcon;
    public Text nameText;
    public Text numText;
    public string propId;
    public int num;
    public int index;
    public BagController bag
    {
        get { return GetComponentInParent<BagController>(); }
    }
    public void initSlot(BagController.useType use_type , GDEItemData prop)
    {
        currentUseType = use_type;
        if(currentUseType == BagController.useType.end)
        {
            Islocked = true;
        }
        else
        {
            Islocked = false;
            if(string.IsNullOrEmpty(prop.id))
            {
                IsEmpty = true;
            }
            else
            {
                IsEmpty = false;
                propId = prop.id;
                num = prop.num;

                consumableItem D = SDDataManager.Instance.getConsumableById(propId);
                nameText.text = D.NAME;
                numText.text = "" + num;
            }
        }
    }

    public void BtnTapped()
    {
        if(currentUseType == BagController.useType.change)
        {
            Item_change();
        }
        else if(currentUseType == BagController.useType.use)
        {
            Item_use();
        }
    }
    public void Item_change()
    {
        bag.item_change(this);
    }
    public void Item_use()
    {
        bag.item_use(this);
    }
}
