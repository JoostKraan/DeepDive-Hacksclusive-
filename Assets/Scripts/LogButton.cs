using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogButton : MonoBehaviour {
    [SerializeField] private Image SpriteRenderer;
    public HackingHandler HackingHandler;
    public bool IsValid = false;

    public void PressedFunc() {
        if (HackingHandler.Attempts > HackingHandler.HackingAttempts) return;
        if (HackingHandler.GuessedRightHack) return;

        HackingHandler.Attempts++;

        if (IsValid == true) {
            SpriteRenderer.color = Color.green;
            

            StartCoroutine(HackingHandler.FinishedPasswordLogging(true));
        } else {
            SpriteRenderer.color = Color.red;
           
            if (HackingHandler.Attempts >= HackingHandler.HackingAttempts) {
                StartCoroutine(HackingHandler.FinishedPasswordLogging(false));
            }
        }
    }
}
