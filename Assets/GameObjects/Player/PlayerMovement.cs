using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 6f;
    public float turnRate = 2f;
    GameObject carryObject = null;

    public float reach = 1.1f;

    public LayerMask layerMask;
    void Start()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, 9f, transform.position.z - 4f);
    }

    // Update is called once per frame

    Vector2Int getLookAtOffset() {
        switch (Mathf.RoundToInt(transform.rotation.eulerAngles.y / 45) % 8) {
            case 0: return new Vector2Int(0, 1);
            case 1: return new Vector2Int(1, 1);
            case 2: return new Vector2Int(1, 0);
            case 3: return new Vector2Int(1, -1);
            case 4: return new Vector2Int(0, -1);
            case 5: return new Vector2Int(-1, -1);
            case 6: return new Vector2Int(-1, 0);
            case 7: return new Vector2Int(-1, 1);
        }
        return new Vector2Int(0, 0);
    }

InteractableFurniture processingFurniture = null;
    void Update()
    {
        if (transform.position.y > 1.1f)
            transform.position = new Vector3 (transform.position.x, 1f, transform.position.z);
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 dir = new Vector3(horizontal, 0f, vertical).normalized;
        if (dir.magnitude >= 0.1f) {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dir), turnRate);
            controller.Move(dir * speed * Time.deltaTime);
            Camera.main.transform.position = new Vector3(transform.position.x, 9f, transform.position.z - 4f);
        }
        
        if (Input.GetButtonDown("Jump")) {
            GameState.Instance.round ^= true;
        }

        /*if (Input.GetButtonDown("Fire3")) {
            GameState gameState = GameState.Instance;
            Vector2Int offset = getLookAtOffset();
            int lx = Mathf.FloorToInt(transform.position.x) + offset.x;
            int ly = Mathf.FloorToInt(transform.position.z) + offset.y;
            GameObject obj = gameState.getGameObjectAtPosition(lx , ly);               
            if (obj != null) {
                InteractableFurniture ifr = obj.GetComponent<InteractableFurniture>();
                if (ifr != null) {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.GetComponent<Renderer>().material.color = new Color(1f, 0f, 0f);
                    cube.transform.localScale = new Vector3(0.8f, 0.1f, 0.8f);
                    if (ifr.PlaceItem(cube)) {
                    }
                }
            }                
        }*/

        if (Input.GetButtonDown("Fire3")) {
            GameState gameState = GameState.Instance;
            Vector3 reachPos = transform.position + transform.forward * reach;
            Collider[] hitColliders = Physics.OverlapBox(reachPos, new Vector3 (0.2f, 1f, 0.2f) , transform.rotation, layerMask);
            for (int i = 0; i < hitColliders.Length; i++)
            {
                Collider col = hitColliders[i];
                InteractableFurniture ifr = col.gameObject.GetComponent<InteractableFurniture>();
                if (ifr != null) {
                    col.transform.Rotate(0, 90, 0);
                    break;
                }                    
            }
        }

        

        if (processingFurniture != null) {
            GameState gameState = GameState.Instance;
            Vector3 reachPos = transform.position + transform.forward * reach;
            Collider[] hitColliders = Physics.OverlapBox(reachPos, new Vector3 (0.2f, 1f, 0.2f) , transform.rotation, layerMask);
            bool found = false;
            for (int i = 0; i < hitColliders.Length; i++)
            {
                Collider col = hitColliders[i];
                InteractableFurniture ifr = col.gameObject.GetComponent<InteractableFurniture>();
                if (ifr == processingFurniture) {
                    found = true;
                    break;
                }                    
            }
            if (!found) {
                processingFurniture.endProcess();
                processingFurniture = null;
            }
                
        }
            

        if (Input.GetButtonUp("Fire2")) {
            if (processingFurniture != null)
                processingFurniture.endProcess();
            processingFurniture = null;
        }

        if (Input.GetButtonDown("Fire2")) {
            GameState gameState = GameState.Instance;
            Vector3 reachPos = transform.position + transform.forward * reach;
            Collider[] hitColliders = Physics.OverlapBox(reachPos, new Vector3 (0.2f, 1f, 0.2f) , transform.rotation, layerMask);
            for (int i = 0; i < hitColliders.Length; i++)
            {
                Collider col = hitColliders[i];
                InteractableFurniture ifr = col.gameObject.GetComponent<InteractableFurniture>();
                if (ifr != null) {
                    processingFurniture = ifr;
                    processingFurniture.startProcess();
                    break;
                }

                Customer cust = col.gameObject.GetComponent<Customer>();
                if (cust != null) {
                    cust.interact();
                    break;
                }        
            }
        }


        if (Input.GetButtonDown("Fire1")) {
            GameState gameState = GameState.Instance;
            Vector3 reachPos = transform.position + transform.forward * reach;
            if (!gameState.round) {
                if (carryObject == null) {
                    Collider[] hitColliders = Physics.OverlapBox(reachPos, new Vector3 (0.2f, 1f, 0.2f) , transform.rotation, layerMask);
                    for (int i = 0; i < hitColliders.Length; i++)
                    {
                        Collider col = hitColliders[i];
                        InteractableFurniture ifr = col.gameObject.GetComponent<InteractableFurniture>();
                        if (ifr != null) {
                            carryObject = col.gameObject;
                            if (carryObject != null)
                            {
                                gameState.setGameObjectAtPosition(null, Mathf.FloorToInt(carryObject.transform.position.x / gameState.gridSize), Mathf.FloorToInt(carryObject.transform.position.z / gameState.gridSize));
                                carryObject.transform.SetParent(transform);
                                carryObject.transform.localPosition = new Vector3(0f,0f,1f);
                                carryObject.transform.rotation = transform.rotation;
                                
                            }
                            break;
                        }
                    }
                } else {
                    int lx = Mathf.FloorToInt(reachPos.x / gameState.gridSize);
                    int ly = Mathf.FloorToInt(reachPos.z / gameState.gridSize);
                    if (gameState.isValidPosition(lx, ly)) {
                        GameObject oldObj = gameState.setGameObjectAtPosition(carryObject, lx, ly);
                        carryObject.transform.parent = null;
                        carryObject = null;
                        if (oldObj) {
                            carryObject = oldObj;
                            oldObj.transform.SetParent(transform);
                            oldObj.transform.localPosition = new Vector3(0f,0f,1f);
                            oldObj.transform.rotation = transform.rotation;
                        }
                    }
                }
            } else {
                Collider[] hitColliders = Physics.OverlapBox(reachPos, new Vector3 (0.2f, 1f, 0.2f) , transform.rotation, layerMask);
                for (int i = 0; i < hitColliders.Length; i++)
                {
                    Collider col = hitColliders[i];
                    InteractableFurniture ifr = col.gameObject.GetComponent<InteractableFurniture>();
                    if (ifr != null) {
                        if (carryObject != null) {
                            if (ifr.PlaceItem(carryObject))
                                carryObject = null;
                        } else {
                            carryObject = ifr.PickUpItem();
                            if (carryObject != null) {
                                carryObject.transform.SetParent(transform);
                                carryObject.transform.localPosition = new Vector3(0f,0f,1f);
                                carryObject.transform.rotation = transform.rotation;
                            }
                        }
                        break;
                    }
                }
            }
        }

        /*if (Input.GetButtonDown("Fire1"))
        {
            GameState gameState = GameState.Instance;
            Vector2Int offset = getLookAtOffset();
            int lx = Mathf.FloorToInt(transform.position.x) + offset.x;
            int ly = Mathf.FloorToInt(transform.position.z) + offset.y;
            if (!gameState.round) {
                if (carryObject != null) {
                    if (gameState.isValidPosition(lx, ly)) {
                        GameObject oldObj = gameState.setGameObjectAtPosition(carryObject, lx, ly);
                        carryObject.transform.parent = null;
                        carryObject = null;
                        if (oldObj) {
                            carryObject = oldObj;
                            oldObj.transform.SetParent(transform);
                            oldObj.transform.localPosition = new Vector3(0f,0f,1f);
                            oldObj.transform.rotation = transform.rotation;
                        }
                    }
                } else {
                    GameObject obj = gameState.getGameObjectAtPosition(lx , ly );
                    if (obj != null)
                    {
                        carryObject = obj;
                        obj.transform.SetParent(transform);
                        obj.transform.localPosition = new Vector3(0f,0f,1f);
                        obj.transform.rotation = transform.rotation;
                        gameState.setGameObjectAtPosition(null, lx, ly);
                    }
                }
            } else {
                GameObject obj = gameState.getGameObjectAtPosition(lx , ly);
               
                if (obj != null) {
                    InteractableFurniture ifr = obj.GetComponent<InteractableFurniture>();
                    if (ifr != null) {
                        if (carryObject != null) {
                            if (ifr.PlaceItem(carryObject))
                                carryObject = null;
                        } else {
                            carryObject = ifr.PickUpItem();
                            if (carryObject != null) {
                                carryObject.transform.SetParent(transform);
                                carryObject.transform.localPosition = new Vector3(0f,0f,1f);
                                carryObject.transform.rotation = transform.rotation;
                            }
                        }
                    }
                }
            }
            
        }*/
    }

    void OnDrawGizmos()
    {
        Vector3 reachPos = transform.position + transform.forward * reach;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(reachPos, new Vector3 (0.2f, 1f, 0.2f));
    }
}
