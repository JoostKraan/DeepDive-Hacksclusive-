using UnityEngine;

public class NotificationPopup : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void Play()
    {
        animator.Play("PopUp");
    }
}
