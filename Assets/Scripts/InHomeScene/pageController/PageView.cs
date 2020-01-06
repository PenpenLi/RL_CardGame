using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public class PageView : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    public ScrollRect rect;
    public int currentIndex;
    [HideInInspector]
    public int minIndex;
    //[HideInInspector]
    public int maxIndex;
    public float minMoveHorizontal;
    [HideInInspector]
    public float startHorizontal;
    [HideInInspector]
    public float endHorizontal;
    [SerializeField]
    public UnityEvent DragStartEvent = new UnityEvent();
    [SerializeField]
    public UnityEvent DragEndEvent = new UnityEvent();

    #region 滑动切换梯队
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (rect == null) rect = GetComponent<ScrollRect>();
        startHorizontal = rect.horizontalNormalizedPosition;
        DragStartEvent?.Invoke();
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (rect == null) rect = GetComponent<ScrollRect>();
        minIndex = 0;
        //maxIndex = SDConstants.MaxBattleTeamNum;
        endHorizontal = rect.horizontalNormalizedPosition;
        if (endHorizontal - startHorizontal > minMoveHorizontal)//右移
        {
            currentIndex = Mathf.Clamp(currentIndex + 1, minIndex, maxIndex);
            DragEndEvent?.Invoke();
        }
        else if(endHorizontal - startHorizontal < -minMoveHorizontal)//左移
        {
            currentIndex = Mathf.Clamp(currentIndex - 1, minIndex, maxIndex);
            DragEndEvent?.Invoke();
        }
        else//不变
        {

        }
        rect.horizontalNormalizedPosition = startHorizontal;
    }
    #endregion
}
