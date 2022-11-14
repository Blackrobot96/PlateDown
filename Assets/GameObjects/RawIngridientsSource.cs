using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RawIngridientsSource : InteractableFurniture
{
    public override GameObject PickUpItem() {        
        return Instantiate(item, new Vector3(0f, 0f, 0f), Quaternion.identity);
    }

    public override bool PlaceItem(GameObject obj) {
        Item objItm = obj.GetComponent<Item>();
        Item itm = item.GetComponent<Item>();
        if (itm != null && objItm.getCurrentStageIndex() == 0L && itm.GetType() == objItm.GetType()) {
            Destroy(objItm.gameObject, 0f);
            return true;
        }
        return false;
    } 
}
