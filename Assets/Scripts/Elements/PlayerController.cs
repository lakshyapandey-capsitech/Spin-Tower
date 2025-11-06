using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float speed = 2f;
    public float bounceHeight = 0.8f;
    private Rigidbody playerRb;
    public Vector3 initialPos;
    public bool isGameOver = false;
    private bool isMovingDown = true;

    public float gameOverDelay = 0.2f;

    private AudioSource audioSource;
    public AudioClip bounceSound;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("PlayerController requires an AudioSource component on the GameObject.");
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        initialPos = transform.position;
        playerRb.useGravity = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isGameOver) return;

        if (collision.gameObject.CompareTag("Danger"))
        {
            // start the coroutine instead of calling OnGameOver directly, helps us to pause the game 
            StartCoroutine(HandleGameOverDelay());

            // set velocity to zero immediately for the stop effect
            playerRb.linearVelocity = Vector3.zero;
            return;
        }

        if (collision.gameObject.CompareTag("Safe"))
        {
            if (collision.contactCount > 0)
            {
                // used to get perperndicular direction
                Vector3 hitNormal = collision.contacts[0].normal;
                float angleThreshold = 0.8f;

                // dot product of both hitNormal and Vector3.Up gives the cosine values, to check the position of the obstacles for better accuracy
                if (Vector3.Dot(hitNormal, Vector3.up) > angleThreshold)
                {
                    isMovingDown = false;
                    initialPos = transform.position;

                    playerRb.linearVelocity = Vector3.zero;

                    if (audioSource != null && bounceSound != null)
                    {
                        audioSource.PlayOneShot(bounceSound);
                    }
                }
            }
        }
    }

    IEnumerator HandleGameOverDelay()
    {
        isGameOver = true;
        yield return new WaitForSeconds(gameOverDelay);

        // after the delay, call the game over logic
        GameManager.Instance?.OnGameOver();
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameOver) return;

        // if moving down, move player downward continuously
        if (isMovingDown)
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime);
        }

        // move upward until the position reaches initialPos.y + bounceHeight, used in bouncing of the player
        else if (transform.position.y < initialPos.y + bounceHeight)
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime);
        }

        // when reached the top of the bounce
        else
        {
            isMovingDown = true;
            initialPos = transform.position;
        }
    }
}