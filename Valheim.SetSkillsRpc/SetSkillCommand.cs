using Jotunn.Entities;
using Jotunn.Extensions;
using System;
using System.Linq;

namespace Valheim.SetSkillsRpc
{
    public class SetSkillCommand : ConsoleCommand
    {
        private static CustomRPC SkillChangeRPC;

        public SetSkillCommand(CustomRPC rpc)
        {
            SkillChangeRPC = rpc;
        }

        public override string Name => "skill";

        public override string Help => "skill PlayerName {UP,DOWN,SET} [Skill1,Skill2,Skill3] [Value1,Value2,Value3]. For player names with spaces use _";

        public override void Run(string[] args)
        {
            if (args.Length != 4)
            {
                Console.instance.Print($"Command expects 4 arguments. {Help}");
                return;
            }

            var playerId = GetIDFromName(args[0]);
            var operation = args[1].ToLower();

            // Check operation validity 
            if (!new[] { "up", "down", "set"}.Any(s => operation.Equals(s))){
                Console.instance.Print("Invalid operation. Operations are UP, DOWN, SET");
                return;
            }

            var skillNames = args[2].Split(',');
            if(!AreSkillsValid(skillNames))
                return;

            var skillValues = args[3].Split(',');
            if(!AreSkillValuesValid(skillValues, skillNames))
                return;
            
            var dataToSend = new SkillChangeOperation
            {
                PlayerID = playerId,
                Operation = operation,
                SkillNames = skillNames,
                SkillValues = skillValues
            };

            string json = SimpleJson.SimpleJson.SerializeObject(dataToSend);
            ZPackage package = new ZPackage();
            package.Write(json);

            SkillChangeRPC.SendPackage(ZRoutedRpc.instance.GetServerPeerID(), package);
        }

        private bool AreSkillsValid(string[] skillNames)
        {
            foreach (var skillName in skillNames)
            {
                Skills.SkillType skill;
                if (!Enum.TryParse(skillName.CapitalizeFirstLetter(), out skill))
                {
                    Console.instance.Print("One or more skills is invalid.");
                    return false;
                }
            }
            return true;
        } 

        private bool AreSkillValuesValid(string[] skillValues, string[] skillNames)
        {
            var length = skillValues.Length;
            if (length != skillNames.Length && length != 1)
            {
                Console.instance.Print("Number of skill values should be 1 to apply to all, or match the number of skills to apply individually.");
                return false;
            }
            foreach (var skillValue in skillValues)
            {
                int value;
                if (!int.TryParse(skillValue, out value))
                {
                    Console.instance.Print("One or more value is invalid. Please use integers.");
                    return false;
                }
            }
            return true;
        }

        private long GetIDFromName(string searchPlayer)
        {
            var replacedSearch = searchPlayer.ToLower().Replace("_", " ");
            var players = ZNet.instance.m_players;
            foreach(var p in players)
            {
                Console.instance.Print($"{p.m_name}: {p.m_characterID}");
            }
            var player = players.FirstOrDefault(p => p.m_name.ToLower().Contains(replacedSearch));
            if (!player.m_characterID.IsNone()) return player.m_characterID.UserID;
            throw new InvalidOperationException("Player does not exist.");
        }

    }
}
