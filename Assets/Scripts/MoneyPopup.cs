using TMPro;
using UnityEngine;

public class MoneyPopup : MonoBehaviour
{
    private Animator _animator;
    private TMP_Text _moneyText;

    public float money;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _moneyText = GetComponent<TMP_Text>();
        PlayPopup(money);
    }
    private void Start()
    {
        Destroy(gameObject, 3); //Destroy self after 3 seconds
    }
    public void PlayPopup(float money)
    {
        money = Mathf.RoundToInt(money);

        bool isNegative = money < 0;
        if (isNegative)
        {
            _moneyText.text = $"-${money}";
            _moneyText.color = Color.red;
        }
        else
        {
            _moneyText.text = $"+${money}";
        }

        _animator.Play("PopUpFade");
    }
}
