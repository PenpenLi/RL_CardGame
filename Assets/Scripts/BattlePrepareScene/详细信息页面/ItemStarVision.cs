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
        refreshStarNum(0);
    }
    public void refreshStarNum(int oldNum)
    {
        if (AllStarParent == null) AllStarParent = transform;
        if(!isInMiddle)
        {
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
            for (int i = 0; i < AllStarParent.childCount; i++)
            {
                if (i < StarNum)
                {
                    AllStarParent.GetChild(i).GetComponent<CanvasGroup>().alpha = 1;
                    AllStarParent.GetChild(i).localScale = Vector3.one;
                }
                else
                {
                    AllStarParent.GetChild(i).GetComponent<CanvasGroup>().alpha = 0;
                    AllStarParent.GetChild(i).localScale = Vector3.zero;
                }
            }
        }

        
    }
}
