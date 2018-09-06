using UnityEngine;
using UnityEngine.Video;

public class LaserPointerControllerInput : MonoBehaviour
{
    // Reference to the object being tracked. // Controller, in this case.
    private SteamVR_TrackedObject trackedObj;

    //-------------Laser-------------
    // Reference to Laser's prefab
    public GameObject laserPrefab;

    // Reference to instance of laser(Actual laser in the scene)
    private GameObject laser;

    // Transform component of laser(instance)
    private Transform laserTransform;

    // Position where laser hits
    private Vector3 hitPoint;

    //-------------Menu-------------
    public LayerMask ButtonMask;
    public Color HoverColor;
    public Color DefaultColor;
    public Color PressedColor;
    public VideoPlayer videoPlayer;
    private bool showLaser = false;
    private float resetColorTimer = 0.0f;
    private float resetColorTiming = 1.0f;
    public GameObject[] buttons;
    public GameObject PauseButton;

    [Header("Menu")]
    public GameObject Briefing;
    public GameObject Instruction;
    public GameObject MainMenu;
    public GameObject PauseMenu;

    // Device property to provide easy access to controller.
    // Uses the tracked object's index to return the controller's input
    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    // Use this for initialization
    void Start()
    {
        // Spawns a new laser and save reference to laser
        laser = Instantiate(laserPrefab);

        // Store laser's transform component
        laserTransform = laser.transform;
    }

    private void ShowLaser(RaycastHit hit)
    {
        // Shows laser
        laser.SetActive(true);

        // Position laser between controller and the point where raycast hits.
        // Using Lerp because it allows 2 positions and the % it should travel.
        // If pass in 0.5f(50%), it returns the precise middle point of the 2 positions
        laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, 0.5f);

        // Point laser at the position where raycast hit
        laserTransform.LookAt(hitPoint);

        // Scale the laser so it fits perfectly between the 2 positions
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y, hit.distance);
    }

    // Interact with specific button according to name
    private void Interact(string buttonName)
    {
        // Proceed to different actions depending on the buttonName
        switch (buttonName)
        {
            case "Play Button Cube":
                videoPlayer.Play();
                MainMenu.SetActive(false);
                break;
            case "Resume Button Cube":
                videoPlayer.Play();
                PauseMenu.SetActive(false);
                break;
            case "MainMenu Button Cube":
                videoPlayer.Stop();
                MainMenu.SetActive(true);
                PauseMenu.SetActive(false);
                laser.SetActive(false);
                break;
            default:
                break;
        }
    }

    private void ResetButtonColor()
    {
        // Ensure there's button/s
        if (buttons.Length == 0)
        {
            Debug.LogError("buttons cannot be found.");
            return;
        }

        // Reset each buttons' color to default color
        foreach (GameObject button in buttons)
        {
            button.GetComponent<Renderer>().material.color = DefaultColor;
        }
    }

    // Update is called once per frame
    void Update ()
    {
        // Only show laser when Menu is active
        if (MainMenu.activeSelf || PauseMenu.activeSelf)
        {
            if (!showLaser)
            {
                // When press Hair Trigger
                if (Controller.GetHairTriggerDown())
                {
                    showLaser = true;
                }
            }
            else
            {
                RaycastHit hit;

                // Shoot a ray from controller.
                // When it hits something(Only GameObjects player can interact with to)
                if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100, ButtonMask))
                {
                    // Store the point of contact
                    hitPoint = hit.point;
                    // Show the laser
                    ShowLaser(hit);
                    
                    // Resets buttons to its default color
                    resetColorTimer += 1.0f * Time.deltaTime;
                    if (resetColorTimer >= resetColorTiming)
                    {
                        ResetButtonColor();
                        resetColorTimer = 0.0f;
                    }

                    // When aimed at button,change it to hover color
                    GameObject buttonHit = hit.collider.gameObject;
                    buttonHit.GetComponent<Renderer>().material.color = HoverColor;

                    // When press Hair Trigger
                    if (Controller.GetHairTriggerDown())
                    {
                        // Change to Pressed color
                        buttonHit.GetComponent<Renderer>().material.color = PressedColor;

                        // Proceed to Interact with the button
                        Interact(buttonHit.name);
                        
                        // Reset variables
                        showLaser = false;
                        resetColorTimer = 0.0f;
                    }
                }
            }
        }
        else
        {
            // Hides laser
            laser.SetActive(false);

            if (Briefing.activeSelf)
            {
                // When Trigger(Skip) is press
                if (Controller.GetHairTriggerDown())
                {
                    Briefing.SetActive(false);
                    Instruction.SetActive(true);
                }
            }
            else if (Instruction.activeSelf)
            {
                // When Trigger(Skip) is press
                if (Controller.GetHairTriggerDown())
                {
                    Instruction.SetActive(false);
                    MainMenu.SetActive(true);
                }
            }

            // When Video is playing
            if(videoPlayer.isPlaying)
            {
                // Show Pause Button
                if(!PauseButton.activeSelf)
                {
                    PauseButton.SetActive(true);
                }

                // When press on Application Menu Button(Small Button above touchpad)
                if(Controller.GetPress(SteamVR_Controller.ButtonMask.ApplicationMenu))
                {
                    videoPlayer.Pause();
                    PauseMenu.SetActive(true);
                    PauseButton.SetActive(false);
                }
            }
        }
    }
}
