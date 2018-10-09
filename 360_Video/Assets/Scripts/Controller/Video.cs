using System.IO;
using UnityEngine;
using UnityEngine.Video;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Video : MonoBehaviour
{
    [Tooltip("Sphere that envelops [CameraRig]")]
    public VideoPlayer videoPlayer;
    //[Tooltip("Default: 3 videos")]
    //public int videosToPlay = 3;
    [Tooltip("Empty GameObject that holds Questionnaire under [CameraRig] -> Camera(head) -> Camera(eye) -> Canvas")]
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
    
    void Start ()
    {
        // Load videos from Assets/Imports/Videos
        videoUrls = new List<string>();
        LoadVideos();

        videoUrlsToPlay = new List<string>();
        // Copy video clips into temporary list
        foreach (string clip in videoUrls)
        {
            videoUrlsToPlay.Add(clip);
        }
        videoUrlsToPlay.TrimExcess();

        //// Randomly select video from videoClips
        //RandomSelectVideos();

        // Ensure there's video/s to play
        if (videoUrlsToPlay.Count < 1)
        {
            Debug.LogError("Video Clips not found.");
            return;
        }
        else
        {
            // TODO: Set to play the video which developer had previously input in DataCollector
            videoPlayer.url = videoUrlsToPlay[videoIndex];

            //// Set "1st video to play" to 1st clip in videoClips
            //videoPlayer.url = videoUrlsToPlay[0];
        }
        // Listen for end point of each video
        videoPlayer.loopPointReached += FinishPlaying;
    }

    private void LoadVideos()
    {
        DirectoryInfo directory = new DirectoryInfo(@"C:\360DegreeVideos\");

        // Load .mp4 videos from Assets/Imports/Videos/ and add into Url list
        foreach (var file in directory.GetFiles("*.mp4", SearchOption.AllDirectories))
        {
            videoUrls.Add(directory.FullName + file.Name);
        }
        videoUrls.TrimExcess();
    }

    // Randomly select video from videoClips
    //private void RandomSelectVideos()
    //{
    //    List<string> tempVideoUrls = new List<string>();
    //    videoUrlsToPlay = new List<string>();

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

        //if (videoUrlsToPlay.Count >= 1)
        //{
        //    // Remove played video clip
        //    videoUrlsToPlay.Remove(videoUrlsToPlay[0]);
        //    videoUrlsToPlay.TrimExcess();
        //}
        // Allow Rating
        Questionnaire.SetActive(true);
    }

    //public void ChangeToNextVideo()
    //{
    //    // When still have video clips to play
    //    if (videoUrlsToPlay.Count >= 1)
    //    {
    //        // Change video clip to the next one
    //        videoPlayer.url = videoUrlsToPlay[0];
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

    // Update is called once per frame
    void Update ()
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
