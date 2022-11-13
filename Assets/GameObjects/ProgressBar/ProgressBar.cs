using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ProgressBar : MonoBehaviour
{
    public Slider slider;
    public Image iconComp;
    
    public void setProgress(int value) {
        slider.value = value;
    }

    void LateUpdate() 
    {
        transform.LookAt(transform.position + Camera.main.transform.forward);
    }

    public void setIcon(Sprite newIcon) {
        iconComp.sprite = newIcon;
    }
}
