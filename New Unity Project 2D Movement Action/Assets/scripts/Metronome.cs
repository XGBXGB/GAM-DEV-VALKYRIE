using UnityEngine;
using System.Collections;

public class Metronome : MonoBehaviour
{
	public double bpm;
	double nextTick = 0.0F; // The next tick in dspTime
	bool ticked = false;
	float ctr = 0.0f;
	float interval = 0.0f;

	void Start() {
		double startTick = AudioSettings.dspTime;
		nextTick = startTick + (60.0 / bpm);
	}

	void Update() {
		ctr+=Time.deltaTime;
		double timePerTick = 60.0f / bpm;
		double dspTime = AudioSettings.dspTime;

		while ( dspTime >= nextTick ) {
			ticked = false;
			nextTick += timePerTick;
		}
		if (Input.GetKeyDown ("space")) {
			if (ctr >= interval*0.5)
				Debug.Log ("Perfect! " + ctr);
			else if (ctr >= interval*0.25)
				Debug.Log ("Good " + ctr);
			else
				Debug.Log ("Miss...");
		}
	}

	void LateUpdate() {
		if ( !ticked && nextTick >= AudioSettings.dspTime ) {
			if (interval == 0.0f)
				interval = ctr;
			else
				interval = (interval + ctr) / 2;
			ctr = 0;
			ticked = true;
		}
	}
}