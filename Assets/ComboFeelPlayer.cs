using MoreMountains.Feedbacks;
using UnityEngine;

public class ComboFeelPlayer : MonoBehaviour
{
    [SerializeField] private MMF_Player enabledPlayer;
    [SerializeField] private MMF_Player updatedPlayer;
    [SerializeField] public MMF_Player disabledPlayer;
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

    public void PlayDisable()
    {
        if (disabledPlayer) disabledPlayer.PlayFeedbacks();
    }
    
    public float GetDisableAnimationDuration()
    {
        if (disabledPlayer != null)
        {
            return disabledPlayer.TotalDuration;
        }
        return 0f;
    }
}
