//--------------------------------------------------------------------------------
/*
 * This Script is used for collecing Data generated/User Input
 * 
 * Used in Start Scene, attached to Empty GameObject "DataCollector"
 * Require 2 InputField variables : User ID, Video ID(1 / 2 / 3 / 4 ...depends on how many videos are there)
 * which can be found under Canvas GameObject in Hierarchy.
 * Among the 2 InputFields, only User ID's GameObject need to be active.
 */
//--------------------------------------------------------------------------------

using System.IO;    // For Directory, File, StreamWriter
using UnityEngine;  // Default Unity Script (MonoBehaviour, Application, GameObject, Header, Tooltip, HideInInspector, DontDestroyOnLoad, Debug, Time)
using UnityEngine.UI; // For InputField
using UnityEngine.SceneManagement; // For SceneManager

public class DataCollector : MonoBehaviour
{
    // For other scripts to access DataCollector
    public static DataCollector Instance;
    
    [Tooltip("Time interval to collect Headset Position & Rotation. Default: 1.0f")]
    public float dataRecordInterval = 1f;
    
    [Header("Under Canvas")]

    // User ID InputField
    public InputField inputField;
    
    // Video ID InputField
    public InputField VideoIdInputField;
    
    // For holding SteamVR Camera Component
    [HideInInspector]
    public GameObject user;

    // For converting User Input into respective data
    private string dataID;
    [HideInInspector]
    public int VideoID;
    [HideInInspector]
    public string currentFolderPath;
    
    // For Recording Head Movement Data
    [HideInInspector]
    public bool startRecording = false;
    private float time = 0f;

    // Enum for different Data type to record
    public enum DataToRecord
    {
        HeadMovement,
        RatingVideo
    }

    // Runs before Start()
    private void Awake()
    {
        // Allow DataCollector Instance to be alive after change scene
        DontDestroyOnLoad(this);
    }

    // Runs at the start of first frame
    private void Start()
    {
        // Set this instance of DataCollector to allow other scripts to access its variables and data
        Instance = this;

        // Allow input of User ID
        inputField.Select();
    }

    // Runs at every frame
    private void Update ()
    {
        // Start Recording when bool is set to true
        if (startRecording)
        {
            time += Time.deltaTime;

            // Record Head Movement Data every data Record Interval(Default: 1.0f)
            if (time > dataRecordInterval)
            {
                // Reset timer
                time = 0;

                // Write generated data into csv file
                PushData(GenerateData());
            }
        }
	}

    // Returns generated Head Movement Data string
    private string GenerateData()
    {
        string data = "";

        // Get Time Information into data string
        data += System.DateTime.Now.ToString("HH");
        data += ":";
        data += System.DateTime.Now.ToString("mm");
        data += ":";
        data += System.DateTime.Now.ToString("ss");
        data += ":";
        data += System.DateTime.Now.ToString("FFF");

        // Seperator
        data += ",";

        // Get Headset Position in vector 3 format
        string posstr = user.GetComponent<SteamVR_Camera>().head.transform.position.ToString("F3");

        // Change , to . to prevent Position data to be seperated
        data += ChangeLetters(posstr, ',', '.');

        // Seperator
        data += ",";

        // Get Headset Rotation in vector 3 format
        string rotstr = user.GetComponent<SteamVR_Camera>().head.transform.rotation.ToString("F3");

        // Change , to . to prevent Position data to be seperated
        data += ChangeLetters(rotstr, ',', '.');

        return data;
    }

    // Edit the current file by adding the new text
    private void PushData(string text)
    {
        // Open csv file at the path return from GetPath()
        StreamWriter sw = File.AppendText(GetCSVPath(DataToRecord.HeadMovement, currentFolderPath));

        // Write onto the file
        sw.WriteLine(text);

        // Close the file
        sw.Close();
    }

    // Returns the fodler path being used to store the csv files
    private string GetFolderPath()
    {
        string Folder = "/Data/";

        // If the folder path already exists, create a new folder with a duplicate number
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
    }

    // Duplicate or Create new folder
    private void CreateFolder()
    {
        currentFolderPath = GetFolderPath();
        // Create new folder and csv files
        Directory.CreateDirectory(currentFolderPath);
        CreateCSV(DataToRecord.HeadMovement, currentFolderPath);
        CreateCSV(DataToRecord.RatingVideo, currentFolderPath);
    }

    // Returns the file path being used to store the data
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

        return folderPath + File + ".csv";
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

    //--------------------------------------------------------------------------------
    //                                  PUBLIC FUNCTIONS
    //--------------------------------------------------------------------------------

    // Link to User ID InputField OnEndEdit()
    // This Registers User ID and use it to create csv file.
    // Then Set Video ID InputField to active and allow its input.
    public void ProceedToSelectVideo()
    {
        // If no text, dont let them proceed
        if (inputField.text == null)
            return;

        // Register User ID
        dataID = inputField.text;

        // Create Folder and CSV for user based on input
        CreateFolder();

        // Allow input of Video ID
        VideoIdInputField.gameObject.SetActive(true);
        VideoIdInputField.Select();
    }

    // Link to Video ID InputField OnEndEdit()
    // This Registers Video ID, which is used to determine the video file to play.
    // Then Set bool to true so can start Recording Head Movement Data.
    // And Change Scene to Main Scene (Watch video)
    public void Start360MovieTheatre()
    {
        // Try Parse VideoID Input into int and put in VideoID
        if (VideoIdInputField.text == null || !int.TryParse(VideoIdInputField.text, out VideoID))
        {
            // If no text, dont let them proceed
            Debug.LogWarning("VideoID Input cannot be parse into int.");
            return;
        }

        // Start recording Head Movement
        startRecording = true;

        // Change to MainScene
        SceneManager.LoadScene("MainScene");
    }
}