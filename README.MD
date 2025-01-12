# Valheim.SetSkillsRPC

A Valheim Custom RPC mod to remotely set skills of other players. Was made as a concept to undo skill loss from family members dying but didn't really catch on,
and its basically redundant now with the difficulty settings added in the Hildirs Request update.

Mod consists of a new console command called "skill", syntaxed by "skill <player_name> <set,up,down> <skillname[]> <value[]>." If a players name contains spaces, _ is used to concat the name into a single param which is then substituted in the backend.

#  Setup Guide

Templated from JotunnModStub, so please see [Jötunn Docs](https://valheim-modding.github.io/Jotunn/guides/overview.html) detailed documentation and setup.
