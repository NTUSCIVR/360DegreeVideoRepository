using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DataCollector : MonoBehaviour
{
    public static DataCollector Instance;
    
    [Tooltip("Time interval to collect Headset Position & Rotation. Default: 1.0f")]
    public float dataRecordInterval = 1f;
    
    [Header("Under [CameraRig] -> Camera(head) -> Camera(eye) -> Canvas")]
    [Tooltip("ID Input Field")]
    public InputField inputField;
    public InputField VideoIdInputField;

    [HideInInspector]
    public bool startRecording = false;
    [HideInInspector]
    public GameObject user;
    [HideInInspector]
    public int VideoID;
    [HideInInspector]
    public string currentFolderPath;

    private string dataID = "";
    private float time = 0f;

    public enum DataToRecord
    {
        HeadMovement,
        RatingVideo
    }

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        Instance = this;
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
                StreamWriter sw = File.AppendText(GetCSVPath(DataToRecord.HeadMovement, currentFolderPath));
                sw.WriteLine(GenerateData());
                sw.Close();
            }
        }
	}

    public void ProceedToSelectVideo()
    {
        // Create Folder and CSV for user based on input
        dataID = inputField.text;
        CreateFolder();

        // Allow input
        VideoIdInputField.gameObject.SetActive(true);
        VideoIdInputField.Select();
    }

    public void Start360MovieTheatre()
    {
        // Try Parse VideoID Input into int and put in VideoID
        if(!int.TryParse(VideoIdInputField.text,out VideoID))
        {
            Debug.LogWarning("VideoID Input cannot be parse into int.");
        }

        // Start recording Head Movement
        startRecording = true;

        // Change to MainScene
        SceneManager.LoadScene("MainScene");
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

    // Duplicates Folder with a (Duplicate Count) at the back of ID
    private string GetFolderPath()
    {
        string Folder = "/Data/";

#if UNITY_EDITOR
        string filePath = Application.dataPath + Folder + dataID;
        int duplicateCounts = 0;
        while (true)
        {
            if (Directory.Exists(filePath))
            {
                ++duplicateCounts;
                filePath = Application.dataPath + Folder + dataID + "(" + duplicateCounts.ToString() + ")";
            }
            else
                break;
        }
        return filePath;
#elif UNITY_STANDALONE_WIN
        string filePath = Application.dataPath + Folder + dataID;
        int duplicateCounts = 0;
        while (true)
        {
            if (Directory.Exists(filePath))
            {
                ++duplicateCounts;
                filePath = Application.dataPath + Folder + dataID + "(" + duplicateCounts.ToString() + ")";
            }
            else
                break;
        }
        return filePath;
#endif
    }

    // Duplicate or Create new folder
    private void CreateFolder()
    {
        currentFolderPath = GetFolderPath();
        // Create new folder
        Directory.CreateDirectory(currentFolderPath);
        CreateCSV(DataToRecord.HeadMovement, currentFolderPath);
        CreateCSV(DataToRecord.RatingVideo, currentFolderPath);
    }

    public string GetCSVPath(DataToRecord dataToRecord, string folderPath)
    {
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
        return folderPath + File + ".csv";
#elif UNITY_STANDALONE_WIN
        return folderPath + File + ".csv";
#endif
    }

    // Create new CSV
    private void CreateCSV(DataToRecord dataToRecord, string folderPath)
    {
        // Create new CSV and Write in Data Headers on first line
        StreamWriter output = File.CreateText(GetCSVPath(dataToRecord, folderPath));
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
