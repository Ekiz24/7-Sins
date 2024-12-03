using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Conductor : MonoBehaviour
{
	// 用于显示 "HIT" 或 "MISS"。
	public TextMesh statusText;
	public GameObject musicNotePrefab;

	// 一些音频文件可能在开始时包含一个空的间隔。我们将减去这个空的偏移量来计算歌曲的实际位置。
	public float songOffset;

	// 所有音乐音符在歌曲中的节拍位置应在编辑器中输入此数组。
	// 参见图片: http://shinerightstudio.com/posts/music-syncing-in-rhythm-games/pic1.png
	public float[] track;

	// 用于确定位置的四个 GameObject
	public GameObject startLineObject;
	public GameObject posYObject;
	public GameObject finishLineObject;
	public GameObject removeLineObject;
	public GameObject winScreen;
	public GameObject loseScreen;
	public GameObject[] trashBag;
	public GameObject[] missCounters;

	// 音符的起始位置X。
	private float startLineX;

	// 音符的位置Y。
	private float posY;

	// 音符的终点线（玩家击打的位置X）。
	private float finishLineX;

	// 音符应被销毁的位置X。
	private float removeLineX;

	// 容忍的偏移量位置。（如果玩家击打音符稍微不准确，我们会容忍并将其计为成功击打。）
	public float tolerationOffset;

	// 每个节拍持续多少秒。这可以通过 (60 / BPM) 计算。
	public float secondsPerBeat;

	// 屏幕上显示的节拍数。（可以想象成乐谱上的每小节节拍数。）
	public float BeatsShownOnScreen = 4f;

	// 播放歌曲。
	public AudioSource songAudioSource;

	// 播放节拍。
	public AudioSource beatAudioSource;

	// 当前歌曲位置。（我们不希望在编辑器中显示这个值，因此使用 "NonSerialized"）
	[NonSerialized] public float songposition;

	// 数组 "track" 的下一个索引。
	private int indexOfNextNote;

	// 队列，保存当前在屏幕上的音乐节点的引用。
	private int missCount;
	public int totalMissCount = 5;
	private Queue<MusicNote> notesOnScreen;

	// 记录上一帧音频引擎经过的时间。我们用这个来计算歌曲的位置。
	private float dsptimesong;

	private bool songStarted = false;
	private bool songFinished = false;
	private bool gameLost = false;

	void PlayerInputted()
	{
		// 如果歌曲还没有开始，则开始播放歌曲。
		if (!songStarted)
		{
			songStarted = true;
			StartSong();
			statusText.text = "";
			return;
		}

		// 播放节拍声音。
		beatAudioSource.Play();

		if (notesOnScreen.Count > 0)
		{
			// 获取队首的音符。
			MusicNote frontNote = notesOnScreen.Peek();

			// 音符到终点线的距离。
			float offset = Mathf.Abs(frontNote.gameObject.transform.position.x - finishLineX);

			// 音符命中。
			if (offset <= tolerationOffset)
			{
				// 将颜色更改为绿色以表示 "HIT"。
				frontNote.ChangeSprite(true);

				statusText.text = "HIT!";

				// 移除引用。（现在下一个音符移动到队列的最前面。）
				notesOnScreen.Dequeue();
			}
		}
	}

	void Start()
	{
		// 初始化位置变量
		if (startLineObject != null)
		{
			startLineX = startLineObject.transform.position.x;
		}
		if (posYObject != null)
		{
			posY = posYObject.transform.position.y;
		}
		if (finishLineObject != null)
		{
			finishLineX = finishLineObject.transform.position.x;
		}
		if (removeLineObject != null)
		{
			removeLineX = removeLineObject.transform.position.x;
		}

		// 初始化一些变量。
		notesOnScreen = new Queue<MusicNote>();
		indexOfNextNote = 0;
	}

	void StartSong()
	{
		// 使用 AudioSettings.dspTime 获取音频引擎的准确经过时间。
		dsptimesong = (float)AudioSettings.dspTime;

		// 播放歌曲。
		songAudioSource.Play();
	}

	void Update()
	{
		// 检查按键输入。
		if (Input.GetKeyDown(KeyCode.Space) && !gameLost)
		{
			PlayerInputted();
		}

		if (!songStarted) return;

		// 计算歌曲位置。（经过的时间 - 上一帧经过的时间）。
		songposition = (float)(AudioSettings.dspTime - dsptimesong - songOffset);

		// 检查是否需要实例化一个新的音符。（我们通过 (songposition / secondsPerBeat) 获取当前歌曲的节拍位置。）
		// 参见音符生成的图片（注意方向是反的）：
		// http://shinerightstudio.com/posts/music-syncing-in-rhythm-games/pic2.png
		float beatToShow = songposition / secondsPerBeat + BeatsShownOnScreen;

		// 检查轨道中是否还有音符，并检查下一个音符是否在我们打算显示在屏幕上的范围内。
		if (indexOfNextNote < track.Length && track[indexOfNextNote] < beatToShow && !gameLost)
		{

			// 实例化一个新的音乐音符。（如果你希望最小化实例化游戏对象时的延迟，可以搜索“对象池化”以获取更多信息。）
			// 我们不关心位置和旋转，因为我们稍后会在 MusicNote.Initialize(...) 中设置它们。
			MusicNote musicNote = ((GameObject)Instantiate(musicNotePrefab, Vector2.zero, Quaternion.identity)).GetComponent<MusicNote>();

			musicNote.Initialize(this, startLineX, finishLineX, removeLineX, posY, track[indexOfNextNote]);

			// 将音符推入队列以供参考。
			notesOnScreen.Enqueue(musicNote);

			// 更新下一个音符的索引。
			indexOfNextNote++;
		}

		// 检查歌曲是否播放完成
		if (songposition >= songAudioSource.clip.length && !songFinished)
		{
			songFinished = true;
			CheckGameResult();
		}

		// 循环队列以检查是否有音符到达终点线。
		if (notesOnScreen.Count > 0)
		{
			MusicNote currNote = notesOnScreen.Peek();

			if (currNote.transform.position.x <= finishLineX - tolerationOffset)
			{
				// 将颜色更改为红色以表示未命中。
				currNote.ChangeSprite(false);

				notesOnScreen.Dequeue();
				statusText.text = "MISS!";
				UpdateTrashBag();
				UpdateMissCount();
				missCount++;
				if (missCount >= totalMissCount)
				{
					songAudioSource.Stop();
					gameLost = true;
					loseScreen.SetActive(true);
				}
			}
		}
		// 注意，当音符到达移除线时，它会在 MusicNote 的 Update() 函数中自行移除。
	}

	private void UpdateTrashBag()
	{
		if (missCount < 4)
		{
			trashBag[missCount].SetActive(true);
		}
	}
	private void UpdateMissCount()
	{
		if (missCount < 5)
		{
			missCounters[missCount].SetActive(true);
		}
	}

	private void CheckGameResult()
	{
		if (missCount < 3)
		{
			winScreen.SetActive(true);
			PlayerPrefs.SetInt("SlothCompleted", 1);
			PlayerPrefs.Save();
			Debug.Log("You Win!");
		}
		else
		{
			loseScreen.SetActive(true);
		}
	}

	public void RestartGame()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene("Sloth");
	}

	public void ExitToMainMenu()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
	}
}
