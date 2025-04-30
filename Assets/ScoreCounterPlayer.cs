using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class ScoreCounterPlayer : MonoBehaviour
{
    [SerializeField] private MMF_Player player;
    private MMF_TMPCountTo counter;
    private Queue<float> scoresToAdd = new Queue<float>();
    private float counterDuration;
    private float currentScore = 0f;
    private bool isProcessing = false;

    private void Start()
    {
        counter = player.GetFeedbackOfType<MMF_TMPCountTo>();
        counterDuration = player.TotalDuration;
    }

    public void AddScore(float score)
    {
        if (score != 0) scoresToAdd.Enqueue(score);
        if (!isProcessing)
        {
            StartCoroutine(ProcessQueue());
        }
    }

    private IEnumerator ProcessQueue()
    {
        isProcessing = true;

        while (scoresToAdd.Count > 0)
        {
            float scoreToAdd = scoresToAdd.Dequeue();
            float newScore = currentScore + scoreToAdd;

            counter.CountFrom = currentScore;
            counter.CountTo = newScore;

            player.PlayFeedbacks();
            Debug.Log("Playing now");

            yield return new WaitForSeconds(counterDuration);

            currentScore = newScore;
        }

        isProcessing = false;
    }
}