using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Core;
using Game;
using Sein.World;
using UnityEngine;

// Token: 0x020009F5 RID: 2549
public static class Randomizer
{
	public static void initialize()
	{
		Randomizer.OHKO = false;
		Randomizer.ZeroXP = false;
		Randomizer.BonusActive = true;
		Randomizer.GiveAbility = false;
		Randomizer.Chaos = false;
		Randomizer.ChaosVerbose = false;
		Randomizer.Returning = false;
		Randomizer.Sync = false;
		Randomizer.ForceMaps = false;
		Randomizer.SyncMode = 4;
		Randomizer.StringKeyPickupTypes = new List<string> {"TP", "SH", "NO", "WT", "MU", "HN", "WP", "RP", "WS"};
		Randomizer.ShareParams = "";
		RandomizerChaosManager.initialize();
		Randomizer.DamageModifier = 1f;
		Randomizer.Table = new Hashtable();
		Randomizer.GridFactor = 4.0;
		Randomizer.Message = "Good luck on your rando!";
		Randomizer.MessageProvider = (RandomizerMessageProvider)ScriptableObject.CreateInstance(typeof(RandomizerMessageProvider));
		Randomizer.ProgressiveMapStones = true;
		Randomizer.ForceTrees = false;
		Randomizer.CluesMode = false;
		Randomizer.Shards = false;
		Randomizer.WorldTour = false;
		Randomizer.SeedMeta = "";
		Randomizer.MistySim = new WorldEvents();
		Randomizer.MistySim.MoonGuid = new MoonGuid(1061758509, 1206015992, 824243626, -2026069462);
		Randomizer.TeleportTable = new Hashtable();
		Randomizer.TeleportTable["Forlorn"] = "forlorn";
		Randomizer.TeleportTable["Grotto"] = "moonGrotto";
		Randomizer.TeleportTable["Sorrow"] = "valleyOfTheWind";
		Randomizer.TeleportTable["Grove"] = "spiritTree";
		Randomizer.TeleportTable["Swamp"] = "swamp";
		Randomizer.TeleportTable["Valley"] = "sorrowPass";
		Randomizer.TeleportTable["Ginso"] = "ginsoTree";
		Randomizer.TeleportTable["Horu"] = "mountHoru";
		Randomizer.TeleportTable["Glades"] = "sunkenGlades";
		Randomizer.TeleportTable["Blackroot"] = "mangroveFalls";
		Randomizer.Entrance = false;
		Randomizer.DoorTable = new Hashtable();
		Randomizer.ColorShift = false;
		Randomizer.MessageQueue = new Queue();
		Randomizer.MessageQueueTime = 0;
		Randomizer.QueueBash = false;
		Randomizer.BashWasQueued = false;
		Randomizer.BashTap = false;
		Randomizer.fragsEnabled = false;
		Randomizer.TickCounter = 60;
		Randomizer.LockedCount = 0;
		Randomizer.ResetTrackerCount = 0;
		Randomizer.HotCold = false;
		Randomizer.HotColdTypes = new string[] {"EV", "RB17", "RB19", "RB21", "RB28", "SK"};
		Randomizer.HotColdItems = new Dictionary<int, RandomizerHotColdItem>();
		Randomizer.HotColdMaps = new List<int>();
		int HotColdSaveId = 2000;
		Randomizer.HoruScene = "";
		Randomizer.HoruMap = new Hashtable();
		Randomizer.HoruMap["mountHoruStomperSystemsR"] = 2640380;
		Randomizer.HoruMap["mountHoruProjectileCorridor"] = 1720288;
		Randomizer.HoruMap["mountHoruMovingPlatform"] = 3040304;
		Randomizer.HoruMap["mountHoruLaserTurretsR"] = 2160192;
		Randomizer.HoruMap["mountHoruBlockableLasers"] = -919624;
		Randomizer.HoruMap["mountHoruBigPushBlock"] = -199724;
		Randomizer.HoruMap["mountHoruBreakyPathTop"] = -1639664;
		Randomizer.HoruMap["mountHoruFallingBlocks"] = -959848;
		Randomizer.OpenMode = true;
		Randomizer.OpenWorld = false;
		RandomizerDataMaps.LoadGladesData();
		RandomizerDataMaps.LoadGinsoData();
		RandomizerDataMaps.LoadForlornData();
		RandomizerDataMaps.LoadHoruData();
		RandomizerDataMaps.LoadValleyData();
		RandomizerColorManager.Initialize();
		RandomizerPlantManager.Initialize();
		RandomizerRebinding.ParseRebinding();
		RandomizerSettings.ParseSettings();
		Randomizer.RelicZoneLookup = new Dictionary<string, string>();
		RandomizerTrackedDataManager.Initialize();
		RandomizerStatsManager.Initialize();
		Randomizer.RelicCount = 0;
		Randomizer.GrenadeZone = "MIA";
		Randomizer.StompZone = "MIA";
		Randomizer.RepeatablePickups = new HashSet<int>();
		Randomizer.StompTriggers = false;
		Randomizer.SpawnWith = "";
		Randomizer.IgnoreEnemyExp = false;
		bool relicCountOverride = false;
		try {
			if(File.Exists("randomizer.dat")) {
				string[] allLines = File.ReadAllLines("randomizer.dat");
				string[] flagLine = allLines[0].Split(new char[] { '|' });
				string s = flagLine[1];
				string[] flags = flagLine[0].Split(new char[] { ',' });
				Randomizer.SeedMeta = allLines[0];
				foreach (string rawFlag in flags)
				{
					string flag = rawFlag.ToLower();
					if (flag == "ohko")
					{
						Randomizer.OHKO = true;
					}
					if (flag.StartsWith("worldtour"))
					{
						Randomizer.WorldTour = true;
						if(flag.Contains("=")) {
							relicCountOverride = true;
							Randomizer.RelicCount = int.Parse(flag.Substring(10));
						}
					}
					if (flag.StartsWith("sync"))
					{
						Randomizer.Sync = true;
						Randomizer.SyncId = flag.Substring(4);
						RandomizerSyncManager.Initialize();
					}
					if (flag.StartsWith("frags/"))
					{
						Randomizer.fragsEnabled = true;
						string[] fragParams = flag.Split(new char[]
						{
							'/'
						});
						Randomizer.maxFrags =  int.Parse(fragParams[2]);
						Randomizer.fragKeyFinish = int.Parse(fragParams[1]);
					}
					if (flag.StartsWith("mode="))
					{
						string modeStr = flag.Substring(5).ToLower();
						int syncMode;
						if (modeStr == "shared")
						{
							syncMode = 1;
						} else if (modeStr == "none") {
							syncMode = 4;
						} else {
							syncMode = int.Parse(modeStr);
						}
						Randomizer.SyncMode = syncMode;
					}
					if (flag.StartsWith("shared="))
					{
						Randomizer.ShareParams = flag.Substring(7);
					}
					if (flag == "noextraexp")
					{
						Randomizer.IgnoreEnemyExp = true;
					}
					if (flag == "0xp")
					{
						Randomizer.IgnoreEnemyExp = true;
						Randomizer.ZeroXP = true;
					}
					if (flag == "nobonus")
					{
						Randomizer.BonusActive = false;
					}
					if (flag == "nonprogressivemapstones")
					{
						Randomizer.ProgressiveMapStones = false;
					}
					if (flag == "forcetrees")
					{
						Randomizer.ForceTrees = true;
					}
					if (flag == "forcemaps")
					{
						Randomizer.ForceMaps = true;
					}
					if (flag == "clues")
					{
						Randomizer.CluesMode = true;
						RandomizerClues.initialize();
					}
					if (flag == "shards")
					{
						Randomizer.Shards = true;
					}
					if (flag == "entrance")
					{
						Randomizer.Entrance = true;
					}
					if (flag == "closeddungeons")
					{
						Randomizer.OpenMode = false;
					}
					if (flag == "openworld")
					{
						Randomizer.OpenWorld = true;
					}
					if (flag.StartsWith("hotcold="))
					{
						Randomizer.HotCold = true;
						Randomizer.HotColdTypes = rawFlag.Substring(8).Split(new char[]{ '+' });
						Array.Sort(Randomizer.HotColdTypes);
					}
					if (flag.StartsWith("sense="))
					{
						Randomizer.HotColdTypes = rawFlag.Substring(6).Split(new char[] { '+' });
						Array.Sort(Randomizer.HotColdTypes);
					}
					if (flag == "noaltr")
					{
						Randomizer.AltRDisabled = true;
					}
					if (flag == "stomptriggers")
					{
						Randomizer.StompTriggers = true;
					}
				}
				for (int i = 1; i < allLines.Length; i++)
				{
					string[] lineParts = allLines[i].Split(new char[] { '|' });
					int coords;
					int.TryParse(lineParts[0], out coords);
					if (coords == 2) {
						SpawnWith = lineParts[1] + lineParts[2];
						continue;
					}
					int index = Array.BinarySearch<string>(Randomizer.HotColdTypes, lineParts[1]);
					if (index < 0)
					{
						index = -index - 1;
					}
					while (index < Randomizer.HotColdTypes.Length && Randomizer.HotColdTypes[index].Substring(0, 2) == lineParts[1])
					{
						if (Randomizer.HotColdTypes[index] == lineParts[1] || Randomizer.HotColdTypes[index].Substring(2) == lineParts[2])
						{
							if (Math.Abs(coords) > 100)
							{
								Randomizer.HotColdItems.Add(coords, new RandomizerHotColdItem(Randomizer.HashKeyToVector(coords), HotColdSaveId));
								HotColdSaveId++;
							}
							else
							{
								Randomizer.HotColdMaps.Add(coords);
							}
						}
						index++;
					}
					if (Randomizer.StringKeyPickupTypes.Contains(lineParts[1]))
					{
						Randomizer.Table[coords] = new RandomizerAction(lineParts[1], lineParts[2]);
						if(lineParts[1] == "WT") {
							Randomizer.RelicZoneLookup[lineParts[2]] = lineParts[3];
							if(!relicCountOverride) {
								Randomizer.RelicCount++;
							}
						}
						if(lineParts[1] == "RP") {
							Randomizer.RepeatablePickups.Add(coords);
						}
					}
					else
					{
						int id;
						int.TryParse(lineParts[2], out id);
						if (lineParts[1] == "EN")
						{
							// door entries are coord|EN|targetX|targetY
							int doorY;
							int.TryParse(lineParts[3], out doorY);
							Randomizer.DoorTable[coords] = new Vector3((float)id, (float)doorY);
						}
						else 
						{
							Randomizer.Table[coords] = new RandomizerAction(lineParts[1], id);
							if (lineParts[1] == "SK") {
								if(id == 51) {
									GrenadeZone = lineParts[3];
								} else if(id == 4) {
									StompZone = lineParts[3];
								}
							}
							if (Randomizer.CluesMode && lineParts[1] == "EV" && id % 2 == 0)
							{
								RandomizerClues.AddClue(lineParts[3], id / 2);
							}
						}
					}
				}
				Randomizer.HotColdMaps.Sort();
				if (Randomizer.CluesMode) {
					RandomizerClues.FinishClues();
				}
			} else {
				Randomizer.printInfo("Error: randomizer.dat not found");
			}
		}
		catch(Exception e) {
			Randomizer.printInfo("Error parsing randomizer.dat:" + e.Message, 300);
		}
	RandomizerBonusSkill.Reset();
	}

	public static void getPickup()
	{
		Randomizer.getPickup(Characters.Sein.Position);
	}

	public static void WarpTo(Vector3 position, int warpDelay) {
		Randomizer.Warping = warpDelay;
		Randomizer.WarpTarget = position;
		if (!Characters.Sein.Controller.CanMove || !Characters.Sein.Active || Characters.Sein.IsSuspended)
		{
			DelayedWarp = true;
			return;
		}
		if(Characters.Sein.Abilities.Dash && Characters.Sein.Abilities.Dash.IsDashingOrChangeDashing)
			Characters.Sein.Abilities.Dash.StopDashing();
		Characters.Sein.Position = position;
		Characters.Sein.Speed = new Vector3(0f, 0f);
		Characters.Ori.Position = new Vector3(position.x, position.y+5);
		Scenes.Manager.SetTargetPositions(Characters.Sein.Position);
		UI.Cameras.Current.CameraTarget.SetTargetPosition(Characters.Sein.Position);
		UI.Cameras.Current.MoveCameraToTargetInstantly(true);
	}

	public static void returnToStart()
	{
		if (Characters.Sein.Abilities.Carry.IsCarrying || !Characters.Sein.Controller.CanMove || !Characters.Sein.Active)
			return;
		if (Items.NightBerry != null)
		{
			Items.NightBerry.transform.position = new Vector3(-755f, -400f);
		}
		RandomizerStatsManager.WarpedToStart();
		RandomizerBonusSkill.LastAltR = Characters.Sein.Position;
		Randomizer.Returning = true;
		Characters.Sein.Position = new Vector3(189f, -215f);
		Characters.Sein.Speed = new Vector3(0f, 0f);
		Characters.Ori.Position = new Vector3(190f, -210f);
		Scenes.Manager.SetTargetPositions(Characters.Sein.Position);
		UI.Cameras.Current.CameraTarget.SetTargetPosition(Characters.Sein.Position);
		UI.Cameras.Current.MoveCameraToTargetInstantly(true);
		int value = World.Events.Find(Randomizer.MistySim).Value;
		if (value != 1 && value != 8)
		{
			World.Events.Find(Randomizer.MistySim).Value = 10;
		}
	}

	public static void getEvent(int ID)
	{
		RandomizerBonus.CollectPickup();
		if (Randomizer.ColorShift)
		{
			Randomizer.changeColor();
		}
		RandomizerSwitch.GivePickup((RandomizerAction)Randomizer.Table[ID * 4], ID * 4, true);
	}

	public static void showHint(string message)
	{
		LastMessageCredits = false;
		Randomizer.Message = message;
		Randomizer.MessageQueue.Enqueue(message);
	}

	public static void showHint(string message, int frames)
	{
		LastMessageCredits = false;
		Randomizer.Message = message;
		Randomizer.MessageQueue.Enqueue(new object[] {message, frames});
	}

	public static void printInfo(string message)
	{
		Randomizer.MessageQueue.Enqueue(message);
	}

	public static void printInfo(string message, int frames)
	{
		Randomizer.MessageQueue.Enqueue(new object[] {message, frames});
	}

	public static void playLastMessage()
	{
		if(LastMessageCredits)
			Randomizer.showCredits(Randomizer.Message, 5);
		else
			Randomizer.MessageQueue.Enqueue(Randomizer.Message);
	}

	public static void log(string message)
	{
		StreamWriter streamWriter = File.AppendText("randomizer.log");
		streamWriter.WriteLine(message);
		streamWriter.Flush();
		streamWriter.Dispose();
	}

	public static bool WindRestored()
	{
		return Sein.World.Events.WindRestored && Scenes.Manager.CurrentScene != null && Scenes.Manager.CurrentScene.Scene != "forlornRuinsResurrection" && Scenes.Manager.CurrentScene.Scene != "forlornRuinsRotatingLaserFlipped";
	}

	public static void getSkill()
	{
		Characters.Sein.Inventory.IncRandomizerItem(27, 1);
		Randomizer.getPickup();
		Randomizer.showProgress();
	}

	public static void hintAndLog(float x, float y)
	{
		string message = ((int)x).ToString() + " " + ((int)y).ToString();
		Randomizer.showHint(message);
		Randomizer.log(message);
	}

	public static int GetHashKey(Vector3 position)
	{
		int baseId = (int)(Math.Floor((double)((int)position.x) / Randomizer.GridFactor) * Randomizer.GridFactor) * 10000 + (int)(Math.Floor((double)((int)position.y) / Randomizer.GridFactor) * Randomizer.GridFactor);
		if(Randomizer.Table.ContainsKey(baseId)) 
			return baseId;
		
		for (int x = -1; x <= 1; x++)
		{
			for (int y = -1; y <= 1; y++)
			{
				int offsetCoord = baseId + (int)Randomizer.GridFactor * (10000 * x + y);
				if (Randomizer.Table.ContainsKey(offsetCoord))
					return offsetCoord;
			}
		}
		for (int x = -2; x <= 2; x += 4)
		{
			for (int y = -1; y <= 1; y++)
			{
				int offsetCoord = baseId + (int)Randomizer.GridFactor * (10000 * x + y);
				if (Randomizer.Table.ContainsKey(offsetCoord))
					return offsetCoord;
			}
		}
		Randomizer.printInfo("Error finding pickup at " + ((int)position.x).ToString() + ", " + ((int)position.y).ToString());
		return -1;

	}

	public static void getPickup(Vector3 position)
	{
		try {
			RandomizerBonus.CollectPickup();
			RandomizerStatsManager.IncPickup();
			if (Randomizer.ColorShift)
			{
				Randomizer.changeColor();
			}
			int hashKey = GetHashKey(position);
			if (hashKey != -1)
			{
				RandomizerSwitch.GivePickup((RandomizerAction)Randomizer.Table[hashKey], hashKey, true);
				if (Randomizer.HotColdItems.ContainsKey(hashKey))
				{
					Characters.Sein.Inventory.SetRandomizerItem(Randomizer.HotColdItems[hashKey].Id, 1);
					RandomizerColorManager.UpdateHotColdTarget();
				}
				return;
			}
		}
		catch(Exception e) {
			Randomizer.LogError("GetPickup: " + e.Message);
		}
	}

	public static void Update()
	{
		Randomizer.UpdateMessages();
		if (Characters.Sein && SkillTreeManager.Instance != null && SkillTreeManager.Instance.NavigationManager.IsVisible)
		{
			if (Characters.Sein.IsSuspended)
			{
				SkillTreeManager.Instance.NavigationManager.FadeAnimator.SetParentOpacity(1f);
			}
			else
			{
				SkillTreeManager.Instance.NavigationManager.FadeAnimator.SetParentOpacity(RandomizerSettings.AbilityMenuOpacity);
			}
		}
		Randomizer.Tick();
		if (Characters.Sein && !Characters.Sein.IsSuspended)
		{
			RandomizerBonus.Update();
			if (!Randomizer.ColorShift)
			{
				RandomizerColorManager.UpdateColors();
			}
			Randomizer.UpdateHoruCutsceneStatus();
			if (Characters.Sein.Inventory.GetRandomizerItem(82) > 0 && Items.NightBerry != null)
			{
				Items.NightBerry.transform.position = new Vector3(-910f, -300f);
				Characters.Sein.Inventory.SetRandomizerItem(82, 0);
			}
			if(RandomizerBonusSkill.LevelExplosionCooldown > 0)
			{
				RandomizerBonusSkill.LevelExplosionCooldown--;
				if(RandomizerBonusSkill.LevelExplosionCooldown > 10) {
					Characters.Sein.Energy.SetCurrent(RandomizerBonusSkill.OldEnergy);
					Characters.Sein.Mortality.Health.SetAmount(RandomizerBonusSkill.OldHealth);
				}
			}
			if (Randomizer.Chaos)
			{
				RandomizerChaosManager.Update();
			}
			if (Randomizer.Sync)
			{
				RandomizerSyncManager.Update();
			}
			if (Randomizer.Warping > 0) {
				if (Randomizer.DelayedWarp)
				{
					Randomizer.DelayedWarp = false;
					Randomizer.WarpTo(Randomizer.WarpTarget, Randomizer.Warping);
				} else {
					Characters.Sein.Position = Randomizer.WarpTarget;
					Characters.Sein.Speed = new Vector3(0f, 0f);
					Characters.Ori.Position = new Vector3(Randomizer.WarpTarget.x, Randomizer.WarpTarget.y-5);
					bool loading = false;
					foreach (SceneManagerScene sms in Scenes.Manager.ActiveScenes)
					{
						if (sms.CurrentState == SceneManagerScene.State.Loading)
						{
							loading = true;
							break;
						}
					}
					if(!loading)
					Randomizer.Warping--;
					if(Randomizer.Warping == 0 && Randomizer.SaveAfterWarp)
					{
						GameController.Instance.CreateCheckpoint();
						GameController.Instance.SaveGameController.PerformSave();
						Randomizer.SaveAfterWarp = false;
					}
				}				
			} else if (Randomizer.Returning)
			{
				Characters.Sein.Position = new Vector3(189f, -215f);
				if (Scenes.Manager.CurrentScene.Scene == "sunkenGladesRunaway")
				{
					Randomizer.Returning = false;
				}
			}
		}
		if (CreditsActive)
			return;
		if (RandomizerRebinding.ReloadSeed.IsPressed())
		{
			Randomizer.initialize();
			Randomizer.showSeedInfo();
			return;
		}
		if (RandomizerRebinding.ShowStats.IsPressed() && Characters.Sein)
		{
			RandomizerStatsManager.ShowStats(10);
			return;
		}
		if (RandomizerRebinding.ListTrees.IsPressed() && Characters.Sein)
		{
			Randomizer.MessageQueueTime = 0;
			RandomizerTrackedDataManager.ListTrees();
			return;
		}
		if (RandomizerRebinding.ListRelics.IsPressed() && Characters.Sein)
		{
			Randomizer.MessageQueueTime = 0;
			RandomizerTrackedDataManager.ListRelics();
			return;
		}
		if (RandomizerRebinding.ListMapAltars.IsPressed() && Characters.Sein)
		{
			Randomizer.MessageQueueTime = 0;
			RandomizerTrackedDataManager.ListMapstones();
			return;
		}
		if (RandomizerRebinding.ListTeleporters.IsPressed() && Characters.Sein)
		{
			Randomizer.MessageQueueTime = 0;
			RandomizerTrackedDataManager.ListTeleporters();
			return;
		}
		if (RandomizerRebinding.BonusSwitch.IsPressed() && Characters.Sein)
		{
			RandomizerBonusSkill.SwitchBonusSkill();
			return;
		}
		if (RandomizerRebinding.BonusToggle.IsPressed() && Characters.Sein)
		{
			RandomizerBonusSkill.ActivateBonusSkill();
			return;
		}
		if (RandomizerRebinding.BonusSwitch.IsPressed() && Characters.Sein)
		{
			RandomizerBonusSkill.SwitchBonusSkill();
			return;
		}
		if (RandomizerRebinding.ReplayMessage.IsPressed())
		{
			Randomizer.playLastMessage();
			return;
		}
		if (RandomizerRebinding.ReturnToStart.IsPressed() && Characters.Sein && Randomizer.Warping <= 0)
		{
			if(Randomizer.AltRDisabled || RandomizerBonus.AltRDisabled())
			{
				Randomizer.printInfo("Return to start is disabled!");
				return;
			}
			Randomizer.returnToStart();
			return;
		}
		if (RandomizerRebinding.ShowProgress.IsPressed() && Characters.Sein)
		{
			Randomizer.MessageQueueTime = 0;
			Randomizer.showProgress();
			return;
		}
		if (RandomizerRebinding.ColorShift.IsPressed())
		{
			string obj = "Color shift enabled";
			if (Randomizer.ColorShift)
			{
				obj = "Color shift disabled";
			}
			else
			{
				Randomizer.changeColor();
			}
			Randomizer.ColorShift = !Randomizer.ColorShift;
			Randomizer.printInfo(obj);
		}
		if (RandomizerRebinding.ToggleChaos.IsPressed() && Characters.Sein)
		{
			if (Randomizer.Chaos)
			{
				Randomizer.showChaosMessage("Chaos deactivated");
				Randomizer.Chaos = false;
				RandomizerChaosManager.ClearEffects();
				return;
			}
			Randomizer.showChaosMessage("Chaos activated");
			Randomizer.Chaos = true;
			return;
		}
		else if (RandomizerRebinding.ChaosVerbosity.IsPressed() && Randomizer.Chaos)
		{
			Randomizer.ChaosVerbose = !Randomizer.ChaosVerbose;
			if (Randomizer.ChaosVerbose)
			{
				Randomizer.showChaosMessage("Chaos messages enabled");
				return;
			}
			Randomizer.showChaosMessage("Chaos messages disabled");
			return;
		}
		else
		{
			if (RandomizerRebinding.ForceChaosEffect.IsPressed() && Randomizer.Chaos && Characters.Sein)
			{
				RandomizerChaosManager.SpawnEffect();
				return;
			}
			return;
		}
	}

	public static void showChaosEffect(string message)
	{
		if (Randomizer.ChaosVerbose)
		{
			Randomizer.printInfo(message);
		}
	}

	public static void showChaosMessage(string message)
	{
		Randomizer.printInfo(message);
	}

	public static void getMapStone()
	{
		if (!Randomizer.ProgressiveMapStones) {
			Randomizer.getPickup();
			return;
		}
		RandomizerBonus.CollectMapstone();
		RandomizerStatsManager.FoundMapstone();

		if (Randomizer.ColorShift) {
			Randomizer.changeColor();
		}
		RandomizerSwitch.GivePickup((RandomizerAction)Randomizer.Table[20 + RandomizerBonus.MapStoneProgression() * 4], 20 + RandomizerBonus.MapStoneProgression() * 4, true);
	}

	public static void showProgress()
	{
		string text = "";
		if(Randomizer.ForceTrees || Randomizer.CluesMode)
		{
			if (RandomizerBonus.SkillTreeProgression() == 10)
			{
				text += "$Trees (10/10)$  ";
			}
			else
			{
				text = text + "Trees (" + RandomizerBonus.SkillTreeProgression().ToString() + "/10)  ";
			}
		}	
		if (Randomizer.WorldTour && Characters.Sein) {
			int relics = Characters.Sein.Inventory.GetRandomizerItem(302);
			if(relics < Randomizer.RelicCount) {
				text += "Relics (" + relics.ToString() + "/"+Randomizer.RelicCount.ToString() + ") ";
			} else {
				text += "$Relics (" + relics.ToString() + "/"+Randomizer.RelicCount.ToString() + ")$ ";
			}
		}
		if (RandomizerBonus.MapStoneProgression() == 9 && Randomizer.ForceMaps)
		{
			text += "$Maps (9/9)$  ";
		}
		else
		{
			text = text + "Maps (" + RandomizerBonus.MapStoneProgression().ToString() + "/9)  ";
		}
		text = text + "Total (" + RandomizerBonus.GetPickupCount().ToString() + "/256)\n";
		if (Randomizer.CluesMode)
		{
			text += RandomizerClues.GetClues();
		}
		else
		{
			if (Keys.GinsoTree)
			{
				text += "*WV (3/3)*  ";
			}
			else
			{
				text = text + " *WV* (" + RandomizerBonus.WaterVeinShards().ToString() + "/3)  ";
			}
			if (Keys.ForlornRuins)
			{
				text += "#GS (3/3)#  ";
			}
			else
			{
				text = text + "#GS# (" + RandomizerBonus.GumonSealShards().ToString() + "/3)  ";
			}
			if (Keys.MountHoru)
			{
				text += "@SS (3/3)@";
			}
			else
			{
				text = text + " @SS@ (" + RandomizerBonus.SunstoneShards().ToString() + "/3)";
			}
		}
		if (Randomizer.fragsEnabled)
		{
			text = string.Concat(new string[] { text, " Frags: (", RandomizerBonus.WarmthFrags().ToString(), "/", Randomizer.fragKeyFinish.ToString(), ")" });
		}
		if(RandomizerBonus.ForlornEscapeHint())
		{
            string s_color = "";
            string g_color = "";
         	if(Characters.Sein)
         	{
	            if(Characters.Sein.PlayerAbilities.HasAbility(AbilityType.Stomp))
	                s_color = "$";
	            if(Characters.Sein.PlayerAbilities.HasAbility(AbilityType.Grenade))
	                g_color = "$";         		
         	}

			text += "\n" +s_color + "Stomp: " + StompZone + s_color + g_color+ "    Grenade: "+ GrenadeZone + g_color;
		}
		Randomizer.printInfo(text);
	}

	public static void showSeedInfo()
	{
		string obj = "v3.0 - seed loaded: " + Randomizer.SeedMeta;
		Randomizer.printInfo(obj);
	}

	public static void changeColor()
	{
		if (Characters.Sein)
		{
			Characters.Sein.PlatformBehaviour.Visuals.SpriteRenderer.material.color = new Color(FixedRandom.Values[0], FixedRandom.Values[1], FixedRandom.Values[2], 0.5f);
		}
	}

	public static void UpdateMessages()
	{
		if (Randomizer.MessageQueueTime <= 0)
		{
			if (Randomizer.MessageQueue.Count == 0)
			{
				return;
			}
			object queueItem = Randomizer.MessageQueue.Dequeue();
			string message;
			if (queueItem is object[])
			{
				message = (string)((object[])queueItem)[0];
				Randomizer.MessageQueueTime = (int)((object[])queueItem)[1] / 2;
			}
			else
			{
				message = (string)queueItem;
				Randomizer.MessageQueueTime = 60;
			}
			Randomizer.MessageProvider.SetMessage(message);
			UI.Hints.Show(Randomizer.MessageProvider, HintLayer.GameSaved, (float)Randomizer.MessageQueueTime / 30f + 1f);
		}
		Randomizer.MessageQueueTime--;
	}

	public static void OnDeath()
	{
		if (Randomizer.Sync)
		{
			RandomizerSyncManager.onDeath();
		}
		RandomizerBonusSkill.OnDeath();
		RandomizerTrackedDataManager.UpdateBitfields();
		RandomizerStatsManager.OnDeath();
	}

	public static void OnSave()
	{
		if (Randomizer.Sync)
		{
			RandomizerSyncManager.onSave();
		}
		RandomizerBonusSkill.OnSave();
	}

	public static bool canFinalEscape()
	{
		if (Randomizer.fragsEnabled && RandomizerBonus.WarmthFrags() < Randomizer.fragKeyFinish)
		{
			Randomizer.printInfo(string.Concat(new string[]
			{
				"Frags: (",
				RandomizerBonus.WarmthFrags().ToString(),
				"/",
				Randomizer.fragKeyFinish.ToString(),
				")"
			}));
			return false;
		}
		if (Randomizer.WorldTour) {
			int relics = Characters.Sein.Inventory.GetRandomizerItem(302);
			if(relics < Randomizer.RelicCount) {
				Randomizer.printInfo("Relics (" + relics.ToString() + "/" + Randomizer.RelicCount.ToString() + ")");
				return false;
			}
		}
		if (Randomizer.ForceTrees && RandomizerBonus.SkillTreeProgression() < 10)
		{
			Randomizer.printInfo("Trees (" + RandomizerBonus.SkillTreeProgression().ToString() + "/10)");
			return false;
		}
		if (Randomizer.ForceMaps && RandomizerBonus.MapStoneProgression() < 9)
		{
			Randomizer.printInfo("Maps (" + RandomizerBonus.MapStoneProgression().ToString() + "/9)");
			return false;
		}
		return true;
	}

	public static void EnterDoor(Vector3 position)
	{
		if (!Randomizer.Entrance)
		{
			return;
		}
		int num = (int)(Math.Floor((double)((int)position.x) / Randomizer.GridFactor) * Randomizer.GridFactor) * 10000 + (int)(Math.Floor((double)((int)position.y) / Randomizer.GridFactor) * Randomizer.GridFactor);
		if (Randomizer.DoorTable.ContainsKey(num))
		{
			Characters.Sein.Position = (Vector3)Randomizer.DoorTable[num];
			return;
		}
		for (int i = -1; i <= 1; i++)
		{
			for (int j = -1; j <= 1; j++)
			{
				if (Randomizer.DoorTable.ContainsKey(num + (int)Randomizer.GridFactor * (10000 * i + j)))
				{
					Characters.Sein.Position = (Vector3)Randomizer.DoorTable[num + (int)Randomizer.GridFactor * (10000 * i + j)];
					return;
				}
			}
		}
		for (int k = -2; k <= 2; k += 4)
		{
			for (int l = -1; l <= 1; l++)
			{
				if (Randomizer.DoorTable.ContainsKey(num + (int)Randomizer.GridFactor * (10000 * k + l)))
				{
					Characters.Sein.Position = (Vector3)Randomizer.DoorTable[num + (int)Randomizer.GridFactor * (10000 * k + l)];
					return;
				}
			}
		}
		Randomizer.showHint("Error using door at " + ((int)position.x).ToString() + ", " + ((int)position.y).ToString());
	}

	public static void getSkill(int tree)
	{
		RandomizerTrackedDataManager.SetTree(tree);
		Randomizer.getSkill();
	}

	public static int ordHash(string s)
	{
		int num = 0;
		foreach (char c in s)
		{
			num += (int)c;
		}
		return num;
	}

	// Token: 0x0600381D RID: 14365
	public static void UpdateHoruCutsceneStatus()
	{
		if (!Characters.Sein.Controller.CanMove)
		{
			if (Randomizer.HoruScene != "")
			{
				if (Randomizer.HoruScene != Scenes.Manager.CurrentScene.Scene && Scenes.Manager.CurrentScene.Scene == "mountHoruHubMid")
				{
					Randomizer.getPickup(new Vector3(0f, (float)((int)Randomizer.HoruMap[Randomizer.HoruScene])));
				}
				Randomizer.HoruScene = Scenes.Manager.CurrentScene.Scene;
				return;
			}
			if (Scenes.Manager.CurrentScene.Scene.StartsWith("mountHoru"))
			{
				Randomizer.HoruScene = Scenes.Manager.CurrentScene.Scene;
				return;
			}
		}
		else
		{
			Randomizer.HoruScene = "";
		}
	} 

	public static void PrintImmediately(string creditText, int seconds, bool mute, bool setMessage, bool devOnly)
	{
		if(devOnly && !RandomizerSettings.Dev)
			return;
		MessageProvider.SetMessage(creditText);
		if(mute)
		{
			CachedVolume = Math.Max(GameSettings.Instance.SoundEffectsVolume, CachedVolume);
			GameSettings.Instance.SoundEffectsVolume = 0f;
		}
		UI.Hints.Show(Randomizer.MessageProvider, HintLayer.GameSaved, (float)seconds);
		if(mute)
			ResetVolume = 3;
		if(setMessage)
		{
			Message = creditText;
			LastMessageCredits = true;
		}
	}

	public static void LogError(string errorText) {
		Randomizer.log(errorText);
		Randomizer.PrintImmediately(errorText, 15, false, false, true);
	}

	public static void showCredits(string creditText, int seconds)
	{
		PrintImmediately(creditText, seconds, true, true, false);
	}

	// Token: 0x06003848 RID: 14408
	public static void Tick()
	{
		Randomizer.TickCounter--;
		if (Randomizer.TickCounter <= 0)
		{
			Randomizer.TickCounter = 60;
			if(ResetVolume == 1)
			{
				ResetVolume = 0;
				GameSettings.Instance.SoundEffectsVolume = CachedVolume;
			} else if(ResetVolume > 1) {
				ResetVolume--;
			}

			if(RepeatableCooldown > 0)
				RepeatableCooldown--;
			if(RandomizerStatsManager.StatsTimer > 0)
				RandomizerStatsManager.StatsTimer--;
			RandomizerStatsManager.IncTime();
			if(Scenes.Manager.CurrentScene != null)
			{
				string scene = Scenes.Manager.CurrentScene.Scene;
				if(scene == "titleScreenSwallowsNest")
				{
					ResetTrackerCount++;
					if(ResetTrackerCount > 10)
					{
						RandomizerTrackedDataManager.Reset();
						ResetTrackerCount = 0;
					}
					if(RandomizerCreditsManager.CreditsDone)
					{
						RandomizerCreditsManager.CreditsDone = false;
					}
				}
				else if(scene == "creditsScreen")
				{
					if(!CreditsActive && !RandomizerCreditsManager.CreditsDone)
					{
						CreditsActive = true;
						RandomizerCreditsManager.Initialize();
					}
				}
				else if (scene == "theSacrifice" && RandomizerStatsManager.Active)
				{
					foreach (SceneManagerScene sms in Scenes.Manager.ActiveScenes)
					{
						if (sms.MetaData.Scene == "creditsScreen" && sms.CurrentState == SceneManagerScene.State.Loading)
						{
							RandomizerStatsManager.Finish();
						}
					}
				}
			}

			if(CreditsActive && !RandomizerCreditsManager.CreditsDone)
					RandomizerCreditsManager.Tick();

			if(Characters.Sein)
			{
				if(JustSpawned && SpawnWith != "" && Characters.Sein.Inventory) {
					JustSpawned = false;
					RandomizerAction spawnItem;
					if(Randomizer.StringKeyPickupTypes.Contains(SpawnWith.Substring(0, 2)))
						spawnItem = new RandomizerAction(SpawnWith.Substring(0, 2), SpawnWith.Substring(2));
					else
						spawnItem = new RandomizerAction(SpawnWith.Substring(0, 2), int.Parse(SpawnWith.Substring(2)));
					RandomizerSwitch.GivePickup(spawnItem, 2, true);
				}
				if(!Characters.Sein.IsSuspended && Scenes.Manager.CurrentScene != null)
				{
					ResetTrackerCount = 0;
					RandomizerTrackedDataManager.UpdateBitfields();
					RandomizerColorManager.UpdateHotColdTarget();
					if (Characters.Sein.Position.y > 937f && Sein.World.Events.WarmthReturned && Scenes.Manager.CurrentScene.Scene == "ginsoTreeWaterRisingEnd")
					{
						if (Characters.Sein.Abilities.Bash.IsBashing)
						{
							Characters.Sein.Abilities.Bash.BashGameComplete(0f);
						}
						Characters.Sein.Position = new Vector3(750f, -120f);
						return;
					}
					if (Scenes.Manager.CurrentScene.Scene == "catAndMouseResurrectionRoom" && !Randomizer.canFinalEscape())
					{
						if (Randomizer.Entrance)
						{
							Randomizer.EnterDoor(new Vector3(-242f, 489f));
							return;
						}
						Characters.Sein.Position = new Vector3(20f, 105f);
						return;
					}
					else if (!Characters.Sein.Controller.CanMove && Scenes.Manager.CurrentScene.Scene == "moonGrottoGumosHideoutB")
					{
						Randomizer.LockedCount++;
						if (Randomizer.LockedCount >= 4)
						{
							GameController.Instance.ResetInputLocks();
							return;
						}
					}
					else
					{
						Randomizer.LockedCount = 0;
					}
				}
			}

		}
	}

	public static Vector3 HashKeyToVector(int key)
	{
		if (key >= 0)
		{
			if (key % 10000 > 5000)
			{
				return new Vector3((float)key / 10000f, -(float)(10000 - key % 10000));
			}
			return new Vector3((float)key / 10000f, (float)(key % 10000));
		}
		else
		{
			if (-key % 10000 > 5000)
			{
				return new Vector3((float)key / 10000f, (float)(10000 - -(float)key % 10000));
			}
			return new Vector3((float)key / 10000f, -(float)(-(float)key % 10000));
		}
	}

	public static int RepeatableCheck(Vector3 position){
		// 2: grabbable, 1: cooldown, 0: not repeatable
		try{
			if(RepeatablePickups.Contains(GetHashKey(position)))
			{
				if(RepeatableCooldown <= 0)
				{
					RepeatableCooldown = 2;
					return 2;
				} else {
					return 1;
				}
			} 			
		}
		catch(Exception e) {
			Randomizer.LogError("RepeatableCheck: " + e.Message);
		}
		return 0;
	}

	// Token: 0x0400322E RID: 12846
	public static Hashtable Table;

	// Token: 0x0400322F RID: 12847
	public static bool GiveAbility;

	// Token: 0x04003230 RID: 12848
	public static double GridFactor;

	// Token: 0x04003231 RID: 12849
	public static RandomizerMessageProvider MessageProvider;

	// Token: 0x04003232 RID: 12850
	public static bool OHKO;

	// Token: 0x04003233 RID: 12851
	public static bool ZeroXP;

	// Token: 0x04003234 RID: 12852
	public static bool BonusActive;

	// Token: 0x04003235 RID: 12853
	public static string Message;

	// Token: 0x04003236 RID: 12854
	public static bool Chaos;

	// Token: 0x04003237 RID: 12855
	public static bool ChaosVerbose;

	// Token: 0x04003238 RID: 12856
	public static float DamageModifier;

	// Token: 0x04003239 RID: 12857
	public static bool ProgressiveMapStones;

	// Token: 0x0400323A RID: 12858
	public static bool ForceTrees;

	// Token: 0x0400323B RID: 12859
	public static string SeedMeta;

	// Token: 0x0400323C RID: 12860
	public static Hashtable TeleportTable;

	// Token: 0x0400323D RID: 12861
	public static WorldEvents MistySim;

	// Token: 0x0400323E RID: 12862
	public static bool Returning;

	// Token: 0x0400323F RID: 12863
	public static bool CluesMode;

	public static bool Shards;

	// Token: 0x04003240 RID: 12864
	public static bool ColorShift;

	// Token: 0x04003241 RID: 12865
	public static Queue MessageQueue;

	// Token: 0x04003242 RID: 12866
	public static int MessageQueueTime;

	// Token: 0x04003243 RID: 12867
	public static bool Sync;

	// Token: 0x04003244 RID: 12868
	public static string SyncId;

	// Token: 0x04003245 RID: 12869
	public static int SyncMode;

	// Token: 0x04003246 RID: 12870
	public static string ShareParams;

	// Token: 0x04003247 RID: 12871
	public static List<string> StringKeyPickupTypes;

	// Token: 0x04003248 RID: 12872
	public static bool ForceMaps;

	// Token: 0x0400324B RID: 12875
	public static bool Entrance;

	// Token: 0x0400324C RID: 12876
	public static Hashtable DoorTable;

	// Token: 0x0400324D RID: 12877
	public static bool QueueBash;

	// Token: 0x0400324E RID: 12878
	public static bool BashWasQueued;

	// Token: 0x0400324F RID: 12879
	public static bool BashTap;

	public static bool WorldTour;

	// Token: 0x04003251 RID: 12881
	public static bool fragsEnabled;

	public static int fragKeyFinish;

	// Token: 0x04003252 RID: 12882
	public static int maxFrags;

	// Token: 0x04003258 RID: 12888
	public static ArrayList GinsoData;

	// Token: 0x04003259 RID: 12889
	public static ArrayList ForlornData;

	// Token: 0x0400325A RID: 12890
	public static ArrayList HoruData;

	// Token: 0x0400325B RID: 12891
	public static bool OpenMode;

	// Token: 0x04003300 RID: 13056
	public static string HoruScene;

	// Token: 0x04003301 RID: 13057
	public static Hashtable HoruMap;

	// Token: 0x04003332 RID: 13106
	public static int TickCounter;

	// Token: 0x040032F7 RID: 13047
	public static ArrayList GladesData;

	// Token: 0x04003304 RID: 13060
	public static int LockedCount;

	public static int ResetTrackerCount;

	public static Vector3 WarpTarget;

	public static int Warping;

	public static bool HotCold;

	public static string[] HotColdTypes;

	public static Dictionary<int, RandomizerHotColdItem> HotColdItems;

	public static List<int> HotColdMaps;

	public static Dictionary<string, string> RelicZoneLookup;

	public static int RelicCount;

	// Token: 0x0400337A RID: 13178
	public static ArrayList ValleyStompDoorData;

	// Token: 0x0400337B RID: 13179
	public static ArrayList ValleyLeverDoorData;

	public static string GrenadeZone;
	// welcome to the...
	public static string StompZone;

	public static bool CreditsActive;

	public static float CachedVolume;

	public static int ResetVolume;

	public static bool LastMessageCredits;

	public static bool sacrificeStarted;

	public static bool AltRDisabled;

	public static HashSet<int> RepeatablePickups;

	public static int RepeatableCooldown;

	public static bool OpenWorld;

	public static bool StompTriggers;

	public static string SpawnWith;

	public static bool JustSpawned;

	public static bool DelayedWarp;

	public static bool SaveAfterWarp;

	public static bool IgnoreEnemyExp;
}
