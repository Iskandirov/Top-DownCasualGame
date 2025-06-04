using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpineAnimationController : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;
    public string animationName = "attack";
    public float interval = 10f;

    private float timer = 0f;
    private bool isPlaying = false;

    void Start()
    {
        skeletonAnimation.AnimationState.Event += HandleEvent;
        skeletonAnimation.AnimationState.Complete += OnAnimationComplete;
        skeletonAnimation.timeScale = 0f;
    }

    void Update()
    {
        if (isPlaying) return;

        timer += Time.deltaTime;

        if (timer >= interval)
        {
            PlayAnimationOnce();
            timer = 0f;
        }
    }

    void PlayAnimationOnce()
    {
        isPlaying = true;

        skeletonAnimation.AnimationState.SetAnimation(0, animationName, false);
        skeletonAnimation.timeScale = 1f; // Увімкнути відтворення
    }

    void OnAnimationComplete(Spine.TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == animationName)
        {
            skeletonAnimation.timeScale = 0f;
            isPlaying = false;
        }
    }

    void HandleEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        // Можна обробляти івенти зі Spine, якщо треба
    }
}
