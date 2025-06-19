using System.Collections.Generic;
using System.Security.Principal;
using UnityEngine;
using UnityEngine.Tilemaps;

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
        //    controls.Main.Movement.performed += ctx => Move(ctx.ReadValue<Vector2>());
        controls.Main.Movement.performed += ctx => inputMovement = ctx.ReadValue<Vector2>();
    }
private void Update()
    {
        directionKey();
        Debug.Log(inputMovement);
    }

    private void Move(Vector2 direction)
    {
        //Debug.Log(direction);
        if (CanMove(direction))
        {
            transform.position += (Vector3)direction;
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
            Debug.Log(direction);
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
}
