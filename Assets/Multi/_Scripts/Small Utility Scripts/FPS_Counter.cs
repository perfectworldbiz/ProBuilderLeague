using UnityEngine;
using System.Collections;

public class FPS_Counter : MonoBehaviour {

    public static FPS_Counter fpsCounter;
    float frameCount = 0;
    float dt = 0.0f;
    public float fps = 0.0f;
    float updateRate = 4.0f;  // 4 updates per sec.

    void Awake()
    {
        fpsCounter = this;
    }

    void Update()
    {
        frameCount++;
        dt += Time.deltaTime;
        if (dt > 1.0 / updateRate)
        {
            fps = frameCount / dt;
            frameCount = 0;
            dt -= 1.0f / updateRate;
        }
    }
}
