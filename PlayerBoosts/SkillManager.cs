using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starvers.PlayerBoosts
{
	public class SkillManager : IEnumerable<StarverSkill>
	{
		#region Fields
		private StarverSkill[] skills;
		#endregion
		#region Properties
		public int Count => StarverSkill.Count;

		public StarverSkill this[int id] => skills[id];
		#endregion
		#region Ctor
		public SkillManager()
		{
			Load();
		}
		#endregion
		#region Load
		public void Load()
		{
			var types = typeof(SkillManager).Assembly.GetTypes();
			var skillTypes = types.Where(type => type.IsSubclassOf(typeof(StarverSkill)) && !type.IsAbstract);
			foreach (var type in skillTypes)
			{
				var skill = (StarverSkill)Activator.CreateInstance(type);
				skill.Load();
				skills[skill.ID] = skill;
			}
		}
		#endregion
		#region GetSkill
		public SkillT GetSkill<SkillT>() where SkillT : StarverSkill, new()
		{
			return SkillInstance<SkillT>.Value;
		}
		#endregion
		#region Update
		public void Update()
		{
			for (int i = 0; i < Count; i++)
			{
				skills[i].UpdateState();
			}
		}
		#endregion
		#region GetEnumerator
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		public IEnumerator<StarverSkill> GetEnumerator() => ((IEnumerable<StarverSkill>)skills).GetEnumerator();
		#endregion
	}

	public static class SkillInstance<SkillT> where SkillT : StarverSkill
	{
		public static SkillT Value { get; private set; }
		public static byte ID { get; private set; }
		public static void Load(SkillT skill, byte id)
		{
			if (Value != null)
			{
				throw new InvalidOperationException();
			}
			Value = skill;
			ID = id;
		}
	}

	public static class SkillID<T> where T : StarverSkill, new()
	{
		public static byte Value => SkillInstance<T>.ID;
	}
}
