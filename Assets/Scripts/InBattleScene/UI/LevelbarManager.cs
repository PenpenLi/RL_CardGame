using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 关卡进度条管理器，新的关卡开启时会向前移动
/// </summary>
public class LevelbarManager : MonoBehaviour
{
    public Transform[] levelPos;
    public Transform levelFlag;
    public int curLv;
    public float speed = 1.0f;
    public bool startActionFlag = false;

    public void setupLevelAnim()
    {
        int lv = SDGameManager.Instance.currentLevel;
        curLv = lv % SDConstants.LevelNumPerSection;
        //if (curLv == 0) curLv = SDConstants.LevelNumPerSection;
        startActionFlag = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (startActionFlag)
        {
            if(curLv == 0)
            {
                levelFlag.position = levelPos[0].position;
                startActionFlag = false;
            }
            else
            {
                levelFlag.position += Vector3.right * Time.deltaTime * 0.5f * speed;
                if(levelFlag.position.x >= levelPos[curLv].position.x)
                {
                    startActionFlag = false;
                }
            }
        }
    }
}
