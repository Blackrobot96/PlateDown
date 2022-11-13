using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plate : Item
{
    GameObject content = null;
    public override bool combine(GameObject other) {
        Plate plate = other.GetComponent<Plate>();
        if (content == null && plate == null && currentStage == 0) {
            content = other;
            content.transform.SetParent(transform);
            content.transform.rotation = transform.rotation;
            content.transform.localPosition = new Vector3(0f, 0.1f, 0f);
            return true;
        }
        return false;
    }

    public bool isEmpty() {
        return content == null && currentStage == 0;
    }

    public bool clean() {
        setStage(0);
        return true;
    }

    public GameObject getContent() {
        return content;
    }

    public override bool trash() {
        if (!isEmpty()) {
            Destroy(content, 0f);
            content = null;
            setStage(1);
        }
        return false;
    }

    public override bool isProcessable()
    {
        return getCurrentStageIndex() == 1;
    }
    void Update()
    {
        if (processing && isProcessable())
        {
            progressbar.gameObject.SetActive(true);
            progressbar.setProgress(Mathf.RoundToInt((currentProgress / stages[currentStage].processTime) * 100));             
            if (currentProgress >= stages[currentStage].processTime) {
                currentProgress = 0f;
                currentStage = 0;
                setStage(currentStage);
            } else 
                currentProgress += Time.deltaTime;
        } else
            progressbar.gameObject.SetActive(false);
    }

}
