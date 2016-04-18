using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using MonsterLove.StateMachine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
	enum State
	{
		Start,
		Score,
		Exit
	}

	public Image fadeImage;
	public float fadeInDuration;
	public float fadeOutDuration;

	public Text narwhals;
	public Text henchmens;
	public Text damageDealt;
	public Text collectedCoins;
	public Text wavesPwned;
	public Text secondsRaving;

	StateMachine<State> _fsm;
	bool _canClick;

	void Awake()
	{
		_fsm = StateMachine<State>.Initialize(this, State.Start);

		narwhals.text = Stats.narwhalKills.ToString();
		henchmens.text = Stats.henchmanKills.ToString();
		damageDealt.text = Stats.damageDone.ToString("0.0");
		collectedCoins.text = Stats.rainbowCoinsPickedUp.ToString();
		wavesPwned.text = Stats.wavesCleared.ToString();
		secondsRaving.text = Mathf.FloorToInt(Stats.secondsRainbow).ToString();
	}

	IEnumerator Start()
	{
		fadeImage.gameObject.SetActive(true);
		yield return new WaitForSeconds(0.5f);
		GetComponent<AudioSource>().Play();
	}

	void Start_Enter()
	{
		fadeImage.DOFade(0f, fadeInDuration)
			.OnComplete(() =>
			{
				_canClick = true;
			});

	}

	void Score_Enter()
	{
		if (_canClick && Input.anyKey)
		{
			_fsm.ChangeState(State.Exit);
		}
	}

	void Exit_Enter()
	{
		fadeImage.DOFade(1f, fadeOutDuration)
			.OnComplete(() =>
			{
				SceneManager.LoadScene(0);
			});
	}
}
