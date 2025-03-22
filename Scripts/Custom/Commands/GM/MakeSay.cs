using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using Server.Commands;

namespace Server.Misc
{

	public class MakeSay
	{

		private class InternalTarget : Target
		{

			private string m_TextToSay;

			public InternalTarget( string TextToSay )
				: base( 20, true, TargetFlags.None )
			{
				m_TextToSay = TextToSay;
			}

			protected override void OnTarget( Mobile from, object target )
			{
				if ( target != null )
					if ( target is Mobile ) {

						if ( target is PlayerMobile ) {

							PlayerMobile targ = target as PlayerMobile;

							if ( from != targ && from.AccessLevel > targ.AccessLevel ) {
								CommandLogging.WriteLine( from, "{0} {1} forcing speech on {2}: {3}", from.AccessLevel, CommandLogging.Format( from ), CommandLogging.Format( targ ), m_TextToSay );
								( (Mobile)target ).Say( m_TextToSay );
							}
						}
						else
							( (Mobile)target ).Say( m_TextToSay );
					}

					else if ( target is Item )
						( (Item)target ).PublicOverheadMessage( 0, 49, true, m_TextToSay );
					else
						from.SendMessage( MessageUtil.MessageColorGM, "You must target a mobile or item" );
			}
		}

		public static void Initialize()
		{
			CommandSystem.Register( "MakeSay", AccessLevel.GameMaster, new CommandEventHandler( EventCommand_MakeSay ) );
			CommandSystem.Register( "MS", AccessLevel.GameMaster, new CommandEventHandler( EventCommand_MakeSay ) );
		}

		public static void EventCommand_MakeSay( CommandEventArgs e )
		{
			string toSay = e.ArgString.Trim();

			if ( toSay.Length > 0 ) {
				e.Mobile.Target = new InternalTarget( e.ArgString );
				e.Mobile.SendMessage( MessageUtil.MessageColorGM, "Target what you would like to say this." );
			}
			else
				e.Mobile.SendMessage( MessageUtil.MessageColorGM, "Format: .makesay \"<text>\"" );

		}
	}

}