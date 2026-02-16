using UnityEngine;

public class SqueakPlayer : MonoBehaviour
{
    public AudioClip squeakSound;
    public float minSqueakInterval = 3f;
    public float maxSqueakInterval = 7f;
    
    private float _squeakTimer;
    private float _nextSqueakTime;
    private AudioSource _audioSource;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _nextSqueakTime = Random.Range(minSqueakInterval, maxSqueakInterval);
    }

    // Update is called once per frame
    void Update()
    {
        HandleSqueakTimer();
    }
    
    private void HandleSqueakTimer()
    {
        _squeakTimer += Time.deltaTime;

        if (_squeakTimer >= _nextSqueakTime)
        {
            _squeakTimer = 0f;

            if (squeakSound != null)
            {
                _audioSource.PlayOneShot(squeakSound);
            }

            // Set the next squeak interval randomly
            _nextSqueakTime = Random.Range(minSqueakInterval, maxSqueakInterval);
        }
    }
}
