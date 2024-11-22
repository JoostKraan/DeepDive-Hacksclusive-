using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoginPanel : MonoBehaviour
{
    private TMP_Text usernameField;
    private TMP_Text passwordField;
    private TMP_Text loginField;
    [SerializeField] private EmailHandler EmailHandlerClass;

    private void Awake()
    {
        usernameField = transform.GetChild(0)
            .GetChild(0)
            .Find("Text")
            .GetComponent<TMP_Text>();
        passwordField = transform.GetChild(1)
            .GetChild(0)
            .Find("Text")
            .GetComponent<TMP_Text>();
        loginField = transform.GetChild(2)
            .GetComponent<TMP_Text>();
    }

    private bool coroutineActive;
    public void OnSignIn()
    {
        bool hasUsername = usernameField.text.Length > 1;
        bool correctPassword = passwordField.text.Trim((char)8203).ToLower() == "admin";

        if (!hasUsername)
        {
            if (!coroutineActive)
            {
                StartCoroutine(IncorrectField("Please provide a username!"));
            }
        }
        else if (!correctPassword)
        {
            if (!coroutineActive)
            {
                StartCoroutine(IncorrectField("Incorrect username or password!"));
            }
        }
        else
        {
            transform.parent.gameObject.SetActive(false);
            EmailHandlerClass.Username = usernameField.text;
        }
    }

    private IEnumerator IncorrectField(string text)
    {
        coroutineActive = true;

        loginField.text = text;
        yield return new WaitForSeconds(2.75f);
        loginField.text = string.Empty;

        coroutineActive = false;
    }
}
