using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steak : Item
{
    void Update()
    {
        if (processing && isProcessable())
        {
            progressbar.gameObject.SetActive(true);
            progressbar.setProgress(Mathf.RoundToInt((currentProgress / stages[currentStage].processTime) * 100));             
            if (currentProgress >= stages[currentStage].processTime) {
                currentProgress = 0f;
                currentStage += 1;
                setStage(currentStage);
            } else 
                currentProgress += Time.deltaTime;
        } else
            progressbar.gameObject.SetActive(false);
    }
}
