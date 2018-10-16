using System.IO;
using UnityEngine;
using UnityEngine.Video;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Video : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    // Enable this variable when need to play multiple videos at one go
    // Disable this variable when only need to play one video
    //[Tooltip("Default: 3 videos")]
    //public int videosToPlay = 3;

    [Tooltip("Under [CameraRig] -> Camera(head) -> Camera(eye) -> Canvas")]
    public GameObject Questionnaire;
    
    [Header("Controller Models under [CameraRig]")]
    public GameObject ContLeft;
    public GameObject ContRight;

    private List<string> videoUrls;
    private List<string> videoUrlsToPlay;
    private int videoIndex;

    private void Awake()
    {
        if (DataCollector.Instance != null)
        {
            //find the steamvr eye and assign it to data collector
            DataCollector.Instance.user = FindObjectOfType<SteamVR_Camera>().gameObject;
            videoIndex = DataCollector.Instance.VideoID;
        }
    }

    private void Start ()
    {
        // Load videos from C:/360DegreeVideos/
        videoUrls = new List<string>();
        LoadVideos();
        videoUrlsToPlay = new List<string>();

        // Randomly select video from videoClips
        //RandomSelectVideos();

        // Enable this line of code when only need to play one selected video
        // Disable this line of code when need to play multiple videos at one go
        CheckDimensionsNSetVideo(videoUrls[videoIndex]);

        // Enable this section of codes when need to play multiple videos at one go
        // Disable this section of codes when only need to play one selected video
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

        // Listen for end point of each video
        videoPlayer.loopPointReached += FinishPlaying;
    }

    private void LoadVideos()
    {
        DirectoryInfo directory = new DirectoryInfo(@"C:\360DegreeVideos\");

        // Load .mp4 videos from C:/360DegreeVideos/ and add into Url list
        foreach (var file in directory.GetFiles("*.mp4", SearchOption.AllDirectories))
        {
            videoUrls.Add(directory.FullName + file.Name);
        }
        videoUrls.TrimExcess();
    }

    // Make a temporary VideoPlayer GameObject to get dimensions of chosen video
    private void CheckDimensionsNSetVideo(string url)
    {
        GameObject tempVideo = new GameObject();
        VideoPlayer tempVideoPlayer = tempVideo.AddComponent<VideoPlayer>();
        tempVideoPlayer.renderMode = VideoRenderMode.RenderTexture;
        tempVideoPlayer.targetTexture = new RenderTexture(1, 1, 0);
        tempVideoPlayer.source = VideoSource.Url;
        tempVideoPlayer.url = url;
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
        RenderSettings.skybox.SetTexture("_MainTex", texture);
    }

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

    private void Restart()
    {
        SceneManager.LoadScene("StartScene");
        Destroy(DataCollector.Instance.gameObject);
    }
    
    private void Update ()
    {
		if (videoPlayer.isPlaying)
        {
            // Hide Controller models when video is playing
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

        // Restart from start scene
        if (Input.GetKey(KeyCode.R))
        {
            Restart();
        }
    }
}
