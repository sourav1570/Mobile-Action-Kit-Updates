using System.Collections;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine;
using System;

namespace MobileActionKit
{
    public class VideoLoadingController : MonoBehaviour
    {
        public RawImage RawImageToShowVideo;
        public VideoPlayer Videoplayer;
        public AudioSource Audiosource;
        double ClipLength;
        double ClipTime;
        public GameObject TapToContinue_Button;

        [HideInInspector]
        public int VideoLengthTime;
        [HideInInspector]
        public int VideoPlayerTime;

        private void Start()
        {
            ClipLength = Videoplayer.clip.length;
            VideoLengthTime = Convert.ToInt32(ClipLength);
            Videoplayer.Prepare();
        }
        public void ShowVideo()
        {
            StartCoroutine(Playvideo());
        }
        IEnumerator Playvideo()
        {
            WaitForSeconds wait = new WaitForSeconds(1);
            while (!Videoplayer.isPrepared)
            {
                yield return wait;
                break;
            }
            RawImageToShowVideo.texture = Videoplayer.texture;
            Videoplayer.Play();
            Audiosource.Play();
        }
        void Update()
        {
            ClipTime = Videoplayer.time;
            VideoPlayerTime = Convert.ToInt32(ClipTime);
            if (VideoPlayerTime >= VideoLengthTime)
            {
                RawImageToShowVideo.gameObject.SetActive(false);
                TapToContinue_Button.SetActive(true);
            }
        }
    }
}
