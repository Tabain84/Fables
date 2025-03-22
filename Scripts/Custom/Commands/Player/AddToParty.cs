using Server.Engines.PartySystem;
using Server.Gumps;
using Server.Mobiles;


namespace Server.Commands
{
    public class DotCommand_AddToParty
    {
        public static void Initialize()
        {
            CommandSystem.Register("AddToParty", AccessLevel.Player, new CommandEventHandler(OnCommand_AddToParty));
        }

        [Usage("AddToParty")]
        [Description("Brings up a list of online players to invite to your group")]
        private static void OnCommand_AddToParty(CommandEventArgs e)
        {
            new GroupInstance(e.Mobile, e.ArgString);
        }
        /*{
        string theName = e.ArgString.ToLower();
        if (theName == "")
        {
            e.Mobile.SendMessage( "Please include the name of the player you want to summon" );
            return;
        }

        List<NetState> states = NetState.Instances;
        for ( int i = 0; i < states.Count; ++i )
        {
            Mobile m = ((NetState)states[i]).Mobile;

            if ( m != null && !m.Deleted)
            {
                if ( m.Name.ToLower().IndexOf(theName) >= 0 )
                {
                    m.MoveToWorld( e.Mobile.Location, e.Mobile.Map );
                    e.Mobile.SendMessage( "Summoned " + m.Name );
                }
            }
        }
    }*/

        private class GroupInstance : IPlayerSelect
        {
            PlayerMobile m_Player;
            public GroupInstance(Mobile player, string argString)
            {
                m_Player = (PlayerMobile)player;
                PlayerSelect.SelectOnlinePlayer(m_Player, this, argString);
            }

            public void OnPlayerSelected(PlayerMobile selectedMobile)
            {
                if (selectedMobile != null)
                {
                    Party p = Party.Get(m_Player);
                    Party mp = Party.Get(selectedMobile);

                    if (m_Player == selectedMobile)
                        m_Player.SendLocalizedMessage(1005439); // You cannot add yourself to a party.
                    else if (p != null && p.Leader != m_Player)
                        m_Player.SendLocalizedMessage(1005453); // You may only add members to the party if you are the leader.
                    else if (selectedMobile.Party is Mobile)
                        return;
                    else if (p != null && (p.Members.Count + p.Candidates.Count) >= Party.Capacity)
                        m_Player.SendLocalizedMessage(1008095); // You may only have 10 in your party (this includes candidates).
                    else if (mp != null && mp == p)
                        m_Player.SendLocalizedMessage(1005440); // This person is already in your party!
                    else if (mp != null)
                        m_Player.SendLocalizedMessage(1005441); // This person is already in a party!
                    else
                        Party.Invite(m_Player, selectedMobile);
                }
                else
                    m_Player.SendMessage(MessageUtil.MessageColorPlayer, "Unable to find that player.");
            }

            public void OnPlayerSelectCanceled()
            {
                m_Player.SendMessage(MessageUtil.MessageColorPlayer, "Unable to find that player.");
            }
        }
    }
}