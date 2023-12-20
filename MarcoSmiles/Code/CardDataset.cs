using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]public class CardDataset
{
    public string datasetName;
    public bool isSelected;
    public bool isTrained;
    public List<NoteImage> noteImages;

    public CardDataset()
    {
        datasetName = "";
        isSelected = false;
        isTrained = false;
        noteImages = new List<NoteImage>();
    }
}
