using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ScoreHandler : MonoBehaviour
{
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
        _notificationMoneyText = GameObject.Find("NotifactionMoney").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
        notificationPopup = GameObject.Find("NotifactionMoney").transform.GetChild(0).GetComponent<NotificationPopup>();
        _totalMoneyTransform = GameObject.Find("TotalMoney").transform;
        _totalMoneyText = _totalMoneyTransform.GetComponent<TMP_Text>();
    }

    private void Start()
    {
        TotalMoney = 1000;
    }

    public void AddMoney(float amount, float multiplier = 1)
    {
        amount *= multiplier;

        TotalMoney += amount;
        notificationMoney = amount;

        UpdateText();

        notificationPopup.Play();

        var moneyPopup = Resources.Load("Prefabs/MoneyPopup");
        moneyPopup.GetComponent<MoneyPopup>().money = amount;
        Instantiate(moneyPopup, _totalMoneyTransform); //Spawn money popup
    }

    private void UpdateText()
    {
        int totalMoneyRounded = Mathf.RoundToInt(TotalMoney);
        int notificationMoneyRounded = Mathf.RoundToInt(notificationMoney);

        _totalMoneyText.text = $"${totalMoneyRounded}";
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