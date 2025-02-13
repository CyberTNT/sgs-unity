using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using NativeWebSocket;

public class WS : GlobalSingletonMono<WS>
{
    private WebSocket websocket;

    public async void Connect()
    {
        if (websocket != null && websocket.State is WebSocketState.Open) return;

        websocket = new WebSocket(Url.WEB_SOCKET + "?user_id=" + Model.Self.Instance.UserId.ToString());

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
        };

        websocket.OnMessage += (bytes) =>
        {
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            var msgType = JsonUtility.FromJson<WebsocketMessage>(message).msg_type;
            Debug.Log("OnMessage! " + message);

            switch (msgType)
            {
                case "add_player":
                    var playerJson = JsonUtility.FromJson<AddPlayerMessage>(message).player;
                    Model.Room.Instance.AddPlayer(playerJson);
                    break;

                case "remove_player":
                    var removePlayerMessage = JsonUtility.FromJson<removePlayerMessage>(message);
                    Model.Room.Instance.RemovePlayer(removePlayerMessage);
                    break;

                case "set_already":
                    var setAlreadyMessage = JsonUtility.FromJson<SetAlreadyMessage>(message);
                    Model.Room.Instance.SetAlready(setAlreadyMessage);
                    break;

                case "start_game":
                    messages.Clear();
                    var startGameMessage = JsonUtility.FromJson<StartGameMessage>(message);
                    Model.Room.Instance.StartGame(startGameMessage);
                    break;

                case "surrender":
                    var team = JsonUtility.FromJson<SurrenderMessage>(message).team;
                    Model.GameOver.Instance.Surrender(team);
                    break;

                case "change_skin":
                    var changeSkinMessage = JsonUtility.FromJson<ChangeSkinMessage>(message);
                    Model.SgsMain.Instance.players[changeSkinMessage.position].ChangeSkin(changeSkinMessage.skin_id);
                    break;

                default:
                    messages.Add(message);
                    break;
            }
        };

        await websocket.Connect();
    }

    private List<string> messages = new List<string>();

    public async Task<string> PopMsg()
    {
        while (messages.Count == 0) await Task.Yield();
        var msg = messages[0];
        messages.RemoveAt(0);
        return msg;
    }

    private void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
#endif
    }

    public async void SendJson(WebsocketMessage json)
    {
        var message = JsonUtility.ToJson(json);
        Debug.Log("send:" + message);
        if (websocket.State == WebSocketState.Open) await websocket.SendText(message);
    }

    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }
}