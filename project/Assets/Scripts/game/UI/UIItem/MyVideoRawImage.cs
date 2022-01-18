
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

/// <summary>
/// 视频贴图
/// </summary>
public class MyVideoRawImage : MyUIItem
{
    public enum VideoImageType
    { path, url }

    private RenderTexture rt;

    public void Play()
    {
        this.videoPlayer.Play();
    }

    public void Stop()
    {
        this.videoPlayer.Stop();
    }

    public void Pause()
    {
        this.videoPlayer.Pause();
    }

    public void SetData(string path, VideoImageType type, VideoPlayer.EventHandler startAction = null, VideoPlayer.EventHandler endAction = null,
     VideoPlayer.EventHandler prepareCompletedAction = null)
    {
        this.rt = new RenderTexture(1920, 1080, 16, RenderTextureFormat.ARGB32);
        this.myRawImage.texture = this.rt;
        this.videoPlayer.targetTexture = this.rt;

        if (type == VideoImageType.path)
        {
            this.videoPlayer.source = VideoSource.VideoClip;
            this.videoPlayer.clip = Resources.Load<VideoClip>(@"Video\" + path);
        }
        else
        {
            this.videoPlayer.source = VideoSource.Url;
            this.videoPlayer.url = path;
        }
        if (startAction != null)
            this.videoPlayer.started += startAction;
        if (endAction != null)
            this.videoPlayer.loopPointReached += endAction;
        if (prepareCompletedAction != null)
            this.videoPlayer.prepareCompleted += prepareCompletedAction;
    }

    protected override void OnClose()
    {
        if (this.rt)
        {
            this.rt.Release();
        }
        this.Stop();
        this.videoPlayer = null;
    }

    #region UI_AUTOCODE_RC 436f0426f32b40228d397790b355e158

    public RawImage myRawImage;
    public VideoPlayer videoPlayer;

    public void CacheReference()
    {
        var rc = this.gameObject.GetComponent<ReferenceCollector>();
        this.myRawImage = rc.GetReference<RawImage>(0); // name: MyVideoRawImage
        this.videoPlayer = rc.GetReference<VideoPlayer>(1); // name: MyVideoRawImage
    }

    #endregion

}