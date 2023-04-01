using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Item.UI.Plot
{
    [RequireComponent(typeof(Animator))]
    public class CharaterAnim : MonoBehaviour
    {
        public Storyteller storyteller;
        public List<string> names = new() { "left", "right", "end" };
        private Animator animator_;
        public Animator animator
        {
            get { if (animator_ == null) animator_ = GetComponent<Animator>(); return animator_; }
        }

        public void Ended()
        {
            foreach (var it in names)
            {
                animator.SetBool(it, false);
            }
            animator.SetBool("end", true);
            StartCoroutine(Start_(Time.time));
        }

        private IEnumerator Start_(float last)
        {
            while(Time.time-0.15f<=last)yield return null;
            animator.SetBool("end", false);
        }

        public void AddThis(string name)
        {
            storyteller.Charaters.TryAdd(name, this);
        }
    }
}