using UnityEngine;

public class MouseMovement : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float rotationSpeed = 5f;
    [Tooltip("How close the cat must be before the mouse runs away")]
    public float flightDistance = 2f;
    [Tooltip("If cat overlaps the mouse within this distance, stop moving")]
    public float caughtDistance = 0.2f;

    private CatDetector catDetector;
    private bool _caught;

    private Vector2 _moveDirection;
    private Camera _mainCam;
    private Vector2 _screenBounds;

    public void SetCatDetector(CatDetector cd)
    {
        catDetector = cd;
    }

    private void Start()
    {
        _mainCam = Camera.main;
        SetScreenBounds();
        PickNewDirection();
    }

    private void Update()
    {
        if (_caught) return;
        UpdateDirection();
        MoveMouse();
        CheckBounds();
    }
    
    private void UpdateDirection()
    {
        Vector2 catPos = catDetector.GetCatWorldPosition();
        float distance = Vector2.Distance(transform.position, catPos);

        if (distance <= caughtDistance)
        {
            // Mouse caught!
            Debug.Log("Caught!");
            _caught = true;
            return;
        } 
        
        if (distance <= flightDistance)
        {
            // Run away from cursor
            _moveDirection = ((Vector2)transform.position - catPos).normalized;
        }
        // Else: keep current movement direction
    }

    private void MoveMouse()
    {
        // Move the mouse forward
        transform.Translate(_moveDirection * (moveSpeed * Time.deltaTime), Space.World);
        
        if (_moveDirection != Vector2.zero)
        {
            // Smooth rotation
            float targetAngle = Mathf.Atan2(_moveDirection.y, _moveDirection.x) * Mathf.Rad2Deg - 90f;
            Quaternion targetRotation = Quaternion.AngleAxis(targetAngle, Vector3.forward);

            // Smoothly rotate towards target
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
    
    private void CheckBounds()
    {
        Vector3 pos = transform.position;

        // If mouse leaves bounds, pick a new direction
        if (pos.x > _screenBounds.x || pos.x < -_screenBounds.x ||
            pos.y > _screenBounds.y || pos.y < -_screenBounds.y)
        {
            PickNewDirection();

            // Clamp back inside
            pos.x = Mathf.Clamp(pos.x, -_screenBounds.x, _screenBounds.x);
            pos.y = Mathf.Clamp(pos.y, -_screenBounds.y, _screenBounds.y);
            transform.position = pos;
        }
    }
    
    private void PickNewDirection()
    {
        float angle = Random.Range(0f, 360f);
        _moveDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
    }

    private void SetScreenBounds()
    {
        // World coordinates of screen corners
        Vector3 screenTopRight = _mainCam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        _screenBounds = new Vector2(screenTopRight.x - 0.5f, screenTopRight.y - 0.5f);
    }
}