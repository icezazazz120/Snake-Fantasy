using System.Collections;
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
    [SerializeField]
    private Text text;
    [SerializeField]
    private Text statText;
    [SerializeField]
    private Text enemyText;
    [SerializeField]
    private GameObject button;

    private bool isControl = false;
    private bool isBattle = false;
    private bool canAttack = true;

    private GameObject currentEnemy;
    private float HeadHealth;
    private float HeadAttack;
    private float HeadDefense;

    private float EnemyHealth;
    private float EnemyAttack;
    private float EnemyDefense;

    private float CurrentHealth;

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
        HeroPickup headStats = firstHero.GetComponent<HeroPickup>();
        if (headStats != null)
        {
            HeadHealth = headStats.Health;
            HeadAttack = headStats.Attack;
            HeadDefense = headStats.Defense;
            CurrentHealth = HeadHealth;
        }
    }

    private void Update()
    {
        if (!isControl)
        {
            directionKey();
        }
        CheckCollision();
        if (isBattle && canAttack && (Input.anyKeyDown || inputMovement != Vector2.zero))
        {
            Battle();
        }
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

            for (int i = 0; i < heroLine.Count; i++)
            {
                if (i < previousPositions.Count)
                {
                    heroLine[i].position = previousPositions[i];
                }
            }

        }
    }

    private bool CanMove(Vector2 direction)
    {
        Vector3Int gridPosition = groundTilemap.WorldToCell(transform.position + (Vector3)direction);
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
        Collider2D hitHero = Physics2D.OverlapCircle(transform.position, 0.1f, LayerMask.GetMask("Hero"));
        Collider2D hitEnemy = Physics2D.OverlapCircle(transform.position, 0.1f, LayerMask.GetMask("Monster"));
        if (hitHero != null)
        {
            HeroPickup pickup = hitHero.GetComponent<HeroPickup>();
            HeroPickup headStats = heroLine[0].GetComponent<HeroPickup>();
            if (pickup != null && headStats != null)
            {
                AddHero(pickup.prefabIndex);
                HeadHealth = headStats.Health;
                HeadAttack = headStats.Attack;
                HeadDefense = headStats.Defense;
            }
            Destroy(hitHero.gameObject);
        }
        if (hitEnemy != null && !isBattle)
        {
            HeroPickup Enemy = hitEnemy.GetComponent<HeroPickup>();
            if (Enemy != null)
            {
                EnemyHealth = Enemy.Health;
                EnemyAttack = Enemy.Attack;
                EnemyDefense = Enemy.Defense;
                
                isBattle = true;
                isControl = true;
                currentEnemy = hitEnemy.gameObject;
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("HeroInLine"))
        {
            isControl = true;
            button.gameObject.SetActive(true);
            text.text = "Game Over";
        }
    }
    private void Battle()
    {
        canAttack = false;
        float headDamage = Mathf.Max(0, HeadAttack - EnemyDefense);
        float enemyDamage = Mathf.Max(0, EnemyAttack - HeadDefense);
        CurrentHealth -= enemyDamage;
        EnemyHealth -= headDamage;

        text.text = $"You dealt {headDamage} Damage!\nEnemy dealt {enemyDamage} Damage!";
        statText.text = $"HP: {CurrentHealth} \nATK: {HeadAttack} \nDEF: {HeadDefense}";
        enemyText.text = $"HP: {EnemyHealth} \nATK: {EnemyAttack} \nDEF: {EnemyDefense}";

        if (CurrentHealth <= 0)
        {
            Vector3 oldHeadPosition = heroLine[0].position;
            GameObject deadHero = heroLine[0].gameObject;
            Destroy(deadHero);
            heroLine.RemoveAt(0);
            UpdateHeroLineAfterDeath(oldHeadPosition);
            if (previousPositions.Count > 0)
                previousPositions.RemoveAt(0);

            if (heroLine.Count > 0)
            {
                CurrentHealth = HeadHealth;
                HeroPickup newStats = heroLine[0].GetComponent<HeroPickup>();
                HeadHealth = newStats.Health;
                HeadAttack = newStats.Attack;
                HeadDefense = newStats.Defense;
                statText.text = $"HP: {CurrentHealth} \nATK: {HeadAttack} \nDEF: {HeadDefense}";
                enemyText.text = $"HP: {EnemyHealth} \nATK: {EnemyAttack} \nDEF: {EnemyDefense}";
            }
            else
            {
                text.text = "Game Over!";
                button.SetActive(true);
                isBattle = false;
                return;
            }
        }

        if (EnemyHealth <= 0)
        {
            currentEnemy.layer = LayerMask.NameToLayer("Default");
            Destroy(currentEnemy);
            isBattle = false;
            isControl = false;
            canAttack = true;
            text.text = "";
            enemyText.text = "";
            currentEnemy = null;
            return;
        }

        StartCoroutine(WaitForNextTurn());

    }
    private IEnumerator WaitForNextTurn()
    {
        yield return new WaitForSeconds(0.1f);
        canAttack = true;
    }
    private void UpdateHeroLineAfterDeath(Vector3 oldHeadPosition)
    {
        List<Vector3> newPositions = new List<Vector3>();

        newPositions.Add(oldHeadPosition);

        for (int i = 1; i < heroLine.Count; i++)
        {
            if (i < previousPositions.Count)
                newPositions.Add(previousPositions[i - 1]);
            else
                newPositions.Add(heroLine[i].position);
        }

        for (int i = 0; i < heroLine.Count; i++)
        {
            heroLine[i].position = newPositions[i];
        }

        previousPositions = new List<Vector3>(newPositions);
    }
}