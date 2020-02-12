using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Starvers.AuraSystem.Skills;
using Starvers.AuraSystem.Skills.Base;
using Starvers.Events;
using TShockAPI;

namespace Starvers.AuraSystem
{
	public static class SkillManager
	{
		#region Skills
		public static Skill[] Skills { get; private set; }
#if false
			= new Skill[]
		{
			new Avalon(),
			new ExCalibur(),
			new FireBall(),
			new Musket(),
			new GaeBolg(),
			new EnderWand(),
			new WindRealm(),
			new Whirlwind(),
			new Shuriken(),
			new SpiritStrike(),
			new AvalonGradation(),
			new LawAias(),
			new LimitlessSpark(),
			new MagnetStorm(),
			new FlameBurning(),
			new JusticeFromSky(),
			new TrackingMissile(),
			new PosionFog(),
			new CDLess(),
			new NStrike(),
			new Sacrifice(),
			new TheWorld(),
			new MirrorMana(),
			new Chaos(),
			new Cosmos(),
			new ChordMana(),
			new FromHell(),
			new StarEruption(),
			new NatureRage(),
			new NatureGuard(),
			new FishingRod(),
			new AlcoholFeast(),
			new GreenWind(),
			new FrozenCraze(),
			new LimitBreak(),
			new MiracleMana(),
			new UltimateSlash(),
			new UniverseBlast(),
			new NatureStorm(),
			new UnstableTele(),
			new GreenCrit()
		};
#endif
		#endregion
		#region LoadSkills
		public static void LoadSkills()
		{
			var members = typeof(SkillIDs).GetEnumNames();
			Skills = new Skill[members.Length];
			var ass = typeof(Skill).Assembly;
			for (int i = 0; i < members.Length; i++)
			{
				Skills[i] = Activator.CreateInstance(ass.GetType($"Starvers.AuraSystem.Skills.{members[i]}")) as Skill;
			}
		}
		#endregion
		#region Handle
		public static void Handle(StarverPlayer player, Vector2 vel, int slot)
		{
			try
			{
				slot -= 1;

				var skill = Skills[player.Skills[slot]];

				ReleaseSkillEventArgs args = new ReleaseSkillEventArgs
				{
					Slot = slot,
					SkillID = (SkillIDs)player.Skills[slot],
					Banned = skill.BossBan && BossSystem.Bosses.Base.StarverBoss.AliveBoss > 0,
					MPCost = skill.MP,
					CD = skill.CD
				};

				player.ReleasingSkill(args);

				skill = Skills[(int)args.SkillID];

				if (args.Banned)
				{
					player.SendCombatMSsg("该技能已被禁用", Color.Pink);
				}
				else if (args.MPCost > player.MP)
				{
					player.SendCombatMSsg("MP不足", Color.Pink);
				}
				else if (player.ForceIgnoreCD == false && (player.IgnoreCD == false || skill.ForceCD) && player.CDs[slot] > 0)
				{
					player.SendCombatMSsg("技能冷却中", Color.Pink);
				}
				else
				{
					skill.Release(player, vel);
					if (skill.ForceCD || !player.IgnoreCD)
					{
						player.CDs[slot] += args.CD;
					}
					player.MP -= args.MPCost;
					player.LastSkill = (int)args.SkillID;
				}

				player.ReleasedSkill(args);
			}
			catch (Exception e)
			{
				TSPlayer.Server.SendErrorMessage(e.ToString());
				player.SendMessage($"技能使用失败, 原因请查询日志", Color.Red);
				TShock.Log.Info(e.ToString());
			}
		}
		#endregion
		#region GetSlot
		public static int GetSlotByItemID(int type)
		{
			for (int i = 0; i < StarverAuraManager.SkillSlot.Length; i++)
			{
				if (type == StarverAuraManager.SkillSlot[i].Item)
				{
					return i + 1;
				}
			}
			return 0;
		}
		public static int GetSlot(int type)
		{
			for (int i = 0; i < StarverAuraManager.SkillSlot.Length; i++)
			{
				if (type == StarverAuraManager.SkillSlot[i].Proj)
				{
					return i + 1;
				}
			}
			return 0;
		}
		#endregion
	}
}
