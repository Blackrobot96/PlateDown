using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityIntQueue
{
    struct PrioItem {
        public int cost;
        public int index;

        public PrioItem (int index, int cost) {
            this.index = index;
            this.cost = cost;
        }

        public override string ToString() {
            return $"Cost: {cost} Index: {index}";
        }
    }
    List<PrioItem> queue = new List<PrioItem>();
    public void Enqueue(int item, int cost) {
        PrioItem pi = new PrioItem(item, cost);
        for (int i = 0; i < queue.Count; i++) {
            if (queue[i].cost >= cost) {
                queue.Insert(i, pi);
                return;
            }
        }
        queue.Add(pi);
    }

    public int length() {
        return queue.Count;
    }

    public int Dequeue() {
        PrioItem pi = queue[0];
        queue.RemoveAt(0);
        return pi.index;
    }

    public override string ToString() {
        string ret = $"Queue Length({length()}): ";
        foreach (PrioItem pi in queue) {
            ret += pi.ToString() + " ";
        }
        return ret;
    }
}

