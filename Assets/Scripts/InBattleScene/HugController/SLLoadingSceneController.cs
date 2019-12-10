using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using I2.Loc;
using UnityEngine.SceneManagement;

/// <summary>
/// 读取页面
/// </summary>
public class SLLoadingSceneController : MonoBehaviour
{
    public static string loadingSceneName = "MenuScene";
    public static string loadingScenePreAnimationName = "";
    public static string sceneToLoad;
    public Image balckLayer;
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

        if (isTransitingScene)
        {
            print("scene still transiting cant transit now(场景正在加载，无法重复加载)");
            return;
        }

        sceneToLoad = sceneName;
        Application.backgroundLoadingPriority = ThreadPriority.High;
        SceneManager.LoadSceneAsync(loadingSceneName, LoadSceneMode.Additive);
    }
    public void showFailHintText()
    {
        int rand = Random.Range(0, 30);
        string txt = SDGameManager.T("LOADING_HINT_TEXT_" + (rand + 1));
        loadingText.DOText(txt, 0.1f);
    }
    private void Start()
    {
        refreshLogo();
        if (sceneToLoad != "")
        {
            StartCoroutine(LoadAsynchronously());
        }
    }
    private void Update()
    {
        fillamount = Mathf.MoveTowards(fillamount, _fillTarget, Time.deltaTime * 5);
        progressBar.fillAmount = fillamount;
        int pc = Mathf.RoundToInt(fillamount * 100);
        loadingProgressText.text = pc + "%";
        maskTrans.localScale = new Vector3(pc * 1f / 100, 1, 1);
    }
    public void refreshLogo()
    {
        if(LocalizationManager.CurrentLanguage == "English")
        {
            logoImg.sprite = logoSps[1];
        }
        else
        {
            logoImg.sprite = logoSps[0];
            if(LocalizationManager.CurrentLanguage == "Chinese (Traditional)")
            {
                logoImg.sprite = logoSps[2];
            }
        }
    }
    public IEnumerator LoadAsynchronously()
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
        balckLayer.gameObject.SetActive(true);
        balckLayer.color = Color.clear;
        balckLayer.DOFade(1, 0.3f);
        yield return new WaitForSeconds(0.3f);
        if (loadingScenePreAnimationName != "black")
        {
            mainUI.DOFade(1, 0.3f);
        }
        yield return new WaitForSeconds(0.4f);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(loadingSceneName));
        fillamount = 0f;
        _fillTarget = 0.1f;
        //

        //
        _fillTarget = 0.2f;
        _asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Single);
        _asyncOperation.allowSceneActivation = false;
        _fillTarget = 0.5f;
        while (_asyncOperation == null || _asyncOperation.progress < 0.9f)
        {
            _fillTarget = 0.5f + _asyncOperation.progress / 2.0f;
            yield return new WaitForEndOfFrame();
        }
        _fillTarget = 1;
        yield return new WaitForSeconds(0.3f);
        //complete
        mainUI.DOFade(0, 0.3f);
        DontDestroyOnLoad(gameObject);
        yield return new WaitForSeconds(0.3f);
        _asyncOperation.allowSceneActivation = true;
        balckLayer.DOFade(0, 0.3f);
        yield return new WaitForSeconds(0.3f);
        isTransitingScene = false;
        Destroy(gameObject);
    }
    public IEnumerator IELoadScene()
    {
        if (loadingScenePreAnimationName.Length > 0)
        {
            _asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Single);
        }
        else
        {
            _asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Single);
        }
        _asyncOperation.allowSceneActivation = false;
        yield return new WaitForEndOfFrame();
    }
}
