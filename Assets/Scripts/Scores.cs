using UnityEngine;
using TMPro;
using System;

public class Scores : MonoBehaviour
{
	private TextMeshProUGUI text;
	private Controls controls;

	public GameObject player;

	void Start()
	{
		text = GetComponent<TextMeshProUGUI>();
		controls = player.GetComponent<Controls>();
	}

	void Update()
	{
		if(controls != null)
		{
			text.text = $"Highscore: {Math.Floor(controls.highScore)}\nScores: {Math.Floor(controls.scores)}";
		}
	}
}

