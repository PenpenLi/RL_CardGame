using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;
public class RuneDetailPanel : ItemDetailPanel
{
    [Space(25)]
    public ItemStarVision starVision;
    public string ownerId;
    public int quality;
    public int level;
    public bool locked;
    public RuneDetailPanel() { itemType = SDConstants.ItemType.Rune; }

    public void initDetailPanel(GDERuneData rune)
    {
        RuneItem R = SDDataManager.Instance.getRuneItemById(rune.id);
        ownerId = rune.ownerId;
        locked = rune.locked;
        id = rune.id;
        hashcode = rune.hashcode;
        if(itemNameText) itemNameText.text = R.NAME;
        if(itemDescText) itemDescText.text = R.DESC;
        quality = R.Quality;
        level = rune.level;
        if(itemExtraText)
            itemExtraText.text = SDGameManager.T("Lv.") + level + "·"
                + SDDataManager.Instance.rarityString(quality);
        starVision.StarNum = quality;
        //priceText.text = ""+SDDataManager.Instance.getCoinWillImproveCost(level, quality);
    }
    public void initDetailPanel(int hashcode)
    {
        GDERuneData rune = SDDataManager.Instance.getRuneOwnedByHashcode(hashcode);
        if (rune != null) initDetailPanel(rune);
        else Debug.Log("initing_rune_detail_fails : get null by hashcode");
    }
}
