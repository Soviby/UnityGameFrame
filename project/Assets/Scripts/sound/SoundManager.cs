using FMODUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
/// <summary>
/// Fmod音效管理器
/// </summary>
public class SoundManager
{
    //音效大小
    public static float Volume_Sound
    {
        get
        {
            return 1;
        }
    }
    //音乐大小
    public static float Volume_Music
    {
        get
        {
            return 1;
        }
    }

    static GameObject _root;
    static Transform _target;

    public static bool InitOK
    {
        get
        {
            return _root && InitBankLoaded;
        }
    }

    public static bool InitBankLoaded
    {
        get
        {
            foreach (string b in BanksKey)
            {
                if (!FMODUnity.RuntimeManager.HasBankLoaded(b))
                    return false;
            }

            return true;
        }
    }

    public static Transform Target { get => _target; set => _target = value; }

    private static List<string> BanksKey = new List<string>
    {
        "bgm",
        "bgm.strings",
        "effect",
    };

    static List<StudioEventEmitter> _3dstudioEventList = new List<StudioEventEmitter>();
    //初始化
    public static void Init()
    {
        _root = RuntimeManager.Instance.gameObject;

        //加载Bank
        foreach (string b in BanksKey)
        {
            FMODUnity.RuntimeManager.LoadBank(b, true);
            Debug.Log("Loaded bank " + b);
        }
    }

    public static void LateUpdate()
    {
        if (Target)
        {
            _root.transform.position = Target.transform.position + new Vector3(0, 0, 0);
            _root.transform.rotation = Target.transform.rotation;
            RuntimeManager.SetListenerLocation(_root.transform);//每帧设置收听者的位置
        }
    }
    //所有音乐暂停和恢复(安卓)
    public static void OnAppPause(bool pause)
    {
        RuntimeManager.PauseAllEvents(pause);
        if (pause)
        {//暂停
            RuntimeManager.CoreSystem.mixerSuspend();
        }
        else
        {//恢复
            RuntimeManager.CoreSystem.mixerResume();
        }
    }

    //所有音乐暂停和恢复(苹果)
    public static void OnAppFocus(bool pause)
    {
        RuntimeManager.PauseAllEvents(!pause);
        if (!pause)
        {//暂停
            RuntimeManager.CoreSystem.mixerSuspend();
        }
        else
        {//恢复
            RuntimeManager.CoreSystem.mixerResume();
        }
    }

    //停止所有音乐
    public static void StopAllSound()
    {
        if (!_root) return;
        var fmodList = _root.GetComponentsInChildren<StudioEventEmitter>();
        if (fmodList == null) return;
        for (int i = 0; i < fmodList.Length; i++)
        {
            fmodList[i].Stop();
            GameObject.Destroy(fmodList[i].gameObject);
        }
    }
    //停止指定的音乐
    public static void StopMusic(string eventName)
    {
        var eventNameGo = eventName.Replace('/', '_');
        var studioEvent = _root.FindChild<StudioEventEmitter>(eventNameGo);
        if (studioEvent)
        {
            studioEvent.Stop();
            UnityEngine.Object.Destroy(studioEvent.gameObject);
        }
    }
    //停止当前音乐
    public static string StopCurrentMusic()
    {
        string currentMusic = _current_music;
        StopMusic(currentMusic);
        _current_music = "";
        return currentMusic;
    }

    private static string _current_music = "";
    //播放音乐
    public static void PlayMusic(string eventName)
    {
        if (!string.IsNullOrEmpty(_current_music))
            StopMusic(_current_music);
        _current_music = eventName;
        _playSoundDo(eventName);
    }

    //播放音效
    public static void PlaySound(string eventName)
    {
        _PlaySound(eventName, null);
    }
    //播放音效
    public static void PlaySound(string eventName, GameObject voicer)
    {
        _PlaySound(eventName, voicer);
    }

    private static void _PlaySound(string eventName, GameObject voicer)
    {
        if (string.IsNullOrEmpty(eventName))
            return;
        if (voicer)
        {
            RuntimeManager.PlayOneShotAttached(eventName, voicer, Volume_Sound);
        }
        else
        {
            RuntimeManager.PlayOneShot(eventName, default, Volume_Sound);
        }
    }

    public static void _playSoundDo(string eventName, GameObject voicer = null)
    {
        StudioEventEmitter studioEvent = null;
        if (!voicer)
        {
            var eventNameGo = eventName.Replace('/', '_');
            studioEvent = _root.FindChild<StudioEventEmitter>(eventNameGo, false);
            if (!studioEvent)
            {
                var go = new GameObject(eventNameGo);
                go.name = eventNameGo;
                go.transform.SetParent(_root.transform);
                go.transform.localPosition = Vector3.zero;
                studioEvent = go.GetMissComponent<StudioEventEmitter>();
                studioEvent.OverrideAttenuation = true;
                studioEvent.OverrideMaxDistance = int.MaxValue;
                studioEvent.OverrideMinDistance = int.MaxValue;
                studioEvent.SetVolume(Volume_Music);
            }
        }
        else
        {
            studioEvent = voicer.GetMissComponent<StudioEventEmitter>();
            if (studioEvent.IsPlaying())
                studioEvent.Stop();
            studioEvent.OverrideAttenuation = true;
            studioEvent.OverrideMaxDistance = 20;
            studioEvent.OverrideMinDistance = 3;
            studioEvent.SetVolume(Volume_Music);
            if (!_3dstudioEventList.Contains(studioEvent))
                _3dstudioEventList.Add(studioEvent);
        }
        studioEvent.Event = eventName;
        studioEvent.Play();
    }

    public static void GMBMGStop(string[] scene_name)
    {
        StopCurrentMusic();
    }

    public static void GMBMGPlay(string[] scene_name)
    {
        PlayMusic("event:/" + scene_name[0]);
    }

    public static void GMEffectPlay(string[] scene_name)
    {
        PlaySound("event:/" + scene_name[0]);
    }

}