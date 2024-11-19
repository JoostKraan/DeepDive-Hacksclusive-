using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreHandler : MonoBehaviour
{
    public int TotalMoney;
    private TMP_Text _moneyText;

private void Awake()
    {
        _moneyText = GameObject.Find("MoneyText").GetComponent<TMP_Text>();
    }
    public void AddMoney(int amount = 1)
    {
        TotalMoney += amount;

        _moneyText.text = TotalMoney.ToString();
    }
}