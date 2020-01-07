using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

public class simpleSlotSet : MonoBehaviour
{
    [SerializeField]
    private int index;
    public int Index { get { return index; } }

    public string id;
    public int hashcode;
    public int exp;

    public Image selectedImg;
    public Image lockedImg;
    public Image emptyImg;
    #region isSeleced;
    private bool _isSelected;
    public bool isSelected
    {
        get { return _isSelected; }
        set
        {
            _isSelected = value;
            selectedImg.gameObject.SetActive(_isSelected);
        }
    }
    #endregion
    #region isLocked
    private bool _isLocked;
    public bool isLocked
    {
        get { return _isLocked; }
        set
        {
            _isLocked = value;
            lockedImg.gameObject.SetActive(_isLocked);
        }
    }
    #endregion
    #region isEmpty
    private bool _isEmpty;
    public bool isEmpty
    {
        get { return _isEmpty; }
        set
        {
            _isEmpty = value;
            emptyImg.gameObject.SetActive(_isEmpty);
        }
    }
    #endregion
    public Image itemImg;
    public Image frameImg;
    public Text upText;
    public Text downText;
    public SDConstants.ItemType type;
    public ItemStarVision starVision;

    public delegate void ClickListener(int index);
    public event ClickListener OnBtnTapped;

    public void BtnTapped()
    {
        OnBtnTapped?.Invoke(index);
    }
    public void resetOnBtnTapped()
    {
        OnBtnTapped = null;
    }

    public void initRune(GDERuneData rune)
    {
        if (rune == null || rune.Hashcode <= 0)
        {
            isEmpty = true;
            hashcode = 0;
            id = string.Empty;
            exp = 0;

            return;
        }
        isEmpty = false;
        if (upText)
        {
            upText.text = SDGameManager.T("Lv.") + rune.level;
        }
        if (starVision)
        {
            starVision.StarNum = rune.star;
        }
        hashcode = rune.Hashcode;
        id = rune.id;
    }
}
