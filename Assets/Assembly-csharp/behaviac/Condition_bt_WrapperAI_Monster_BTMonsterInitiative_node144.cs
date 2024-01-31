using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Condition_bt_WrapperAI_Monster_BTMonsterInitiative_node144 : Condition
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			OutOfControlType outOfControlType = ((ObjAgent)pAgent).GetOutOfControlType();
			OutOfControlType outOfControlType2 = OutOfControlType.Taunt;
			bool flag = outOfControlType == outOfControlType2;
			return (!flag) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS;
		}
	}
}
