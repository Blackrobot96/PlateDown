using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableFurniture : MonoBehaviour
{
    public GameObject item = null;
    public AudioSource processAudio;
    public virtual GameObject PickUpItem() {
        if (item == null)
            return null;
        GameObject retItem = item;
        Item itm = item.GetComponent<Item>();
        if (itm != null)
            itm.endProcess();
        retItem.transform.SetParent(null);
        item = null;
        return retItem;
    }

    public virtual bool PlaceItem(GameObject obj) {
        if (item == null) {
            item = obj;
            item.transform.SetParent(transform);
            item.transform.rotation = transform.rotation;
            item.transform.localPosition = new Vector3(0f, 1f, 0f);

            return true;
        }
        else {
            Item itm = item.GetComponent<Item>();
            Item objitm = obj.GetComponent<Item>();
            if (itm != null) {
                if (itm.combine(obj)) {
                    return true;
                }
                else if (objitm != null && objitm.combine(item)) {
                    itm.endProcess();
                    item = null;
                    return PlaceItem(obj);
                }
            }
            return false;
        }
    } 

    public virtual bool startProcess()
    {
        return false;
    }
    public virtual bool endProcess()
    {
        return false;
    }
}
