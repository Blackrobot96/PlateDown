using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : InteractableFurniture
{
    public int plateCount = 0;
    bool occupied = false;
    
    /*public override bool start() {
        if (item != null)
            item.transform.localPosition = new Vector3(0f, 1f, -0.25f);
        return true;
    }*/

    public bool isOccupied() {
        return occupied;
    }
    void Start()
    {
        GameState.Instance.emptyTables.Add(this);
    }
}
