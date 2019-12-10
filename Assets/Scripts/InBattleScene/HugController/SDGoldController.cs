using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 怪物掉落金币控制器
/// </summary>
public class SDGoldController : MonoBehaviour
{
    public Transform[] goldsPrefab;


    public Text goldText;
    public Transform goldParent;
    public Transform goldPos;
    private int[] goldArr;

    private float MOVE_HEIGHT = 60f;
    private float MOVE_WIDTH = 30f;
    private float MOVE_UP_DOWN_TIME = 0.1f;
    // Start is called before the first frame update
    [HideInInspector]
    public GameController GC;
    void Start()
    {
        GC = GetComponentInParent<GameController>();
    }

    public void showGold(float goldR)
    {
        int goldNum = (int)(SDConstants.goldRatio * goldR);
        goldText.gameObject.SetActive(true);
        //
        GC.allCoinsGet += goldNum;
        goldText.text = "GOLD-" + goldNum;
        //SDDataManager.Instance.AddCoin(goldNum);
    }
    public void hideGoid()
    {
        foreach(Transform t in goldParent)
        {
            Destroy(t.gameObject);
        }
        goldText.gameObject.SetActive(false);
    }
    public void createGold(Transform goldPrefab, int num ,Transform pos)
    {
        if (num == 0) return;
        StartCoroutine(IECreateGold(goldPrefab,num));
    }
    public IEnumerator IECreateGold(Transform goldPrefab,int num)
    {
        for(int i = 0; i < num; i++)
        {
            Transform t = Instantiate(goldPrefab) as Transform;
            t.parent = goldParent;
            t.localScale = Vector3.one;
            t.localPosition = goldPos.localPosition;
            StartCoroutine(IEGoldAnim(t));
            yield return new WaitForSeconds(0.1f);
        }
    }
    public IEnumerator IEGoldAnim(Transform t)
    {
        float randX = Random.Range(-MOVE_WIDTH, MOVE_WIDTH);
        t.DOLocalMove(t.localPosition + new Vector3(randX, MOVE_HEIGHT, 0), MOVE_UP_DOWN_TIME);
        yield return new WaitForSeconds(MOVE_UP_DOWN_TIME);
        t.DOLocalMove(t.localPosition + new Vector3(randX, -MOVE_HEIGHT, 0), MOVE_UP_DOWN_TIME);
        yield return new WaitForSeconds(MOVE_UP_DOWN_TIME);
    }
}
