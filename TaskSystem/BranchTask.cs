using Starvers.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerrariaApi.Server;
using TShockAPI;

namespace Starvers.TaskSystem
{
	using Color = Microsoft.Xna.Framework.Color;
	public abstract class BranchTask : ITask
	{
		protected string name;
		protected Color? colorOfName;
		public StarverPlayer TargetPlayer { get; }
		public string Description { get; protected set; }
		/// <summary>
		/// 属于哪条支线?
		/// </summary>
		public abstract BLID BLID { get; }
		public BranchLine LineBelongTo
		{
			get
			{
				return Starver.Instance.TSKS.BranchTaskLines[(byte)BLID];
			}
		}

		protected BranchTask(StarverPlayer player)
		{
			TargetPlayer = player;
		}

		public virtual void Start()
		{
			if(name != null)
			{
				TargetPlayer.SendCombatMSsg(name, colorOfName ?? Color.Silver);
			}
		}

		public virtual void OnDeath()
		{
			TargetPlayer.SendFailMessage("任务由于死亡而失败");
			End(false);
		}

		public virtual void OnPickAnalogItem(AuraSystem.Realms.AnalogItem item)
		{

		}

		public virtual void OnUpdateItemDrop(UpdateItemDropEventArgs args)
		{

		}

		public virtual void OnLeave()
		{
			End(false);
		}

		public virtual void OnLogout()
		{
			End(false);
		}

		public virtual void OnGetData(GetDataEventArgs args)
		{

		}

		public virtual void Updating(int Timer)
		{

		}
		public virtual void Updated(int Timer)
		{

		}

		public virtual void StrikingNPC(NPCStrikeEventArgs args)
		{

		}
		public virtual void StrikedNPC(NPCStrikeEventArgs args)
		{

		}

		public virtual void ReleasingSkill(ReleaseSkillEventArgs args)
		{

		}
		public virtual void ReleasedSkill(ReleaseSkillEventArgs args)
		{

		}

		protected virtual void End(bool success)
		{
			TargetPlayer.BranchTaskEnd(success);
		}
	}
}
