using TMPro;
using UnityEngine;

public class MoneyPopup : MonoBehaviour
{
    private Animator _animator;
    private TMP_Text _moneyText;

    public float money;
    public string prefix;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _moneyText = GetComponent<TMP_Text>();
        PlayPopup(money);
    }
    private void Start()
    {
        Destroy(gameObject, 6); //Destroy self after 6 seconds
    }
    public void PlayPopup(float money)
    {
        //money = Mathf.RoundToInt(money);

        bool isNegative = money < 0;
        if (isNegative)
        {
            _moneyText.text = $"-{prefix}{money}";
            _moneyText.color = Color.red;
        }
        else
        {
            _moneyText.text = $"+{prefix}{money}";
        }

        _animator.Play("PopUpFade");
    }
}
