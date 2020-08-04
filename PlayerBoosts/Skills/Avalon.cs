using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.GameContent.NetModules;
using Terraria.ID;
using Terraria.Net;
using TShockAPI;

namespace Starvers.PlayerBoosts.Skills
{
	public class Avalon : StarverSkill
	{
		private static Random rand = new Random();
		public Avalon() 
		{
			MPCost = 30;
			CD = 60 * 30;
			Description = "幻想乡，这个技能可以给予你5s的无敌,\n随后附加多种回血buff,苟命专用";
			Author = "三叶草";
			LevelNeed = 10;
		}
		public override void Release(StarverPlayer player, Vector vel)
		{
			var power = CreativePowerManager.Instance.GetPower<CreativePowers.GodmodePower>();
			power.SetEnabledState(player.Index, true);

			var sound = new NetMessage.NetSoundInfo(player.Center, (ushort)rand.Next(47, 56));
			NetMessage.PlayNetSound(sound, player.Index);

			AsyncRelease(player);
		}
		private async void AsyncRelease(StarverPlayer player)
		{
			await Task.Run(() =>
			{
				try
				{
					var life = player.Life;
					Thread.Sleep(5000);
					var power = CreativePowerManager.Instance.GetPower<CreativePowers.GodmodePower>();
					power.SetEnabledState(player.Index, false);

					player.Life = life;
					player.SetBuff(BuffID.RapidHealing, 10 * 60);
					player.SetBuff(BuffID.NebulaUpLife3, 10 * 60);

					var sound = new NetMessage.NetSoundInfo(player.Center, 19);
					NetMessage.PlayNetSound(sound);
				}
				catch(Exception e)
				{
					TSPlayer.Server.SendErrorMessage(e.ToString());
				}
			});
		}
	}
}
