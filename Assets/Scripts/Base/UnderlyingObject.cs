using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Base
{
    /// <summary>
    /// The Base Exception of logic to throw out with local catch
    /// </summary>
    [Serializable]
    public class BaseInException : Exception
    {
        public BaseInException() { }
        public BaseInException(string message) : base(message + "_In_Throw_" + Time.time) { }
        public BaseInException(string message, Exception inner) : base(message, inner) { }
        public BaseInException(MonoBehaviour mono) : base(mono.name + "_In_Throw_" + Time.time) { }
        protected BaseInException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// Rethrow BaseInException
    /// </summary>
    [Serializable]
    public class BaseOutException : Exception
    {
        public BaseOutException() { }
        public BaseOutException(string message) : base(message) { }
        public BaseOutException(string message, Exception inner) : base(message, inner) { }
        public BaseOutException(BaseInException baseIn):base(baseIn.Message) { }
        protected BaseOutException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }


    /// <summary>
    /// Passed as a parameter of the coroutine-imitation
    /// <para>The point is to allow external control or complete laissez-faire</para>
    /// </summary>
    public class Carrier
    {
        public Carrier() { }

        public Carrier(string baseName, int rank = 0, State state = State.Active, float value = 0)
        {
            BaseName = baseName;
            Rank = rank;
            this.state = state;
            Value = value;
        }

        public Carrier(State state)
        {
            this.state = state;
        }

        public Carrier(float value)
        {
            Value = value;
        }

        public Carrier(int rank)
        {
            Rank = rank;
        }

        public string BaseName = "";
        public int Rank = 0;
        public State state = State.Active;
        public float Value = 0;
    }

    public abstract class UnderlyingObject : MonoBehaviour
    {
        public float DoTimeClock;

        private readonly List<(Action<Carrier>, Carrier)> Executor = new();
        [SuppressMessage("Style", "IDE0044:添加只读修饰符", Justification = "<挂起>")]
        private readonly List<(Action<Carrier>, Carrier)> FixedExecutor = new();
        private bool __IsAddExecutor = false;

        /// <summary>
        /// A function that has ended normal working logic
        /// </summary>
        private void Update()
        {
            update();
            for (var i = 0; i < Executor.Count;)
            {
                var it = Executor[i];
                if (it.Item2.state == State.Active)
                    it.Item1(it.Item2);
                if (it.Item2.state == State.Destroy)
                    Remove(it.Item1);
                else
                    i++;
            }
        }

        /// <summary>
        /// A function that has ended normal working logic
        /// </summary>
        private void FixedUpdate()
        {
            fixedUpdate();
            for (var i = 0; i < FixedExecutor.Count;)
            {
                var it = FixedExecutor[i];
                if (it.Item2.state == State.Active)
                    it.Item1(it.Item2);
                if (it.Item2.state == State.Destroy)
                    RemoveF(it.Item1);
                else
                    i++;
            }
        }

        public void Add(Action<Carrier> executor, Carrier carrier)
        {
            try
            {
                if (Executor.FindIndex(T => T.Item1 == executor) != -1)
                    throw new BaseInException("public void Add(Action<Carrier> executor, Carrier carrier),Try adding the same function repeatedly");
                Executor.Add((executor, carrier));
                __IsAddExecutor = true;
            }
            catch (BaseInException ex)
            {
                Debug.LogWarning(ex.Message);
                Executor.Add((T => { executor(T); }, carrier));
            }
        }

        public void Add(Action<Carrier> executor)
        {
            Add(executor, new Carrier());
        }

        public bool Remove(Action<Carrier> executor, Carrier carrier, bool isex = true)
        {
            if (Executor.Contains((executor, carrier)))
            {
                Executor.Remove((executor, carrier));
                return true;
            }
            else if (FixedExecutor.Contains((executor, carrier)))
            {
                FixedExecutor.Remove((executor, carrier));
                return true;
            }
            else return false;

        }

        public bool Remove(int id, bool isex)
        {
            try
            {
                if (isex)
                {
                    if (id < 0 || id >= Executor.Count) throw new BaseInException("public bool Remove(int id),id for Executor is over stack");
                    Executor.RemoveAt(id);
                }
                else
                {
                    if (id < 0 || id >= FixedExecutor.Count) throw new BaseInException("public bool Remove(int id),id for FixedExecutor is over stack");
                    FixedExecutor.RemoveAt(id);
                }
                return true;
            }
            catch (BaseInException ex)
            {
                Debug.LogWarning(ex.Message);
                return false;
            }
        }

        public bool Remove()
        {
            if (__IsAddExecutor && Executor.Count > 0)
            {
                Executor.Remove(Executor[^1]);
                return true;
            }
            else if (!__IsAddExecutor && FixedExecutor.Count > 0)
            {
                FixedExecutor.Remove(Executor[^1]);
                return true;
            }
            else return false;
        }

        public bool Remove(Action<Carrier> executor, bool isex = true)
        {
            return Remove(Executor.FindIndex(T => T.Item1 == executor), isex);
        }

        public void RemoveClearly(bool isex = true)
        {
            if (isex) Executor.Clear();
            else FixedExecutor.Clear();
        }

        public void AddF(Action<Carrier> executor, Carrier carrier)
        {
            try
            {
                if (FixedExecutor.FindIndex(T => T.Item1 == executor) != -1)
                    throw new BaseInException("public void AddF(Action<Carrier> executor, Carrier carrier),Try adding the same function repeatedly");
                FixedExecutor.Add((executor, carrier));
                __IsAddExecutor = false;
            }
            catch (BaseInException ex)
            {
                Debug.LogWarning(ex.Message);
                FixedExecutor.Add((T => { executor(T); }, carrier));
            }
        }

        public void AddF(Action<Carrier> executor)
        {
            AddF(executor, new Carrier());
        }

        public void RemoveF(Action<Carrier> executor) => Remove(executor, false);

        public void RemoveF(int id) => Remove(id, false);

        public bool SetState(Action<Carrier> executor, State state)
        {
            var cat = Executor.Find(T => T.Item1 == executor);
            var t = false;
            if (cat != (null, null))
            {
                cat.Item2.state = state;
                t = true;
            }

            cat = FixedExecutor.Find(T => T.Item1 == executor);
            {
                if (cat != (null, null)) cat.Item2.state = state;
                t = true;
            }
            return t;
        }

        public bool OpenState(Action<Carrier> executor)
        {
            return SetState(executor, State.Active);
        }

        public bool CloseState(Action<Carrier> executor)
        {
            return SetState(executor, State.Ended);
        }

#pragma warning disable IDE1006 // 命名样式
        public virtual void fixedUpdate()
        {
        }

        public virtual void update()
        {
        }

#pragma warning restore IDE1006 // 命名样式
    }


    public enum State
    {
        Preparation,
        Active,
        Ended,
        Destroy
    }
}