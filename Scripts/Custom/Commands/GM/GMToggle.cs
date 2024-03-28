/*
 * Created by SharpDevelop.
 * User:    steamin
 * Date:    20.06.2004
 * Time:    02:04
 * Version: 1.0.0
 */
	/// <summary>
	/// CharacterAccessLevelToggle [<NewLevel>]
	/// CALT [<NewLevel>]
	/// Switches a player character between the different access levels if the account level is higher than a player.
	/// <NewLevel> might be Player | 0, Counselor | 1, GM | Game Master | 2, Seer | 3, Administrator | 4
	/// Without a parametet it switch between player and the account level.
	/// Nice to test something with different access permission.
	/// For a normal player account it will do nothing.
	/// 
	/// Examples:
	/// CALT
	/// Switch character with a higher level to a player
	/// or switch a player access level to account level.
	/// </summary>

using System;
using Server;
using Server.Accounting;
using Server.Commands;

namespace Server.Misc
{

	public class CharacterAccessLevelToggle
	{
    public static void Initialize()
    {
        CommandSystem.Register("GMToggle", AccessLevel.Player, new CommandEventHandler(CharacterAccessLevelToggle_OnCommand));
  	}

		[Usage( "GMToggle [<NewLevel>]" )]
		[Description( "Switches a player character between the different access levels if"
		             + " the account level is higher than a player. "
		             + "<NewLevel> might be Player | 0, Counselor | 1, Game Master | 2, Seer | 3, Administrator | 4" )]
    public static void CharacterAccessLevelToggle_OnCommand( CommandEventArgs e )
		{		  
		  try
		  {
		    Mobile m_Mob = e.Mobile;
		    AccessLevel al_MobLevel = m_Mob.AccessLevel;
		    Account a_Account = (Account)m_Mob.Account;
		    AccessLevel al_AccLevel = a_Account.AccessLevel;
		    if ( !m_Mob.Player || al_AccLevel == AccessLevel.Player )
		    {
          e.Mobile.SendMessage( "You no not have access to that command.");
		      return;
		    }
		    
		    AccessLevel al_NewLevel;
		    switch (e.Length)
		    {
		      case 1:
		        switch ( e.GetString(0).ToLower() )
		        {
				      case "player":
		            al_NewLevel = AccessLevel.Player;
				        break;
				      case "0":
		            al_NewLevel = AccessLevel.Player;
				        break;
				      case "counselor":
		            al_NewLevel = AccessLevel.Counselor;
				        break;
				      case "1":
		            al_NewLevel = AccessLevel.Counselor;
				        break;
				      case "game master":
		            al_NewLevel = AccessLevel.GameMaster;
				        break;
				      case "gm":
		            al_NewLevel = AccessLevel.GameMaster;
				        break;
				      case "2":
		            al_NewLevel = AccessLevel.GameMaster;
				        break;
				      case "seer":
		            al_NewLevel = AccessLevel.Seer;
				        break;
				      case "3":
		            al_NewLevel = AccessLevel.Seer;
				        break;
				      case "administrator":
		            al_NewLevel = AccessLevel.Administrator;
				        break;
				      case "4":
		            al_NewLevel = AccessLevel.Administrator;
				        break;
		          default:
                e.Mobile.SendMessage( "Wrong Parameter: " + e.GetString(0));
		            return;
		        }
		        break;
		      case 0:
		        if (al_MobLevel == AccessLevel.Player)
		        {
		          al_NewLevel = al_AccLevel;
		        }
		        else
		        {
		          al_NewLevel = AccessLevel.Player;
		        }
		        break;
		      default:
            e.Mobile.SendMessage( "Usage: GMToggle [<NewLevel>]" );
		        return;
		    }

        if ( al_NewLevel > al_AccLevel )
        {
            e.Mobile.SendMessage( "Your account level is only " + al_AccLevel);
            return;
        }
        
        if ( al_NewLevel == al_MobLevel )
        {
            e.Mobile.SendMessage( "Your access level is already " + al_MobLevel);
            return;
        }

        m_Mob.AccessLevel = al_NewLevel;
		  }
      catch (Exception err)
      {
        e.Mobile.SendMessage( "Exception: " + err.Message );
      }
		}
	}
}
