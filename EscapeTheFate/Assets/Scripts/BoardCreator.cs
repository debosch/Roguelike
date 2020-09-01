using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardCreator : MonoBehaviour
{
    [SerializeField] 
    private class Count
    {
        public readonly int Minimum;
        public readonly int Maximum;

        public Count(int min, int max)
        {
            Minimum = min;
            Maximum = max;
        }
    };
    
    [SerializeField] private int columns = 8;
    [SerializeField] private int rows = 8;
    [SerializeField] private GameObject exit;
    [SerializeField] private GameObject[] floorTiles;
    [SerializeField] private GameObject[] innerWallsTiles;
    [SerializeField] private GameObject[] outerWallsTiles;
    [SerializeField] private GameObject[] foodTiles;
    [SerializeField] private GameObject[] enemyTiles;
    
    private readonly Count foodCount = new Count(1,5);
    private readonly Count wallCount = new Count(5,9);
    private readonly List<Vector3> gridPositions = new List<Vector3>();
    
    private Transform boardHolder;
    private int tempLevel = 0;

    private void InitiateList()
    {
        gridPositions.Clear();

        for (var x = 1; x < columns - 1; x++)
        {
            for (var y = 1; y < rows - 1; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    private void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;

        for (var x = -1; x < columns + 1; x++)
        {
            for (var y = -1; y < rows + 1; y++)
            {
                var floorTile = floorTiles[Random.Range(0, floorTiles.Length)];

                if (x == -1 || x == columns || y == -1 || y == rows)
                    floorTile = outerWallsTiles[Random.Range(0, outerWallsTiles.Length)];
                
                var instance = Instantiate(floorTile, new Vector3(x, y, 0f), Quaternion.identity);
                
                instance.transform.SetParent(boardHolder);
            }
        }
    }

    private Vector3 RandomPosition()
    {
        var randomIndex = Random.Range(0, gridPositions.Count);
        var randomPosition = gridPositions[randomIndex];
        
        gridPositions.RemoveAt(randomIndex);

        return randomPosition;
    }

    private void LayoutObjectAtRandomPosition(GameObject[] tileArray, int minimum, int maximum)
    {
        var objectCount = Random.Range(minimum, maximum + 1);

        for (var i = 0; i < objectCount; i++)
        {
            var randomPosition = RandomPosition();

            var tile = tileArray[Random.Range(0, tileArray.Length)];

            var instance = Instantiate(tile, randomPosition, Quaternion.identity);
            
            instance.transform.SetParent(boardHolder);
        }
    }

    public void SetupScene(int level)
    {
        InitiateList();
        BoardSetup();
        
        LayoutObjectAtRandomPosition(innerWallsTiles, wallCount.Minimum, wallCount.Maximum);
        LayoutObjectAtRandomPosition(foodTiles, foodCount.Minimum, foodCount.Maximum);

        var enemyCount = (int)Mathf.Log(level, 2f);
        
        LayoutObjectAtRandomPosition(enemyTiles, enemyCount, enemyCount);

        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);
    }

    private void ReloadScene()
    {
        Destroy(boardHolder.gameObject);
        Destroy(exit);
        SetupScene(tempLevel);
    }

    private void Start()
    {
        
    }

    
    private void Update()
    {
     
    }
}
