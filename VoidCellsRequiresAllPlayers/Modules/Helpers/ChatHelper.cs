namespace VoidCellsRequiresAllPlayers
{
    public static class ChatHelper
    {
        private const string RedColor = "ff0000";
        private const string GreenColor = "32cd32";

        public static void VoidStartMessage()
        {
            var message = $"<color=#{GreenColor}>All players will be healed on void cell interaction!</color>";
            RoR2.Chat.SendBroadcastChat(new RoR2.Chat.SimpleChatMessage { baseToken = message });
        }

        public static void AllPlayersAreRequired()
        {
            var message = $"<color=#{RedColor}>All players are required to be in zone to start!</color>";
            RoR2.Chat.SendBroadcastChat(new RoR2.Chat.SimpleChatMessage { baseToken = message });
        }
    }
}
