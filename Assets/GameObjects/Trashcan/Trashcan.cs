using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trashcan : InteractableFurniture
{
    public override GameObject PickUpItem() {
        return null;
    }

    public override bool PlaceItem(GameObject obj) {
        Item itm = obj.GetComponent<Item>();
        if (itm != null) {
            return itm.trash();
        }
        return false;
    } 
}
