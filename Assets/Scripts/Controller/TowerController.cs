using System.Collections.Generic;
using UnityEngine;

public class TowerController : MonoBehaviour
{
    // Rotation Settings
    public float rotationSpeed = 0.1f;
    public Vector2 startPos;
    public bool isSwiping;

    // Stack and Player Management
    public GameObject stackPrefab;
    public float gapBetweenStacks = 2f;
    private GameObject player;
    private float lowestStackY;
    private List<GameObject> stacks = new List<GameObject>();

    // Camera Movement
    private Camera mainCamera;
    
    private Vector3 cameraTargetPos;
    private float cameraMoveStartTime;
    public float cameraMoveDuration = 0.5f;
    private bool isCameraMoving;

    public PlayerController playerController;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        SpawnInitialStacks();
        mainCamera = Camera.main;
    }

    void SpawnInitialStacks()
    {
        for (int i = 0; i < 4; i++)
        {
            Vector3 spawnPos = new Vector3(0, -i * gapBetweenStacks, 0);
            GameObject stack = Instantiate(stackPrefab, spawnPos, Quaternion.identity);

            if (i == 0)
            {
                stack.GetComponent<Obstacle>().GenerateRandomStack(true);
            }
            else
            {
                stack.GetComponent<Obstacle>().GenerateRandomStack();
            }

            stacks.Add(stack);
            // to maintain hierarchy, making the current gameObject the child of the TowerParent gameObject
            stack.transform.SetParent(transform);
        }

        lowestStackY = -3 * gapBetweenStacks;
    }

    void Update()
    {
        if (playerController.isGameOver || !GameManager.Instance.gameStarted) return;

        // For touch screens
        if (Input.touchCount > 0)
        {
            // to detect Active touches on the screen, 0 meant it's the first touch
            Touch touch = Input.GetTouch(0);

            // TouchPhase.Began is triggered once, the moment the user places their finger on the screen.
            if (touch.phase == TouchPhase.Began)
            {
                startPos = touch.position;
                isSwiping = true;
            }
            // rotation code here
            else if (touch.phase == TouchPhase.Moved && isSwiping)
            {
                RotateHelix(touch.position.x - startPos.x);
                startPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isSwiping = false;
            }
        }
        // For keys -> GetMouseButttonDown(0) means button is pressed now
        else if (Input.GetMouseButtonDown(0))
        {
            startPos = Input.mousePosition;
            isSwiping = true;
        }
        // button is continuously in pressing state and mouse is swiping(dragging) 
        else if (Input.GetMouseButton(0) && isSwiping)
        {
            RotateHelix(Input.mousePosition.x - startPos.x);
            startPos = Input.mousePosition;
        }
        // button gets released
        else if (Input.GetMouseButtonUp(0))
        {
            isSwiping = false;
        }

        CheckStackClear();

        if (isCameraMoving && mainCamera)
        {
            float t = (Time.time - cameraMoveStartTime) / cameraMoveDuration;
            if (t < 1f)
            {
                mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, cameraTargetPos, t);
            }
            else
            {
                mainCamera.transform.position = cameraTargetPos;
                isCameraMoving = false;
            }
        }
    }

    void CheckStackClear()
    {
        if (!player) return;

        for (int i = stacks.Count - 1; i >= 0; i--)
        {
            if (stacks[i] && player.transform.position.y < stacks[i].transform.position.y)
            {
                StackCleared(i);
                GameManager.Instance.AddAndUpdateScore(10);
                break;
            }
        }
    }

    void StackCleared(int index)
    {
        GameObject clearedStack = stacks[index];
        // removing reference from List
        stacks.RemoveAt(index);
        if (clearedStack)
        {
            // Removing the obstacle GameObject from scene
            Destroy(clearedStack);
        }

        lowestStackY -= gapBetweenStacks;

        // Qusternion.identity represents no rotation
        GameObject newStack = Instantiate(stackPrefab, new Vector3(0f, lowestStackY, 0f), Quaternion.identity, transform);
        newStack.GetComponent<Obstacle>().GenerateRandomStack();

        stacks.Add(newStack);

        if (mainCamera)
        {
            cameraTargetPos = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y - gapBetweenStacks, mainCamera.transform.position.z);
            cameraMoveStartTime = Time.time;
            isCameraMoving = true;
        }
    }

    void RotateHelix(float swipeDistance)
    {
        float rotation = -swipeDistance * rotationSpeed;
        transform.Rotate(0f, rotation, 0f);
    }
}
