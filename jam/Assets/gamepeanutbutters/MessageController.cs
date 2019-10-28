using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MessageController : MonoBehaviour {
    [Header("Configuration")]
    [Tooltip("Key that will skip to end of message and skip to next message.")]
    public KeyCode SkipKey = KeyCode.Space;
    [Tooltip("Number of characters to add every frame.")]
    public int CharacterBunch = 1;
    [Tooltip("Length of time to spend on each frame.")]
    public float CharacterDelay = 0.01f;
    [Tooltip("Time to wait after each message before moving to the next.")]
    public float MessageDelay = 2f;
    [Tooltip("Whether MessageDelay is used or if SkipKey needs to be pressed to move on.")]
    public bool RequireConfirmation = false;

    [Header("Events")]
    [Tooltip("Fired before any messages in the queue are played.")]
    public UnityEvent OnPanelOpen;
    [Tooltip("Fired after all messages in the queue have played.")]
    public UnityEvent OnPanelClose;
    [Tooltip("Fired when message starts displaying.")]
    public MessageEvent OnMessageStart;
    [Tooltip("Fired after full text is visible, but before delaying or firing AddMessage callback.")]
    public MessageEvent OnMessageEnd;

    [Header("References")]
    [Tooltip("Text component to write to.")]
    public Text messageText;

    private Queue<Message> messages;
    private static MessageController _instance;
    private bool playingMessages = false;
    private bool interruptCurrentMessage = false;

    private bool skipKeyActive = false;
    private bool ShouldSkipText() => skipKeyActive || interruptCurrentMessage;

    private void Awake()
    {
        _instance = this.CheckSingleton(_instance);
        messages = new Queue<Message>(5);
    }

    private void Update()
    {
        if (playingMessages) skipKeyActive = Input.GetKeyDown(SkipKey);
    }

    /// <summary>
    /// Adds a dialog message to the message queue.
    /// </summary>
    /// <param name="message">The message to add.</param>
    /// <param name="interrupt">Clears all previous messages, immediately showing this one.</param>
    /// <param name="shouldLog">Whether or not to add it to the message log.</param>
    public static void AddMessage(string message, bool interrupt = false, bool shouldLog = false, Action postAction = null)
    {
        if (interrupt && _instance.playingMessages)
        {
            _instance.messages.Clear();
        }
        var log = shouldLog ? () => MessageLogger.LogMessage(message) : (Action)(() => { });
        var post = postAction != null ? postAction : () => { };
        Message m = new Message {
            Text = message,
            Action = () => { log(); post(); },
            RequireConfirmation = _instance.RequireConfirmation
        };
        _instance.messages.Enqueue(m);
        if (!_instance.playingMessages)
        {
            _instance.StartCoroutine(_instance.PlayMessages());
        }
    }

    private IEnumerator PlayMessages()
    {
        OnPanelOpen.Invoke();
        playingMessages = true;
        while (playingMessages)
        {
            var currentMessage = messages.Dequeue();
            var currentMessageText = currentMessage.Text;
            OnMessageStart.Invoke(currentMessageText);
            for (int i = CharacterBunch; i < currentMessageText.Length; i += CharacterBunch)
            {
                if (ShouldSkipText())
                {
                    break;
                }
                messageText.text = currentMessageText.Substring(0, i);
                yield return new WaitForSecondsOr(CharacterDelay, ShouldSkipText);
            }
            messageText.text = currentMessageText;
            OnMessageEnd.Invoke(currentMessageText);
            skipKeyActive = false;
            if (RequireConfirmation)
            {
                while (!ShouldSkipText())
                {
                    yield return null;
                }
            } else
            {
                yield return new WaitForSecondsOr(MessageDelay, ShouldSkipText);
            }
            skipKeyActive = false;
            interruptCurrentMessage = false;
            currentMessage.Action();
            if (messages.Count == 0)
            {
                playingMessages = false;
            }
        }
        messageText.text = string.Empty;
        OnPanelClose.Invoke();
    }

    [Serializable]
    public class MessageEvent : UnityEvent<string> { }

    private struct Message
    {
        public string Text;
        public Action Action;
        public bool RequireConfirmation;
    }
}
