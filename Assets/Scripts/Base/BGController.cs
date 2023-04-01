using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Base
{
    [RequireComponent(typeof(SpriteController))]
    public class BGController : MonoBehaviour
    {
        [SerializeField] List<Sprite> sprites = new();


        private void Start()
        {
            GetComponent<SpriteController>().Sprite_Update0(sprites[Random.Range(0, sprites.Count)]);
        }
    }
}