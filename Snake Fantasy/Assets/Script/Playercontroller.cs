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
    //private PlayerMovement controls;

    //private void Awake()
    //{
    //    controls = new PlayerMovement();
    //}

    //private void OnEnable()
    //{
    //    controls.Enable();
    //}

    //private void OnDisable()
    //{
    //    controls.Disable();
    //}

    //private void Start()
    //{
    //    controls.Main.Movement.performed += ctx => Move(ctx.ReadValue<Vector2>());
    //}
    private void Update()
    {
        directionKey();
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
        if (Input.GetKeyDown(KeyCode.W) && currentDirection != "s")
        {
            direction = Vector3.up;
            currentDirection = "w";
            Move(direction);
        }
        else if (Input.GetKeyDown(KeyCode.S) && currentDirection != "w")
        {
            direction = Vector3.down;
            currentDirection = "s";
            Move(direction);
            Debug.Log(direction);
        }
        else if (Input.GetKeyDown(KeyCode.A) && currentDirection != "d")
        {
            direction = Vector3.left;
            currentDirection = "a";
            Move(direction);
        }
        else if (Input.GetKeyDown(KeyCode.D) && currentDirection != "a")
        {
            direction = Vector3.right;
            currentDirection = "d";
            Move(direction);
        }
    }
}
