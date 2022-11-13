using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState: MonoBehaviour
{
    public int levelWidth = 10;
    public int levelHeight = 5;

    public float gridSize = 2;
    GameObject[] levelObjects;

    public GameObject playerObject;

    public bool round = false;

    public enum Direction {UP, DOWN, LEFT, RIGHT}
    [System.Serializable]
    public class SpawnObject {
        public Vector2Int position;
        public Direction direction;
        public GameObject gameObject;
    }

    public SpawnObject[] spawnObjects;
    public GameObject customer;

    public List<Table> emptyTables = new List<Table>();

    public List<GameObject> outsideQueue = new List<GameObject>();

    [System.Serializable]
    public class MenuItem {
        public GameObject gameObject;
        public int[] states;
    }
    public MenuItem[] menuItems;
    private static GameState _instance;
    public static GameState Instance { get { return _instance; } }

    public int score = 0;
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    void Start()
    {
        float floorThickness = 0.1f;
        levelObjects = new GameObject[levelWidth * levelHeight];
        for (int i = 0; i < levelWidth * levelHeight; i++) {
            Vector2Int pos = getPosFromIndex(i);
            int x = pos.x;
            int y = pos.y;
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.transform.localScale = new Vector3(gridSize, floorThickness, gridSize);
            floor.transform.position = new Vector3(x * gridSize + gridSize / 2f, 0f - floorThickness / 2 , y * gridSize + gridSize / 2f);
            floor.name = $"Floor {i} X: {x} Y: {y}";
            floor.tag = "Floor";
            /*if (Random.Range(0, 10) == 5) {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.GetComponent<Renderer>().material.color = new Color(Random.Range(0, 1f),Random.Range(0, 1f),Random.Range(0, 1f));
                cube.AddComponent<InteractableFurniture>();
                setGameObjectAtPosition(cube, x,y);
            }*/
        }

        foreach (SpawnObject spawn in spawnObjects) {
            GameObject obj = Instantiate(spawn.gameObject, new Vector3(0f, 0f, 0f), Quaternion.identity);
            obj.transform.localScale = new Vector3(gridSize, 1f, gridSize);
            setGameObjectAtPosition(obj, spawn.position.x, spawn.position.y, spawn.direction); 
        }

        float doorZ = Mathf.Floor(levelHeight/2f) * gridSize + gridSize/2f;
        
        GameObject door = GameObject.CreatePrimitive(PrimitiveType.Cube);
        door.transform.localScale = new Vector3(gridSize, floorThickness, gridSize);
        door.transform.position = new Vector3(gridSize/2f - gridSize, 0f - floorThickness / 2 , doorZ);
        door.GetComponent<Renderer>().material.color = new Color(1f, 0f, 0f);
        door.name = $"Door";
        door.tag = "Floor";

        BoxCollider b1 = gameObject.AddComponent<BoxCollider>();
        b1.center = new Vector3 (gridSize/2f - gridSize * 2, 1, levelHeight * gridSize/ 2f);
        b1.size = new Vector3(gridSize, 1f, levelHeight * gridSize);

        BoxCollider d1 = gameObject.AddComponent<BoxCollider>();
        d1.center = new Vector3 (gridSize/2f - gridSize, 1, doorZ/2f - gridSize/2f);
        d1.size = new Vector3(gridSize, 1f, doorZ);

        BoxCollider d2 = gameObject.AddComponent<BoxCollider>();
        d2.center = new Vector3 (gridSize/2f - gridSize, 1, doorZ + doorZ/2f + gridSize/2f);
        d2.size = new Vector3(gridSize, 1f, doorZ);

        BoxCollider b2 = gameObject.AddComponent<BoxCollider>();
        b2.center = new Vector3 (levelWidth * gridSize + 0.5f, 1, levelHeight * gridSize/2f);
        b2.size = new Vector3(1f, 1f, levelHeight * gridSize);

        BoxCollider b3 = gameObject.AddComponent<BoxCollider>();
        b3.center = new Vector3 (levelWidth * gridSize / 2f, 1, -0.5f);
        b3.size = new Vector3(levelWidth * gridSize + 1f, 1f, 1f);
        
        BoxCollider b4 = gameObject.AddComponent<BoxCollider>();
        b4.center = new Vector3 (levelWidth * gridSize / 2f, 1, levelHeight * gridSize + 0.5f);
        b4.size = new Vector3(levelWidth * gridSize + 1f, 1f, 1f);

        Instantiate(playerObject, new Vector3(-0.5f, 1, doorZ), Quaternion.identity);
    }
    public float timeToSpawn = 15f;
    float lastSpawn = 0f;
    void Update() {
        if (round) {
            lastSpawn += Time.deltaTime;
            if ((timeToSpawn - score * 0.1f) < lastSpawn) {
                lastSpawn = Random.Range(-10f, 10f);
                addCustomer();
            }
        }
    }

    public void addCustomer() {
        outsideQueue.Add(Instantiate(customer, new Vector3(0f, 0f, 0f), Quaternion.identity));
        updateQueue();        
    }

    public void customerEnter(GameObject customer) {
        float doorZ = Mathf.Floor(levelHeight/2f) * gridSize + gridSize/2f;
        customer.transform.position = new Vector3(gridSize/2f - gridSize, 1f, doorZ);
        outsideQueue.Remove(customer);
        updateQueue();
    }

    public void updateQueue() {
        float doorZ = Mathf.Floor(levelHeight/2f) * gridSize + gridSize/2f;
        for (int i = 0; i < outsideQueue.Count; i++) {
            outsideQueue[i].transform.position = new Vector3(gridSize/2f - gridSize, 1f, doorZ - (gridSize * (1+i)));
        }
    }

    public int getPos(int x, int y) {
        return x + y * levelWidth;
    }

    public Vector2Int getPosFromIndex(int i) {
        return new Vector2Int(i % levelWidth, Mathf.FloorToInt(i / levelWidth));
    }

    public bool isValidPosition(int x, int y) {
        return x >= 0 && x < levelWidth && y >= 0 && y < levelHeight;
    }

    public GameObject setGameObjectAtPosition(GameObject obj, int x, int y) {
        return setGameObjectAtPosition(obj, x, y, Direction.DOWN);
    }

    public GameObject setGameObjectAtPosition(GameObject obj, int x, int y, Direction dir) {
        if (isValidPosition(x,y)) {
            int pos = getPos(x,y);
            GameObject currentObject = levelObjects[pos];
            levelObjects[pos] = obj;
            if (obj != null) {
                obj.transform.position = new Vector3(x * gridSize + gridSize / 2f, 0f, y * gridSize + gridSize / 2f);
                obj.transform.rotation = getRotationFromDirection(dir);
            }
            return currentObject;
        }
        return null;
    }

    Quaternion getRotationFromDirection(Direction dir) {
        if (dir == Direction.UP)
            return Quaternion.Euler(0, 0,0);
        if (dir == Direction.DOWN)
            return Quaternion.Euler(0,-180,0);
        if (dir == Direction.LEFT)
            return Quaternion.Euler(0,-90,0);
        if (dir == Direction.RIGHT)
            return Quaternion.Euler(0,90,0);
        return Quaternion.identity;
    }

    public GameObject getGameObjectAtPosition(int x, int y) {
        //Debug.Log($"X: {x} Y: {y}");
        if (isValidPosition(x,y)) {
            return getGameObjectAtIndex(getPos(x,y));
        }
        return null;
    }

    public GameObject getGameObjectAtIndex(int x) {
        if (levelObjects.Length > x) {
            return levelObjects[x];
        }
        return null;
    }

    public void Lose() {
        Debug.Log("LOST");
        round = false;
    }
}
