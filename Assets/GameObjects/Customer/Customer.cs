using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject progressbarref;
    ProgressBar progressbar;

    public CustomerPatience[] customerPatience;
    [System.Serializable]
     public class CustomerPatience {
        public Sprite icon;
        public CustomerPhases phase;
        public float patience;
    }
    public enum CustomerPhases {
        WaitingForTable,
        WalkingToTable,
        ChoosingMeal,
        WaitingToOrder,
        WaitingForFood,
        WaitingToBeServed,
        Eating,
        Leaving
    }

    CustomerPatience currentPhase = null;
    float currentPatience = 0f;

    public float speed = 1f;

    Table table = null;

    Item meal = null;
    int mealState = 0;
    void Start()
    {        
        GameObject pb = Instantiate(progressbarref, new Vector3(0f,0f,0f), Quaternion.identity);
        pb.transform.SetParent(transform);
        pb.transform.localPosition = new Vector3(0f,1.2f,0f);

        progressbar = pb.GetComponentInChildren<ProgressBar>();
        progressbar.setProgress(0);
        setPhase(CustomerPhases.WaitingForTable);
        GameState gs = GameState.Instance;
        /*int ly = Mathf.FloorToInt(transform.position.z / gs.gridSize);
        
        if (gs.emptyTables.Count > 0) {

            Table tab = gs.emptyTables[0];
            if (tab != null) {
                gs.emptyTables.Remove(tab);
            }
            
            int tx = Mathf.FloorToInt(tab.gameObject.transform.position.x / gs.gridSize);
            int ty = Mathf.FloorToInt(tab.gameObject.transform.position.z / gs.gridSize);
            Debug.Log($"Empty Table at X:{tx} Y:{ty}");
            HashSet<int> visited = new HashSet<int>();
            PriorityIntQueue queue = new PriorityIntQueue();
            queue.Enqueue(gs.getPos(0, ly), Mathf.Abs(tx - 0) + Mathf.Abs(ty - ly));
            Dictionary<int, int> path = new Dictionary<int, int>();
            path.Add(gs.getPos(0, ly), -1);
            while (queue.length() != 0) {
                Debug.Log(queue.ToString());
                int pos = queue.Dequeue();
                visited.Add(pos);
                Vector2Int card = gs.getPosFromIndex(pos);
                if (card.x == tx && card.y == ty) 
                {
                    int curr = pos;
                    while (curr != -1) {
                        Vector2Int asd = gs.getPosFromIndex(curr);
                        Debug.Log($"Pos: {curr} X: {asd.x} Y: {asd.y}");
                        curr = path[curr];
                    }
                    break;
                }

                GameObject obj = gs.getGameObjectAtPosition(card.x, card.y);
                if (obj != null) {
                    continue;
                }
                else {
                    
                    Vector2Int[] next = {new Vector2Int(card.x + 1, card.y), new Vector2Int(card.x - 1, card.y), new Vector2Int(card.x, card.y + 1), new Vector2Int(card.x, card.y - 1)};
                    foreach (Vector2Int nx in next) {
                        int npos = gs.getPos(nx.x, nx.y);
                        if (gs.isValidPosition(nx.x, nx.y) && !visited.Contains(npos) && !path.ContainsKey(npos)) {
                            Debug.Log($"Add {npos}");
                            path.Add(npos, pos);
                            queue.Enqueue(gs.getPos(nx.x, nx.y), Mathf.Abs(tx - nx.x) + Mathf.Abs(ty - nx.y));
                        }
                    }                    
                }
            }

        } else
            Debug.LogError("No empty Tables!");



        */
        



        //for (int i = 0; i < levelHeight * levelWidth; i++) {
            
        //}
    }

    CustomerPatience getPhaseData(CustomerPhases pha) {
        foreach (CustomerPatience pat in customerPatience) {
            if (pat.phase == pha)
                return pat;
        }
        return null;
    }

    void setPhase(CustomerPhases pha) {
        currentPatience = 0f;
        currentPhase = getPhaseData(pha);
        progressbar.setIcon(currentPhase.icon);
    }

    void Update()
    {
        GameState gs = GameState.Instance;
        if (!gs.round)
            return;
        if (currentPhase.phase == CustomerPhases.WaitingForTable) {
            
            currentPatience += Time.deltaTime;
            if (currentPatience >= currentPhase.patience)
                gs.Lose();
            progressbar.setProgress(Mathf.RoundToInt((currentPatience / currentPhase.patience) * 100));
            if (gs.outsideQueue[0] == gameObject && gs.emptyTables.Count > 0) {

                table = gs.emptyTables[0];
                if (table != null) {
                    gs.emptyTables.Remove(table);
                }
                setPhase(CustomerPhases.WalkingToTable);
                gs.customerEnter(gameObject);
            }
        }
        if (currentPhase.phase == CustomerPhases.WalkingToTable) {
            Vector3 tablePos = table.transform.position;
            tablePos.x += gs.gridSize;
            tablePos.y = 1f;
            transform.position = tablePos;
            setPhase(CustomerPhases.ChoosingMeal);
        }
        if (currentPhase.phase == CustomerPhases.ChoosingMeal) {
            currentPatience += Time.deltaTime;
            if (currentPatience >= currentPhase.patience) {
                setPhase(CustomerPhases.WaitingToOrder);
            }
            progressbar.setProgress(Mathf.RoundToInt((currentPatience / currentPhase.patience) * 100));
        }
        if (currentPhase.phase == CustomerPhases.WaitingToOrder || currentPhase.phase == CustomerPhases.WaitingForFood || currentPhase.phase == CustomerPhases.WaitingToBeServed) {
            currentPatience += Time.deltaTime;
            if (currentPatience >= currentPhase.patience) {
                gs.Lose();
            }
            if (currentPhase.phase == CustomerPhases.WaitingForFood || currentPhase.phase == CustomerPhases.WaitingToBeServed) {
                GameObject tableObject = table.item;
                if (tableObject != null) {
                    Plate plate = tableObject.gameObject.GetComponent<Plate>();
                    if (plate != null) {
                        GameObject plateContent = plate.getContent();
                        if (plateContent != null) {
                            Item itm = plateContent.GetComponent<Item>();
                            if (itm != null && itm.getCurrentStageIndex() == mealState && itm.GetType() == meal.GetType()) {
                                setPhase(CustomerPhases.Eating);
                            }
                        }
                    }
                }
            }
            progressbar.setProgress(Mathf.RoundToInt((currentPatience / currentPhase.patience) * 100));
        }
        if (currentPhase.phase == CustomerPhases.Eating) {
            currentPatience += Time.deltaTime;
            if (currentPatience >= currentPhase.patience) {
                GameObject tableObject = table.item;
                if (tableObject != null) {
                    Plate plate = tableObject.gameObject.GetComponent<Plate>();
                    if (plate != null) {
                        plate.trash();
                    }
                }
                setPhase(CustomerPhases.Leaving);
                gs.emptyTables.Add(table);
                gs.score += 1;
                Debug.Log($"Done eating meal");
            }
            progressbar.setProgress(Mathf.RoundToInt((currentPatience / currentPhase.patience) * 100));
        }
        if (currentPhase.phase == CustomerPhases.Leaving) {
            Destroy(gameObject, 0f);
        }
    }

    public void interact() {
        GameState gs = GameState.Instance;
        if (currentPhase.phase == CustomerPhases.WaitingToOrder) {
            int mealItem = Random.Range(0, gs.menuItems.Length);
            int itemVariation = Random.Range(0, gs.menuItems[mealItem].states.Length);
            GameObject menuItem = gs.menuItems[mealItem].gameObject;
            GameObject it = Instantiate(menuItem, new Vector3(0f, 0f, 0f), Quaternion.identity);
            it.transform.SetParent(gameObject.transform);
            it.transform.localPosition = new Vector3(0f, 1.7f, 0f);
            Item item = it.GetComponent<Item>();
            mealState = gs.menuItems[mealItem].states[itemVariation];
            item.initStage = mealState;
            meal = item;
            
            setPhase(CustomerPhases.WaitingForFood);
        }
    }


    
    /*bool doit(int x, int y, List<Moves> moves){
        if (gs.isValidPosition(x, y) && visited.Contains(gs.getPos(x, y)))
            return false;
        visited.Add(gs.getPos(x, y));
    
        GameObject obj = gs.getGameObjectAtPosition(x, y);
        if (obj != null) {
            Table tab = obj.GetComponent<Table>();
            if (tab != null && !tab.isOccupied()) {
                return true;
            }
        }
        else {
            if (doit(x-1, y, moves)) {
                moves.Add(Moves.Left);
                return true;
            }
            if (doit(x+1, y, moves)) return Right;
            if (doit(x, y-1, moves)) return Down;
            if (doit(x, y+1, moves)) return Up;
        }
        
        return false;
    }

    bool isFreeTable(int x, int y) {

    }*/
    // Update is called once per frame
}
