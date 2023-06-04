using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// File Management Class.
/// </summary>
public static class FileUtils{

    /// <summary>
    /// Path AppData.
    /// </summary>
    static string path = Application.persistentDataPath;

    /// <summary>
    /// Dataset filename.
    /// </summary>
    public static string filename = "marcosmiles_dataset.csv";

    /// <summary>
    /// Confusion matrix filename.
    /// </summary>
    public static string confusiongrid_filename = "confusion_grid_data.csv";

    /// <summary>
    /// Folder with datasets.
    /// </summary>
    static string folderName = "MyDatasets";

    /// <summary>
    /// Dataset name.
    /// </summary>
    public static string defaultFolder = "DefaultDataset";

    /// <summary>
    /// Selected dataset.
    /// </summary>
    public static string selectedDataset = defaultFolder;

    /// <summary>
    /// Confusion matrix dimensions.
    /// </summary>
    public static int matrixSize = 24;

    /// <summary>
    /// Function to get the dataset folder.
    /// </summary>
    /// <returns>Dataset folder name</returns>
    public static string GetFolderName(){
        return folderName;
    }

    /// <summary>
    /// Generates the path for the file to be used. the path consists of: path (The folder in the application appdata);
    /// folderName (Datasets folder) e filename.
    /// </summary>
    /// <param name="filename">Filename</param>
    /// <returns></returns>
    public static string GeneratePath(string filename){
        return $"{path}/{folderName}/{selectedDataset}/{filename}";
    }

    /// <summary>
    /// Generates the path to the file to be used from a specific folder.
    /// The path consists of: path (The application's appdata folder); folderName (Folder of datasets);
    /// folder (folder passed as parameter, within which you want to search) and filename (file name)
    /// </summary>
    /// <param name="filename">Filename</param>
    /// <param name="folder">Folder name</param>
    /// <returns></returns>
    private static string GeneratePath(string filename, string folder){
        return $"{path}/{folderName}/{folder}/{filename}";
    }

    /// <summary>
    /// Get dataset path.
    /// </summary>
    /// <returns>Dataset path</returns>
    public static string GeneratePath(){
        return $"{GeneratePath("")}";
    }

    /// <summary>
    /// Get dataset path.
    /// </summary>
    /// <param name="folder">Folder name</param>
    /// <returns>Dataset path</returns>
    public static string PrintPathFolder(string folder){
        return $"{GeneratePath("",$"{folder}")}";
    }

    /// <summary>
    /// Save the list of positions to file.
    /// </summary>
    /// <param name="data">List of positions</param>
    public static void Save(List<Position> data){

        System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
        customCulture.NumberFormat.NumberDecimalSeparator = ".";
        System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

        // Check if file exist
        if(File.Exists(GeneratePath(filename))){
            // If the file is in the path, create a string containing all the features and id of the note, to save to the file in the path

            foreach (var item in data){
                var str = $"{item.Left_Hand.FF1}, {item.Left_Hand.FF2}, {item.Left_Hand.FF3}, {item.Left_Hand.FF4}, {item.Left_Hand.FF5}," +
                    $" {item.Left_Hand.NFA1}, {item.Left_Hand.NFA2}, {item.Left_Hand.NFA3}, {item.Left_Hand.NFA4}," +
                    $" {item.Right_Hand.FF1}, {item.Right_Hand.FF2}, {item.Right_Hand.FF3}, {item.Right_Hand.FF4}, {item.Right_Hand.FF5}," +
                    $" {item.Right_Hand.NFA1}, {item.Right_Hand.NFA2}, {item.Right_Hand.NFA3}, {item.Right_Hand.NFA4}," +
                    $"{item.Left_Hand2.FF1}, {item.Left_Hand2.FF2}, {item.Left_Hand2.FF3}, {item.Left_Hand2.FF4}, {item.Left_Hand2.FF5}," +   // second device
                    $" {item.Left_Hand2.NFA1}, {item.Left_Hand2.NFA2}, {item.Left_Hand2.NFA3}, {item.Left_Hand2.NFA4}," +
                    $" {item.Right_Hand2.FF1}, {item.Right_Hand2.FF2}, {item.Right_Hand2.FF3}, {item.Right_Hand2.FF4}, {item.Right_Hand2.FF5}," +
                    $" {item.Right_Hand2.NFA1}, {item.Right_Hand2.NFA2}, {item.Right_Hand2.NFA3}, {item.Right_Hand2.NFA4}," +
                    $" {item.ID}"
                    .ToString(System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));

                //  New line to save the next features
                str += Environment.NewLine;

                //  Save the string into the file
                File.AppendAllText(GeneratePath(filename), str);
            }

            // Clean list position array
            _GM.list_posizioni.Clear();
            Debug.Log("DATASET SAVED");
        }else{

            //  File not exist
            //  Create the new file
            File.Create(GeneratePath(filename)).Dispose();

            //  Save positions on the file
            Save(data);
        }
    }

    /// <summary>
    /// Save the confusion matrix.
    /// </summary>
    /// <param name="confusionList"></param>
    public static void Save(int[,] confusionList){

        if(File.Exists(GeneratePath(confusiongrid_filename))){
            var str = "";
            for(int i = 0; i < matrixSize; i++){
                for(int j = 0; j < matrixSize; j++){
                    str += confusionList[i, j];
                    if (j != matrixSize-1)
                        str += ", ";
                }
                str += Environment.NewLine;
            }

            File.WriteAllText(GeneratePath(confusiongrid_filename), str);
            Debug.Log("CONFUSION MATRIX SAVED");
        }else{
            // File not exist
            File.Create(GeneratePath(confusiongrid_filename)).Dispose();

            // Save the confusion matrix.
            Save(confusionList);
        }
    }


    /// <summary>
    /// Load file content.
    /// </summary>
    /// <param name="name">Filename</param>
    /// <returns></returns>
    public static string LoadFile(string name)
    {
        //  Read the file
        if (File.Exists(GeneratePath($"{name}"))){
            return File.ReadAllText(GeneratePath($"{name}"));
        }else{
            // File not exist
            return null;
        }
    }

    /// <summary>
    /// Updates the trainedNotes variable, containing all trained notes (which are present in the dataset)
    /// </summary>
    /// <param name="filename"></param>
    public static void UpdateTrainedNotesList(string filename){
        var id_list = new List<int>();

        var txt = LoadFile(filename);

        if(txt != null){
            var rows = txt.Split('\n').Select(tag => tag.Trim()).Where(tag => !string.IsNullOrEmpty(tag));

            foreach (var item in rows){
                var tmp = int.Parse(item.Split(',').Last());        //  tmp = last element = ID

                if (!id_list.Any(x => id_list.Contains(tmp)))       // if the list of IDs does not contain tmp
                    id_list.Add(tmp);                               //  att tmp to IDs list
            }
        }
        //  Updated recorded notes list
        _GM.trainedNotes = id_list;
    }

    /// <summary>
    /// Delete note from dataset.
    /// </summary>
    /// <param name="note">Note</param>
    public static Task DeleteRowsNote(int note){
        return Task.Run(() =>{
            var filePath = GeneratePath(filename);
            var txt = LoadFile(filename);

            var rows = txt.Split('\n').Select(tag => tag.Trim()).Where(tag => !string.IsNullOrEmpty(tag));

            //  Delete all rows in dataset
            File.WriteAllText(filePath, "");

            // Write on dataset file the notes to be maintained

            foreach (var row in rows){
                int tmp_id = int.Parse(row.Split(',').Last());
                if (tmp_id != note){
                    var actualRow = "";
                    actualRow = row + "\n";
                    File.AppendAllText(filePath, actualRow);
                }
            }
            // Update recorded notes list
            UpdateTrainedNotesList(filename);
        });
    }


    /// <summary>
    /// Save text file.
    /// </summary>
    /// <param name="txt"></param>
    public static void SaveTxt(string txt) {
        var filePath = GeneratePath(filename);
        try{
            //  Create the file if not exist
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write)){
                File.AppendAllText(filePath, txt);
            }
        }
        catch (Exception ex){
            Debug.Log("Exception caught in process:" + ex.ToString());
        }
    }


    /// <summary>
    /// Converts bytes to a .py file and writes it into GeneratePath [AppData/LocalLow].
    /// </summary>
    /// <param name="file">File to convert</param>
    /// <param name="name">Name new file</param>
    public static void SavePy(byte[] file, string name)
    {
        //  Set name and extension
        string filename = name + ".py";
        string filePath = $"{path}/{folderName}/{filename}";

        try{
            if (!File.Exists(filePath)){
                using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write)){
                    fs.Write(file, 0, file.Length);
                }
            }else{
                Debug.Log("File already exist");
            }
        }
        catch (Exception ex){
            Debug.Log("Exception caught in process:" + ex.ToString());
        }
    }

    #region Import/Export

    /// <summary>
    /// Upload dataset
    /// </summary>
    /// <param name="path">Dataset path</param>
    /// <param name="destination">Destination path</param>
    public static void Import(string path, string destination){
        Debug.Log("Destination " + destination);
        Debug.Log("Path " + path);
        ProcessDirectory(path, destination, true);
    }

    /// <summary>
    /// Dataset Export
    /// </summary>
    /// <param name="path">Dataset path</param>
    /// <param name="destination">Destination path</param>
    public static void Export(string path, string destination){
        ProcessDirectory(path, destination);
    }

    #endregion

    #region Dataset Folder/Files

    /// <summary>
    /// Copy content of a folder into another folder.
    /// </summary>
    /// <param name="dir">Folder to copy</param>
    /// <param name="destination">Destination path</param>
    /// <param name="setDS">True if the input dataset is to be selected, false otherwise</param>
    private static void ProcessDirectory(string dir, string destination, bool setDS = false){
        Directory.CreateDirectory(destination);
        string[] files = Directory.GetFiles(dir);
        foreach (var item in files)
            ProcessFile(item, destination);
        string[] dirs = Directory.GetDirectories(dir);
        var tmp = dir.Split('/').ToList().Last();
        if(setDS)
            selectedDataset = tmp;
    }

    /// <summary>
    /// Save file into another destination
    /// </summary>
    /// <param name="file">File path</param>
    /// <param name="destination">Destination path</param>
    private static void ProcessFile(string file, string destination){
        var tmp = file.Split('/').ToList();
        var tmp1 = tmp.Last().Split('\\').ToList().Last();
        var filename = tmp1;
        string str = destination + '/' + filename;
        if (!File.Exists(str)){
            File.Copy(file, str);
        }
    }

    #endregion

    /// <summary>
    /// Check that all the necessary files are in place to enter the play session
    /// </summary>
    /// <returns>True if ok, false otherwise1</returns>
    public static bool CheckForDefaultFiles(){
        if (!Directory.Exists(GeneratePath())){
            Directory.CreateDirectory(GeneratePath());     
            return false;
        }else{
            if(!File.Exists(GeneratePath("ML.py")))
                File.Copy($"{path}/{folderName}/ML.py",GeneratePath("ML.py"));

            if (!File.Exists(GeneratePath("bias_out.txt")) || !File.Exists(GeneratePath("weights_out.txt"))){
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Delete all files in the selected dataset.
    /// </summary>
    public static void ClearDefaultDatasetDirectory(){
        if(Directory.Exists(GeneratePath())){
            var dir = new DirectoryInfo(GeneratePath());

            foreach(var file in dir.EnumerateFiles()){
                file.Delete();
            }

            foreach(var folder in dir.EnumerateDirectories()){
                folder.Delete(true);
            }
        }

        string nameFile = "ML";                                         //  Python file name.
        var MLFile = Resources.Load<TextAsset>("Text/" + nameFile);     //  Get script from resource folder (file .txt)
        SavePy(MLFile.bytes, MLFile.name);                              //  Convert .txt in script .py

        UpdateTrainedNotesList(filename);
    }
}