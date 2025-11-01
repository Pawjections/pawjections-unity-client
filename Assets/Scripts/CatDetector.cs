using UnityEngine;
using UnityEngine.InputSystem;

public class CatDetector : MonoBehaviour
{
    private Camera _cam;

    private void Awake()
    {
        _cam = Camera.main;
    }

    public Vector2 GetCatWorldPosition()
    {
        Vector2 screenPos = Mouse.current.position.ReadValue();
        float zDist = Mathf.Abs(_cam.transform.position.z);
        return _cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, zDist));
    }
}
