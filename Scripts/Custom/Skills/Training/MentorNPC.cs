using System;
using System.Collections.Generic;
using System.Text;

using Server.Mobiles;
using Server.Items;
using Server.ContextMenus;

namespace Server.Training
{
	public class Mentor : Mobile
	{
		[Constructable]
		public Mentor()
		{
			InitStats( 31, 41, 51 );

			SpeechHue = Utility.RandomDyedHue();
			Title = "the Mentor";
			Hue = 1013;

			this.Body = 0x190;
			this.Name = "Pythacious Rand";
			this.CantWalk = true;
			this.Blessed = true;
			
			AddItem( new Doublet( 2680 ) );
			AddItem( new Boots( 1109 ) );
			AddItem( new LongPants( 2706 ) );
			AddItem( new FancyShirt( 2673));
			//AddItem( new HalfApron( Utility.RandomDyedHue() ) );

			Utility.AssignRandomHair( this );

			Container pack = new Backpack();

			//pack.DropItem( new Gold( 250, 300 ) );

			pack.Movable = false;

			AddItem( pack );
		}

		public override bool ClickTitle { get { return true; } }


		public Mentor( Serial serial )
			: base( serial )
		{
		}

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			if ( from.Alive )
				list.Add( new TrainEntry( from, this ) );

			base.GetContextMenuEntries( from, list );
		}


		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 ); // version 
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
	public class TrainEntry : ContextMenuEntry
	{
		private Mobile m_Mentor;

		public TrainEntry( Mobile from, Mobile mentor )
			: base( 6146, 2 )
		{
			m_Mentor = mentor;
		}

		public override void OnClick()
		{
			if ( !Owner.From.CheckAlive() )
				return;

			if ( Owner.From.HasGump( typeof( TrainGump ) ) )
				Owner.From.CloseGump( typeof( TrainGump ) );

			m_Mentor.Say( "Allow me to guide your learning." );

			int spendingExp = 0;
			SkillName toTrain = SkillName.TasteID;

			object[] state = new object[2];

			state[0] = spendingExp;
			state[1] = toTrain;

			Owner.From.SendGump( new TrainGump( Owner.From as PlayerMobile, state ) );
		}
	}
}