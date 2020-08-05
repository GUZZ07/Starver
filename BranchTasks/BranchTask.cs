using Starvers.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerrariaApi.Server;
using TShockAPI;

namespace Starvers.BranchTasks
{
	public abstract class BranchTask
	{
		public StarverPlayer TargetPlayer { get; }

		protected BranchTask(StarverPlayer player)
		{
			TargetPlayer = player;
		}

		public virtual void Start()
		{

		}

		public virtual void OnDeath()
		{
			// TargetPlayer.SendFailMessage("任务由于死亡而失败");
			End(false);
		}

		//public virtual void OnPickAnalogItem(AuraSystem.Realms.AnalogItem item)
		//{

		//}

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

		public virtual void PreStrikeNPC(PreNPCStrikeEventArgs args)
		{

		}
		public virtual void PostStrikeNPC(PostNPCStrikeEventArgs args)
		{

		}

		public virtual void PreReleaseSkill(PreReleaseSkillEventArgs args)
		{

		}
		public virtual void PostReleaseSkill(PostReleaseSkillEventArgs args)
		{

		}

		public virtual void CreatingProj(GetDataHandlers.NewProjectileEventArgs args)
		{

		}

		protected virtual void End(bool success)
		{
			// TargetPlayer.BranchTaskEnd(success);
		}
	}
}
