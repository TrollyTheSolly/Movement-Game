using MoreMountains.Feedbacks;
using UnityEngine;

public class FeedbackManager : MonoBehaviour
{
    [SerializeField] private MMF_Player jumpFeedback;
    [SerializeField] private MMF_Player landingFeedback;
    public void PlayJumpFeedback()
    {
        jumpFeedback.PlayFeedbacks();
    }

    public void PlayLandingFeedback()
    {
        landingFeedback.PlayFeedbacks();
    }
}
