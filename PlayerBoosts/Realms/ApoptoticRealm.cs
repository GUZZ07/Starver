using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace Starvers.PlayerBoosts.Realms
{
	using Shapes;
	/// <summary>
	/// 凋亡结界
	/// 使处于结界中的玩家不断扣血
	/// </summary>
	public class ApoptoticRealm : SRealm<IRealmShape>
	{
        public ApoptoticRealm() : this(new AnotherCircle { Radium = 16 * 30 })
        {

        }
		public ApoptoticRealm(IRealmShape shape) : base(shape)
		{
			DefTimeLeft = 60 * 20;
			shape.BorderProjID = ProjectileID.VortexVortexLightning;
			shape.DefProjTimeLeft = 60;
		}

		protected override void Initialize()
		{

		}

		protected override void InternalUpdate()
		{
			base.InternalUpdate();
			if (timer % 20 == 0)
			{
				int damage;
				foreach (var player in Starver.Instance.Players)
				{
					if (player?.Alive != true)
					{
						continue;
					}
					if (InRange(player))
					{
						damage = Math.Max(1, player.Life / 60);
						player.Life -= damage;
						player.SendCombatText(damage.ToString(), Color.Blue);
					}
				}
			}

		}
        #region AnotherCircle
        private class AnotherCircle : IRealmShape
        {
			private Vector2 projSize;
			private IRealm owner;
			private double angle;

            public int? DefProjTimeLeft
            {
				get;
				set;
            }
            public int? BorderProjID
            {
				get;
				set;
            }
            public double Rotation
            {
				get;
				set;
            }
			public float Radium
            {
				get;
				set;
            }
            #region Wasted
            public bool AtBorder(Entity entity)
            {
                throw new NotImplementedException();
            }

            public bool Intersect(Entity entity)
            {
                throw new NotImplementedException();
            }
            #endregion

            public void Begin(IRealm owner)
            {
				var proj = new Projectile();
				proj.SetDefaultsDirect((int)BorderProjID);
				projSize = proj.Size;

				this.owner = owner;
            }

            public bool InRange(Entity entity)
            {
				var relativePos = entity.Center - owner.Center;
				relativePos.X = Math.Abs(relativePos.X);
				relativePos.Y = Math.Abs(relativePos.Y);
				relativePos += entity.Size / 2 + projSize / 2;
				return relativePos.Length() < Radium;
			}

            public void Kill()
            {

            }

            public void Update(int timer)
			{
				int idx = Utils.NewProj(owner.Center + Vector.FromPolar(angle, Radium), new Vector2(0.001f, 0.001f), ProjectileID.VortexVortexLightning, 1, 20, Main.myPlayer);
				Main.projectile[idx].aiStyle = -2;
				Main.projectile[idx].timeLeft = 60;
				angle += Math.PI / 30;
			}
        }
        #endregion
    }
}
