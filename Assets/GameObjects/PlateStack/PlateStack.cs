using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateStack : InteractableFurniture
{
    public int stackSize = 8;
    public int currentSize = 0;
    GameObject[] stack;

    void Start() {
        stack = new GameObject[stackSize];
        for (int i = 0; i < stack.Length; i++) {
            GameObject plate = Instantiate(item, new Vector3(0f, 0f, 0f), Quaternion.identity);
            PlaceItem(plate);
        }
    }

    public override GameObject PickUpItem() {
        if (currentSize > 0) {
            currentSize -= 1;
            GameObject retItem = stack[currentSize];
            retItem.transform.SetParent(null);
            return retItem;
        }
        else
            return null;
    }

    public override bool PlaceItem(GameObject obj) {
        Plate plate = obj.GetComponent<Plate>();
        if (currentSize < stackSize && plate != null && plate.isEmpty()) {
            obj.transform.SetParent(transform);
            obj.transform.rotation = transform.rotation;
            obj.transform.localPosition = new Vector3(0f, 1f + 0.08f * currentSize, 0f);
            stack[currentSize] = obj;
            currentSize += 1;
            return true;
        } else
            return false;
    } 

}