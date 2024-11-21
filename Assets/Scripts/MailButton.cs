using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class MailButton : MonoBehaviour {
    public EmailStruct EmailData;
    public GameObject ComputerScreenGameObject;
    public EmailHandler EmailHandler;
    public int Index;

    public void OpenMail() {
        if (EmailData == null) return;

        ComputerScreenGameObject.transform.Find("EmailText").GetComponent<TMP_Text>().text = string.Format(EmailData.EmailData.email, EmailData.Sender, "Tempy");
        ComputerScreenGameObject.transform.Find("FileText").GetComponent<TMP_Text>().text = EmailData.FileType.fileNaam;

        EmailHandler.SelectEmail(Index);
    }
}
