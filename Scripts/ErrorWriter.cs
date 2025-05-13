using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;

public static class ErrorWriter
{
    private static string _path = @"D:\UnityProjects\PudgeMiniGame\Assets\Script";
    private static int _count = 0;
    //private static ErrorWriter _instance;

    // public static ErrorWriter GetInstance()
    // {
    //     return _instance == null ? _instance : new ErrorWriter();
    // }

    static ErrorWriter()
    {
        File.WriteAllText(_path, String.Empty);
    }
    
    public static void Write(string message)
    {
        using (StreamWriter writer = new StreamWriter(_path, true))
        {
            writer.WriteLine($"[{_count}]: " + message);
        }
    }
}
