using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TerrainSpawner : MonoBehaviour
{
    private GameLogicScript _logic;

    // Array stores terrain prefabs entered via the inspector
    public GameObject[] earthArray;
    public GameObject[] platformArray;

    // Count the number of new earth and platform prefabs spawned
    private uint _earthCount = 0;
    private uint _platformCount = 0;

    // Store the shuffled indexes that are used to access terrain prefabs in earthArray amd platformArray
    private List<int> _earthIndexes = new List<int>();
    private List<int> _platformIndexes = new List<int>();

    // Use as indexes to access _earthIndexes and _platformIndexes
    private int _earthi = 0;
    private int _platformi = 0;

    // Flag if _earthIndexes and _platformIndexes have been shuffled
    private bool _shuffledEarthIndexes = false;
    private bool _shuffledPlatformIndexes = false;


    // Start is called before the first frame update
    void Start()
    {
        _logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<GameLogicScript>();
        if (_logic.infiniteTerrain)
        {
            SpawnEarthInfiniteRandom();
            SpawnPlatformInfiniteRandom();
        }
        else
        {
            SpawnEarthSequential();
            SpawnPlatformSequential();
        }
    }

    private void Update()
    {
        
    }

    // Spawn each of the 9 earth prefabs once in order
    public void SpawnEarthSequential()
    {
        if (_earthCount < 9)
        {
            Instantiate(earthArray[_earthCount], new Vector3(transform.position.x, transform.position.y, 0), transform.rotation);
            _earthCount++;
            Debug.Log("Earth prefabs spawned: " + _earthCount);
        }
    }

    // Spawn each of the 9 platform prefabs once in order
    public void SpawnPlatformSequential()
    {
        if (_platformCount < 9)
        {
            Instantiate(platformArray[_platformCount], new Vector3(transform.position.x, transform.position.y, 0), transform.rotation);
            _platformCount++;
            Debug.Log("Platform prefabs spawned: " + _platformCount);
        }
    }

    // Spawn earth prefabs infinitely in random, non-repeating permutations. Prefabs are spawned in phases. The terrain move speed is increased after
    // each phase
    public void SpawnEarthInfiniteRandom()
    {
        if (_earthCount < 3)
        {
            // Phase 1: Each of the first 3 earth prefabs are spawned in random non-repeating permutations.
            if (_earthCount == 0)
            {
                _earthIndexes.Add(0);
                _earthIndexes.Add(1);
                _earthIndexes.Add(2);
                Shuffle(ref _earthIndexes);
            }
            Instantiate(earthArray[_earthIndexes[_earthi]], new Vector3(transform.position.x, transform.position.y, 0), transform.rotation);
            _earthCount++;
            _earthi++;
            Debug.Log("Earth prefabs spawned: " + _earthCount);
            
        }
        else if (_earthCount < 9)
        {
            // Phase 2: Each of the first 6 earth prefabs are spawned in random non-repeating permutations.
            if (_earthCount == 3)
            {
                _earthi = 0;
                _earthIndexes.Add(3);
                _earthIndexes.Add(4);
                _earthIndexes.Add(5);
                Shuffle(ref _earthIndexes);
                if (_logic.terrainMoveSpeed < _logic.terrainTopSpeed)
                    _logic.terrainMoveSpeed += _logic.terrainSpeedIncrease;
            }
            Instantiate(earthArray[_earthIndexes[_earthi]], new Vector3(transform.position.x, transform.position.y, 0), transform.rotation);
            _earthCount++;
            _earthi++;
            Debug.Log("Earth prefabs spawned: " + _earthCount);
        }
        else if (_earthCount < 18)
        {
            // Phase 3: All 9 of the earth prefabs are spawned in random non-repeating permutations.
            if (_earthCount == 9)
            {
                _earthi = 0;
                _earthIndexes.Add(6);
                _earthIndexes.Add(7);
                _earthIndexes.Add(8);
                Shuffle(ref _earthIndexes);
                if (_logic.terrainMoveSpeed < _logic.terrainTopSpeed)
                    _logic.terrainMoveSpeed += _logic.terrainSpeedIncrease;
            }
            Instantiate(earthArray[_earthIndexes[_earthi]], new Vector3(transform.position.x, transform.position.y, 0), transform.rotation);
            _earthCount++;
            _earthi++;
            Debug.Log("Earth prefabs spawned: " + _earthCount);
        }
        else
        {
            // Phase 4: All 9 of the earth prefabs are spawned in random non-repeating permutations. This process repeats infinitely.
            if (!_shuffledEarthIndexes)
            {
                _earthi = 0;
                Shuffle(ref _earthIndexes);
                _shuffledEarthIndexes = true;
                if (_logic.terrainMoveSpeed < _logic.terrainTopSpeed)
                    _logic.terrainMoveSpeed += _logic.terrainSpeedIncrease;
            }
            Instantiate(earthArray[_earthIndexes[_earthi]], new Vector3(transform.position.x, transform.position.y, 0), transform.rotation);
            _earthCount++;
            _earthi++;
            Debug.Log("Earth prefabs spawned: " + _earthCount);
            if (_earthi == _earthIndexes.Count - 1)
            {
                // After all 9 earth prefabs have spawned, reshuffle the plaftorm indexes and start again 
                _earthi = 0;
                _shuffledEarthIndexes = false;
            }
        }
    }

    // Spawn platform prefabs infinitely in random, non-repeating permutations. Prefabs are spawned in phases.
    public void SpawnPlatformInfiniteRandom()
    {
        // Phase 1: Each of the first 3 platform prefabs are spawned in random non-repeating permutations.
        if (_platformCount < 3)
        {
            if (_platformCount == 0)
            {
                _platformIndexes.Add(0);
                _platformIndexes.Add(1);
                _platformIndexes.Add(2);
                Shuffle(ref _platformIndexes);
            }
            Instantiate(platformArray[_platformIndexes[_platformi]], new Vector3(transform.position.x, transform.position.y, 0), transform.rotation);
            _platformCount++;
            _platformi++;
            Debug.Log("Platform prefabs spawned: " + _platformCount);
        }
        else if (_platformCount < 9)
        {
            // Phase 2: Each of the first 6 platform prefabs are spawned in random non-repeating permutations.
            if (_platformCount == 3)
            {
                _platformi = 0;
                _platformIndexes.Add(3);
                _platformIndexes.Add(4);
                _platformIndexes.Add(5);
                Shuffle(ref _platformIndexes);
            }
            Instantiate(platformArray[_platformIndexes[_platformi]], new Vector3(transform.position.x, transform.position.y, 0), transform.rotation);
            _platformCount++;
            _platformi++;
            Debug.Log("Platform prefabs spawned: " + _platformCount);
        }
        else if (_platformCount < 18)
        {
            // Phase 3: All 9 of the platform prefabs are spawned in random non-repeating permutations.
            if (_platformCount == 9)
            {
                _platformi = 0;
                _platformIndexes.Add(6);
                _platformIndexes.Add(7);
                _platformIndexes.Add(8);
                Shuffle(ref _platformIndexes);
            }
            Instantiate(platformArray[_platformIndexes[_platformi]], new Vector3(transform.position.x, transform.position.y, 0), transform.rotation);
            _platformCount++;
            _platformi++;
            Debug.Log("Platform prefabs spawned: " + _platformCount);
        }
        else
        {
            // Phase 4: All 9 of the platform prefabs are spawned in random non-repeating permutations. This process repeats infinitely.
            if (!_shuffledPlatformIndexes)
            {
                _platformi = 0;
                Shuffle(ref _platformIndexes);
                _shuffledPlatformIndexes = true;
            }
            Instantiate(platformArray[_platformIndexes[_platformi]], new Vector3(transform.position.x, transform.position.y, 0), transform.rotation);
            _platformCount++;
            _platformi++;
            Debug.Log("Platform prefabs spawned: " + _platformCount);
            if (_platformi == _platformIndexes.Count - 1)
            {
                // After all 9 platform prefabs have spawned, reshuffle the plaftorm indexes and start again 
                _platformi = 0;
                _shuffledPlatformIndexes = false;
            }
        }
    }

    // Fisher-Yates shuffle algorithm
    private void Shuffle(ref List<int> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            int k = rng.Next(n--);
            int temp = list[n];
            list[n] = list[k];
            list[k] = temp;
        }
    }
}
