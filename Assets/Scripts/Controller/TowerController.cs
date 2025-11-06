using System.Collections.Generic;
using UnityEngine;

public class TowerController : MonoBehaviour
{
    // Rotation Settings
    public float rotationSpeed = 60f;
    // this startPos variable is used to get the position of touch of the fingers 
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
    private Vector3 cameraStartPos;
    private Vector3 cameraTargetPos;
    private float cameraMoveStartTime;
    private float cameraMoveDuration = 0.5f;
    private bool isCameraMoving;

    public PlayerController playerController;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        SpawnInitialStacks();
        mainCamera = Camera.main;
    }

    // Instanitiating obstacle prefabs here
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
        if (playerController.isGameOver) return;

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

            // for smoothly transitiong over the cameraStartPos and cameraTargetPos over time t
            mainCamera.transform.position = Vector3.Lerp(cameraStartPos, cameraTargetPos, t);

            if (t >= 1f)
            {
                isCameraMoving = false;
            }
        }
    }

    void CheckStackClear()
    {
        if (!player) return;

        for (int i = stacks.Count - 1; i >= 0; i--)
        {
            if (stacks[i])
            {
                float playerY = player.transform.position.y;
                float stackY = stacks[i].transform.position.y;

                if (playerY < stackY)
                {
                    StackCleared(i);
                    GameManager.Instance.AddAndUpdateScore(10);
                    break;
                }
            }
        }
    }


    void StackCleared(int idx)
    {
        GameObject clearedStack = stacks[idx];

        // removing reference from List
        stacks.RemoveAt(idx);
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
            cameraStartPos = mainCamera.transform.position;
            cameraTargetPos = new Vector3(cameraStartPos.x, cameraStartPos.y - gapBetweenStacks, cameraStartPos.z);
            cameraMoveStartTime = Time.time;
            isCameraMoving = true;
        }
    }

    void RotateHelix(float swipeDistance)
    {
        float normalizedSwipe = Mathf.Clamp(swipeDistance, -100f, 100f);

        float rotationAmount = -normalizedSwipe * rotationSpeed * 0.017f;
        transform.Rotate(0f, rotationAmount, 0f);

    }

}
