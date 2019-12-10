using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SDWeapon : BasicRoleProperty
{
    protected EquipPosition equipPos = EquipPosition.Hand;
    public SDConstants.WeaponType _weaponType;
    public int grade;
    private void Start()
    {

    }
    public void initGradeShow(int grade)
    {
        this.grade = grade;
        AddMultiplier(this.grade);
    }
}
