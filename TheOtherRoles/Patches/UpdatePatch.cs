using HarmonyLib;
using System;
using System.IO;
using System.Net.Http;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;
using TheOtherRoles.Objects;
using System.Collections.Generic;
using System.Linq;

namespace TheOtherRoles.Patches {
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    class HudManagerUpdatePatch
    {
        static void resetNameTagsAndColors() {
            Dictionary<byte, PlayerControl> playersById = Helpers.allPlayersById();

            foreach (PlayerControl player in PlayerControl.AllPlayerControls) {
                String playerName = player.Data.PlayerName;
                if (Morphling.morphTimer > 0f && Morphling.player == player && Morphling.morphTarget != null) playerName = Morphling.morphTarget.Data.PlayerName; // Temporary hotfix for the Morphling's name

                player.nameText.text = Helpers.hidePlayerName(PlayerControl.LocalPlayer, player) ? "" : playerName;
                if (PlayerControl.LocalPlayer.Data.Role.IsImpostor && player.Data.Role.IsImpostor) {
                    player.nameText.color = Palette.ImpostorRed;
                } else {
                    player.nameText.color = Color.white;
                }
            }
            if (MeetingHud.Instance != null) {
                foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates) {
                    PlayerControl playerControl = playersById.ContainsKey((byte)player.TargetPlayerId) ? playersById[(byte)player.TargetPlayerId] : null;
                    if (playerControl != null) {
                        player.NameText.text = playerControl.Data.PlayerName;
                        if (PlayerControl.LocalPlayer.Data.Role.IsImpostor && playerControl.Data.Role.IsImpostor) {
                            player.NameText.color = Palette.ImpostorRed;
                        } else {
                            player.NameText.color = Color.white;
                        }
                    }
                }
            }
            if (PlayerControl.LocalPlayer.Data.Role.IsImpostor) {
                List<PlayerControl> impostors = PlayerControl.AllPlayerControls.ToArray().ToList();
                impostors.RemoveAll(x => !x.Data.Role.IsImpostor);
                foreach (PlayerControl player in impostors)
                    player.nameText.color = Palette.ImpostorRed;
                if (MeetingHud.Instance != null)
                    foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates) {
                        PlayerControl playerControl = Helpers.playerById((byte)player.TargetPlayerId);
                        if (playerControl != null && playerControl.Data.Role.IsImpostor)
                            player.NameText.color =  Palette.ImpostorRed;
                    }
            }

        }

        static void setPlayerNameColor(PlayerControl p, Color color) {
            p.nameText.color = color;
            if (MeetingHud.Instance != null)
                foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                    if (player.NameText != null && p.PlayerId == player.TargetPlayerId)
                        player.NameText.color = color;
        }

        static void setNameColors() {
            if (Jester.player != null && Jester.player == PlayerControl.LocalPlayer)
                setPlayerNameColor(Jester.player, Jester.color);
            else if (Mayor.player != null && Mayor.player == PlayerControl.LocalPlayer)
                setPlayerNameColor(Mayor.player, Mayor.color);
            else if (Engineer.player != null && Engineer.player == PlayerControl.LocalPlayer)
                setPlayerNameColor(Engineer.player, Engineer.color);
            else if (Sheriff.player != null && Sheriff.player == PlayerControl.LocalPlayer) 
                setPlayerNameColor(Sheriff.player, Sheriff.color);
            else if (Lighter.player != null && Lighter.player == PlayerControl.LocalPlayer) 
                setPlayerNameColor(Lighter.player, Lighter.color);
            else if (Detective.player != null && Detective.player == PlayerControl.LocalPlayer) 
                setPlayerNameColor(Detective.player, Detective.color);
            else if (TimeMaster.player != null && TimeMaster.player == PlayerControl.LocalPlayer)
                setPlayerNameColor(TimeMaster.player, TimeMaster.color);
            else if (Medic.player != null && Medic.player == PlayerControl.LocalPlayer)
                setPlayerNameColor(Medic.player, Medic.color);
            else if (Shifter.player != null && Shifter.player == PlayerControl.LocalPlayer)
                setPlayerNameColor(Shifter.player, Shifter.color);
            else if (Swapper.player != null && Swapper.player == PlayerControl.LocalPlayer)
                setPlayerNameColor(Swapper.player, Swapper.color);
            else if (Seer.player != null && Seer.player == PlayerControl.LocalPlayer)
                setPlayerNameColor(Seer.player, Seer.color);  
            else if (Hacker.player != null && Hacker.player == PlayerControl.LocalPlayer) 
                setPlayerNameColor(Hacker.player, Hacker.color);
            else if (Tracker.player != null && Tracker.player == PlayerControl.LocalPlayer) 
                setPlayerNameColor(Tracker.player, Tracker.color);
            else if (Snitch.player != null && Snitch.player == PlayerControl.LocalPlayer) 
                setPlayerNameColor(Snitch.player, Snitch.color);
            else if (Jackal.player != null && Jackal.player == PlayerControl.LocalPlayer) {
                // Jackal can see his sidekick
                setPlayerNameColor(Jackal.player, Jackal.color);
                if (Sidekick.player != null) {
                    setPlayerNameColor(Sidekick.player, Jackal.color);
                }
                if (Jackal.fakeSidekick != null) {
                    setPlayerNameColor(Jackal.fakeSidekick, Jackal.color);
                }
            }
            else if (Spy.player != null && Spy.player == PlayerControl.LocalPlayer) {
                setPlayerNameColor(Spy.player, Spy.color);
            } else if (SecurityGuard.player != null && SecurityGuard.player == PlayerControl.LocalPlayer) {
                setPlayerNameColor(SecurityGuard.player, SecurityGuard.color);
            } else if (Arsonist.player != null && Arsonist.player == PlayerControl.LocalPlayer) {
                setPlayerNameColor(Arsonist.player, Arsonist.color);
            } else if (Guesser.player != null && Guesser.player == PlayerControl.LocalPlayer) {
                setPlayerNameColor(Guesser.player, Guesser.player.Data.Role.IsImpostor ? Palette.ImpostorRed : Guesser.color);
            } else if (Bait.player != null && Bait.player == PlayerControl.LocalPlayer) {
                setPlayerNameColor(Bait.player, Bait.color);
            } else if (Vulture.player != null && Vulture.player == PlayerControl.LocalPlayer) {
                setPlayerNameColor(Vulture.player, Vulture.color);
            } else if (Medium.player != null && Medium.player == PlayerControl.LocalPlayer) {
                setPlayerNameColor(Medium.player, Medium.color);
            } else if (Lawyer.player != null && Lawyer.player == PlayerControl.LocalPlayer) {
                setPlayerNameColor(Lawyer.player, Lawyer.color);
            } else if (Pursuer.player != null && Pursuer.player == PlayerControl.LocalPlayer) {
                setPlayerNameColor(Pursuer.player, Pursuer.color);
            }

            // No else if here, as a Lover of team Jackal needs the colors
            if (Sidekick.player != null && Sidekick.player == PlayerControl.LocalPlayer) {
                // Sidekick can see the jackal
                setPlayerNameColor(Sidekick.player, Sidekick.color);
                if (Jackal.player != null) {
                    setPlayerNameColor(Jackal.player, Jackal.color);
                }
            }

            // No else if here, as the Impostors need the Spy name to be colored
            if (Spy.player != null && PlayerControl.LocalPlayer.Data.Role.IsImpostor) {
                setPlayerNameColor(Spy.player, Spy.color);
            }

            // Crewmate roles with no changes: Mini
            // Impostor roles with no changes: Morphling, Camouflager, Vampire, Godfather, Eraser, Janitor, Cleaner, Warlock, BountyHunter,  Witch and Mafioso
        }

        static void setNameTags() {
            // Mafia
            if (PlayerControl.LocalPlayer != null && PlayerControl.LocalPlayer.Data.Role.IsImpostor) {
                foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                    if (Godfather.player != null && Godfather.player == player)
                            player.nameText.text = player.Data.PlayerName + " (G)";
                    else if (Mafioso.player != null && Mafioso.player == player)
                            player.nameText.text = player.Data.PlayerName + " (M)";
                    else if (Janitor.player != null && Janitor.player == player)
                            player.nameText.text = player.Data.PlayerName + " (J)";
                if (MeetingHud.Instance != null)
                    foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                        if (Godfather.player != null && Godfather.player.PlayerId == player.TargetPlayerId)
                            player.NameText.text = Godfather.player.Data.PlayerName + " (G)";
                        else if (Mafioso.player != null && Mafioso.player.PlayerId == player.TargetPlayerId)
                            player.NameText.text = Mafioso.player.Data.PlayerName + " (M)";
                        else if (Janitor.player != null && Janitor.player.PlayerId == player.TargetPlayerId)
                            player.NameText.text = Janitor.player.Data.PlayerName + " (J)";
            }

            // Lovers
            if (Lovers.lover1 != null && Lovers.lover2 != null && (Lovers.lover1 == PlayerControl.LocalPlayer || Lovers.lover2 == PlayerControl.LocalPlayer)) {
                string suffix = Helpers.cs(Lovers.color, " ♥");
                Lovers.lover1.nameText.text += suffix;
                Lovers.lover2.nameText.text += suffix;

                if (MeetingHud.Instance != null)
                    foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                        if (Lovers.lover1.PlayerId == player.TargetPlayerId || Lovers.lover2.PlayerId == player.TargetPlayerId)
                            player.NameText.text += suffix;
            }

            // Lawyer
            bool localIsLawyer = Lawyer.player != null && Lawyer.target != null && Lawyer.player == PlayerControl.LocalPlayer;
            bool localIsKnowingTarget = Lawyer.player != null && Lawyer.target != null && Lawyer.targetKnows && Lawyer.target == PlayerControl.LocalPlayer;
            if (localIsLawyer || (localIsKnowingTarget && !Lawyer.player.Data.IsDead)) {
                string suffix = Helpers.cs(Lawyer.color, " §");
                Lawyer.target.nameText.text += suffix;

                if (MeetingHud.Instance != null)
                    foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                        if (player.TargetPlayerId == Lawyer.target.PlayerId)
                            player.NameText.text += suffix;
            }

            // Hacker and Detective
            if (PlayerControl.LocalPlayer != null && !PlayerControl.LocalPlayer.Data.IsDead && (PlayerControl.LocalPlayer == Hacker.player || PlayerControl.LocalPlayer == Detective.player || PlayerControl.LocalPlayer == Medium.player)) {
                if (MeetingHud.Instance != null) {
                    foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates) {
                        var target = Helpers.playerById(player.TargetPlayerId);
                        if (target != null)  player.NameText.text += $" ({(Helpers.isLighterColor(target.Data.DefaultOutfit.ColorId) ? "L" : "D")})";
                    }
                }
            }
        }

        static void updateShielded() {
            if (Medic.shielded == null) return;

            if (Medic.shielded.Data.IsDead || Medic.player == null || Medic.player.Data.IsDead) {
                Medic.shielded = null;
            }
        }

        static void timerUpdate() {
            Hacker.hackerTimer -= Time.deltaTime;
            Lighter.lighterTimer -= Time.deltaTime;
            Trickster.lightsOutTimer -= Time.deltaTime;
            Tracker.corpsesTrackingTimer -= Time.deltaTime;
        }

        public static void miniUpdate() {
            if (Mini.player == null || Camouflager.camouflageTimer > 0f) return;
                
            float growingProgress = Mini.growingProgress();
            float scale = growingProgress * 0.35f + 0.35f;
            string suffix = "";
            if (growingProgress != 1f)
                suffix = " <color=#FAD934FF>(" + Mathf.FloorToInt(growingProgress * 18) + ")</color>"; 

            Mini.player.nameText.text += suffix;
            if (MeetingHud.Instance != null) {
                foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                    if (player.NameText != null && Mini.player.PlayerId == player.TargetPlayerId)
                        player.NameText.text += suffix;
            }

            if (Morphling.player != null && Morphling.morphTarget == Mini.player && Morphling.morphTimer > 0f)
                Morphling.player.nameText.text += suffix;
        }

        static void updateImpostorKillButton(HudManager __instance) {
            if (!PlayerControl.LocalPlayer.Data.Role.IsImpostor) return;
            bool enabled = true;
            if (Vampire.player != null && Vampire.player == PlayerControl.LocalPlayer)
                enabled = false;
            else if (Mafioso.player != null && Mafioso.player == PlayerControl.LocalPlayer && Godfather.player != null && !Godfather.player.Data.IsDead)
                enabled = false;
            else if (Janitor.player != null && Janitor.player == PlayerControl.LocalPlayer)
                enabled = false;
            
            if (enabled) __instance.KillButton.Show();
            else __instance.KillButton.Hide();
        }

        static void Postfix(HudManager __instance)
        {
            if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) return;

            CustomButton.HudUpdate();
            resetNameTagsAndColors();
            setNameColors();
            updateShielded();
            setNameTags();

            // Impostors
            updateImpostorKillButton(__instance);
            // Timer updates
            timerUpdate();
            // Mini
            miniUpdate();
        }
    }
}
