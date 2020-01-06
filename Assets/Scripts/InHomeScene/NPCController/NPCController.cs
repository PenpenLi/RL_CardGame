using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCController : MonoBehaviour
{
    public int currentDialogueIndex;
    [DisplayName("NPC对话云")]
    public Transform DialogueCloud;
    public Text dialogueNameText;
    public Text dialogueContentText;
    //
    public Image NPCFace;
    public Talker talker;
    [Header("Extra")]
    public Transform ExtraTrans;

    public void Init()
    {
        if (talker == null) return;
        TalkerInfo info = talker.Info;
        showDalogueData(info.DefaultDialogue);
        //
        currentDialogueIndex = UnityEngine.Random.Range(0, info.DefaultDialogue.Words.Count);
        UIEffectManager.Instance.showAnimFadeIn(DialogueCloud);
        //
        if (ExtraTrans) ExtraTrans.gameObject.SetActive(false);
    }
    public void showDalogueData(Dialogue dialogue)
    {
        DialogueWords words = dialogue.Words[currentDialogueIndex];
        //
        dialogueNameText.text = words.TalkerName + ":";
        dialogueContentText.text = words.Words;
    }


    public void BtnTapped()
    {
        if (talker == null) return;
        TalkerInfo info = talker.Info;
        if (info.DefaultDialogue.Words[currentDialogueIndex].IsValid)
        {


            return;
        }

        if (!ExtraTrans) return;
        UIEffectManager.Instance.showAnimFadeIn(ExtraTrans);
        //

    }

    public void ExtraTransClose()
    {
        UIEffectManager.Instance.hideAnimFadeOut(ExtraTrans);
    }
}
