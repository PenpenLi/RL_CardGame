using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PerRole_DataBox : MonoBehaviour
{

    public OneRoleClassData ThisORCD;
    [Header("Box显示索引")]
    public Text Name_Show;
    public Text Level_Show;
    public Slider LU_EXPShow;
    public Image Role_Show;
    public Image Bg_Show;
    public RectTransform Mastery_Show;
    public Image Job_Show;
    public RectTransform StarList_Show;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void PlayerTouchThisBox()
    {

    }
    public void ShowORCDInThisBox()
    {
        Name_Show.text = ThisORCD.ThisRoleShow.Name;
        Level_Show.text = "Lv." + ThisORCD.ThisRoleShow.Level.ToString();
        //Role_Show
        //Bg_Show
        //Mastery_Show
        //Job_Show
        //StarList_Show
        for(int i = 0; i < StarList_Show.childCount; i++)
        {
            StarList_Show.GetChild(i).gameObject.SetActive(i < ThisORCD.ThisRoleShow.Quality ? true : false);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ShowORCDInThisBox();
    }
}
