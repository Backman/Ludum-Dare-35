using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using MonsterLove.StateMachine;
using UnityEngine.SceneManagement;

public class IntroScreen : MonoBehaviour
{
	enum State
	{
		Splash,
		Controls,
		StartGame
	}

	public Image splashImage;
	public Image controlImage;
	public Image fadeImage;
	public AudioClip clickSound;
	private bool _canClick;
	public float fadeInDuration = 1.5f;
	public float nextImageFadeInDuration = 0.7f;
	public float nextImageFadeOutDuration = 0.7f;
	public float startGameFadeDuration = 1f;

	StateMachine<State> _fsm;

	void Awake()
	{
		fadeImage.gameObject.SetActive(true);
		_fsm = StateMachine<State>.Initialize(this, State.Splash);
	}

	void Start()
	{
		fadeImage.DOFade(0f, fadeInDuration)
			.OnComplete(() =>
			{
				_canClick = true;
			});
	}

	void Splash_Update()
	{
		if (_canClick && Input.anyKeyDown)
		{
			_fsm.ChangeState(State.Controls);
			Music.PlayClipAtPoint(clickSound, Camera.main.transform.position, 0.3f, 1f);
		}
	}

	void Controls_Enter()
	{
		_canClick = false;
		var sequence = DOTween.Sequence();
		sequence.Append(fadeImage.DOFade(1f, nextImageFadeInDuration))
			.AppendCallback(() =>
			{
				splashImage.gameObject.SetActive(false);
				controlImage.gameObject.SetActive(true);
			})
			.Append(fadeImage.DOFade(0f, nextImageFadeOutDuration))
			.AppendCallback(() =>
			{
				_canClick = true;
			});
	}

	void Controls_Update()
	{
		if (_canClick && Input.anyKeyDown)
		{
			_fsm.ChangeState(State.StartGame);
			Music.PlayClipAtPoint(clickSound, Camera.main.transform.position, 0.3f, startGameFadeDuration);
		}
	}

	void StartGame_Enter()
	{
		var sequence = DOTween.Sequence();
		sequence.Append(fadeImage.DOFade(1f, 1f))
			.AppendCallback(() =>
			{
				SceneManager.LoadScene("bastard_man");
			});
	}
}
