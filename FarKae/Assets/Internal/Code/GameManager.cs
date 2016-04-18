using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	public Image fadeImage;

	public float gameOverFadeDuration = 0.6f;
	public bool gameOver;

	void Awake()
	{
		instance = this;
	}

	public void GameOver(float fadeDelay)
	{
		fadeImage.DOFade(1f, gameOverFadeDuration)
			.SetDelay(fadeDelay)
			.OnComplete(() =>
			{
				SceneManager.LoadScene("game_over");
			});
	}
}
