using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RawIngridientsSource : InteractableFurniture
{
    public override GameObject PickUpItem() {        
        return Instantiate(item, new Vector3(0f, 0f, 0f), Quaternion.identity);
    }

    public override bool PlaceItem(GameObject obj) {
        return false;
    } 
}
