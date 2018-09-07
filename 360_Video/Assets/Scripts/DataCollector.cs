using System.IO;
using UnityEngine;
using TMPro;

public class DataCollector : MonoBehaviour
{
    public TMP_InputField inputField;
    
    private string dataID = "";

    public bool startRecording = false;

    public float dataRecordInterval = 1f;
    private float time = 0f;
    
    public GameObject user;
    public GameObject Briefing;

    public enum DataToRecord
    {
        HeadMovement,
        RatingVideo
    }

    private void Start()
    {
        // Allow input
        inputField.Select();
    }

    // Update is called once per frame
    void Update ()
    {
        if(startRecording)
        {
            time += Time.deltaTime;
            if (time > dataRecordInterval)
            {
                time = 0;
                StreamWriter sw = File.AppendText(GetCSVPath(DataToRecord.HeadMovement));
                sw.WriteLine(GenerateData());
                sw.Close();
            }
        }
	}

    public void Start360MovieTheatre()
    {
        dataID = inputField.text;
        CreateFolder();
        startRecording = true;
        Briefing.SetActive(true);
        inputField.gameObject.SetActive(false);
    }

    private string GenerateData()
    {
        string data = "";
        data += System.DateTime.Now.ToString("HH");
        data += ":";
        data += System.DateTime.Now.ToString("mm");
        data += ":";
        data += System.DateTime.Now.ToString("ss");
        data += ":";
        data += System.DateTime.Now.ToString("FFF");
        data += ",";
        string posstr = user.GetComponent<SteamVR_Camera>().head.transform.position.ToString("F3");
        data += ChangeLetters(posstr, ',', '.');
        data += ",";
        string rotstr = user.GetComponent<SteamVR_Camera>().head.transform.rotation.ToString("F3");
        data += ChangeLetters(rotstr, ',', '.');
        return data;
    }

    private string GetFolderPath()
    {
        string Folder = "/Data/";
        
#if UNITY_EDITOR
        return Application.dataPath + Folder + dataID + "/";
#elif UNITY_STANDALONE_WIN
        return Application.dataPath + Folder + dataID + "/";
#endif
    }

    // Replace existing folder or Create new folder
    private void CreateFolder()
    {
        // Found same folder
        if (Directory.Exists(GetFolderPath()))
        {
            // Delete files inside the folder first
            foreach(FileInfo file in new DirectoryInfo(GetFolderPath()).GetFiles())
            {
                file.Delete();
            }
            // Delete the folder
            Directory.Delete(GetFolderPath());
        }

        // Create new folder
        Directory.CreateDirectory(GetFolderPath());
        CreateCSV(DataToRecord.HeadMovement);
        CreateCSV(DataToRecord.RatingVideo);
    }

    public string GetCSVPath(DataToRecord dataToRecord)
    {
        string Folder = "/Data/";
        string File = "";

        switch(dataToRecord)
        {
            case DataToRecord.HeadMovement:
                File += "/HeadMovement";
                break;
            case DataToRecord.RatingVideo:
                File += "/RatingVideo";
                break;
        }
#if UNITY_EDITOR
        return Application.dataPath + Folder + dataID + File + ".csv";
#elif UNITY_STANDALONE_WIN
        return Application.dataPath + Folder + dataID + File + ".csv";
#endif
    }

    private void CreateCSV(DataToRecord dataToRecord)
    {
        if (File.Exists(GetCSVPath(dataToRecord)))
        {
            File.Delete(GetCSVPath(dataToRecord));
        }
        StreamWriter output = File.CreateText(GetCSVPath(dataToRecord));
        switch (dataToRecord)
        {
            case DataToRecord.HeadMovement:
                output.WriteLine("Time, Position, Rotation");
                break;
            case DataToRecord.RatingVideo:
                output.WriteLine("Video Url, Rating(-2 to 2)");
                break;
        }
        output.Close();
    }

    private string ChangeLetters(string str, char letter, char toBeLetter)
    {
        char[] ret = str.ToCharArray();
        for(int i = 0; i < ret.Length; ++i)
        {
            if(ret[i] == letter)
            {
                ret[i] = toBeLetter;
            }
        }
        return new string(ret);
    }
}
