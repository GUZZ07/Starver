using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starvers.PlayerBoosts
{
    /// <summary>
    /// 用于强化武器和饰品
    /// </summary>
    public struct BoostData
    {
        public int Type;
        public int Level;

        public override string ToString()
        {
            return $"{Type}&{Level}";
        }

        public static BoostData FromString(string value)
        {
            var values = value.Split('&');
            return new BoostData
            {
                Type = int.Parse(values[0]),
                Level = int.Parse(values[1])
            };
        }
        public static List<BoostData> SplitFromString(string value)
        {
            var values = value.Split(',');
            var datas = new List<BoostData>(values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                datas[i] = FromString(values[i]);
            }
            return datas;
        }
    }
}
