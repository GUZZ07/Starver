using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace Starvers.AuraSystem.Realms
{
	public class AnalogItem : RectRealm
	{
		private int itemIndex = 400;
		private int id;
		private int stack;
		public Data16 ExtraData;
		public int Index => itemIndex;
		public Item RealItem
		{
			get => Main.item[itemIndex];
		}
		public new int? TimeLeft
		{
			get;
			set;
		}
		public int ID
		{
			get
			{
				return id;
			}
			set
			{
				id = value;
				RealItem.netDefaults(id);
				StarverPlayer.All.SendData(PacketTypes.UpdateItemDrop, "", itemIndex);
				SetSize();
			}
		}
		public int Stack
		{
			get
			{
				return stack;
			}
			set
			{
				RealItem.stack = stack;
				StarverPlayer.All.SendData(PacketTypes.UpdateItemDrop, "", itemIndex);
			}
		}
		public bool LockPos
		{
			get;
			set;
		}
		public bool LockStack
		{
			get;
			set;
		}
		public float? WidthOverride { get; set; }
		public float? HeightOverride { get; set; }
		/// <summary>
		/// 消失后是否重新生成
		/// </summary>
		public bool Lock
		{
			get;
			set;
		}

		public AnalogItem(int id, int stack = 1, int? timeLeft = null) : base(false)
		{
			this.id = id;
			this.stack = stack;
			this.TimeLeft = timeLeft;
		}

		/// <summary>
		/// 清除这个物品
		/// </summary>
		public override void Kill()
		{
			RealItem.netDefaults(0);
			RealItem.active = false;
			StarverPlayer.All.SendData(PacketTypes.UpdateItemDrop, "", itemIndex);
			base.Kill();
		}
		protected override void InternalUpdate()
		{
			if (TimeLeft.HasValue)
			{
				if (--TimeLeft == 0)
				{
					Kill();
					return;
				}
			}
			if (!RealItem.active)
			{
				if (Lock)
				{
					itemIndex = Utils.NewItem(Center, id, stack);
				}
				else
				// if (id == ItemID.FallenStar && Main.dayTime)
				{
					Kill();
					return;
				}
			}
			else if (!LockStack)
			{
				stack = RealItem.stack;
			}
			if (LockPos)
			{
				RealItem.Center = Center;
				StarverPlayer.All.SendData(PacketTypes.UpdateItemDrop, "", itemIndex);
			}
			else
			{
				Center = RealItem.Center;
			}
			foreach (var player in Starver.Players)
			{
				if (player == null || !player.Active)
				{
					continue;
				}
				if (HasOverlapping(player))
				{
					player.OnPickAnalogItem(this);
					if (!Active)
					{
						return;
					}
				}
			}
			RealItem.keepTime = 60 * 60;
		}
		protected override void SetDefault()
		{
			if (itemIndex == 400)
			{
				itemIndex = Utils.NewItem(Center, ID, stack);
				RealItem.keepTime = 60 * 60;
				//S
				SetSize();
			}
		}
		protected void SetSize()
		{
			(Width, Height) = (16 * 6 + RealItem.width, 16 * 6 + RealItem.height);
			Width = WidthOverride ?? Width;
			Height = HeightOverride ?? Height;
		}
	}
}
