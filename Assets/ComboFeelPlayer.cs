using MoreMountains.Feedbacks;
using UnityEngine;

public class ComboFeelPlayer : MonoBehaviour
{
    [SerializeField] private MMF_Player enabledPlayer;
    [SerializeField] private MMF_Player updatedPlayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void OnEnable()
    {
        enabledPlayer.PlayFeedbacks();
    }

    public void PlayUpdate()
    {
        updatedPlayer.PlayFeedbacks();
    }
}
