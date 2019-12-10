using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TP_Controller : MonoBehaviour
{
    /// <summary>
    /// 角色类型(怪物，英雄，女神)
    /// </summary>
    public SDConstants.CharacterType _characterType;
    /// <summary>
    /// 战斗单元引用
    /// </summary>
    public BattleRoleData _unit;
    #region UI构建
    public Text TpText;
    public Text AddTpText;
    #endregion
    public int currentTp = 0;
    public int maxTp = 0;
    #region 动画构建
    private float TEXT_FADE_TIME = 0.15f;
    private float TEXT_MOVE_TIME = 0.2f;
    private float TEXT_MOVE_HEIGHT = 50f;
    public Transform foreImg;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        _unit = GetComponentInParent<BattleRoleData>();
        _characterType = _unit._Tag;
        refreshState();
    }
    /// <summary>
    /// 刷新蓝量数值与颜色
    /// </summary>
    public void refreshState()
    {
        StartCoroutine(IERefreshState());
    }
    public IEnumerator IERefreshState()
    {
        yield return new WaitForSeconds(0.5f);
        maxTp = _unit.ThisBasicRoleProperty()._role.ReadCurrentRoleRA((int)AttributeData.Tp);
        currentTp = 0;
        TpText.text = "" + currentTp;
        checkTpFull();
    }

    public void consumeTp(int val)
    {
        if (SDGameManager.Instance.DEBUG_NO_TP_USE) return;
        currentTp -= val;
        TpText.text = "" + currentTp;
        checkTpFull();
    }
    public void addTp(int val)
    {
        currentTp += val;
        currentTp = Mathf.Min(currentTp + val, maxTp);
        TpText.text = "" + currentTp;
        checkTpFull();
    }
    public void setTp(int val)
    {
        currentTp = val < maxTp ? val : maxTp;
        TpText.text = "" + currentTp;
        checkTpFull();
    }
    public void showAddTpAnimation(int val)
    {
        if (gameObject.activeSelf)
            StartCoroutine(IEShowAddTpAnim(val));
    }
    public IEnumerator IEShowAddTpAnim(int val)
    {
        AddTpText.transform.localPosition = Vector3.zero;
        AddTpText.color = SDConstants.color_blue_dark;
        AddTpText.text = "+" + val;
        AddTpText.gameObject.SetActive(true);
        AddTpText.DOFade(1, TEXT_FADE_TIME);
        yield return new WaitForSeconds(TEXT_FADE_TIME);
        AddTpText.transform.DOLocalMove(Vector2.up * TEXT_MOVE_HEIGHT, TEXT_MOVE_TIME);
        AddTpText.DOFade(0, TEXT_FADE_TIME);
        AddTpText.DOFade(0, TEXT_FADE_TIME);
        yield return new WaitForSeconds(TEXT_FADE_TIME);
        AddTpText.gameObject.SetActive(false);
    }
    public void checkTpFull()
    {
        if (currentTp == maxTp)
        {
            TpText.color = Color.white;
        }
        else
        {
            TpText.color = Color.white;
        }
        refreshTpBarChart();
    }
    public void refreshTpBarChart()
    {
        float scale = maxTp>0?currentTp * 1f / maxTp:0;
        foreImg.transform.localScale = new Vector3(scale, 1, 1);
    }
}
