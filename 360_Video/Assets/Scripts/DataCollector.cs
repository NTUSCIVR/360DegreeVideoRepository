using System.IO;
using UnityEngine;
using TMPro;

public class DataCollector : MonoBehaviour
{
    private string dataID = "";

    [HideInInspector]
    public bool startRecording = false;

    [Tooltip("Default: 1.0f")]
    public float dataRecordInterval = 1f;
    private float time = 0f;

    [Tooltip("Camera(eye) under [CameraRig] -> Camera(head)")]
    public GameObject user;

    [Header("Under [CameraRig] -> Camera(head) -> Camera(eye) -> Canvas")]
    public TMP_InputField inputField;
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
        // Create Folder and CSV for user based on input
        dataID = inputField.text;
        CreateFolder();

        // Start recording Head Movement
        startRecording = true;

        // Show Briefing and Hide InputField
        Briefing.SetActive(true);
        inputField.gameObject.SetActive(false);
    }

    // Generate Head Movement Data
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

    // Replace existing CSV or Create new CSV
    private void CreateCSV(DataToRecord dataToRecord)
    {
        // Found same CSV
        if (File.Exists(GetCSVPath(dataToRecord)))
        {
            // Delete the CSV
            File.Delete(GetCSVPath(dataToRecord));
        }

        // Create new CSV and Write in Data Headers on first line
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

    // Change "letter" in "str" to "toBeLetter"
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
