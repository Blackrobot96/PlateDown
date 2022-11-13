using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cooktop : InteractableFurniture
{
    public override bool PlaceItem(GameObject obj) {
        Item itm = obj.GetComponent<Item>();        
        if (base.PlaceItem(obj) && itm != null && item != null) {
            if (itm.cookable)
                itm.startProcess();
            item.transform.localPosition = new Vector3(0f, 1.024f, 0f);
            return true;
        }
        return false;
    }

    public override GameObject PickUpItem() {
        if (item != null) {
            Item itm = item.GetComponent<Item>();
            if (itm != null) {
                itm.endProcess();
            }
        }
        return base.PickUpItem();
    }
}
