using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Base;
using System;
using System.Runtime.InteropServices;
using System.IO;
using Newtonsoft.Json;

namespace Main.Control
{
    public static class InputAxis
    {
        public static string Horizonta = "Horizontal";
        public static string Vertical = "Vertical";
        public static string X = "Mouse X";
        public static string Y = "Mouse Y";
        public static string S = "Mouse ScrollWheel";
    }

    public class PMovement : MainPlayer
    {
        public static PMovement main;
        Vector3 LastMousePosition;
        [Header("Speed")]
        public float speed = 1;

        private void Awake()
        {
            if (main != null && main != this) Destroy(gameObject);
            main = this;
        }

        //ÒÆ¶¯¿ØÖÆ
        public override void update()
        {
            MainTransform.position += Input.GetAxis(InputAxis.Vertical) * speed * Time.deltaTime * Vector3.up;
            MainTransform.position += Input.GetAxis(InputAxis.Horizonta) * speed * Time.deltaTime * Vector3.right;

            var vecg = Input.mousePosition - LastMousePosition;
            LastMousePosition = Input.mousePosition;
            if (Input.GetMouseButtonDown(0))
            {
                MainTransform.position -= Mathf.Clamp(vecg.y, -1, 1) * speed * Time.deltaTime * Vector3.up;
                MainTransform.position -= Mathf.Clamp(vecg.x, -1, 1) * speed * Time.deltaTime * Vector3.right;
            }
            /*if (Input.GetKey(KeyCode.Mouse0))
            {
                MainTransform.position -= Input.GetAxis(InputAxis.Y) * 3 * speed * Time.deltaTime * Vector3.up;
                MainTransform.position -= Input.GetAxis(InputAxis.X) * 3 * speed * Time.deltaTime * Vector3.right;
            }*/

            MainCamera.orthographicSize -= Input.GetAxis(InputAxis.S) * 5 * speed * Time.deltaTime;
            if (Input.GetKeyDown(KeyCode.E)) MainCamera.orthographicSize -= 5 * speed * Time.deltaTime;
            if (Input.GetKeyDown(KeyCode.Z)) MainCamera.orthographicSize += 5 * speed * Time.deltaTime;
            MainCamera.orthographicSize = Mathf.Clamp(MainCamera.orthographicSize, 1, 100000);

            if (Input.touchCount == 1)
            {
                var vec = Input.GetTouch(0).deltaPosition;
                MainTransform.position -= Mathf.Clamp(vec.y, -1, 1) * speed * Time.deltaTime * Vector3.up;
                MainTransform.position -= Mathf.Clamp(vec.x, -1, 1) * speed * Time.deltaTime * Vector3.right;
            }
            if (Input.touchCount >= 2)
            {
                Vector2 vec1 = Input.GetTouch(0).deltaPosition, vec2 = Input.GetTouch(1).deltaPosition;
                if (vec1.y * vec2.y < 0) MainCamera.orthographicSize += 5 * speed * Time.deltaTime;
                else MainCamera.orthographicSize -= 5 * speed * Time.deltaTime;
                if (vec1.x * vec2.x < 0) MainCamera.orthographicSize += 5 * speed * Time.deltaTime;
                else MainCamera.orthographicSize -= 5 * speed * Time.deltaTime;
            }
        }

        public override void Init(MainMap from)
        {
            Movement_Map map = from as Movement_Map;
            this.MainTransform.position = new Vector3(map.x, map.y, -10);
            this.MainCamera.orthographicSize = map.orthographicSize;
            speed = map.speed;
        }

        public Movement_Map Save()
        {
            return new Movement_Map(this);
        }

        public override void OnSave(string path)
        {
            File.WriteAllText(MainSystem.PMovement_Json_FullName(path), JsonConvert.SerializeObject(Save()));
        }

        public override void OnLoad(string path)
        {
            string p = MainSystem.PMovement_Json_FullName(path);
            if (File.Exists(p))
                Init(JsonConvert.DeserializeObject<Movement_Map>(File.ReadAllText(p)));
            else
                Init(new Movement_Map());
        }
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class Movement_Map : MainMap
    {
        public Movement_Map() { }

        public Movement_Map(PMovement from)
        {
            Init(from);
        }
        public override void Init(MainPlayer from)
        {
            var pos = from.MainTransform.position;
            x = pos.x;
            y = pos.y;
            orthographicSize = (from as PMovement).MainCamera.orthographicSize;
            speed = (from as PMovement).speed;
        }

        public float x = 0;
        public float y = 0;
        public float orthographicSize = 5;
        public float speed = 30;

    }

}
