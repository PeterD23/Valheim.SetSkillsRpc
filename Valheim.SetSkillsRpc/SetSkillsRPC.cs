using BepInEx;
using Jotunn.Entities;
using Jotunn.Managers;
using System;
using System.Collections;
using UnityEngine;
using static Skills;

namespace Valheim.SetSkillsRpc
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    //[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    internal class SetSkillsRPC : BaseUnityPlugin
    {
        public const string PluginGUID = "com.jotunn.SetSkillsRPC";
        public const string PluginName = "SetSkillsRPC";
        public const string PluginVersion = "0.0.1";

        public static CustomRPC SkillChangeRPC;


        // Use this class to add your own localization to the game
        // https://valheim-modding.github.io/Jotunn/tutorials/localization.html
        public static CustomLocalization Localization = LocalizationManager.Instance.GetLocalization();

        private void Awake()
        {
            Jotunn.Logger.LogInfo("SetSkillsRPC has landed");

            SkillChangeRPC = NetworkManager.Instance.AddRPC("SkillChange", SkillChangeRPCServerReceive, SkillChangeRPCClientReceive);
            CommandManager.Instance.AddConsoleCommand(new SetSkillCommand(SkillChangeRPC));
        }

        private IEnumerator SkillChangeRPCServerReceive(long sender, ZPackage package)
        {
            Jotunn.Logger.LogMessage($"Data has been received on the Server by {sender}");

            yield return new WaitForSeconds(1f);

            var data = package.ReadString();
            Jotunn.Logger.LogMessage($"Received data is {data.Length} bytes");

            var operation = SimpleJson.SimpleJson.DeserializeObject<SkillChangeOperation>(data);
            long userId = operation.PlayerID;

            if (userId == ZRoutedRpc.instance.GetServerPeerID())
            {
                Jotunn.Logger.LogMessage($"Operation target is the server");
                InvokeSetSkill(data);
                yield break;
            }
            Jotunn.Logger.LogMessage($"Forwarding package to {userId}");
            SkillChangeRPC.SendPackage(userId, new ZPackage(package.GetArray()));
        }

        private IEnumerator SkillChangeRPCClientReceive(long sender, ZPackage package)
        {
            Jotunn.Logger.LogMessage($"Data has been received on the Client by {sender}");

            yield return new WaitForSeconds(1f);

            var data = package.ReadString();
            InvokeSetSkill(data);
        }

        private void InvokeSetSkill(string data)
        {
            var operation = SimpleJson.SimpleJson.DeserializeObject<SkillChangeOperation>(data);
            var set = operation.Operation;
            var values = operation.SkillValues;

            var i = 0;

            var player = Player.m_localPlayer.m_skills;
            foreach (var skill in operation.SkillNames)
            {
                SkillType @enum;
                Enum.TryParse(skill, out @enum);
                var currentLevel = player.GetSkillLevel(@enum);
                var value = int.Parse(values.Length > 1 ? values[i] : values[0]);
                if (set.Equals("up"))
                {
                    player.CheatRaiseSkill(skill, value);
                }
                else if (set.Equals("down"))
                {
                    player.CheatRaiseSkill(skill, -value);
                }
                else if (set.Equals("set"))
                {   
                    Player.m_localPlayer.m_skills.CheatRaiseSkill(skill, value - currentLevel);
                }
                Player.m_localPlayer.OnSkillLevelup(@enum, currentLevel);
                i++;
                break;
            }
        }
    }
}