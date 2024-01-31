using System;

namespace Assets.Scripts.GameLogic
{
	public struct CampJudgeRecord
	{
		public int killNum;

		public int deadNum;

		public int gainCoin;

		public int hurtToHero;

		public int sufferHero;

		public void Reset()
		{
			this.killNum = 0;
			this.deadNum = 0;
			this.gainCoin = 0;
			this.hurtToHero = 0;
			this.sufferHero = 0;
		}
	}
}
