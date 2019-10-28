using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Id_Name_List : MonoBehaviour
{
    public List<Person> AllNameList;
}

public class Person
{
    public string name;
    public int id;
    public Person(string _name,int _id)
    {
        name = _name;
        id = _id;
    }
}