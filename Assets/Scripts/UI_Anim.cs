using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_Anim : MonoBehaviour
{
    public float AEuler;
    public float ASizeRate;
    public float ATime;
    private bool isC;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void WhenClicking()
    {
        if (!isC)
        {
            StopCoroutine("UIClicking");
            isC = true;
            StartCoroutine("UIClicking");
        }
    }
    IEnumerator UIClicking()
    {
        transform.DOPunchRotation(Vector3.forward * AEuler, ATime);
        transform.DOPunchScale(Vector3.one * ASizeRate, ATime);
        yield return new WaitForSeconds(ATime);
        isC = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
