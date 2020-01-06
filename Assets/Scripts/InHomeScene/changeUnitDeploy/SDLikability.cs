using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

public class SDLikability : MonoBehaviour
{
    public List<Transform> AllHeartSliders;
    public SDHeroDetail heroDetail;
    public int heartLevel = 0;

    public void resetThisPanel()
    {
        for(int i = 0; i < AllHeartSliders.Count; i++)
        {
            AllHeartSliders[i].localScale = Vector3.up + Vector3.forward;
        }
    }
    public void initLikabilityVision()
    {
        resetThisPanel();
        int likabiliyExp 
            = SDDataManager.Instance.GetHeroOwnedByHashcode(heroDetail.Hashcode).likability;
        heartLevel = 0;
        if (likabiliyExp> SDConstants.MinHeartVolume)
        {
            heartLevel = 1;
            AllHeartSliders[0].localScale = Vector3.one;
            if(likabiliyExp > SDConstants.MinHeartVolume * 3.5f)
            {
                heartLevel = 2;
                AllHeartSliders[1].localScale = Vector3.one;
                float a = Mathf.Min((likabiliyExp - SDConstants.MinHeartVolume * 3.5f) * 1f
                    / (SDConstants.MinHeartVolume * 5f),1);
                AllHeartSliders[2].localScale = new Vector3(a, 1, 1);
                if(likabiliyExp >= SDConstants.MinHeartVolume * 8.5f)
                {
                    heartLevel = 3;
                }
            }
            else
            {
                float a = (likabiliyExp - SDConstants.MinHeartVolume) * 1f
                    / (SDConstants.MinHeartVolume * 2.5f);
                AllHeartSliders[1].localScale = new Vector3(a, 1, 1);
            }
        }
        else
        {
            float a = likabiliyExp * 1f / SDConstants.MinHeartVolume;
            AllHeartSliders[0].localScale = new Vector3(a, 1, 1);
        }
    }
}
