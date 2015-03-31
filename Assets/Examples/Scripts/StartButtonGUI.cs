using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StartButtonGUI : HideableGUI {

    private Button button;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => { 
            this.Show(false);
            SampleLockstepPeer.Instance.StartSimulation(); 
        });
    }
	
}
