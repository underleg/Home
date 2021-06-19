using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Unity.Profiling;
using TMPro;

public class ProfilerMB : MonoBehaviour
{
    public static ProfilerMarker m_profileTimer1 = new ProfilerMarker("Profile Timer 1");

    int m_updateCount = 0;

    string statsText;
    ProfilerRecorder systemMemoryRecorder;
    ProfilerRecorder gcMemoryRecorder;
    ProfilerRecorder vidMemoryRecorder;
    ProfilerRecorder mainThreadTimeRecorder;
    ProfilerRecorder drawCallsRecorder;
    ProfilerRecorder updateTimer1Recorder;

    public TextMeshPro m_debugText;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(m_debugText);
    }

    static double GetRecorderFrameAverage(ProfilerRecorder recorder)
    {
        var samplesCount = recorder.Capacity;
        if (samplesCount == 0)
            return 0;

        double r = 0;
        
        //unsafe
        {
            var samples = new List<ProfilerRecorderSample>();
            recorder.CopyTo(samples);

            for (var i = 0; i < samples.Count; ++i)
            {
                r += samples[i].Value;
            }
            r /= samples.Count;
        }

        return r;
    }

    void OnEnable()
    {
        systemMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "System Used Memory");
        gcMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Reserved Memory");
        vidMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Video Used Memory");
        mainThreadTimeRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "Main Thread", 15);
        drawCallsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count");
        updateTimer1Recorder = ProfilerRecorder.StartNew(m_profileTimer1);
    }

    void OnDisable()
    {
        systemMemoryRecorder.Dispose();
        gcMemoryRecorder.Dispose();
        vidMemoryRecorder.Dispose();
        mainThreadTimeRecorder.Dispose();
        drawCallsRecorder.Dispose();
        updateTimer1Recorder.Dispose();
    }

    void Update()
    {
        if (m_updateCount == 0)
        {
            double fms = GetRecorderFrameAverage(mainThreadTimeRecorder) * (1e-6f);
            double fps = 0.0f;
            if(fms > 0)
            {
                fps = 1000.0f / fms;
            }

            //fms = 60 / fms;
            string debugStr = "Frame Time: " + fms.ToString("F2") + "ms (" + fps.ToString("F0") + ")\n";
            debugStr += "GC Memory: " + gcMemoryRecorder.LastValue / (1024 * 1024) + "MB\n";
            debugStr += "Vid Memory: " + vidMemoryRecorder.LastValue / (1024 * 1024) + "MB\n";    
            debugStr += "Sys Memory: " + systemMemoryRecorder.LastValue / (1024 * 1024) + "MB\n";
            debugStr += "Draw Calls: " + GetRecorderFrameAverage(drawCallsRecorder) + "\n";

            fms = GetRecorderFrameAverage(updateTimer1Recorder) * (1e-6f);
            debugStr += "Timer 1: " + fms.ToString("F4") + "ms\n";

            m_debugText.SetText(debugStr);
        }

        m_updateCount = (m_updateCount + 1) % 100;
    }

}
