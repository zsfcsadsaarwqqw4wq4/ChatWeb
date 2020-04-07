using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class EnumHelper
    {
        public enum ThemeEnum
        {
            [Description("贴吧")]//描述
            System = 1,
            [Description("Word")]
            Procurement = 2,
            [Description("其他主题")]
            FixedAssets = 3
        }
        public enum TiebaEnum
        {
            [Description("电影")]//描述
            One = 1,
            [Description("游戏")]
            two = 2,
            [Description("小说")]
            three = 3,
            [Description("科技")]
            four = 4,
            [Description("汽车")]
            five = 5
        }
        public enum Message
        {
            [Description("系统消息")]//描述
            One = 1,
            [Description("聊天消息")]
            two = 2,
            [Description("已读消息")]
            three = 3,
            [Description("历史消息")]
            four = 4,
            [Description("添加好友")]
            five = 5
        }
        public enum PassWord
        {
            [Description("聊天类型不变")]//描述
            One = 1,
            [Description("私密聊天")]
            two = 2,
            [Description("正常聊天")]
            three = 3,
            [Description("输入的密码有误")]
            four = 4
        }
        /// <summary>
        /// 获取枚举的描述属性
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string GetEnumDescription(Enum enumValue)
        {
            
            string value = enumValue.ToString();
            FieldInfo field = enumValue.GetType().GetField(value);
            object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);  //获取描述属性
            if (objs == null || objs.Length == 0)  //当描述属性没有时，直接返回名称
                return value;
            DescriptionAttribute descriptionAttribute = (DescriptionAttribute)objs[0];
            return descriptionAttribute.Description;
        }
    }
}
