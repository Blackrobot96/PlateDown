using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sink : InteractableFurniture
{
    public override bool PlaceItem(GameObject obj)
    {
        if (base.PlaceItem(obj) && item != null) {
            item.transform.localPosition = new Vector3(0f, 0.83f, 0f);
            return true;
        }
        return false;
    }
    public override bool startProcess()
    {
        if (item != null) {
            Plate plate = item.GetComponent<Plate>();
            if (plate != null) {
                if (processAudio != null && plate.isProcessable())
                    processAudio.Play();
                return plate.startProcess();
            }
            return true;
        }
        return false;
    }
    public override bool endProcess()
    {
        if (item != null) {
            Plate plate = item.GetComponent<Plate>();
            if (processAudio != null)
                processAudio.Pause();
            
            if (plate != null) {
                return plate.endProcess();
            }
            return true;
        }
        return false;
    }
}
