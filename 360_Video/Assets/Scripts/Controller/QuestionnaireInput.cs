//--------------------------------------------------------------------------------
/*
 * This Script is used for recording User Input(Rating).
 * 
 * Used in Main Scene, attached to Controller GameObjects "Controller(left/right)" under [CameraRig] in Hierarchy.
 * Require 1 Video Script variable and 4 GameObject variables : Ending, Questionnaire, Circle Image, Targets(5 Ring images)
 * The 4 GameObject variables can be found under Canvas GameObject, which is under Camera(eye) of [CameraRig] in Hierarchy.
 * Video Script variable can be found in "Script GameObject" in Hierarchy.
 * Questionnaire's GameObject do not need to be active.
 */
//--------------------------------------------------------------------------------

using System.IO; // For StreamWrite, File
using UnityEngine;  // Default Unity Script (MonoBehaviour, GameObject, Tooltip, GetComponent(), Vector2)
using UnityEngine.Video; // Fore VideoPlayer

public class QuestionnaireInput : MonoBehaviour
{
    // Reference to the object being tracked. // Controller, in this case.
    private SteamVR_TrackedObject trackedObj;
    
    [Tooltip("In Script GameObject")]
    // Reference to Video Script
    public Video videoScript;

    [Tooltip("Under [CameraRig] -> Camera(head) -> Camera(eye) -> Canvas")]
    public GameObject Ending;

    // Questionnaire objects
    public GameObject QuestionnaireObject;
    [Tooltip("Circle Image")]
    public GameObject Choice;
    [Tooltip("Under Ring Images GameObject")]
    public GameObject[] Targets;

    // Reference to video player
    private VideoPlayer videoPlayer;
    
    // Reference to DataCollector
    private DataCollector dataCollector;

    // For moving visual feedback of choice and recording of Rating
    private int ChoiceIndex = 0;

    // Device property to provide easy access to controller.
    // Uses the tracked object's index to return the controller's input
    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    // Runs at the start of first frame
    private void Start()
    {
        // Get Reference to Controllers
        trackedObj = GetComponent<SteamVR_TrackedObject>();

        // Set Reference to DataCollector if it's alive
        if (DataCollector.Instance != null)
        {
            dataCollector = DataCollector.Instance;
        }

        // Get Reference to VideoPlayer in Video Script
        videoPlayer = videoScript.videoPlayer;
    }

    // Records Rating Data into csv file
    private void RecordData()
    {
        // Open csv file at the path return from GetCSVPath()
        StreamWriter sw = File.AppendText(dataCollector.GetCSVPath(DataCollector.DataToRecord.RatingVideo, dataCollector.currentFolderPath));
        string url = videoPlayer.url;
        string rating = (ChoiceIndex - 2).ToString();

        // Write onto the file
        sw.WriteLine(url + "," + rating);

        // Close the file
        sw.Close();
    }

    // Runs at every frame
    private void Update()
    {
        // When Questionnaire is active
        if (QuestionnaireObject.activeSelf)
        {
            // Moving the Circle(To allow rating selection) if Touchpad is pressed
            if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
            {
                // Track Finger's Position on Touchpad
                Vector2 touchPad = Controller.GetAxis();

                // Right side
                if (touchPad.x > 0.75f)
                {
                    if(ChoiceIndex + 1 <= 4)
                    {
                        ChoiceIndex += 1;
                    }
                }
                // Left side
                else if (touchPad.x < -0.75f)
                {
                    if (ChoiceIndex - 1 >= 0)
                    {
                        ChoiceIndex -= 1;
                    }
                }

                // Adjust Position and Rotation of Circle image for visual feedback
                Choice.transform.SetPositionAndRotation(Targets[ChoiceIndex].transform.position, Choice.transform.rotation);
            }

            // Confirm the selection if Hair Trigger is pressed
            if(Controller.GetHairTriggerDown())
            {
                // Proceed to Record Rating Data
                RecordData();

                // Enable this section of commented codes when need to play multiple videos at one go
                // Disable this section of commented codes when only need to play one video
                // Change to Next Video waiting to be played
                //videoScript.ChangeToNextVideo();

                //// If still have video to play
                //if (videoPlayer.enabled)
                //{
                //    // Show CountDown Text
                //    CountDown.gameObject.SetActive(true);
                //}
                //else
                //{
                //    // Show Ending
                //    Ending.SetActive(true);
                //}

                // Enable this line of code when only need to play one video
                // Disable this line of code when need to play multiple videos at one go
                // Show Ending
                Ending.SetActive(true);

                // Hide Questionnaire
                QuestionnaireObject.SetActive(false);
            }
        }
    }
}
