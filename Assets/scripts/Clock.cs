using System;
using UnityEngine;
using TMPro;

public class Clock : MonoBehaviour {

	const float
		degreesPerHour = 30f,
		degreesPerMinute = 6f,
		degreesPerSecond = 6f;

	public Transform hoursTransform, minutesTransform, secondsTransform;
	public bool continuous;
	public Transform player;
	public float facingThreshold = 0.7f;
	public float interactionDistance = 3f;
	public TMP_Text hintText;

	bool isEnd = false;

    void Start()
    {
		hintText.enabled = false;
    }
    void Update () {
		if (FindObjectOfType<EggPlacementDetector>().isPlaced())
		{
			float distance = Vector3.Distance(player.position, transform.position);
			Vector3 toClock = (transform.position - player.position).normalized;
			Vector3 playerForward = player.forward;

			bool isFacing = Vector3.Dot(playerForward, toClock) > facingThreshold;

			if (distance <= interactionDistance && isFacing)
			{
				if (hintText != null && !isEnd)
				{
					hintText.text = "Press 'F' to touch time";
					hintText.enabled = true;
				}
				if (Input.GetKeyDown(KeyCode.F))
				{
					isEnd = true;
					hintText.enabled = false;
					FindObjectOfType<ScreenFade>().StartWhiteFade();
				}
			}
			else
			{
				if (hintText != null)
					hintText.enabled = false;
			}
			return;
		}
		else if (continuous)
		{
			UpdateContinuous();
		}
		else
		{
			UpdateDiscrete();
		}
	}

	void UpdateContinuous () {
		TimeSpan time = DateTime.Now.TimeOfDay;
		hoursTransform.localRotation =
			Quaternion.Euler(0f, (float)time.TotalHours * degreesPerHour, 0f);
		minutesTransform.localRotation =
			Quaternion.Euler(0f, (float)time.TotalMinutes * degreesPerMinute, 0f);
		secondsTransform.localRotation =
			Quaternion.Euler(0f, (float)time.TotalSeconds * degreesPerSecond, 0f);
	}

	void UpdateDiscrete () {
		DateTime time = DateTime.Now;
		hoursTransform.localRotation =
			Quaternion.Euler(0f, time.Hour * degreesPerHour, 0f);
		minutesTransform.localRotation =
			Quaternion.Euler(0f, time.Minute * degreesPerMinute, 0f);
		secondsTransform.localRotation =
			Quaternion.Euler(0f, time.Second * degreesPerSecond, 0f);
	}
}