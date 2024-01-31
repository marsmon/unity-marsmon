using System;

namespace Assets.Scripts.GameLogic
{
	public struct JudgeStatc
	{
		public int KillNum;

		public int DeadNum;

		public int AssitNum;

		public int GainCoin;

		public int HurtToHero;

		public int HurtToAll;

		public int SufferHero;

		public void Reset()
		{
			this.KillNum = 0;
			this.DeadNum = 0;
			this.AssitNum = 0;
			this.GainCoin = 0;
			this.HurtToHero = 0;
			this.HurtToAll = 0;
			this.SufferHero = 0;
		}
	}
}
