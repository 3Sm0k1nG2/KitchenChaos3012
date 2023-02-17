using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterVisual : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;

    private void Start() {
        stoveCounter.OnTurnOnOff += StoveCounter_OnTurnOnOff;
        stoveCounter.OnStartCooking += StoveCounter_OnStartCooking;
        stoveCounter.OnStopCooking += StoveCounter_OnStopCooking;
    }

    private void StoveCounter_OnTurnOnOff(object sender, StoveCounter.OnTurnOnOffEventArgs e) {
        Debug.Log(name + " is turned " + (e.isOn ? "on" : "off") + ".");
        stoveCounter.stoveOnVisual.SetActive(e.isOn);
    }

    private void StoveCounter_OnStartCooking(object sender, System.EventArgs e) {
        Debug.Log(name + " has started cooking.");
        stoveCounter.sizzlingParticles.SetActive(true);
    }

    private void StoveCounter_OnStopCooking(object sender, System.EventArgs e) {
        Debug.Log(name + " has stoped cooking.");
        stoveCounter.sizzlingParticles.SetActive(false);
    }
}
