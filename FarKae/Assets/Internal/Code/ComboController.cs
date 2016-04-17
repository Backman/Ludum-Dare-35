using UnityEngine;
using System.Collections;

public class ComboController : MonoBehaviour
{
	public static ComboController instance;

	public float comboDuration;

	void Awake()
	{
		instance = this;
	}

	/// <summary>
	/// Maybe add counter shiet
	/// </summary>
	public void Combo()
	{

	}
}
