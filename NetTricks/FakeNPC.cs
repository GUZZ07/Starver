using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Microsoft.Xna.Framework;
using System.IO;

namespace Starvers.NetTricks
{
	public class FakeNPC : NetTrick
	{
		public byte Index { get; set; }
		public short NetID { get; set; }
		public Vector2 Position { get; set; }
		public Vector2 Velocity { get; set; }
		public int Target { get; set; }
		public int Life { get; set; }
		public byte Direction { get; set; }
		public byte DirectionY { get; set; }
		public byte SpriteDirection { get; set; }

		public bool IsLifeMax { get; set; }

		public float?[] NPCai { get; }

		public FakeNPC()
		{
			NPCai = new float?[NPC.maxAI];
		}
		protected override byte[] SerializeDatas()
		{
			#region Initialize Writer
			using var stream = new MemoryStream(30);
			using var writer = new BinaryWriter(stream);

			var pos = stream.Position;
			stream.Position += 2;
			#endregion
			writer.Write((byte)PacketTypes.NpcUpdate);
			writer.Write((short)Index);
			writer.WriteVector2(Position);
			writer.WriteVector2(Velocity);
			writer.Write((ushort)Target);
			#region WriteBB
			BitsByte bb = default;
			bb[0] = Direction > 0;
			bb[1] = DirectionY > 0;
			bb[2] = NPCai[0].HasValue;
			bb[3] = NPCai[1].HasValue;
			bb[4] = NPCai[2].HasValue;
			bb[5] = NPCai[3].HasValue;
			bb[6] = SpriteDirection > 0;
			bb[7] = IsLifeMax;

			BitsByte bb2 = 0;
			bb2[0] = false;
			bb2[1] = true;
			bb2[2] = false;

			writer.Write(bb);
			writer.Write(bb2);
			#endregion
			#region WriteAI
			foreach (var ai in NPCai)
			{
				if (ai != null)
				{
					writer.Write((float)ai);
				}
			}
			#endregion
			#region WriteID
			writer.Write(NetID);
			#endregion
			#region WriteLife
			if (!IsLifeMax)
			{
				if (Life <= byte.MaxValue)
				{
					writer.Write((byte)sizeof(byte));
					writer.Write((byte)Life);
				}
				if (byte.MaxValue < Life && Life <= short.MaxValue)
				{
					writer.Write((byte)sizeof(short));
					writer.Write((short)Life);
				}
				else
				{
					writer.Write((byte)sizeof(int));
					writer.Write(Life);
				}
			}
			#endregion
			#region WriteReleaseOwner
			if (0 <= NetID && NetID < Main.npcCatchable.Length && Main.npcCatchable[NetID])
			{
				writer.Write((byte)Main.myPlayer);
			}
			#endregion
			#region WriteLength
			var posNow = stream.Position;
			stream.Position = pos;
			writer.Write((short)posNow);
			stream.Position = posNow;
			#endregion
			return stream.ToArray();
		}
	}
}
