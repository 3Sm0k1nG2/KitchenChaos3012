using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private ProgressBarUIParentCounter progressBarUIParent;
    [SerializeField] private Image barImage;

    private void Awake() {
        barImage.fillAmount = 0f;
    }

    private void Start() {
        progressBarUIParent.OnProgressChanged += AlternateActionCounter_OnProgressChanged;
        Hide();
    }

    private void AlternateActionCounter_OnProgressChanged(object sender, IProgressBarUIParent.OnProgressChangedEventArgs e) {
        barImage.fillAmount = e.progressNormalized;
        Show();

        if(e.progressNormalized == 0f || e.progressNormalized == 1f) {
            Hide();
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}
