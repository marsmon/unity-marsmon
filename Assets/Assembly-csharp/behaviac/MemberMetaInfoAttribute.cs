using System;
using System.Reflection;

namespace behaviac
{
	[AttributeUsage]
	public class MemberMetaInfoAttribute : TypeMetaInfoAttribute
	{
		private float m_range;

		public float Range
		{
			get
			{
				return this.m_range;
			}
		}

		public MemberMetaInfoAttribute(string displayName, string description) : this(displayName, description, 1f)
		{
		}

		public MemberMetaInfoAttribute(string displayName, string description, float range)
		{
			this.m_range = 1f;
			base..ctor(displayName, description);
			this.m_range = range;
		}

		public MemberMetaInfoAttribute()
		{
			this.m_range = 1f;
			base..ctor();
		}

		private static string getEnumName(object obj)
		{
			if (obj == null)
			{
				return string.Empty;
			}
			Type type = obj.GetType();
			if (!type.get_IsEnum())
			{
				return string.Empty;
			}
			string name = Enum.GetName(type, obj);
			if (string.IsNullOrEmpty(name))
			{
				return string.Empty;
			}
			return name;
		}

		public static string GetEnumDisplayName(object obj)
		{
			if (obj == null)
			{
				return string.Empty;
			}
			string result = MemberMetaInfoAttribute.getEnumName(obj);
			FieldInfo field = obj.GetType().GetField(obj.ToString());
			Attribute[] array = (Attribute[])field.GetCustomAttributes(typeof(MemberMetaInfoAttribute), false);
			if (array.Length > 0)
			{
				result = ((MemberMetaInfoAttribute)array[0]).DisplayName;
			}
			return result;
		}

		public static string GetEnumDescription(object obj)
		{
			if (obj == null)
			{
				return string.Empty;
			}
			string result = MemberMetaInfoAttribute.getEnumName(obj);
			FieldInfo field = obj.GetType().GetField(obj.ToString());
			Attribute[] array = (Attribute[])field.GetCustomAttributes(typeof(MemberMetaInfoAttribute), false);
			if (array.Length > 0)
			{
				result = ((MemberMetaInfoAttribute)array[0]).Description;
			}
			return result;
		}
	}
}
