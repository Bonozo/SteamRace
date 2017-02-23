using UnityEngine;

public class CharacterLeanBehaviour : MonoBehaviour
{
    private Animator animator;
    private float leanAngleNormalised;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (animator != null)
        {
            float leanInput = Input.GetAxis("Lean");

            // lerp the lean angle to the input which ranges from -1.0 to 1.0
            leanAngleNormalised = Mathf.Lerp(leanAngleNormalised, leanInput, Time.deltaTime * 5.0f);

            if (Mathf.Approximately(leanInput, 0.0f) && Mathf.Abs(leanAngleNormalised) < 0.01f)
            {
                // play idle animation
                animator.Play("Idle");
            }
            else
            {
                if (leanAngleNormalised < 0.0f)
                {
                    // play lean left animation at specific frame
                    animator.Play("Lean Left", -1, Mathf.Abs(leanAngleNormalised));
                }
                else if (leanAngleNormalised > 0.0f)
                {
                    // play lean right animation at specific frame
                    animator.Play("Lean Right", -1, Mathf.Abs(leanAngleNormalised));
                }
            }
        }
    }
}