#region References
using Server.Network;
#endregion

namespace Server
{
	public enum EffectLayer
	{
		Head = 0,
		RightHand = 1,
		LeftHand = 2,
		Waist = 3,
		LeftFoot = 4,
		RightFoot = 5,
		CenterFeet = 7
	}

	public enum ParticleSupportType
	{
		Full,
		Detect,
		None
	}

	public static class Effects
	{
		public static ParticleSupportType ParticleSupportType { get; set; } = ParticleSupportType.Detect;

		public static bool SendParticlesTo(NetState state)
		{
			if (ParticleSupportType == ParticleSupportType.Full)
			{
				return true;
			}

			if (ParticleSupportType == ParticleSupportType.Detect)
			{
				return state.IsUOTDClient || state.IsSAClient || state.IsEnhancedClient;
			}

			return false;
		}

		public static void PlayExplodeSound(IPoint3D p, Map map)
		{
			PlaySound(p, map, Utility.RandomList(283, 284, 285, 286, 519, 773, 774, 775, 776, 777, 1231));
		}

		public static void PlaySound(IPoint3D p, Map map, int soundID)
		{
			if (soundID <= -1)
			{
				return;
			}

			if (map != null)
			{
				Packet playSound = null;

				var eable = map.GetClientsInRange(new Point3D(p));

				foreach (var state in eable)
				{
					state.Mobile?.ProcessDelta();

					if (playSound == null)
					{
						playSound = Packet.Acquire(new PlaySound(soundID, p));
					}

					state.Send(playSound);
				}

				eable.Free();

				Packet.Release(playSound);
			}
		}

		public static void SendBoltEffect(IEntity e)
		{
			SendBoltEffect(e, true, 0);
		}

		public static void SendBoltEffect(IEntity e, bool sound)
		{
			SendBoltEffect(e, sound, 0);
		}

		public static void SendBoltEffect(IEntity e, int hue)
		{
			SendBoltEffect(e, true, hue);
		}

		public static void SendBoltEffect(IEntity e, bool sound, int hue, bool delay)
		{
			if (delay)
			{
				Timer.DelayCall(() => SendBoltEffect(e, sound, hue));
			}
			else
			{
				SendBoltEffect(e, sound, hue);
			}
		}

		public static void SendBoltEffect(IEntity e, bool sound, int hue)
		{
			var map = e.Map;

			if (map == null)
			{
				return;
			}

			e.ProcessDelta();

			Packet preEffect = null, postEffect = null, boltEffect = null, playSound = null;

			var eable = map.GetClientsInRange(e.Location);

			foreach (var state in eable)
			{
				var mobile = state.Mobile;

				if (mobile != null && mobile.CanSee(e))
				{
					var sendParticles = SendParticlesTo(state);

					if (sendParticles)
					{
						if (preEffect == null)
						{
							preEffect = Packet.Acquire(new TargetParticleEffect(e, 0, 10, 5, 0, 0, 5031, 3, 0));
						}

						state.Send(preEffect);
					}

					if (boltEffect == null)
					{
						if (hue == 0)
						{
							boltEffect = Packet.Acquire(new BoltEffectNew(e));
						}
						else
						{
							boltEffect = Packet.Acquire(new BoltEffect(e, hue));
						}
					}

					state.Send(boltEffect);

					if (sendParticles)
					{
						if (postEffect == null)
						{
							postEffect = Packet.Acquire(new GraphicalEffect(EffectType.FixedFrom, e.Serial, Serial.Zero, 0, e.Location, e.Location, 0, 0, false, 0));
						}

						state.Send(postEffect);
					}

					if (sound)
					{
						if (playSound == null)
						{
							playSound = Packet.Acquire(new PlaySound(0x29, e));
						}

						state.Send(playSound);
					}
				}
			}

			eable.Free();

			Packet.Release(preEffect);
			Packet.Release(postEffect);
			Packet.Release(boltEffect);
			Packet.Release(playSound);
		}

		public static void SendBoltEffect(NetState state, IEntity e)
		{
			SendBoltEffect(state, e, true);
		}

		public static void SendBoltEffect(NetState state, IEntity e, bool sound)
		{
			SendBoltEffect(state, e, sound, 0);
		}

		public static void SendBoltEffect(NetState state, IEntity e, int hue)
		{
			SendBoltEffect(state, e, true, hue);
		}

		public static void SendBoltEffect(NetState state, IEntity e, bool sound, int hue)
		{
			if (SendParticlesTo(state))
			{
				state.Send(new TargetParticleEffect(e, 0, 10, 5, hue, 0, 5031, 3, 0));
			}

			state.Send(new BoltEffect(e, hue));
			state.Send(new TargetEffect(e, 0x375A, 10, 30, hue, 2));

			if (sound)
			{
				state.Send(new PlaySound(0x29, e));
			}
		}

		public static void SendLocationEffect(IPoint3D p, Map map, int itemID, int duration)
		{
			SendLocationEffect(p, map, itemID, duration, 10, 0, 0);
		}

		public static void SendLocationEffect(IPoint3D p, Map map, int itemID, int duration, int speed)
		{
			SendLocationEffect(p, map, itemID, duration, speed, 0, 0);
		}

		public static void SendLocationEffect(IPoint3D p, Map map, int itemID, int duration, int hue, int renderMode)
		{
			SendLocationEffect(p, map, itemID, duration, 10, hue, renderMode);
		}

		public static void SendLocationEffect(IPoint3D p, Map map, int itemID, int duration, int speed, int hue, int renderMode)
		{
			SendPacket(p, map, new LocationEffect(p, itemID, speed, duration, hue, renderMode));
		}

		public static void SendLocationParticles(IEntity e, int itemID, int speed, int duration, int effect)
		{
			SendLocationParticles(e, itemID, speed, duration, 0, 0, effect, 0);
		}

		public static void SendLocationParticles(IEntity e, int itemID, int speed, int duration, int effect, int unknown)
		{
			SendLocationParticles(e, itemID, speed, duration, 0, 0, effect, unknown);
		}

		public static void SendLocationParticles(IEntity e, int itemID, int speed, int duration, int hue, int renderMode, int effect, int unknown)
		{
			var map = e.Map;

			if (map != null)
			{
				Packet particles = null, regular = null;

				var eable = map.GetClientsInRange(e.Location);

				foreach (var state in eable)
				{
					state.Mobile?.ProcessDelta();

					if (SendParticlesTo(state))
					{
						if (particles == null)
						{
							particles = Packet.Acquire(new LocationParticleEffect(e, itemID, speed, duration, hue, renderMode, effect, unknown));
						}

						state.Send(particles);
					}
					else if (itemID != 0)
					{
						if (regular == null)
						{
							regular = Packet.Acquire(new LocationEffect(e, itemID, speed, duration, hue, renderMode));
						}

						state.Send(regular);
					}
				}

				eable.Free();

				Packet.Release(particles);
				Packet.Release(regular);
			}
		}

		public static void SendTargetEffect(IEntity target, int itemID, int duration)
		{
			SendTargetEffect(target, itemID, duration, 0, 0);
		}

		public static void SendTargetEffect(IEntity target, int itemID, int speed, int duration)
		{
			SendTargetEffect(target, itemID, speed, duration, 0, 0);
		}

		public static void SendTargetEffect(IEntity target, int itemID, int duration, int hue, int renderMode)
		{
			SendTargetEffect(target, itemID, 10, duration, hue, renderMode);
		}

		public static void SendTargetEffect(IEntity target, int itemID, int speed, int duration, int hue, int renderMode)
		{
			if (target is Mobile mt)
			{
				mt.ProcessDelta();
			}

			SendPacket(target.Location, target.Map, new TargetEffect(target, itemID, speed, duration, hue, renderMode));
		}

		public static void SendTargetParticles(IEntity target, int itemID, int speed, int duration, int effect, EffectLayer layer)
		{
			SendTargetParticles(target, itemID, speed, duration, 0, 0, effect, layer, 0);
		}

		public static void SendTargetParticles(IEntity target, int itemID, int speed, int duration, int effect, EffectLayer layer, int unknown)
		{
			SendTargetParticles(target, itemID, speed, duration, 0, 0, effect, layer, unknown);
		}

		public static void SendTargetParticles(IEntity target, int itemID, int speed, int duration, int hue, int renderMode, int effect, EffectLayer layer, int unknown)
		{
			if (target is Mobile m)
			{
				m.ProcessDelta();
			}

			var map = target.Map;

			if (map != null)
			{
				Packet particles = null, regular = null;

				var eable = map.GetClientsInRange(target.Location);

				foreach (var state in eable)
				{
					state.Mobile?.ProcessDelta();

					if (SendParticlesTo(state))
					{
						if (particles == null)
						{
							particles = Packet.Acquire(new TargetParticleEffect(target, itemID, speed, duration, hue, renderMode, effect, (int)layer, unknown));
						}

						state.Send(particles);
					}
					else if (itemID != 0)
					{
						if (regular == null)
						{
							regular = Packet.Acquire(new TargetEffect(target, itemID, speed, duration, hue, renderMode));
						}

						state.Send(regular);
					}
				}

				eable.Free();

				Packet.Release(particles);
				Packet.Release(regular);
			}
		}

		public static void SendMovingEffect(IEntity from, IEntity to, int itemID, int speed, int duration, bool fixedDirection, bool explodes)
		{
			SendMovingEffect(from, to, itemID, speed, duration, fixedDirection, explodes, 0, 0);
		}

		public static void SendMovingEffect(IEntity from, IEntity to, int itemID, int speed, int duration, bool fixedDirection, bool explodes, int hue, int renderMode)
		{
			if (from is Mobile mf)
			{
				mf.ProcessDelta();
			}

			if (to is Mobile mt)
			{
				mt.ProcessDelta();
			}

			SendPacket(from.Location, from.Map, new MovingEffect(from, to, itemID, speed, duration, fixedDirection, explodes, hue, renderMode));
		}

		public static void SendMovingParticles(IEntity from, IEntity to, int itemID, int speed, int duration, bool fixedDirection, bool explodes, int effect, int explodeEffect, int explodeSound)
		{
			SendMovingParticles(from, to, itemID, speed, duration, fixedDirection, explodes, 0, 0, effect, explodeEffect, explodeSound, 0);
		}

		public static void SendMovingParticles(IEntity from, IEntity to, int itemID, int speed, int duration, bool fixedDirection, bool explodes, int effect, int explodeEffect, int explodeSound, int unknown)
		{
			SendMovingParticles(from, to, itemID, speed, duration, fixedDirection, explodes, 0, 0, effect, explodeEffect, explodeSound, unknown);
		}

		public static void SendMovingParticles(IEntity from, IEntity to, int itemID, int speed, int duration, bool fixedDirection, bool explodes, int hue, int renderMode, int effect, int explodeEffect, int explodeSound, int unknown)
		{
			SendMovingParticles(from, to, itemID, speed, duration, fixedDirection, explodes, hue, renderMode, effect, explodeEffect, explodeSound, (EffectLayer)255, unknown);
		}

		public static void SendMovingParticles(IEntity from, IEntity to, int itemID, int speed, int duration, bool fixedDirection, bool explodes, int hue, int renderMode, int effect, int explodeEffect, int explodeSound, EffectLayer layer, int unknown)
		{
			if (from is Mobile mf)
			{
				mf.ProcessDelta();
			}

			if (to is Mobile mt)
			{
				mt.ProcessDelta();
			}

			var map = from.Map;

			if (map != null)
			{
				Packet particles = null, regular = null;

				var eable = map.GetClientsInRange(from.Location);

				foreach (var state in eable)
				{
					state.Mobile?.ProcessDelta();

					if (SendParticlesTo(state))
					{
						if (particles == null)
						{
							particles = Packet.Acquire(new MovingParticleEffect(from, to, itemID, speed, duration, fixedDirection, explodes, hue, renderMode, effect, explodeEffect, explodeSound, layer, unknown));
						}

						state.Send(particles);
					}
					else if (itemID > 1)
					{
						if (regular == null)
						{
							regular = Packet.Acquire(new MovingEffect(from, to, itemID, speed, duration, fixedDirection, explodes, hue, renderMode));
						}

						state.Send(regular);
					}
				}

				eable.Free();

				Packet.Release(particles);
				Packet.Release(regular);
			}

			//SendPacket( from.Location, from.Map, new MovingParticleEffect( from, to, itemID, speed, duration, fixedDirection, explodes, hue, renderMode, effect, explodeEffect, explodeSound, unknown ) );
		}

		public static void SendPacket(Point3D origin, Map map, Packet p)
		{
			if (map != null)
			{
				Packet.Acquire(p);

				var eable = map.GetClientsInRange(origin);

				foreach (var state in eable)
				{
					state.Mobile?.ProcessDelta();
					state.Send(p);
				}

				eable.Free();

				Packet.Release(p);
			}
		}

		public static void SendPacket(IPoint3D origin, Map map, Packet p)
		{
			if (map != null)
			{
				Packet.Acquire(p);

				var eable = map.GetClientsInRange(new Point3D(origin));

				foreach (var state in eable)
				{
					state.Mobile?.ProcessDelta();
					state.Send(p);
				}

				eable.Free();

				Packet.Release(p);
			}
		}
	}
}
