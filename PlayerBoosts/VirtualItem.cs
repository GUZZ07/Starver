using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starvers.PlayerBoosts
{
    public struct VirtualItem
    {
        public int RawItemType;
        public int Stack;

        public override string ToString()
        {
            return $"{RawItemType}&{Stack}";
        }
        public string ToIcon()
        {
            return $"[i/s{Stack}:{RawItemType}";
        }

        public static VirtualItem FromString(string value)
        {
            var values = value.Split('&');
            return new VirtualItem
            {
                RawItemType = int.Parse(values[0]),
                Stack = int.Parse(values[1])
            };
        }
        public static List<VirtualItem> SplitFromString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return new List<VirtualItem>();
            }
            var values = value.Split(',');
            var datas = new List<VirtualItem>(values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                datas[i] = FromString(values[i]);
            }
            return datas;
        }
    }
}
