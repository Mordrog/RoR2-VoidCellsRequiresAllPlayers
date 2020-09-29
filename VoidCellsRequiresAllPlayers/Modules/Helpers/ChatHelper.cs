namespace Mordrog
{
    public static class ChatHelper
    {
        private const string RedColor = "ff0000";

        public static void AllPlayersAreRequired()
        {
            var message = $"<color=#{RedColor}>All players are required to be in zone to start!</color>";
            RoR2.Chat.SendBroadcastChat(new RoR2.Chat.SimpleChatMessage { baseToken = message });
        }
    }
}
