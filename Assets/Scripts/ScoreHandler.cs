using TMPro;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

public class ScoreHandler : MonoBehaviour
{
    public int TotalMoney;
    private TMP_Text _notificationMoneyText;
    private Transform _totalMoneyTransform;
    private TMP_Text _totalMoneyText;

    private int notificationMoney;

    private void Awake()
    {
        _notificationMoneyText = GameObject.Find("MoneyText").GetComponent<TMP_Text>();
        _totalMoneyTransform = GameObject.Find("TotalMoney").transform;
        _totalMoneyText = _totalMoneyTransform.GetComponent<TMP_Text>();
    }

    private void Start()
    {
        TotalMoney = 1000;
    }

    public void AddMoney(int amount = 1)
    {
        TotalMoney += amount;


        var moneyPopup = Resources.Load("Prefabs/MoneyPopup");
        Instantiate(moneyPopup, _totalMoneyTransform); //Spawn money popup
    }

    private void UpdateText()
    {
        _totalMoneyText.text = $"${TotalMoney}";
        _notificationMoneyText.text = notificationMoney.ToString();
    }

    private void Update()
    {
        UpdateText();
    }
}