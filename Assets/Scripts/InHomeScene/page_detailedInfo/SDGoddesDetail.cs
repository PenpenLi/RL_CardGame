using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

public class SDGoddesDetail : MonoBehaviour
{
    public SDConstants.CharacterType type = SDConstants.CharacterType.Goddess;
    public string Id;
    public List<int> UsedTeamIds;
    [Header("守护者信息可视化")]
    public Image goddessCharacterDrawingImg;
    public Image raceImg;
    public Text raceText;
    public Text nameText;
    //public Text rarityText;
    public int lv;
    public int quality;
    public ItemStarVision starNumVision;
    public Transform expSlider;
    public Transform volumeSlider;
    //public Text volumeText;
    public simpleSlotSet[] RuneEquipList = new simpleSlotSet[4];
    [Space]
    public Transform lockedPanel;
    public Transform lockedPanelSlider;
    [SerializeField, ReadOnly]
    private bool _islocked;
    public bool isLocked
    {
        get { return _islocked; }
        set
        {
            _islocked = value;
            lockedPanel.gameObject.SetActive(_islocked);
        }
    }
    public Transform emptyPanel;
    [SerializeField,ReadOnly]
    private bool _isEmpty;
    public bool isEmpty
    { 
        get { return _isEmpty; }
        set
        {
            _isEmpty = value;emptyPanel.gameObject.SetActive(_isEmpty);
        }
    }
    //
    public GoddessDetailPanel GDP
    {
        get { return GetComponentInParent<GoddessDetailPanel>(); }
    }

    public void initgoddessDetailVision(GDEgoddessData goddess)
    {
        isEmpty = false;
        GoddessInfo info = SDDataManager.Instance.getGoddessInfoById(goddess.id);
        Id = goddess.id;
        int integrity = SDDataManager.Instance.getIntegrityByVolume(goddess.volume,info.Quality);
        if (integrity < 1)
        {
            isLocked = true;
            lockedPanelSlider.localScale = new Vector3
                (SDDataManager.Instance.getRateAppraochIntegrity(goddess.volume, info.Quality), 1, 1);
            return;
        }
        isLocked = false;
        lv = SDDataManager.Instance.getLevelByExp(goddess.exp);
        nameText.text = SDGameManager.T("Lv.") + lv+"·"+info.name;
        quality = info.Quality;
        //rarityText.text = SDDataManager.Instance.rarityString(quality);
        //
        expSlider.localScale = new Vector3
            (SDDataManager.Instance.getExpRateByExp(goddess.exp), 1, 1);
        volumeSlider.localScale = new Vector3
            (SDDataManager.Instance.getRateAppraochIntegrity(goddess.volume, info.Quality), 1, 1);

        //
        if(SDDataManager.Instance.getRuneEquippedByPosAndGoddess
            (0,goddess.id,out GDERuneData data0))
        {
            RuneEquipList[0].initRune(data0);
        }
        else
        {
            RuneEquipList[0].isEmpty = true;
        }
        if (SDDataManager.Instance.getRuneEquippedByPosAndGoddess
            (1, goddess.id, out GDERuneData data1))
        {
            RuneEquipList[1].initRune(data1);
        }
        else
        {
            RuneEquipList[1].isEmpty = true;

        }
        if (SDDataManager.Instance.getRuneEquippedByPosAndGoddess
            (2, goddess.id, out GDERuneData data2))
        {
            RuneEquipList[2].initRune(data2);
        }
        else
        {
            RuneEquipList[2].isEmpty = true;
        }
        if (SDDataManager.Instance.getRuneEquippedByPosAndGoddess
            (3, goddess.id, out GDERuneData data3))
        {
            RuneEquipList[3].initRune(data3);
        }
        else
        {
            RuneEquipList[3].isEmpty = true;
        }

        for(int i = 0; i < RuneEquipList.Length; i++)
        {
            simpleSlotSet slot = RuneEquipList[i];
            slot.resetOnBtnTapped();
            slot.OnBtnTapped += GDP.initRuneEquipAndSetPanel;
        }
    }
}
