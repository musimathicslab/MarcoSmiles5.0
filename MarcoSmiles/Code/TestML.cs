using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class to perform machine learning operations.
/// </summary>
public static class TestML
{
    private static int delaytime = 12;
    private static int counter = -1;
    private static int oldIndexToSend=0;   // -1
    private static int toCompare = 0;

    /// <summary>
    /// Weights, layer 1.
    /// </summary>
    private static List<List<double>> W1 = new List<List<double>>();

    /// <summary>
    /// Bias, layer 1.
    /// </summary>
    private static List<double> B1 = new List<double>();

    /// <summary>
    /// Weights, layer 2.
    /// </summary>
    private static List<List<double>> W2 = new List<List<double>>();

    /// <summary>
    /// Bias, layer 2.
    /// </summary>
    private static List<double> B2 = new List<double>();

    /// <summary>
    /// Last training for the selected configuration.
    /// </summary>
    public static System.DateTime DateLatestLearning;

    /// Temp variable to contain min and max.
    private static string[] readText;

    /// <summary>
    /// Min value for each feature in the dataset.
    /// </summary>
    private static string[] min;

    /// <summary>
    /// Max value for each feature in the dataset.
    /// </summary>
    private static string[] max;


   /// <summary>.
    /// ReadArraysFromFormattedFile returns a list of float arrays. The arrays are inserted into the list in the order in which they are read from the file.
    /// The list is formatted as follows:
    /// - As long as vectors are read from the file, then these vectors are added to the list and returned.
    /// - As soon as the function realises it has read an array, it inserts an empty array [] into the list.
    ///
    /// For example:
    /// - in the file bias.txt, arrays are contained. Each array represents a bias:
    /// The first array represents B1; the second array B2 etc...
    ///
    /// - in the weights.txt file, arrays are contained, each containing a list of arrays. So all the arrays contained in the returned list,
    /// as long as an empty array [] is not read, will be part of the first array read.
    /// In this way, we format the list into n array blocks, where n is the number of arrays contained in the file:
    /// All arrays contained in the first array block (those found before the first empty array[]) represent the array W1,
    /// All arrays contained in the second array block (those found after the first empty array[] and before the second empty array [])
    /// represent array W2
    /// etc...
    /// </summary>
    /// <param name="name">Name of the file to be opened</param>
    /// <returns>
    /// A float list containing the numbers read from the file. The list is formatted logically so that any arrays contained in the file can be constructed.
    /// </returns>
    private static List< List<double> > ReadArraysFromFormattedFile(string name){

        var text = FileUtils.LoadFile(name);

        if (text == null)
            return null;

        DateLatestLearning = File.GetLastWriteTime(FileUtils.GeneratePath(name));

        text = text.Replace("[", "");
        text = text.Replace("\n", "");

        string[] vettori = text.Split(']');       // StringSplitOptions.RemoveEmptyEntries remove empty entry

        List< List <string> > temp = new List< List<string> >();

        foreach (string vettore in vettori){
            temp.Add(vettore.Split(' ')                                       // split by ' '
                .Select(tag => tag.Trim())                                    // remove empty entry
                .Where(tag => !string.IsNullOrEmpty(tag)).ToList());
        }

        List<List<double>> listOfReadArrays = new List< List<double> >();

        // Convert values from string to float
        foreach ( var arr in temp){
            double[] t = new double[arr.Count];
            for (int i = 0; i < arr.Count; i++){
                t[i] = double.Parse(arr[i], CultureInfo.InvariantCulture);
            }
            listOfReadArrays.Add(t.ToList());
        }
        return listOfReadArrays;
    }


    /// <summary>
    /// Fills Matrices W1 ; B1 ; W2; B2.  Reads the file of min and max and loads it into memory.
    /// Uses the ReadArraysFromFormattedFile method to read from a file a list of arrays of type float.
    /// This returned list is logically formatted.
    /// </summary>
    public static bool Populate(){
        B1.Clear(); B2.Clear(); W1.Clear(); W2.Clear();
        
        List<List<double>> biasArrays = ReadArraysFromFormattedFile("bias_out.txt");
        if (biasArrays == null)
            return false;

        B1 = biasArrays.ElementAt(0);
        B2 = biasArrays.ElementAt(1);

        List<List<double>> weightsArrays = ReadArraysFromFormattedFile("weights_out.txt");
        if (weightsArrays == null)
            return false;

        // W1
        int j = 0; 
        while (Enumerable.SequenceEqual(weightsArrays.ElementAt(j), new List<double> { } ) == false)
        {
            j++;
        }

        for(int i = 0; i < j ; i++){
            W1.Add(weightsArrays.ElementAt(i));     
        }

        // W2
        int k = j+1;
        while (Enumerable.SequenceEqual(weightsArrays.ElementAt(k), new List<double> { } ) == false){
            k++;
        }

        for (int i = 0 ; i < k - (j + 1) ; i++){
            W2.Add(weightsArrays.ElementAt(j + 1 + i));
        }

        try{
            readText = File.ReadAllLines(FileUtils.GeneratePath("min&max_values_dataset_out.txt"));
            min = readText[0].Split(' ');
            max = readText[1].Split(' ');
        }
        catch(Exception){
            Debug.LogError("MinMax file not exist.");
        }
        return true;
    }

    /// <summary>
    /// Perform prediction.
    /// Use matrix W1, B1, W2 e B2 to perform operations.
    /// </summary>
    /// <param name="features">Features</param>
    /// <returns>Predicted note</returns>
    public static int ReteNeurale(float[] features){
      for(int i = 0; i < features.Length; i++){
            //Debug.Log("features " + i + "  =" + features[i]);
        }

        double[] MyFeatures = new double[36];

        MyFeatures = ScaleValues(features);

        // output_hidden1 has same number of elements of B1 BiasLAYER1
        var output_hidden1 = new double[B1.Count];
        // output_hidden2 has same number of elements of B2 BIAS LAYER2
        var output_hidden2 = new double[B2.Count];

        double x, w, r;

        for (int i = 0; i < output_hidden1.Length; i++){
            output_hidden1[i] += B1.ElementAt(i);

            for (int j = 0; j < features.Length; j++){
                x = MyFeatures[j];
                w = W1[j][i];
                r = x * w;

                output_hidden1[i] += r;
            }
            if (output_hidden1[i] <= 0)
                output_hidden1[i] = 0;
        }

        for (int i = 0; i < output_hidden2.Length; i++){
            output_hidden2[i] += B2.ElementAt(i);
            for (int j = 0; j < output_hidden1.Length; j++){
                x = output_hidden1[j];
                w = W2[j][i];
                r = x * w;

                output_hidden2[i] += r;
            }
        }

        double sum = 0;
        foreach (var item in output_hidden2){
            sum += Mathf.Exp((float)item);
        }

        var toRet = new double[output_hidden2.Length];
        
        for (int i = 0; i < output_hidden2.Length; i++){
            toRet[i] = Mathf.Exp((float)output_hidden2[i]) / sum;
        }

        var toRetMax = toRet.Max();  // Max probability neural network
        int index=toRet.ToList().IndexOf(toRet.Max());

        int toSend=0;
        if (counter == -1 && toRetMax > 0.9){
            counter = 0;
            oldIndexToSend = index;
            toSend= index;
            toCompare = index;
        }

        toSend = oldIndexToSend;
        if (index == toCompare && toRetMax > 0.9){
            counter++;
            if (counter > delaytime && toRetMax > 0.9){
                toSend = toCompare;
                oldIndexToSend=toCompare;
            }
        }else{
            counter = 0;
            toCompare = index;
        }
        return toSend;
    }


    /// <summary>
    /// MinMax scaling of the features between 0 and 1.
    /// </summary>
    /// <param name="unscaledFeatures">Features</param>
    /// <returns>Scaled features</returns>
    private static double[] ScaleValues(float[] unscaledFeatures){
        var scaledFeatures = new double[unscaledFeatures.Length];
        var minValues = new double[unscaledFeatures.Length];
        var maxValues = new double[unscaledFeatures.Length];
        
        for (int i = 0; i < unscaledFeatures.Length; i++){
            minValues[i] = double.Parse(min[i], CultureInfo.InvariantCulture);
            maxValues[i] = double.Parse(max[i], CultureInfo.InvariantCulture);
        }
        var work = new double[unscaledFeatures.Length];
        for (int i = 0; i < unscaledFeatures.Length; i++){
            scaledFeatures[i] = (unscaledFeatures[i] - minValues[i]) / (maxValues[i] - minValues[i]);
        }
        return scaledFeatures;
    }
}