using System;
using tsf4g_tdr_csharp;

namespace ResData
{
	public class ResSkillCombineCfgInfo : IUnpackable, tsf4g_csharp_interface
	{
		public int iCfgID;

		public int iDependCfgID;

		public int iMutexCfgID1;

		public int iMutexCfgID2;

		public int iMutexCfgID3;

		public int iTriggerRate;

		public int iCroupID;

		public int iFirstLifeStealAttenuation;

		public int iFollowUpLifeStealAttenuation;

		public int iNextDeltaFadeRate;

		public int iNextLowFadeRate;

		public byte bClearRule;

		public int iOverlayFadeRate;

		public byte bOverlayRule;

		public byte bOverlayMax;

		public byte bEffectType;

		public byte bEffectSubType;

		public byte bShowType;

		public byte bFloatTextID;

		public byte[] szSkillCombineName_ByteArray;

		public byte[] szSkillCombineDesc_ByteArray;

		public byte[] szPrefab_ByteArray;

		public int iDuration;

		public int iDurationGrow;

		public ResDT_SkillFunc[] astSkillFuncInfo;

		public byte bSrcType;

		public byte[] szIconPath_ByteArray;

		public byte bIsShowBuff;

		public byte bShowBuffPriority;

		public byte bGrowthType;

		public byte bIsInheritByKiller;

		public byte bCanSkillCrit;

		public int iDamageLimit;

		public int iMonsterDamageLimit;

		public int iLongRangeReduction;

		public byte bEffectiveTargetType;

		public byte bIsAssistEffect;

		public byte bAgeImmeExcute;

		public byte bNotGetHate;

		public int iExtraEffectSlotType;

		public byte[] szNameReplacement_ByteArray;

		public byte[] szNameReplacementColor_ByteArray;

		public string szSkillCombineName;

		public string szSkillCombineDesc;

		public string szPrefab;

		public string szIconPath;

		public string szNameReplacement;

		public string szNameReplacementColor;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly uint LENGTH_szSkillCombineName = 128u;

		public static readonly uint LENGTH_szSkillCombineDesc = 128u;

		public static readonly uint LENGTH_szPrefab = 128u;

		public static readonly uint LENGTH_szIconPath = 128u;

		public static readonly uint LENGTH_szNameReplacement = 64u;

		public static readonly uint LENGTH_szNameReplacementColor = 64u;

		public ResSkillCombineCfgInfo()
		{
			this.szSkillCombineName_ByteArray = new byte[1];
			this.szSkillCombineDesc_ByteArray = new byte[1];
			this.szPrefab_ByteArray = new byte[1];
			this.astSkillFuncInfo = new ResDT_SkillFunc[4];
			for (int i = 0; i < 4; i++)
			{
				this.astSkillFuncInfo[i] = new ResDT_SkillFunc();
			}
			this.szIconPath_ByteArray = new byte[1];
			this.szNameReplacement_ByteArray = new byte[1];
			this.szNameReplacementColor_ByteArray = new byte[1];
			this.szSkillCombineName = string.Empty;
			this.szSkillCombineDesc = string.Empty;
			this.szPrefab = string.Empty;
			this.szIconPath = string.Empty;
			this.szNameReplacement = string.Empty;
			this.szNameReplacementColor = string.Empty;
		}

		private void TransferData()
		{
			this.szSkillCombineName = StringHelper.UTF8BytesToString(ref this.szSkillCombineName_ByteArray);
			this.szSkillCombineName_ByteArray = null;
			this.szSkillCombineDesc = StringHelper.UTF8BytesToString(ref this.szSkillCombineDesc_ByteArray);
			this.szSkillCombineDesc_ByteArray = null;
			this.szPrefab = StringHelper.UTF8BytesToString(ref this.szPrefab_ByteArray);
			this.szPrefab_ByteArray = null;
			this.szIconPath = StringHelper.UTF8BytesToString(ref this.szIconPath_ByteArray);
			this.szIconPath_ByteArray = null;
			this.szNameReplacement = StringHelper.UTF8BytesToString(ref this.szNameReplacement_ByteArray);
			this.szNameReplacement_ByteArray = null;
			this.szNameReplacementColor = StringHelper.UTF8BytesToString(ref this.szNameReplacementColor_ByteArray);
			this.szNameReplacementColor_ByteArray = null;
		}

		public TdrError.ErrorType construct()
		{
			TdrError.ErrorType errorType = TdrError.ErrorType.TDR_NO_ERROR;
			this.iCfgID = 0;
			this.iDependCfgID = 0;
			this.iMutexCfgID1 = 0;
			this.iMutexCfgID2 = 0;
			this.iMutexCfgID3 = 0;
			this.iTriggerRate = 0;
			this.iCroupID = 0;
			this.iFirstLifeStealAttenuation = 0;
			this.iFollowUpLifeStealAttenuation = 0;
			this.iNextDeltaFadeRate = 0;
			this.iNextLowFadeRate = 0;
			this.bClearRule = 0;
			this.iOverlayFadeRate = 0;
			this.bOverlayRule = 0;
			this.bOverlayMax = 0;
			this.bEffectType = 0;
			this.bEffectSubType = 0;
			this.bShowType = 0;
			this.bFloatTextID = 0;
			this.iDuration = 0;
			this.iDurationGrow = 0;
			for (int i = 0; i < 4; i++)
			{
				errorType = this.astSkillFuncInfo[i].construct();
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			this.bSrcType = 0;
			this.bIsShowBuff = 0;
			this.bShowBuffPriority = 0;
			this.bGrowthType = 0;
			this.bIsInheritByKiller = 0;
			this.bCanSkillCrit = 0;
			this.iDamageLimit = 0;
			this.iMonsterDamageLimit = 0;
			this.iLongRangeReduction = 0;
			this.bEffectiveTargetType = 0;
			this.bIsAssistEffect = 0;
			this.bAgeImmeExcute = 0;
			this.bNotGetHate = 0;
			this.iExtraEffectSlotType = -1;
			return errorType;
		}

		public TdrError.ErrorType unpack(ref byte[] buffer, int size, ref int usedSize, uint cutVer)
		{
			if (buffer == null || buffer.GetLength(0) == 0 || size > buffer.GetLength(0))
			{
				return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
			}
			TdrReadBuf tdrReadBuf = new TdrReadBuf(ref buffer, size);
			TdrError.ErrorType result = this.unpack(ref tdrReadBuf, cutVer);
			usedSize = tdrReadBuf.getUsedSize();
			return result;
		}

		public TdrError.ErrorType unpack(ref TdrReadBuf srcBuf, uint cutVer)
		{
			if (cutVer == 0u || ResSkillCombineCfgInfo.CURRVERSION < cutVer)
			{
				cutVer = ResSkillCombineCfgInfo.CURRVERSION;
			}
			if (ResSkillCombineCfgInfo.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readInt32(ref this.iCfgID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iDependCfgID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iMutexCfgID1);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iMutexCfgID2);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iMutexCfgID3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iTriggerRate);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iCroupID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iFirstLifeStealAttenuation);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iFollowUpLifeStealAttenuation);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iNextDeltaFadeRate);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iNextLowFadeRate);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bClearRule);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iOverlayFadeRate);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bOverlayRule);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bOverlayMax);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bEffectType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bEffectSubType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bShowType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bFloatTextID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			uint num = 0u;
			errorType = srcBuf.readUInt32(ref num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num > (uint)this.szSkillCombineName_ByteArray.GetLength(0))
			{
				if ((long)num > (long)((ulong)ResSkillCombineCfgInfo.LENGTH_szSkillCombineName))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szSkillCombineName_ByteArray = new byte[num];
			}
			if (1u > num)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szSkillCombineName_ByteArray, (int)num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szSkillCombineName_ByteArray[(int)(num - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num2 = TdrTypeUtil.cstrlen(this.szSkillCombineName_ByteArray) + 1;
			if ((ulong)num != (ulong)((long)num2))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			uint num3 = 0u;
			errorType = srcBuf.readUInt32(ref num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num3 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num3 > (uint)this.szSkillCombineDesc_ByteArray.GetLength(0))
			{
				if ((long)num3 > (long)((ulong)ResSkillCombineCfgInfo.LENGTH_szSkillCombineDesc))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szSkillCombineDesc_ByteArray = new byte[num3];
			}
			if (1u > num3)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szSkillCombineDesc_ByteArray, (int)num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szSkillCombineDesc_ByteArray[(int)(num3 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num4 = TdrTypeUtil.cstrlen(this.szSkillCombineDesc_ByteArray) + 1;
			if ((ulong)num3 != (ulong)((long)num4))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			uint num5 = 0u;
			errorType = srcBuf.readUInt32(ref num5);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num5 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num5 > (uint)this.szPrefab_ByteArray.GetLength(0))
			{
				if ((long)num5 > (long)((ulong)ResSkillCombineCfgInfo.LENGTH_szPrefab))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szPrefab_ByteArray = new byte[num5];
			}
			if (1u > num5)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szPrefab_ByteArray, (int)num5);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szPrefab_ByteArray[(int)(num5 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num6 = TdrTypeUtil.cstrlen(this.szPrefab_ByteArray) + 1;
			if ((ulong)num5 != (ulong)((long)num6))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readInt32(ref this.iDuration);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iDurationGrow);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 4; i++)
			{
				errorType = this.astSkillFuncInfo[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readUInt8(ref this.bSrcType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			uint num7 = 0u;
			errorType = srcBuf.readUInt32(ref num7);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num7 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num7 > (uint)this.szIconPath_ByteArray.GetLength(0))
			{
				if ((long)num7 > (long)((ulong)ResSkillCombineCfgInfo.LENGTH_szIconPath))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szIconPath_ByteArray = new byte[num7];
			}
			if (1u > num7)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szIconPath_ByteArray, (int)num7);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szIconPath_ByteArray[(int)(num7 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num8 = TdrTypeUtil.cstrlen(this.szIconPath_ByteArray) + 1;
			if ((ulong)num7 != (ulong)((long)num8))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt8(ref this.bIsShowBuff);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bShowBuffPriority);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bGrowthType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsInheritByKiller);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bCanSkillCrit);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iDamageLimit);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iMonsterDamageLimit);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iLongRangeReduction);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bEffectiveTargetType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsAssistEffect);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bAgeImmeExcute);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bNotGetHate);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iExtraEffectSlotType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			uint num9 = 0u;
			errorType = srcBuf.readUInt32(ref num9);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num9 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num9 > (uint)this.szNameReplacement_ByteArray.GetLength(0))
			{
				if ((long)num9 > (long)((ulong)ResSkillCombineCfgInfo.LENGTH_szNameReplacement))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szNameReplacement_ByteArray = new byte[num9];
			}
			if (1u > num9)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szNameReplacement_ByteArray, (int)num9);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szNameReplacement_ByteArray[(int)(num9 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num10 = TdrTypeUtil.cstrlen(this.szNameReplacement_ByteArray) + 1;
			if ((ulong)num9 != (ulong)((long)num10))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			uint num11 = 0u;
			errorType = srcBuf.readUInt32(ref num11);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num11 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num11 > (uint)this.szNameReplacementColor_ByteArray.GetLength(0))
			{
				if ((long)num11 > (long)((ulong)ResSkillCombineCfgInfo.LENGTH_szNameReplacementColor))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szNameReplacementColor_ByteArray = new byte[num11];
			}
			if (1u > num11)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szNameReplacementColor_ByteArray, (int)num11);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szNameReplacementColor_ByteArray[(int)(num11 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num12 = TdrTypeUtil.cstrlen(this.szNameReplacementColor_ByteArray) + 1;
			if ((ulong)num11 != (ulong)((long)num12))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			this.TransferData();
			return errorType;
		}

		public TdrError.ErrorType load(ref byte[] buffer, int size, ref int usedSize, uint cutVer)
		{
			if (buffer == null || buffer.GetLength(0) == 0 || size > buffer.GetLength(0))
			{
				return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
			}
			TdrReadBuf tdrReadBuf = new TdrReadBuf(ref buffer, size);
			TdrError.ErrorType result = this.load(ref tdrReadBuf, cutVer);
			usedSize = tdrReadBuf.getUsedSize();
			return result;
		}

		public TdrError.ErrorType load(ref TdrReadBuf srcBuf, uint cutVer)
		{
			srcBuf.disableEndian();
			if (cutVer == 0u || ResSkillCombineCfgInfo.CURRVERSION < cutVer)
			{
				cutVer = ResSkillCombineCfgInfo.CURRVERSION;
			}
			if (ResSkillCombineCfgInfo.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readInt32(ref this.iCfgID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iDependCfgID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iMutexCfgID1);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iMutexCfgID2);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iMutexCfgID3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iTriggerRate);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iCroupID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iFirstLifeStealAttenuation);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iFollowUpLifeStealAttenuation);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iNextDeltaFadeRate);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iNextLowFadeRate);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bClearRule);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iOverlayFadeRate);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bOverlayRule);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bOverlayMax);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bEffectType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bEffectSubType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bShowType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bFloatTextID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num = 128;
			if (this.szSkillCombineName_ByteArray.GetLength(0) < num)
			{
				this.szSkillCombineName_ByteArray = new byte[ResSkillCombineCfgInfo.LENGTH_szSkillCombineName];
			}
			errorType = srcBuf.readCString(ref this.szSkillCombineName_ByteArray, num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num2 = 128;
			if (this.szSkillCombineDesc_ByteArray.GetLength(0) < num2)
			{
				this.szSkillCombineDesc_ByteArray = new byte[ResSkillCombineCfgInfo.LENGTH_szSkillCombineDesc];
			}
			errorType = srcBuf.readCString(ref this.szSkillCombineDesc_ByteArray, num2);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num3 = 128;
			if (this.szPrefab_ByteArray.GetLength(0) < num3)
			{
				this.szPrefab_ByteArray = new byte[ResSkillCombineCfgInfo.LENGTH_szPrefab];
			}
			errorType = srcBuf.readCString(ref this.szPrefab_ByteArray, num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iDuration);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iDurationGrow);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 4; i++)
			{
				errorType = this.astSkillFuncInfo[i].load(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readUInt8(ref this.bSrcType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num4 = 128;
			if (this.szIconPath_ByteArray.GetLength(0) < num4)
			{
				this.szIconPath_ByteArray = new byte[ResSkillCombineCfgInfo.LENGTH_szIconPath];
			}
			errorType = srcBuf.readCString(ref this.szIconPath_ByteArray, num4);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsShowBuff);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bShowBuffPriority);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bGrowthType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsInheritByKiller);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bCanSkillCrit);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iDamageLimit);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iMonsterDamageLimit);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iLongRangeReduction);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bEffectiveTargetType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsAssistEffect);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bAgeImmeExcute);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bNotGetHate);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iExtraEffectSlotType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num5 = 64;
			if (this.szNameReplacement_ByteArray.GetLength(0) < num5)
			{
				this.szNameReplacement_ByteArray = new byte[ResSkillCombineCfgInfo.LENGTH_szNameReplacement];
			}
			errorType = srcBuf.readCString(ref this.szNameReplacement_ByteArray, num5);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num6 = 64;
			if (this.szNameReplacementColor_ByteArray.GetLength(0) < num6)
			{
				this.szNameReplacementColor_ByteArray = new byte[ResSkillCombineCfgInfo.LENGTH_szNameReplacementColor];
			}
			errorType = srcBuf.readCString(ref this.szNameReplacementColor_ByteArray, num6);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			this.TransferData();
			return errorType;
		}
	}
}
