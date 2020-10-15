using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading.Tasks;

namespace redemrp_fishing
{
	public class Fishing : BaseScript
	{

		public int ReelInPrompt { get; set; }
		public int ResetCastPrompt { get; set; }
		public int HookPrompt { get; set; }
		public int KeepPrompt { get; set; }
		public int ThrowFishBackPrompt { get; set; }
		public int TargetFish { get; set; }
		public int FishingState_ { get; set; }
		public bool Hooked { get; set; }
		public FishBait CurrentBait { get; set; }
		public bool isReelIn { get; set; }
		public bool isFeeling { get; set; }
		public int time { get; set; }
		public int FXTimer { get; set; }
		public int FindFishTimer { get; set; }
		public int TimeCanBeAdded { get; set; }
		public float tenstion_ { get; set; }
		public bool SellActive { get; set; }
		public bool Selling { get; set; }
		public bool AddItem { get; set; }
		public bool ShowInfo { get; set; }
		public int Info { get; set; }
		public int CatchTimer { get; set; }
		private FishData CurrentFishData { get; set; }

		public Fishing()
		{
			this.ReelInPrompt = CreateReelInPrompt();
			this.ResetCastPrompt = CreateResetCastPrompt();
			this.HookPrompt = CreateHookPrompt();
			this.KeepPrompt = CreateKeepPrompt();
			this.ThrowFishBackPrompt = CreateThrowFishBackPrompt();
			EventHandlers["redemrp_fishing:UseBait"] += new Action<string>(UseBait);
			int sellBlip = Function.Call<int>((Hash)0x554d9d53f696d002, 1664425300, 2998.792f, 477.8959f, 42.0f);
			API.SetBlipSprite(sellBlip, 1106719664, 0);
			Function.Call((Hash)0x9CB1A1623062F402, sellBlip, "Fish Market");
			Tick += FishingCore;
			Tick += FishingMinigame;
			RequestPtfx();

		}

		private static readonly List<FishData> FishDatas = new List<FishData> {
			new FishData(API.GetHashKey("A_C_FishBluegil_01_ms"), FishLocation.River, FishBait.p_finishedragonfly01x, 0.2f, 0.9f, "smallfish", "PROVISION_FISH_BLUEGILL", "PROVISION_BLUEGILL_DESC"),
			new FishData(API.GetHashKey("A_C_FishBluegil_01_sm"), FishLocation.Lake, FishBait.p_baitCheese01x | FishBait.p_baitBread01x, 0.2f, 0.5f, "smallfish", "PROVISION_FISH_BLUEGILL", "PROVISION_BLUEGILL_DESC"),
			new FishData(API.GetHashKey("A_C_FishMuskie_01_lg"), FishLocation.Lake, FishBait.p_FinisdFishlure01x, 6.3f, 9.5f, "largefish", "PROVISION_FISH_MUSKIE", "PROVISION_MUSKIE_DESC"),
			new FishData(API.GetHashKey("A_C_FishBullHeadCat_01_ms"), FishLocation.Swamp, FishBait.p_finishedragonfly01x, 0.25f, 0.9f, "smallfish", "PROVISION_FISH_BULLHEAD_CATFISH", "PROVISION_BULHDCATFSH_DESC"),
			new FishData(API.GetHashKey("A_C_FishBullHeadCat_01_sm"), FishLocation.Swamp, FishBait.p_baitCorn01x | FishBait.p_baitCheese01x, 0.1f, 0.5f, "smallfish", "PROVISION_FISH_BULLHEAD_CATFISH", "PROVISION_BULHDCATFSH_DESC"),
			new FishData(API.GetHashKey("A_C_FishChainPickerel_01_sm"), FishLocation.River, FishBait.p_baitCorn01x | FishBait.p_baitCheese01x, 0.25f, 0.9f, "smallfish", "PROVISION_FISH_CHAIN_PICKEREL", "PROVISION_CHNPKRL_DESC"),
			new FishData(API.GetHashKey("A_C_FishChannelCatfish_01_lg"), FishLocation.Swamp, FishBait.p_finishedragonfly01x, 6.3f, 9.2f, "largefish", "PROVISION_FISH_CHANNEL_CATFISH", "PROVISION_CHNCATFSH_DESC"),
			new FishData(API.GetHashKey("A_C_FishLargeMouthBass_01_ms"), FishLocation.Swamp, FishBait.p_finishdcrawd01x, 1.8f, 2.7f, "mediumfish", "PROVISION_FISH_LARGEMOUTH_BASS", "PROVISION_LRGMTHBASS_DESC"),
			new FishData(API.GetHashKey("A_C_FishLongNoseGar_01_lg"), FishLocation.Swamp, FishBait.p_FinisdFishlure01x, 6.3f, 9.2f, "largefish", "PROVISION_FISH_LONGNOSE_GAR", "PROVISION_LNGNOSEGAR_DESC"),
			new FishData(API.GetHashKey("A_C_FishNorthernPike_01_lg"), FishLocation.River, FishBait.p_baitCricket01x | FishBait.p_finishedragonfly01x, 6.3f, 9.2f, "largefish", "PROVISION_FISH_NORTHERN_PIKE", "PROVISION_NPIKE_DESC"),
			new FishData(API.GetHashKey("A_C_FishPerch_01_ms"), FishLocation.River, FishBait.p_baitBread01x | FishBait.p_baitCorn01x, 0.26f, 0.9f, "smallfish", "PROVISION_FISH_PERCH", "PROVISION_PERCH_DESC"),
			new FishData(API.GetHashKey("A_C_FishPerch_01_sm"), FishLocation.Lake, FishBait.p_baitBread01x | FishBait.p_baitCorn01x, 0.15f, 0.5f, "smallfish", "PROVISION_FISH_PERCH", "PROVISION_PERCH_DESC"),
			new FishData(API.GetHashKey("A_C_FishRainbowTrout_01_ms"), FishLocation.Lake, FishBait.p_baitCricket01x | FishBait.p_FinisdFishlure01x, 0.5f, 1.5f, "mediumfish", "PROVISION_FISH_STEELHEAD_TROUT", "PROVISION_FISH_STHDTROUT_DESC"),
			new FishData(API.GetHashKey("A_C_FishRainbowTrout_01_lg"), FishLocation.Lake, FishBait.p_baitCricket01x | FishBait.p_FinisdFishlure01x, 1.5f, 2.7f, "mediumfish", "PROVISION_FISH_STEELHEAD_TROUT", "PROVISION_FISH_STHDTROUT_DESC"),
			new FishData(API.GetHashKey("A_C_FishRedfinPickerel_01_ms"), FishLocation.River, FishBait.p_FinisdFishlure01x, 0.26f, 0.9f, "smallfish", "PROVISION_FISH_REDFIN_PICKEREL", "PROVISION_RDFNPCKREL_DESC"),
			new FishData(API.GetHashKey("A_C_FishRedfinPickerel_01_sm"), FishLocation.River, FishBait.p_baitCheese01x, 0.17f, 0.5f, "smallfish", "PROVISION_FISH_REDFIN_PICKEREL", "PROVISION_RDFNPCKREL_DESC"),
			new FishData(API.GetHashKey("A_C_FishRockBass_01_ms"), FishLocation.Lake, FishBait.p_baitCorn01x | FishBait.p_baitCheese01x, 0.26f, 0.9f, "smallfish", "PROVISION_FISH_ROCK_BASS", "PROVISION_ROCKBASS_DESC"),
			new FishData(API.GetHashKey("A_C_FishRockBass_01_sm"), FishLocation.Lake, FishBait.p_FinisdFishlure01x, 0.1f, 0.4f, "smallfish", "PROVISION_FISH_ROCK_BASS", "PROVISION_ROCKBASS_DESC"),
			new FishData(API.GetHashKey("A_C_FishSalmonSockeye_01_ms"), FishLocation.River, FishBait.p_baitCricket01x | FishBait.p_finishedragonfly01x, 1.8f, 2.7f, "mediumfish", "PROVISION_FISH_SOCKEYE_SALMON", "PROVISION_SCKEYESAL_DESC"),
			new FishData(API.GetHashKey("A_C_FishSmallMouthBass_01_ms"), FishLocation.River, FishBait.p_baitCheese01x, 1.8f, 2.7f, "mediumfish", "PROVISION_FISH_SMALLMOUTH_BASS", "PROVISION_SMLMTHBASS_DESC"),

		};
		private async Task FishingCore()
		{
			bool ResetCast = Function.Call<bool>((Hash)0x21E60E230086697F, this.ResetCastPrompt);
			if (ResetCast)
			{
				var data = GetFishTaskState();
				data.TransitionFlag = 7;
				data.Tension = 0.0f;
				data.ShakeFightMultiplier = 0.0f;
				data.RodShakeMultiplier = 0.0f;
				API.ClearPedTasks(this.TargetFish, 0, 0);
				this.Hooked = false;
				this.TargetFish = 0;
				this.tenstion_ = 0.0f;
				SetFishTaskState(data);
				Function.Call((Hash)0x8A0FB4D03A630D21, this.ResetCastPrompt, 0);
				Function.Call((Hash)0x71215ACCFDE075EE, this.ResetCastPrompt, 0);
				Function.Call((Hash)0x8A0FB4D03A630D21, this.HookPrompt, 0);
				Function.Call((Hash)0x71215ACCFDE075EE, this.HookPrompt, 0);
				if (this.FishingState_ == 7)
				{
					Random brake = new Random();
					bool brakeRandom = brake.Next(30) == 1;
					if (brakeRandom)
					{
						RemoveBait();

					}
				}

			}

			int PlayerPed = API.PlayerPedId();
			Vector3 coords = API.GetEntityCoords(PlayerPed, true, true);
			if (API.Vdist(coords.X, coords.Y, coords.Z, 2998.792f, 477.8959f, 42.0f) < 2f && !this.Selling)
			{
				if (!this.SellActive)
				{
					this.SellActive = true;
					long str = Function.Call<long>((Hash)0xFA925AC00EB830B9, 10, "LITERAL_STRING", "Press ~INPUT_JUMP~ to sell fish");
					Function.Call((Hash)0xFA233F8FE190514C, str);
					Function.Call((Hash)0xE9990552DEC71600);

				}

				if (API.IsControlJustPressed(0, 0xD9D0E1C0))
				{
					this.Selling = true;
					API.FreezeEntityPosition(PlayerPed, true);
					Exports["progressBars"].startUI(2000, "Selling in progress...");
					await BaseScript.Delay(2000);
					TriggerServerEvent("redemrp_fishing:Sell");
					this.Selling = false;
					API.FreezeEntityPosition(PlayerPed, false);
				}
			}
			else
			{
				if (this.SellActive)
				{
					this.SellActive = false;
					long str = Function.Call<long>((Hash)0xFA925AC00EB830B9, 10, "LITERAL_STRING", " ");
					Function.Call((Hash)0xFA233F8FE190514C, str);
					Function.Call((Hash)0xE9990552DEC71600);

				}

			}
			await BaseScript.Delay(1);
		}

		private async Task FishingMinigame()
		{
			var data = GetFishTaskState();

			this.FishingState_ = data.FishingState;
			if (this.FishingState_ == 6 || this.FishingState_ == 7)
			{
				Function.Call((Hash)0x8A0FB4D03A630D21, this.ResetCastPrompt, 1);
				Function.Call((Hash)0x71215ACCFDE075EE, this.ResetCastPrompt, 1);
			}
			else
			{
				Function.Call((Hash)0x8A0FB4D03A630D21, this.ResetCastPrompt, 0);
				Function.Call((Hash)0x71215ACCFDE075EE, this.ResetCastPrompt, 0);
				this.Hooked = false;

			}

			if (this.FishingState_ == 12)
			{
				if (this.CatchTimer == 0)
				{
					this.CatchTimer = API.GetGameTimer() + 3000;

				}
				if (this.CatchTimer < API.GetGameTimer())
				{
					float weight = data.FishWeight / 54.25f;
					if (!this.ShowInfo)
					{
						this.ShowInfo = true;
						data.Tension = 0.0f;
						data.ShakeFightMultiplier = 0.0f;
						data.RodShakeMultiplier = 0.0f;
						this.tenstion_ = 0.0f;
						SetFishTaskState(data);
						long label = Function.Call<long>((Hash)0xFA925AC00EB830B9, 38, "ITEM_CAUGHT_PUMP", weight * 2.2, this.CurrentFishData.Name);
						int block = API.RequestFlowBlock(API.GetHashKey("SHOP_BROWSING_MAIN_FLOW"));
						this.Info = Function.Call<int>((Hash)0x0C827D175F1292F4, "", "InfoBox");
						Function.Call((Hash)0x58BAA5F635DA2FF4, this.Info, "isVisible", true);
						Function.Call((Hash)0x617FCA1C5652BBAD, this.Info, "itemLabel", label);
						Function.Call((Hash)0x617FCA1C5652BBAD, this.Info, "itemDescription", this.CurrentFishData.Desc);
						Function.Call((Hash)0x10A93C057B6BD944, block);
						Function.Call((Hash)0x3B7519720C9DCB45, block, API.GetHashKey("INFO_CARD_ENTRY_POINT"));
						Function.Call((Hash)0x4C6F2C4B7A03A266, -1295789154, block);

						Function.Call((Hash)0x8A0FB4D03A630D21, this.KeepPrompt, 1);
						Function.Call((Hash)0x71215ACCFDE075EE, this.KeepPrompt, 1);
						Function.Call((Hash)0x8A0FB4D03A630D21, this.ThrowFishBackPrompt, 1);
						Function.Call((Hash)0x71215ACCFDE075EE, this.ThrowFishBackPrompt, 1);
					}

					bool isKeep = Function.Call<bool>((Hash)0x21E60E230086697F, this.KeepPrompt);
					bool isThrow = Function.Call<bool>((Hash)0x21E60E230086697F, this.ThrowFishBackPrompt);
					this.Hooked = false;
					if (isKeep && !this.AddItem)
					{
						data.TransitionFlag = 32;
						this.AddItem = true;
						TriggerServerEvent("redemrp_fishing:AddFish", this.CurrentFishData.Type, weight);
						SetFishTaskState(data);
						data.FishHandle = 0;
						this.Hooked = false;
						RemoveBait();
						int fish = this.TargetFish;
						API.DetachEntity(fish, true, true);
						API.SetEntityAsMissionEntity(fish, true, true);
						API.DeleteEntity(ref fish);
						API.SetEntityCoords(this.TargetFish, 0.0f, 0.0f, 0.0f, false, false, false, false);
						this.TargetFish = 0;
						this.CatchTimer = 0;
						SetFishTaskState(data);

					}
					if (isThrow)
					{
						data.TransitionFlag = 64;
						data.FishHandle = 0;
						this.TargetFish = 0;
						this.CatchTimer = 0;
						SetFishTaskState(data);

					}
				}
			}
			else
			{
				this.AddItem = false;
				if (this.ShowInfo)
				{
					Function.Call((Hash)0x58BAA5F635DA2FF4, this.Info, "isVisible", false);
					this.ShowInfo = false;
				}
				Function.Call((Hash)0x8A0FB4D03A630D21, this.KeepPrompt, 0);
				Function.Call((Hash)0x71215ACCFDE075EE, this.KeepPrompt, 0);
				Function.Call((Hash)0x8A0FB4D03A630D21, this.ThrowFishBackPrompt, 0);
				Function.Call((Hash)0x71215ACCFDE075EE, this.ThrowFishBackPrompt, 0);
			}

			if (this.FishingState_ == 6)
			{
				var bobberCoords = API.GetEntityCoords(data.BobberHandle, true, true);
				var _bobberCoords = API.GetEntityCoords(data.BobberHandle, true, true);
				float distance_van = API.Vdist(bobberCoords.X, bobberCoords.Y, bobberCoords.Z, 3154.16f, 572.1613f, 42.0f);
				if (distance_van > 750.0f)
				{
					Function.Call((Hash)0x71215ACCFDE075EE, this.HookPrompt, 1);
					int gametimer = API.GetGameTimer();
					if (this.FindFishTimer < gametimer && this.TargetFish == 0)
					{
						this.TargetFish = GetClosestFish(data.BobberHandle);
						Debug.WriteLine($"Test{this.TargetFish}");
						this.FindFishTimer = gametimer + 2000;
					}

					if (this.TargetFish != 0)
					{
						API.ClearPedTasks(this.TargetFish, 0, 0);
						API.TaskGoToCoordAnyMeans(this.TargetFish, bobberCoords.X, bobberCoords.Y, bobberCoords.Z, 1.5f, 0, true, 0, 0.5f);
						bobberCoords = API.GetEntityCoords(data.BobberHandle, true, true);
						var fishCoords = API.GetEntityCoords(this.TargetFish, true, true);
						var fishCoords2 = API.GetEntityCoords(this.TargetFish, true, true);

						while (API.Vdist(bobberCoords.X, bobberCoords.Y, bobberCoords.Z, fishCoords.X, fishCoords.Y, fishCoords.Z) > 1.1f && this.FishingState_ == 6)
						{
							await BaseScript.Delay(10);
							fishCoords = API.GetEntityCoords(this.TargetFish, true, true);
							bobberCoords = API.GetEntityCoords(data.BobberHandle, true, true);
							data = GetFishTaskState();
							this.FishingState_ = data.FishingState;
							if (API.Vdist(bobberCoords.X, bobberCoords.Y, bobberCoords.Z, _bobberCoords.X, _bobberCoords.Y, _bobberCoords.Z) > 0.5f)
							{
								API.ClearPedTasks(this.TargetFish, 0, 0);
								API.TaskGoToCoordAnyMeans(this.TargetFish, bobberCoords.X, bobberCoords.Y, bobberCoords.Z, 1.5f, 0, true, 0, 0.5f);
								_bobberCoords = API.GetEntityCoords(data.BobberHandle, true, true);
							}

						}
						API.ClearPedTasks(this.TargetFish, 0, 0);
						API.TaskGoToCoordAnyMeans(this.TargetFish, fishCoords2.X, fishCoords2.Y, fishCoords2.Z, 1.0f, 0, false, 0, 1.0f);
						while (API.Vdist(bobberCoords.X, bobberCoords.Y, bobberCoords.Z, fishCoords.X, fishCoords.Y, fishCoords.Z) < 1.5f && !this.Hooked && this.FishingState_ == 6)
						{
							await BaseScript.Delay(10);
							fishCoords = API.GetEntityCoords(this.TargetFish, true, true);
							bobberCoords = API.GetEntityCoords(data.BobberHandle, true, true);
							Function.Call((Hash)0x8A0FB4D03A630D21, this.HookPrompt, 1);
							bool hookclick = Function.Call<bool>((Hash)0x21E60E230086697F, this.HookPrompt);
							data = GetFishTaskState();
							this.FishingState_ = data.FishingState;
							if (hookclick && !this.Hooked)
							{
								this.Hooked = true;
								API.SetBlockingOfNonTemporaryEvents(this.TargetFish, true);
								Function.Call((Hash)0x1A52076D26E09004, API.PlayerPedId(), this.TargetFish);
								data.FishHandle = this.TargetFish;
								data.TransitionFlag = 4;
								float min = this.CurrentFishData.Min;
								float max = this.CurrentFishData.Max;
								float weight = API.GetRandomFloatInRange(min, max);
								data.FishWeight = weight * 54.25f;
								SetFishTaskState(data);

							}

						}
						Function.Call((Hash)0x8A0FB4D03A630D21, this.HookPrompt, 0);
						if (!this.Hooked && this.TargetFish > 0)
						{
							this.TargetFish = 0;
							this.FindFishTimer = gametimer + 5000;
						}

					}
				}
			}
			else
			{

				Function.Call((Hash)0x8A0FB4D03A630D21, this.HookPrompt, 0);
				Function.Call((Hash)0x71215ACCFDE075EE, this.HookPrompt, 0);
			}

			if (this.FishingState_ == 7)
			{
				Random rng = new Random();
				bool randomBool = rng.Next(2) == 1;
				bool reelIn = API.IsControlPressed(2, 0x8FFC75D6);
				int gametimer = API.GetGameTimer();
				var feelTime = this.time + (5000 - Math.Sqrt((data.FishWeight / 54.25f)) * 900);
				if ((data.FishWeight / 54.25f) > 5f)
				{
					feelTime = this.time + (6000 - Math.Sqrt((data.FishWeight / 54.25f)) * 500);
				}
				if (!this.isFeeling && randomBool && this.isReelIn && (gametimer > feelTime || this.time == 0))
				{
					var playerCoords = API.GetEntityCoords(API.PlayerPedId(), true, true);
					float distance = data.Distance + 8.0f;
					API.ClearPedTasks(data.FishHandle, 0, 0);
					API.TaskSmartFleeCoord(data.FishHandle, playerCoords.X, playerCoords.Y, playerCoords.Z, distance, -1, true, true);
					this.isFeeling = true;
					data.FishsizeIndex = 0;
					this.isReelIn = false;
					Debug.WriteLine($"time added{this.TimeCanBeAdded}");
					this.time = API.GetGameTimer() + 5000 + this.TimeCanBeAdded;
					var fishCoords = API.GetEntityCoords(this.TargetFish, true, true);

				}
				if (reelIn && !this.isReelIn && !this.isFeeling)
				{
					this.isReelIn = true;
					API.ClearPedTasks(data.FishHandle, 0, 0);
					API.TaskGoToEntity(data.FishHandle, API.PlayerPedId(), -1, 1.0f, 1.5f, 0.0f, 0);
					data.FishsizeIndex = 4;
				}
				float X = data.FishingRodX;
				float Y = data.FishingRodY;
				float X_Y = Math.Abs((X - 0.5f)) + Math.Abs((Y - 0.5f));
				if (reelIn)
				{
					float weight = (data.FishWeight / 54.25f);
					if (this.isFeeling)
					{
						if (weight < 0.9f)
						{
							this.tenstion_ += weight / 300f;
						}
						else if (weight < 3.5f && weight > 0.90)
						{
							this.tenstion_ += weight / 600f;
						}
						else if (weight < 10.5f && weight > 3.50f)
						{
							this.tenstion_ += weight / 1000f;
						}
					}
					else
					{
						if (weight < 0.9f)
						{
							this.tenstion_ += weight / 500f;
						}
						else if (weight < 3.5f && weight > 0.90f)
						{
							this.tenstion_ += weight / 700f;
						}
						else if (weight < 10.5f && weight > 3.50f)
						{
							this.tenstion_ += weight / 1200f;
						}
					}

					if (this.tenstion_ + (X_Y / 8) > 1f)
					{
						Random brake = new Random();
						bool brakeRandom = brake.Next(40) == 1;
						if (brakeRandom)
						{
							API.ClearPedTasks(this.TargetFish, 0, 0);
							data.TransitionFlag = 2;
							data.Tension = 0.0f;
							data.ShakeFightMultiplier = 0.0f;
							data.RodShakeMultiplier = 0.0f;
							this.tenstion_ = 0.0f;
							this.tenstion_ = 0.0f;
							API.ClearPedTasks(this.TargetFish, 0, 0);
							this.TargetFish = 0;
							RemoveBait();
							this.Hooked = false;

						}
					}
				}
				else
				{
					this.tenstion_ = Math.Max(this.tenstion_ - 0.0025f, 0f);
				}
				if (this.tenstion_ + (X_Y / 8) > 0.7f)
				{
					this.time = this.time - 15;
				}

				data.Tension = this.tenstion_ + (X_Y / 8);
				data.RodShakeMultiplier = (this.tenstion_ + (X_Y / 8f)) * 1.1f;
				data.ShakeFightMultiplier = (this.tenstion_ + (X_Y / 8f)) * 1.2f;

				if (this.isFeeling)
				{
					PlayPtfx();
				}
				else
				{
					this.TimeCanBeAdded = this.TimeCanBeAdded + 18;
				}

				if (this.isFeeling && gametimer > this.time)
				{
					this.isFeeling = false;
					this.TimeCanBeAdded = 0;
					API.ClearPedTasks(data.FishHandle, 0, 0);

				}
				bool rightClick = API.IsControlPressed(2, 0xF84FA74F);

				data.FishingRodX = rightClick ? 1f - API.GetControlNormal(2, 0xD6C4ECDC) : 0f;
				data.FishingRodY = rightClick ? 1f - API.GetControlNormal(2, 0xE4130778) : 0f;
				Function.Call((Hash)0x8A0FB4D03A630D21, this.ReelInPrompt, 1);
				Function.Call((Hash)0x71215ACCFDE075EE, this.ReelInPrompt, 1);

				SetFishTaskState(data);
				data = GetFishTaskState();
				this.FishingState_ = data.FishingState;
				await BaseScript.Delay(10);
			}
			else
			{
				Function.Call((Hash)0x8A0FB4D03A630D21, this.ReelInPrompt, 0);
				Function.Call((Hash)0x71215ACCFDE075EE, this.ReelInPrompt, 0);

			}

		}
		[StructLayout(LayoutKind.Explicit, Size = 0xC0)]
		[SecurityCritical]
		internal unsafe struct UnsafeFishTaskState
		{
			[FieldOffset(offset: 0x00)] internal int fishingState;
			[FieldOffset(offset: 0x08)] internal float throwingTargetDistance;
			[FieldOffset(offset: 0x10)] internal float distance;
			[FieldOffset(offset: 0x18)] internal float curvature;
			[FieldOffset(offset: 0x20)] internal float unk0;
			[FieldOffset(offset: 0x28)] internal int hookFlag;
			[FieldOffset(offset: 0x30)] internal int transitionFlag;
			[FieldOffset(offset: 0x38)] internal int fishHandle;
			[FieldOffset(offset: 0x40)] internal float fishweight;
			[FieldOffset(offset: 0x48)] internal float fishPower;
			[FieldOffset(offset: 0x50)] internal int scriptTimer;
			[FieldOffset(offset: 0x58)] internal int hookHandle;
			[FieldOffset(offset: 0x60)] internal int bobberHandle;
			[FieldOffset(offset: 0x68)] internal float rodShakeMult;
			[FieldOffset(offset: 0x70)] internal float unk1;
			[FieldOffset(offset: 0x78)] internal float unk2;
			[FieldOffset(offset: 0x80)] internal int unk3;
			[FieldOffset(offset: 0x88)] internal float shakeFightMult;
			[FieldOffset(offset: 0x90)] internal int fishSizeIndex;
			[FieldOffset(offset: 0x98)] internal float unk4;
			[FieldOffset(offset: 0xA0)] internal float unk5;
			[FieldOffset(offset: 0xA8)] internal float tension;
			[FieldOffset(offset: 0xB0)] internal float fishingRodX;
			[FieldOffset(offset: 0xB8)] internal float fishingRodY;
			public FishTaskState GetData()
			{
				return new FishTaskState(fishingState, throwingTargetDistance, distance, curvature, unk0, hookFlag, transitionFlag, fishHandle, fishweight, fishPower, scriptTimer, hookHandle, bobberHandle, rodShakeMult, unk1, unk2, unk3, shakeFightMult, fishSizeIndex, fishingRodX, fishingRodY, tension);
			}
		}

		public struct FishTaskState
		{
			public int FishingState { get; set; }
			public float ThrowingTargetDistance { get; set; }
			public float Distance { get; set; }
			public float Curvature { get; set; }
			public float Unk0 { get; set; }
			public int HookFlag { get; set; }
			public int TransitionFlag { get; set; }
			public int FishHandle { get; set; }
			public float FishWeight { get; set; }
			public float FishPower { get; set; }
			public int ScriptTimer { get; set; }
			public int HookHandle { get; set; }
			public int BobberHandle { get; set; }
			public float RodShakeMultiplier { get; set; }
			public float Unk1 { get; set; }
			public float Unk2 { get; set; }
			public int Unk3 { get; set; }
			public float ShakeFightMultiplier { get; set; }
			public int FishsizeIndex { get; set; }
			public float FishingRodX { get; set; }
			public float FishingRodY { get; set; }
			public float Tension { get; set; }

			public FishTaskState(int fishingState, float throwingTargetDistance, float distance, float curvature, float unk0, int hookFlag, int transitionFlag, int fishHandle, float fishweight, float fishPower, int scriptTimer, int hookHandle, int bobberHandle, float rodShakeMult, float unk1, float unk2, int unk3, float shakeFightMult, int fishSizeIndex, float fishingRodX, float fishingRodY, float tension)
			{
				FishingState = fishingState;
				ThrowingTargetDistance = throwingTargetDistance;
				Distance = distance;
				Curvature = curvature;
				Unk0 = unk0;
				HookFlag = hookFlag;
				TransitionFlag = transitionFlag;
				FishHandle = fishHandle;
				FishWeight = fishweight;
				FishPower = fishPower;
				ScriptTimer = scriptTimer;
				HookHandle = hookHandle;
				BobberHandle = bobberHandle;
				RodShakeMultiplier = rodShakeMult;
				Unk1 = unk1;
				Unk2 = unk2;
				Unk3 = unk3;
				ShakeFightMultiplier = shakeFightMult;
				FishsizeIndex = fishSizeIndex;
				FishingRodX = fishingRodX;
				FishingRodY = fishingRodY;
				Tension = tension;

			}

			internal UnsafeFishTaskState GetStruct()
			{
				return new UnsafeFishTaskState
				{
					fishingState = FishingState,
					throwingTargetDistance = ThrowingTargetDistance,
					distance = Distance,
					curvature = Curvature,
					unk0 = Unk0,
					hookFlag = HookFlag,
					transitionFlag = TransitionFlag,
					fishHandle = FishHandle,
					fishweight = FishWeight,
					fishPower = FishPower,
					scriptTimer = ScriptTimer,
					hookHandle = HookHandle,
					bobberHandle = BobberHandle,
					rodShakeMult = RodShakeMultiplier,
					unk1 = Unk1,
					unk2 = Unk2,
					unk3 = Unk3,
					shakeFightMult = ShakeFightMultiplier,
					fishSizeIndex = FishsizeIndex,
					fishingRodX = FishingRodX,
					fishingRodY = FishingRodY,
					tension = Tension,

				};

			}

		}

		public FishTaskState GetFishTaskState()
		{
			var data = new UnsafeFishTaskState();
			unsafe
			{
				Function.Call((Hash)0xF3735ACD11ACD500, API.PlayerPedId(), ((IntPtr)(&data)).ToInt32());
				return data.GetData();
			}
		}

		public void SetFishTaskState(FishTaskState state)
		{
			var data = state.GetStruct();
			unsafe
			{
				Function.Call((Hash)0xF3735ACD11ACD501, API.PlayerPedId(), ((IntPtr)(&data)).ToInt32());
			}
		}

		private async Task RequestPtfx()
		{
			Function.Call((Hash)0xF66090013DE648D5, "MGFSH");
			API.RequestPtfxAsset();
			API.UseParticleFxAsset("MGFSH");
			while (!Function.Call<bool>((Hash)0xD0976CC34002DB57, "MGFSH"))
			{
				await BaseScript.Delay(100);
			}
			Debug.WriteLine($"Załadowane");

		}
		private bool isFishIntesedted(int model, Vector3 coords)
		{
			var data = FishDatas.FirstOrDefault(i => i.ModelHash == model);
			if (data == null) return false;
			this.CurrentFishData = data;
			return data.FishBait == this.CurrentBait;
		}
		private void UseBait(string bait)
		{
			var data = GetFishTaskState();
			if (data.FishingState == 1)
			{
				if (!Enum.TryParse(bait, true, out FishBait _bait))
				{
					return;

				}
				uint weapon = 0;
				API.GetCurrentPedWeapon(API.PlayerPedId(), ref weapon, false, 0, false);

				if (weapon != 2879944532) return;

				this.CurrentBait = _bait;

				Function.Call((Hash)0x2C28AC30A72722DA, API.PlayerPedId(), Enum.GetName(typeof(FishBait), this.CurrentBait), 0);
				string baitName = Enum.GetName(typeof(FishBait), this.CurrentBait);
				TriggerServerEvent("redemrp_fishing:RemoveBait", baitName);
			}

		}
		private void RemoveBait()
		{
			string baitName = Enum.GetName(typeof(FishBait), this.CurrentBait);
			if (baitName == "p_baitBread01x" || baitName == "p_baitCorn01x" || baitName == "p_baitBread01x" || baitName == "p_baitCheese01x" || baitName == "p_baitWorm01x" || baitName == "p_baitWorm01x")
			{
				Function.Call((Hash)0x9B0C7FA063E67629, API.PlayerPedId(), 0, 0, 1);
				this.CurrentBait = (FishBait)1;

			}
		}
		private async Task PlayPtfx()
		{
			if (this.FXTimer < API.GetGameTimer())
			{
				Random rnd = new Random();
				var fishCoords = API.GetEntityCoords(this.TargetFish, true, true);
				float waterHeight = fishCoords.Z;
				API.GetWaterHeight(fishCoords.X, fishCoords.Y, fishCoords.Z, ref waterHeight);
				API.UseParticleFxAsset("scr_mg_fishing");
				Function.Call(Hash.START_NETWORKED_PARTICLE_FX_NON_LOOPED_AT_COORD, "scr_mg_fish_struggle", fishCoords.X, fishCoords.Y, fishCoords.Z, 0f, 0f, (float)(rnd.NextDouble() * 360.0), 1f, 0, 0, 0);

				Function.Call((Hash)0xDCF5BA95BBF0FABA, API.GetSoundId(), "VFX_SPLASH", fishCoords.X, fishCoords.Y, fishCoords.Z, 1048576000, 0, 0, 1);
				Function.Call((Hash)0x503703EC1781B7D6, API.GetSoundId(), "FishSize", API.PlayerPedId(), 1f);
				this.FXTimer = API.GetGameTimer() + 20;
			}

		}
		public int CreateReelInPrompt()
		{
			string str = "Reel In";
			var prompt = Function.Call<int>((Hash)0x04F97DE45A519419);
			Function.Call((Hash)0xB5352B7494A08258, prompt, 0x8FFC75D6);
			long test = Function.Call<long>((Hash)0xFA925AC00EB830B9, 10, "LITERAL_STRING", str);
			Function.Call((Hash)0x5DD02A8318420DD7, prompt, test);
			Function.Call((Hash)0x8A0FB4D03A630D21, prompt, 0);
			Function.Call((Hash)0x71215ACCFDE075EE, prompt, 0);
			Function.Call((Hash)0xCC6656799977741B, prompt, 1);
			Function.Call((Hash)0xF7AA2696A22AD8B9, prompt);
			return prompt;
		}

		public int CreateResetCastPrompt()
		{
			string str = "Reset Cast";
			var prompt = Function.Call<int>((Hash)0x04F97DE45A519419);
			Function.Call((Hash)0xB5352B7494A08258, prompt, 0xDB096B85);
			long test = Function.Call<long>((Hash)0xFA925AC00EB830B9, 10, "LITERAL_STRING", str);
			Function.Call((Hash)0x5DD02A8318420DD7, prompt, test);
			Function.Call((Hash)0x8A0FB4D03A630D21, prompt, 0);
			Function.Call((Hash)0x71215ACCFDE075EE, prompt, 0);
			Function.Call((Hash)0xCC6656799977741B, prompt, 1);
			Function.Call((Hash)0xF7AA2696A22AD8B9, prompt);
			return prompt;
		}
		public int CreateHookPrompt()
		{
			string str = "Hook Fish";
			var prompt = Function.Call<int>((Hash)0x04F97DE45A519419);
			Function.Call((Hash)0xB5352B7494A08258, prompt, 0xCEFD9220);
			long test = Function.Call<long>((Hash)0xFA925AC00EB830B9, 10, "LITERAL_STRING", str);
			Function.Call((Hash)0x5DD02A8318420DD7, prompt, test);
			Function.Call((Hash)0x8A0FB4D03A630D21, prompt, 0);
			Function.Call((Hash)0x71215ACCFDE075EE, prompt, 0);
			Function.Call((Hash)0xCC6656799977741B, prompt, 1);
			Function.Call((Hash)0xF7AA2696A22AD8B9, prompt);
			return prompt;
		}

		public int CreateKeepPrompt()
		{
			string str = "Keep";
			var prompt = Function.Call<int>((Hash)0x04F97DE45A519419);
			Function.Call((Hash)0xB5352B7494A08258, prompt, 0xCEFD9220);
			long test = Function.Call<long>((Hash)0xFA925AC00EB830B9, 10, "LITERAL_STRING", str);
			Function.Call((Hash)0x5DD02A8318420DD7, prompt, test);
			Function.Call((Hash)0x8A0FB4D03A630D21, prompt, 0);
			Function.Call((Hash)0x71215ACCFDE075EE, prompt, 0);
			Function.Call((Hash)0xCC6656799977741B, prompt, 1);
			Function.Call((Hash)0xF7AA2696A22AD8B9, prompt);
			return prompt;
		}

		public int CreateThrowFishBackPrompt()
		{
			string str = "Throw Back";
			var prompt = Function.Call<int>((Hash)0x04F97DE45A519419);
			Function.Call((Hash)0xB5352B7494A08258, prompt, 0xE30CD707);
			long test = Function.Call<long>((Hash)0xFA925AC00EB830B9, 10, "LITERAL_STRING", str);
			Function.Call((Hash)0x5DD02A8318420DD7, prompt, test);
			Function.Call((Hash)0x8A0FB4D03A630D21, prompt, 0);
			Function.Call((Hash)0x71215ACCFDE075EE, prompt, 0);
			Function.Call((Hash)0xCC6656799977741B, prompt, 1);
			Function.Call((Hash)0xF7AA2696A22AD8B9, prompt);
			return prompt;
		}

		public int GetClosestFish(int bobberHandle)
		{
			var itemSet = API.CreateItemset(true);
			int playerPed = API.PlayerPedId();
			var bobberCoords = API.GetEntityCoords(bobberHandle, true, true);
			int size = Function.Call<int>((Hash)0x59B57C4B06531E1E, bobberCoords.X, bobberCoords.Y, bobberCoords.Z, 12.5f, itemSet, 1);
			int output = 0;
			if (size == 0)
			{
				return 0;
			}
			foreach (int index in Enumerable.Range(0, size - 1))
			{

				int entity = API.GetIndexedItemInItemset(index, itemSet);

				if (!API.IsPedAPlayer(entity))
				{
					if (entity != playerPed)
					{
						if (!API.IsEntityDead(entity))
						{
							var fishCoords = API.GetEntityCoords(entity, true, true);
							if (API.Vdist(bobberCoords.X, bobberCoords.Y, bobberCoords.Z, fishCoords.X, fishCoords.Y, fishCoords.Z) > 2.5f)
							{
								if (isFishIntesedted(API.GetEntityModel(entity), bobberCoords))
								{
									output = entity;

									break;

								}
							}
						}
					}

				}

			}
			if (API.IsItemsetValid(itemSet))
			{

				API.DestroyItemset(itemSet);
			}

			return output;
		}

		internal class FishData
		{
			public int ModelHash { get; }
			public FishLocation FishLocation { get; }
			public FishBait FishBait { get; }
			public float Min { get; }
			public float Max { get; }
			public string Type { get; }
			public string Name { get; }
			public string Desc { get; }


			public FishData(int modelHash, FishLocation fishLocation, FishBait fishBait, float min, float max, string type, string name, string desc)
			{
				ModelHash = modelHash;
				FishLocation = fishLocation;
				FishBait = fishBait;
				Min = min;
				Max = max;
				Type = type;
				Name = name;
				Desc = desc;
			}
		}

		public enum FishBait
		{
			none,
			p_baitBread01x,
			p_baitCorn01x,
			p_baitCheese01x,
			p_baitCricket01x,
			p_finishedragonfly01x,
			p_FinisdFishlure01x,
			p_finishdcrawd01x

		}
		public enum FishLocation
		{
			River,
			Lake,
			Swamp

		}

	}

}