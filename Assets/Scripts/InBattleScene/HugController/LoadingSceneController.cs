using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class LoadingSceneController : MonoBehaviour
{
    public static string loadingSceneName = "SLLoadScene";
    public static string loadingScenePreAnimationName = "";
    public static string sceneToLoad;
    public Image blackLayer;
    public Image progressBar;
    public CanvasGroup mainUI;
    public Text loadingText;
    protected AsyncOperation _asyncOperation;
    protected float _fillTarget = 0;
    float fillamount = 0;
    public static bool isTransitingScene = false;
    public Transform monster;
    public Text loadingProgressText;
    public Image loadingForeImg;
    public RectTransform maskTrans;
    public Image logoImg;
    public Sprite[] logoSps;
    public static void LoadScene(string sceneName)
    {
#if UNITY_ANDROID && ANDROID_GP
        RTAndroidBackController.Instance.clearStack();
        RTAndroidBackController.Instance.enableBackBtn = true;
#endif
        if (isTransitingScene)
        {
            print("scene still transiting, cant transit now");
            return;
        }
        sceneToLoad = sceneName;
        Application.backgroundLoadingPriority = ThreadPriority.High;
        SceneManager.LoadSceneAsync(loadingSceneName, LoadSceneMode.Additive);
    }
    public void showFailHintText()
    {
        int rand = Random.Range(0, 30);
        string txt = "LOADING_HINT_TEXT_" + (rand + 1);
        loadingText.DOText(txt, 0.1f);
    }
    // Start is called before the first frame update
    void Start()
    {
        refreshLogo();
        if (sceneToLoad != "")
        {
            StartCoroutine(LoadingAsynchronously());
        }
    }
    public IEnumerator LoadingAsynchronously()
    {
        if (loadingScenePreAnimationName == "black")
        {
            mainUI.alpha = 0;
            loadingText.gameObject.SetActive(false);
            monster.gameObject.SetActive(false);
        }
        else
        {
            showFailHintText();
            loadingText.gameObject.SetActive(true);
            monster.gameObject.SetActive(true);
        }
        isTransitingScene = true;

        string previousSceneName = SceneManager.GetActiveScene().name;
        mainUI.alpha = 0;
        blackLayer.gameObject.SetActive(true);
        blackLayer.color = Color.clear;
        blackLayer.DOFade(1, 0.3f);

        yield return new WaitForSeconds(0.3f);
        if (loadingScenePreAnimationName != "black")
        {
            mainUI.DOFade(1, 0.3f);
        }
        yield return new WaitForSeconds(0.4f);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(loadingSceneName));
        fillamount = 0f;_fillTarget = 0.2f;
        if (loadingScenePreAnimationName.Length > 0)
        {
            _asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Single);
        }
        else
        {
            _asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Single);
        }
        _asyncOperation.allowSceneActivation = false;
        _fillTarget = 0.5f;
        while (_asyncOperation == null || _asyncOperation.progress < 0.9f)
        {
            _fillTarget = 0.5f + _asyncOperation.progress / 2.0f;
            yield return new WaitForEndOfFrame();
            //print("loading process 5 pg:"+_asyncOperation.progress);
        }

        _fillTarget = 1;
        yield return new WaitForSeconds(0.3f);
        //complete
        mainUI.DOFade(0, 0.3f);
        //print("loading process 6");
        GameObject.DontDestroyOnLoad(gameObject);
        //print("loading process 7");
        yield return new WaitForSeconds(0.3f);
        _asyncOperation.allowSceneActivation = true;
        //print("loading process 8");

        blackLayer.DOFade(0, 0.3f);

        yield return new WaitForSeconds(0.3f);

        isTransitingScene = false;

        GameObject.Destroy(gameObject);
    }
    public void refreshLogo()
    {
        //不同语言对应不同logo
        logoImg.sprite = logoSps[0];
    }
    // Update is called once per frame
    void Update()
    {
        fillamount = Mathf.MoveTowards(fillamount, _fillTarget, Time.deltaTime * 5);
        progressBar.fillAmount = fillamount;
        loadingProgressText.text = Mathf.RoundToInt(fillamount * 100) + "%";
        maskTrans.sizeDelta = new Vector2(550.5f * fillamount, maskTrans.sizeDelta.y);
    }
}
