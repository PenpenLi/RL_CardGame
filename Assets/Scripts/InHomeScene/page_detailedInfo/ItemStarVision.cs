using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemStarVision : MonoBehaviour
{
    int _starNum = 0;
    public int StarNum
    {
        get { return _starNum; }
        set
        {
            int s = _starNum;
            _starNum = value;
            refreshStarNum(s);
        }
    }
    public Transform AllStarParent;
    public bool isInMiddle;
    private void Start()
    {
        AllStarParent = transform;
        DebugSelf();
        refreshStarNum(0);
    }
    public void refreshStarNum(int oldNum)
    {
        if (AllStarParent == null) AllStarParent = transform;
        HorizontalLayoutGroup HLG = GetComponent < HorizontalLayoutGroup > ();
        if (!isInMiddle)
        {
            HLG.childAlignment = TextAnchor.MiddleLeft;
            for (int i = 0; i < AllStarParent.childCount; i++)
            {
                if (i < StarNum)
                {
                    AllStarParent.GetChild(i).GetComponent<CanvasGroup>().alpha = 1;
                }
                else
                {
                    AllStarParent.GetChild(i).GetComponent<CanvasGroup>().alpha = 0;
                }
            }
        }
        else
        {
            HLG.childAlignment = TextAnchor.MiddleCenter;
            for (int i = 0; i < AllStarParent.childCount; i++)
            {
                if (i < StarNum)
                {
                    //AllStarParent.GetChild(i).GetComponent<CanvasGroup>().alpha = 1;
                    AllStarParent.GetChild(i).gameObject.SetActive(true);
                }
                else
                {
                    //AllStarParent.GetChild(i).GetComponent<CanvasGroup>().alpha = 0;
                    AllStarParent.GetChild(i).gameObject.SetActive(false);
                }
            }
        }

        
    }

    void DebugSelf()
    {
        ContentSizeFitter CSF = GetComponent<ContentSizeFitter>();
        if (CSF)
        {
            CSF.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        }
        float d = 0;
        for(int i = 0; i < transform.childCount; i++)
        {
            RectTransform rt = transform.GetChild(i).GetComponent<RectTransform>();
            if (rt.sizeDelta.x <= 1) rt.sizeDelta = new Vector2(20, rt.sizeDelta.y);
            d += rt.sizeDelta.x;
        }
        RectTransform RT = GetComponent<RectTransform>();
        RT.sizeDelta = new Vector2(d, RT.sizeDelta.y);
        if (CSF)
        {
            CSF.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
    }
}
