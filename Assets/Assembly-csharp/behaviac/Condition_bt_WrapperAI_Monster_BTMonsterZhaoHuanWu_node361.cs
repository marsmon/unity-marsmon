using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Condition_bt_WrapperAI_Monster_BTMonsterZhaoHuanWu_node361 : Condition
	{
		private int opl_p1;

		public Condition_bt_WrapperAI_Monster_BTMonsterZhaoHuanWu_node361()
		{
			this.opl_p1 = 10000;
		}

		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			uint objID = (uint)pAgent.GetVariable(2120643215u);
			bool flag = ((ObjAgent)pAgent).IsDistanceToActorMoreThanRange(objID, this.opl_p1);
			bool flag2 = true;
			bool flag3 = flag == flag2;
			return (!flag3) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS;
		}
	}
}
