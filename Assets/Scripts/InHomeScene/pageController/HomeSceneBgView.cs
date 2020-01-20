using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HomeSceneBgView : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    public ScrollRect rect;
    [SerializeField]
    private int currentIndex;
    public int CurrentIndex
    {
        get { return currentIndex; }
        set 
        {
            currentIndex = value;
            showEndVisual(currentIndex);
        }
    }
#if UNITY_EDITOR
    [ReadOnly]
#endif
    public float floatData;
    //
    public int minIndex;
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
    [Header("VisualSign")]
    public Transform leftSign;
    public Transform rightSign;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (rect == null) rect = GetComponent<ScrollRect>();
        startHorizontal = rect.horizontalNormalizedPosition;
        DragStartEvent?.Invoke();
        Debug.Log("start_drag");
    }
   public void OnEndDrag(PointerEventData eventData)
    {
        if (rect == null) rect = GetComponent<ScrollRect>();
        endHorizontal = rect.horizontalNormalizedPosition;
        if(endHorizontal - startHorizontal > minMoveHorizontal)//右移
        {
            CurrentIndex = Mathf.Clamp(currentIndex + 1, minIndex, maxIndex);
            DragEndEvent?.Invoke();
        }
        else if(endHorizontal - startHorizontal < - minMoveHorizontal)//左移
        {
            CurrentIndex = Mathf.Clamp(currentIndex - 1, minIndex, maxIndex);
            DragEndEvent?.Invoke();
        }
        else
        {

        }
        startHorizontal = rect.horizontalNormalizedPosition;
        Debug.Log("end_drag");
    }

    void showEndVisual(int index)
    {
        int main = maxIndex - minIndex + 1;
        float per = 1f / (main-1);
        int current = index - minIndex;
        if (rect == null) rect = GetComponent<ScrollRect>();
        rect.horizontalNormalizedPosition = per * current;
        //
        if (index > minIndex) { leftSign.gameObject.SetActive(true); }
        else leftSign.gameObject.SetActive(false);
        //
        if (index < maxIndex) { rightSign.gameObject.SetActive(true); }
        else rightSign.gameObject.SetActive(false);
        //
        floatData = rect.horizontalNormalizedPosition;
    }
}
