using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;

public class HackingHandler : MonoBehaviour {
    [System.Serializable] private class CorrectTemplatesClass {
        [SerializeField] public string Username;
        [SerializeField] public string Password;
    }

    [Header("Classes")] 
    [SerializeField] private Browser BrowserClass;
    [SerializeField] private ScoreHandler ScoreHandlerClass;
    [SerializeField] private WifiManager WifiManagerClass;

    [Header("Transforms")]
    [SerializeField] private Transform LogsTransform;

    [Header("Prefabs")] 
    [SerializeField] private GameObject LoginDetailsPrefab;
    [SerializeField] private GameObject LogTextPrefab;

    [Header("Variables")] 
    [SerializeField] private bool IsProcessing;
    [SerializeField] private List<GameObject> Logs = new List<GameObject>();

    [SerializeField] public int HackingAttempts = 3;
    [SerializeField] public int Attempts = 0;

    [Header("Presets")] 
    [SerializeField] private List<string> Names = new List<string>();
    [SerializeField] private List<string> Passwords = new List<string>();
    [SerializeField] private List<CorrectTemplatesClass> CorrectTemplates = new List<CorrectTemplatesClass>();

    public bool GuessedRightHack;

    public void PasswordGrabber() {
        if (IsProcessing) return;
        if (WifiManagerClass.isConnected == false) {
            AddTextLog("Try connecting to the internet first!");
            return;
        }

        if (BrowserClass.IsOnValidTab) {
            LogHackingPasswords();
        } else {
            AddTextLog("Go to a website to start password grabbing!");
        }
    }

    public int AceededMaxAttempts() {
        Attempts++;
        return Attempts;
    }

    private void AddTextLog(string Message) {
        if (Logs.Count >= 8) return;
        GameObject LogObject = Instantiate(LogTextPrefab, LogsTransform);
        StartCoroutine(ArchiveLog(LogObject));

        LogObject.transform.Find("TextField").gameObject.GetComponent<TMP_Text>().text = string.Format("LOG: {0}", Message);
        Logs.Add(LogObject);
    }

    private void LogHackingPasswords() {
        ClearLogs();

        int RandomRightAnswer = Random.Range(0, 8);
        IsProcessing = true;

        for (int i = 0; i < 8; i++) {
            GameObject LogObject = Instantiate(LoginDetailsPrefab, LogsTransform);
            Logs.Add(LogObject);
            
            if (RandomRightAnswer == i) {
                CorrectTemplatesClass RealInfo = CorrectTemplates[Random.Range(0, CorrectTemplates.Count)];
                LogObject.transform.Find("Username").gameObject.GetComponent<TMP_Text>().text =
                    string.Format("Username: {0}", RealInfo.Username);
                LogObject.transform.Find("Password").gameObject.GetComponent<TMP_Text>().text =
                    string.Format("Password: {0}", RealInfo.Password);
                LogObject.GetComponent<LogButton>().IsValid = true;
                LogObject.GetComponent<LogButton>().HackingHandler = gameObject.GetComponent<HackingHandler>();
                LogObject.name = RealInfo.Username;
            } else {
                string Username = Names[Random.Range(0, Names.Count)];
                string Password = Passwords[Random.Range(0, Passwords.Count)];
                LogObject.transform.Find("Username").gameObject.GetComponent<TMP_Text>().text =
                    string.Format("Username: {0}", Username);
                LogObject.transform.Find("Password").gameObject.GetComponent<TMP_Text>().text =
                    string.Format("Password: {0}", Password);
                LogObject.GetComponent<LogButton>().IsValid = false;
                LogObject.GetComponent<LogButton>().HackingHandler = gameObject.GetComponent<HackingHandler>();
                LogObject.name = Username;
            }
        }
    }

    private IEnumerator ArchiveLog(GameObject LogObject) {
        yield return new WaitForSeconds(5);
        Logs.Remove(LogObject);
        Destroy(LogObject);
    }

    public IEnumerator FinishedPasswordLogging(bool GuessedRight) {
        GuessedRightHack = true;
        yield return new WaitForSeconds(1.5f);
        ClearLogs();

        if (GuessedRight) {
            ScoreHandlerClass.AddMoney(0.04f);
        } else {
            ScoreHandlerClass.AddMoney(-0.0012f);
        }

        IsProcessing = false;
        GuessedRightHack = false;
        Attempts = 0;
    }

    private void ClearLogs() {
        foreach (GameObject LogObject in Logs) {
            Destroy(LogObject);
        }

        Logs.Clear();
    }
}
