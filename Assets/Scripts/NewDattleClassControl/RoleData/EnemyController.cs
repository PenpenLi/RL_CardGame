using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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
    /// <summary>
    /// 普通关卡小怪和Boss掉落
    /// </summary>
    /// <returns></returns>
    public List<int> getDropItemsId()
    {
        int lv = SDGameManager.Instance.currentLevel;
        int maxPassLv = SDDataManager.Instance.GetMaxPassLevel();
        int minLv = 0;int maxLv = 0;
        List<int> dropItemsLd = new List<int>();
        List<Dictionary<string, string>> itemDatas = SDDataManager.Instance.ReadFromCSV("material");
        for(int i = 0; i < itemDatas.Count; i++)
        {
            Dictionary<string, string> s = itemDatas[i];
            minLv = SDDataManager.Instance.getInteger(s["minLv"]);
            maxLv = SDDataManager.Instance.getInteger(s["maxLv"]);
            if (lv>=minLv && lv <= maxLv)
            {
                int weight = SDDataManager.Instance.getInteger(s["weight"]);
                int materilaID = SDDataManager.Instance.getInteger(s["id"]);
                //
                for(int m = 0; m < weight; m++)
                {
                    dropItemsLd.Add(materilaID);
                }
            }
        }
        return dropItemsLd;
    }
    public int getDropItemId()
    {
        List<int> dropItemsId = getDropItemsId();
        int dropId = 0;
            //= dropItemsId[UnityEngine.Random.Range(0, dropItemsId.Count)];
        return dropId;
    }
    public void showDropItem()
    {

        if(UnityEngine.Random.Range(0,1f) < DropProb)
        {
            int dropId = getDropItemId();
            SDDataManager.Instance.addMaterial(dropId);
            string message = SDDataManager.Instance.getMaterialNameById(dropId) + " +1";
            //
            //SDGameManager.Instance.DropMaterials[dropId % 10000]++;
            //
            //StartCoroutine(IEShowDropItemAnim());
        }
    }
    public IEnumerator IEShowDropItemAnim()
    {
        dropItemImg.gameObject.SetActive(true);
        Transform t = dropItemImg.transform;
        //t.DOLocalMoveY(t.localPosition.y + MOVE_UP_HEIGHT, MOVE_UP_TIME);
        //t.DOScale(Vector3.one * MOVE_UP_SCALE, MOVE_UP_TIME);
        yield return new WaitForSeconds(MOVE_UP_TIME);
        //dropItemImg.DOFade(0, 0.3f);
    }
    public void showDropItems()
    {
        List<int> dropItemsId = getDropItemsId();
        //Image[] imgs = dropItems.GetComponentsInChildren<Image>();
        int currLv = SDGameManager.Instance.currentLevel;
        int badgeId = getBadgeIdByLevel();



        //StartCoroutine(IEShowDropItemAnim());
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
            this.RARate = _enemy.RARate;
            this.CRI = _enemy.CRI;
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
