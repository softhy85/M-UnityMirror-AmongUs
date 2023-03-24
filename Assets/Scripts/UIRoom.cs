using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lobbying
{

    public class UIRoom : MonoBehaviour
    {

        [SerializeField] TMPro.TMP_Text roomName;
        [SerializeField] TMPro.TMP_Text nbClientsInRoom;
        [SerializeField] TMPro.TMP_Text maxClientsInRoom;
        [SerializeField] Image lockIcon;

        public void SetRoomUI(string roomName, bool roomIsPrivate)
		{
            this.roomName.SetText(roomName);
            this.lockIcon.enabled = roomIsPrivate;
            //nbClientsInRoom.SetText(room.clientsInRoom.Count.ToString());
            //lockIcon.enabled = !room.isPublic;
        }

    }

}