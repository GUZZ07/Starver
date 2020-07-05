using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Starvers.NetTricks
{
	public abstract class NetTrick
	{
		public bool[] NoBroadCast { get; }

		protected NetTrick()
		{
			NoBroadCast = new bool[Netplay.Clients.Length];
		}

		protected abstract byte[] SerializeDatas();
		public virtual void SendData(int client = -1)
		{
			var bytes = SerializeDatas();
			if (client == -1)
			{
				for (int i = 0; i < Netplay.Clients.Length; i++)
				{
					if (!NoBroadCast[i] && Netplay.Clients[i].IsConnected())
					{
						Netplay.Clients[i].Socket.AsyncSend(bytes,0,bytes.Length, Netplay.Clients[i].ServerWriteCallBack);
					}
				}
			}
			else
			{
				if (!NoBroadCast[client] && Netplay.Clients[client].IsConnected())
				{
					Netplay.Clients[client].Socket.AsyncSend(bytes, 0, bytes.Length, Netplay.Clients[client].ServerWriteCallBack);
				}
			}
		}
	}
}
