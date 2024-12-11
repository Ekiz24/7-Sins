using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.SceneManagement; // 引入 SceneManagement 命名空间

public class AudioManager : MonoBehaviour
{
    [Header("------Audio Source------------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;
    // Start is called before the first frame update

    [Header("------Audio Clip-----------")]
    public AudioClip background;
    public AudioClip Slice;
    public AudioClip Chew;
    public AudioClip Coin;
    public AudioClip Coinfloor;
    public AudioClip Shoot;
    public AudioClip Puzzle;
    public static AudioManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }

    }
    private void Start()
    {
        // 检查当前场景名称
        if (SceneManager.GetActiveScene().name == "Sloth")
        {
            // 禁用脚本
            this.enabled = false;
            return;
        }

        musicSource.clip = background;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    public void Stop(AudioClip clip)
    {
        musicSource.Stop();
    }
}
