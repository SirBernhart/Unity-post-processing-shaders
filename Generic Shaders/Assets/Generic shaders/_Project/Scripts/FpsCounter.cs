using UnityEngine;
using UnityEngine.UI;

public class FpsCounter : MonoBehaviour
{
    [SerializeField] private Text currFpsText;
    [SerializeField] private Text averageFpsText;
    [SerializeField] private float hudRefreshRate = 1f;

    // Attach this to a GUIText to make a frames/second indicator.
//
// It calculates frames/second over each updateInterval,
// so the display does not keep changing wildly.
//
// It is also fairly accurate at very low FPS counts (<10).
// We do this not by simply counting frames per interval, but
// by accumulating FPS for each frame. This way we end up with
// correct overall FPS even if the interval renders something like
// 5.5 frames.

    float updateInterval = 0.4f;
    private double accum = 0.0; // FPS accumulated over the interval
    private int frames = 0; // Frames drawn over the interval
    private int totalReadings = 0;
    private double timeleft; // Left time for current interval
    private double fps = 15.0f; // Current FPS
    private double lastSample;
    private int gotIntervals = 0;
    private double accumFps;

    private void Start()
    {
        timeleft = updateInterval;
        lastSample = Time.realtimeSinceStartup;
    }

    double GetFPS()
    {
        return fps;
    }

    bool HasFPS()
    {
        return gotIntervals > 2;
    }

    private void Update()
    {
        ++frames;
        var newSample = Time.realtimeSinceStartup;
        var deltaTime = newSample - lastSample;
        lastSample = newSample;

        timeleft -= deltaTime;
        accum += 1.0 / deltaTime;

        // Interval ended - update GUI text and start new interval
        if( timeleft <= 0.0 )
        {
            totalReadings++;
            // display two fractional digits (f2 format)
            fps = accum/frames;
            accumFps += fps;
            currFpsText.text = "FPS: " + ((int)fps).ToString();
            averageFpsText.text = "Average: " + ((int)accumFps/totalReadings).ToString();
            timeleft = updateInterval;
            accum = 0.0;
            frames = 0;
            ++gotIntervals;
        }
    }
}