using System;

namespace behaviac
{
	[AttributeUsage]
	public class MethodMetaInfoAttribute : TypeMetaInfoAttribute
	{
		public virtual bool IsNamedEvent
		{
			get
			{
				return false;
			}
		}

		public MethodMetaInfoAttribute(string displayName, string description) : base(displayName, description)
		{
		}

		public MethodMetaInfoAttribute()
		{
		}
	}
}
