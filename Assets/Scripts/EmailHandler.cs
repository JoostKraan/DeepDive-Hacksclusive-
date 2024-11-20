using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EmailHandler : MonoBehaviour {
    [System.Serializable]
    public class Email {
        public string email;
        public string subject;
    }

    [System.Serializable]
    public class File {
        public string fileNaam;
        public string version;
        public int vertrouwlijkheid;
        public int documentType;
    }

    [System.Serializable]
    public class Root {
        public List<Email> emails;
        public List<File> files;
        public List<string> namen;
        public List<string> company;
        public List<string> NonCompany;
    }

    [Header("Prefabs")] 
    [SerializeField] private GameObject MailButton;

    [Header("Paths")]
    [SerializeField] private Transform MailButtonParentTransform;
    [SerializeField] private GameObject MailNotification;
 
    [Header("Data")]
    [SerializeField] private AudioSource MailReceivedAudioSource;
    [SerializeField] private Animator MailReceivedAnimation;
    private Dictionary<int, EmailStruct> ActiveEmails = new Dictionary<int, EmailStruct>();
    public TextAsset jsonFile;
    
    
    // -- Privates
    private Root Data;
    private int EmailInterval = 5;
    private int SelectedEmailIndex = -1;


    void Start() {
        Data = JsonUtility.FromJson<Root>(jsonFile.text);
        StartCoroutine(EmailLoop());
    }

    private IEnumerator EmailLoop() {
        while (true) {
            SendEmail(); // Call your function here
            EmailInterval = Random.Range(5, 15);
            yield return new WaitForSeconds(EmailInterval);
        }
    }

    public void SelectEmail(int Index) {
        SelectedEmailIndex = Index;
    }

    public void TrashEmail() {
        if (SelectedEmailIndex == -1) return;

        EmailStruct EmailData = ActiveEmails[SelectedEmailIndex];
        Destroy(EmailData.Prefab);
        ActiveEmails.Remove(SelectedEmailIndex);

        gameObject.transform.Find("Inside").Find("EmailText").GetComponent<TMP_Text>().text = "no email";
        gameObject.transform.Find("Inside").Find("FileText").GetComponent<TMP_Text>().text = "No File";

        SelectedEmailIndex = -1;
    }

    public void DownloadEmailContents() {
        if (SelectedEmailIndex == -1) return;
        EmailStruct EmailData = ActiveEmails[SelectedEmailIndex];

        if (EmailData.FileType.vertrouwlijkheid <= 0) {
            EmailData.Prefab.GetComponent<Image>().color = Color.red;
        } else {
            EmailData.Prefab.GetComponent<Image>().color = Color.green;
        }
    }

    private void SendEmail() {
        if (ActiveEmails.Count >= 5) return;
        GameObject NewMailButton = Instantiate(MailButton);

        string Sender = Data.namen[Random.Range(0, Data.namen.Count)];
        string Company = Data.company[Random.Range(0, Data.company.Count)];

        NewMailButton.transform.Find("Company").GetComponent<TMP_Text>().text = Company;
        NewMailButton.transform.Find("Zender").GetComponent<TMP_Text>().text = Sender;

        Email RandomEmail = Data.emails[Random.Range(0, Data.emails.Count)];
        File RandomFileType = Data.files[Random.Range(0, Data.files.Count)];
        NewMailButton.transform.Find("Onderwerp").GetComponent<TMP_Text>().text = string.Format("Subject: {0}", RandomEmail.subject);

        int NewIndex = Random.Range(0, 999999);
        ActiveEmails.Add(NewIndex, 
            new EmailStruct(Sender, Company, true, RandomEmail, RandomFileType, NewMailButton));

        MailButton ButtonComponent = NewMailButton.GetComponent<MailButton>();
        ButtonComponent.EmailData = ActiveEmails[NewIndex];
        ButtonComponent.ComputerScreenGameObject = gameObject.transform.Find("Inside").gameObject;
        ButtonComponent.EmailHandler = gameObject.GetComponentInParent<EmailHandler>();
        ButtonComponent.Index = NewIndex;

        MailNotification.SetActive(true);
        MailReceivedAudioSource.Play();
        MailReceivedAnimation.Play("PopUp");

        NewMailButton.transform.SetParent(MailButtonParentTransform);
    }
}
