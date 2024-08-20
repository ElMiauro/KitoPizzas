using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneDialScript : MonoBehaviour
{
    public AudioClip[] phoneTones;
	AudioSource AS;
	public KeyCode[] keys = new KeyCode[] { KeyCode.Keypad0, KeyCode.Keypad1, KeyCode.Keypad2, KeyCode.Keypad3, KeyCode.Keypad4, KeyCode.Keypad5, KeyCode.Keypad6, KeyCode.Keypad7, KeyCode.Keypad8, KeyCode.Keypad9 };
	private void Start()
	{
		AS = GetComponent<AudioSource>();
	}
	void Update()
    {
		for (int i = 0; i < 10; i++)
		{
			if (Input.GetKeyDown(keys[i])){
				PlayTone(i);
			}
		}   
    }

	void PlayTone(int idx)
	{
		if (AS.isPlaying) AS.Stop();
		AS.PlayOneShot(phoneTones[idx]);
	}
}
