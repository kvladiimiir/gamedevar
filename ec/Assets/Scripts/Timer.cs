﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
	bool m_paused;
	float m_seconds;
	public Text m_text;

	public void Reset()
	{
		m_paused = true;
		m_seconds = 0f;
		UpdateTimerColor();
	}

	public void SetPaused(bool paused)
	{
		m_paused = paused;
	}

	public float GetTime()
	{
		return m_seconds;
	}

	void Start()
	{
		Reset();
	}

	void Update()
	{
		if (m_paused)
		{
			return;
		}
	}

	public void SetTime(float time)
	{
		if (time <= 0)
		{
			time = 0;
		}

		m_seconds = time;
		m_text.text = string.Format("{0:D2}:{1:D2}", (int)(m_seconds / 60), (int)(m_seconds % 60));
		UpdateTimerColor();
	}

	void UpdateTimerColor()
	{
		if (m_seconds <= 60.0f)
		{
			m_text.color = Color.white;
		}
		else if (m_seconds <= 90.0f)
		{
			m_text.color = Color.yellow;
		}
		else
		{
			m_text.color = Color.red;
		}
	}
}
