using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

/// <summary>
/// 弹窗工具
/// </summary>
public class PopoutController : MonoBehaviour
{
    public enum PopoutWIndowAlertType
    {
        MessageDismissWithDelay,
        ConfirmMessage,
        ConfirmOrCancel,
    }
    public enum PopoutWindowAlertActionType
    {
        OnDismiss,
        OnConfirm,
        OnCancel,
    }

    public static string popoutPrefabName = "Windows/PopoutConfirm";
    public static List<PopoutController> pendingPopouts = new List<PopoutController>();
    public static bool isShowingPopout = false;
    public static UnityEngine.Object popout0 = null;
    public static UnityEngine.Object popout1 = null;
    public static UnityEngine.Object popoutMessage = null;

    public Canvas rootCanvas;
    public CanvasGroup alertWindow;
    public Image blackLayer;
    public Image panelBgImg;
    public Image panelBgImg1;
    public Text titleText;
    public Text messageText;
    //public Text woodNumText;
    //public Text 
    public Button confirmBtn;
    public Button cancelBtn;
    //public Button confirmBtn;

    public bool _dismissAfterTapBtn = true;
    public bool preventMultipleTaps = true;
    public bool deactiveWhenDismiss = false;
    public bool autoResizeText = true;

    public Action<PopoutController, PopoutWindowAlertActionType> _action;
    public string _title;
    public string _message;
    public PopoutWIndowAlertType _TYPE;
    public PopoutWIndowAlertType _type
    {
        get { return _TYPE; }
        set 
        {
            if(_TYPE != value)
            {
                _TYPE = value;
                //refreshBtnsInPanel(_TYPE);
            }
        }
    }
    public bool _isDoingAction = false;
    [HideInInspector]
    public Vector3 fadeEndScale = Vector3.right;

    public static PopoutController CreatePopoutMessage(string message,int sortinggoder
        ,Action<PopoutController,PopoutWindowAlertActionType> action = null)
    {
        string name = popoutPrefabName;
        UnityEngine.Object prefab = null;
        name = "Windows/PopoutMessage";
        if (popoutMessage == null)
        {
            Debug.Log("Loading popout file " + name);
            popoutMessage = Resources.Load(name);

        }
        prefab = popoutMessage;

        Debug.Log("Loading file " + name);
        GameObject tf = Instantiate(prefab) as GameObject;
        PopoutController pop = tf.GetComponent<PopoutController>();
        pop.SetupMsgFlow("", message, sortinggoder, true
            , PopoutWIndowAlertType.MessageDismissWithDelay
            , action);
        isShowingPopout = false;
        //SDGameManager.Instance.audio
        return pop;
    }
    public static PopoutController CreateDamagePopoutMessage(string fontname, Vector3 pos
        ,string message, int sortingoder
        , Action<PopoutController,PopoutWindowAlertActionType> action = null)
    {
        string name = popoutPrefabName;
        UnityEngine.Object prefab = null;
        name = "Windows/PopoutDamageMessage";
        switch (fontname)
        {
            case "font1":name += "1";break;
            case "font2":name += "2";break;
            case "font3":name += "3";break;
            default:break;
        }
        if (popoutMessage == null)
        {
            Debug.Log("Loading popout file" + name);
            popoutMessage = Resources.Load(name);
            DontDestroyOnLoad(popoutMessage);
        }
        prefab = popoutMessage;

        GameObject tf = Instantiate(prefab) as GameObject;
        tf.transform.position = pos;
        PopoutController pop = tf.GetComponent<PopoutController>();
        pop.Setup
            ("", "-" + message, sortingoder, true
            , PopoutWIndowAlertType.MessageDismissWithDelay, action);
        return pop;
    }
    public static PopoutController CreatePopoutAlert(string title, string message,int sortingoder
        , bool dismissAfterTapBtn,PopoutWIndowAlertType type
        ,Action<PopoutController,PopoutWindowAlertActionType> action = null
        , string style = "")
    {
        string name = popoutPrefabName;
        UnityEngine.Object prefab = null;
        if (style == "con")
        {
            name = "Windows/PopoutConfirmCon";
            if (popout1 == null)
            {
                Debug.Log("Loading popout file " + name);
                popout1 = Resources.Load(name);
                DontDestroyOnLoad(popout1);
            }
            prefab = popout1;
        }
        else
        {
            if (popout0 == null)
            {
                Debug.Log("Loading popout file " + name);
                popout0 = Resources.Load(name);
                DontDestroyOnLoad(popout0);
            }
            prefab = popout0;
        }
        Debug.Log("Loading file " + name);
        GameObject tf = Instantiate(prefab) as GameObject;
        PopoutController pop = tf.GetComponent<PopoutController>();
        pop.Setup(title, message, sortingoder, dismissAfterTapBtn, type, action);
        //SDGameManager.Instance.audio
        return pop;
    }
    public static PopoutController CreatePopoutUnitSpeak(string unitName,string message
        ,string unitImageAddress
        ,int sortingoder
        ,bool dismissAfterTapBtn,PopoutWIndowAlertType type
        ,Action<PopoutController,PopoutWindowAlertActionType> action = null)
    {
        string name = popoutPrefabName;
        UnityEngine.Object prefab = null;
        name = "Windows/PopoutUnitSpeak";
        if (popoutMessage == null)
        {
            Debug.Log("Loading popout file " + name);
            popoutMessage = Resources.Load(name);
            DontDestroyOnLoad(popoutMessage);
        }
        prefab = popoutMessage;
        GameObject tf = Instantiate(prefab) as GameObject;
        //tf.transform.position = pos;
        PopoutController pop = tf.GetComponent<PopoutController>();
        pop.Setup(unitName, message, sortingoder, dismissAfterTapBtn, type, action);
        pop.panelBgImg1.sprite = Resources.Load<Sprite>(unitImageAddress);
        pop.fadeEndScale = Vector3.zero;
        return pop;
    }

    public void Setup(string title,string message,int sortingoder,bool dismissAfterTapBtn
        , PopoutWIndowAlertType type
        , Action<PopoutController, PopoutWindowAlertActionType> action)
    {
        _title = title;
        _message = message;
        _dismissAfterTapBtn = dismissAfterTapBtn;
        if (autoResizeText) { }
        _type = type;
        _action = action;
        titleText.text = _title;
        messageText.text = _message;
        Show(sortingoder);
    }
    public void SetupMsgFlow(string title,string message,int sortingoder,bool dismissAfterTapBtn
        ,PopoutWIndowAlertType type,Action<PopoutController,PopoutWindowAlertActionType> action)
    {
        _title = title;
        _message = message;
        _type = type;
        _action = action;
        titleText.text = _title;
        messageText.text = _message;
        ShowMsgFlow(sortingoder);
    }
    public void Show(int sortingoder)
    {
        rootCanvas.gameObject.SetActive(true);
        rootCanvas.sortingOrder = sortingoder;
        rootCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        rootCanvas.worldCamera = FindObjectOfType<Camera>();
        PopoutController.isShowingPopout = true;
        if (blackLayer != null)
        {
            blackLayer.color = Color.clear;
            blackLayer.DOFade(0.6f, 0.2f).SetUpdate(UpdateType.Normal, true);
        }
        refreshBtnsInPanel(_type);
        alertWindow.alpha = 0;
        alertWindow.DOFade(1f, 0.3f).SetUpdate(UpdateType.Normal, true);
        alertWindow.transform.localScale = fadeEndScale;
        alertWindow.transform.DOScale(Vector3.one, 0.2f)
            .SetEase(Ease.OutBack).SetUpdate(UpdateType.Normal, true);
        if(_type == PopoutWIndowAlertType.MessageDismissWithDelay)
        {
            float dur = _message.Length / 40.0f * 2.0f + 0.6f;
            StartCoroutine(IEWaitAndDismiss(dur));
        }
    }
    static int messageFlowIndex = 0;
    public void ShowMsgFlow(int sortingoder)
    {
        rootCanvas.gameObject.SetActive(true);
        rootCanvas.sortingOrder = sortingoder;
        confirmBtn.gameObject.SetActive(false);
        cancelBtn.gameObject.SetActive(false);
        confirmBtn.gameObject.SetActive(false);
        isShowingPopout = true;
        if (blackLayer != null)
        {
            blackLayer.color = Color.clear;
            blackLayer.DOFade(0.5f, 0.2f).SetUpdate(UpdateType.Normal, true);
        }
        alertWindow.alpha = 0;
        alertWindow.DOFade(0.8f, 0.3f).SetUpdate(UpdateType.Normal, true);
        alertWindow.transform.localScale = fadeEndScale;
        alertWindow.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack)
            .SetUpdate(UpdateType.Normal, true);
        alertWindow.transform.localPosition = Vector3.up * messageFlowIndex * 100;
        messageFlowIndex++;
        if (messageFlowIndex >= 4)
        {
            messageFlowIndex = 0;
        }
        if(_type == PopoutWIndowAlertType.MessageDismissWithDelay)
        {
            StartCoroutine(DismissMsgFlow(_message.Length / 80.0f + 0.5f));
        }
    }
    public IEnumerator IEWaitAndDismiss(float time)
    {
        yield return new WaitForSeconds(time);
        if(this)
            StartCoroutine(Dismiss());
    }
    public IEnumerator Dismiss()
    {
        alertWindow.DOFade(0, 0.5f).SetUpdate(UpdateType.Normal, true);
        alertWindow.transform.DOScale(fadeEndScale, 0.2f).SetEase(Ease.InBack).SetUpdate(UpdateType.Normal, true);
        yield return new WaitForSeconds(0.1f);
        if (blackLayer != null)
        {
            blackLayer.DOFade(0, 0.3f).SetUpdate(UpdateType.Normal, true);
        }
        yield return new WaitForSeconds(0.3f);
        PopoutController.isShowingPopout = false;
        popoutMessage = null;
        if (deactiveWhenDismiss)
        {
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
        //if(_action!=null)_action.Invoke(this,PopoutWindowAlertActionType.OnDismiss);
    }
    public IEnumerator DismissMsgFlow(float time)
    {
        yield return new WaitForSeconds(time);
        alertWindow.DOFade(0, 0.3f).SetUpdate(UpdateType.Normal, true);
        alertWindow.transform.DOLocalMove(Vector3.up * 450f, 0.8f)
            .SetEase(Ease.OutCubic).SetUpdate(UpdateType.Normal, true);
        messageFlowIndex--;
        messageFlowIndex = Mathf.Max(0, messageFlowIndex);
        yield return new WaitForSeconds(0.1f);
        if (blackLayer != null)
        {
            blackLayer.DOFade(0, 0.2f).SetUpdate(UpdateType.Normal,true);
        }
        yield return new WaitForSeconds(0.3f);
        PopoutController.isShowingPopout = false;
        popoutMessage = null;
        if (deactiveWhenDismiss)
        {
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }

    }
    public void Btn_ConfirmTapped()
    {
        TappedWithAction(PopoutWindowAlertActionType.OnConfirm);
    }
    public void Btn_CancelTapped()
    {
        TappedWithAction(PopoutWindowAlertActionType.OnCancel);
    }
    public void Btn_DismissTapped()
    {
        TappedWithAction(PopoutWindowAlertActionType.OnDismiss);
    }
    public void TappedWithAction(PopoutWindowAlertActionType type)
    {
        if (preventMultipleTaps)
        {
            if (_isDoingAction) return;
        }
        StartCoroutine(IETappedWithAction(type));
    }
    public IEnumerator IETappedWithAction(PopoutWindowAlertActionType type)
    {
        _isDoingAction = true;
        if (_action != null) _action.Invoke(this, type);
        if (_dismissAfterTapBtn)
        {
            StartCoroutine(Dismiss());
        }
        yield return new WaitForSeconds(0.5f);
        _isDoingAction = false;
    }


    public void refreshBtnsInPanel(PopoutWIndowAlertType currentType)
    {
        if(currentType == PopoutWIndowAlertType.ConfirmMessage)
        {
            confirmBtn.gameObject.SetActive(true);
            cancelBtn.gameObject.SetActive(false);
        }
        else if(currentType == PopoutWIndowAlertType.ConfirmOrCancel)
        {
            confirmBtn.gameObject.SetActive(true);
            cancelBtn.gameObject.SetActive(true);
        }
        else if(currentType == PopoutWIndowAlertType.MessageDismissWithDelay)
        {
            confirmBtn.gameObject.SetActive(false);
            cancelBtn.gameObject.SetActive(false);
        }
    }
}
