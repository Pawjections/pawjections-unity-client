using System.Collections.Generic;
using UnityEngine;

public class MouseSpawner : MonoBehaviour
{
    public GameObject mousePrefab;
    public int maxMice = 3;
    public float spawnInterval = 5f;
    
    private float _spawnTimer;
    private Camera _mainCam;
    private readonly List<GameObject> _activeMice = new List<GameObject>();
    
    [SerializeField] private CatDetector catDetectorInScene;
    [SerializeField] private float spawnMargin = 1f;
    private Vector3 _screenTopRight;
    private Vector3 _screenBottomLeft;

    private void Start()
    {
        _mainCam = Camera.main;
        _screenTopRight = _mainCam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        _screenBottomLeft = _mainCam.ScreenToWorldPoint(Vector3.zero);
    }

    private void Update()
    {
        HandleSpawning();
        CleanupDestoryedMice();
    }

    private void HandleSpawning()
    {
        _spawnTimer += Time.deltaTime;

        bool canSpawn = (_activeMice.Count < maxMice);

        if (canSpawn && _spawnTimer >= spawnInterval)
        {
            _spawnTimer = 0f;
            SpawnMouse();
        }
    }

    private void SpawnMouse()
    {
        Vector3 spawnPos = GetRandomSpawnPosition();
        GameObject mouse = Instantiate(mousePrefab, spawnPos, Quaternion.identity);
        _activeMice.Add(mouse);

        AssignCatDetector(mouse);
    }

    private void AssignCatDetector(GameObject mouse)
    {
        // Assign the cat detector to the spawned mouse
        MouseMovement movement = mouse.GetComponent<MouseMovement>();
        if (movement != null && catDetectorInScene != null)
        {
            movement.SetCatDetector(catDetectorInScene);
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float x = Random.Range(_screenBottomLeft.x + spawnMargin, _screenTopRight.x - spawnMargin);
        float y = Random.Range(_screenBottomLeft.y + spawnMargin, _screenTopRight.y - spawnMargin);

        return new Vector3(x, y, 0f);
    }
    
    private void CleanupDestoryedMice()
    {
        // Clean up destroyed mice from the list
        _activeMice.RemoveAll(m => m == null);
    }
}
