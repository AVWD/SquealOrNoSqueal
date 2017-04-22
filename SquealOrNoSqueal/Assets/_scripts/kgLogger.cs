using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class kgLogger : MonoBehaviour
{
    public enum PostLevel
    {
        quiet, standard, verbose
    }

    public enum PostChannel
    {
        ASSERT, ERROR, WARNING, INFO, GUI, GAMELOGIC, CONTENT, SYSTEM, INPUT, GRAPHICS
    }

    protected static kgLogger _instance;
    private static uint _logIdx = 0;
    private static string _filename = string.Format("{0}{1}Log{2}log-{3:MM-dd-yyyy hh-mm-ss-tt}.htm", Environment.CurrentDirectory, Path.DirectorySeparatorChar, Path.DirectorySeparatorChar, DateTime.Now);
    private static readonly string _entry = @"<div class='{0} post' id='log_{1}'><div class='timeStamp'>{2} {3}</div><div class='channel'>{0}</div><a class='stackToggle' href='#'>STACK</a><div class='message'>{4}</div><div class='stackMsg'><div><pre>{5}</pre></div></div></div>";
    private static readonly string _entryQuiet = @"<div class='{0} post quiet' id='log_{1}'><div class='timeStamp'>{2} {3}</div><div class='message'>{4}</div></div>";

    public static kgLogger instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject loggerObject = new GameObject("Logger");
                kgLogger component = loggerObject.AddComponent<kgLogger>();
                _instance = component;
                UnityEngine.Object.DontDestroyOnLoad(loggerObject);
            }
            return _instance;
        }
    }

    // Use this for initialization
    void Start()
    {
        using (TextWriter tw = new StreamWriter(_filename))
        {
            tw.WriteLine("<link rel='stylesheet' href='assets/css/default.css'>");
            tw.WriteLine("<script src='assets/js/jq.min.js'></script>");
            tw.WriteLine("<script src='assets/js/default.js'></script>");
            tw.WriteLine("<div id='channelContainer' class='channels'></div><h1>LOG - {0} {1}</h1>", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString());
        }
    }

    public void Post(string channel, string message, PostLevel level = PostLevel.standard)
    {
        string stack = (level == PostLevel.verbose) ? UnityEngine.StackTraceUtility.ExtractStackTrace() : string.Empty;

        using (TextWriter tw = new StreamWriter(_filename, true))
        {
            if (level == PostLevel.quiet)
            {
                tw.WriteLine(_entryQuiet, channel, _logIdx, DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString(), message);
            }
            else
            {
                tw.WriteLine(_entry, channel, _logIdx, DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString(), message, stack);
            }
        }

        _logIdx++;
    }

    public void Post(PostChannel channel, string message, PostLevel level = PostLevel.standard)
    {
        string stack = (level == PostLevel.verbose) ? UnityEngine.StackTraceUtility.ExtractStackTrace() : string.Empty;

        using (TextWriter tw = new StreamWriter(_filename, true))
        {
            if (level == PostLevel.quiet)
            {
                tw.WriteLine(_entryQuiet, channel, _logIdx, DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString(), message);
            }
            else
            {
                tw.WriteLine(_entry, channel, _logIdx, DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString(), message, stack);
            }
        }

        _logIdx++;
    }
}
