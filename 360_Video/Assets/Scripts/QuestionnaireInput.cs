using UnityEngine;
using UnityEngine.Video;
using TMPro;
using System.IO;

public class QuestionnaireInput : MonoBehaviour
{
    // Reference to the object being tracked. // Controller, in this case.
    private SteamVR_TrackedObject trackedObj;

    [Tooltip("Sphere that envelops [CameraRig]")]
    public VideoPlayer videoPlayer;

    [Header("Script GameObject")]
    public DataCollector dataCollector;
    public Video videoScript;

    [Header("Under [CameraRig] -> Camera(head) -> Camera(eye) -> Canvas")]
    public GameObject Ending;
    public TextMeshProUGUI CountDown;
    private float CountDownTimer = 5.0f;
    private float CountDownScale = 1.0f;

    // Questionnaire objects
    public GameObject QuestionnaireObject;
    [Tooltip("Circle Image")]
    public GameObject Choice;
    [Tooltip("Under Ring Images GameObject")]
    public GameObject[] Targets;
    private int ChoiceIndex = 0;


    // Device property to provide easy access to controller.
    // Uses the tracked object's index to return the controller's input
    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    // Use this for initialization
    void Start()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    private void RecordData()
    {
        StreamWriter sw = File.AppendText(dataCollector.GetCSVPath(DataCollector.DataToRecord.RatingVideo));
        string url = videoPlayer.url;
        string rating = (ChoiceIndex - 2).ToString();
        sw.WriteLine(url + "," + rating);
        sw.Close();
    }

    // Update is called once per frame
    void Update()
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
                videoScript.ChangeToNextVideo();

                // if still have video to play
                if (videoPlayer.enabled)
                {
                    CountDown.gameObject.SetActive(true);
                }
                else
                {
                    Ending.SetActive(true);
                }
                QuestionnaireObject.SetActive(false);
            }
        }

        if(CountDown.gameObject.activeSelf)
        {
            // Start Counting down
            CountDownTimer -= Time.deltaTime;
            CountDown.text = Mathf.CeilToInt(CountDownTimer).ToString();

            // Scale down every second
            CountDownScale -= Time.deltaTime;
            CountDown.transform.localScale = new Vector3(CountDownScale, CountDownScale, CountDownScale);

            // Pop back to full size every next second
            if(CountDownScale <= 0.0f)
            {
                CountDownScale = 1.0f;
            }
            
            if(CountDownTimer <= 0)
            {
                // Reset Countdown then play next video
                CountDown.gameObject.SetActive(false);
                CountDownTimer = 5.0f;
                CountDownScale = 1.0f;
                videoPlayer.Play();
            }
        }
    }
}
