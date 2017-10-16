using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CanvasButtonController : MonoBehaviour {
	Transform child = null;

	// Use this for initialization
	void Start () {
		int nChildCOunt = transform.childCount;

		for (int i = 0; i < nChildCOunt; i++) {
			child = transform.GetChild (i);

            if (child.name == "Button_StartEffect")
                child.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnButtonStartEffect);
		}
	}

    void OnButtonStartEffect () {
        ForkParticlePlugin.Instance.Test();
	}
}
