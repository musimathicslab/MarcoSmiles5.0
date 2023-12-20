using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CardManager : MonoBehaviour
{
    public List<CardDataset> cardDatasets = new List<CardDataset>();

    public Transform container;

    public GameObject cardPrefab;

    public GameObject notePrefab;

    public GameObject addPanel;

    public GameObject delatePanel;

    public GameObject modifyPanel;


    private void Start()
    {
        collectData();
        instantiateObject();
    }
    // raccoglie i dati necessari per la creazione delle cards all'interno della cartella dei datasets
    public void collectData()
    {
        CardDataset card;
        NoteImage note;

        DirectoryInfo dir = new DirectoryInfo(FileUtils.GeneratePath());
        dir = dir.Parent;
        foreach (DirectoryInfo diri in dir.GetDirectories())
        {
            card = new CardDataset();
            card.datasetName = diri.Name;
            if(String.Compare(FileUtils.selectedDataset, diri.Name) == 0)
            {
                card.isSelected = true;
            }
            if (FileUtils.CheckForDefaultFiles(diri.Name))
            {
                card.isTrained = true;
            }
            foreach(DirectoryInfo dirj in diri.GetDirectories())
            {
                if(String.Compare(dirj.Name, "CapturedPhoto") == 0)
                {
                    foreach(var fili in dirj.GetFiles())
                    {
                        note = new NoteImage();
                        var tmp = fili.Name.Split(".").ToList();
                        tmp.Remove(tmp.Last());

                        note.noteName = tmp.Last();
                        Texture2D tex = new Texture2D(2, 2);
                        tex.LoadImage(File.ReadAllBytes(fili.FullName));
                        note.noteImage= Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                        card.noteImages.Add(note);
                    }
                }
            }
            cardDatasets.Add(card);
        }
    }
    //si occupa di istanziare le cards nella scena utilizando i dati raccolti 
    public void instantiateObject()
    {
        foreach (CardDataset cardi in cardDatasets)
        {
            if (cardi.isSelected == true)
            {
                GameObject card = Instantiate(cardPrefab, container);
                card.SetActive(true);
                card.transform.Find("TextContent").Find("NameDataset").gameObject.GetComponent<TMP_Text>().text = cardi.datasetName;
                card.transform.Find("TextContent").Find("Check").gameObject.SetActive(cardi.isSelected);
                card.transform.Find("TrainedText").gameObject.SetActive(cardi.isTrained);
                card.transform.Find("RegisteredText").gameObject.SetActive(!cardi.noteImages.Any());
                foreach (NoteImage notei in cardi.noteImages)
                {
                    GameObject imnote = Instantiate(notePrefab, card.transform.Find("ScrolNotes").Find("NoteContent"));
                    imnote.SetActive(true);
                    imnote.transform.Find("ImageHeand").gameObject.GetComponent<Image>().sprite = notei.noteImage;
                    imnote.transform.Find("ImageHeand").Find("TextNote").gameObject.GetComponent<TMP_Text>().text = notei.noteName;
                }
            }
        }
        foreach (CardDataset cardi in cardDatasets)
        {
            if (cardi.isSelected == false)
            {
                GameObject card = Instantiate(cardPrefab, container);
                card.SetActive(true);
                card.transform.Find("TextContent").Find("NameDataset").gameObject.GetComponent<TMP_Text>().text = cardi.datasetName;
                card.transform.Find("TextContent").Find("Check").gameObject.SetActive(cardi.isSelected);
                card.transform.Find("TrainedText").gameObject.SetActive(cardi.isTrained);
                card.transform.Find("RegisteredText").gameObject.SetActive(!cardi.noteImages.Any());
                foreach (NoteImage notei in cardi.noteImages)
                {
                    GameObject imnote = Instantiate(notePrefab, card.transform.Find("ScrolNotes").Find("NoteContent"));
                    imnote.SetActive(true);
                    imnote.transform.Find("ImageHeand").gameObject.GetComponent<Image>().sprite = notei.noteImage;
                    imnote.transform.Find("ImageHeand").Find("TextNote").gameObject.GetComponent<TMP_Text>().text = notei.noteName;
                }
            }
        }
    }

    // cancella tutte le cards nella scena 
    private void delateAllCard()
    {
        foreach(Transform child in container)
        {
            Destroy(child.gameObject);
        }
    }

    //quando una cards viene cliccata seleziona il dataset a cui fa riferimento e ricarica la pagina 
    public void selectDataset(GameObject name)
    {
        FileUtils.selectedDataset = name.GetComponent<TMP_Text>().text;
        TestML.Populate();
        FileUtils.CheckForDefaultFiles();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

   
    
    public void openAddPanel()
    {
        addPanel.SetActive(true);

    }

    public void closeAddPanel()
    {
        addPanel.SetActive(false);

    }

    public void addDataset(GameObject text)
    {
        DirectoryInfo dir = new DirectoryInfo(FileUtils.GeneratePath());
        dir = dir.Parent;
        if (text.GetComponent<TMP_Text>().text.Length>1)
        {
            dir.CreateSubdirectory(text.GetComponent<TMP_Text>().text);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

    }


    public void openModifyPanel(GameObject datasetName)
    {
        modifyPanel.SetActive(true);
        modifyPanel.transform.Find("NameDataset").gameObject.GetComponent<TMP_Text>().text = datasetName.GetComponent<TMP_Text>().text;
        modifyPanel.transform.Find("card").Find("ModifyText").gameObject.GetComponent<TMP_InputField>().text = datasetName.GetComponent<TMP_Text>().text;
    }

    public void closeModifyPanel()
    {
        modifyPanel.SetActive(false);
    }

    public void modifyDataset()
    {
        GameObject datasetName = modifyPanel.transform.Find("NameDataset").gameObject;
        GameObject newDatasetName = modifyPanel.transform.Find("card").Find("ModifyText").Find("Text Area").Find("Text").gameObject;

        DirectoryInfo dir = new DirectoryInfo(FileUtils.GeneratePath());
        dir = dir.Parent;
        if(String.Compare(dir.GetDirectories(datasetName.GetComponent<TMP_Text>().text)[0].Name, FileUtils.selectedDataset) == 0)
        {
            Directory.Move(dir.GetDirectories(datasetName.GetComponent<TMP_Text>().text)[0].FullName, dir.FullName + "/" + newDatasetName.GetComponent<TMP_Text>().text);
            selectDataset(newDatasetName);
        }
        Directory.Move(dir.GetDirectories(datasetName.GetComponent<TMP_Text>().text)[0].FullName, dir.FullName+"/"+ newDatasetName.GetComponent<TMP_Text>().text);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }


    public void openDelatePanel(GameObject datasetName)
    {
        delatePanel.SetActive(true);
        delatePanel.transform.Find("NameDataset").gameObject.GetComponent<TMP_Text>().text = datasetName.GetComponent<TMP_Text>().text;
    }

    public void closeDelatePanel()
    {
        delatePanel.SetActive(false);
    }

    public void delateDataset()
    {
        GameObject datasetName = delatePanel.transform.Find("NameDataset").gameObject;

        DirectoryInfo dir = new DirectoryInfo(FileUtils.GeneratePath());
        dir = dir.Parent;
        Directory.Delete(dir.GetDirectories(datasetName.GetComponent<TMP_Text>().text)[0].FullName, true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }
}
