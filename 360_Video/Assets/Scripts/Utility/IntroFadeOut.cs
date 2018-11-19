//--------------------------------------------------------------------------------
/*
 * This Script is used for fading out of Briefing, Instructions and Ending.
 * Allow input to : Skip(Briefing, Instructions, Ending)
 * 
 * Used in Main Scene, attached to Empty GameObject "Script GameObject"
 * Require 3 GameObject variables : Briefing, Instruction, Ending
 * which can be found under Canvas GameObject, which is under Camera(eye) of [CameraRig] in Hierarchy.
 * Among the 3 GameObjects, only Briefing's GameObject need to be active.
 */
//--------------------------------------------------------------------------------

using TMPro; // For TextMeshProGUI
using UnityEngine;  // Default Unity Script (MonoBehaviour, Application, Header, Tooltip, Debug, Time)
using UnityEngine.UI; // For Image

public class IntroFadeOut : MonoBehaviour
{
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
    
    // Reference to CountDown Script
    private CountDown countDownScript;

    // Runs at the start of first frame
    private void Start ()
    {
        // Ensure there's Brefing to set reference to its child/granchild
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

        // Ensure there's Instruction to set reference to its child/granchild
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

        // Ensure there's Ending to set reference to its child/granchild
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
        
        // Get reference to CountDown Script
        countDownScript = GetComponent<CountDown>();
    }
	
    // Returns color with decreased alpha component in color at FadingRate
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

    // Runs at every frame
    private void Update ()
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
                // Hide Briefing
                Briefing.SetActive(false);

                // Show Instruction
                Instruction.SetActive(true);
            }

            // Skip Briefing
            if(Input.GetKeyDown(KeyCode.Space))
            {
                // Hide Briefing
                Briefing.SetActive(false);

                // Show Instruction
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
                // Hide Instruction
                Instruction.SetActive(false);
                // Show Count Down
                countDownScript.CountDownTMP.gameObject.SetActive(true);
            }

            // Skip Insturction
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // Hide Instruction
                Instruction.SetActive(false);
                // Show Count Down Text
                countDownScript.CountDownTMP.gameObject.SetActive(true);
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
                // Hide Ending
                Ending.SetActive(false);
#if UNITY_EDITOR
                // Stop running Application
                UnityEditor.EditorApplication.isPlaying = false;
#else
                // Quit Application
                Application.Quit();
#endif
            }

            // Skip Ending and Quit Application
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // Hide Ending
                Ending.SetActive(false);
#if UNITY_EDITOR
                // Stop running Application
                UnityEditor.EditorApplication.isPlaying = false;
#else
                // Quit Application
                Application.Quit();
#endif
            }
        }
    }
}
