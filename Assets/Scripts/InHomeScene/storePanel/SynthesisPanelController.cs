using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SynthesisPanelController : MonoBehaviour
{
    public ScrollRect scollrect;
    public Transform SynthesisCard;

    public void CloseThisPanel()
    {
        UIEffectManager.Instance.showAnimFadeIn(transform);
    }
}
