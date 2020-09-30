using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Starvers.PlayerBoosts.Skills
{
	public class RealmOfDefense : StarverSkill
	{
		public RealmOfDefense()
		{
			MPCost = 500;
			CD = 60 * 120;
			Description = @"制造一个能弹开敌对攻击的结界";
			Author = "1413";
			LevelNeed = 6000;
			ForceCD = true;
			Summary = "[6000][击败月球领主解锁]制造一个能反弹一切威胁的结界";
		}
		public override bool CanSet(StarverPlayer player)
		{
			player.SendBlueText("该技能还没做好");
			return false;
		}
		public override void Release(StarverPlayer player, Vector vel)
		{
			//var realm = new ReflectingRealm(player)
			//{
			//	DefaultTimeLeft = 60 * 60,
			//	Radium = 16 * 30,
			//	Center = player.Center
			//};
			//Starver.Instance.Aura.AddRealm(realm);
		}
	}
}
