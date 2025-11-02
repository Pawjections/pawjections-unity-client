using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

public class CatDetector : MonoBehaviour
{
    [Header("UDP")]
    public string listenIP = "127.0.0.1";
    public int listenPort = 9000;

    [Header("Coordinate Mode")]
    public bool useNormalized = true;
    public int projWidth = 1920; 
    public int projHeight = 1080;

    [Header("Smoothing")]
    [Range(0f, 1f)] public float lerp = 0.6f;

    [Serializable]
    public class Msg
    {
        public double t;
        public int id;
        public float conf;
        public float Ux, Uy;
        public float Ux_n, Uy_n;
        public float Bw, Bh;
    }

    UdpClient _udp;
    bool _running;

    readonly ConcurrentQueue<Msg> _queue = new ConcurrentQueue<Msg>();

    private Camera _cam;
    private Vector3 _catWorldPos;
    private bool _hasCatPos = false;

    void Awake()
    {
        _cam = Camera.main;
        Application.runInBackground = true;
    }

    void OnEnable() => StartUDP();
    void OnDisable() => StopUDP();

    void Update()
    {
        // Grab newest cat position
        if (_queue.TryDequeue(out var last))
        {
            float ux = useNormalized ? Mathf.Clamp01(last.Ux_n) : Mathf.Clamp(last.Ux / projWidth, 0f, 1f);
            float uy = useNormalized ? Mathf.Clamp01(last.Uy_n) : Mathf.Clamp(last.Uy / projHeight, 0f, 1f);

            // Convert to screen pixels
            float sx = ux * Screen.width;
            float sy = uy * Screen.height;
            
            // Flip Y: screen y goes from top to bottom
            sy = Screen.height - sy;

            // Convert to world position
            Vector3 screenPoint = new Vector3(sx, sy, Mathf.Abs(_cam.transform.position.z));
            Vector3 newWorldPos = _cam.ScreenToWorldPoint(screenPoint);

            _catWorldPos = Vector3.Lerp(_catWorldPos, newWorldPos, lerp);
            _hasCatPos = true;

            // Keep only the latest entry
            while (_queue.TryDequeue(out _)) { }
        }
    }

    void StartUDP()
    {
        if (_running) return;
        _running = true;

        _udp = new UdpClient(new IPEndPoint(IPAddress.Parse(listenIP), listenPort));
        _ = Task.Run(async () =>
        {
            while (_running)
            {
                try
                {
                    var res = await _udp.ReceiveAsync();
                    string json = Encoding.UTF8.GetString(res.Buffer);
                    var msg = JsonUtility.FromJson<Msg>(json);
                    _queue.Enqueue(msg);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"UDP receive error: {e.Message}");
                }
            }
        });
    }

    void StopUDP()
    {
        _running = false;
        try { _udp?.Close(); } catch { }
        _udp = null;
        while (_queue.TryDequeue(out _)) { }
    }

    public Vector2 GetCatWorldPosition()
    {
        if (!_hasCatPos)
            return transform.position; // fallback

        return _catWorldPos;
    }

    private void OnDrawGizmos()
    {
        if (_hasCatPos)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_catWorldPos, 0.1f);
        }
    }
}