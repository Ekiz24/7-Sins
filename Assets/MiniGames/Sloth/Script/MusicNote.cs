using System;
using UnityEngine;

public class MusicNote : MonoBehaviour
{
	// Keep a reference of the conductor.
	public Conductor conductor;

	// We keep the start and end positionX to perform interpolation.
	public float startX;
	public float endX;
	public float removeLineX;
	public float beat;

	public Sprite hitSprite;
	public Sprite missSprite;
	private SpriteRenderer spriteRenderer;

	public void Initialize(Conductor conductor, float startX, float endX, float removeLineX, float posY, float beat)
	{
		this.conductor = conductor;
		this.startX = startX;
		this.endX = endX;
		this.beat = beat;
		this.removeLineX = removeLineX;

		// Set to initial position.
		transform.position = new Vector2(startX, posY + 0.2f); // 0.2f 是使音符出现在线条上方的偏移量。

		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	void Update()
	{
		// 根据歌曲的位置更新音符的位置。
		// （将其视为每帧根据歌曲的位置“重置”而不是“更新”音符的位置。）
		// 参见图片: http://shinerightstudio.com/posts/music-syncing-in-rhythm-games/pic3.png（注意方向是反的。）
		transform.position = new Vector2(startX + (endX - startX) * (1f - (beat - conductor.songposition / conductor.secondsPerBeat) / conductor.BeatsShownOnScreen), transform.position.y);

		// 当超出屏幕（移除线）时移除自身。
		if (startX > endX && transform.position.x < removeLineX)
		{
			Destroy(gameObject);
		}
		else if (startX < endX && transform.position.x > removeLineX)
		{
			Destroy(gameObject);
		}
	}

	// Change the color to indicate whether its a "HIT" or a "MISS".
	public void ChangeSprite(bool hit)
	{
		if (hit)
		{
			spriteRenderer.sprite = hitSprite;
		}
		else
		{
			spriteRenderer.sprite = missSprite;
		}
	}
}
