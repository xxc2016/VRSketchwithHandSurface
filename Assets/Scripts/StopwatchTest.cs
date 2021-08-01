using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System;
using System.IO;

public class StopwatchTest : MonoBehaviour
{
    //计时
    Stopwatch stopwatch;
    TimeSpan timeSpan;
    String time;
    //计笔画数
    int strokesCount;
    //计表面数
    int surfacesCount;

    StreamWriter sw;//true追加，false覆盖
    // Start is called before the first frame update
    void Start()
    {

        sw = new StreamWriter("E:/data.csv", true);

        //第一行
        sw.Write("Time,");
        sw.Write("StrokesCount,");
        sw.WriteLine("SurfacesCount");

        stopwatch = new Stopwatch();
        strokesCount = 10;
        surfacesCount = 8;


        //刷新缓存
        sw.Flush();
        //关闭流
        sw.Close();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            stopwatch.Reset();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            stopwatch.Start();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            stopwatch.Stop();
            timeSpan = stopwatch.Elapsed;
            time = timeSpan.Hours.ToString() + ":" 
                + timeSpan.Minutes.ToString() + ":" 
                + timeSpan.Seconds.ToString() + ":"
                + timeSpan.Milliseconds.ToString();
            UnityEngine.Debug.Log(time);
            CountStrokes();
            WriteData();
            ScreenCapture.CaptureScreenshot("E:/screenshot"+Time.time +".png");

        }
    }

    private void WriteData()
    {

        sw = new StreamWriter("E:/data.csv", true);

        //第二行
        sw.Write(time + ",");
        sw.Write(strokesCount.ToString() + ",");
        sw.WriteLine(surfacesCount.ToString());

        //刷新缓存
        sw.Flush();
        //关闭流
        sw.Close();
    }

    private void CountStrokes() {
        strokesCount = 0;
        surfacesCount = GameObject.Find("Draw Surface").transform.childCount-1;
        for(int i = 0; i <= surfacesCount; i++) {
            strokesCount += GameObject.Find("Draw Surface").transform.GetChild(i).childCount;
        }
    }
}
