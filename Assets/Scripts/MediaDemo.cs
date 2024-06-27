using RenderHeads.Media.AVProVideo;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Demo {
    public class MediaDemo : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            MediaManager.m_Ins.SetMediaUrl("AVProVideoSamples/BigBuckBunny-360p30-H264.mp4",MediaPathType.RelativeToStreamingAssetsFolder);
            MediaManager.m_Ins.GetMediaPlayer.Events.AddListener(OnVideoEvent);
            //²¥·Å
         //   MediaManager.m_Ins.Play();
            //ÔÝÍ£
           // MediaManager.m_Ins.Pause();
           
        }

        private void OnVideoEvent(MediaPlayer arg0, MediaPlayerEvent.EventType arg1, ErrorCode arg2)
        {
            switch (arg1)
            {
                case MediaPlayerEvent.EventType.MetaDataReady:
                    break;
                case MediaPlayerEvent.EventType.ReadyToPlay: 
                    break;
                case MediaPlayerEvent.EventType.Started: 
                    break;
                case MediaPlayerEvent.EventType.FirstFrameReady:
                    break;
                case MediaPlayerEvent.EventType.FinishedPlaying:
                    break;
                case MediaPlayerEvent.EventType.Closing:
                    print(".Closing...");
                    break;
                case MediaPlayerEvent.EventType.Error:
                    print(".Error...");
                    break;
                case MediaPlayerEvent.EventType.SubtitleChange:
                    break;
                case MediaPlayerEvent.EventType.Stalled:
                    break;
                case MediaPlayerEvent.EventType.Unstalled:
                    break;
                case MediaPlayerEvent.EventType.ResolutionChanged:
                    break;
                case MediaPlayerEvent.EventType.StartedSeeking:
                    break;
                case MediaPlayerEvent.EventType.FinishedSeeking:
                    print("²¥·Å½áÊø...");
                    break;
                case MediaPlayerEvent.EventType.StartedBuffering:
                    break;
                case MediaPlayerEvent.EventType.FinishedBuffering: 
                    break;
                case MediaPlayerEvent.EventType.PropertiesChanged:
                    break;
                case MediaPlayerEvent.EventType.PlaylistItemChanged:
                    break;
                case MediaPlayerEvent.EventType.PlaylistFinished:
                    break;
                case MediaPlayerEvent.EventType.TextTracksChanged:
                    break;
                //case MediaPlayerEvent.EventType.TextCueChanged://=>  SubtitleChange
                //  break;
                default:
                    break;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                 MediaManager.m_Ins.SetMediaUrl("AVProVideoSamples/RenderHeads-1080p30-H264.mp4", MediaPathType.RelativeToStreamingAssetsFolder);
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                MediaManager.m_Ins.Stop(); 
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                MediaManager.m_Ins.Rewind(); 
            }
        }
    }
}
