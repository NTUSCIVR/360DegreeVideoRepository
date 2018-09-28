using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IntroFadeOut : MonoBehaviour
{
    // Rate of Fading out
    [Tooltip("Rate of Fading. Higher means fade out faster;Lower means fade out slower. Default: 0.15f")]
    public float FadingRate = 0.15f;

    [Header("UI stuffs. Under [CameraRig] -> Camera(head) -> Camera(eye) -> Canvas")]
    // Reference to Briefing and its child/components
    public GameObject Briefing;
    private Image Brief_Panel;
    private TextMeshProUGUI Brief_Text;

    // Reference to Instruction and its child/components
    public GameObject Instruction;
    private Image Instruc_Panel;
    private TextMeshProUGUI Instruc_Title;
    private Image Instruc_Image;
    private TextMeshProUGUI Instruc_Text;

    // Reference to Ending and its child/components
    public GameObject Ending;
    private Image Ending_Panel;
    private TextMeshProUGUI Ending_Text;

    private Video videoScript;

    // Use this for initialization
    void Start ()
    {
        // Ensure there's Brefing to set
		if(!Briefing)
        {
            Debug.LogError("Briefing GameObject not found.");
            return;
        }
        else
        {
            // Get the Components
            Brief_Panel = Briefing.transform.Find("Briefing Panel").GetComponent<Image>();
            Brief_Text = Briefing.transform.Find("Briefing Panel").Find("TextMeshPro Text").GetComponent<TextMeshProUGUI>();
        }

        // Ensure there's Instruction to set
        if (!Instruction)
        {
            Debug.LogError("Instructions GameObject not found.");
            return;
        }
        else
        {
            // Get the Components
            Instruc_Panel = Instruction.transform.Find("Instructions Panel").GetComponent<Image>();
            Instruc_Title = Instruction.transform.Find("Instructions Panel").Find("Title TextMeshPro Text").GetComponent<TextMeshProUGUI>();
            Instruc_Image = Instruction.transform.Find("Instructions Panel").Find("Controls Image").GetComponent<Image>();
            Instruc_Text = Instruction.transform.Find("Instructions Panel").Find("Content TextMeshPro Text").GetComponent<TextMeshProUGUI>();
        }

        // Ensure there's Ending to set
        if (!Ending)
        {
            Debug.LogError("Ending GameObject not found.");
            return;
        }
        else
        {
            // Get the Components
            Ending_Panel = Ending.transform.Find("Ending Panel").GetComponent<Image>();
            Ending_Text = Ending.transform.Find("Ending Panel").Find("TextMeshPro Text").GetComponent<TextMeshProUGUI>();
        }

        // Get Video Script
        videoScript = GetComponent<Video>();
    }
	
    private Color Fade(Color color)
    {
        Color Temp = color;
        if (Temp.a > 0.0f)
        {
            // Fade accoding to Fading Rate
            Temp.a -= FadingRate * Time.deltaTime;
        }
        else
        {
            Temp.a = 0.0f;
        }
        return Temp;
    }

	// Update is called once per frame
	void Update ()
    {
        // When Briefing is active
        if(Briefing.activeSelf)
        {
            // Proceed to Fade
            Brief_Panel.color = Fade(Brief_Panel.color);
            Brief_Text.color = Fade(Brief_Text.color);

            // When finish fading out
            if(Brief_Panel.color.a == 0.0f && Brief_Text.color.a == 0.0f)
            {
                // Show Instruction
                Briefing.SetActive(false);
                Instruction.SetActive(true);
            }

            // Skip Briefing
            if(Input.GetKeyDown(KeyCode.Space))
            {
                Briefing.SetActive(false);
                Instruction.SetActive(true);
            }
        }
        // When Insturction is active
        else if(Instruction.activeSelf)
        {
            // Proceed to Fade
            Instruc_Panel.color = Fade(Instruc_Panel.color);
            Instruc_Title.color = Fade(Instruc_Title.color);
            Instruc_Image.color = Fade(Instruc_Image.color);
            Instruc_Text.color = Fade(Instruc_Text.color);

            // When finish fading out
            if (Instruc_Panel.color.a == 0.0f &&
                Instruc_Title.color.a == 0.0f && Instruc_Image.color.a == 0.0f && Instruc_Text.color.a == 0.0f)
            {
                // Play videos
                Instruction.SetActive(false);
                videoScript.videoPlayer.Play();
            }

            // Skip Insturction
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Instruction.SetActive(false);
                videoScript.videoPlayer.Play();
            }
        }
        // When Ending is active
        else if (Ending.activeSelf)
        {
            // Proceed to Fade
            Ending_Panel.color = Fade(Ending_Panel.color);
            Ending_Text.color = Fade(Ending_Text.color);

            // When finish fading out
            if (Ending_Panel.color.a == 0.0f && Ending_Text.color.a == 0.0f)
            {
                // Quit Application
                Ending.SetActive(false);
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }

            // Skip Ending and Quit Application
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Ending.SetActive(false);
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
        }
    }
}
