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
    private float initDelay = 0.1f;
    public void OnBeginDrag(PointerEventData data)
    {
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
    public void initRoleModelToRolePosPlace()
    {
        StartCoroutine(IEInitRoleModelToRolePosPlace());
    }
    public IEnumerator IEInitRoleModelToRolePosPlace()
    {
        ResetThisPanel();
        yield return new WaitForSeconds(initDelay);
        List<int> heroes = SDDataManager.Instance.getHeroTeamByTeamId(STUP.CurrentTeamId).heroes;
        for (int i = 0; i < heroes.Count; i++)
        {
            if (SDDataManager.Instance.GetHeroOwnedByHashcode(heroes[i]) != null)
            {
                Transform s = Instantiate(RoleModel) as Transform;
                CharacterModelController CMC = s.GetComponentInChildren<CharacterModelController>();
                CMC.heroHashcode = heroes[i];
                int pos = SDDataManager.Instance.GetHeroOwnedByHashcode(heroes[i]).teamPos;
                SDDataManager.Instance.GetHeroOwnedByHashcode(heroes[i]).teamPos = checkBenefitPos(pos);
                pos = SDDataManager.Instance.GetHeroOwnedByHashcode(heroes[i]).teamPos;
                Vector2 aim = AllEnablePosPlace[pos].position;
                s.position = aim;
                s.SetParent(RoleGroup);
                s.localScale = Vector3.zero;
                s.gameObject.SetActive(true);
                //显示对应模型
                s.SetSiblingIndex(i);
                s.DOScale(Vector3.one,initDelay).SetEase(Ease.OutBack);
                yield return new WaitForSeconds(initDelay);
            }
        }
    }
    public int checkBenefitPos(int oldPos)
    {
        for (int i = 0; i < AllEnablePosPlace.Length; i++)
        {
            int newPos = (oldPos + i) % AllEnablePosPlace.Length;
            if (!AllPosStatus[newPos])
            {
                AllPosStatus[newPos] = true;
                return newPos;
            }
        }
        return -1;//不成立，所以按照出错处理
    }
    public void ResetThisPanel()
    {
        for(int i = 0; i < RoleGroup.childCount; i++)
        {
            Destroy(RoleGroup.GetChild(i).gameObject);
        }
        for(int i = 0; i < AllPosStatus.Length; i++) { AllPosStatus[i] = false; }
    }
}
