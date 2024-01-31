using Assets.Scripts.GameLogic.DataCenter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class GameLoader : MonoSingleton<GameLoader>
	{
		private struct LoaderHelperWrapper
		{
			public LoaderHelper loadHelper;

			public int duty;
		}

		private struct LHCWrapper
		{
			public LoaderHelperCamera lhc;

			public LoaderHelper loadHelper;

			public int duty;
		}

		public delegate void LoadProgressDelegate(float progress);

		public delegate void LoadCompleteDelegate();

		private static GameSerializer s_serializer = new GameSerializer();

		private ArrayList levelList = new ArrayList();

		private ArrayList levelDesignList = new ArrayList();

		private ArrayList levelArtistList = new ArrayList();

		private List<string> soundBankList = new List<string>();

		public List<ActorMeta> actorList = new List<ActorMeta>();

		public ListView<ActorConfig> staticActors = new ListView<ActorConfig>();

		private int _nProgress;

		public bool isLoadStart;

		private GameLoader.LoadProgressDelegate LoadProgressEvent;

		private GameLoader.LoadCompleteDelegate LoadCompleteEvent;

		private float coroutineTime;

		private CCoroutine m_handle_PreSpawnSoldiers;

		private CCoroutine m_handle_SpawnDynamicActor;

		private CCoroutine m_handle_SpawnStaticActor;

		private CCoroutine m_handle_LoadAgeRecursiveAssets;

		private CCoroutine m_handle_LoadNoActorAssets;

		private CCoroutine m_handle_LoadActorAssets;

		private CCoroutine m_handle_LoadCommonAssets;

		private CCoroutine m_handle_LoadDesignLevel;

		private CCoroutine m_handle_LoadArtistLevel;

		private CCoroutine m_handle_LoadCommonAssetBundle;

		private CCoroutine m_handle_LoadCommonEffect;

		private CCoroutine m_handle_CoroutineLoad;

		private CCoroutine m_handle_AnalyseResPreload;

		private static Dictionary<string, string> s_vertexShaderMap;

		private List<ActorPreloadTab> actorPreload;

		private ActorPreloadTab noActorPreLoad;

		private List<ActorPreloadTab> ageRefAssetsList;

		public int nProgress
		{
			get
			{
				return this._nProgress;
			}
			set
			{
				if (value >= this._nProgress)
				{
					this._nProgress = value;
				}
			}
		}

		static GameLoader()
		{
			// Note: this type is marked as 'beforefieldinit'.
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("S_Game_Scene/Cloth_Lightmap_Wind", "S_Game_Scene/Light_VertexLit/Cloth_Lightmap_Wind");
			dictionary.Add("S_Game_Scene/Cloth_Wind", "S_Game_Scene/Light_VertexLit/Cloth_Wind");
			dictionary.Add("S_Game_Effects/Scroll2TexBend_LightMap", "S_Game_Effects/Light_VertexLit/Scroll2TexBend_LightMap");
			dictionary.Add("S_Game_Effects/Scroll2TexBend", "S_Game_Effects/Light_VertexLit/Scroll2TexBend");
			dictionary.Add("S_Game_Scene/Diffuse_NotFog", "S_Game_Scene/Light_VertexLit/Diffuse_NotFog");
			GameLoader.s_vertexShaderMap = dictionary;
		}

		public void ResetLoader()
		{
			this.levelList.Clear();
			this.actorList.Clear();
			this.levelDesignList.Clear();
			this.levelArtistList.Clear();
			this.soundBankList.Clear();
			this.staticActors.Clear();
			this._nProgress = 0;
			if (this.isLoadStart)
			{
				Singleton<CCoroutineManager>.GetInstance().StopCoroutine(this.m_handle_PreSpawnSoldiers, true);
				Singleton<CCoroutineManager>.GetInstance().StopCoroutine(this.m_handle_SpawnDynamicActor, true);
				Singleton<CCoroutineManager>.GetInstance().StopCoroutine(this.m_handle_SpawnStaticActor, true);
				Singleton<CCoroutineManager>.GetInstance().StopCoroutine(this.m_handle_LoadAgeRecursiveAssets, true);
				Singleton<CCoroutineManager>.GetInstance().StopCoroutine(this.m_handle_LoadNoActorAssets, true);
				Singleton<CCoroutineManager>.GetInstance().StopCoroutine(this.m_handle_LoadActorAssets, true);
				Singleton<CCoroutineManager>.GetInstance().StopCoroutine(this.m_handle_LoadCommonAssets, true);
				Singleton<CCoroutineManager>.GetInstance().StopCoroutine(this.m_handle_LoadDesignLevel, true);
				Singleton<CCoroutineManager>.GetInstance().StopCoroutine(this.m_handle_LoadArtistLevel, true);
				Singleton<CCoroutineManager>.GetInstance().StopCoroutine(this.m_handle_LoadCommonAssetBundle, true);
				Singleton<CCoroutineManager>.GetInstance().StopCoroutine(this.m_handle_LoadCommonEffect, true);
				Singleton<CCoroutineManager>.GetInstance().StopCoroutine(this.m_handle_CoroutineLoad, true);
				Singleton<CCoroutineManager>.GetInstance().StopCoroutine(this.m_handle_AnalyseResPreload, true);
				this.isLoadStart = false;
			}
		}

		public void AddLevel(string name)
		{
			this.levelList.Add(name);
		}

		public void AddDesignSerializedLevel(string name)
		{
			this.levelDesignList.Add(name);
		}

		public void AddArtistSerializedLevel(string name)
		{
			this.levelArtistList.Add(name);
		}

		public void AddSoundBank(string name)
		{
			this.soundBankList.Add(name);
		}

		public void AddActor(ref ActorMeta actorMeta)
		{
			this.actorList.Add(actorMeta);
		}

		public void AddStaticActor(ActorConfig actor)
		{
			this.staticActors.Add(actor);
		}

		public void Load(GameLoader.LoadProgressDelegate progress, GameLoader.LoadCompleteDelegate finish)
		{
			if (this.isLoadStart)
			{
				return;
			}
			Debug.Log("GameLoader Start Load");
			this.LoadProgressEvent = progress;
			this.LoadCompleteEvent = finish;
			this._nProgress = 0;
			this.isLoadStart = true;
			this.m_handle_CoroutineLoad = Singleton<CCoroutineManager>.GetInstance().StartCoroutine(this.CoroutineLoad());
		}

		private bool ShouldYieldReturn()
		{
			return Time.realtimeSinceStartup - this.coroutineTime > 0.08f;
		}

		private void UpdateProgress(LoaderHelperCamera lhc, int oldProgress, int duty, int index, int count)
		{
			this.coroutineTime = Time.realtimeSinceStartup;
			this.nProgress = oldProgress + duty * index / count;
			this.LoadProgressEvent((float)this.nProgress * 0.0001f);
			if (lhc != null)
			{
				lhc.Update();
			}
		}

		[DebuggerHidden]
		private IEnumerator LoadCommonAssetBundle(GameLoader.LoaderHelperWrapper InWrapper)
		{
			GameLoader.<LoadCommonAssetBundle>c__Iterator15 <LoadCommonAssetBundle>c__Iterator = new GameLoader.<LoadCommonAssetBundle>c__Iterator15();
			<LoadCommonAssetBundle>c__Iterator.InWrapper = InWrapper;
			<LoadCommonAssetBundle>c__Iterator.<$>InWrapper = InWrapper;
			<LoadCommonAssetBundle>c__Iterator.<>f__this = this;
			return <LoadCommonAssetBundle>c__Iterator;
		}

		[DebuggerHidden]
		private IEnumerator LoadCommonEffect(GameLoader.LoaderHelperWrapper InWrapper)
		{
			GameLoader.<LoadCommonEffect>c__Iterator16 <LoadCommonEffect>c__Iterator = new GameLoader.<LoadCommonEffect>c__Iterator16();
			<LoadCommonEffect>c__Iterator.InWrapper = InWrapper;
			<LoadCommonEffect>c__Iterator.<$>InWrapper = InWrapper;
			<LoadCommonEffect>c__Iterator.<>f__this = this;
			return <LoadCommonEffect>c__Iterator;
		}

		[DebuggerHidden]
		private IEnumerator LoadArtistLevel(GameLoader.LoaderHelperWrapper InWrapper)
		{
			GameLoader.<LoadArtistLevel>c__Iterator17 <LoadArtistLevel>c__Iterator = new GameLoader.<LoadArtistLevel>c__Iterator17();
			<LoadArtistLevel>c__Iterator.InWrapper = InWrapper;
			<LoadArtistLevel>c__Iterator.<$>InWrapper = InWrapper;
			<LoadArtistLevel>c__Iterator.<>f__this = this;
			return <LoadArtistLevel>c__Iterator;
		}

		private static void ChangeToVertexLit()
		{
			GameObject gameObject = GameObject.Find("Artist");
			Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Renderer renderer = componentsInChildren[i];
				if (!(null == renderer) && renderer.sharedMaterials != null)
				{
					for (int j = 0; j < renderer.sharedMaterials.Length; j++)
					{
						if (!(null == renderer.sharedMaterials[j]) && !(null == renderer.sharedMaterials[j].shader))
						{
							string text = renderer.sharedMaterials[j].shader.name;
							text = GameLoader.ChangeVertexShader(text);
							renderer.sharedMaterials[j].shader = Shader.Find(text);
						}
					}
				}
			}
		}

		private static string ChangeVertexShader(string oldShader)
		{
			if (GameLoader.s_vertexShaderMap.ContainsKey(oldShader))
			{
				return GameLoader.s_vertexShaderMap.get_Item(oldShader);
			}
			if (oldShader.Contains("S_Game_Scene/Light/"))
			{
				return oldShader.Replace("S_Game_Scene/Light/", "S_Game_Scene/Light_VertexLit/");
			}
			return oldShader;
		}

		[DebuggerHidden]
		private IEnumerator LoadDesignLevel(GameLoader.LoaderHelperWrapper InWrapper)
		{
			GameLoader.<LoadDesignLevel>c__Iterator18 <LoadDesignLevel>c__Iterator = new GameLoader.<LoadDesignLevel>c__Iterator18();
			<LoadDesignLevel>c__Iterator.InWrapper = InWrapper;
			<LoadDesignLevel>c__Iterator.<$>InWrapper = InWrapper;
			<LoadDesignLevel>c__Iterator.<>f__this = this;
			return <LoadDesignLevel>c__Iterator;
		}

		[DebuggerHidden]
		private IEnumerator LoadCommonAssets(GameLoader.LoaderHelperWrapper InWrapper)
		{
			GameLoader.<LoadCommonAssets>c__Iterator19 <LoadCommonAssets>c__Iterator = new GameLoader.<LoadCommonAssets>c__Iterator19();
			<LoadCommonAssets>c__Iterator.InWrapper = InWrapper;
			<LoadCommonAssets>c__Iterator.<$>InWrapper = InWrapper;
			<LoadCommonAssets>c__Iterator.<>f__this = this;
			return <LoadCommonAssets>c__Iterator;
		}

		[DebuggerHidden]
		private IEnumerator AnalyseResourcePreload(LoaderHelper loadHelper)
		{
			GameLoader.<AnalyseResourcePreload>c__Iterator1A <AnalyseResourcePreload>c__Iterator1A = new GameLoader.<AnalyseResourcePreload>c__Iterator1A();
			<AnalyseResourcePreload>c__Iterator1A.loadHelper = loadHelper;
			<AnalyseResourcePreload>c__Iterator1A.<$>loadHelper = loadHelper;
			<AnalyseResourcePreload>c__Iterator1A.<>f__this = this;
			return <AnalyseResourcePreload>c__Iterator1A;
		}

		[DebuggerHidden]
		private IEnumerator AnalyseActorAssets(LoaderHelper loadHelper)
		{
			GameLoader.<AnalyseActorAssets>c__Iterator1B <AnalyseActorAssets>c__Iterator1B = new GameLoader.<AnalyseActorAssets>c__Iterator1B();
			<AnalyseActorAssets>c__Iterator1B.loadHelper = loadHelper;
			<AnalyseActorAssets>c__Iterator1B.<$>loadHelper = loadHelper;
			<AnalyseActorAssets>c__Iterator1B.<>f__this = this;
			return <AnalyseActorAssets>c__Iterator1B;
		}

		[DebuggerHidden]
		private IEnumerator LoadActorAssets(GameLoader.LHCWrapper InWrapper)
		{
			GameLoader.<LoadActorAssets>c__Iterator1C <LoadActorAssets>c__Iterator1C = new GameLoader.<LoadActorAssets>c__Iterator1C();
			<LoadActorAssets>c__Iterator1C.InWrapper = InWrapper;
			<LoadActorAssets>c__Iterator1C.<$>InWrapper = InWrapper;
			<LoadActorAssets>c__Iterator1C.<>f__this = this;
			return <LoadActorAssets>c__Iterator1C;
		}

		private static List<List<string>> composite(string[] input)
		{
			List<List<string>> list = new List<List<string>>();
			if (input.Length > 8)
			{
				throw new Exception("only support less than 8 words");
			}
			byte b = 0;
			while ((float)b < Mathf.Pow(2f, (float)input.Length))
			{
				List<string> list2 = new List<string>();
				for (int i = 0; i < input.Length; i++)
				{
					if ((b >> i & 1) > 0)
					{
						list2.Add(input[i]);
					}
				}
				list.Add(list2);
				b += 1;
			}
			return list;
		}

		[DebuggerHidden]
		private IEnumerator AnalyseNoActorAssets(LoaderHelper loadHelper)
		{
			GameLoader.<AnalyseNoActorAssets>c__Iterator1D <AnalyseNoActorAssets>c__Iterator1D = new GameLoader.<AnalyseNoActorAssets>c__Iterator1D();
			<AnalyseNoActorAssets>c__Iterator1D.loadHelper = loadHelper;
			<AnalyseNoActorAssets>c__Iterator1D.<$>loadHelper = loadHelper;
			<AnalyseNoActorAssets>c__Iterator1D.<>f__this = this;
			return <AnalyseNoActorAssets>c__Iterator1D;
		}

		[DebuggerHidden]
		private IEnumerator LoadNoActorAssets(GameLoader.LHCWrapper InWrapper)
		{
			GameLoader.<LoadNoActorAssets>c__Iterator1E <LoadNoActorAssets>c__Iterator1E = new GameLoader.<LoadNoActorAssets>c__Iterator1E();
			<LoadNoActorAssets>c__Iterator1E.InWrapper = InWrapper;
			<LoadNoActorAssets>c__Iterator1E.<$>InWrapper = InWrapper;
			<LoadNoActorAssets>c__Iterator1E.<>f__this = this;
			return <LoadNoActorAssets>c__Iterator1E;
		}

		[DebuggerHidden]
		private IEnumerator AnalyseAgeRecursiveAssets(LoaderHelper loadHelper)
		{
			GameLoader.<AnalyseAgeRecursiveAssets>c__Iterator1F <AnalyseAgeRecursiveAssets>c__Iterator1F = new GameLoader.<AnalyseAgeRecursiveAssets>c__Iterator1F();
			<AnalyseAgeRecursiveAssets>c__Iterator1F.loadHelper = loadHelper;
			<AnalyseAgeRecursiveAssets>c__Iterator1F.<$>loadHelper = loadHelper;
			<AnalyseAgeRecursiveAssets>c__Iterator1F.<>f__this = this;
			return <AnalyseAgeRecursiveAssets>c__Iterator1F;
		}

		[DebuggerHidden]
		private IEnumerator LoadAgeRecursiveAssets(GameLoader.LHCWrapper InWrapper)
		{
			GameLoader.<LoadAgeRecursiveAssets>c__Iterator20 <LoadAgeRecursiveAssets>c__Iterator = new GameLoader.<LoadAgeRecursiveAssets>c__Iterator20();
			<LoadAgeRecursiveAssets>c__Iterator.InWrapper = InWrapper;
			<LoadAgeRecursiveAssets>c__Iterator.<$>InWrapper = InWrapper;
			<LoadAgeRecursiveAssets>c__Iterator.<>f__this = this;
			return <LoadAgeRecursiveAssets>c__Iterator;
		}

		[DebuggerHidden]
		private IEnumerator SpawnStaticActor(GameLoader.LoaderHelperWrapper InWrapper)
		{
			GameLoader.<SpawnStaticActor>c__Iterator21 <SpawnStaticActor>c__Iterator = new GameLoader.<SpawnStaticActor>c__Iterator21();
			<SpawnStaticActor>c__Iterator.InWrapper = InWrapper;
			<SpawnStaticActor>c__Iterator.<$>InWrapper = InWrapper;
			<SpawnStaticActor>c__Iterator.<>f__this = this;
			return <SpawnStaticActor>c__Iterator;
		}

		[DebuggerHidden]
		private IEnumerator SpawnDynamicActor(GameLoader.LoaderHelperWrapper InWrapper)
		{
			GameLoader.<SpawnDynamicActor>c__Iterator22 <SpawnDynamicActor>c__Iterator = new GameLoader.<SpawnDynamicActor>c__Iterator22();
			<SpawnDynamicActor>c__Iterator.InWrapper = InWrapper;
			<SpawnDynamicActor>c__Iterator.<$>InWrapper = InWrapper;
			<SpawnDynamicActor>c__Iterator.<>f__this = this;
			return <SpawnDynamicActor>c__Iterator;
		}

		[DebuggerHidden]
		private IEnumerator PreSpawnSoldiers(GameLoader.LoaderHelperWrapper InWrapper)
		{
			GameLoader.<PreSpawnSoldiers>c__Iterator23 <PreSpawnSoldiers>c__Iterator = new GameLoader.<PreSpawnSoldiers>c__Iterator23();
			<PreSpawnSoldiers>c__Iterator.InWrapper = InWrapper;
			<PreSpawnSoldiers>c__Iterator.<$>InWrapper = InWrapper;
			<PreSpawnSoldiers>c__Iterator.<>f__this = this;
			return <PreSpawnSoldiers>c__Iterator;
		}

		[DebuggerHidden]
		private IEnumerator CoroutineLoad()
		{
			GameLoader.<CoroutineLoad>c__Iterator24 <CoroutineLoad>c__Iterator = new GameLoader.<CoroutineLoad>c__Iterator24();
			<CoroutineLoad>c__Iterator.<>f__this = this;
			return <CoroutineLoad>c__Iterator;
		}

		private void ReleaseMemoryIfNeed()
		{
			if (DeviceCheckSys.GetAvailMemoryMegaBytes() > 100)
			{
				return;
			}
			Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("ActorInfo");
			Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharIcon");
			Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharBattle");
			Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharShow");
			Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharLoading");
			Resources.UnloadUnusedAssets();
			GC.Collect();
		}

		public void AdvanceStopLoad()
		{
			if (this.isLoadStart)
			{
				Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("ActorInfo");
				Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharIcon");
				Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharBattle");
				Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharShow");
				Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharLoading");
				Singleton<CResourceManager>.GetInstance().UnloadUnusedAssets();
				GC.Collect();
				Singleton<EventRouter>.instance.BroadCastEvent(EventID.ADVANCE_STOP_LOADING);
			}
			this.ResetLoader();
		}
	}
}
