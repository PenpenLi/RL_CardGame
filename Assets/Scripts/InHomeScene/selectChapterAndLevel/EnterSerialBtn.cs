using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnterSerialBtn : MonoBehaviour
{
    public int LocalSerialIndex;
    [Space(15)]
    public Text SerialText;
    public int startLevelIndex
    {
        get { return LocalSerialIndex * SDConstants.LevelNumPerSerial; }
    }
    public int finishLevelIndex
    {
        get { return LocalSerialIndex * SDConstants.LevelNumPerSerial
                + SDConstants.LevelNumPerSerial; }
    }
    public Image BtnMainImg;
    public Sprite EnableSprite;
    public Sprite UnableSprite;
    private bool _isLocked;
    LevelEnterPanel LEP;
    public bool IsLocked
    {
        get { return _isLocked; }
        set 
        {
            _isLocked = value;
            BtnMainImg.sprite = _isLocked ? UnableSprite : EnableSprite;
        }
    }
    public void RefreshBtnStatus(int chapterIndex)
    {
        int _SerialIndex = chapterIndex * SDConstants.SerialNumPerChapter + LocalSerialIndex;
        int maxSection = SDDataManager.Instance.GetMaxPassSection();
        int maxSerial = maxSection % SDConstants.SectionNumPerSerial 
            != SDConstants.SectionNumPerSerial-1
            ? (maxSection - (maxSection % SDConstants.SectionNumPerSerial))
            / SDConstants.SectionNumPerSerial: (maxSection - 
            (maxSection % SDConstants.SectionNumPerSerial))
            / SDConstants.SectionNumPerSerial + 1;
        string n = string.Format("{0:D3}-{1:D3}", startLevelIndex, finishLevelIndex);
        SerialText.text = n;
        IsLocked = maxSerial < _SerialIndex;
    }
    public void BtnTapped()
    {
        if (IsLocked) return;
        LEP = LEP == null ? GetComponentInParent<LevelEnterPanel>() : LEP;
        LEP.enterSelectedSerial(LocalSerialIndex);
    }
}
