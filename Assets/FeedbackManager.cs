using MoreMountains.Feedbacks;
using UnityEngine;

public class FeedbackManager : MonoBehaviour
{
    [SerializeField] private MMF_Player jumpFeedback;
    [SerializeField] private MMF_Player landingFeedback;
    public void PlayJumpFeedback()
    {
        Debug.Log("Playing jump feedback...");
        jumpFeedback.PlayFeedbacks();
    }

    public void PlayLandingFeedback()
    {
        Debug.Log("Playing landing feedback...");
        landingFeedback.PlayFeedbacks();
    }
}
