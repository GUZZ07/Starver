using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starvers.PlayerBoosts.Skills
{
	
	using Microsoft.Xna.Framework;

	public class MirrorMana : StarverSkill
	{
		public MirrorMana()
		{
			LevelNeed = 15;
			CD = 60 * 40;
			MPCost = 35;
			Author = "zhou_Qi";
			Description = @"将你带回到最初始的位置，恢复少许MP与HP
""通过镜面折射出的映像进行传送，这一魔法工艺早已失传
现今的工匠们仅能通过复刻魔镜上的铭文来再现这一奇迹""";
		}
		public override void Release(StarverPlayer player, Vector vel)
		{
			player.TSPlayer.Spawn(Terraria.PlayerSpawnContext.RecallFromItem);
			// player.MP += player.MaxMP / 3;
			player.Heal(player.LifeMax / 3);
		}
	}
}
