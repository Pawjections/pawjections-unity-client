using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class KinecatReceiver : MonoBehaviour
{
    private UdpClient _client;
    private Thread _receiveThread;

    public int port = 9000;

    private float _lastReceivedTime = 0f;
    public float timeoutSeconds = 0.5f;

    public bool detected = false;

    private void Start()
    {
        _client = new UdpClient(port);

        _receiveThread = new Thread(new ThreadStart(ReceiveData))
        {
            IsBackground = true
        };
        _receiveThread.Start();
    }

    private void ReceiveData()
    {
        var anyIP = new IPEndPoint(IPAddress.Any, port);

        while (true)
        {
            var data = _client.Receive(ref anyIP);
            var text = Encoding.UTF8.GetString(data);
            
            _lastReceivedTime = Time.time;
            detected = true;
        }
    }

    private void Update()
    {
        if (Time.time - _lastReceivedTime > timeoutSeconds)
        {
            detected = false;
        }
    }

    private void OnApplicationQuit()
    {
        _receiveThread?.Abort();
        _client?.Close();
    }
}