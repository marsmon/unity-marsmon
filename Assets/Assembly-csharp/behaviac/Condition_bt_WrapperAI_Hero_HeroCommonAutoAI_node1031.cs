using System;

namespace behaviac
{
	internal class Condition_bt_WrapperAI_Hero_HeroCommonAutoAI_node1031 : Condition
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			int num = (int)pAgent.GetVariable(1460711631u);
			int num2 = 0;
			bool flag = num == num2;
			return (!flag) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS;
		}
	}
}
