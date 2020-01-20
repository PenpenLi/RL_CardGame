using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

public class DrawAnimRound : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler
{
    public bool AnimOn;
    public bool startAnim;
    public RectTransform AnimRound;
    public bool useLR;
    public Transform lightRoundList;
    Image[] lightRound = new Image[0];
    public Color LRColor;
    public Image middleLight;
    Color ml_baseColor;
    public float EulerBackSpeed;
    public float MToRRate;
    public float RMaxEuler;
#if UNITY_EDITOR
    [SerializeField,ReadOnly]
#endif
    private float RoundAngle;
    Vector2 startDragPos;
    float BeginDragTime;
    public float DragTime;
    public SummonAltarPanel.AltarType CurrentAltarType;
    [SerializeField]
    public UnityEvent OnAnimEndSetEvent = new UnityEvent();
    public SummonAltarPanel SAP
    {
        get { return GetComponentInParent<SummonAltarPanel>(); }
    }
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
        if (!startAnim)
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
        //StartAltar();
        //
        Debug.Log("ReachTarget2");
        yield return new WaitForSeconds(0.5f);
        BackToBase();
        middleLight.DOColor(ml_baseColor, 0.5f);
        foreach(Image c in lightRound)
        {
            c.DOColor(LRColor, 0.5f);
        }
    }
    public void StartAltar()
    {
        if(CurrentAltarType == SummonAltarPanel.AltarType.n_oneTime)
        {
            SAP.confirm_altar_n_oneTime();
        }
        else if(CurrentAltarType == SummonAltarPanel.AltarType.n_tenTime)
        {
            SAP.confirm_altar_n_tenTimes();
        }
        else if(CurrentAltarType == SummonAltarPanel.AltarType.r_oneTime)
        {
            SAP.confirm_altar_r_oneTime();
        }
    }
    public void clearOAESE()
    {
        OnAnimEndSetEvent = new UnityEvent();
    }
    private void FixedUpdate()
    {
        if (!AnimOn) return;
        AnimRound.eulerAngles = new Vector3(0, 0, RoundAngle);
    }



    public void ResetSelf()
    {
        AnimRound.eulerAngles = Vector3.zero;
        startAnim = false;
        middleLight.color = ml_baseColor;
        if (lightRound.Length > 0)
        {
            for(int i = 0; i < lightRound.Length; i++)
            {
                lightRound[i].color = LRColor;
            }
        }
    }
}
