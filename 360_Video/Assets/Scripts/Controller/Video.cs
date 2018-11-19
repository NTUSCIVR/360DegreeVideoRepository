//--------------------------------------------------------------------------------
/*
 * This Script is used for playing 360 Degree Video once, and show Questionnaire for Rating afterwards.
 * Allow Inputs to : Restart(Load StartScene and Input User ID and video choice again),
 *                   Increase Volume,
 *                   Decrese Volume,
 *                   Skip Video
 * 
 * Used in Main Scene, attached to Empty GameObject "Script GameObject".
 * Require 1 VideoPlayer variable and 3 GameObject variables : Questionnaire, Left Controller Model, Right Controller Model
 * Which Questionnaire can be found under Canvas GameObject, which is under Camera(eye) of [CameraRig] in Hierarchy,
 * VideoPlayer can be found as "Video" in Hierarchy,
 * Controller Models can be found under Controller(left/right), which is under [CameraRig] in Hierarchy.
 * Among the variables, only Questionnaire's GameObject do not need to be active.
 */
//--------------------------------------------------------------------------------

using System.IO; // For DirectoryInfo, SearchOption
using UnityEngine;  // Default Unity Script (MonoBehaviour, GameObject, Header, Tooltip, Debug, FindObjectOfType, RenderTexture, Destroy, Input, KeyCode)
using UnityEngine.Video; // For VideoPlayer, VideoRenderMode, VideoSource
using System.Collections.Generic; // For List<>
using UnityEngine.SceneManagement; // For SceneManager

public class Video : MonoBehaviour
{
    // Reference to video player
    public VideoPlayer videoPlayer;
    
    [Tooltip("Under [CameraRig] -> Camera(head) -> Camera(eye) -> Canvas")]
    public GameObject Questionnaire;
    
    [Header("Controller Models under [CameraRig]")]
    public GameObject ContLeft;
    public GameObject ContRight;

    private List<string> videoUrls;
    private int videoIndex;

    // Enable these variable when need to play multiple videos at one go
    // Disable these variable when only need to play one video
    //[Tooltip("Default: 3 videos")]
    //public int videosToPlay = 3;
    //private List<string> videoUrlsToPlay;

    // Runs before Start()
    private void Awake()
    {
        // If DataCollector is alive
        if (DataCollector.Instance != null)
        {
            // Find the SteamVR eye GameObject and assign it to DataCollector
            DataCollector.Instance.user = FindObjectOfType<SteamVR_Camera>().gameObject;

            // Applies VideoIndex
            videoIndex = DataCollector.Instance.VideoID;
        }
    }

    // Runs at the start of first frame
    private void Start ()
    {
        // Initiate List
        videoUrls = new List<string>();

        // Load videos from C:/360DegreeVideos/
        LoadVideos();
        
        // Enable this line of code when only need to play one selected video
        // Disable this line of code when need to play multiple videos at one go
        CheckDimensionsNSetVideo(videoUrls[videoIndex - 1]);

        // Enable this section of codes when need to play multiple videos at one go
        // Disable this section of codes when only need to play one selected video
        // Intiate List
        //videoUrlsToPlay = new List<string>();
        // Ensure there's video/s to play
        //if (videoUrlsToPlay.Count < 1)
        //{
        //    Debug.LogError("Video Clips not found.");
        //    return;
        //}
        //else
        //{
        //    // Set "1st video to play" to 1st clip in videoClips
        //    CheckDimensionsNSetVideo(videoUrlsToPlay[0]);
        //}
        // Randomly select video from videoClips
        //RandomSelectVideos();

        // Listen for video finish playing's event call, calls FinishPlaying() afterwards
        videoPlayer.loopPointReached += FinishPlaying;
    }

    // Load videos from C:/360DegreeVideos/
    private void LoadVideos()
    {
        DirectoryInfo directory = new DirectoryInfo(@"C:\360DegreeVideos\");

        // Loads every mp4 file path into List of string, videoUrls
        foreach (var file in directory.GetFiles("*.mp4", SearchOption.AllDirectories))
        {
            videoUrls.Add(directory.FullName + file.Name);
        }

        // Trim Excess to prevent extra space in List
        videoUrls.TrimExcess();
    }

    // Make a temporary VideoPlayer GameObject to get dimensions of chosen video
    // Proceed to set the video as soon as it is prepared
    private void CheckDimensionsNSetVideo(string url)
    {
        GameObject tempVideo = new GameObject();
        VideoPlayer tempVideoPlayer = tempVideo.AddComponent<VideoPlayer>();
        tempVideoPlayer.renderMode = VideoRenderMode.RenderTexture;
        tempVideoPlayer.targetTexture = new RenderTexture(1, 1, 0);
        tempVideoPlayer.source = VideoSource.Url;
        tempVideoPlayer.url = url;

        // Listen for prepare complete event call, and proceed to set the video with prepared information
        tempVideoPlayer.prepareCompleted += (VideoPlayer source) =>
        {
            SetVideo(url, source.texture.width, source.texture.height);
            Destroy(tempVideo);
        };

        tempVideoPlayer.Prepare();
    }

    // Set video with passed in url and dimension we got from CheckDimensionsNSetVideo()
    private void SetVideo(string url, int width, int height)
    {
        videoPlayer.url = url;
        RenderTexture texture = new RenderTexture(width, height, 24);
        videoPlayer.targetTexture = texture;

        // Set RenderTexture onto Skybox
        RenderSettings.skybox.SetTexture("_MainTex", texture);
    }

    // Stops Playing Video and Quit Application
    private void FinishPlaying(VideoPlayer _videoPlayer)
    {
        // Stop playing video
        _videoPlayer.Stop();
        
        // Clear Screen
        RenderTexture texture = new RenderTexture(1, 1, 0);
        _videoPlayer.targetTexture = texture;
        RenderSettings.skybox.SetTexture("_MainTex", texture);

        // Enable this section of codes when need to play multiple videos at one go
        // Disable this section of codes when only need to play one video
        //if (videoUrlsToPlay.Count >= 1)
        //{
        //    // Remove played video clip
        //    videoUrlsToPlay.Remove(videoUrlsToPlay[0]);
        //    videoUrlsToPlay.TrimExcess();
        //}

        // Allow Rating
        Questionnaire.SetActive(true);
    }
    
    // Loads StartScene
    private void Restart()
    {
        SceneManager.LoadScene("StartScene");
        // Destroy current DataCollector Instance, as StartScene will have its new instance of DataCollector
        Destroy(DataCollector.Instance.gameObject);
    }

    // Update is called once per frame
    private void Update ()
    {
        // When video is playing
		if (videoPlayer.isPlaying)
        {
            // Hide Controller models
            ContLeft.SetActive(false);
            ContRight.SetActive(false);

            // Skip to second last second
            if (Input.GetKeyDown(KeyCode.Space))
            {
                FinishPlaying(videoPlayer);
            }
        }
        else
        {
            // Show Controller models
            ContLeft.SetActive(true);
            ContRight.SetActive(true);
        }

        // Proceed to Restart if 'R' is pressed
        if (Input.GetKey(KeyCode.R))
        {
            Restart();
        }

        // Increase volume by 0.1f if 'Up Arrow' is pressed
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            if (videoPlayer.GetDirectAudioVolume(0) + 0.1f < 1f)
                videoPlayer.SetDirectAudioVolume(0, videoPlayer.GetDirectAudioVolume(0) + 0.1f);
            else
                videoPlayer.SetDirectAudioVolume(0, 1f);
        }

        // Decrease volume by 0.1f if 'Down Arrow' is pressed
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            if (videoPlayer.GetDirectAudioVolume(0) - 0.1f > 0f)
                videoPlayer.SetDirectAudioVolume(0, videoPlayer.GetDirectAudioVolume(0) - 0.1f);
            else
                videoPlayer.SetDirectAudioVolume(0, 0f);
        }
    }

    // Enable this section of codes when need to play multiple videos at one go
    // Disable this section of codes when only need to play one video
    //public void ChangeToNextVideo()
    //{
    //    // When still have video clips to play
    //    if (videoUrlsToPlay.Count >= 1)
    //    {
    //        // Change video clip to the next one
    //        CheckDimensionsNSetVideo(videoUrlsToPlay[0]);
    //    }
    //    else
    //    {
    //        videoPlayer.enabled = false;
    //    }
    //}

    // Enable this section of codes when need to play multiple videos at one go
    // Disable this section of codes when only need to play one video
    // Randomly select video from videoClips
    //private void RandomSelectVideos()
    //{
    //    List<string> tempVideoUrls = new List<string>();

    //    // Copy video clips into temporary list
    //    foreach (string clip in videoUrls)
    //    {
    //        tempVideoUrls.Add(clip);
    //    }
    //    tempVideoUrls.TrimExcess();

    //    int counter = videosToPlay;

    //    // Loop until finish random selecting videos
    //    while (counter > 0)
    //    {
    //        // Random an index within 0 to number of clips in videoClips
    //        int randomIndex = Random.Range(0, tempVideoUrls.Count);

    //        // Add randomly selected clip into videoClipsToPlay
    //        videoUrlsToPlay.Add(tempVideoUrls[randomIndex]);
    //        videoUrlsToPlay.TrimExcess();

    //        // Remove selected clip to prevent same video being chosen
    //        tempVideoUrls.Remove(tempVideoUrls[randomIndex]);
    //        tempVideoUrls.TrimExcess();

    //        counter -= 1;
    //    }
    //    tempVideoUrls.Clear();
    //}
}
