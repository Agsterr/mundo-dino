using System;

namespace OpenWorldDinoSurvival.Multiplayer.Chat
{
    public enum ChatMessageType
    {
        Global,
        Private,
        System
    }

    [Serializable]
    public struct ChatMessage
    {
        public ChatMessageType Type;
        public ulong SenderClientId;
        public ulong TargetClientId;
        public string SenderName;
        public string Text;
        public DateTime Timestamp;

        public static ChatMessage System(string text)
        {
            return new ChatMessage
            {
                Type = ChatMessageType.System,
                Text = text,
                Timestamp = DateTime.Now
            };
        }
    }
}
