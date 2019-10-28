using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

public class RTBagItemDetail : MonoBehaviour
{
    public SDConstants.BagItemType type;
    public SDConstants.ItemType equipType;
    public SingleItem equipItem;
    public RTSingleBagItem bagItem;
    public Text itemName;
    public Text itemType;
    public Image itemImg;
    public Text itemDes;
    public Text levelText;
    public RTEquipUpTimes itemUpTimes;
    public Image frameImg;
    public Transform statusPanel;
    public Text itemHp;
    public Text itemAtk;
    public Text itemMp;
    public Text itemMgDamage;

    public Text sellOneText;
    public Text sellOneTitleText;
    public Transform sellOnePrice;
    public Text sellAllText;
    public Button sellAllBtn;
    public Button useBtn;

    public int sellPrice;
    public RTBagPanel bagPanel;
    private bool canUseInGame = false;
    public string specialStr;
    public string propFunctionName;
    public int param;
    public Transform heroesSelectPanel;
    public Text suitCareerText;

    public Transform summonBrn;
    public Transform resolveBtn;
    public Transform getHeroAnim;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void initEquipItemDetail()
    {

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
