using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Spawn : MonoBehaviour
{
    public enum ObjectType { Hero1, Hero2, Hero3, Hero4, Monsters }

    [SerializeField]
    private Playercontroller playercontroller;
    [SerializeField]
    private Tilemap groundTilemap;

    public GameObject[] objectprefab;

    //[SerializeField]
    //private GameObject Hero;
    //[SerializeField]
    //private GameObject Monster;

    public float hero1Chance = 0.2f;
    public float hero2Chance = 0.2f;
    public float hero3Chance = 0.2f;
    public float hero4Chance = 0.2f;
    public float monsterChance = 0.2f;
    public int maxObject = 5;
    public float spawnInterval = 0.5f;
    public float heroLifeTime = 10f;

    private List<Vector3> goodSpawnPositions = new List<Vector3>();
    private List<GameObject> spawnObjects = new List<GameObject>();
    private bool isSpawning = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GatherGoodPosition();
    }

    // Update is called once per frame
    void Update()
    {
        if(!isSpawning && ActiveObjectCount() < maxObject)
        {
            StartCoroutine(SpawnIfNeed());
        }
    }

    private int ActiveObjectCount()
    {
        spawnObjects.RemoveAll(item => item == null);
        return spawnObjects.Count;
    }

    private IEnumerator SpawnIfNeed()
    {
        isSpawning = true;
        while(ActiveObjectCount() < maxObject)
        {
            spawnObject();
            yield return new WaitForSeconds(spawnInterval);
        }
        isSpawning = false;
    }

    private bool PositionHasObject(Vector3 positionToCheck)
    {
        return spawnObjects.Any(checkObj => checkObj && Vector3.Distance(checkObj.transform.position, positionToCheck) < 1.0f);
    }

    private ObjectType RandomObjectType()
    {
        float randomChoice = Random.value;

        if(randomChoice <= monsterChance)
        {
            return ObjectType.Monsters;
        }
        else if (randomChoice <= hero1Chance)
        {
            return ObjectType.Hero1;
        }
        else if (randomChoice <= (hero1Chance + hero2Chance))
        {
            return ObjectType.Hero2;
        }
        else if (randomChoice <= (hero1Chance + hero2Chance + hero3Chance))
        {
            return ObjectType.Hero3;
        }
        else if (randomChoice <= (hero1Chance + hero2Chance + hero3Chance + hero4Chance))
        {
            return ObjectType.Hero4;
        }
        else
        {
            return ObjectType.Monsters;
        }
    }

    private void spawnObject()
    {
        if (goodSpawnPositions.Count == 0) return;

        Vector3 spawnPosition = Vector3.zero;
        bool validPositionFound = false;

        while(!validPositionFound && goodSpawnPositions.Count > 0)
        {
            int randomIndex = Random.Range(0, goodSpawnPositions.Count);
            Vector3 potentialPosition = goodSpawnPositions[randomIndex];
            Vector3 leftPosition = potentialPosition + Vector3.left;
            Vector3 rightPosition = potentialPosition + Vector3.right;

            if(!PositionHasObject(leftPosition) && !PositionHasObject(rightPosition))
            {
                spawnPosition = potentialPosition;
                validPositionFound = true;
            }
            goodSpawnPositions.RemoveAt(randomIndex);
        }
        if(validPositionFound)
        {
            ObjectType objectType = RandomObjectType();
            GameObject gameObject = Instantiate(objectprefab[(int)objectType], spawnPosition, Quaternion.identity);
            spawnObjects.Add(gameObject);

            if(objectType != ObjectType.Monsters)
            {
                StartCoroutine(DestoryObjectAfterTime(gameObject, heroLifeTime));
            }
        }
    }

    private IEnumerator DestoryObjectAfterTime(GameObject gameObject, float time)
    {
        yield return new WaitForSeconds(time);

        if(gameObject)
        {
            spawnObjects.Remove(gameObject);
            goodSpawnPositions.Add(gameObject.transform.position);
            Destroy(gameObject);
        }
    }

    private void GatherGoodPosition() 
    {
        goodSpawnPositions.Clear();
        BoundsInt boundInt = groundTilemap.cellBounds;
        TileBase[] allTiles = groundTilemap.GetTilesBlock(boundInt);
        Vector3 start = groundTilemap.CellToWorld(new Vector3Int(boundInt.xMin, boundInt.yMin, 0));

        for(int x = 0; x < boundInt.size.x; x++)
        {
            for (int y = 0; y < boundInt.size.y; y++)
            {
                TileBase tile = allTiles[x + y * boundInt.size.x];
                if(tile != null)
                {
                    Vector3 place = start + new Vector3(x + 0.5f, y + 0.5f, 0);
                    goodSpawnPositions.Add(place);
                }
            }
        }
    }

    
}
