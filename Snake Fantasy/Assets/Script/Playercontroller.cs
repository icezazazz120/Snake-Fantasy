using System.Collections.Generic;
using System.Security.Principal;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Playercontroller : MonoBehaviour
{
    [SerializeField]
    private Tilemap groundTilemap;
    [SerializeField]
    private Tilemap collisionTilemap;
    [SerializeField]
    private Tilemap[] herosTilemap;
    [SerializeField]
    private Tilemap monstersTilemap;

    private string currentDirection;
    private PlayerMovement controls;
    Vector2 inputMovement;

    public GameObject[] heroPrefabs;

    private List<Transform> heroLine = new List<Transform>();
    private List<Vector3> previousPositions = new List<Vector3>();
    private List<Transform> heroInLine = new List<Transform>();
    [SerializeField]
    private Text text;
    [SerializeField]
    private GameObject button;

    private bool isLose = false;

    private void Awake()
    {
        controls = new PlayerMovement();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Start()
    {
        controls.Main.Movement.performed += ctx => inputMovement = ctx.ReadValue<Vector2>();
        GameObject firstHero = Instantiate(heroPrefabs[0], transform.position, Quaternion.identity);
        heroLine.Add(firstHero.transform);
        previousPositions.Add(transform.position); 
    }

    private void Update()
    {
        if(!isLose)
        {
            directionKey();
        }
        CheckCollision();
    }

    private void FixedUpdate()
    {

    }

    private void Move(Vector2 direction)
    {
        if (CanMove(direction))
        {
            Vector3 currentHeadPos = transform.position;
            Vector3 nextPos = currentHeadPos + (Vector3)direction;

            previousPositions.Insert(0, currentHeadPos);
            if (previousPositions.Count > heroLine.Count)
                previousPositions.RemoveAt(previousPositions.Count - 1);

            transform.position = nextPos;

            for(int i = 0; i < heroLine.Count; i++)
            {
                heroLine[i].position = previousPositions[i];
            }

        }
    }

    private bool CanMove(Vector2 direction)
    {
        Vector3Int gridPosition = groundTilemap.WorldToCell(transform.position+(Vector3)direction);
        if (!groundTilemap.HasTile(gridPosition) || collisionTilemap.HasTile(gridPosition))
        {
            return false;
        }
        return true;
    }

    private void directionKey()
    {
        Vector2 direction;
        if ((Input.GetKeyDown(KeyCode.W) || inputMovement.y == 1) && currentDirection != "s")
        {
            direction = Vector3.up;
            currentDirection = "w";
            inputMovement.y = 0;
            Move(direction);
        }
        else if ((Input.GetKeyDown(KeyCode.S) || inputMovement.y == -1) && currentDirection != "w")
        {
            direction = Vector3.down;
            currentDirection = "s";
            inputMovement.y = 0;
            Move(direction);
        }
        else if ((Input.GetKeyDown(KeyCode.A) || inputMovement.x == -1) && currentDirection != "d")
        {
            direction = Vector3.left;
            currentDirection = "a";
            inputMovement.x = 0;
            Move(direction);
        }
        else if ((Input.GetKeyDown(KeyCode.D) || inputMovement.x == 1) && currentDirection != "a")
        {
            direction = Vector3.right;
            currentDirection = "d";
            inputMovement.x = 0;
            Move(direction);
        }

    }

    public void AddHero(int prefabIndex)
    {
        if (prefabIndex < 0 || prefabIndex >= heroPrefabs.Length) return;
        Vector3 spawnPos = heroLine[heroLine.Count - 1].position;
        GameObject newHero = Instantiate(heroPrefabs[prefabIndex], spawnPos, Quaternion.identity);
        newHero.tag = "HeroInLine";
        newHero.layer = 7;

        heroLine.Add(newHero.transform);
        previousPositions.Add(spawnPos);
    }

    public void CheckCollision()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, 0.1f, LayerMask.GetMask("Hero"));
        if (hit != null)
        {
            Debug.Log("hit");
            HeroPickup pickup = hit.GetComponent<HeroPickup>();
            if (pickup != null)
            {
                AddHero(pickup.prefabIndex);
            }
            Destroy(hit.gameObject);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("HeroInLine"))
        {
            isLose = true;
            button.gameObject.SetActive(true);
            text.text = "Game Over";
            Debug.Log("hitheroinline");
        }
    }
}
