using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

/// <summary>
/// Class that allows a py file to be launched on the command line, opens windows bash
/// </summary>
public class StartLearning : MonoBehaviour
{

    /// <summary>
    /// Start the ML.py script on the command line, then start the learning on the dataset and the selected configuration.
    /// </summary>
    public void Learn(){
        Debug.Log("Python Starting");
        Debug.Log(FileUtils.GeneratePath());

        // Provide arguments
        string path = FileUtils.GeneratePath();
        string script = @"ML.py";        // Name file .py

        ProcessStartInfo pythonInfo = new ProcessStartInfo();

        pythonInfo.WorkingDirectory = path;
        pythonInfo.FileName = @"python";
        pythonInfo.Arguments = $"{script}";  // name of the file to be launched and possible arguments
        pythonInfo.CreateNoWindow = false;
        pythonInfo.UseShellExecute = false;

        UnityEngine.Debug.Log("Python Starting");

        try{
            using (Process process = Process.Start(pythonInfo)){
            }
        }
        catch (Exception e){
            Debug.Log(e);
        }
        UnityEngine.Debug.Log("Python Has finished!");
    }
}

