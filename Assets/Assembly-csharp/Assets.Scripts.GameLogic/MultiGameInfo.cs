using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using CSProtocol;
using System;

namespace Assets.Scripts.GameLogic
{
	public abstract class MultiGameInfo : GameInfoBase
	{
		public override void PreBeginPlay()
		{
			base.PreBeginPlay();
			this.PreparePlayer();
			this.ResetSynchrConfig();
			this.LoadAllTeamActors();
		}

		protected virtual void PreparePlayer()
		{
			MultiGameContext multiGameContext = this.GameContext as MultiGameContext;
			DebugHelper.Assert(multiGameContext != null);
			if (multiGameContext == null)
			{
				return;
			}
			if (Singleton<GamePlayerCenter>.instance.GetAllPlayers().get_Count() > 0)
			{
			}
			Singleton<GamePlayerCenter>.instance.ClearAllPlayers();
			uint num = 0u;
			string text = string.Empty;
			int num2 = -1;
			for (int i = 0; i < 2; i++)
			{
				text = text + "camp" + i.ToString() + "|";
				int num3 = 0;
				while ((long)num3 < (long)((ulong)multiGameContext.MessageRef.astCampInfo[i].dwPlayerNum))
				{
					uint dwObjId = multiGameContext.MessageRef.astCampInfo[i].astCampPlayerInfo[num3].stPlayerInfo.dwObjId;
					Player player = Singleton<GamePlayerCenter>.GetInstance().GetPlayer(dwObjId);
					num2++;
					DebugHelper.Assert(player == null, "你tm肯定在逗我");
					if (num == 0u && i + 1 == 1)
					{
						num = dwObjId;
					}
					bool flag = multiGameContext.MessageRef.astCampInfo[i].astCampPlayerInfo[num3].stPlayerInfo.bObjType == 2;
					if (player != null)
					{
						goto IL_6F8;
					}
					string openId = string.Empty;
					uint vipLv = 0u;
					int honorId = 0;
					int honorLevel = 0;
					uint wangZheCnt = 0u;
					ulong uid;
					uint logicWrold;
					uint level;
					if (flag)
					{
						if (Convert.ToBoolean(multiGameContext.MessageRef.stDeskInfo.bIsWarmBattle))
						{
							uid = multiGameContext.MessageRef.astCampInfo[i].astCampPlayerInfo[num3].stPlayerInfo.stDetail.stPlayerOfNpc.ullFakeUid;
							logicWrold = multiGameContext.MessageRef.astCampInfo[i].astCampPlayerInfo[num3].stPlayerInfo.stDetail.stPlayerOfNpc.dwFakeLogicWorldID;
							level = multiGameContext.MessageRef.astCampInfo[i].astCampPlayerInfo[num3].stPlayerInfo.stDetail.stPlayerOfNpc.dwFakePvpLevel;
							openId = string.Empty;
						}
						else
						{
							uid = 0uL;
							logicWrold = 0u;
							level = multiGameContext.MessageRef.astCampInfo[i].astCampPlayerInfo[num3].stPlayerInfo.dwLevel;
							openId = string.Empty;
						}
					}
					else
					{
						uid = multiGameContext.MessageRef.astCampInfo[i].astCampPlayerInfo[num3].stPlayerInfo.stDetail.stPlayerOfAcnt.ullUid;
						logicWrold = (uint)multiGameContext.MessageRef.astCampInfo[i].astCampPlayerInfo[num3].stPlayerInfo.stDetail.stPlayerOfAcnt.iLogicWorldID;
						level = multiGameContext.MessageRef.astCampInfo[i].astCampPlayerInfo[num3].stPlayerInfo.dwLevel;
						openId = Utility.UTF8Convert(multiGameContext.MessageRef.astCampInfo[i].astCampPlayerInfo[num3].szOpenID);
						vipLv = multiGameContext.MessageRef.astCampInfo[i].astCampPlayerInfo[num3].stPlayerInfo.stDetail.stPlayerOfAcnt.stGameVip.dwCurLevel;
						honorId = multiGameContext.MessageRef.astCampInfo[i].astCampPlayerInfo[num3].stPlayerInfo.stDetail.stPlayerOfAcnt.iHonorID;
						honorLevel = multiGameContext.MessageRef.astCampInfo[i].astCampPlayerInfo[num3].stPlayerInfo.stDetail.stPlayerOfAcnt.iHonorLevel;
						wangZheCnt = multiGameContext.MessageRef.astCampInfo[i].astCampPlayerInfo[num3].stPlayerInfo.stDetail.stPlayerOfAcnt.dwWangZheCnt;
					}
					GameIntimacyData intimacyData = null;
					if (!flag)
					{
						CSDT_CAMPPLAYERINFO cSDT_CAMPPLAYERINFO = multiGameContext.MessageRef.astCampInfo[i].astCampPlayerInfo[num3];
						if (cSDT_CAMPPLAYERINFO == null)
						{
							goto IL_81D;
						}
						string text2 = StringHelper.UTF8BytesToString(ref cSDT_CAMPPLAYERINFO.stPlayerInfo.szName);
						ulong ullUid = cSDT_CAMPPLAYERINFO.stPlayerInfo.stDetail.stPlayerOfAcnt.ullUid;
						int iLogicWorldID = cSDT_CAMPPLAYERINFO.stPlayerInfo.stDetail.stPlayerOfAcnt.iLogicWorldID;
						byte bIntimacyRelationPrior = cSDT_CAMPPLAYERINFO.stIntimacyRelation.bIntimacyRelationPrior;
						COMDT_INTIMACY_CARD_INFO stIntimacyRelationData = cSDT_CAMPPLAYERINFO.stIntimacyRelation.stIntimacyRelationData;
						LoadEnt loadEnt = new LoadEnt();
						for (int j = 0; j < (int)stIntimacyRelationData.bIntimacyNum; j++)
						{
							COMDT_INTIMACY_CARD_DATA cOMDT_INTIMACY_CARD_DATA = stIntimacyRelationData.astIntimacyData[j];
							if (cOMDT_INTIMACY_CARD_DATA != null)
							{
								for (int k = 0; k < 2; k++)
								{
									int num4 = 0;
									while ((long)num4 < (long)((ulong)multiGameContext.MessageRef.astCampInfo[k].dwPlayerNum))
									{
										CSDT_CAMPPLAYERINFO cSDT_CAMPPLAYERINFO2 = multiGameContext.MessageRef.astCampInfo[k].astCampPlayerInfo[num4];
										if (multiGameContext.MessageRef.astCampInfo[k].astCampPlayerInfo[num4].stPlayerInfo.bObjType != 2)
										{
											ulong ullUid2 = cSDT_CAMPPLAYERINFO2.stPlayerInfo.stDetail.stPlayerOfAcnt.ullUid;
											int iLogicWorldID2 = cSDT_CAMPPLAYERINFO2.stPlayerInfo.stDetail.stPlayerOfAcnt.iLogicWorldID;
											if (cSDT_CAMPPLAYERINFO2 != null && (ullUid2 != ullUid || iLogicWorldID2 != iLogicWorldID))
											{
												if ((int)cOMDT_INTIMACY_CARD_DATA.wIntimacyValue >= IntimacyRelationViewUT.Getlevel2_maxValue() && cOMDT_INTIMACY_CARD_DATA.stUin.ullUid == ullUid2 && (ulong)cOMDT_INTIMACY_CARD_DATA.stUin.dwLogicWorldId == (ulong)((long)iLogicWorldID2) && IntimacyRelationViewUT.IsRelationPriorityHigher(cOMDT_INTIMACY_CARD_DATA.bIntimacyState, loadEnt.state, bIntimacyRelationPrior))
												{
													string text3 = StringHelper.UTF8BytesToString(ref cSDT_CAMPPLAYERINFO2.stPlayerInfo.szName);
													string text4 = string.Format("----FRData Loading s1: 玩家:{0} ----对方名字:{1}, 新关系:{2}替换之前关系:{3}", new object[]
													{
														text2,
														text3,
														(COM_INTIMACY_STATE)cOMDT_INTIMACY_CARD_DATA.bIntimacyState,
														loadEnt.state
													});
													loadEnt.intimacyValue = (int)cOMDT_INTIMACY_CARD_DATA.wIntimacyValue;
													loadEnt.otherSideName = text3;
													loadEnt.state = (COM_INTIMACY_STATE)cOMDT_INTIMACY_CARD_DATA.bIntimacyState;
													loadEnt.otherSideUlluid = ullUid2;
													loadEnt.otherSideWorldId = iLogicWorldID2;
												}
											}
										}
										num4++;
									}
								}
							}
						}
						if (loadEnt.state != COM_INTIMACY_STATE.COM_INTIMACY_STATE_NULL)
						{
							string relationInLoadingMenu = IntimacyRelationViewUT.GetRelationInLoadingMenu((byte)loadEnt.state, loadEnt.otherSideName);
							intimacyData = new GameIntimacyData(loadEnt.intimacyValue, loadEnt.state, loadEnt.otherSideUlluid, loadEnt.otherSideWorldId, relationInLoadingMenu);
						}
					}
					player = Singleton<GamePlayerCenter>.GetInstance().AddPlayer(dwObjId, i + COM_PLAYERCAMP.COM_PLAYERCAMP_1, (int)multiGameContext.MessageRef.astCampInfo[i].astCampPlayerInfo[num3].stPlayerInfo.bPosOfCamp, level, flag, Utility.UTF8Convert(multiGameContext.MessageRef.astCampInfo[i].astCampPlayerInfo[num3].stPlayerInfo.szName), 0, (int)logicWrold, uid, vipLv, openId, multiGameContext.MessageRef.astCampInfo[i].astCampPlayerInfo[num3].dwShowGradeOfRank, multiGameContext.MessageRef.astCampInfo[i].astCampPlayerInfo[num3].dwClassOfRank, wangZheCnt, honorId, honorLevel, intimacyData, multiGameContext.MessageRef.astCampInfo[i].astCampPlayerInfo[num3].ullUserPrivacyBits);
					DebugHelper.Assert(player != null, "创建player失败");
					if (player != null)
					{
						player.isGM = (multiGameContext.MessageRef.astCampInfo[i].astCampPlayerInfo[num3].bIsGM > 0);
						goto IL_6F8;
					}
					goto IL_6F8;
					IL_81D:
					num3++;
					continue;
					IL_6F8:
					for (int l = 0; l < multiGameContext.MessageRef.astCampInfo[i].astCampPlayerInfo[num3].stPlayerInfo.astChoiceHero.Length; l++)
					{
						COMDT_CHOICEHERO cOMDT_CHOICEHERO = multiGameContext.MessageRef.astCampInfo[i].astCampPlayerInfo[num3].stPlayerInfo.astChoiceHero[l];
						int dwHeroID = (int)cOMDT_CHOICEHERO.stBaseInfo.stCommonInfo.dwHeroID;
						if (dwHeroID != 0)
						{
							bool flag2 = (cOMDT_CHOICEHERO.stBaseInfo.stCommonInfo.dwMaskBits & 4u) > 0u && (cOMDT_CHOICEHERO.stBaseInfo.stCommonInfo.dwMaskBits & 1u) == 0u;
							if (multiGameContext.MessageRef.astCampInfo[i].astCampPlayerInfo[num3].stPlayerInfo.bObjType == 1)
							{
								if (flag2)
								{
								}
							}
							if (player != null)
							{
								player.AddHero((uint)dwHeroID);
							}
						}
					}
					if (player != null)
					{
						string text5 = string.Format("{0}:{1}|", player.OpenId, player.LogicWrold);
						text += text5;
						goto IL_81D;
					}
					goto IL_81D;
				}
			}
			MonoSingleton<TGPSDKSys>.GetInstance().GameStart(text);
			if (Singleton<WatchController>.GetInstance().IsWatching)
			{
				Player playerByUid = Singleton<GamePlayerCenter>.GetInstance().GetPlayerByUid(Singleton<WatchController>.GetInstance().TargetUID);
				if (playerByUid != null)
				{
					Singleton<GamePlayerCenter>.GetInstance().SetHostPlayer(playerByUid.PlayerId);
				}
				else
				{
					Singleton<GamePlayerCenter>.GetInstance().SetHostPlayer(num);
				}
			}
			else
			{
				Player playerByUid2 = Singleton<GamePlayerCenter>.GetInstance().GetPlayerByUid(Singleton<CRoleInfoManager>.instance.masterUUID);
				DebugHelper.Assert(playerByUid2 != null, "load multi game but hostPlayer is null");
				Singleton<GamePlayerCenter>.GetInstance().SetHostPlayer(playerByUid2.PlayerId);
			}
			multiGameContext.levelContext.m_isWarmBattle = Convert.ToBoolean(multiGameContext.MessageRef.stDeskInfo.bIsWarmBattle);
			multiGameContext.SaveServerData();
		}

		protected virtual void ResetSynchrConfig()
		{
			MultiGameContext multiGameContext = this.GameContext as MultiGameContext;
			DebugHelper.Assert(multiGameContext != null);
			Singleton<FrameSynchr>.GetInstance().SetSynchrConfig(multiGameContext.MessageRef.dwKFrapsFreqMs, (uint)multiGameContext.MessageRef.bKFrapsLater, (uint)multiGameContext.MessageRef.bPreActFrap, multiGameContext.MessageRef.dwRandomSeed);
		}

		public override void OnLoadingProgress(float Progress)
		{
			if (!Singleton<WatchController>.GetInstance().IsWatching)
			{
				CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1083u);
				cSPkg.stPkgData.stMultGameLoadProcessReq.wProcess = Convert.ToUInt16(Progress * 100f);
				Singleton<NetworkModule>.GetInstance().SendGameMsg(ref cSPkg, 0u);
			}
			CUILoadingSystem.OnSelfLoadProcess(Progress);
		}

		public override void StartFight()
		{
			base.StartFight();
		}

		public override void EndGame()
		{
			base.EndGame();
		}
	}
}
