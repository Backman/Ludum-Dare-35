using UnityEngine;
using System.Collections;

public class Stats : MonoBehaviour
{
	public static int narwhalKills { get; set; }
	public static int henchmanKills { get; set; }
	public static int rainbowCoinsPickedUp { get; set; }
	public static int enemiesSpawned { get; set; }
	public static int highestCombo { get; set; }
	public static int wavesCleared { get; set; }

	public static float secondsRainbow { get; set; }
	public static float damageDone { get; set; }
	public static float unitsMoved { get; set; }

	public static void Save()
	{
		PlayerPrefs.SetInt("NARWHAL_KILLS", narwhalKills);
		PlayerPrefs.SetInt("HENCHMAN_KILLS", henchmanKills);
		PlayerPrefs.SetInt("RAINBOW_COINS_PICKED_UP", rainbowCoinsPickedUp);
		PlayerPrefs.SetInt("WAVES_CLEARED", wavesCleared);
		PlayerPrefs.SetInt("ENEMIES_SPAWNED", enemiesSpawned);
		PlayerPrefs.SetInt("HIGHEST_COMBO", enemiesSpawned);
		PlayerPrefs.SetFloat("SECONDS_RAINBOW", damageDone);
		PlayerPrefs.SetFloat("DAMAGE_DONE", damageDone);
		PlayerPrefs.SetFloat("UNITS_MOVED", unitsMoved);
		PlayerPrefs.Save();
	}

	public static void Reset()
	{
		narwhalKills = 0;
		henchmanKills = 0;
		rainbowCoinsPickedUp = 0;
		enemiesSpawned = 0;
		wavesCleared = 0;
		secondsRainbow = 0f;
		damageDone = 0f;
		unitsMoved = 0f;
	}
}
