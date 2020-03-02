using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Starvers.AuraSystem.Skills
{
	using Base;
	using Realms;
	public class RealmOfDefense : Skill
	{
		public RealmOfDefense() : base(SkillIDs.RealmOfDefense)
		{
			MP = 900;
			CD = 60 * 60;
			Description = @"制造一个能弹开敌对攻击的结界";
			Author = "1413";
			Level = 1000;
			SetText();
		}
		public override void Release(StarverPlayer player, Vector2 vel)
		{
			var realm = new ReflectingRealm(player)
			{
				DefaultTimeLeft = 60 * 60,
				Radium = 16 * 30,
				Center = player.Center
			};
			Starver.Instance.Aura.AddRealm(realm);
		}
	}
}
