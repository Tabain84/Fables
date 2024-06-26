using System;

using Server.Engines.Quests;
using Server.Items;
using Server.Mobiles;

namespace Server.Multis
{
	public class RatCamp : BaseCamp
	{
		public virtual Mobile Ratmen => new Ratman();

		[CommandProperty(AccessLevel.GameMaster)]
		public override TimeSpan DecayDelay => TimeSpan.FromMinutes(5.0);

		[Constructable]
		public RatCamp()
			: base(0x1F6D)// dummy garbage at center
		{
		}

		public RatCamp(Serial serial)
			: base(serial)
		{
		}

		public override void AddComponents()
		{
			Visible = false;

			AddItem(new Static(0x10ee), 0, 0, 0);
			AddItem(new Static(0xfac), 0, 6, 0);

			switch (Utility.Random(3))
			{
				case 0:
				{
					AddItem(new Item(0xDE3), 0, 6, 0); // Campfire
					AddItem(new Item(0x974), 0, 6, 1); // Cauldron
					break;
				}
				case 1:
				{
					AddItem(new Item(0x1E95), 0, 6, 1); // Rabbit on a spit
					break;
				}
				default:
				{
					AddItem(new Item(0x1E94), 0, 6, 1); // Chicken on a spit
					break;
				}
			}

			AddItem(new Item(0x41F), 5, 5, 0); // Gruesome Standart South

			AddCampChests();

			for (var i = 0; i < 4; i++)
			{
				AddMobile(Ratmen, Utility.RandomMinMax(-7, 7), Utility.RandomMinMax(-7, 7), 0);
			}

			switch (Utility.Random(2))
			{
				default:
				case 0: Prisoner = new EscortableNoble(); break;
				case 1: Prisoner = new EscortableSeekerOfAdventure(); break;
			}

			Prisoner.IsPrisoner = true;
			Prisoner.CantWalk = true;

			Prisoner.YellHue = Utility.RandomList(0x57, 0x67, 0x77, 0x87, 0x117);

			AddMobile(Prisoner, Utility.RandomMinMax(-2, 2), Utility.RandomMinMax(-2, 2), 0);
		}

		public override void OnEnter(Mobile m)
		{
			if (m.Player && Prisoner?.Deleted == false && Prisoner.CantWalk)
			{
				int number;

				switch (Utility.Random(8))
				{
					case 0: number = 502261; break; // HELP!
					case 1: number = 502262; break; // Help me!
					case 2: number = 502263; break; // Canst thou aid me?!
					case 3: number = 502264; break; // Help a poor prisoner!
					case 4: number = 502265; break; // Help! Please!
					case 5: number = 502266; break; // Aaah! Help me!
					case 6: number = 502267; break; // Go and get some help!
					default: number = 502268; break; // Quickly, I beg thee! Unlock my chains! If thou dost look at me close thou canst see them.	
				}

				Prisoner.Yell(number);
			}
		}

		public override void AddItem(Item item, int xOffset, int yOffset, int zOffset)
		{
			if (item?.Deleted == false)
			{
				item.Movable = false;
			}

			base.AddItem(item, xOffset, yOffset, zOffset);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(2); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var version = reader.ReadInt();

			switch (version)
			{
				case 2: break;
				case 1:
				{
					Prisoner = reader.ReadMobile<BaseCreature>();
					break;
				}
				case 0:
				{
					Prisoner = reader.ReadMobile<BaseCreature>();
					_ = reader.ReadItem();
					break;
				}
			}
		}
	}
}
