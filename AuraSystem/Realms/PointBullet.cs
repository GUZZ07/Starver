using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Microsoft.Xna.Framework;

namespace Starvers.AuraSystem.Realms
{
	public class PointBullet : CircleRealm
	{
		private int idx;
		private Action<PointBullet> AIUpdate;
		private Action<PointBullet> Killing;
		public Data16 Datas;
		public Rectangle? Range { get; }
		public int ProjID { get; }
		public Projectile Proj
		{
			get
			{
				if (idx < 0 || idx >= Main.maxProjectiles || Main.projectile[idx].type != ProjID || !Main.projectile[idx].active)
				{
					return null;
				}
				return Main.projectile[idx];
			}
		}
		public Vector2 Velocity { get; set; }
		public Vector2 Accel { get; set; }
		public PointBullet(int projID, Rectangle? range = null, Action<PointBullet> AIupdate = null,Action<PointBullet> killing = null) : base(true)
		{
			ProjID = projID;
			Range = range;
			AIUpdate = AIupdate;
			Killing = killing;
		}
		protected override void InternalUpdate()
		{
			if (Range != null)
			{
				var range = (Rectangle)Range;
				if (
					Center.X < range.Left ||
					Center.X > range.Right ||
					Center.Y < range.Top ||
					Center.Y > range.Bottom
				)
				{
					Clear();
					return;
				}
			}
			if (Proj == null)
			{
				SpawnProj();
			}
			try
			{
				AIUpdate?.Invoke(this);
			}
			catch (Exception e)
			{
				StarverPlayer.All.SendErrorMessage(e.ToString());
				Clear();
			}
			Velocity += Accel;
			Center += Velocity;
			if (Proj.velocity != Velocity)
			{
				Proj.velocity = Velocity;
				Proj.SendData();
			}
		}
		protected override void SetDefault()
		{
			
		}
		public override void Start()
		{
			base.Start();
			SpawnProj();
		}
		public void Clear()
		{
			try
			{
				Killing?.Invoke(this);
				Proj?.KillMeEx();
			}
			finally
			{
				base.Kill();
			}
		}
		public override void Kill()
		{
			try
			{
				Killing?.Invoke(this);
				Proj.KillMeEx();
			}
			finally
			{
				base.Kill();
			}
		}
		private void SpawnProj()
		{
			idx = Utils.NewProj(Center, default, ProjID, -1, 0);
			Main.projectile[idx].aiStyle = -1;
			Main.projectile[idx].netImportant = true;
			Main.projectile[idx].tileCollide = false;
		}
	}
}
