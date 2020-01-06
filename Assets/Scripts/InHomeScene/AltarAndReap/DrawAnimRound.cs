using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class DrawAnimRound : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler
{
    public bool AnimOn;
    public bool startAnim;
    public RectTransform AnimRound;
    public bool useLR;
    public Transform lightRoundList;
    Image[] lightRound;
    public Color LRColor;
    public Image middleLight;
    Color ml_baseColor;
    public float EulerBackSpeed;
    public float MToRRate;
    public float RMaxEuler;
    [SerializeField,ReadOnly]
    private float RoundAngle;
    Vector2 startDragPos;
    float BeginDragTime;
    public float DragTime;
    public void OnBeginDrag(PointerEventData data)
    {
        if (startAnim) return;
        ml_baseColor = middleLight.color;
        lightRound = lightRoundList.GetComponentsInChildren<Image>();
        BeginDragTime = Time.time;
        startDragPos = Camera.main.ScreenToWorldPoint(data.pressPosition);
    }
    public void OnDrag(PointerEventData data)
    {
        if (startAnim) return;
        else if (!startAnim)
        {
            float _time = Time.time - BeginDragTime;
            Vector2 Pos = Camera.main.ScreenToWorldPoint(data.position);
            Vector2 v0 = startDragPos - (Vector2)AnimRound.position;
            Vector2 v1 = Pos - (Vector2)AnimRound.position;
            float e = Vector2.SignedAngle(v0, v1);
            if (e > 10 || _time > DragTime)
            {
                startAnim = true; BackToBase();
            }
            RoundAngle = e * MToRRate;
            if (useLR)
            {
                RefreshAllLR();
            }
            if (Mathf.Abs(RoundAngle) > RMaxEuler)
            {
                startAnim = true;
                StartCoroutine(RoundFinishAnimStart());
            }
        }
    }
    public void RefreshAllLR()
    {
        float M = 360f / lightRound.Length;
        for(int i = 0; i < lightRound.Length; i++)
        {
            if (Mathf.Abs(RoundAngle) >= M * i && lightRound[i].color!=Color.white)
            {
                lightRound[i].DOColor(Color.white, 0.5f);
            }
        }
    }
    public void OnEndDrag(PointerEventData data)
    {
        if (!startAnim)
        {
            BackToBase();
        }
    }
    void BackToBase()
    {
        StartCoroutine(IEBackToBase());
    }
    IEnumerator IEBackToBase()
    {
        float T = RoundAngle / EulerBackSpeed;
        DOTween.To(() => RoundAngle, x
                => RoundAngle = x, 0, T)
                .SetEase(Ease.OutBack);
        yield return new WaitForSeconds(T);
        startAnim = false;
    }
    IEnumerator RoundFinishAnimStart()
    {
        middleLight.DOColor(Color.white,0.5f);
        yield return new WaitForSeconds(0.5f);
        Debug.Log("ReachTarget");
        OnAnimEndSetEvent?.Invoke();
        yield return new WaitForSeconds(0.5f);
        BackToBase();
        middleLight.DOColor(ml_baseColor, 0.5f);
        foreach(Image c in lightRound)
        {
            c.DOColor(LRColor, 0.5f);
        }
    }
    public void clearOAESE()
    {
        OnAnimEndSetEvent = null;
    }
    private void FixedUpdate()
    {
        if (!AnimOn) return;
        AnimRound.eulerAngles = new Vector3(0, 0, RoundAngle);
    }

    public event SimpleTriggerListener OnAnimEndSetEvent;
}
