using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    protected bool processing = false;
    public Stages[] stages;
    public int initStage = 0;

    [System.Serializable]
    public class Stages {
        public Sprite processIcon;
        public GameObject stage;
        public float processTime;
    }
    protected int currentStage = 0;
    protected float currentProgress = 0;
    protected GameObject currentGameObject = null;
    public GameObject progressBarRef;
    protected ProgressBar progressbar;

    public bool cookable = false;

    void Start()
    {
        GameObject pb = Instantiate(progressBarRef, new Vector3(0f,0f,0f), Quaternion.identity);
        pb.transform.SetParent(transform);
        pb.transform.localPosition = new Vector3(0f,1.2f,0f);
        progressbar = pb.GetComponentInChildren<ProgressBar>();
        progressbar.gameObject.SetActive(false);

        if (stages.Length > 0)
            setStage(initStage);
    }

    public bool startProcess()
    {
        if (isProcessable()) {
            processing = true;
            return true;
        }
        return false;
    }
    public bool endProcess()
    {
        currentProgress = 0f;
        processing = false;
        return false;
    }

    public virtual bool isProcessable()
    {
        return getStagesCount() - 1 > getCurrentStageIndex();
    }

    public int getStagesCount() {
        return stages.Length;
    }

    public int getCurrentStageIndex() {
        return currentStage;
    }

    public GameObject getCurrentStage() {
        return stages[currentStage].stage;
    }

    public void setStage(int stage) {
        currentStage = stage;
        progressbar.setIcon(stages[currentStage].processIcon);
        GameObject obj = Instantiate(stages[currentStage].stage, new Vector3(0f,0f,0f), Quaternion.identity);
        setChild(obj);
    }
    void setChild(GameObject obj) {
        clearChild();
        currentGameObject = obj;
        obj.transform.SetParent(transform);
        obj.transform.localPosition = new Vector3(0f,0f,0f);
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = new Vector3(GameState.Instance.gridSize, 1f, GameState.Instance.gridSize);
    }

    void clearChild() {
        Destroy(currentGameObject, 0f);
    }

    void clearAllChildren() {
        for (int i = 0; i < transform.childCount; i++) {
            Transform child = transform.GetChild(i);
            Destroy(child.gameObject, 0f);
        }
    }

    public virtual bool combine(GameObject other) {
        return false;
    }

    public virtual bool trash() {
        clearAllChildren();
        Destroy(this, 0f);
        return true;
    }
}
