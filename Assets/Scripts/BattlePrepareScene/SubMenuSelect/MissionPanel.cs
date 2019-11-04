using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionPanel : MonoBehaviour
{
    public Image currentMissionImg;
    public Image missionKindIcon;
    public Text missionRequirement;
    public enum missionKind
    {
        plot,
        daily,
        weekly,
        challenging,
    }
    public missionKind currentKind = missionKind.plot;
    public void WhenOpenThisPanel()
    {

    }


}
