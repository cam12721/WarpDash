using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainMovement : MonoBehaviour
{
    public float despawnLimitX = -68;       // Terrain prefab will be destroyed after it reaches despawnLimitX
    private GameLogicScript _logic;
    private TerrainSpawner _spawner;

    // Start is called before the first frame update
    void Start()
    {
        _logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<GameLogicScript>();
        _spawner = GameObject.FindGameObjectWithTag("Spawner").GetComponent<TerrainSpawner>();

    }

    private void FixedUpdate()
    {
        // Move terrain to the left at speed set by moveSpeed
        transform.position += _logic.terrainMoveSpeed * Time.deltaTime * Vector3.left;
    }

    // Update is called once per frame
    void Update()
    {
        // Once this Column object has reached despawnLimitX, delete it and spawn a new one
        if (transform.position.x < despawnLimitX)
        {
            if (gameObject.CompareTag("Earth"))
            {
                if (_logic.infiniteTerrain)
                {
                    _spawner.SpawnEarthInfiniteRandom();
                    Destroy(gameObject);
                }
                else
                {
                    _spawner.SpawnEarthSequential();
                    Destroy(gameObject);
                }
            }
            else if (gameObject.CompareTag("Platform"))
            {
                if (_logic.infiniteTerrain)
                {
                    _spawner.SpawnPlatformInfiniteRandom();
                    Destroy(gameObject);
                }
                else
                {
                    _spawner.SpawnPlatformSequential();
                    Destroy(gameObject);
                }
            }
        }
    }
}
