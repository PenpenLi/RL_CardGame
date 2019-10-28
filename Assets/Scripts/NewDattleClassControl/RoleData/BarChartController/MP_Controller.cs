using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MP_Controller : MonoBehaviour
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
    public Text MpText;
    public Text AddMpText;
    #endregion
    public int currentMp = 0;
    public int maxMp = 0;
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
        maxMp = _unit.ThisBasicRoleProperty()._role.ReadCurrentRoleRA((int)AttributeData.Mp);
        currentMp = maxMp;
        MpText.text = "" + currentMp;
        checkMpFull();
    }

    public void consumeMp(int val)
    {
        if (SDGameManager.Instance.DEBUG_NO_MP_USE) return;
        currentMp -= val;
        MpText.text = "" + currentMp;
        checkMpFull();
    }
    public void addMp(int val)
    {
        currentMp += val;
        currentMp = Mathf.Min(currentMp + val, maxMp);
        MpText.text = "" + currentMp;
        checkMpFull();
    }
    public void setMp(int val)
    {
        currentMp = val < maxMp ? val : maxMp;
        MpText.text = "" + currentMp;
        checkMpFull();
    }
    public void showAddMpAnimation(int val)
    {
        if (gameObject.activeSelf)
            StartCoroutine(IEShowAddMpAnim(val));
    }
    public IEnumerator IEShowAddMpAnim(int val)
    {
        AddMpText.transform.localPosition = Vector3.zero;
        AddMpText.color = SDConstants.color_blue_dark;
        AddMpText.text = "+" + val;
        AddMpText.gameObject.SetActive(true);
        AddMpText.DOFade(1, TEXT_FADE_TIME);
        yield return new WaitForSeconds(TEXT_FADE_TIME);
        AddMpText.transform.DOLocalMove(Vector2.up * TEXT_MOVE_HEIGHT, TEXT_MOVE_TIME);
        yield return new WaitForSeconds(TEXT_MOVE_TIME);
        AddMpText.DOFade(0, TEXT_FADE_TIME);
        yield return new WaitForSeconds(TEXT_FADE_TIME);
        AddMpText.gameObject.SetActive(false);
    }
    public void checkMpFull()
    {
        if (currentMp == maxMp)
        {
            MpText.color = Color.white;
        }
        else
        {
            MpText.color = Color.white;
        }
        refreshMpBarChart();
    }
    public void refreshMpBarChart()
    {
        float scale = maxMp>0?currentMp * 1f / maxMp:0;
        foreImg.transform.localScale = new Vector3(scale, 1, 1);
    }
}
