using System;
using System.IO;
using UnityEngine;

public class Screenshotter : MonoBehaviour
{
    public KeyCode key = KeyCode.Pause;
    public string path = "Screenshots";

    private string prefix;
    private int shotsTaken = 0;
    private static Screenshotter _instance;

    private void Awake()
    {
        _instance = this.CheckSingleton(_instance);

        // prefix screenshots with the time that the current play session started
        prefix = DateTime.Now.ToString($"{Application.productName}-yyyyMMddhhmmss");
    }

    void Update()
    {
        if (Input.GetKeyDown(key))
        {
            string name = $"{prefix}{shotsTaken.ToString("D3")}.png";
            ScreenCapture.CaptureScreenshot(name);
            File.Move(name, Path.Combine(path, name));
            shotsTaken++;
            Debug.Log($"Screenshot taken: {name}");
        }
    }

    /// <summary>
    /// Takes a screenshot. If you couldn't guess.
    /// </summary>
    public static void Shoot()
    {
        string name = $"{_instance.prefix}{_instance.shotsTaken.ToString("D3")}.png";
        ScreenCapture.CaptureScreenshot(name);
        string path = Path.Combine(_instance.path, name);
        File.Move(name, path);
        _instance.shotsTaken++;
        Debug.Log($"Screenshot taken: {path}");
    }
}
