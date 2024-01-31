using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Hero_HeroCommonAutoAI_node86 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			int searchRange = ((ObjAgent)pAgent).GetSearchRange();
			pAgent.SetVariable<int>("p_srchRange", searchRange, 2451377514u);
			return result;
		}
	}
}
