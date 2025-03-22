using System;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.Commands
{
	public class DotCommand_Point
	{
		public static void Initialize( )
		{
			CommandSystem.Register("Point", AccessLevel.Player, new CommandEventHandler(OnCommand_Point));
		}

		[Usage("Point")]
		[Description("'Points' to the targetted location")]
		private static void OnCommand_Point(CommandEventArgs e)
		{
			PlayerMobile thePlayer = e.Mobile as PlayerMobile;
			if (thePlayer == null)
			{
				thePlayer.SendMessage(MessageUtil.MessageColorError, "Error: you're not a PlayerMobile");
				return;
			}

			e.Mobile.Target = new InternalPointTarget(thePlayer, e.Arguments);
		}

		private class InternalPointTarget : Target
		{
			PlayerMobile _Player;
			String[] _Arguments;
			public InternalPointTarget(PlayerMobile player, String[] arguments)
				: base(15, true, TargetFlags.None)
			{
				_Player = player;
				_Arguments = arguments;
			}

			protected override void OnTarget(Mobile from, object o)
			{
				string hereMessage = _Player.Name + " points here";
				if (o is Item)
				{
					Item theItem = ( o as Item );
					if (theItem.Map != _Player.Map || ( theItem.Parent == null && _Player.GetDistanceToSqrt(theItem.Location) > 25 ))
					{
						_Player.SendMessage(MessageUtil.MessageColorError, "That's too far away!");
						return;
					}

					string itemName = theItem.Name;

					if (itemName == null || itemName == String.Empty)
					{
						if (theItem.Amount > 1)
							itemName = theItem.Amount.ToString() + " " + theItem.ItemData.Name;
						else
							itemName = theItem.ItemData.Name;
					}
					else if (theItem.Amount > 1)
					{
						itemName = theItem.Amount.ToString() + " " + itemName;
					}

					string messageString = "*" + _Player.Name + " points at " + itemName + "*";

					//Ok, this is complicated, because of hidden GMs.  If it's a concealed
					//GM, only send the point to other nearly GMs
					if (_Player.Hidden)
					{
						foreach (Mobile theMob in _Player.GetMobilesInRange(25))
						{
							if (theMob.AccessLevel > AccessLevel.Player)
							{
								_Player.PrivateOverheadMessage(MessageType.Regular, 0, false, messageString, theMob.NetState);

								if (theItem.Parent == null)
									theItem.LabelTo(theMob, hereMessage);
							}
						}
					}
					else
					{
						_Player.RevealingAction();
						_Player.PublicOverheadMessage(MessageType.Regular, _Player.EmoteHue, false, messageString);

						if (theItem.Parent == null)
							theItem.PublicOverheadMessage( MessageType.Regular, _Player.EmoteHue, false, hereMessage );
					}
				}
				else if (o is Mobile)
				{
					Mobile pointedMob = ( o as Mobile );
					if (pointedMob.Map != _Player.Map || _Player.GetDistanceToSqrt(pointedMob) > 25)
					{
						_Player.SendMessage(MessageUtil.MessageColorError, "That's too far away!");
						return;
					}

					string messageString = "*" + _Player.Name + " points at " + pointedMob.Name + "*";

					//Ok, this is complicated, because of hidden GMs.  If it's a concealed
					//GM, only send the point to other nearly GMs
					if (_Player.Hidden)
					{
						foreach (Mobile theMob in _Player.GetMobilesInRange(25))
						{
							if (theMob.AccessLevel > AccessLevel.Player)
							{
								_Player.PrivateOverheadMessage( MessageType.Regular, _Player.EmoteHue, false, messageString, theMob.NetState );
								pointedMob.PrivateOverheadMessage( MessageType.Regular, _Player.EmoteHue, false, hereMessage, theMob.NetState );
							}
						}
					}
					else
					{
						_Player.RevealingAction();
						_Player.PublicOverheadMessage( MessageType.Regular, _Player.EmoteHue, false, messageString );
						pointedMob.PublicOverheadMessage( MessageType.Regular, _Player.EmoteHue, false, hereMessage );
					}

				}
				else if (o is IPoint3D)
				{
					//have to convert from iPoint3D to Point3D, then move the mobile there
					Point3D thePoint = new Point3D(o as IPoint3D);

					if (_Player.GetDistanceToSqrt(thePoint) > 25)
					{
						_Player.SendMessage(MessageUtil.MessageColorError, "That's too far away!");
						return;
					}

					TempPointerItem thePointer = new TempPointerItem();
					thePointer.Visible = !_Player.Hidden;
					thePointer.MoveToWorld(thePoint, _Player.Map);

					string messageString = "*" + _Player.Name + " points at that spot" + "*";

					//Ok, this is complicated, because of hidden GMs.  If it's a concealed
					//GM, only send the point to other nearly GMs
					if (_Player.Hidden)
					{
						foreach (Mobile theMob in _Player.GetMobilesInRange(25))
						{
							if (theMob.AccessLevel > AccessLevel.Player)
							{
								_Player.PrivateOverheadMessage( MessageType.Regular, _Player.EmoteHue, false, messageString, theMob.NetState );
								thePointer.LabelTo(theMob, hereMessage);
							}
						}
					}
					else
					{
						_Player.RevealingAction();
						_Player.PublicOverheadMessage( MessageType.Regular, _Player.EmoteHue, false, messageString );
						thePointer.PublicOverheadMessage( MessageType.Regular, _Player.EmoteHue, false, hereMessage );
					}



				}
				else
					_Player.SendMessage( MessageUtil.MessageColorError, "Canceled, or invalid location");
			}
		}
	}



	public class TempPointerItem : Item
	{
		[Constructable]
		public TempPointerItem( )
			: base(0x206E)
		{
			this.Name = "a location marker";
			this.Movable = false;

			Timer.DelayCall(TimeSpan.FromSeconds(5.0), new TimerStateCallback(TempPointerDecay), this);
		}

		public void TempPointerDecay(object pointerObject)
		{
			this.Delete();
		}

		public TempPointerItem(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();

			this.Delete();
		}
	}
}