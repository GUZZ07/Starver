using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace Starvers.Enemies.Bosses
{
	public class EyeEx : StarverBoss
	{
		protected int Target
		{
			get => TNPC.target;
			set => TNPC.target = value;
		}
		protected StarverPlayer TargetPlayer
		{
			get => Starver.Instance.Players[Target];
		}
		public EyeEx() : base(NPCID.EyeofCthulhu)
		{
			defLifes = 3;
			defLife = 45000;
			defDefense = 1000;
		}

		public override void Spawn(Vector2 position, int level = 2000)
		{
			base.Spawn(position, level);
		}

		protected override void RealAI()
		{
			base.RealAI();
		}
	}
}
