using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

public class oneStockController : MonoBehaviour
{
    [Header("LeftPart")]
    public Image bgIcon;
    public Image frameIcon;
    public Image icon;
    public ItemStarVision starVision;
    public Text upText;
    public Text downText;
    public Transform emptyPanel;
    public Transform lockPanel;
    //
    [Header("RightPart")]
    public Text UsedNumText;
    public Button btn_add;
    public Button btn_reduce;
    [Header("Data")]
    public int CurrentNum;
    public string itemId;
    public void btnTapped_add()
    {
        //if (Item.type != SDConstants.ItemType.Consumable) return;
        GDEItemData data = SDDataManager.Instance.getConsumeableDataById(itemId);
        int num = data.num;
        if (CurrentNum < num) CurrentNum++;
        refreshSelf();
    }
    public void btnTapped_reduce()
    {
        if (CurrentNum > 0) CurrentNum--;
        refreshSelf();
    }
    public void refreshSelf()
    {
        UsedNumText.text = "" + CurrentNum;
        if (CurrentNum <= 0) btn_reduce.interactable = false;
        else btn_reduce.interactable = true;
        //
        GDEItemData data = SDDataManager.Instance.getConsumeableDataById(itemId);
        int num = data.num;
        //
        if (CurrentNum >= num) btn_add.interactable = false;
        else btn_add.interactable = true;

        //
        consumableItem item = SDDataManager.Instance.getConsumableById(itemId);
        //
        icon.sprite = item.IconFromAtlas;
        bgIcon.sprite = SDDataManager.Instance.baseBgSpriteByRarity(item.LEVEL);
        frameIcon.sprite = SDDataManager.Instance.baseFrameSpriteByRarity(item.LEVEL);
        if (starVision) { starVision.StarNum = item.LEVEL; }
        upText?.gameObject.SetActive(false);
        downText.text = "X" + data.num;
    }

    public void Init(string id)
    {
        itemId = id;
        CurrentNum = 0;
        refreshSelf();
    }

}
