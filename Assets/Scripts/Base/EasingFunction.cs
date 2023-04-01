using System;
using UnityEngine;

namespace Base
{
    [Serializable]
    public static class EasingFunction
    {
        public static Vector2 Curve(Vector2 from, Vector2 to, float t, EasingType curve = EasingType.Linear)
        {
            return Lerp(from, to, CurveUnit(t, curve));
        }

        public static Vector3 Curve(Vector3 from, Vector3 to, float t, EasingType curve = EasingType.Linear)
        {
            return Lerp(from, to, CurveUnit(t, curve));
        }

        public static float Curve(float from, float to, float t, EasingType curve = EasingType.Linear)
        {
            return Lerp(from, to, CurveUnit(t, curve));
        }

        public static Color Curve(Color from, Color to, float t, EasingType curve = EasingType.Linear)
        {
            Color current;
            current.r = Lerp(from.r, to.r, CurveUnit(t,curve));
            current.g = Lerp(from.g, to.g, CurveUnit(t, curve));
            current.b = Lerp(from.b, to.b, CurveUnit(t, curve));
            current.a = Lerp(from.a, to.a, CurveUnit(t, curve));
            return current;
        }

        private static float CurveUnit(float t, EasingType curve)
        {
            return curve switch
            {
                EasingType.InSine => InSine(t),
                EasingType.OutSine => OutSine(t),
                EasingType.InOutSine => InOutSine(t),
                EasingType.InQuad => InQuad(t),
                EasingType.OutQuad => OutQuad(t),
                EasingType.InOutQuad => InOutQuad(t),
                EasingType.InCubic => InCubic(t),
                EasingType.OutCubic => OutCubic(t),
                EasingType.InOutCubic => InOutCubic(t),
                EasingType.InQuart => InQuart(t),
                EasingType.OutQuart => OutQuart(t),
                EasingType.InOutQuart => InOutQuart(t),
                EasingType.InQuint => InQuint(t),
                EasingType.OutQuint => OutQuint(t),
                EasingType.InOutQuint => InOutQuint(t),
                EasingType.InExpo => InExpo(t),
                EasingType.OutExpo => OutExpo(t),
                EasingType.InOutExpo => InOutExpo(t),
                EasingType.InCirc => InCirc(t),
                EasingType.OutCirc => OutCirc(t),
                EasingType.InOutCirc => InOutCirc(t),
                EasingType.InBack => InBack(t),
                EasingType.OutBack => OutBack(t),
                EasingType.InOutBack => InOutBack(t),
                EasingType.InElastic => InElastic(t),
                EasingType.OutElastic => OutElastic(t),
                EasingType.InOutElastic => InOutElastic(t),
                EasingType.InBounce => InBounce(t),
                EasingType.OutBounce => OutBounce(t),
                EasingType.InOutBounce => InOutBounce(t),
                _ => Linear(t)
            };
        }

        private static float Lerp(float from, float to, float t)
        {
            return from + (to - from) * t;
        }

        private static Vector2 Lerp(Vector2 from, Vector2 to, float t)
        {
            return from + (to - from) * t;
        }

        private static Vector3 Lerp(Vector3 from, Vector3 to, float t)
        {
            return from + (to - from) * t;
        }

        private static float Linear(float t)
        {
            return t;
        }

        private static float InSine(float t)
        {
            return 1 - MathF.Cos(t * MathF.PI / 2);
        }

        private static float OutSine(float t)
        {
            return MathF.Sin(t * MathF.PI / 2);
        }

        private static float InOutSine(float t)
        {
            return -(MathF.Cos(MathF.PI * t) - 1) / 2;
        }

        private static float InPow(float t, int exp)
        {
            return MathF.Pow(t, exp);
        }

        private static float OutPow(float t, int exp)
        {
            return 1 - MathF.Pow(1 - t, exp);
        }

        private static float InOutPow(float t, int exp)
        {
            return t < 0.5 ? MathF.Pow(2 * t, exp) / 2f : 1 - MathF.Pow(-2 * t + 2, exp) / 2;
        }

        private static float InQuad(float t)
        {
            return t * t;
        }

        private static float OutQuad(float t)
        {
            return 1 - (1 - t) * (1 - t);
        }

        private static float InOutQuad(float t)
        {
            return t < 0.5 ? 2 * t * t : 1 - MathF.Pow(-2 * t + 2, 2) / 2;
        }

        private static float InCubic(float t)
        {
            return InPow(t, 3);
        }

        private static float OutCubic(float t)
        {
            return OutPow(t, 3);
        }

        private static float InOutCubic(float t)
        {
            return InOutPow(t, 3);
        }

        private static float InQuart(float t)
        {
            return InPow(t, 4);
        }

        private static float OutQuart(float t)
        {
            return OutPow(t, 4);
        }

        private static float InOutQuart(float t)
        {
            return InOutPow(t, 4);
        }

        private static float InQuint(float t)
        {
            return InPow(t, 5);
        }

        private static float OutQuint(float t)
        {
            return OutPow(t, 5);
        }

        private static float InOutQuint(float t)
        {
            return InOutPow(t, 5);
        }

        private static float InExpo(float t)
        {
            return t == 0 ? 0 : MathF.Pow(2, 10 * t - 10);
        }

        private static float OutExpo(float t)
        {
            return Math.Abs(t - 1) < float.Epsilon ? 1 : 1 - MathF.Pow(2, -10 * t);
        }

        // https://easings.net/#easeInOutExpo
        private static float InOutExpo(float t)
        {
            return t == 0
                ? 0
                : Math.Abs(t - 1) < float.Epsilon
                    ? 1
                    : t < 0.5
                        ? MathF.Pow(2, 20 * t - 10) / 2
                        : (2 - MathF.Pow(2, -20 * t + 10)) / 2;
        }

        private static float InCirc(float t)
        {
            return 1 - MathF.Sqrt(1 - MathF.Pow(t, 2));
        }

        private static float OutCirc(float t)
        {
            return MathF.Sqrt(1 - MathF.Pow(t - 1, 2));
        }

        private static float InOutCirc(float t)
        {
            return t < 0.5
                ? (1 - MathF.Sqrt(1 - MathF.Pow(2 * t, 2))) / 2
                : (MathF.Sqrt(1 - MathF.Pow(-2 * t + 2, 2)) + 1) / 2;
        }

        private static float InBack(float t)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1;
            return c3 * t * t * t - c1 * t * t;
        }

        private static float OutBack(float t)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1;
            return 1 + c3 * MathF.Pow(t - 1, 3) + c1 * MathF.Pow(t - 1, 2);
        }

        private static float InOutBack(float t)
        {
            const float c1 = 1.70158f;
            const float c2 = c1 * 1.525f;

            return t < 0.5
                ? MathF.Pow(2 * t, 2) * ((c2 + 1) * 2 * t - c2) / 2
                : (MathF.Pow(2 * t - 2, 2) * ((c2 + 1) * (t * 2 - 2) + c2) + 2) / 2;
        }

        private static float InElastic(float t)
        {
            const float c4 = 2 * MathF.PI / 3;

            return t == 0
                ? 0
                : Math.Abs(t - 1) < float.Epsilon
                    ? 1
                    : -MathF.Pow(2, 10 * t - 10) * MathF.Sin((t * 10 - 10.75f) * c4);
        }

        private static float OutElastic(float t)
        {
            const float c4 = 2 * MathF.PI / 3;

            return t == 0
                ? 0
                : Math.Abs(t - 1) < float.Epsilon
                    ? 1
                    : MathF.Pow(2, -10 * t) * MathF.Sin((t * 10 - 0.75f) * c4) + 1;
        }

        private static float InOutElastic(float t)
        {
            const float c5 = 2 * MathF.PI / 4.5f;

            return t == 0
                ? 0
                : Math.Abs(t - 1) < float.Epsilon
                    ? 1
                    : t < 0.5
                        ? -(MathF.Pow(2, 20 * t - 10) * MathF.Sin((20 * t - 11.125f) * c5)) / 2
                        : MathF.Pow(2, -20 * t + 10) * MathF.Sin((20 * t - 11.125f) * c5) / 2 + 1;
        }

        private static float InBounce(float t)
        {
            return 1 - OutBounce(1 - t);
        }

        private static float OutBounce(float t)
        {
            const float n1 = 7.5625f;
            const float d1 = 2.75f;

            return t switch
            {
                < 1 / d1 => n1 * t * t,
                < 2 / d1 => n1 * (t -= 1.5f / d1) * t + 0.75f,
                < 2.5f / d1 => n1 * (t -= 2.25f / d1) * t + 0.9375f,
                _ => n1 * (t -= 2.625f / d1) * t + 0.984375f
            };
        }

        private static float InOutBounce(float t)
        {
            return t < 0.5
                ? (1 - OutBounce(1 - 2 * t)) / 2
                : (1 + OutBounce(2 * t - 1)) / 2;
        }

/*
 Linear,
InSine,
OutSine,
InOutSine,
InQuad,
eOutQuad,
InOutQuad,
InCubic,
OutCubic,
InOutCubic,
InQuart,
eOutQuart,
InOutQuart,
InQuint,
eOutQuint,
InOutQuint,
InExpo,
OutExpo,
InOutExpo,
InCirc,
OutCirc,
InOutCirc,
InBack,
OutBack,
InOutBack,
InElastic,
OutElastic,
InOutElastic,
InBounce,
OutBounce,
InOutBounce*/
    }
}