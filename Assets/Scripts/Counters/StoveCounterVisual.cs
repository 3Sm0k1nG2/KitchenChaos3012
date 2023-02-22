using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterVisual : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;
    [SerializeField] private GameObject stoveOnVisual;
    [SerializeField] private GameObject sizzlingParticles;

    private void Start() {
        stoveCounter.OnStateChanged += StoveCounter_OnStateChanged;
    }

    private void StoveCounter_OnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e) {
        switch(e.state) {
            case StoveCounter.States.Off:
                stoveOnVisual.SetActive(false);
                sizzlingParticles.SetActive(false);
                break;
            case StoveCounter.States.On:
                stoveOnVisual.SetActive(true);
                sizzlingParticles.SetActive(false);
                break;
            case StoveCounter.States.Uncookable:
                stoveOnVisual.SetActive(true);
                sizzlingParticles.SetActive(false);
                break;
            case StoveCounter.States.Cooking:
                stoveOnVisual.SetActive(true);
                sizzlingParticles.SetActive(true);
                break;
        }
    }
}
