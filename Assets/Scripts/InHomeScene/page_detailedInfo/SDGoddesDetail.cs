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
    public Text rarityText;
    public int quality;
    public ItemStarVision starNumVision;
    public Text lvText;
    public Transform expSlider;


    public void initgoddessDetailVision(GDEgoddessData goddess)
    {
        RoGoddessData G = SDDataManager.Instance.getGoddessData(goddess);
        nameText.text = G.name;
        lvText.text = SDGameManager.T("Lv.") + G.lv;
        quality = G.quality;
        rarityText.text = SDDataManager.Instance.rarityString(quality);
    }
}
