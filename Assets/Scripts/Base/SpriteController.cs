using System;
using UnityEngine;
using UnityEngine.UI;

namespace Base
{
    [Serializable]
    public class SpriteCInException : BaseInException
    {
        public SpriteCInException() { }
        public SpriteCInException(string message) : base(message) { }
        public SpriteCInException(string message, Exception inner) : base(message, inner) { }
        public SpriteCInException(MonoBehaviour mono) : base(mono.name) { }
        protected SpriteCInException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class SpriteCOutException : BaseOutException
    {
        public SpriteCOutException() { }
        public SpriteCOutException(string message) : base(message) { }
        public SpriteCOutException(string message, Exception inner) : base(message, inner) { }
        public SpriteCOutException(SpriteCInException baseIn) : base(baseIn.Message) { }
        protected SpriteCOutException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    public class SpriteController : UnderlyingObject
    {
        private bool isImage = true;
        [HideInInspector] public Image Icat;
        [HideInInspector] public SpriteRenderer SRcat;

        public Sprite First;
        public Sprite Second;
        public Sprite GetNow() => (isImage) ? Icat.sprite : SRcat.sprite;
        public Color GetNowColor() => (isImage) ? Icat.color : SRcat.color;

        private bool isEnd;
        private readonly bool isRandomVariations = false;

        private void Awake()
        {
            TryInitComponent();
            try
            {
                if (Icat == null && SRcat == null)
                    throw new SpriteCInException("private void Awake(),no refered component");
                else if (Second == null)
                    Second = GetNow();
            }
            catch (SpriteCInException ex)
            {
                Debug.LogWarning(ex.Message);
                AddInitComponent();
            }
            catch (Exception ex)
            {
                throw new SpriteCOutException("private void Awake(),unknow error", ex);
            }
        }

        public override void update()
        {
            if (isImage) Is_Image_Update0(); 
            else Is_SpriteRender_Update0(); 
        }

        private void AddInitComponent() 
        {
            if (gameObject.transform.parent != null)
            {
                Icat = gameObject.AddComponent<Image>();
                isImage = true;
            }
            else
            {
                SRcat = GetComponent<SpriteRenderer>();
                isImage = false;
            }
        }

        private void TryInitComponent()
        {
            if (TryGetComponent(out Image tocat))
            {
                Icat = tocat;
                isImage = true;
            }
            else
            {
                SRcat = GetComponent<SpriteRenderer>();
                isImage = false;
            }
        }

        private void Is_SpriteRender_Update0()
        {
            if (isRandomVariations)
            {
                var cat = SRcat.color;

                static float CC(float f)
                {
                    return Mathf.Clamp(f + UnityEngine.Random.Range(-1.0f, 1.0f) * 0.0003f, 0, 1);
                }

                SetColor(CC(cat.r), CC(cat.g), CC(cat.b));
            }
        }

        private void Is_Image_Update0()
        {
            if (isRandomVariations)
            {
                var cat = Icat.color;

                static float CC(float f)
                {
                    return Mathf.Clamp(f + UnityEngine.Random.Range(-1.0f, 1.0f) * 0.0003f, 0, 1);
                }

                SetColor(CC(cat.r), CC(cat.g), CC(cat.b));
            }
        }

        public void LoadFromResources(string name)
        {
            Sprite tmp = Resources.Load<Sprite>(name);
            try
            {
                if (tmp == null) throw new SpriteCInException("public void LoadFromResources(string name),Resources.Load<Sprite>(name),name'file cann't found");
                else
                {
                    First = tmp;
                    UpdateSprite();
                }
            }
            catch(SpriteCInException ex)
            {
                Debug.LogWarning(ex.Message);
            }
            catch (Exception ex)
            {
                throw new SpriteCOutException("public void LoadFromResources(string name),unknow error", ex);
            }
        }

        public void SetSize(Vector3 size)
        {
            if (isImage) Icat.transform.localScale = size;
            else SRcat.transform.localScale = size;
        }

        public void SetSize(float x, float y)
        {
            SetSize(new Vector3(x, y, 1));
        }

        public void SetAColor(float a)
        {
            SetColor(1, 1, 1, a);
        }

        public void SetColor(float r = 1, float g = 1, float b = 1, float a = 1) => SetColor(new(r, g, b, a));

        public void SetColor(Color color)
        {
            if (isImage) Icat.color = color;
            else SRcat.color = color;
        }

        public void SetRGBColor(float rgb=1)
        {
            SetColor(rgb, rgb, rgb, 1);
        }

        public void ConversionFromTwo()
        {
            if (GetNow() == null || GetNow() != First) Sprite_Update0(First);
            else Sprite_Update0(Second);
        }

        public void UpdateSprite()
        {
            if (isImage)
            {
                Second = Icat.sprite;
                Icat.sprite = First;
            }
            else
            {
                Second = SRcat.sprite;
                SRcat.sprite = First;
            }
        }

        /// <summary>
        /// Direct update
        /// </summary>
        /// <param name="sprite"></param>
        public void Sprite_Update0(Sprite sprite)
        {
            if (isImage) Icat.sprite = sprite;
            else  SRcat.sprite = sprite;
        }

        /// <summary>
        /// Gradients are made before updating
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="time"></param>
        public void Sprite_Update1(Sprite sprite, int time = 50)
        {
            First = sprite;
            isEnd = false;
            AddF(__Coroutine_Sprite_Update1, new Carrier { Rank = time, Value = time, state = State.Active });
        }

        /// <summary>
        /// Gradient before update (A only)
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="time"></param>
        public void JustA_SpriteUpdate1(Sprite sprite, int time = 50)
        {
            First = sprite;
            isEnd = false;
            AddF(__CoroutineJustA_Sprite_Update1, new Carrier { Rank = time, Value = time, state = State.Active });
        }

        private void __Coroutine_Sprite_Update1(Carrier carrier)
        {
            var v = carrier.Value / 2.0f;
            if (carrier.Rank-- >= v)
            {
                var cat = EasingFunction.Curve(new Vector3(0, 0, 0), new Vector3(1, 1, 1), (carrier.Rank - v) / v);
                SetColor(cat.x, cat.y, cat.z);
            }
            else
            {
                var cat = EasingFunction.Curve(new Vector3(1, 1, 1), new Vector3(0, 0, 0), carrier.Rank / v);
                SetColor(cat.x, cat.y, cat.z);
                if (!isEnd)
                {
                    UpdateSprite();
                    isEnd = true;
                }
            }

            if (carrier.Rank <= 0)
                carrier.state = State.Destroy;
        }

        private void __CoroutineJustA_Sprite_Update1(Carrier carrier)
        {
            var v = carrier.Value / 2.0f;
            if (carrier.Rank-- >= v)
            {
                var cat = EasingFunction.Curve(0, 1, (carrier.Rank - v) / v);
                SetAColor(cat);
            }
            else
            {
                var cat = EasingFunction.Curve(1, 0, carrier.Rank / v);
                SetAColor(cat);
                if (!isEnd)
                {
                    UpdateSprite();
                    isEnd = true;
                }
            }

            if (carrier.Rank <= 0)
                carrier.state = State.Destroy;
        }
    }
}