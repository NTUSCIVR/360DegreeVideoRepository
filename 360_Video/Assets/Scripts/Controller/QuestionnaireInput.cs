using System.IO;
using UnityEngine;
using UnityEngine.Video;

public class QuestionnaireInput : MonoBehaviour
{
    // Reference to the object being tracked. // Controller, in this case.
    private SteamVR_TrackedObject trackedObj;
    
    [Header("In Script GameObject")]
    public Video videoScript;

    [Header("Under [CameraRig] -> Camera(head) -> Camera(eye) -> Canvas")]
    public GameObject Ending;

    // Questionnaire objects
    public GameObject QuestionnaireObject;
    [Tooltip("Circle Image")]
    public GameObject Choice;
    [Tooltip("Under Ring Images GameObject")]
    public GameObject[] Targets;

    private VideoPlayer videoPlayer;
    private DataCollector dataCollector;
    private int ChoiceIndex = 0;

    // Device property to provide easy access to controller.
    // Uses the tracked object's index to return the controller's input
    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }
    
    private void Start()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        if (DataCollector.Instance != null)
        {
            dataCollector = DataCollector.Instance;
        }
        videoPlayer = videoScript.videoPlayer;
    }

    private void RecordData()
    {
        StreamWriter sw = File.AppendText(dataCollector.GetCSVPath(DataCollector.DataToRecord.RatingVideo, dataCollector.currentFolderPath));
        string url = videoPlayer.url;
        string rating = (ChoiceIndex - 2).ToString();
        sw.WriteLine(url + "," + rating);
        sw.Close();
    }
    
    private void Update()
    {
        if (QuestionnaireObject.activeSelf)
        {
            // Moving the Circle(To allow rating selection)
            if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
            {
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
                Choice.transform.SetPositionAndRotation(Targets[ChoiceIndex].transform.position, Choice.transform.rotation);
            }

            // Confirm the selection
            if(Controller.GetHairTriggerDown())
            {
                RecordData();

                // Enable this section of commented codes when need to play multiple videos at one go
                // Disable this section of commented codes when only need to play one video
                //videoScript.ChangeToNextVideo();

                // if still have video to play
                //if (videoPlayer.enabled)
                //{
                //    CountDown.gameObject.SetActive(true);
                //}
                //else
                //{
                //Ending.SetActive(true);
                //}

                // Enable this line of code when only need to play one video
                // Disable this line of code when need to play multiple videos at one go
                Ending.SetActive(true);

                QuestionnaireObject.SetActive(false);
            }
        }
    }
}
