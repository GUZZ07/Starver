using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starvers.PlayerBoosts
{
    using Items;
    /// <summary>
    /// 用于强化武器和饰品
    /// </summary>
    public struct BoostData
    {
        public int Type;                // 当这个值大于10000时，请将它减去10000并作为弹幕ID(因为有的武器只能靠弹幕来捕获)
        public int Level;

        public override string ToString()
        {
            return $"{Type}&{Level}";
        }
        #region Convert
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
            if (string.IsNullOrWhiteSpace(value))
            {
                return new List<BoostData>();
            }
            var values = value.Split(',');
            var datas = new List<BoostData>(values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                datas[i] = FromString(values[i]);
            }
            return datas;
        }
        #endregion

        public ItemBoost GetItemBoost()
        {
            return Starver.Instance.ItemBoosts.GetBoost(Type);
        }

    }
}
