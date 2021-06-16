using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ProtobufProto.Model;

namespace Game.UI
{
    public delegate void MouseDownEventHandle(GameObject obj);
    public class RoomTitle : MonoBehaviour
    {
        public RoomInfo Room;
        public event MouseDownEventHandle OnPointerDown;
        public void OnMouseDown()
        {
            OnPointerDown?.Invoke(gameObject);
        }
    }

}

