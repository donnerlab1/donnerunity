using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(TwitchIRC))]
public class TwitchChatExample : MonoBehaviour
{
    public int maxMessages = 100; //we start deleting UI elements when the count is larger than this var.
    private LinkedList<GameObject> messages =
        new LinkedList<GameObject>();
    public UnityEngine.UI.InputField inputField;
    public UnityEngine.UI.Button submitButton;
    public UnityEngine.RectTransform chatBox;
    public UnityEngine.UI.ScrollRect scrollRect;
    private TwitchIRC IRC;
    //when message is recieved from IRC-server or our own message.
    void OnChatMsgRecieved(string msg)
    {
        //parse from buffer.
        int msgIndex = msg.IndexOf("PRIVMSG #");
        string msgString = msg.Substring(msgIndex + IRC.channelName.Length + 11);
        string user = msg.Substring(1, msg.IndexOf('!') - 1);

        //remove old messages for performance reasons.
        if (messages.Count > maxMessages)
        {
            Destroy(messages.First.Value);
            messages.RemoveFirst();
        }

        //add new message.
        CreateUIMessage(user, msgString);
    }
    public void CreateUIMessage(string userName, string msgString)
    {
        Color32 c = ColorFromUsername(userName);
        string nameColor = "#" + c.r.ToString("X2") + c.g.ToString("X2") + c.b.ToString("X2");
        GameObject go = new GameObject("twitchMsg");
        var text = go.AddComponent<UnityEngine.UI.Text>();
        var layout = go.AddComponent<UnityEngine.UI.LayoutElement>();
        go.transform.SetParent(chatBox);
        messages.AddLast(go);

        layout.minHeight = 20f;
        text.text = "<color=" + nameColor + "><b>" + userName + "</b></color>" + ": " + msgString;
        text.color = Color.black;
        text.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        scrollRect.velocity = new Vector2(0, 1000f);
    }
    //when Submit button is clicked or ENTER is pressed.
    public void OnSubmit()
    {
        if (inputField.text.Length > 0)
        {
            IRC.SendMsg(inputField.text); //send message.
            CreateUIMessage(IRC.nickName, inputField.text); //create ui element.
            inputField.text = "";
        }
    }
    Color ColorFromUsername(string username)
    {
        Random.seed = username.Length + (int)username[0] + (int)username[username.Length - 1];
        return new Color(Random.Range(0.25f, 0.55f), Random.Range(0.20f, 0.55f), Random.Range(0.25f, 0.55f));
    }
    // Use this for initialization
    void Start()
    {
        IRC = this.GetComponent<TwitchIRC>();
        //IRC.SendCommand("CAP REQ :twitch.tv/tags"); //register for additional data such as emote-ids, name color etc.
        IRC.messageRecievedEvent.AddListener(OnChatMsgRecieved);
    }
}
/*
 * Thanks to https://github.com/Grahnz/TwitchIRC-Unity
 * 
 The MIT License (MIT)

Copyright (c) 2015 Grahnz

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

     */
