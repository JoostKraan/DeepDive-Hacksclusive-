using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ScoreHandler : MonoBehaviour
{
    public string prefix = "B$ ";
    public bool click;
    public float moneyToAdd;
    [Space(10)]

    public float TotalMoney;
    private TMP_Text _notificationMoneyText;
    private Transform _totalMoneyTransform;
    private TMP_Text _totalMoneyText;

    private NotificationPopup notificationPopup;

    private float notificationMoney;

    private void Awake()
    {
        _notificationMoneyText = GameObject.Find("NotificationMoney").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
        notificationPopup = GameObject.Find("NotificationMoney").transform.GetChild(0).GetComponent<NotificationPopup>();
        _totalMoneyTransform = GameObject.Find("TotalMoney").transform;
        _totalMoneyText = _totalMoneyTransform.GetComponent<TMP_Text>();
    }

    private void Start()
    {
        TotalMoney = 0.0012f;
    }

    public void AddMoney(float amount, float multiplier = 1)
    {
        amount *= multiplier;

        TotalMoney += amount;
        notificationMoney = amount;

        UpdateText();

        if (amount > 0)
        {
            notificationPopup.Play();
        }
        var moneyPopup = Resources.Load("Prefabs/MoneyPopup");
        var moneyPopupScript = moneyPopup.GetComponent<MoneyPopup>();
        moneyPopupScript.money = amount;
        moneyPopupScript.prefix = prefix;
        Instantiate(moneyPopup, _totalMoneyTransform); //Spawn money popup
    }

    private void UpdateText()
    {
        int decimalPoint = 10000;
        float totalMoneyRounded = Mathf.Round(TotalMoney * decimalPoint) / decimalPoint;
        float notificationMoneyRounded = Mathf.Round(notificationMoney * decimalPoint) / decimalPoint;

        _totalMoneyText.text = $"{prefix}{totalMoneyRounded}";
        _notificationMoneyText.text = $"{notificationMoneyRounded}";
    }

    private void Update()
    {
        UpdateText();

        if (click)
        {
            AddMoney(moneyToAdd);
            click = false;
        }
    }
}