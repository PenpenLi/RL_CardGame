using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using GameDataEditor;
using DG.Tweening;

public class RolePosControllerInTeam : MonoBehaviour, IBeginDragHandler, IEndDragHandler,IDragHandler
{
    public Transform[] AllEnablePosPlace;
    public bool[] AllPosStatus;
    //public int currentDragPos = 0;
    public int currentMGIndex;
    [Space(25)]
    public Transform RoleModel;
    //public List<Transform> ModelGroup;
    public Transform RoleGroup;
    public SelectTeamUnitPanel STUP { get { return GetComponentInParent<SelectTeamUnitPanel>(); } }
    private float initDelay = 0.05f;
    public void OnBeginDrag(PointerEventData data)
    {
        if (RoleGroup.childCount <= currentMGIndex)
        {
            GetComponent<Image>().raycastTarget = false;return;
        }

        Ray targetChooseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector2 origin = new Vector2(targetChooseRay.origin.x, targetChooseRay.origin.y);
        int index = 0;
        for (int i = 0; i < RoleGroup.childCount; i++)
        {
            float dis0 = Vector2.Distance(RoleGroup.GetChild(index).position, origin);
            float dis1 = Vector2.Distance(RoleGroup.GetChild(i).position, origin);
            if (dis0 > dis1)
            {
                index = i;
            }
        }
        currentMGIndex = index;
    }
    public void OnEndDrag(PointerEventData data)
    {
        if (RoleGroup.childCount <= currentMGIndex) return;

        Vector2 origin = RoleGroup.GetChild(currentMGIndex).position;
        int index = 0;//确认角色新位置
        for (int i = 0; i < AllEnablePosPlace.Length; i++)
        {
            float dis0 = Vector2.Distance(AllEnablePosPlace[index].position, origin);
            float dis1 = Vector2.Distance(AllEnablePosPlace[i].position, origin);
            if (dis0 > dis1)
            {
                index = i;
            }
        }
        setRolePlace(index);
    }
    public void OnDrag(PointerEventData data)
    {
        if (RoleGroup.childCount <= currentMGIndex) return;

        Ray targetChooseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector2 origin = new Vector2(targetChooseRay.origin.x, targetChooseRay.origin.y);
        RoleGroup.GetChild(currentMGIndex).position = origin;
    }
    public void setRolePlace(int aimPosIndex)
    {
        int heroHC 
            = RoleGroup.GetChild(currentMGIndex).GetComponentInChildren<CharacterModelController>().heroHashcode;

        //如果选中新槽位已有角色，则调换两角色槽位
        for (int i = 0; i < RoleGroup.childCount; i++)
        {
            int heroHashcode
                = RoleGroup.GetChild(i).GetComponentInChildren<CharacterModelController>().heroHashcode;
            if(SDDataManager.Instance.getHeroPosInTeamByHashcode(heroHashcode) == aimPosIndex
                && i != currentMGIndex)
            {
                int oldPos = SDDataManager.Instance.getHeroPosInTeamByHashcode(heroHC);
                SDDataManager.Instance.GetHeroOwnedByHashcode(heroHashcode).teamPos = oldPos;
                RoleGroup.GetChild(i).position = AllEnablePosPlace[oldPos].position;
            }
        }

        //改写当前角色槽位
        RoleGroup.GetChild(currentMGIndex).position = AllEnablePosPlace[aimPosIndex].position;
        SDDataManager.Instance.GetHeroOwnedByHashcode(heroHC).teamPos = aimPosIndex;
    }



    List<GDEHeroData> teamNumbers = new List<GDEHeroData>();
    public void initRoleModelToRolePosPlace()
    {
        ResetThisPanel();
        teamNumbers = SDDataManager.Instance.getHerosFromTeam(STUP.CurrentTeamId);
        for (int i = 0; i < teamNumbers.Count; i++)
        {
            if (teamNumbers[i] != null)
            {
                Debug.Log("载入模型对应hashcode：" + teamNumbers[i].hashCode + "_" + i);
                int pos = teamNumbers[i].teamPos;
                if (checkBenefitPos(pos) >= 0)
                {
                    teamNumbers[i].teamPos
                        = checkBenefitPos(pos);
                    SDDataManager.Instance.setHeroTeamPos
                        (teamNumbers[i].hashCode, teamNumbers[i].teamPos);
                }
            }
        }
        StartCoroutine(IEInitRoleModelToRolePosPlace());
    }
    public IEnumerator IEInitRoleModelToRolePosPlace()
    {
        for (int i = 0; i < teamNumbers.Count; i++)
        {
            Debug.Log("载入模型对应hashcode：" + teamNumbers[i].hashCode + "_" + i);
            int pos = teamNumbers[i].teamPos;
            Transform s = Instantiate(RoleModel) as Transform;
            CharacterModelController CMC
                = s.GetComponentInChildren<CharacterModelController>();
            //CMC.heroHashcode = teamNumbers[i].hashCode;
            //CMC.initCharacterModel(teamNumbers[i].hashCode,SDConstants.CharacterAnimType.)
            CMC.initHeroCharacterModel(teamNumbers[i].hashCode);
            Vector2 aim = AllEnablePosPlace[pos].position;
            s.position = aim;
            s.SetParent(RoleGroup);
            s.localScale = Vector3.zero;
            s.gameObject.SetActive(true);
            //显示对应模型
            s.SetSiblingIndex(i);
            s.DOScale(Vector3.one, initDelay).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(initDelay);
        }
        //STUP.usingIEcannotTap = false;
    }
    public int checkBenefitPos(int oldPos)
    {
        for (int i = 0; i < SDConstants.MaxSelfNum; i++)
        {
            int newPos = (oldPos + i) % SDConstants.MaxSelfNum;
            if (!AllPosStatus[newPos])
            {
                AllPosStatus[newPos] = true;
                return newPos;
            }
        }

        List<GDEHeroData> list = SDDataManager.Instance.getHerosFromTeam(STUP.CurrentTeamId);
        for(int i = 0; i < SDConstants.MaxSelfNum; i++)
        {
            bool flag = false;
            foreach(GDEHeroData H in list)
            {
                if (i == H.teamPos)
                {
                    flag = true;break;
                }
            }
            if (!flag) return i;
        }

        Debug.Log("出现错误，所有位置均被占据");
        return -1;//不成立，所以按照出错处理
    }
    public void ResetThisPanel()
    {
        for(int i = 0; i < RoleGroup.childCount; i++)
        {
            Destroy(RoleGroup.GetChild(i).gameObject);
        }
        for(int i = 0; i < AllPosStatus.Length; i++) { AllPosStatus[i] = false; }
        teamNumbers.Clear();
    }
}
