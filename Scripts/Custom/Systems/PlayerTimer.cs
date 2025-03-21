using Server;
using System;
using System.Collections;
using System.Collections.Generic;

using Server.Misc;
using Server.Mobiles;
using Server.Training;
using Server.Commands;

namespace Server.Misc
{
    public class PlayerTimer
    {
        private static readonly List<Mobile> _activePlayers = new List<Mobile>();
        private static DateTime _lastMessageTime = DateTime.MinValue;


        public static void Initialize()
        {
            Console.WriteLine("Player Timer running.");
            // Start the timer when the server boots
            Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromSeconds(5), OnTick);
            EventSink.Login += PlayerTimerInit;
            EventSink.Logout += OnPlayerLogout;
        }

        private static void PlayerTimerInit(LoginEventArgs e)
        {

            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm != null && !_activePlayers.Contains(pm))
            {
                _activePlayers.Add(pm); //Add player to list of online playercharacters to iterate timer over.
                pm.StatGained = DateTime.Now;
            }
        
            pm.StatGained = DateTime.Now;
        }
        private static void OnPlayerLogout(LogoutEventArgs e)
        {
            Mobile player = e.Mobile;

            if (player != null && _activePlayers.Contains(player))
            {
                _activePlayers.Remove(player); //Remove player from timer list when logging out.
            }
        }

        private static void OnTick()
        {
            if (_activePlayers.Count == 0)
            {
                return; //Nobody online, move on
            }

            DateTime now = DateTime.UtcNow;

            bool shouldSendMessage = (now - _lastMessageTime) >= Server.Settings.MessageDelay;

            for (int i = 0; i < _activePlayers.Count; i++)
            {
                PlayerMobile player = _activePlayers[i] as PlayerMobile;

                if (player != null && player.NetState != null)
                {
                    DoStatGain(player);
                    DoSkillTraining(player);

                    if (shouldSendMessage && player.AccessLevel == AccessLevel.Player)
                    {
                        if (player.SpecialSkills[0] == SkillName.TasteID)
                        {
                            player.SendMessage(MessageUtil.MessageColorPlayer, "You have not set your specializations yet. Please type .spec and select your skills.");
                        }
                    }
                }
                else
                {
                    // Cleanup invalid players
                    _activePlayers.RemoveAt(i);
                    i--;  // Adjust the index after removal
                }
            }
        }
        
        public static void DoStatGain( PlayerMobile from )
        {
            if ( from.StatGained + Server.Settings.StatDelay <= DateTime.Now ) {

                from.StatGained = DateTime.Now;

                Exp.ExpGain.AwardExperience( from, (int)(Server.Settings.PassiveExpHour / (60 / (int)Server.Settings.StatDelay.TotalMinutes)), true, false );

                if ( Utility.RandomBool() ) { //50% to gain a point every StatDelay minutes.
                    switch ( Utility.Random( 3 ) ) {
                        case 0: {
                                if ( from.StrLock == StatLockType.Up && from.RawStr != 125 )
                                    SkillCheck.GainStat( from, SkillCheck.Stat.Str );

                                else if ( from.DexLock == StatLockType.Up )
                                    goto case 1;
                            }
                            break;
                        case 1: {
                                if ( from.DexLock == StatLockType.Up && from.RawDex != 125 )
                                    SkillCheck.GainStat( from, SkillCheck.Stat.Dex );
                                else if ( from.IntLock == StatLockType.Up )
                                    goto case 2;
                            }
                            break;
                        case 2: {
                                if ( from.IntLock == StatLockType.Up && from.RawInt != 125 )
                                    SkillCheck.GainStat( from, SkillCheck.Stat.Int );
                                else if ( from.StrLock == StatLockType.Up )
                                    goto case 0;
                            }
                            break;
                    }
                }
            }
        }
        public static void DoSkillTraining( PlayerMobile from )
        {
            List<SkillName> trainList = new List<SkillName>( from.TrainingPoints.Keys );

            if ( trainList.Count > 0 ) {
                for ( int i = 0; i <= trainList.Count - 1; i++ ) {

                    int trainAmount = 5;

                    if ( SkillMaster.IsEasyskill( trainList[i] ) )
                        trainAmount = 6;
                    else if ( SkillMaster.IsHardskill( trainList[i] ) )
                        trainAmount = 4;

                    if ( from.TrainingPoints[trainList[i]] >= 6 )
                        SkillTraining.TrainSkill( from, trainList[i], trainAmount );
                    else
                        SkillTraining.TrainSkill( from, trainList[i], from.TrainingPoints[trainList[i]] );
                }
            }
        }
    }
}
