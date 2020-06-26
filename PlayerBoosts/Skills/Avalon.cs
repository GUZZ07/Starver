using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.GameContent.NetModules;
using Terraria.ID;
using Terraria.Net;
using TShockAPI;

namespace Starvers.PlayerBoosts.Skills
{
	public class Avalon : StarverSkill
	{
		public Avalon() 
		{
			MPCost = 30;
			CD = 60 * 30;
			Description = "幻想乡，这个技能可以给予你5s的伪无敌,\n随后附加多种回血buff,苟命专用";
			Author = "三叶草";
			LevelNeed = 10;
		}
		public override void Release(StarverPlayer player, Vector vel)
		{
			var power = CreativePowerManager.Instance.GetPower<CreativePowers.GodmodePower>();
			power.SetEnabledState(player.Index, true);
			//var powerID = power.PowerId; // 5
			//var packet = NetCreativePowersModule.PreparePacket(powerID, 1);
			//packet.Writer.Write(true);
			//NetManager.Instance.SendData(Netplay.Clients[player.Index].Socket, packet);
			AsyncRelease(player);
		}
		private async void AsyncRelease(StarverPlayer player)
		{
			await Task.Run(() =>
			{
				try
				{
					Thread.Sleep(5000);
					var power = CreativePowerManager.Instance.GetPower<CreativePowers.GodmodePower>();
					power.SetEnabledState(player.Index, false);
					player.SetBuff(BuffID.RapidHealing, 10 * 60);
					player.SetBuff(BuffID.NebulaUpLife3, 10 * 60);
				}
				catch(Exception e)
				{
					TSPlayer.Server.SendErrorMessage(e.ToString());
				}
			});
		}
	}
}
