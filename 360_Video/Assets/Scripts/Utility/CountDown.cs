using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class CountDown : MonoBehaviour
{

    [Tooltip("Sphere that envelops [CameraRig]")]
    public VideoPlayer videoPlayer;

    [Header("Under [CameraRig] -> Camera(head) -> Camera(eye) -> Canvas")]
    public TextMeshProUGUI CountDownTMP;

    private float CountDownTimer = 5.0f;
    private float CountDownScale = 1.0f;

	void Update ()
    {
        if (CountDownTMP.gameObject.activeSelf)
        {
            // Start Counting down
            CountDownTimer -= Time.deltaTime;
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
                // Reset Countdown then play next video
                CountDownTimer = 5.0f;
                CountDownScale = 1.0f;
                CountDownTMP.transform.localScale = new Vector3(CountDownScale, CountDownScale, CountDownScale);
                videoPlayer.Play();
                CountDownTMP.gameObject.SetActive(false);
            }
        }
    }
}
