using UnityEngine;

namespace PU.Element
{
    public class CRSAC : MonoBehaviour
    {
        [SerializeField] Animator animator;

        public void Set(bool t)
        {
            animator.SetBool("isIdle", t);
        }

        public void Conversion()
        {
            animator.SetBool("isIdle", !animator.GetBool("isIdle"));
        }
    }
}
