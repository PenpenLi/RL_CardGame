using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GameDataEditor;

public class EnemyController : BasicRoleProperty
{
    public SDConstants.EstateType _currentEstate;
    public bool isFacingRight = true;
    public string dropItemStr;
    public SDEnemy _enemy;
    public Image dropItemImg;
    public Transform dropItems;
    //
    private float MOVE_UP_TIME = 0.6f;
    private float MOVE_UP_HEIGHT = 100f;
    private float MOVE_UP_SCALE = 1.3f;
    //
    public Job AttackPreferenceInJob = Job.End;
    public Race AttackPreferenceInRace = Race.End;
    public bool CunningAttack = false;

    public float DropProb = 0.2f;

    SDDropController _dropc;
    public SDDropController DropC
    {
        get 
        {
            if (_dropc == null) _dropc = GetComponentInChildren<SDDropController>();
            return _dropc;
        }
    }
    public GameController GameC
    {
        get { return GetComponentInParent<GameController>(); }
    }
    public void showDropItem()
    {
        if(UnityEngine.Random.Range(0,1f) < DropProb)
        {
            GDEItemData M = DropC.oneDropReward();
            //SDDataManager.Instance.addMaterial(M.id,M.num);
            string message = SDDataManager.Instance.getMaterialNameById(M.id) + " +"
                + M.num;
            //dropItemImg.sprite = 

            StartCoroutine(IEShowDropItemAnim());
            //GameC.allDropsGet.Add(M);
            GameC.AddDrop(M);
        }
    }
    public IEnumerator IEShowDropItemAnim()
    {
        dropItemImg.gameObject.SetActive(true);
        Transform t = dropItemImg.transform;
        t.DOLocalMoveY(t.localPosition.y + MOVE_UP_HEIGHT, MOVE_UP_TIME);
        t.DOScale(Vector3.one * MOVE_UP_SCALE, MOVE_UP_TIME);
        yield return new WaitForSeconds(MOVE_UP_TIME);
        dropItemImg.DOFade(0, 0.3f);
    }
    public void showDropItems()
    {
        //List<int> dropItemsId = getDropItemsId();
        //Image[] imgs = dropItems.GetComponentsInChildren<Image>();
        int currLv = SDGameManager.Instance.currentLevel;
        int badgeId = getBadgeIdByLevel();



        StartCoroutine(IEShowDropItemAnim());
    }
    public int getBadgeIdByLevel()
    {
        int currLv = SDGameManager.Instance.currentLevel;
        int badgeId = 0;



        return badgeId;
    }
    public void initEnemyBasicProperties()
    {
        if (_enemy)
        {
            this.RoleBasicRA = _enemy.RoleBasicRA;
            this.CRIDmg = _enemy.CRIDmg;
            this.DmgReduction = _enemy.DmgReduction;
            this.DmgReflection = _enemy.DmgReflection;
            this.RewardRate = _enemy.RewardRate;
            this.BarChartRegendPerTurn = _enemy.BarChartRegendPerTurn;

            this.ID = _enemy.ID;
            this.Name = _enemy.Name;
            this.LEVEL = _enemy.LEVEL;
        }

        initRoleClassData();
    }
}
