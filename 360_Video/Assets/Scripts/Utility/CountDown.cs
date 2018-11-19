//--------------------------------------------------------------------------------
/*
 * This Script is used for counting down and play video afterwards.
 * 
 * Used in Main Scene, attached to Empty GameObject "Script GameObject"
 * Require 1 GameObject variable : CountDown Text MeshPro Text
 * which can be found under Canvas GameObject, which is under Camera(eye) of [CameraRig] in Hierarchy.
 * Do not need to be active.
 */
//--------------------------------------------------------------------------------

using TMPro; // For TextMeshProGUI
using UnityEngine;  // Default Unity Script (MonoBehaviour, Header, GetComponent, Mathf, Vector3, Time)
using UnityEngine.Video; // For VideoPlayer

public class CountDown : MonoBehaviour
{
    [Header("Under [CameraRig] -> Camera(head) -> Camera(eye) -> Canvas")]
    public TextMeshProUGUI CountDownTMP;

    // Reference to video player component
    private VideoPlayer videoPlayer;

    // For Count Down
    private float CountDownTimer = 5.0f;
    private float CountDownScale = 1.0f;

    // Runs at the start of first frame
    private void Start()
    {
        // Get reference to video player
        videoPlayer = GetComponent<Video>().videoPlayer;
    }

    // Runs at every frame
    private void Update ()
    {
        // When Count Down Text is active
        if (CountDownTMP.gameObject.activeSelf)
        {
            // Start Counting down
            CountDownTimer -= Time.deltaTime;

            // Round off to smallest integer higher than CountDownTimer and convert to string for setting to text
            CountDownTMP.text = Mathf.CeilToInt(CountDownTimer).ToString();

            // Scale down every second
            CountDownScale -= Time.deltaTime;
            CountDownTMP.transform.localScale = new Vector3(CountDownScale, CountDownScale, CountDownScale);

            // Pop back to full size every next second
            if (CountDownScale <= 0.0f)
            {
                CountDownScale = 1.0f;
            }

            if (CountDownTimer <= 0.0f)
            {
                // Reset Countdown
                CountDownTimer = 5.0f;
                CountDownScale = 1.0f;
                CountDownTMP.transform.localScale = new Vector3(CountDownScale, CountDownScale, CountDownScale);

                // Play video
                videoPlayer.Play();

                // Hide Count Down Text
                CountDownTMP.gameObject.SetActive(false);
            }
        }
    }
}
