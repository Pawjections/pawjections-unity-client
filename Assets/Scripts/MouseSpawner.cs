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

        if (catDetectorInScene == null)
        {
            catDetectorInScene = FindObjectOfType<CatDetector>();
            if (catDetectorInScene != null)
                Debug.LogWarning("MouseSpawner: catDetectorInScene was not assigned in inspector — auto-found one in the scene.");
            else
                Debug.LogError("MouseSpawner: No CatDetector found in scene. Please assign one to the spawner or add a CatDetector to the scene.");
        }
    }

    private void Update()
    {
        HandleSpawning();
        CleanupDestroyedMice();
    }

    private void HandleSpawning()
    {
        _spawnTimer += Time.deltaTime;

        if (_activeMice.Count >= maxMice)
            return;

        if (_spawnTimer >= spawnInterval)
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
        if (catDetectorInScene == null)
        {
            Debug.LogWarning("AssignCatDetector skipped: no catDetectorInScene available.");
            return;
        }

        MouseMovement movement = mouse.GetComponent<MouseMovement>();
        if (movement != null)
        {
            movement.SetCatDetector(catDetectorInScene);
            // helpful debug to ensure assignment happened
            // Debug.Log($"Assigned CatDetector to {mouse.name}");
        }
        else
        {
            Debug.LogWarning("Spawned mouse prefab does not contain MouseMovement component.");
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float x = Random.Range(_screenBottomLeft.x + spawnMargin, _screenTopRight.x - spawnMargin);
        float y = Random.Range(_screenBottomLeft.y + spawnMargin, _screenTopRight.y - spawnMargin);

        return new Vector3(x, y, 0f);
    }

    private void CleanupDestroyedMice()
    {
        _activeMice.RemoveAll(m => m == null);
    }
}