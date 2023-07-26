using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.Aspects;
using Assets.Src.Lib.Cheats;
using ResourcesSystem.Loader;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.Repositories;
using JetBrains.Annotations;
using NLog;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Aspects.Science;
using SharedCode.Entities;
using SharedCode.Entities.Cloud;
using SharedCode.Entities.Engine;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.ChainCalls;
using SharedCode.EntitySystem.EntityPropertyResolvers;
using SharedCode.Refs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.CustomData;
using Assets.Src.Server.Impl;
using Uins;
using UnityEngine;
using ColonyShared.SharedCode.Utils;
using SharedCode.Wizardry;
using Assets.Src.Aspects.Impl.Factions.Template;
using Object = UnityEngine.Object;
using SharedVector3 = SharedCode.Utils.Vector3;
using Assets.Src.Lib.Unity;
using SharedCode.Utils.Threads;
using Assets.Src.Tools;
using Assets.Tools;
using ColonyShared.SharedCode.Wizardry;
using SharedCode.MapSystem;
using GeneratedCode.DeltaObjects;
using SharedCode.Utils;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using System.Threading;
using Assets.ColonyShared.SharedCode.Shared;
using SharedCode.EntitySystem.Delta;
using Assets.Src.ResourcesSystem.Base;
using ColonyHelpers;
using ResourceSystem.Aspects.Misc;
using SharedCode.MovementSync;
using Assets.Src.SpawnSystem;
using ColonyShared.GeneratedCode.Shared.Aspects;
using SharedCode.Utils.Extensions;
using SharedCode.AI;
using Src.Debugging;
using Src.Locomotion.Unity;
using ResourceSystem.Utils;
using UnityEngine.Scripting;
using SharedCode.Repositories;
using GeneratedCode.MapSystem;
using Core.Cheats;
using Core.Environment.Logging.Extension;
using Entities.GameMapData;
using ReactiveProps;
using ReactivePropsNs;
using ResourceSystem.Aspects.Item.Templates;
using SharedCode.Serializers;
using SharedCode.Serializers.Protobuf;

namespace Assets.Src.Cluster.Cheats
{
    public static partial class ClusterCheats
    {
        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetLogger("Cheats");

        [Cheat]
        public static async Task DamageItems(float percent)
        {
            await AsyncUtils.RunAsyncTask(() => DamageItemsImpl(percent));
        }

        private static async Task DamageItemsImpl(float percent)
        {
            var characterId = GameState.Instance.CharacterRuntimeData.CharacterId;
            var worldSceneRepoId = GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId;

            using (var cheatServiceWrapper = await GameState.Instance.ClientClusterNode.Get<ICheatServiceEntityServer>(worldSceneRepoId))
            {
                var cheatServiceEntity = cheatServiceWrapper.Get<ICheatServiceEntityServer>(worldSceneRepoId);
                await cheatServiceEntity.DamageAllItems(characterId, percent);
            }
        }


        [Cheat]
        public static void CountGameObjects()
        {
            var sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCount;
            List<GameObject> roots = new List<GameObject>();
            for (int i = 0; i < sceneCount; i++)
            {
                var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                roots.AddRange(scene.GetRootGameObjects());
            }
            var allGos = roots.SelectMany(x => x.GetComponentsInChildren<UnityEngine.Transform>().Select(y => y.gameObject));
            var groups = allGos.GroupBy(x => x.name.Length > 10 ? x.name.Substring(0, 10) : x.name).ToDictionary(x => x.Key, x => x.ToList());
            StringBuilder builder = new StringBuilder();
            builder.Append("ObjectsCount: ").Append(groups.Sum(x => x.Value.Count)).AppendLine();
            foreach (var group in groups)
            {
                builder.Append(group.Key).Append(" ").Append(group.Value).AppendLine();
            }
            Logger.IfError()?.Message(builder.ToString()).Write();
        }

        [Cheat]
        public static void MoveTo(string teleportScene)
        {
            TeleportTo(GameState.Instance.CharacterRuntimeData.CharacterId, teleportScene);
        }
        
        public static void TeleportTo(Guid characterId, string teleportScene)
        {
            throw new NotImplementedException("It did not work anyway");
            /*if (characterId != GameState.Instance.CharacterRuntimeData.CharacterId)
                return;

            //string teleportTo = "Test_Obj".ToLower(); 
            string teleportTo = teleportScene.ToLower();
            var mapsRoot = GameResourcesHolder.Instance.LoadResource<MapRootDef>("/Scenes/MapRoot");
            var map = mapsRoot.Maps.Where(v => v.DebugName.ToLower() == teleportTo).FirstOrDefault();
            if (map.Map != null)
            {
                var CharacterId = characterId.ToString();
                var UserId = GameState.Instance.CharacterRuntimeData.UserId.ToString();

                var worldNodeServiceId = GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId;
                using (var worldNodeServiceWrapper = await NodeAccessor.Repository.Get<IWorldNodeServiceEntityServer>(worldNodeServiceId))
                {
                    var worldNodeInfo = await worldNodeServiceWrapper.Get<IWorldNodeServiceEntityServer>(worldNodeServiceId)
                        .GetWorldNodeIdForPosition(((IResource) map.Map.Target).Address.ToString(),
                            UnityWorldSpace.ToVector3(Vector3.zero));
                    if (worldNodeInfo.Id == Guid.Empty)
                    {
                         Logger.IfError()?.Message("Requested {1} scene that is absent in this cluster. Declining login",  teleportTo).Write();
                        return;
                    }

                     Logger.IfInfo()?.Message("Redirecting to unity dedicated {0}:{1}",  worldNodeInfo.Host, worldNodeInfo.UnityPort).Write();

                    await GameState.Instance.Disconnect(false);

                    await Awaiters.NextFrame;

                    var host = worldNodeInfo.Host;
                    var port = worldNodeInfo.Port;
                    var worldNodeId = worldNodeInfo.Id;
                    var unityPort = worldNodeInfo.UnityPort;

                     Logger.IfInfo()?.Message("Connect to world node host {0} port {1}",  host, port).Write();
                    await ((EntitiesRepositoryBase) GameState.Instance.ClientClusterNode).ConnectExternal(host, port);
                    await Task.Delay(1000);

                    while (!NodeAccessor.Repository.StopToken.IsCancellationRequested)
                    {
                        using (var wrapper = await NodeAccessor.Repository.Get<IWorldNodeServiceEntityServer>(worldNodeId))
                        {
                            if (wrapper != null)
                            {
                                 Logger.IfInfo()?.Message("World node host {0} port {1} connected",  host, port).Write();
                                while (!NodeAccessor.Repository.StopToken.IsCancellationRequested)
                                {
                                    var result = await wrapper.Get<IWorldNodeServiceEntityServer>(worldNodeId).Teleport(worldNodeServiceId);
                                    if (result)
                                        break;

                                    GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId = worldNodeId;
                                     Logger.IfError()?.Message("World node teleport error. Try again").Write();;
                                    await Task.Delay(1000, NodeAccessor.Repository.StopToken);
                                }

                                break;
                            }
                        }

                        await Task.Delay(10);
                    }

                    await Awaiters.NextFrame;
                    UnityConnectionParams newParams = new UnityConnectionParams(host, unityPort, map.Map);
                    await GameState.Instance.OnLobbyToClientRequest(newParams, false);
                    await Awaiters.NextFrame;
                    while (GameState.Instance.User == null && !NodeAccessor.Repository.StopToken.IsCancellationRequested)
                    {
                        await Task.Delay(100, NodeAccessor.Repository.StopToken);
                    }

                    await Awaiters.NextFrame;
                    GameState.Instance.User.CmdSendUserId(CharacterId, UserId, false);
                    GameState.Instance.User.CmdSpawnPawn(CharacterId);
                    //onSuccess();
                }
            }*/
        }

        [Cheat]
        public static void GCGeneration()
        {
            var maxGen = GC.MaxGeneration;

            var myStringBuilder = new StringBuilder();
            for (int i = 0; i < maxGen; i++)
            {
                var g = GC.CollectionCount(i);
                myStringBuilder.AppendFormat("Generation {0}: {1} times keeped alive\n", i, g);
            }
            Logger.IfInfo()?.Message(myStringBuilder.ToString()).Write();
        }

        [Cheat]
        public static void Hotload(string folder)
        {
            GameResourcesHolder.Instance.Reload(folder);
        }

        [Cheat]
        public static void GCCollect(int generation = -1)
        {
            if (generation < 0)
            {
                GC.Collect();
                 Logger.IfInfo()?.Message("GC.Collect()").Write();;
            }
            else
            {
                GC.Collect(generation);
                Logger.IfInfo()?.Message($"GC.Collect({generation})").Write();
            }
        }

        [Cheat]
        public static void UnloadUnusedAssets()
        {
             Logger.IfInfo()?.Message("Resources.UnloadUnusedAssets()").Write();;

            Logger.IfInfo()?.Message("ScriptableObject " + Resources.FindObjectsOfTypeAll(typeof(UnityEngine.ScriptableObject)).Length).Write();
            Logger.IfInfo()?.Message("UnityEngine.Object " + Resources.FindObjectsOfTypeAll(typeof(UnityEngine.Object)).Length).Write();
            Logger.IfInfo()?.Message("Textures " + Resources.FindObjectsOfTypeAll(typeof(Texture)).Length).Write();
            Logger.IfInfo()?.Message("AudioClips " + Resources.FindObjectsOfTypeAll(typeof(AudioClip)).Length).Write();
            Logger.IfInfo()?.Message("Meshes " + Resources.FindObjectsOfTypeAll(typeof(Mesh)).Length).Write();
            Logger.IfInfo()?.Message("Materials " + Resources.FindObjectsOfTypeAll(typeof(Material)).Length).Write();
            Logger.IfInfo()?.Message("GameObjects " + Resources.FindObjectsOfTypeAll(typeof(GameObject)).Length).Write();
            Logger.IfInfo()?.Message("Components " + Resources.FindObjectsOfTypeAll(typeof(Component)).Length).Write();
            
            Resources.UnloadUnusedAssets();
            
            Logger.IfInfo()?.Message("ScriptableObject " + Resources.FindObjectsOfTypeAll(typeof(UnityEngine.ScriptableObject)).Length).Write();
            Logger.IfInfo()?.Message("UnityEngine.Object " + Resources.FindObjectsOfTypeAll(typeof(UnityEngine.Object)).Length).Write();
            Logger.IfInfo()?.Message("Textures " + Resources.FindObjectsOfTypeAll(typeof(Texture)).Length).Write();
            Logger.IfInfo()?.Message("AudioClips " + Resources.FindObjectsOfTypeAll(typeof(AudioClip)).Length).Write();
            Logger.IfInfo()?.Message("Meshes " + Resources.FindObjectsOfTypeAll(typeof(Mesh)).Length).Write();
            Logger.IfInfo()?.Message("Materials " + Resources.FindObjectsOfTypeAll(typeof(Material)).Length).Write();
            Logger.IfInfo()?.Message("GameObjects " + Resources.FindObjectsOfTypeAll(typeof(GameObject)).Length).Write();
            Logger.IfInfo()?.Message("Components " + Resources.FindObjectsOfTypeAll(typeof(Component)).Length).Write();
        }

        [Cheat]
        public static void GCGetTotalMemory()
        {
            var memory = GC.GetTotalMemory(true);
            var kb = memory / 1024;
            var mb = kb / 1024;
            kb -= mb * 1024;

            Logger.IfInfo()?.Message($"GC.GetTotalMemory(true) = {mb} MB, {kb} KB").Write();
        }

        [Cheat]
        public static void GameObjectInstatiate()
        {
            var stats = UnityHelpers.GameObjectInstantiateStatistics;

            var myStringBuilder = new StringBuilder();
            myStringBuilder.AppendFormat("Total Instatiated = {0}\n", stats.Sum(v => v.Value));
            foreach (var pair in stats)
            {
                myStringBuilder.AppendFormat("{0} = {1}\n", pair.Key, pair.Value);
            }

            stats.Clear();
            Logger.IfInfo()?.Message(myStringBuilder.ToString()).Write();
        }

        // Читы на добавление айтемов

        [Cheat]
        public static async Task AddItems()
        {
            await AsyncUtils.RunAsyncTask(() => InventorySetSize(20, true, InventoryAddressResolver));
            await Task.Delay(1000);
            string[] lines =
            {
                "/Inventory/Items/Backpacks/BackpackT2Vegan",
                "/Inventory/Items/Backpacks/BeltHardened",
                "/Inventory/Items/Tools/Knife_1_1",
                "/Inventory/Items/Tools/Pick_3",
                "/Inventory/Items/Weapons/New/Spear_1_2",
                "/Inventory/Items/Weapons/New/Sword_2",
                "/Inventory/Items/Consumables/StandardMedicalKit 20",
                "/Inventory/Items/Consumables/StandardRation 10",
                "/Inventory/Items/Mounting/SimpleBaken 2",
                "/Inventory/Items/Consumables/Flask",
                "/Inventory/Items/Consumables/MutagenZ"
            };
            await AddItemsList(lines);
        }

        [Cheat]
        public static async Task UniversalCheat()
        {
            await AddAllKnowledges();
            await AddR1(1000);
            await AddR2(50);
            await AddItems();
            await SergeyCheats.AddClothes(3);
            await SergeyCheats.AddArmor(2);
        }
        
        [Cheat]
        public static async Task AddTestItems()
        {
            await AsyncUtils.RunAsyncTask(() => InventorySetSize(12, true, InventoryAddressResolver));
            await Task.Delay(1000);
            string[] lines =
            {
                "/Inventory/Items/Weapons/Test/Thors_Hammer",
                "/Inventory/Items/Weapons/Test/Dwarfs_Pick",
                "/Inventory/Items/Weapons/Test/StoneKnifeTest",
                "/Inventory/Items/Clothes/Test/HunterBootsTest",
                "/Inventory/Items/Clothes/Test/HunterGlovesTest",
                "/Inventory/Items/Clothes/Test/HunterHatTest",
                "/Inventory/Items/Clothes/Test/HunterJacketTest",
                "/Inventory/Items/Clothes/Test/HunterPantsTest",
                "/Inventory/Items/Armor/Test/ScaleBracersTest",
                "/Inventory/Items/Armor/Test/ScaleCuirassTest",
                "/Inventory/Items/Armor/Test/ScaleGreavesTest",
                "/Inventory/Items/Armor/Test/ScaleHelmetTest"
            };
            await AddItemsList(lines);
        }

        [Cheat]
        public static async Task AddHealthFood()
        {
            string[] lines =
            {
                "/Inventory/Items/Consumables/StandardMedicalKit 20",
                "/Inventory/Items/Consumables/StandardRation 10"
            };
            await AddItemsList(lines);
        }

        [Cheat]
        public static async Task AddFoodHealth()
        {
            string[] lines =
            {
                "/Inventory/Items/Consumables/StandardRation 10",
                "/Inventory/Items/Consumables/StandardMedicalKit 20"    
            };
            await AddItemsList(lines);
        }

        [Cheat]
        public static async Task AddCraft()
        {
            await AsyncUtils.RunAsyncTask(() => InventorySetSize(70, true, InventoryAddressResolver));
            await Task.Delay(1000);
            string[] lines =
            {
                "/Inventory/Items/Weapons/Rootstick 100",
                "/Inventory/Items/Consumables/BeetRoot 100",
                "/Inventory/Items/Consumables/CucumberRock 100",
                "/Inventory/Items/Consumables/GerophitFruit 100",
                "/Inventory/Items/Consumables/Horseshoe 100",
                "/Inventory/Items/Consumables/MossGreen 100",
                "/Inventory/Items/Consumables/RawFineMeat 100",
                "/Inventory/Items/Consumables/RawJuicyMeat 100",
                "/Inventory/Items/Consumables/RawToughMeat 100",
                "/Inventory/Items/Consumables/RoastedJuicyMeat 100",
                "/Inventory/Items/Res/AcidRoot 100",
                "/Inventory/Items/Res/AlphaKvarPheromones 100",
                "/Inventory/Items/Res/Antifreeze 100",
                "/Inventory/Items/Res/BlueEssence 100",
                "/Inventory/Items/Res/Bone 100",
                "/Inventory/Items/Res/Coal 100",
                "/Inventory/Items/Res/ConcentratedLifeEssence 100",
                "/Inventory/Items/Res/FlimsyRope 100",
                "/Inventory/Items/Res/FungalShard 100",
                "/Inventory/Items/Res/GerophitShell 100",
                "/Inventory/Items/Res/HardenedFungus 100",
                "/Inventory/Items/Res/HullFragment 100",
                "/Inventory/Items/Res/ItemBractusResin 100",
                "/Inventory/Items/Res/ItemBractusResinDesert 100",
                "/Inventory/Items/Res/ItemBractusResinJungle 100",
                "/Inventory/Items/Res/JewelBerries 100",
                "/Inventory/Items/Res/KsoGland 100",
                "/Inventory/Items/Res/Limestone 100",
                "/Inventory/Items/Res/MeridCarapace 100",
                "/Inventory/Items/Res/Peat 100",
                "/Inventory/Items/Res/PoisonLiane 100",
                "/Inventory/Items/Res/PressedLeather_3 100",
                "/Inventory/Items/Res/PressedThickLeather 100",
                "/Inventory/Items/Res/PressedThinLeather 100",
                "/Inventory/Items/Res/Pressed_leather_4 100",
                "/Inventory/Items/Res/RawFineLeather 100",
                "/Inventory/Items/Res/RawThickLeather 100",
                "/Inventory/Items/Res/RawThickLeatherJungle 100",
                "/Inventory/Items/Res/RawThinLeather 100",
                "/Inventory/Items/Res/RawThinLeatherJungle 100",
                "/Inventory/Items/Res/ReinforcedHullFragment 100",
                "/Inventory/Items/Res/ReinforcedMeridCarapace 100",
                "/Inventory/Items/Res/Salt 100",
                "/Inventory/Items/Res/SangviraLeaf 100",
                "/Inventory/Items/Res/SangviraStem 100",
                "/Inventory/Items/Res/ScifopodLeg 100",
                "/Inventory/Items/Res/ScifopodLegHardened 100",
                "/Inventory/Items/Res/Scraps 100",
                "/Inventory/Items/Res/ScrapsDesert 100",
                "/Inventory/Items/Res/ScrapsJungle 100",
                "/Inventory/Items/Res/ScyforiaSlime 100",
                "/Inventory/Items/Res/ScyforiaSlimeJungle 100",
                "/Inventory/Items/Res/SnowMeridLiver 100",
                "/Inventory/Items/Res/SpiralStem 100",
                "/Inventory/Items/Res/Stone 100",
                "/Inventory/Items/Res/StoutFoam 100",
                "/Inventory/Items/Res/SwampDecoct 100",
                "/Inventory/Items/Res/TannedThickLeather 100",
                "/Inventory/Items/Res/TemperedSpiralStem 100",
                "/Inventory/Items/Res/WebMelbr 100"
            };
            await AddItemsList(lines);
            await AddWeight();
        }

        [Cheat]
        public static async Task AddGrind()
        {
            await AsyncUtils.RunAsyncTask(() => InventorySetSize(6, true, InventoryAddressResolver));
            await Task.Delay(1000);
            string[] lines =
            {
                "/Inventory/Items/Res/JewelBerries 12",
                "/Inventory/Items/Consumables/Food/RoastedGerophit 11",
                "/Inventory/Items/Consumables/HealingBandage 6",
                "/Inventory/Items/Res/ItemBractusResin 20",
                "/Inventory/Items/Res/MeridCarapace 9",
                "/Inventory/Items/Res/TemperedSpiralStem 15"
            };
            await AddItemsList(lines);
        }

        [Cheat]
        public static async Task AddKey()
        {
            await AsyncUtils.RunAsyncTask(() => InventorySetSize(10, true, InventoryAddressResolver));
            await Task.Delay(1000);
            string[] lines =
            {
                "/Inventory/Items/Quest/Key01Quest/VoidDNA_1 10",
                "/Inventory/Items/Quest/Key02Quest/Explosive 200",
                "/Inventory/Items/Consumables/Food/RoastedJuicyMeat 5",
                "/Inventory/Items/Consumables/Food/RoastedToughMeat 15",
                "/Inventory/Items/Quest/Key04Quest/Cryogel 30",
                "/Inventory/Items/Mounting/GasStation",
                "/Inventory/Items/Quest/Key05Quest/ToxicGas",
                "/Inventory/Items/Consumables/RawFineMeat 300",
                "/Inventory/Items/Res/LifeEssence 80",
            };
            await AddItemsList(lines);
            await AddWeight();
        }

        [Cheat]
        public static async Task AddLock()
        {
            await AsyncUtils.RunAsyncTask(() => InventorySetSize(15, true, InventoryAddressResolver));
            await Task.Delay(1000);
            string[] lines =
            {
                "/Inventory/Items/Weapons/Rootstick 5",
                "/Inventory/Items/Res/Stone 15",
                "/Inventory/Items/Res/SangviraStem 10",
                "/Inventory/Items/Res/HullFragment 5",
                "/Inventory/Items/Res/Bone 10",
                "/Inventory/Items/Res/Coal 15",
                "/Inventory/Items/Res/StoutFoam 5",
                "/Inventory/Items/Res/Antifreeze 5",
                "/Inventory/Items/Res/FungalShard 40",
                "/Inventory/Items/Res/SpiralStem 20",
                "/Inventory/Items/Res/Limestone 20",
                "/Inventory/Items/Res/Peat 20",
            };
            await AddItemsList(lines);
        }

        [Cheat]
        public static async Task AddMountings()
        {
            await AsyncUtils.RunAsyncTask(() => InventorySetSize(6, true, InventoryAddressResolver));
            await Task.Delay(1000);
            string[] lines =
            {
                "/Inventory/Items/Mounting/Fireplace",
                "/Inventory/Items/Mounting/HiddenStash",
                "/Inventory/Items/Mounting/SimpleStash",
                "/Inventory/Items/Mounting/ReinforcedStash",
                "/Inventory/Items/Mounting/MetalStash",
                "/Inventory/Items/Mounting/SimpleBaken 3"
            };
            await AddItemsList(lines);
        }

        

        [Cheat]
        public static async Task AddHealth()
        {
            await AsyncUtils.RunAsyncTask(() => InventorySetSize(3, true, InventoryAddressResolver));
            await Task.Delay(1000);
            string[] lines =
            {
                "/Inventory/Items/Consumables/HealingBandage 5",
                "/Inventory/Items/Consumables/RegenerativeCompress 5",
                "/Inventory/Items/Consumables/StandardMedicalKit 20"
            };
            await AddItemsList(lines);
        }

        [Cheat]
        public static async Task AddFood()
        {
            await AsyncUtils.RunAsyncTask(() => InventorySetSize(13, true, InventoryAddressResolver));
            await Task.Delay(1000);
            string[] lines =
            {
                "/Inventory/Items/Consumables/Food/ColdProtectionSoup 5",
                "/Inventory/Items/Consumables/Food/DryedJuicyMeat 5",
                "/Inventory/Items/Consumables/Food/DryedToughMeat 5",
                "/Inventory/Items/Consumables/Food/MeatCucumberSalat 5",
                "/Inventory/Items/Consumables/Food/MeatStew 5",
                "/Inventory/Items/Consumables/Food/PickledMerid 5",
                "/Inventory/Items/Consumables/Food/RoastedGerophit 5",
                "/Inventory/Items/Consumables/Food/RoastedJuicyMeat 5",
                "/Inventory/Items/Consumables/Food/RoastedToughMeat 5",
                "/Inventory/Items/Consumables/Food/SnowKvarSteak 5",
                "/Inventory/Items/Consumables/Food/SourSpicyStew 5",
                "/Inventory/Items/Consumables/Food/Toxic_protection_3 5",
                "/Inventory/Items/Consumables/StandardRation 10"
            };
            await AddItemsList(lines);
        }

        [Cheat]
        public static async Task AddFP()
        {
            await AsyncUtils.RunAsyncTask(() => InventorySetSize(18, true, InventoryAddressResolver));
            await Task.Delay(1000);
            string[] lines =
            {
                "/Inventory/Items/Exclusive/Armor_FP1_chest",
                "/Inventory/Items/Exclusive/Armor_FP1_head",
                "/Inventory/Items/Exclusive/Armor_FP2_head",
                "/Inventory/Items/Exclusive/Armor_FP3_head",
                "/Inventory/Items/Exclusive/Spear_FP2",
                "/Inventory/Items/Exclusive/Spear_FP1",
                "/Inventory/Items/Exclusive/Belt_FP3",
                "/Inventory/Items/Exclusive/ConsumPack_FP1",
                "/Inventory/Items/Exclusive/ConsumPack_FP2",
                "/Inventory/Items/Exclusive/ConsumPack_FP3",
                "/Inventory/Items/Exclusive/ConsumPack_FP4",
                "/Inventory/Items/Exclusive/ResPack_FP1",
                "/Inventory/Items/Exclusive/ResPack_FP2",
                "/Inventory/Items/Exclusive/ResPack_FP3",
                "/Inventory/Items/Exclusive/ResPack_FP4",
                "/Inventory/Items/Exclusive/Standart_FP1",
                "/Inventory/Items/Exclusive/Standart_FP2",
                "/Inventory/Items/Exclusive/Standart_FP3",
            };
            await AddItemsList(lines);
        }

        [Cheat]
        public static async Task AddBuildRes()
        {
            await AsyncUtils.RunAsyncTask(() => InventorySetSize(30, true, InventoryAddressResolver));
            await Task.Delay(1000);
            string[] lines =
            {
                "/Inventory/Items/Res/Bone 300",
                "/Inventory/Items/Res/Stone 300",
                "/Inventory/Items/Weapons/Rootstick 300",
                "/Inventory/Items/Res/FungalShard 300",
                "/Inventory/Items/Res/Pressed_leather_4 300",
                "/Inventory/Items/Res/FlimsyRope 300",
            };
            await AddItemsList(lines);
            await AddWeight();
        }

        [Cheat]
        public static async Task AddBoards()
        {
            await AsyncUtils.RunAsyncTask(() => InventorySetSize(5, true, InventoryAddressResolver));
            await Task.Delay(1000);
            string[] lines =
            {
                "/Inventory/Items/Weapons/Test/Boards/Board1",
                "/Inventory/Items/Weapons/Test/Boards/Board2",
                "/Inventory/Items/Weapons/Test/Boards/Board3",
                "/Inventory/Items/Weapons/Test/Boards/Board4",
                "/Inventory/Items/Weapons/Test/Boards/Board5",
            };
            await AddItemsList(lines);
        }

        [Cheat]
        public static async Task AddFlasks()
        {
            await AsyncUtils.RunAsyncTask(() => InventorySetSize(10, true, InventoryAddressResolver));
            await Task.Delay(1000);
            string[] lines =
            {
                "/Inventory/Items/Consumables/Flask",
                "/Inventory/Items/Consumables/FlaskBig",
                "/Inventory/Items/Consumables/BattlePotion_2 3",
                "/Inventory/Items/Consumables/BattlePotion_3 3",
                "/Inventory/Items/Consumables/BattlePotion_4 3",
                "/Inventory/Items/Consumables/HeatProtectionSuit",
                "/Inventory/Items/Consumables/ChemicalProtectionSuit",
                "/Inventory/Items/Consumables/EmpowerSuit",
                "/Inventory/Items/Consumables/AdaptantSuit"
            };
            await AddItemsList(lines);
        }
        
        
        // Андрей, не нужно, пожалуйста, делать правки в моих читах без моего согласования. Твоё решение может и правильное, но мне абсолютно неудобное. Буков С.
        // Сергей, эти читы такие же "твои" как и мои. Как и все остальные читы здесь. "Твои" читы перенёс в Assets/Src/Cluster/Cheats/SergeyCheats.cs 
        private static readonly ResourceRef<ItemsCollectionDef> _Clothes = new ResourceRef<ItemsCollectionDef>("/Inventory/Items/Clothes/_AllClothes");

        [Cheat]
        public static Task Clothes(string name) => AddItemsFromCollection(_Clothes, name);

        [Cheat]
        public static Task AllClothes() => AddAllItemsFromCollection(_Clothes);
        
        private static readonly ResourceRef<ItemsCollectionDef> _Armors = new ResourceRef<ItemsCollectionDef>("/Inventory/Items/Armor/_AllArmors");

        [Cheat]
        public static Task Armor(string name) => AddItemsFromCollection(_Armors, name);

        [Cheat]
        public static Task AllArmor() => AddAllItemsFromCollection(_Armors);

        private static async Task AddItemsFromCollection(ItemsCollectionDef collection, string name)
        {
            if (int.TryParse(name, out var setIndex))
                await AddItems(collection.Items.Values.Skip(setIndex-1).First());
            else
                await AddItems(collection.Items[name]);
        }

        private static async Task AddAllItemsFromCollection(ItemsCollectionDef collection)
        {
            await AsyncUtils.RunAsyncTask(() => InventorySetSize(30, true, InventoryAddressResolver));
            await Task.Delay(1000);
            await AddItems(collection.Items.Values.SelectMany(x => x));
        }
        
        [Cheat]
        public static async Task AddWeapon(int setIndex)
        {
            string[] lines = null;
            switch (setIndex)
            {
                case 1:
                    lines = new string[]
                    {
                        "/Inventory/Items/Weapons/Rootstick",
                        "/Inventory/Items/Weapons/New/TorchFoam",
                        "/Inventory/Items/Tools/Knife_1_1",
                        "/Inventory/Items/Tools/Pick_1_1",
                        "/Inventory/Items/Weapons/New/Spear_1_1",
                        "/Inventory/Items/Weapons/New/WarHammer_1_1"
                    };
                    break;

                case 2:
                    lines = new string[]
                    {   
                        "/Inventory/Items/Tools/Knife_1_2",
                        "/Inventory/Items/Tools/Pick_1_2",
                        "/Inventory/Items/Weapons/New/Spear_1_2",
                        "/Inventory/Items/Weapons/New/WarHammer_1_2",
                        "/Inventory/Items/Weapons/New/Sword_1",
                        "/Inventory/Items/Weapons/New/WarAxe_1"
                    };
                    break;

                case 3:
                    lines = new string[]
                    {
                        "/Inventory/Items/Tools/Knife_2",
                        "/Inventory/Items/Tools/Pick_2",
                        "/Inventory/Items/Weapons/New/Spear_2",
                        "/Inventory/Items/Weapons/New/WarHammer_2",
                        "/Inventory/Items/Weapons/New/Sword_2",
                        "/Inventory/Items/Weapons/New/WarAxe_2"
                    };
                    break;

                case 4:
                    lines = new string[]
                    {
                        "/Inventory/Items/Tools/Knife_3",
                        "/Inventory/Items/Tools/Pick_3",
                        "/Inventory/Items/Weapons/New/Spear_3",
                        "/Inventory/Items/Weapons/New/WarHammer_3",
                        "/Inventory/Items/Weapons/New/Sword_3",
                        "/Inventory/Items/Weapons/New/WarAxe_3"
                    };
                    break;

                case 5:
                    lines = new string[]
                    {
                        "/Inventory/Items/Tools/Knife_4",
                        "/Inventory/Items/Tools/Pick_4",
                        "/Inventory/Items/Weapons/New/Spear_4",
                        "/Inventory/Items/Weapons/New/WarHammer_4",
                        "/Inventory/Items/Weapons/New/Sword_4",
                        "/Inventory/Items/Weapons/New/WarAxe_4"
                    };
                    break;
            }
            await AddItemsList(lines);
        }

        [Cheat]
        public static async Task AddAllWeapons()
        {
            await AsyncUtils.RunAsyncTask(() => InventorySetSize(30, true, InventoryAddressResolver));
            await Task.Delay(1000);
            string[] lines =
            {
                "/Inventory/Items/Weapons/Rootstick",
                "/Inventory/Items/Weapons/New/TorchFoam",
                "/Inventory/Items/Weapons/New/Spear_1_1",
                "/Inventory/Items/Weapons/New/Spear_1_2",
                "/Inventory/Items/Weapons/New/Spear_2",
                "/Inventory/Items/Weapons/New/Spear_3",
                "/Inventory/Items/Weapons/New/Spear_4",
                "/Inventory/Items/Weapons/New/Sword_1",
                "/Inventory/Items/Weapons/New/Sword_2",
                "/Inventory/Items/Weapons/New/Sword_3",
                "/Inventory/Items/Weapons/New/Sword_4",
                "/Inventory/Items/Weapons/New/WarAxe_1",
                "/Inventory/Items/Weapons/New/WarAxe_2",
                "/Inventory/Items/Weapons/New/WarAxe_3",
                "/Inventory/Items/Weapons/New/WarAxe_4",
                "/Inventory/Items/Weapons/New/WarHammer_1_1",
                "/Inventory/Items/Weapons/New/WarHammer_1_2",
                "/Inventory/Items/Weapons/New/WarHammer_2",
                "/Inventory/Items/Weapons/New/WarHammer_3",
                "/Inventory/Items/Weapons/New/WarHammer_4",
                "/Inventory/Items/Tools/Knife_1_1",
                "/Inventory/Items/Tools/Knife_1_2",
                "/Inventory/Items/Tools/Knife_2",
                "/Inventory/Items/Tools/Knife_3",
                "/Inventory/Items/Tools/Knife_4",
                "/Inventory/Items/Tools/Pick_1_1",
                "/Inventory/Items/Tools/Pick_1_2",
                "/Inventory/Items/Tools/Pick_2",
                "/Inventory/Items/Tools/Pick_3",
                "/Inventory/Items/Tools/Pick_4",
            };
            await AddItemsList(lines);
        }

        [Cheat]
        public static async Task AddWeapons()
        {
            await AsyncUtils.RunAsyncTask(() => InventorySetSize(20, true, InventoryAddressResolver));
            await Task.Delay(1000);
            string[] lines =
            {
                "/Inventory/Items/Weapons/Rootstick",
                "/Inventory/Items/Weapons/New/TorchFoam",
                "/Inventory/Items/Weapons/New/Spear_1_1",
                "/Inventory/Items/Weapons/New/Spear_1_2",
                "/Inventory/Items/Weapons/New/Spear_2",
                "/Inventory/Items/Weapons/New/Spear_3",
                "/Inventory/Items/Weapons/New/Spear_4",
                "/Inventory/Items/Weapons/New/Sword_1",
                "/Inventory/Items/Weapons/New/Sword_2",
                "/Inventory/Items/Weapons/New/Sword_3",
                "/Inventory/Items/Weapons/New/Sword_4",                
                "/Inventory/Items/Weapons/New/WarAxe_1",
                "/Inventory/Items/Weapons/New/WarAxe_2",
                "/Inventory/Items/Weapons/New/WarAxe_3",
                "/Inventory/Items/Weapons/New/WarAxe_4",
                "/Inventory/Items/Weapons/New/WarHammer_1_1",
                "/Inventory/Items/Weapons/New/WarHammer_1_2",
                "/Inventory/Items/Weapons/New/WarHammer_2",
                "/Inventory/Items/Weapons/New/WarHammer_3",
                "/Inventory/Items/Weapons/New/WarHammer_4"
            };
            await AddItemsList(lines);
        }

        [Cheat]
        public static async Task AddTools()
        {
            await AsyncUtils.RunAsyncTask(() => InventorySetSize(10, true, InventoryAddressResolver));
            await Task.Delay(1000);
            string[] lines =
            {
                "/Inventory/Items/Tools/Knife_1_1",
                "/Inventory/Items/Tools/Knife_1_2",
                "/Inventory/Items/Tools/Knife_2",
                "/Inventory/Items/Tools/Knife_3",
                "/Inventory/Items/Tools/Knife_4",
                "/Inventory/Items/Tools/Pick_1_1",
                "/Inventory/Items/Tools/Pick_1_2",
                "/Inventory/Items/Tools/Pick_2",
                "/Inventory/Items/Tools/Pick_3",
                "/Inventory/Items/Tools/Pick_4"
            };
            await AddItemsList(lines);
        }

        [Cheat]
        public static async Task AddBackpacks()
        {
            await AsyncUtils.RunAsyncTask(() => InventorySetSize(5, true, InventoryAddressResolver));
            await Task.Delay(1000);
            string[] lines =
            {
                "/Inventory/Items/Backpacks/Backpack",
                "/Inventory/Items/Backpacks/BackpackT1Vegan",
                "/Inventory/Items/Backpacks/BackpackT1Yutt",
                "/Inventory/Items/Backpacks/BackpackT2",
                "/Inventory/Items/Backpacks/BackpackT2Vegan"
            };
            await AddItemsList(lines);
        }

        [Cheat]
        public static async Task AddBelts()
        {
            await AsyncUtils.RunAsyncTask(() => InventorySetSize(2, true, InventoryAddressResolver));
            await Task.Delay(1000);
            string[] lines =
            {
                "/Inventory/Items/Backpacks/BeltBasic",
                "/Inventory/Items/Backpacks/BeltHardened"
            };
            await AddItemsList(lines);
        }

        [Cheat]
        public static async Task AddRepairKits()
        {
            await AsyncUtils.RunAsyncTask(() => InventorySetSize(2, true, InventoryAddressResolver));
            await Task.Delay(1000);
            string[] lines =
            {
                "/Inventory/Items/Res/RepairKitTier1 5",
                "/Inventory/Items/Res/RepairKitTier2 5"
            };
            await AddItemsList(lines);
        }

        [Cheat]
        public static async Task AddAntidote()
        {
            await AsyncUtils.RunAsyncTask(() => InventorySetSize(4, true, InventoryAddressResolver));
            await Task.Delay(1000);
            string[] lines =
            {
                "/Inventory/Items/Res/JewelBerries 5",
                "/Inventory/Items/Consumables/WeakAntidote 3",
                "/Inventory/Items/Consumables/MediumAntidote 3",
                "/Inventory/Items/Clothes/New/Toxic_protection_2"
            };
            await AddItemsList(lines);
        }

        [Cheat]
        public static async Task MashaCheat()
        {
            await AsyncUtils.RunAsyncTask(() => InventorySetSize(17, true, InventoryAddressResolver));
            await Task.Delay(1000);
            string[] lines =
            {
                "/Inventory/Items/Backpacks/BackpackT2",
                "/Inventory/Items/Weapons/Test/StoneKnifeTest",
                "/Inventory/Items/Mounting/SimpleBaken 2",
                "/Inventory/Items/Consumables/StandardMedicalKit 10",
                "/Inventory/Items/Consumables/StandardRation 20",
                "/Inventory/Items/Mounting/Fireplace",
                "/Inventory/Items/Consumables/RawToughMeat 10",
                "/Inventory/Items/Weapons/Rootstick 20",
                "/Inventory/Items/Clothes/New/Fire_protection_chest_1",
                "/Inventory/Items/Clothes/New/Fire_protection_feet_1",
                "/Inventory/Items/Clothes/New/Fire_protection_hands_1",
                "/Inventory/Items/Clothes/New/Fire_protection_head_1",
                "/Inventory/Items/Clothes/New/Fire_protection_legs_1",
                "/Inventory/Items/Armor/New/Armor_2_arms",
                "/Inventory/Items/Armor/New/Armor_2_chest",
                "/Inventory/Items/Armor/New/Armor_2_head",
                "/Inventory/Items/Armor/New/Armor_2_legs"
            };
            await AddItemsList(lines);
        }

        [Cheat]
        public static async Task SmokeCheat()
        {
            await AddAllKnowledges();
            await AddSomePerks();
            await AddR1(1000);
            await AddR2(50);
            await MashaCheat();
        }

        // Читы на добавление перков

        [Cheat]
        public static async Task AddAllPerks1()
        {
            string[] lines =
            {
                "AddPerkToAllCollections /Inventory/Perks/Level1/CalorieCons_Level1",
                "AddPerkToAllCollections /Inventory/Perks/Level1/SatMax_Level1",
                "AddPerkToAllCollections /Inventory/Perks/Level1/WaterCons_Level1",
                "AddPerkToAllCollections /Inventory/Perks/Level1/WaterMax_Level1",
                "AddPerkToAllCollections /Inventory/Perks/Level1/Detox_Level1",
                "AddPerkToAllCollections /Inventory/Perks/Level1/Mining_Level1",
                "AddPerkToAllCollections /Inventory/Perks/Level1/InvMaxWgt_Level1",
                "AddPerkToAllCollections /Inventory/Perks/Level1/FallDmg_Level1",
                "AddPerkToAllCollections /Inventory/Perks/Level1/HealthReg_Level1",
                "AddPerkToAllCollections /Inventory/Perks/Level1/HealthMax_Level1",
                "AddPerkToAllCollections /Inventory/Perks/Level1/StamMax_Level1",
                "AddPerkToAllCollections /Inventory/Perks/Level1/StamReg_Level1",
                "AddPerkToAllCollections /Inventory/Perks/Level1/Dmg_Level1",
            };
            await Batch(lines);
        }

        [Cheat]
        public static async Task AddAllPerks2()
        {
            string[] lines =
            {
                "AddPerkToAllCollections /Inventory/Perks/Level2/CalorieCons_Level2",
                "AddPerkToAllCollections /Inventory/Perks/Level2/SatMax_Level2",
                "AddPerkToAllCollections /Inventory/Perks/Level2/WaterCons_Level2",
                "AddPerkToAllCollections /Inventory/Perks/Level2/WaterMax_Level2",
                "AddPerkToAllCollections /Inventory/Perks/Level2/Detox_Level2",
                "AddPerkToAllCollections /Inventory/Perks/Level2/ColdRes_Level2",
                "AddPerkToAllCollections /Inventory/Perks/Level2/HeatRes_Level2",
                "AddPerkToAllCollections /Inventory/Perks/Level2/Mining_Level2",
                "AddPerkToAllCollections /Inventory/Perks/Level2/InvMaxWgt_Level2",
                "AddPerkToAllCollections /Inventory/Perks/Level2/ExtraMeatRes_Level2",
                "AddPerkToAllCollections /Inventory/Perks/Level2/ExtraPlantRes_Level2",
                "AddPerkToAllCollections /Inventory/Perks/Level2/ExtraRockRes_Level2",
                "AddPerkToAllCollections /Inventory/Perks/Level2/FallDmg_Level2",
                "AddPerkToAllCollections /Inventory/Perks/Level2/HealthReg_Level2",
                "AddPerkToAllCollections /Inventory/Perks/Level2/HealthMax_Level2",
                "AddPerkToAllCollections /Inventory/Perks/Level2/StamMax_Level2",
                "AddPerkToAllCollections /Inventory/Perks/Level2/StamReg_Level2",
                "AddPerkToAllCollections /Inventory/Perks/Level2/Dmg_Level2",
            };
            await Batch(lines);
        }

        [Cheat]
        public static async Task AddAllPerks3()
        {
            string[] lines =
            {
                "AddPerkToAllCollections /Inventory/Perks/Level3/CalorieCons_Level3",
                "AddPerkToAllCollections /Inventory/Perks/Level3/WaterCons_Level3",
                "AddPerkToAllCollections /Inventory/Perks/Level3/WaterMax_Level3",
                "AddPerkToAllCollections /Inventory/Perks/Level3/SatMax_Level3",
                "AddPerkToAllCollections /Inventory/Perks/Level3/Detox_Level3",
                "AddPerkToAllCollections /Inventory/Perks/Level3/ColdRes_Level3",
                "AddPerkToAllCollections /Inventory/Perks/Level3/HeatRes_Level3",
                "AddPerkToAllCollections /Inventory/Perks/Level3/Mining_Level3",
                "AddPerkToAllCollections /Inventory/Perks/Level3/InvMaxWgt_Level3",
                "AddPerkToAllCollections /Inventory/Perks/Level3/FallDmg_Level3",
                "AddPerkToAllCollections /Inventory/Perks/Level3/HealthReg_Level3",
                "AddPerkToAllCollections /Inventory/Perks/Level3/HealthMax_Level3",
                "AddPerkToAllCollections /Inventory/Perks/Level3/StamMax_Level3",
                "AddPerkToAllCollections /Inventory/Perks/Level3/StamReg_Level3",
                "AddPerkToAllCollections /Inventory/Perks/Level3/Dmg_Level3",
            };
            await Batch(lines);
        }

        [Cheat]
        public static async Task AddSomePerks()
        {
            string[] lines =
            {
                "AddPerkToAllCollections /Inventory/Perks/Level1/HealthMax_Level1",
                "AddPerkToAllCollections /Inventory/Perks/Level1/Dmg_Level1",
                "AddPerkToAllCollections /Inventory/Perks/Level2/SatMax_Level2",
                "AddPerkToAllCollections /Inventory/Perks/Level2/InvMaxWgt_Level2",
                "AddPerkToAllCollections /Inventory/Perks/Level3/StamMax_Level3",
                "AddPerkToAllCollections /Inventory/Perks/Level3/Detox_Level3"
            };
            await Batch(lines);
        }


        [Cheat]
        public static async Task AddPerk1()
        {
            string[] lines =
            {
                "AddPerkToAllCollections /Inventory/Perks/Level1/HealthMax_Level1",
            };
            await Batch(lines);
        }

        [Cheat]
        public static async Task AddPerk2()
        {
            string[] lines =
            {
                "AddPerkToAllCollections /Inventory/Perks/Level2/Dmg_Level2",
            };
            await Batch(lines);
        }

        [Cheat]
        public static async Task AddPerk3()
        {
            string[] lines =
            {
                "AddPerkToAllCollections /Inventory/Perks/Level3/Detox_Level3",
            };
            await Batch(lines);
        }

        [Cheat]
        public static async Task AddWeight()
        {
            string[] lines =
            {
                "AddPerkToAllCollections /Inventory/Perks/Test/InvWeightMax_Level1_QA",
            };
            await Batch(lines);
        }

        [Cheat]
        public static async Task Stamina100k()
        {
            string[] lines =
            {
                "AddPerkToAllCollections /Inventory/Perks/Test/StamReg_Level2_QA",
            };
            await Batch(lines);
        }

        [Cheat]
        public static async Task GM()
        {
            string[] lines =
            {
                "AddPerkToAllCollections /Inventory/Perks/Test/UberPerk_GM",
            };
            await Batch(lines);
        }

        [Cheat]
        public static async Task SuperMan()
        {
            string[] lines =
            {
                "AddPerkToAllCollections /Inventory/Perks/Test/SuperManPerk",
            };
            await Batch(lines);
        }

        [Cheat]
        public static async Task AddHealthMaxPerks()
        {
            string[] lines =
            {     
                "AddPerkToAllCollections /Inventory/Perks/Level1/HealthMax_Level1",
                "AddPerkToAllCollections /Inventory/Perks/Level1/HealthMaxFallDmg_Level1",
                "AddPerkToAllCollections /Inventory/Perks/Level1/HealthMaxSatMax_Level1",
                "AddPerkToAllCollections /Inventory/Perks/Level2/HealthMax_Level2",
                "AddPerkToAllCollections /Inventory/Perks/Level2/HealthMaxWaterCons_Level2",
                "AddPerkToAllCollections /Inventory/Perks/Level3/HealthMaxSatMax_Level3",
                "AddPerkToAllCollections /Inventory/Perks/Level3/HealthMax_Level3"          
            };
            await Batch(lines);
        }

        [Cheat]
        public static async Task AddHealthRegPerks()
        {
            string[] lines =
            {   
                "AddPerkToAllCollections /Inventory/Perks/Level1/HealthReg_Level1",
                "AddPerkToAllCollections /Inventory/Perks/Level1/HealthRegHeatRes_Level1",
                "AddPerkToAllCollections /Inventory/Perks/Level1/HealthRegWaterMax_Level1",               
                "AddPerkToAllCollections /Inventory/Perks/Level2/HealthReg_Level2",
                "AddPerkToAllCollections /Inventory/Perks/Level2/HealthRegColdRes_Level2",
                "AddPerkToAllCollections /Inventory/Perks/Level2/HealthRegWaterCons_Level2",
                "AddPerkToAllCollections /Inventory/Perks/Level3/HealthReg_Level3",
                "AddPerkToAllCollections /Inventory/Perks/Level3/HealthRegFallDmg_Level3" 
            };
            await Batch(lines);
        }

        [Cheat]
        public static void SpellModifiersTest()
        {
            string[] lines =
            {
                "AddR1 100",
                "AddR2 100",
                "AddPerkToAllCollections /Inventory/Perks/Test/TagsTestPerk",
                "AddPerkToAllCollections /Inventory/Perks/Test/TagsTestPerk2",
                "AddItem /Inventory/Items/Weapons/New/Sword_2",
                "spawn_bot 1 s dropzone"
            };
            Batch(lines);
        }
        
        // Читы на науки
      
        [Cheat]
        public static async Task AddDKW()
        {
            await AddKnowledge("/Inventory/Knowledge/Quest/DropZone/FinishTutorialQuest1KW");
        }

        [Cheat]
        public static async Task AddAllKnowledges()
        {
            await AddKnowledge("/Inventory/Knowledge/Test/SciAllKW");
        }

        [Cheat]
        public static async Task AddZoology()
        {
            await AddKnowledge("/Inventory/Knowledge/Test/SciSurvivalKW");
        }

        [Cheat]
        public static async Task AddSurveying()
        {
            await AddKnowledge("/Inventory/Knowledge/Test/SciMechanicsKW");
        }

        [Cheat]
        public static async Task AddBotany()
        {
            await AddKnowledge("/Inventory/Knowledge/Test/SciChemistryKW");
        }

        [Cheat]
        public static async Task AddGeology()
        {
            await AddKnowledge("/Inventory/Knowledge/Test/SciMetallurgyKW");
        }

        // Чит на атлас

        [Cheat]
        public static async Task AddAtlas()
        {

            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Armor_1KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Armor_2KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Armor_3KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Armor_4KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Backpack_2KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Backpack_3KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Backpack_4KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/BackpackT1Yutt_1KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Battle_potion_3KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Battle_Potion_4KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/BattlePotion_2KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Belt_4KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/BeltBasicKW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Building1KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Building2KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Building3KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Chest_1KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Chest_2KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Chest_3KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Chest_4KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Cocoon_1KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Cold_protection_3KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/ColdProtectionSoupKW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Concentrated_life_essenceKW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/DecoyKvarAlphaKW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/DryedJuicyMeatKW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/DryedToughMeat_1KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/fire_protection_1KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Fireplace_1KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Flask_1KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/GasStationKW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Hardened_fungusKW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Hardened_scyphopod_limbKW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Healing_3KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Healing_4KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/HealingBandageKW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Heat_protection_3KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Kebab_4KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Knife_1_1KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Knife_1_2KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Knife_2KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Knife_3KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Knife_4KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Meat_cucumber_salatKW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/MeatStewKW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Pick_1_1KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Pick_1_2KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Pick_2KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Pick_3KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Pick_4KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Pickled_meridKW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/pressed_kvar_hideKW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Pressed_leather_3KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Pressed_leather_4KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/pressed_normal_hideKW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/ReinforcedHullFragmentKW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/ReinforcedMeridCarapaceKW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/RoastedGerophit_1KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/RoastedJuicyMeatKW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/RoastedToughMeat_1KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/rope_1KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Snow_Kvar_steakKW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Spear_1_1KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Spear_1_2KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Spear_2KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Spear_3KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Spear_4KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Swamp_decoctKW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Sword_1KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Sword_2KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Sword_3KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Sword_4KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/TemperedSpiralStemKW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Torch_1KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Toxic_protection_2KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Toxic_protection_3KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Toxic_protection_4KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/WarAxe_1KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/WarAxe_2KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/WarAxe_3KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/WarAxe_4KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/WarHammer_1_1KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/WarHammer_1_2KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/WarHammer_2KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/WarHammer_3KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/WarHammer_4KW");
        }

        // Чит на стройку

        [Cheat]
        public static async Task AddBuild()
        {
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Building1KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Building2KW");
            await AddKnowledge("/Inventory/Knowledge/FromTechnologies/Atlas_Sessions/Building3KW");
        }

        // Чит для цепочки квестов в альт PvE
        
        [Cheat]
        public static async Task AddAlt()
        {
            await AsyncUtils.RunAsyncTask(() => InventorySetSize(2, true, InventoryAddressResolver));
            await Task.Delay(1000);
            string[] lines =
            {
                "/Inventory/Items/Quest/Key01Quest/VoidDNA_1 10",
                "/Inventory/Items/Weapons/Test/Thors_Hammer",
            };
            await AddItemsList(lines);
            await AddKnowledge("/Inventory/Knowledge/Quest/Alt_Key02Quest/Alt_Key02Quest_Point_01KW");
            await AddKnowledge("/Inventory/Knowledge/Quest/Alt_Key02Quest/Alt_Key02Quest_Point_02KW");
            await AddKnowledge("/Inventory/Knowledge/Quest/Alt_Key02Quest/Alt_Key02Quest_Point_03KW");
            await AddKnowledge("/Inventory/Knowledge/Quest/Alt_Key02Quest/Alt_Key02Quest_Point_04KW");
            await AddKnowledge("/Inventory/Knowledge/Quest/Alt_Key02Quest/Alt_Key02Quest_Point_05KW");
            await AddKnowledge("/Inventory/Knowledge/Quest/Alt_Key02Quest/Alt_Key02Quest_Point_AlternativeKW");
        }

        //Чит на добавление основного квеста

        [Cheat] 
        public static async Task AddKeyQuest()
        {
            var quest = GameResourcesHolder.Instance.LoadResource<QuestDef>("/Inventory/Quest/KeyMainQuest/KeyMainQuest");
            await AddQuest(quest);
        }

        // Читы на спеллы

        [Cheat]
        public static async Task AddExp10000(short x = 1)
        {
            for (short i = 0; i < x; i++)
            {
                await Spell("/Inventory/Quest/Test_QA/TestExpSpell_10000");
            }
        }

        [Cheat]
        public static async Task AddExp1000(short x = 1)
        {
            for (short i = 0; i < x; i++)
            {
                await Spell("/Inventory/Quest/Test_QA/TestExpSpell_1000");
            }
        }

        [Cheat]
        public static async Task AddExp100(short x = 1)
        {
            for (short i = 0; i < x; i++)
            {
                await Spell("/Inventory/Quest/Test_QA/TestExpSpell_100");
            }
        }

        [Cheat]
        public static async Task MinusTemperature(short x)
        {
            for (short i = 0; i < x; i++)
            {
                await Spell("/UtilPrefabs/Spells/QA/MinusTemperature");
            }
        }

        [Cheat]
        public static async Task PlusTemperature(short x)
        {
            for (short i = 0; i < x; i++)
            {
                await Spell("/UtilPrefabs/Spells/QA/PlusTemperature");
            }
        }

        [Cheat]
        public static async Task MinusToxic(short x)
        {
            for (short i = 0; i < x; i++)
            {
                await Spell("/UtilPrefabs/Spells/QA/MinusToxic");
            }
        }

        [Cheat]
        public static async Task PlusToxic(short x)
        {
            for (short i = 0; i < x; i++)
            {
                await Spell("/UtilPrefabs/Spells/QA/PlusToxic");
            }
        }

        [Cheat]
        public static async Task MinusSatiety(short x)
        {
            for (short i = 0; i < x; i++)
            {
                await Spell("/UtilPrefabs/Spells/QA/MinusSatiety");
            }
        }

        [Cheat]
        public static async Task PlusSatiety(short x)
        {
            for (short i = 0; i < x; i++)
            {
                await Spell("/UtilPrefabs/Spells/QA/PlusSatiety");
            }
        }

        [Cheat]
        public static async Task MinusWater(short x)
        {
            for (short i = 0; i < x; i++)
            {
                await Spell("/UtilPrefabs/Spells/QA/MinusWater");
            }
        }

        [Cheat]
        public static async Task PlusWater(short x)
        {
            for (short i = 0; i < x; i++)
            {
                await Spell("/UtilPrefabs/Spells/QA/PlusWater");
            }
        }

        [Cheat]
        public static async Task Killme()
        {
            await ChangeHealth(GameState.Instance.CharacterRuntimeData.CharacterId, -10000000);
        }

        [Cheat]
        public static async Task KillBot(int idx)
        {
            Guid guid;
            lock (_BotsList)
                guid = _BotsList[idx - 1];
            await ChangeHealth(guid, -10000000);
        }

        private static CancellationTokenSource _cts;
        private static SuspendingAwaitable _killTask;

        [Cheat]
        public static async Task KillServerByAddingItems()
        {
            var item = "/Inventory/Items/Weapons/Test/Thors_Hammer";
            var itempath = FolderLoader.CleanUpRelativePath(item);
            BaseItemResource res = GameResourcesHolder.Instance.LoadResource<BaseItemResource>(itempath);
            var pack = new List<ItemResourcePack>() { new ItemResourcePack(res, 1) };

            Guid entityId = GameState.Instance.CharacterRuntimeData.CharacterId;
            Type entityType = typeof(IWorldCharacter);
            var typeId = ReplicaTypeRegistry.GetIdByType(entityType);

            var worldSceneRepoId = GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId;

            var cts = new CancellationTokenSource();

            var oldCts = Interlocked.Exchange(ref _cts, cts);
            if (oldCts != null)
            {
                Logger.IfWarn()?.Message("Server killer is active, restarting").Write();
                await StopKillingServerImpl(oldCts);
            }

            var token = cts.Token;
            _killTask = AsyncUtils.RunAsyncTask(async () =>
            {
                PropertyAddress sourceAddress;
                using (var wrapper = await GameState.Instance.ClientClusterNode.Get(typeId, entityId))
                {
                    var worldCharacter = wrapper.Get<IHasInventoryClientFull>(typeId, entityId, ReplicationLevel.ClientFull);
                    sourceAddress = EntityPropertyResolver.GetPropertyAddress(worldCharacter.Inventory);
                }

                int count = 0;
                using (var cheatServiceWrapper = await GameState.Instance.ClientClusterNode.Get<ICheatServiceEntityServer>(worldSceneRepoId))
                {
                    var cheatServiceEntity = cheatServiceWrapper.Get<ICheatServiceEntityServer>(worldSceneRepoId);
                    while (true)
                    {
                        if (count++ % 1000 == 0)
                             Logger.IfInfo()?.Message("Server killer: {0} items is added...",  count).Write();

                        if (token.IsCancellationRequested)
                        {
                             Logger.IfInfo()?.Message("Server killer: stopping").Write();;
                            token.ThrowIfCancellationRequested();
                        }

                        await cheatServiceEntity.AddSomeItems(pack, sourceAddress);
                    }
                }
            });
        }

        [Cheat]
        public static string Teleport(string args)
        {
            var pawn = SurvivalGuiNode.Instance?.OurUserPawn;
            if(pawn == null)
            {
                return "No pawn set - cant teleport";
            }
            if (SharedCode.Utils.Vector3.TryParse(args, out var position))
            {
                DoTeleport(pawn, position.ToUnity(), pawn.transform.rotation);
                return "Teleported to coords";
            }
            var pawnPosition = pawn.transform.position;
            var points = Object.FindObjectsOfType<PlayerSpawnPoint>()?.OrderBy(p => Vector3.Distance(p.transform.position, pawnPosition));
            if (points.AssertIfNull(nameof(points)))
            {
                return "No spawn points found";
            }
            foreach (var playerSpawnPoint in points)
            {
                if (playerSpawnPoint.name == args || string.IsNullOrEmpty(args))
                {
                    DoTeleport(pawn, playerSpawnPoint.transform.position, playerSpawnPoint.transform.rotation);
                    return "teleported to spawn point";
                }
            }
            return "Spawn point not found";
        }
        private static void DoTeleport(GameObject pawn, Vector3 position, Quaternion rotation)
        {
             Logger.IfInfo()?.Message("Teleport {0} to {1}",  pawn, position).Write();
            pawn.transform.SetPositionAndRotation(position, rotation);
        }

        [Cheat]
        public static ValueTask StopKillingServer()
        {
            var cts = Interlocked.Exchange(ref _cts, null);
            if (cts == null)
            {
                Logger.IfWarn()?.Message("Server killer is not running").Write();
                return new ValueTask();
            }

            return StopKillingServerImpl(cts);
        }

        public static async ValueTask StopKillingServerImpl(CancellationTokenSource cts)
        {
            cts.Cancel();
            try
            {
                await _killTask;
                 Logger.IfInfo()?.Message("Server killer is stopped").Write();;
            }
            catch (OperationCanceledException)
            {
                 Logger.IfInfo()?.Message("Server killer is stopped").Write();;
            }
            catch (Exception e)
            {
                Logger.IfInfo()?.Message(e, "Server killer is stopped with excpetion").Write();
            }
            finally
            {
                cts.Dispose();
            }
        }

        // Execute every line from file as separate cheat
        [Cheat]
        public static async Task Exec(string cheatfile)
        {
            var cheatfilePath = Application.dataPath + "/BotCheats/" + cheatfile;
            await Batch(cheatfilePath);
        }


        [Cheat]
        public static async Task Batch(string fullPathToBatchFile)
        {
            string[] lines = File.ReadAllLines(fullPathToBatchFile);
            await Batch(lines);
        }

        private static async ValueTask Batch(string[] lines)
        {
            bool isCommented = false;
            foreach (var line in lines)
            {
                if (line.StartsWith("/*"))
                    isCommented = true;
                else if (line.StartsWith("*/"))
                    isCommented = false;


                if (!isCommented && !line.StartsWith("//") && line.Length > 0)
                {
                    var res = await CheatsManager.ExecuteCommand(line);
                    Logger.Info(res);
                    if (res.IsSuccess())
                        Logger.IfInfo()?.Message(res.ToString()).Write();
                    else
                        Logger.IfError()?.Message(res.ToString()).Write();
                }
            }
        }

        [Cheat]
        public static async Task ChangeHealth(int deltaValue)
        {
            await ChangeHealth(GameState.Instance.CharacterRuntimeData.CharacterId, deltaValue);
        }
        
        private static async Task ChangeHealth(Guid characterId, int deltaValue)
        {
        
            // Tmp player specific part ('ll be rewrote):

            var typeId = ReplicaTypeRegistry.GetIdByType(typeof(IWorldCharacter));
            if (characterId == Guid.Empty)
            {
                Logger.IfError()?.Message($"{nameof(characterId)} is empty").Write();
                return;
            }

            // Universal part:

            var @ref = new OuterRef<IEntity>(characterId, typeId);

            var repo = GameState.Instance.ClientClusterNode;
            await AsyncUtils.RunAsyncTask(async () =>
            {
                var sceneId = GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId;
                using (var wrapper = await repo.Get<ICheatServiceEntityServer>(sceneId))
                {
                    var cheatServiceEntity = wrapper?.Get<ICheatServiceEntityServer>(sceneId);
                    await cheatServiceEntity.ChangeHealth(@ref, deltaValue);
                    Logger.IfInfo()?.Message($"{nameof(ChangeHealth)}({deltaValue}) @ref:{@ref}. Done").Write();
                }
            });
        }

        [Cheat]
        public static async Task Godmode(bool enable = true)
        {
            // Tmp player specific part ('ll be rewrote):

            var typeId = ReplicaTypeRegistry.GetIdByType(typeof(IWorldCharacter));
            var characterId = GameState.Instance.CharacterRuntimeData.CharacterId;
            if (characterId == Guid.Empty)
            {
                Logger.IfError()?.Message($"{nameof(characterId)} is empty").Write();
                return;
            }

            // Universal part:

            var @ref = new OuterRef<IEntity>(characterId, typeId);

            var repo = GameState.Instance.ClientClusterNode;
            await AsyncUtils.RunAsyncTask(async () =>
            {
                var sceneId = GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId;
                using (var wrapper = await repo.Get<ICheatServiceEntityServer>(sceneId))
                {
                    var cheatServiceEntity = wrapper?.Get<ICheatServiceEntityServer>(sceneId);
                    await cheatServiceEntity.Godmode(@ref, enable);
                    Logger.IfInfo()?.Message($"{nameof(Godmode)} @ref:{@ref}. Done").Write();
                }
            });
        }

        [Cheat]
        public static void Initialize_Bots_System()
        {
            BotCoordinator.IsActive = true;
            Logger.IfInfo()?.Message($"Bots System is initialized!").Write();
        }

        private static readonly List<Guid> _BotsList = new List<Guid>();

        [Cheat]
        public static async Task Bot(int botCount, string configPath, string spawnPoint, string gender = null)
        {
            await Spawn_Bot(botCount, configPath, spawnPoint, gender);
        }
        
        [Cheat]
        public static async Task Spawn_Bot(int botCount, string configPath, string spawnPoint, string gender = null)
        {
            var repository = GameState.Instance.ClientClusterNode;
            await AsyncUtils.RunAsyncTask(async () =>
            {
                if (repository.AssertIfNull(nameof(repository)))
                    return;

                List<Guid> botsIds = new List<Guid>();
                for (int i = 0; i < botCount; i++)
                    botsIds.Add(Guid.NewGuid());
                var characterId = GameState.Instance.CharacterRuntimeData?.CharacterId ?? Guid.Empty;
                if (Guid.Empty == characterId)
                {
                    UI.Logger.IfError()?.Message($"{nameof(characterId)} is empty").Write();
                    return;
                }

                MapOwner ownMap;
                using (var worldCharacterWrapper = await repository.Get<IWorldCharacterClientFull>(characterId))
                {
                    if (worldCharacterWrapper.AssertIfNull(nameof(worldCharacterWrapper)))
                        return;

                    var worldCharacter = worldCharacterWrapper.Get<IWorldCharacterClientFull>(characterId);
                    if (worldCharacter.AssertIfNull(nameof(worldCharacter)))
                        return;
                    ownMap = worldCharacter.MapOwner;
                }

                LegionaryEntityDef botConfig = null;
                var index = GameResourcesHolder.Instance.LoadResource<BotsIndexDef>("/Bots/_index_");
                if (index?.Index != null && index.Index.TryGetValue(configPath, out var @ref))
                    botConfig = @ref.Target;

                if (botConfig == null)
                    botConfig = GameResourcesHolder.Instance.LoadResource<LegionaryEntityDef>(configPath);

                if (botConfig != null)
                {
                    Logger.IfInfo()?.Message($"Spawned bot {botConfig.____GetDebugShortName()}").Write();
                    List<OuterRef<IEntity>> botsRefs = botsIds.Select(v => new OuterRef<IEntity>(v, ReplicaTypeRegistry.GetIdByType(typeof(IWorldCharacter)))).ToList();

                    using (var wrapper = await repository.Get<IMapEntity>(ownMap.OwnerMapId))
                    {
                        if (wrapper.AssertIfNull(nameof(wrapper)))
                            return;

                        var wnse = wrapper.Get<IMapEntityClientBroadcast>(ownMap.OwnerMap.TypeId, ownMap.OwnerMap.Guid, ReplicationLevel.ClientBroadcast);
                        using (var wrapper2 = await repository.GetFirstService<IBotCoordinatorServer>())
                        {
                            if (wrapper2 == null)
                                 Logger.IfError()?.Message("IBotCoordinator not found {0}",  repository.Id).Write();

                            var service = wrapper2?.GetFirstService<IBotCoordinatorServer>();
                            await service.Initialize(wnse.Map, botsRefs, botConfig);
                        }

                        await wnse.SpawnNewBots(botsIds, "/SpawnSystem/SpawnPointTypes/" + spawnPoint);
                        lock (_BotsList)
                            _BotsList.AddRange(botsIds);

                        if (!string.IsNullOrEmpty(gender))
                            AsyncUtils.RunAsyncTask(
                                async () =>
                                {
                                    await Task.Delay(3000);
                                    foreach (var botsRef in botsRefs)
                                        await ChangeGender(botsRef, gender);
                                });
                    }
                }
                else
                    Logger.IfError()?.Message($"Can't spawn bot due config file '{configPath}' is not exists or broken").Write();

            });
        }

        //@param `minTimeout` - timeout 'll be >= defined in entity method called here.
        [Cheat]
        public static async Task UnstuckTeleport(float minTimeout)
        {
             Logger.IfInfo()?.Message("1. UnstuckTeleport").Write();;
            //if (DbgLog.Enabled) DbgLog.Log("1. UnstuckTeleport");

            var charRef = Helpers.GetCharacterRef();
            if (!charRef.IsValid)
            {
                 Logger.IfError()?.Message("Can't get character ref.").Write();;
                return;
            }

            var repo = GameState.Instance.ClientClusterNode;
            await AsyncUtils.RunAsyncTask(async () =>
                {
                    using (var wrapper = await repo.Get(charRef))
                    {
                        var worldCharacter = wrapper.Get<IWorldCharacterClientFull>(charRef, ReplicationLevel.ClientFull);
                        if (worldCharacter != null)
                            await worldCharacter.UnstuckTeleport(minTimeout);
                    }
                });
        }

        private static async Task AddEmptyPerkToCollection(AddressResolver resolver, int count = 0)
        {
            var characterId = GameState.Instance.CharacterRuntimeData.CharacterId;
            await AsyncUtils.RunAsyncTask(async () =>
            {
                await InventorySetSize(1, true, resolver);
                await AddSomeItemsImpl(characterId, typeof(IWorldCharacter), new List<ItemResourcePack>() { new ItemResourcePack(null, (uint)count, -1) }, resolver);
            });
        }

        [Cheat]
        public static async Task AddEmptyPerkAsync()
        {
            await AddEmptyPerkToCollection(PermanentPerksAddressResolver);
            await AddEmptyPerkToCollection(SavedPerksAddressResolver);
        }

        [Cheat]
        public static async Task AddPerkToAllCollections([NotNull] BaseItemResource itemName)
        {
            await AddSomeItemsDo(new List<ItemResourcePack>() { new ItemResourcePack(itemName, 1, -1) }, TemporaryPerksAddressResolver);
        }

        // chain items by '+'
        [Cheat]
        public static async Task AddSomeItems([NotNull] string itemNames, int count = 1)
        {
            GetItemResourcePackFromString(itemNames, count, out List<ItemResourcePack> resources);
            await AddSomeItemsDo(resources, InventoryAddressResolver);
        }

        public static void GetItemResourcePackFromString(in string itemNames, int count, out List<ItemResourcePack> resources)
        {
            var names = itemNames.Split('+');
            resources = new List<ItemResourcePack>();
            foreach (var name in names)
            {
                BaseItemResource res = GameResourcesHolder.Instance.LoadResource<BaseItemResource>(name);
                if (res != null)
                    resources.Add(new ItemResourcePack(res, (uint)count));
            }
        }

        private static async Task AddItemsList([NotNull] string[] itemsList)
        {
            var resources = new List<ItemResourcePack>();
            foreach (var name in itemsList)
            {
                var names = name.Split(' ');
                var itempath = FolderLoader.CleanUpRelativePath(names[0]);
                BaseItemResource res = GameResourcesHolder.Instance.LoadResource<BaseItemResource>(itempath);
                int count = names.Count() > 1 ? int.Parse(names[1]) : 1;
                if (res != null)
                    resources.Add(new ItemResourcePack(res, (uint)count));
            }

            await AddSomeItemsDo(resources, InventoryAddressResolver);
        }
        
        private static async Task AddItems([NotNull] IEnumerable<ItemResource> itemsList)
        {
            var resources = new List<ItemResourcePack>();
            foreach (var res in itemsList)
            {
                int count = 1;
                if (res != null)
                    resources.Add(new ItemResourcePack(res, (uint)count));
            }
            await AddSomeItemsDo(resources, InventoryAddressResolver);
        }

        [Cheat]
        public static async Task GetCCUOnCurrentScene()
        {
            await AsyncUtils.RunAsyncTask(async () =>
            {
                var sceneId = GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId;
                using (var wrapper = await GameState.Instance.ClientClusterNode.Get<ICheatServiceEntityServer>(sceneId))
                {
                    var cheatServiceEntity = wrapper.Get<ICheatServiceEntityServer>(sceneId);
                    var ccu = await cheatServiceEntity.GetCCU();
                     Logger.IfError()?.Message("Current scene CCU: {0}",  ccu).Write();
                }
            });
        }

        private delegate Task<PropertyAddress> AddressResolver(Guid guid, Type type);

        private static async Task<PropertyAddress> InventoryAddressResolver(Guid entityId, Type entityType)
        {
            var typeId = ReplicaTypeRegistry.GetIdByType(entityType);
            using (var wrapper = await GameState.Instance.ClientClusterNode.Get(typeId, entityId))
            {
                var worldCharacter = wrapper.Get<IHasInventoryClientFull>(typeId, entityId, ReplicationLevel.ClientFull);
                return EntityPropertyResolver.GetPropertyAddress(worldCharacter.Inventory);
            }
        }

        private static async Task<PropertyAddress> DollAddressResolver(Guid entityId, Type entityType)
        {
            var typeId = ReplicaTypeRegistry.GetIdByType(entityType);
            using (var wrapper = await GameState.Instance.ClientClusterNode.Get(typeId, entityId))
            {
                var worldCharacter = wrapper.Get<IHasDollClientBroadcast>(typeId, entityId, ReplicationLevel.ClientBroadcast);
                return EntityPropertyResolver.GetPropertyAddress(worldCharacter.Doll);
            }
        }

        private static async Task<PropertyAddress> TemporaryPerksAddressResolver(Guid characterId, Type entityType)
        {
            using (var wrapper = await GameState.Instance.ClientClusterNode.Get<IWorldCharacterClientFull>(characterId))
            {
                var worldCharacter = wrapper.Get<IWorldCharacterClientFull>(characterId);
                return EntityPropertyResolver.GetPropertyAddress(worldCharacter.TemporaryPerks);
            }
        }

        private static async Task<PropertyAddress> SavedPerksAddressResolver(Guid characterId, Type entityType)
        {
            using (var wrapper = await GameState.Instance.ClientClusterNode.Get<IWorldCharacterClientFull>(characterId))
            {
                var worldCharacter = wrapper.Get<IWorldCharacterClientFull>(characterId);
                return EntityPropertyResolver.GetPropertyAddress(worldCharacter.SavedPerks);
            }
        }

        private static async Task<PropertyAddress> PermanentPerksAddressResolver(Guid characterId, Type entityType)
        {
            using (var wrapper = await GameState.Instance.ClientClusterNode.Get<IWorldCharacterClientFull>(characterId))
            {
                var worldCharacter = wrapper.Get<IWorldCharacterClientFull>(characterId);
                return EntityPropertyResolver.GetPropertyAddress(worldCharacter.PermanentPerks);
            }
        }


        [Cheat]
        public static async Task AddItemResource([NotNull] BaseItemResource itemName, int count = 1, int slotId = -1)
        {
            await AddSomeItemsDo(new List<ItemResourcePack>() { new ItemResourcePack(itemName, (uint)count, slotId) }, InventoryAddressResolver);
        }

        [Cheat]
        public static async Task AddItem([NotNull] string itemPath, int count = 1, int slotId = -1, string guid = "", string type = "", bool toInventory = true)
        {
            itemPath = FolderLoader.CleanUpRelativePath(itemPath);
            BaseItemResource res = GameResourcesHolder.Instance.LoadResource<BaseItemResource>(itemPath);

            AddressResolver addressResolver;
            if (toInventory)
                addressResolver = InventoryAddressResolver;
            else
                addressResolver = DollAddressResolver;

            await AddSomeItemsDo(new List<ItemResourcePack>() { new ItemResourcePack(res, (uint)count, slotId) }, addressResolver, guid, type);
        }

        [Cheat]
        public static async Task AddR1(int count)
        {
            await CountR1R2("TechPointR1", count);
        }

        [Cheat]
        public static async Task AddR2(int count)
        {
            await CountR1R2("TechPointR2", count);
        }

        private static async Task CountR1R2(string techpoint, int count)
        {
            await AsyncUtils.RunAsyncTask(async () =>
            {
                if (GameState.Instance.ClientClusterNode.AssertIfNull(nameof(GameState.Instance.ClientClusterNode)))
                    return;

                var resource = GameResourcesHolder.Instance.LoadResource<CurrencyResource>("/Inventory/Currencies/" + techpoint);

                var characterId = GameState.Instance.CharacterRuntimeData.CharacterId;
                if (Guid.Empty == characterId)
                {
                    UI.Logger.IfError()?.Message($"{nameof(characterId)} is empty").Write();
                    return;
                }

                using (var cheatServiceWrapper = await GameState.Instance.ClientClusterNode.Get<ICheatServiceEntityServer>(
                    GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId))
                {
                    if (cheatServiceWrapper.AssertIfNull(nameof(cheatServiceWrapper)))
                        return;

                    var cheatServiceEntity = cheatServiceWrapper.Get<ICheatServiceEntityServer>(GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId);

                    if (cheatServiceEntity.AssertIfNull(nameof(cheatServiceEntity)))
                        return;

                    await cheatServiceEntity.AddTechPoints(new TechPointCount[] { new TechPointCount() { TechPoint = resource, Count = count } }, characterId);
                }
            });
        }

        [Cheat]
        public static async Task AddKnowledge(string knowledgePath)
        {
            await AsyncUtils.RunAsyncTask(async () =>
            {
                if (GameState.Instance.ClientClusterNode.AssertIfNull(nameof(GameState.Instance.ClientClusterNode)))
                    return;

                var knowledgeDef = GameResourcesHolder.Instance.LoadResource<KnowledgeDef>(knowledgePath);

                var characterId = GameState.Instance.CharacterRuntimeData.CharacterId;
                if (Guid.Empty == characterId)
                {
                    UI.Logger.IfError()?.Message($"{nameof(characterId)} is empty").Write();
                    return;
                }

                using (var cheatServiceWrapper = await GameState.Instance.ClientClusterNode.Get<ICheatServiceEntityServer>(
                    GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId))
                {
                    if (cheatServiceWrapper.AssertIfNull(nameof(cheatServiceWrapper)))
                        return;

                    var cheatServiceEntity = cheatServiceWrapper.Get<ICheatServiceEntityServer>(GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId);

                    if (cheatServiceEntity.AssertIfNull(nameof(cheatServiceEntity)))
                        return;

                    await cheatServiceEntity.AddKnowledge(knowledgeDef, characterId);
                }
            });
        }

        [Cheat]
        public static async Task CraftEngine()
        {
            await AsyncUtils.RunAsyncTask(async () =>
            {
                if (GameState.Instance.ClientClusterNode.AssertIfNull(nameof(GameState.Instance.ClientClusterNode)))
                    return;

                var characterId = GameState.Instance.CharacterRuntimeData.CharacterId;
                if (Guid.Empty == characterId)
                {
                    UI.Logger.IfError()?.Message($"{nameof(characterId)} is empty").Write();
                    return;
                }

                using (var craftEngineWrapper = await GameState.Instance.ClientClusterNode.Get<ICraftEngineClientFull>(characterId))
                {
                    if (craftEngineWrapper.AssertIfNull(nameof(craftEngineWrapper)))
                        return;

                    var craftEngine = craftEngineWrapper.Get<ICraftEngineClientFull>(characterId);
                    if (craftEngine.AssertIfNull(nameof(craftEngine)))
                        return;

                    int currentCraftIndex = craftEngine.CraftingQueue.Keys.Min();
                    var str = new StringBuilder();
                    foreach (var queue in craftEngine.CraftingQueue)
                    {
                        str.AppendLine(queue.Key + ": " + queue.Value.CraftRecipe.____GetDebugShortName() + " (" + queue.Value.Count + ")" +
                            ((currentCraftIndex == queue.Key) ? ((SyncTime.Now - craftEngine.StartCraftingTimeUTC0InMilliseconds) * 0.001f).ToString() : ""));
                    }

                    Logger.IfError()?.Message($"CraftEngine: \n{str.ToString()}").Write();
                }
            });
        }

        private static async Task AddSomeItemsDo([NotNull] List<ItemResourcePack> itemsPack, AddressResolver sourceAddressResolver, string guid = "", string type = "")
        {
            Guid characterId = string.IsNullOrEmpty(guid) ? GameState.Instance.CharacterRuntimeData.CharacterId : Guid.Parse(guid);
            Type entityType = string.IsNullOrEmpty(type) ? typeof(IWorldCharacter) : ReplicaTypeRegistry.GetTypeByName(type);
            await AsyncUtils.RunAsyncTask(() => AddSomeItemsImpl(characterId, entityType, itemsPack, sourceAddressResolver));
        }

        private static async Task AddSomeItemsImpl(Guid entityId, Type type, [NotNull] List<ItemResourcePack> itemsPack, AddressResolver sourceAddressResolver)
        {
            if (GameState.Instance.ClientClusterNode.AssertIfNull(nameof(GameState.Instance.ClientClusterNode)))
                return;

            if (Guid.Empty == entityId)
            {
                UI.Logger.IfError()?.Message($"{nameof(entityId)} is empty").Write();
                return;
            }

            using (var cheatServiceWrapper = await GameState.Instance.ClientClusterNode.Get<ICheatServiceEntityServer>(GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId))
            {
                if (cheatServiceWrapper.AssertIfNull(nameof(cheatServiceWrapper)))
                    return;

                var sourceAddress = await sourceAddressResolver(entityId, type);
                if (sourceAddress.AssertIfNull(nameof(sourceAddress)))
                    return;

                var cheatServiceEntity = cheatServiceWrapper
                    .Get<ICheatServiceEntityServer>(GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId);

                if (cheatServiceEntity.AssertIfNull(nameof(cheatServiceEntity)))
                    return;

                await cheatServiceEntity.AddSomeItems(itemsPack, sourceAddress);
            }
        }

        private static ChainCancellationToken _chainCancellationToken;

        /*
        [Cheat]
        public static async void ChainTest([NotNull] ItemResource itemName, int count = 1, int slotId = -1)
        {
            if (GameState.Instance.ClientClusterNode.AssertIfNull(nameof(GameState.Instance.ClientClusterNode)))
                return;

            if (count == 0)
                count = 1;

            slotId = 0;

            var characterId = GameState.Instance.CharacterRuntimeData.CharacterId;

            using (var cheatServiceWrapper =
                await NodeAccessor.Repository.Get<ICheatServiceEntityServer>(GameState.Instance.CharacterRuntimeData
                    .CurrentPrimaryWorldSceneRepositoryId))
            using (var wrapper = await NodeAccessor.Repository.Get<IWorldCharacterClientFull>(characterId))
            {
                var worldCharacter = wrapper.Get<IWorldCharacterClientFull>(characterId);
                var sourceAddress = EntityPropertyResolver.GetPropertyAddress(worldCharacter, nameof(worldCharacter.Inventory));


                var chain = cheatServiceWrapper
                    .Get<ICheatServiceEntityServer>(GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId).Chain()
                    .Delay(6, true)
                    .AddItem(GameResourcesHolder.Instance.GetID(itemName).ToString(), count, slotId, sourceAddress);
                _chainCancellationToken = worldCharacter.ContinueChain(chain)
                    .Delay(2)
                    .MoveItem(sourceAddress, slotId, sourceAddress, slotId + 1, count, Guid.Empty)
                    .Delay(2)
                    .RemoveItem(sourceAddress, slotId + 1, count, Guid.Empty)
                    .Run();
            }
        }*/

        [Cheat]
        public static void CancelChainTest()
        {
            _chainCancellationToken.Cancel(GameState.Instance.ClientClusterNode);
        }

        [Cheat]
        public static async void RemoveItem(int sourceSlot, int count = -1)
        {
            if (GameState.Instance.ClientClusterNode.AssertIfNull(nameof(GameState.Instance.ClientClusterNode)))
                return;

            var characterId = GameState.Instance.CharacterRuntimeData.CharacterId;

            using (var wrapper = await GameState.Instance.ClientClusterNode.Get<IWorldCharacterClientFull>(characterId))
            {
                var worldCharacter = wrapper.Get<IWorldCharacterClientFull>(characterId);
                var itemSlot = worldCharacter.Inventory.Items[sourceSlot];
                var itemEntityId = itemSlot.Item.Id;
                var kindAddress = EntityPropertyResolver.GetPropertyAddress(worldCharacter.Inventory);
                if (count == -1)
                    count = itemSlot.Stack;
                var result = await worldCharacter.RemoveItem(kindAddress, sourceSlot, count, itemEntityId);
                 Logger.IfInfo()?.Message("Remove item result {0}",  result).Write();
            }
        }

        [Cheat]
        public static async Task SetSpawnPoint(string spawnPointType)
        {
            var characterId = GameState.Instance.CharacterRuntimeData.CharacterId;
            var repository = ClusterCommands.ClientRepository;
            SpawnPointTypeDef spawnPoint = GameResourcesHolder.Instance.LoadResource<SpawnPointTypeDef>("/SpawnSystem/SpawnPointTypes/" + spawnPointType);

            await AsyncUtils.RunAsyncTask(async () =>
            {
                using (var wrapper = await repository.Get<IWorldCharacterClientFull>(characterId))
                {
                    var worldCharacter = wrapper.Get<IWorldCharacterClientFull>(characterId);
                    await worldCharacter.AllowedSpawnPointSet(spawnPoint);
                     Logger.IfInfo()?.Message("SetSpawnPoint to {0}",  spawnPoint.____GetDebugShortName()).Write();
                }
            });
        }

        [Cheat]
        public static async Task ChangeMutation(float deltaValue, string faction = "Human")
        {
            var characterId = GameState.Instance.CharacterRuntimeData.CharacterId;
            var repository = ClusterCommands.ClientRepository;

            await AsyncUtils.RunAsyncTask(async () =>
            {
                using (var wrapper = await repository.Get<IWorldCharacterClientFull>(characterId))
                {
                    var worldCharacter = wrapper.Get<IWorldCharacterClientFull>(characterId);
                    var oldFaction = $"Faction: {new MutationState() { Faction = worldCharacter.MutationMechanics.Faction, Stage = worldCharacter.MutationMechanics.Stage, Mutation = worldCharacter.MutationMechanics.Mutation }}";

                    if (faction.IsNullOrWhitespace())
                        await worldCharacter.MutationMechanics.ChangeMutation(deltaValue, null, 0, false);
                    else
                        await worldCharacter.MutationMechanics.ChangeMutation(deltaValue, GameResourcesHolder.Instance.LoadResource<MutatingFactionDef>("/Inventory/Factions/" + faction), 0, false);

                    var newFaction = $"Faction: {new MutationState() { Faction = worldCharacter.MutationMechanics.Faction, Stage = worldCharacter.MutationMechanics.Stage, Mutation = worldCharacter.MutationMechanics.Mutation }}";

                    Logger.IfInfo()?.Message($"ChangeMutation: \n[{oldFaction}] -> \n[{newFaction}]").Write();
                }
            });
        }

        [Cheat]
        public static async Task AddMutHuman(float deltaValue)
        {
            await ChangeMutation(deltaValue, "Human");
        }

        // [Cheat]
        // public static void AddMutXen(float deltaValue)
        // {
        //     ChangeMutation(deltaValue, "Ksenobiot");
        // }


        [Cheat]
        public static async Task Inventory(string guidStr = "")
        {
            if (GameState.Instance.ClientClusterNode.AssertIfNull(nameof(GameState.Instance.ClientClusterNode)))
                return;

            var characterId = Guid.TryParse(guidStr, out Guid guid) ? guid : GameState.Instance.CharacterRuntimeData.CharacterId;
            var sb = new StringBuilder();
            await AsyncUtils.RunAsyncTask(async () =>
            {
                using (var wrapper = await GameState.Instance.ClientClusterNode.Get<IWorldCharacterClientFull>(characterId))
                {
                    var worldCharacter = wrapper.Get<IWorldCharacterClientFull>(characterId);
                    await LogItemsContainer(worldCharacter.Doll.Items, nameof(worldCharacter.Doll), sb);
                    await LogItemsContainer(worldCharacter.Inventory.Items, nameof(worldCharacter.Inventory), sb);
                    await LogItemsContainer(worldCharacter.TemporaryPerks.Items, nameof(worldCharacter.TemporaryPerks), sb);
                    await LogItemsContainer(worldCharacter.SavedPerks.Items, nameof(worldCharacter.SavedPerks), sb);
                    await LogItemsContainer(worldCharacter.PermanentPerks.Items, nameof(worldCharacter.PermanentPerks), sb);
                    await LogItemsContainer(worldCharacter.Currency.Items, nameof(worldCharacter.Currency), sb);
                    sb.AppendLine("---------------------------------");
                }

                Logger.IfInfo()?.Message(sb.ToString()).Write();
            });
        }

        private static async Task LogItemsContainer(IDeltaDictionaryWrapper<int, ISlotItemClientFull> items, string containerName, StringBuilder sb)
        {
            if (items.Any())
            {
                sb.AppendLine("-------- " + containerName + " -----------");
                foreach (var pair in items)
                {
                    var itemName = pair.Value.Item.ItemResource.____GetDebugShortName();
                    var itemStats = await pair.Value.Item.Stats.DumpStatsLocal(true);
                    itemStats = Regex.Replace(itemStats, @"\s{2,}", "");
                    sb.AppendLine($"{pair.Key,3}: {itemName,-15}[{pair.Value.Stack,3}]: {itemStats}");
                }
            }
        }

        [Cheat]
        public static async Task Quest()
        {
            if (GameState.Instance.ClientClusterNode.AssertIfNull(nameof(GameState.Instance.ClientClusterNode)))
                return;

            var characterId = GameState.Instance.CharacterRuntimeData.CharacterId;
            var sb = new StringBuilder();
            await AsyncUtils.RunAsyncTask(async () =>
            {
                using (var wrapper = await GameState.Instance.ClientClusterNode.Get<IWorldCharacterClientFull>(characterId))
                {
                    var worldCharacter = wrapper.Get<IWorldCharacterClientFull>(characterId);

                    sb.AppendLine("-------- Quests -----------");
                    foreach (var pair in worldCharacter.Quest.Quests)
                    {
                        sb.AppendLine($"{pair.Key.____GetDebugShortName(),40} : {pair.Value.PhaseIndex} - {pair.Value.PhaseSuccCounter?.CounterDef.____GetDebugShortName(),40} | {pair.Value.PhaseFailCounter?.CounterDef.____GetDebugShortName(),40}");
                    }
                    sb.AppendLine("---------------------------------");
                }

                Logger.IfInfo()?.Message(sb.ToString()).Write();
            });
        }

        [Cheat]
        public static async Task AddQuest([NotNull] QuestDef quest)
        {
            if (GameState.Instance.ClientClusterNode.AssertIfNull(nameof(GameState.Instance.ClientClusterNode)))
                return;

            await AsyncUtils.RunAsyncTask(async () =>
            {
                var characterId = GameState.Instance.CharacterRuntimeData.CharacterId;
                if (Guid.Empty == characterId)
                {
                    UI.Logger.IfError()?.Message($"{nameof(characterId)} is empty").Write();
                    return;
                }

                using (var cheatServiceWrapper = await GameState.Instance.ClientClusterNode.Get<ICheatServiceEntityServer>(
                    GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId))
                {
                    if (cheatServiceWrapper.AssertIfNull(nameof(cheatServiceWrapper)))
                        return;

                    var cheatServiceEntity = cheatServiceWrapper
                        .Get<ICheatServiceEntityServer>(GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId);

                    if (cheatServiceEntity.AssertIfNull(nameof(cheatServiceEntity)))
                        return;

                    await cheatServiceEntity.AddQuest(quest, characterId);
                    Logger.IfInfo()?.Message($"quest '{quest}' added").Write();
                }
            });
        }

        [Cheat]
        public static async Task RemoveQuest([NotNull] QuestDef quest)
        {
            if (GameState.Instance.ClientClusterNode.AssertIfNull(nameof(GameState.Instance.ClientClusterNode)))
                return;

            var characterId = GameState.Instance.CharacterRuntimeData.CharacterId;
            await AsyncUtils.RunAsyncTask(async () =>
            {
                using (var wrapper = await GameState.Instance.ClientClusterNode.Get<IWorldCharacterClientFull>(characterId))
                    await wrapper.Get<IWorldCharacterClientFull>(characterId).Quest.RemoveQuest(quest);

                Logger.IfInfo()?.Message($"quest '{quest}' removed").Write();
            });
        }

        [Cheat]
        public static async Task SetContainerSize(int size)
        {
            await AsyncUtils.RunAsyncTask(() => InventorySetSize(size, false, InventoryAddressResolver));
        }

        [Cheat]
        public static async Task ChangeContainerSize(int deltaSize)
        {
            await AsyncUtils.RunAsyncTask(() => InventorySetSize(deltaSize, true, InventoryAddressResolver));
        }

        private static async Task InventorySetSize(int sizeOrDelta, bool isDelta, AddressResolver propertyAddressResolver)
        {
            var characterId = GameState.Instance.CharacterRuntimeData.CharacterId;
            using (var wrapper = await GameState.Instance.ClientClusterNode.Get<IWorldCharacterClientFull>(characterId))
            {
                var worldCharacter = wrapper.Get<IWorldCharacterClientFull>(characterId);
                var propertyAddress = await propertyAddressResolver(characterId, typeof(IWorldCharacter));
                var newSize = sizeOrDelta;
                if (isDelta)
                {
                    var container = EntityPropertyResolver.Resolve<IItemsContainerClientFull>(worldCharacter, propertyAddress);
                    newSize += container.Size;
                }

                if (newSize < 0)
                {
                    Logger.Error($"{nameof(InventorySetSize)}({nameof(sizeOrDelta)}={sizeOrDelta}, " +
                                    $"{nameof(isDelta)}{isDelta.AsSign()}) {nameof(newSize)}={newSize} is negative");
                    return;
                }

                await worldCharacter.ContainerApi.ContainerOperationSetSize(propertyAddress, newSize);

                {
                    var container = EntityPropertyResolver.Resolve<IItemsContainerClientFull>(worldCharacter, propertyAddress);
                    if(container.Size != newSize)
                    {
                         Logger.IfError()?.Message("Failed to set container size to {0}",  newSize).Write();
                    }
                }
            }
        }

        [Cheat]
        public static async Task AddTechsAsync()
        {
            string[] lines =
            {
                "AddTech /Inventory/Technology/Survival_Level1"
            };
            await Batch(lines);
        }

        [Cheat]
        public static async Task AddKnow([NotNull] KnowledgeDef knowledge)
        {
            if (GameState.Instance.ClientClusterNode.AssertIfNull(nameof(GameState.Instance.ClientClusterNode)))
                return;

            await AsyncUtils.RunAsyncTask(async () =>
            {
                var characterId = GameState.Instance.CharacterRuntimeData.CharacterId;
                using (var wrapper = await GameState.Instance.ClientClusterNode.Get<IWorldCharacterClientFull>(characterId))
                {
                    var knowledgeEngine =
                        ((IKnowledgeEngine)((IEntityRefExt)wrapper.Get<IWorldCharacterClientFull>(characterId).KnowledgeEngine).GetEntity());
                    await knowledgeEngine.AddKnowledge(knowledge);
                }

                Logger.IfInfo()?.Message($"Adding knowledge {knowledge}").Write();
            });
        }

        public static async Task Tech()
        {
            if (GameState.Instance.ClientClusterNode.AssertIfNull(nameof(GameState.Instance.ClientClusterNode)))
                return;

            await AsyncUtils.RunAsyncTask(async () =>
            {
                var characterId = GameState.Instance.CharacterRuntimeData.CharacterId;
                var sb = new StringBuilder();
                using (var wrapper = await GameState.Instance.ClientClusterNode.Get<IWorldCharacterClientFull>(characterId))
                {
                    var worldCharacter = wrapper.Get<IWorldCharacterClientFull>(characterId);
                    var knownTechnologies = ((IKnowledgeEngine)((IEntityRefExt)worldCharacter.KnowledgeEngine).GetEntity()).KnownTechnologies;
                    sb.AppendLine("---- Technologies ----");
                    foreach (var technologyDef in knownTechnologies)
                        sb.AppendLine($"{technologyDef.____GetDebugRootName()}");

                    sb.AppendLine("---------------------------------");
                }

                Logger.IfInfo()?.Message(sb.ToString()).Write();
            });
        }

        [Cheat]
        public static async Task Spell(string pathToSpell, string parameters = null, Guid entityId = default, string typeName = "IWorldCharacter")
        {
            OuterRef<IEntity> entityRef = new OuterRef<IEntity>(entityId, ReplicaTypeRegistry.GetIdByType(ReplicaTypeRegistry.GetTypeByName(typeName)));
            
            if (entityRef.Guid == Guid.Empty)
                entityRef = EntityInDebugFocus.EntityRef.To<IEntity>();

            if (entityRef.Guid == Guid.Empty)
                entityRef = new OuterRef<IEntity>(GameState.Instance.CharacterRuntimeData.CharacterId, ReplicaTypeRegistry.GetIdByType(typeof(IWorldCharacter)));

            if (entityRef.Guid == Guid.Empty)
                return;
            
            var repository = GameState.Instance.ClientClusterNode;

            if (!pathToSpell.StartsWith("/"))
                pathToSpell = "/UtilPrefabs/Spells/" + pathToSpell;
            
            var @params = parameters?.Split(new[] {':'}, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Split('=')).Select(x => (Name : x[0].ToLower(), Value: x[1]));
            
            await AsyncUtils.RunAsyncTask(async () =>
            {
                var cheatServiceId = GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId;
                using (var cheatServiceWrapper = await repository.Get<ICheatServiceEntityClientBroadcast>(cheatServiceId))
                {
                    var cheatServiceEntity = cheatServiceWrapper?.Get<ICheatServiceEntityServer>(cheatServiceId);
                    if (cheatServiceEntity.AssertIfNull(nameof(cheatServiceEntity)))
                        return;
                    SpellDef spellDef = GameResourcesHolder.Instance.LoadResource<SpellDef>(pathToSpell);
                    var castBuilder = new SpellCastBuilder().SetSpell(spellDef);
                    if (@params != null)
                        foreach (var tuple in @params)
                        {
                            switch (tuple.Name)
                            {
                                case "duration": castBuilder.SetDuration(Convert.ToSingle(tuple.Value)); break;
                                case "direction3": castBuilder.SetDirection3(SharedCode.Utils.Vector3.Parse(tuple.Value)); break;
                                case "direction2": castBuilder.SetDirection2(SharedCode.Utils.Vector2.Parse(tuple.Value)); break;
                                case "position3": castBuilder.SetPosition3(SharedCode.Utils.Vector3.Parse(tuple.Value)); break;
                                case "target": castBuilder.SetTarget(new OuterRef(Guid.Parse(tuple.Value), ReplicaTypeRegistry.GetIdByType(typeof(IWorldCharacter)))); break;
                                default: throw new NotSupportedException(tuple.Name);
                            }
                        }
                    await cheatServiceEntity.CastSpell(entityRef, castBuilder.Build());
                }
            });
        }

        [Cheat]
        public static async Task DumpCurrentUnityServerRepository(string fileName)
        {
            var repo = GameState.Instance.ClientClusterNode;
            var repoId = GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId;
            await AsyncUtils.RunAsyncTask(async () =>
            {
                using (var wrapper = await repo.Get<IRepositoryCommunicationEntityServer>(repoId))
                {
                    await wrapper.Get<IRepositoryCommunicationEntityServer>(repoId).Dump(fileName);
                }
            });
        }

        [Cheat]
        public static async Task DumpAllConnectedRepositories()
        {
            var repo = GameState.Instance.ClientClusterNode;
            await AsyncUtils.RunAsyncTask(async () =>
            {
                var batch = EntityBatch.Create();
                foreach (var pair in ((EntitiesRepository)repo).GetEntitiesCollection(typeof(IRepositoryCommunicationEntity)))
                    batch.Add(pair.Value.TypeId, pair.Value.Id);

                using (var wrapper = await repo.Get(batch))
                {
                    foreach (var pair in ((EntitiesRepository)repo).GetEntitiesCollection(typeof(IRepositoryCommunicationEntity)))
                    {
                        var entity = ((IRepositoryCommunicationEntity)((IEntityRefExt)pair.Value).GetEntity());
                        await entity.Dump("Repository" + entity.Config.ConfigId);
                    }
                }
            });
        }

        [Cheat]
        public static async Task DumpAllServerRepositories()
        {
            IEntitiesRepository repo = GameState.Instance.ClientClusterNode;
            if (repo.AssertIfNull(nameof(repo)))
                return;

            var remoteRepoId = GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId;
            await AsyncUtils.RunAsyncTask(async () =>
            {
                using (var cheatServiceWrapper = await repo.Get<ICheatServiceEntityServer>(remoteRepoId))
                {
                    var serviceEntity = cheatServiceWrapper.Get<ICheatServiceEntityServer>(remoteRepoId);
                    await serviceEntity.DumpAllServerRepositories();
                }

            });
        }

        [Cheat]
        public static async Task ForceGC(int count, string guid = null)
        {
            IEntitiesRepository repo = GameState.Instance.ClientClusterNode;
            if (repo.AssertIfNull(nameof(repo)))
                return;

            var parsedGuid = Guid.Empty;
            if (!string.IsNullOrEmpty(guid))
                Guid.TryParse(guid, out parsedGuid);

            var remoteRepoId = GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId;
            await AsyncUtils.RunAsyncTask(async () =>
            {
                using (var cheatServiceWrapper = await repo.Get<ICheatServiceEntityServer>(remoteRepoId))
                {
                    var serviceEntity = cheatServiceWrapper.Get<ICheatServiceEntityServer>(remoteRepoId);
                    await serviceEntity.ForceGC(count, parsedGuid);
                }

            });
        }

        [Cheat]
        public static void SetGCEnabledOnClient(bool enabled)
        {
             Logger.IfInfo()?.Message("Set gc enabled {0} on client",  enabled).Write();
            if (enabled)
                GarbageCollector.GCMode = GarbageCollector.Mode.Enabled;
            else
                GarbageCollector.GCMode = GarbageCollector.Mode.Disabled;
        }

        [Cheat]
        public static void GetGCEnabledOnClient()
        {
            if (GarbageCollector.GCMode == GarbageCollector.Mode.Enabled)
                 Logger.IfInfo()?.Message("GC is enabled on client").Write();
            else
                 Logger.IfInfo()?.Message("GC is disabled on client").Write();
        }

        [Cheat]
        public static async Task SetGCEnabledOnUnityServer(bool enabled, string guid = null)
        {
            IEntitiesRepository repo = GameState.Instance.ClientClusterNode;
            if (repo.AssertIfNull(nameof(repo)))
                return;

            var parsedGuid = Guid.Empty;
            if (!string.IsNullOrEmpty(guid))
                Guid.TryParse(guid, out parsedGuid);

            var remoteRepoId = GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId;
            await AsyncUtils.RunAsyncTask(async () =>
            {
                using (var cheatServiceWrapper = await repo.Get<ICheatServiceEntityServer>(remoteRepoId))
                {
                    var serviceEntity = cheatServiceWrapper.Get<ICheatServiceEntityServer>(remoteRepoId);
                    await serviceEntity.SetGCEnabled(enabled, parsedGuid);
                }

            });
        }

        [Cheat]
        public static async Task DumpMyRepository(string fileName)
        {
            var repo = GameState.Instance.ClientClusterNode;
            await AsyncUtils.RunAsyncTask(async () =>
            {
                using (var wrapper = await GameState.Instance.ClientClusterNode.Get<IRepositoryCommunicationEntityServer>(repo.Id))
                {
                    await wrapper.Get<IRepositoryCommunicationEntityServer>(repo.Id).Dump(fileName);
                }
            });
        }

        [Cheat]
        public static async Task DumpMyCharacterOnServer(string fileName)
        {
            var repo = GameState.Instance.ClientClusterNode;
            var remoteRepoId = GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId;
            await AsyncUtils.RunAsyncTask(async () =>
            {
                using (var wrapper = await GameState.Instance.ClientClusterNode.Get<IRepositoryCommunicationEntityServer>(remoteRepoId))
                {
                    var entity = wrapper.Get<IRepositoryCommunicationEntityServer>(remoteRepoId);
                    await entity.DumpEntity(ReplicaTypeRegistry.GetIdByType(typeof(IWorldCharacter)), GameState.Instance.CharacterRuntimeData.CharacterId, fileName);
                }
            });
        }

        [Cheat]
        public static async Task DumpMyCharacterOnClient(string fileName)
        {
            var repo = GameState.Instance.ClientClusterNode;
            await AsyncUtils.RunAsyncTask(async () =>
            {
                using (var wrapper = await GameState.Instance.ClientClusterNode.Get<IRepositoryCommunicationEntityServer>(repo.Id))
                {
                    await wrapper.Get<IRepositoryCommunicationEntityServer>(repo.Id).DumpEntity(ReplicaTypeRegistry.GetIdByType(typeof(IWorldCharacter)), GameState.Instance.CharacterRuntimeData.CharacterId, fileName);
                }
            });
        }

        [Cheat]
        public static async Task DumpEntitySerializedData(string fileName, int entityTypeId, string entityId, long replicationMask)
        {
            var repo = GameState.Instance.ClientClusterNode;
            var repoId = GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId;
            await AsyncUtils.RunAsyncTask(async () =>
            {
                using (var wrapper = await repo.Get<IRepositoryCommunicationEntityServer>(repoId))
                {
                    await wrapper.Get<IRepositoryCommunicationEntityServer>(repoId).DumpEntitySerializedData(fileName, entityTypeId, Guid.Parse(entityId), replicationMask);
                }
            });
        }

        [Cheat]
        public static async Task DumpCharacterSerializedDataClientFull(string fileName, string entityId)
        {
            await DumpEntitySerializedData(fileName, ReplicaTypeRegistry.GetIdByType(typeof(IWorldCharacter)), entityId,
                (long)ReplicationLevel.ClientFull);
        }

        [Cheat]
        public static async Task DumpMyCharacterSerializedDataClientBroadcast(string fileName)
        {
            await DumpEntitySerializedData(fileName, ReplicaTypeRegistry.GetIdByType(typeof(IWorldCharacter)),
                GameState.Instance.CharacterRuntimeData.CharacterId.ToString(), (long)ReplicationLevel.ClientBroadcast);
        }

        [Cheat]
        public static async Task DumpMyCharacterSerializedDataClientFull(string fileName)
        {
            await DumpEntitySerializedData(fileName, ReplicaTypeRegistry.GetIdByType(typeof(IWorldCharacter)),
                GameState.Instance.CharacterRuntimeData.CharacterId.ToString(), (long)ReplicationLevel.ClientFull);
        }

        [Cheat]
        public static async Task DumpMyCharacterSerializedDataMaster(string fileName)
        {
            await DumpEntitySerializedData(fileName, ReplicaTypeRegistry.GetIdByType(typeof(IWorldCharacter)),
                GameState.Instance.CharacterRuntimeData.CharacterId.ToString(), (long)ReplicationLevel.Master);
        }

        [Cheat]
        public static async Task SpawnInteractiveObjectEntity(string entityDefPath, float posX, float posY, float posZ)
        {
            IEntitiesRepository repo = GameState.Instance.ClientClusterNode;
            if (repo.AssertIfNull(nameof(repo)))
                return;

            if (entityDefPath.IsNullOrWhitespace())
                return;

            await AsyncUtils.RunAsyncTask(async () =>
            {
                var entityDef = GameResourcesHolder.Instance.LoadResource<InteractiveEntityDef>(entityDefPath);
                using (var cheatServiceWrapper = await repo.Get<ICheatServiceEntityServer>(GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId))
                {
                    var serviceEntity = cheatServiceWrapper.Get<ICheatServiceEntityServer>(GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId);
                    await serviceEntity.SpawnInteractiveObjectEntity(entityDef, new SharedVector3(posX, posY, posZ));
                }
            });
        }

        [Cheat]
        public static async Task SpawnNewMineableEntity(string entityDefPath, float posX, float posY, float posZ)
        {
            IEntitiesRepository repo = GameState.Instance.ClientClusterNode;
            if (repo.AssertIfNull(nameof(repo)))
                return;

            if (entityDefPath.IsNullOrWhitespace())
                return;

            await AsyncUtils.RunAsyncTask(async () =>
            {
                var entityDef = GameResourcesHolder.Instance.LoadResource<MineableEntityDef>(entityDefPath);
                using (var cheatServiceWrapper = await repo.Get<ICheatServiceEntityServer>(GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId))
                {
                    var serviceEntity = cheatServiceWrapper.Get<ICheatServiceEntityServer>(GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId);
                    await serviceEntity.SpawnNewMineableEntity(entityDef, new SharedVector3(posX, posY, posZ));
                }
            });
        }

        [Cheat]
        public static async Task SpawnInteractiveEntity(string entityDefPath, float posX, float posY, float posZ)
        {
            IEntitiesRepository repo = GameState.Instance.ClientClusterNode;
            if (repo.AssertIfNull(nameof(repo)))
                return;

            if (entityDefPath.IsNullOrWhitespace())
                return;

            var entityDef = GameResourcesHolder.Instance.LoadResource<InteractiveEntityDef>(entityDefPath);
            using (var cheatServiceWrapper = await repo.Get<ICheatServiceEntityServer>(GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId))
            {
                var serviceEntity = cheatServiceWrapper.Get<ICheatServiceEntityServer>(GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId);
                await serviceEntity.SpawnInteractiveEntity(entityDef, new SharedVector3(posX, posY, posZ));
            }
        }

        [Cheat]
        public static async Task GetEntitiesCountOnServer()
        {
            IEntitiesRepository repo = GameState.Instance.ClientClusterNode;
            if (repo.AssertIfNull(nameof(repo)))
                return;

            var remoteRepoId = GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId;
            await AsyncUtils.RunAsyncTask(async () =>
            {
                using (var cheatServiceWrapper = await repo.Get<ICheatServiceEntityServer>(remoteRepoId))
                {
                    var serviceEntity = cheatServiceWrapper.Get<ICheatServiceEntityServer>(remoteRepoId);
                    var str = await serviceEntity.GetRepositoryEntitiesCount();
                     Logger.IfError()?.Message("Server entities count (repo {0}): \n {1}",  remoteRepoId, str).Write();
                }

            });
        }

        [Cheat]
        public static async Task GetEntitiesCountOnAllServerRepositories()
        {
            IEntitiesRepository repo = GameState.Instance.ClientClusterNode;
            if (repo.AssertIfNull(nameof(repo)))
                return;

            var remoteRepoId = GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId;
            await AsyncUtils.RunAsyncTask(async () =>
            {
                using (var cheatServiceWrapper = await repo.Get<ICheatServiceEntityServer>(remoteRepoId))
                {
                    var serviceEntity = cheatServiceWrapper.Get<ICheatServiceEntityServer>(remoteRepoId);
                    var str = await serviceEntity.GetRepositoryEntitiesCountOnAllRepositories();
                     Logger.IfError()?.Message("All repositories entities count: \n {0}",  str).Write();
                }

            });
        }

        [Cheat]
        public static void GetEntitiesCountOnClient()
        {
            IEntitiesRepository repo = GameState.Instance.ClientClusterNode;
            if (repo.AssertIfNull(nameof(repo)))
                return;

            var str = ((EntitiesRepository)repo).GetObjectsCount();
             Logger.IfError()?.Message("Client entities count (repo {0}): \n {1}",  repo.Id, str).Write();
        }

        [Cheat]
        public static async Task SetObjectsVisibilityRadius(float enterVisibilityRadius, float leaveVisibilityRadius)
        {
            IEntitiesRepository repo = GameState.Instance.ClientClusterNode;
            if (repo.AssertIfNull(nameof(repo)))
                return;
            await AsyncUtils.RunAsyncTask(async () =>
            {
                using (var cheatServiceWrapper = await repo.Get<ICheatServiceEntityServer>(GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId))
                {
                    var serviceEntity = cheatServiceWrapper.Get<ICheatServiceEntityServer>(GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId);
                    var result = await serviceEntity.SetVisibilityRadius(enterVisibilityRadius, leaveVisibilityRadius);
                    Logger.IfInfo()?.Message(result).Write();
                }
            });
        }

        [Cheat]
        public static async Task GetEntityWaitLongQueuesOnServer()
        {
            IEntitiesRepository repo = GameState.Instance.ClientClusterNode;
            if (repo.AssertIfNull(nameof(repo)))
                return;

            var remoteRepoId = GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId;
            await AsyncUtils.RunAsyncTask(async () =>
            {
                using (var cheatServiceWrapper = await repo.Get<ICheatServiceEntityServer>(remoteRepoId))
                {
                    var serviceEntity = cheatServiceWrapper.Get<ICheatServiceEntityServer>(remoteRepoId);
                    var str = await serviceEntity.GetTooLongEntityWaitQueues();
                    Logger.IfError()?.Message(str).Write();
                }

            });
        }

        [Cheat]
        public static void GetEntityWaitLongQueuesOnClient()
        {
            IEntitiesRepository repo = GameState.Instance.ClientClusterNode;
            if (repo.AssertIfNull(nameof(repo)))
                return;

            bool first = true;
            var sb = StringBuildersPool.Get;
            sb.AppendLine("Long queues:");

            void VisitQueueLength(Type type, Guid id, int length)
            {
                if (length > 10)
                {
                    if (first)
                    {
                        first = false;
                        sb.AppendFormat("Type {0}:", type.GetFriendlyName()).AppendLine();
                    }
                    sb.AppendFormat("Id {0} countOverall {1} count {2}", id, length).AppendLine();
                }
            }

            ((IEntitiesRepositoryExtension)repo).VisitEntityQueueLengths(VisitQueueLength);

            Logger.IfError()?.Message(sb.ToStringAndReturn()).Write();
        }

        [Cheat]
        public static async Task GetEntityStatusWithRepoId(string entityType)
        {
            await GetEntityStatus(entityType, string.Empty);
        }

        [Cheat]
        public static async Task GetEntityStatus(string entityType, string entityId)
        {
            var entityTypeId = 0;
            if (!int.TryParse(entityType, out entityTypeId))
                entityTypeId = ReplicaTypeRegistry.GetIdByType(ReplicaTypeRegistry.GetTypeByName(entityType));

            if (entityTypeId == 0)
            {
                 Logger.IfError()?.Message("unknown type").Write();;
                return;
            }

            var remoteRepoId = GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId;
            var entityGuid = Guid.Empty;
            if (!Guid.TryParse(entityId, out entityGuid))
            {
                 Logger.IfError()?.Message("cant parse guid, using repository id").Write();;
                entityGuid = remoteRepoId;
            }

            await getEntityStatusInternal(remoteRepoId, entityTypeId, entityGuid);
        }

        [Cheat]
        public static async Task GetAllServiceEntitiesStatus()
        {
            var remoteRepoId = GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId;

            IEntitiesRepository repo = GameState.Instance.ClientClusterNode;
            if (repo.AssertIfNull(nameof(repo)))
                return;

            await AsyncUtils.RunAsyncTask(async () =>
            {
                using (var cheatServiceWrapper = await repo.Get<IRepositoryCommunicationEntityServer>(remoteRepoId))
                {
                    var serviceEntity = cheatServiceWrapper.Get<IRepositoryCommunicationEntityServer>(remoteRepoId);
                    var str = await serviceEntity.GetAllServiceEntityStatus();
                    Logger.IfError()?.Message(str).Write();
                }
            });
        }

        [Cheat]
        public static async Task GetAllServiceEntitiesStatusLocal()
        {
            IEntitiesRepository repo = GameState.Instance.ClientClusterNode;
            if (repo.AssertIfNull(nameof(repo)))
                return;

            await AsyncUtils.RunAsyncTask(() =>
            {
                var str = ((IEntitiesRepositoryExtension)repo).GetAllServiceEntitiesOperationLog();
                Logger.IfError()?.Message(str).Write();
            });
        }

        [Cheat]
        public static async Task GetEntityStatusServerSide(string entityType, string entityId)
        {
            var repo = NodeAccessor.Repository;
            if (repo.AssertIfNull(nameof(repo)))
                return;

            var entityTypeId = 0;
            if (!int.TryParse(entityType, out entityTypeId))
                entityTypeId = ReplicaTypeRegistry.GetIdByType(ReplicaTypeRegistry.GetTypeByName(entityType));

            if (entityTypeId == 0)
            {
                 Logger.IfError()?.Message("unknown type").Write();;
                return;
            }

            var entityGuid = Guid.Empty;
            if (!Guid.TryParse(entityId, out entityGuid))
            {
                 Logger.IfError()?.Message("cant parse guid, using repository id").Write();;
                entityGuid = repo.Id;
            }

            await AsyncUtils.RunAsyncTask(() =>
            {
                var str = ((IEntitiesRepositoryExtension)repo).GetEntityStatus(entityTypeId, entityGuid);
                Logger.IfError()?.Message(str).Write();
            });
        }

        [Cheat]
        public static async Task GetAllServiceEntitiesStatusServerSide()
        {
            var repo = NodeAccessor.Repository;
            if (repo.AssertIfNull(nameof(repo)))
                return;
            await AsyncUtils.RunAsyncTask(() =>
            {
                var str = ((IEntitiesRepositoryExtension)repo).GetAllServiceEntitiesOperationLog();
                Logger.IfError()?.Message(str).Write();
            });
        }

        [Cheat]
        public static async Task GetMyCharacterEntityStatus()
        {
            await getEntityStatusInternal(GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId,
                ReplicaTypeRegistry.GetIdByType(typeof(IWorldCharacter)), GameState.Instance.CharacterRuntimeData.CharacterId);
        }

        [Cheat]
        public static async Task GetMyCharacterEntityStatusLocal()
        {
            await getEntityStatusInternal(GameState.Instance.ClientClusterNode.Id,
                ReplicaTypeRegistry.GetIdByType(typeof(IWorldCharacter)), GameState.Instance.CharacterRuntimeData.CharacterId);
        }

        [Cheat]
        public static async Task GetMyWizardEntityStatus()
        {
            IEntitiesRepository repo = GameState.Instance.ClientClusterNode;
            if (repo.AssertIfNull(nameof(repo)))
                return;

            var characterId = GameState.Instance.CharacterRuntimeData.CharacterId;
            if (Guid.Empty == characterId)
            {
                UI.Logger.IfError()?.Message($"{nameof(characterId)} is empty").Write();
                return;
            }

            await AsyncUtils.RunAsyncTask(async () =>
            {
                using (var wrapper = await repo.Get<IWorldCharacterClientFull>(characterId))
                {
                    var characterEntity = wrapper.Get<IWorldCharacterClientFull>(characterId);
                    if (characterEntity == null)
                    {
                         Logger.IfError()?.Message("WorldCharacter not found {0}",  characterId).Write();
                        return;
                    }
                    await getEntityStatusInternal(GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId,
                        ReplicaTypeRegistry.GetIdByType(typeof(IWizardEntity)), characterEntity.Wizard.Id);
                }

            });
        }

        [Cheat]
        public static async Task GetMyWizardEntityStatusLocal()
        {
            IEntitiesRepository repo = GameState.Instance.ClientClusterNode;
            if (repo.AssertIfNull(nameof(repo)))
                return;

            var characterId = GameState.Instance.CharacterRuntimeData.CharacterId;
            if (Guid.Empty == characterId)
            {
                UI.Logger.IfError()?.Message($"{nameof(characterId)} is empty").Write();
                return;
            }

            await AsyncUtils.RunAsyncTask(async () =>
            {
                using (var wrapper = await repo.Get<IWorldCharacterClientFull>(characterId))
                {
                    var characterEntity = wrapper.Get<IWorldCharacterClientFull>(characterId);
                    if (characterEntity == null)
                    {
                         Logger.IfError()?.Message("WorldCharacter not found {0}",  characterId).Write();
                        return;
                    }
                    await getEntityStatusInternal(repo.Id,
                        ReplicaTypeRegistry.GetIdByType(typeof(IWizardEntity)), characterEntity.Wizard.Id);
                }
            });
        }

        private static async Task getEntityStatusInternal(Guid remoteRepoId, int entityTypeId, Guid entityGuid)
        {
            IEntitiesRepository repo = GameState.Instance.ClientClusterNode;
            if (repo.AssertIfNull(nameof(repo)))
                return;

            await AsyncUtils.RunAsyncTask(async () =>
            {
                using (var cheatServiceWrapper = await repo.Get<IRepositoryCommunicationEntityServer>(remoteRepoId))
                {
                    var serviceEntity = cheatServiceWrapper.Get<IRepositoryCommunicationEntityServer>(remoteRepoId);
                    var str = await serviceEntity.GetEntityStatus(entityTypeId, entityGuid);
                    Logger.IfError()?.Message(str).Write();
                }
            });
        }

        [Cheat]
        public static async Task ResetSpawnDaemon(string daemonName)
        {
            {///#Dbg:
                var dic = SpawnDaemonCatalogue.DaemonNameToGuidDic;
                var keys = String.Join(", ", dic.Keys.ToList());
                var vals = String.Join(", ", dic.Values.ToList());
                Logger.IfDebug()?.Message($"ResetSpawnDaemon({daemonName}). SpawnDaemonCatalogue.DaemonNameToGuidDic: N=={dic.Count}.   Keys: {keys}.   Values: {vals}.").Write();
            }

            if (daemonName.IsNullOrWhitespace())
            {
                Logger.IfError()?.Message($"Wrong daemon name \"{daemonName}\" passed.").Write();
                return;
            }

            Guid spawnDaemonId;
            if (!SpawnDaemonCatalogue.DaemonNameToGuidDic.TryGetValue(daemonName, out spawnDaemonId))
            {
                Logger.IfError()?.Message($"{nameof(SpawnDaemonCatalogue)} doesn't contain key \"{daemonName}\".").Write();
                return;
            }

            Logger.IfDebug()?.Message($"ResetSpawnDaemon. GotDaemonId == {spawnDaemonId}.").Write();

            var repo = NodeAccessor.Repository;
            await AsyncUtils.RunAsyncTask(async () =>
            {
                using (var wrapper = await repo.Get<ISpawnDaemonServer>(spawnDaemonId))
                {
                    var daemon = wrapper?.Get<ISpawnDaemonServer>(spawnDaemonId);
                    if (daemon == null)
                    {
                        Logger.IfError()?.Message($"Can't get {nameof(ISpawnDaemonServer)} by ___ (id: {spawnDaemonId}).").Write();
                        return;
                    }

                    await daemon.ResetDaemon();
                }
            });
        }


        [Cheat]
        public static async Task KillAllAround(int r)
        {
            if (r < 0)
            {
                Logger.IfError()?.Message("Wrong radius passed: " + r).Write();
                return;
            }

            var repo = GameState.Instance.ClientClusterNode;
            if (repo.AssertIfNull(nameof(repo)))
                return;

            var characterId = GameState.Instance.CharacterRuntimeData.CharacterId;
            if (characterId == Guid.Empty)
            {
                UI.Logger.IfError()?.Message($"{nameof(characterId)} is empty").Write();
                return;
            }

            // Chose spell:
            SpellDef spellDef;
            //TODO: this shit-design is agreed, while we haven't realization of defs templates
            if (r <= 20)
                spellDef = GlobalConstsHolder.CheatsData.KillAllObjectsAround_20Spell;
            else if (r <= 50)
                spellDef = GlobalConstsHolder.CheatsData.KillAllObjectsAround_50Spell;
            else
                spellDef = GlobalConstsHolder.CheatsData.KillAllObjectsAround_100Spell;

            await AsyncUtils.RunAsyncTask(async () =>
            {
                using (var wrapper = await repo.Get<IWorldCharacter>(characterId))
                {
                    var typeId = ReplicaTypeRegistry.GetIdByType(typeof(IWorldCharacter));
                    var hasWizard = wrapper?.Get<IHasWizardEntityClientFull>(typeId, characterId, ReplicationLevel.ClientFull);
                    if (hasWizard == null)
                    {
                        Logger.IfError()?.Message($"Can't get `{nameof(IHasWizardEntityClientFull)}`").Write();
                        return;
                    }

                    using (var wizardWrapper = await repo.Get(hasWizard.Wizard.TypeId, hasWizard.Wizard.Id))
                    {
                        var wizard = wizardWrapper?.Get<IWizardEntityClientFull>(hasWizard.Wizard.TypeId, hasWizard.Wizard.Id, ReplicationLevel.ClientFull);
                        if (wizard == null)
                        {
                            Logger.IfError()?.Message($"Can't get `{nameof(IWizardEntityClientFull)}`").Write();
                            return;
                        }

                        await wizard.CastSpell(new SpellCast { Def = spellDef, StartAt = SyncTime.Now });
                    }
                }
            });
        }

        #region Entities ping cheats
        [Cheat]
        public static async Task PingLocalUnityThread()
        {
            IEntitiesRepository repo = GameState.Instance.ClientClusterNode;
            if (repo.AssertIfNull(nameof(repo)))
                return;

            await AsyncUtils.RunAsyncTask(async () =>
            {
                var sb = new StringBuilder();
                var sw = new Stopwatch();
                sw.Start();
                sb.AppendLine("PingLocalUnityThread");
                var result = await UnityQueueHelper.RunInUnityThread(() => { return true; });
                sw.Stop();
                sb.AppendFormat("ping: {0} msec", sw.Elapsed.TotalMilliseconds).AppendLine();
                Logger.IfError()?.Message(sb.ToString()).Write();

            });
        }

        [Cheat]
        public static async Task PingLocalUnityThreadServerSide()
        {
            IEntitiesRepository repo = NodeAccessor.Repository;
            if (repo.AssertIfNull(nameof(repo)))
                return;

            await AsyncUtils.RunAsyncTask(async () =>
            {
                var sb = new StringBuilder();
                var sw = new Stopwatch();
                sw.Start();
                sb.AppendLine("PingLocalUnityThreadServerSide");
                var result = await UnityQueueHelper.RunInUnityThread(() => { return true; });
                sw.Stop();
                sb.AppendFormat("ping: {0} msec", sw.Elapsed.TotalMilliseconds).AppendLine();
                Logger.IfError()?.Message(sb.ToString()).Write();

            });
        }

        [Cheat]
        public static async Task PingLocalUnityRepositoryEntityServerSide()
        {
            IEntitiesRepository repo = NodeAccessor.Repository;
            if (repo.AssertIfNull(nameof(repo)))
                return;

            var remoteRepoId = repo.Id;
            await AsyncUtils.RunAsyncTask(async () =>
            {
                var sb = new StringBuilder();
                var sw = new Stopwatch();
                sw.Start();
                sb.AppendLine("PingLocalUnityRepositoryEntityServerSide");
                using (var wrapper = await repo.Get<IRepositoryCommunicationEntityClientFull>(remoteRepoId))
                {
                    var entity = wrapper.Get<IRepositoryCommunicationEntityClientFull>(remoteRepoId);
                    sw.Stop();
                    sb.AppendFormat("get: {0} msec", sw.ElapsedMilliseconds).AppendLine();
                    sw.Restart();
                    var result = await entity.PingDiagnostics.PingLocal();
                    sw.Stop();
                    if (result)
                        sb.AppendFormat("ping: {0} msec", sw.ElapsedMilliseconds).AppendLine();
                }
                Logger.IfError()?.Message(sb.ToString()).Write();

            });
        }

        [Cheat]
        public static async Task PingWriteEntityServerSide(string entityType, string entityId)
        {
            IEntitiesRepository repo = NodeAccessor.Repository;
            if (repo.AssertIfNull(nameof(repo)))
                return;

            var entityTypeId = 0;
            if (!int.TryParse(entityType, out entityTypeId))
                entityTypeId = ReplicaTypeRegistry.GetIdByType(ReplicaTypeRegistry.GetTypeByName(entityType));

            if (entityTypeId == 0)
            {
                 Logger.IfError()?.Message("unknown type").Write();;
                return;
            }

            var entityGuid = Guid.Empty;
            if (!Guid.TryParse(entityId, out entityGuid))
            {
                 Logger.IfError()?.Message("cant parse guid").Write();;
                return;
            }

            await AsyncUtils.RunAsyncTask(async () =>
            {
                var sb = new StringBuilder();
                var sw = new Stopwatch();
                sw.Start();
                sb.AppendLine("PingWriteEntityServerSide");
                using (var wrapper = await repo.Get(entityTypeId, entityGuid))
                {
                    var entity = wrapper.Get<IHasPingDiagnosticsServer>(entityTypeId, entityGuid, ReplicationLevel.Server);
                    sw.Stop();
                    sb.AppendFormat("get: {0} msec", sw.ElapsedMilliseconds).AppendLine();
                    sw.Restart();
                    var result = await entity.PingDiagnostics.PingWrite();
                    sw.Stop();
                    if (result)
                        sb.AppendFormat("ping: {0} msec", sw.ElapsedMilliseconds).AppendLine();
                }
                Logger.IfError()?.Message(sb.ToString()).Write();

            });
        }

        [Cheat]
        public static async Task PingReadEntityServerSide(string entityType, string entityId)
        {
            IEntitiesRepository repo = NodeAccessor.Repository;
            if (repo.AssertIfNull(nameof(repo)))
                return;

            var entityTypeId = 0;
            if (!int.TryParse(entityType, out entityTypeId))
                entityTypeId = ReplicaTypeRegistry.GetIdByType(ReplicaTypeRegistry.GetTypeByName(entityType));

            if (entityTypeId == 0)
            {
                 Logger.IfError()?.Message("unknown type").Write();;
                return;
            }

            var entityGuid = Guid.Empty;
            if (!Guid.TryParse(entityId, out entityGuid))
            {
                 Logger.IfError()?.Message("cant parse guid").Write();;
                return;
            }

            await AsyncUtils.RunAsyncTask(async () =>
            {
                var sb = new StringBuilder();
                var sw = new Stopwatch();
                sw.Start();
                sb.AppendLine("PingReadEntityServerSide");
                using (var wrapper = await repo.Get(entityTypeId, entityGuid))
                {
                    var entity = wrapper.Get<IHasPingDiagnosticsServer>(entityTypeId, entityGuid, ReplicationLevel.Server);
                    sw.Stop();
                    sb.AppendFormat("get: {0} msec", sw.ElapsedMilliseconds).AppendLine();
                    sw.Restart();
                    var result = await entity.PingDiagnostics.PingRead();
                    sw.Stop();
                    if (result)
                        sb.AppendFormat("ping: {0} msec", sw.ElapsedMilliseconds).AppendLine();
                }
                Logger.IfError()?.Message(sb.ToString()).Write();

            });
        }

        [Cheat]
        public static async Task PingReadUnityThreadEntityServerSide(string entityType, string entityId)
        {
            IEntitiesRepository repo = NodeAccessor.Repository;
            if (repo.AssertIfNull(nameof(repo)))
                return;

            var entityTypeId = 0;
            if (!int.TryParse(entityType, out entityTypeId))
                entityTypeId = ReplicaTypeRegistry.GetIdByType(ReplicaTypeRegistry.GetTypeByName(entityType));

            if (entityTypeId == 0)
            {
                 Logger.IfError()?.Message("unknown type").Write();;
                return;
            }

            var entityGuid = Guid.Empty;
            if (!Guid.TryParse(entityId, out entityGuid))
            {
                 Logger.IfError()?.Message("cant parse guid").Write();;
                return;
            }

            await AsyncUtils.RunAsyncTask(async () =>
            {
                var sb = new StringBuilder();
                var sw = new Stopwatch();
                sw.Start();
                sb.AppendLine("PingReadUnityThreadEntityServerSide");
                using (var wrapper = await repo.Get(entityTypeId, entityGuid))
                {
                    var entity = wrapper.Get<IHasPingDiagnosticsServer>(entityTypeId, entityGuid, ReplicationLevel.Server);
                    sw.Stop();
                    sb.AppendFormat("get: {0} msec", sw.ElapsedMilliseconds).AppendLine();
                    sw.Restart();
                    var result = await entity.PingDiagnostics.PingReadUnityThread();
                    sw.Stop();
                    if (result > 0)
                        sb.AppendFormat("ping: {0} msec", sw.ElapsedMilliseconds).AppendLine();
                }
                Logger.IfError()?.Message(sb.ToString()).Write();

            });
        }
        [Cheat]
        public static async Task PingLocalUnityRepositoryEntity()
        {
            IEntitiesRepository repo = GameState.Instance.ClientClusterNode;
            if (repo.AssertIfNull(nameof(repo)))
                return;

            var remoteRepoId = GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId;
            await AsyncUtils.RunAsyncTask(async () =>
            {
                var sb = new StringBuilder();
                var sw = new Stopwatch();
                sw.Start();
                sb.AppendLine("PingLocalUnityRepositoryEntity");
                using (var wrapper = await repo.Get<IRepositoryCommunicationEntityClientFull>(remoteRepoId))
                {
                    var entity = wrapper.Get<IRepositoryCommunicationEntityClientFull>(remoteRepoId);
                    sw.Stop();
                    sb.AppendFormat("get: {0} msec", sw.ElapsedMilliseconds).AppendLine();
                    sw.Restart();
                    var result = await entity.PingDiagnostics.PingLocal();
                    sw.Stop();
                    if (result)
                        sb.AppendFormat("ping: {0} msec", sw.ElapsedMilliseconds).AppendLine();
                }
                Logger.IfError()?.Message(sb.ToString()).Write();

            });
        }

        [Cheat]
        public static async Task PingReadUnityRepositoryEntity()
        {
            IEntitiesRepository repo = GameState.Instance.ClientClusterNode;
            if (repo.AssertIfNull(nameof(repo)))
                return;

            var remoteRepoId = GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId;
            await AsyncUtils.RunAsyncTask(async () =>
            {
                var sb = new StringBuilder();
                var sw = new Stopwatch();
                sw.Start();
                sb.AppendLine("PingReadUnityRepositoryEntity");
                using (var wrapper = await repo.Get<IRepositoryCommunicationEntityClientFull>(remoteRepoId))
                {
                    var entity = wrapper.Get<IRepositoryCommunicationEntityClientFull>(remoteRepoId);
                    sw.Stop();
                    sb.AppendFormat("get: {0} msec", sw.ElapsedMilliseconds).AppendLine();
                    sw.Restart();
                    var result = await entity.PingDiagnostics.PingRead();
                    sw.Stop();
                    if (result)
                        sb.AppendFormat("ping: {0} msec", sw.ElapsedMilliseconds).AppendLine();
                }
                Logger.IfError()?.Message(sb.ToString()).Write();

            });
        }

        [Cheat]
        public static async Task PingWriteUnityRepositoryEntity()
        {
            IEntitiesRepository repo = GameState.Instance.ClientClusterNode;
            if (repo.AssertIfNull(nameof(repo)))
                return;

            var remoteRepoId = GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId;
            await AsyncUtils.RunAsyncTask(async () =>
            {
                var sb = new StringBuilder();
                var sw = new Stopwatch();
                sw.Start();
                sb.AppendLine("PingWriteUnityRepositoryEntity");
                using (var wrapper = await repo.Get<IRepositoryCommunicationEntityClientFull>(remoteRepoId))
                {
                    var entity = wrapper.Get<IRepositoryCommunicationEntityClientFull>(remoteRepoId);
                    sw.Stop();
                    sb.AppendFormat("get: {0} msec", sw.ElapsedMilliseconds).AppendLine();
                    sw.Restart();
                    var result = await entity.PingDiagnostics.PingWrite();
                    sw.Stop();
                    if (result)
                        sb.AppendFormat("ping: {0} msec", sw.ElapsedMilliseconds).AppendLine();
                }
                Logger.IfError()?.Message(sb.ToString()).Write();

            });
        }

        [Cheat]
        public static async Task PingLocalCharacter()
        {
            IEntitiesRepository repo = GameState.Instance.ClientClusterNode;
            if (repo.AssertIfNull(nameof(repo)))
                return;

            var characterId = GameState.Instance.CharacterRuntimeData.CharacterId;
            if (Guid.Empty == characterId)
            {
                UI.Logger.IfError()?.Message($"{nameof(characterId)} is empty").Write();
                return;
            }

            await AsyncUtils.RunAsyncTask(async () =>
            {
                var sb = new StringBuilder();
                var sw = new Stopwatch();
                sw.Start();
                sb.AppendLine("PingLocalCharacter");
                using (var wrapper = await repo.Get<IWorldCharacterClientFull>(characterId))
                {
                    var entity = wrapper?.Get<IWorldCharacterClientFull>(characterId);
                    if (entity == null)
                    {
                         Logger.IfError()?.Message("WorldCharacter not found {0}",  characterId).Write();
                        return;
                    }
                    sw.Stop();
                    sb.AppendFormat("get: {0} msec", sw.ElapsedMilliseconds).AppendLine();
                    sw.Restart();
                    var result = await entity.PingDiagnostics.PingLocal();
                    sw.Stop();
                    if (result)
                        sb.AppendFormat("ping: {0} msec", sw.ElapsedMilliseconds).AppendLine();
                }
                Logger.IfError()?.Message(sb.ToString()).Write();
            });
        }

        [Cheat]
        public static async Task PingReadCharacter()
        {
            IEntitiesRepository repo = GameState.Instance.ClientClusterNode;
            if (repo.AssertIfNull(nameof(repo)))
                return;

            var characterId = GameState.Instance.CharacterRuntimeData.CharacterId;
            if (Guid.Empty == characterId)
            {
                UI.Logger.IfError()?.Message($"{nameof(characterId)} is empty").Write();
                return;
            }
            await AsyncUtils.RunAsyncTask(async () =>
            {
                var sb = new StringBuilder();
                var sw = new Stopwatch();
                sw.Start();
                sb.AppendLine("PingReadCharacter");
                using (var wrapper = await repo.Get<IWorldCharacterClientFull>(characterId))
                {
                    var entity = wrapper?.Get<IWorldCharacterClientFull>(characterId);
                    if (entity == null)
                    {
                         Logger.IfError()?.Message("WorldCharacter not found {0}",  characterId).Write();
                        return;
                    }
                    sw.Stop();
                    sb.AppendFormat("get: {0} msec", sw.ElapsedMilliseconds).AppendLine();
                    sw.Restart();
                    var result = await entity.PingDiagnostics.PingRead();
                    sw.Stop();
                    if (result)
                        sb.AppendFormat("ping: {0} msec", sw.ElapsedMilliseconds).AppendLine();
                }
                Logger.IfError()?.Message(sb.ToString()).Write();

            });
        }

        [Cheat]
        public static async Task PingReadCharacterUnityThread()
        {
            IEntitiesRepository repo = GameState.Instance.ClientClusterNode;
            if (repo.AssertIfNull(nameof(repo)))
                return;

            var characterId = GameState.Instance.CharacterRuntimeData.CharacterId;
            if (Guid.Empty == characterId)
            {
                UI.Logger.IfError()?.Message($"{nameof(characterId)} is empty").Write();
                return;
            }
            await AsyncUtils.RunAsyncTask(async () =>
            {
                var sb = new StringBuilder();
                var sw = new Stopwatch();
                sw.Start();
                sb.AppendLine("PingReadCharacterUnityThread");
                using (var wrapper = await repo.Get<IWorldCharacterClientFull>(characterId))
                {
                    var entity = wrapper?.Get<IWorldCharacterClientFull>(characterId);
                    if (entity == null)
                    {
                         Logger.IfError()?.Message("WorldCharacter not found {0}",  characterId).Write();
                        return;
                    }
                    sw.Stop();
                    sb.AppendFormat("get: {0} msec", sw.Elapsed.TotalMilliseconds).AppendLine();
                    sw.Restart();
                    var result = await entity.PingDiagnostics.PingReadUnityThread();
                    sw.Stop();
                    if (result > 0)
                        sb.AppendFormat("ping: {0} msec. unity thread {1} msec", sw.Elapsed.TotalMilliseconds, result).AppendLine();
                }
                Logger.IfError()?.Message(sb.ToString()).Write();
            });
        }

        [Cheat]
        public static async Task PingWriteCharacter()
        {
            IEntitiesRepository repo = GameState.Instance.ClientClusterNode;
            if (repo.AssertIfNull(nameof(repo)))
                return;

            var characterId = GameState.Instance.CharacterRuntimeData.CharacterId;
            if (Guid.Empty == characterId)
            {
                UI.Logger.IfError()?.Message($"{nameof(characterId)} is empty").Write();
                return;
            }
            await AsyncUtils.RunAsyncTask(async () =>
            {
                var sb = new StringBuilder();
                var sw = new Stopwatch();
                sw.Start();
                sb.AppendLine("PingWriteCharacter");
                using (var wrapper = await repo.Get<IWorldCharacterClientFull>(characterId))
                {
                    var entity = wrapper?.Get<IWorldCharacterClientFull>(characterId);
                    if (entity == null)
                    {
                         Logger.IfError()?.Message("WorldCharacter not found {0}",  characterId).Write();
                        return;
                    }
                    sw.Stop();
                    sb.AppendFormat("get: {0} msec", sw.ElapsedMilliseconds).AppendLine();
                    sw.Restart();
                    var result = await entity.PingDiagnostics.PingWrite();
                    sw.Stop();
                    if (result)
                        sb.AppendFormat("ping: {0} msec", sw.ElapsedMilliseconds).AppendLine();
                }
                Logger.IfError()?.Message(sb.ToString()).Write();
            });
        }


        [Cheat]
        public static async Task PingLocalWizard()
        {
            IEntitiesRepository repo = GameState.Instance.ClientClusterNode;
            if (repo.AssertIfNull(nameof(repo)))
                return;

            var characterId = GameState.Instance.CharacterRuntimeData.CharacterId;
            if (Guid.Empty == characterId)
            {
                UI.Logger.IfError()?.Message($"{nameof(characterId)} is empty").Write();
                return;
            }

            await AsyncUtils.RunAsyncTask(async () =>
            {
                var sb = new StringBuilder();
                sb.AppendLine("PingLocalWizard");
                var sw = new Stopwatch();
                sw.Start();
                using (var wrapper2 = await repo.Get<IWizardEntityClientFull>(characterId))
                {
                    var entity = wrapper2?.Get<IWizardEntityClientFull>(characterId);
                    if (entity == null)
                    {
                         Logger.IfError()?.Message("WizardEntity not found {0}",  characterId).Write();
                        return;
                    }
                    sw.Stop();
                    sb.AppendFormat("get: {0} msec", sw.ElapsedMilliseconds).AppendLine();
                    sw.Restart();
                    var result = await entity.PingDiagnostics.PingLocal();
                    sw.Stop();
                    if (result)
                        sb.AppendFormat("ping: {0} msec", sw.ElapsedMilliseconds).AppendLine();
                }
                Logger.IfError()?.Message(sb.ToString()).Write();

            });
        }

        [Cheat]
        public static async Task PingReadWizard()
        {
            IEntitiesRepository repo = GameState.Instance.ClientClusterNode;
            if (repo.AssertIfNull(nameof(repo)))
                return;

            var characterId = GameState.Instance.CharacterRuntimeData.CharacterId;
            if (Guid.Empty == characterId)
            {
                UI.Logger.IfError()?.Message($"{nameof(characterId)} is empty").Write();
                return;
            }

            await AsyncUtils.RunAsyncTask(async () =>
            {
                var sb = new StringBuilder();
                var sw = new Stopwatch();
                sw.Start();
                sb.AppendLine("PingReadWizard");
                using (var wrapper2 = await repo.Get<IWizardEntityClientFull>(characterId))
                {
                    var entity = wrapper2.Get<IWizardEntityClientFull>(characterId);
                    if (entity == null)
                    {
                         Logger.IfError()?.Message("WizardEntity not found {0}",  characterId).Write();
                        return;
                    }
                    sw.Stop();
                    sb.AppendFormat("get: {0} msec", sw.ElapsedMilliseconds).AppendLine();
                    sw.Restart();
                    var result = await entity.PingDiagnostics.PingRead();
                    sw.Stop();
                    if (result)
                        sb.AppendFormat("ping: {0} msec", sw.ElapsedMilliseconds).AppendLine();
                }
                Logger.IfError()?.Message(sb.ToString()).Write();

            });
        }

        [Cheat]
        public static async Task PingWriteWizard()
        {
            IEntitiesRepository repo = GameState.Instance.ClientClusterNode;
            if (repo.AssertIfNull(nameof(repo)))
                return;

            var characterId = GameState.Instance.CharacterRuntimeData.CharacterId;
            if (Guid.Empty == characterId)
            {
                UI.Logger.IfError()?.Message($"{nameof(characterId)} is empty").Write();
                return;
            }

            await AsyncUtils.RunAsyncTask(async () =>
            {
                var sb = new StringBuilder();
                var sw = new Stopwatch();
                sw.Start();
                sb.AppendLine("PingWriteWizard");
                using (var wrapper2 = await repo.Get<IWizardEntityClientFull>(characterId))
                {
                    var entity = wrapper2.Get<IWizardEntityClientFull>(characterId);
                    if (entity == null)
                    {
                         Logger.IfError()?.Message("WizardEntity not found {0}",  characterId).Write();
                        return;
                    }
                    sw.Stop();
                    sb.AppendFormat("get: {0} msec", sw.ElapsedMilliseconds).AppendLine();
                    sw.Restart();
                    var result = await entity.PingDiagnostics.PingWrite();
                    sw.Stop();
                    if (result)
                        sb.AppendFormat("ping: {0} msec", sw.ElapsedMilliseconds).AppendLine();
                }
                Logger.IfError()?.Message(sb.ToString()).Write();

            });
        }
        [Cheat]
        public static async Task DumpWizard()
        {
            IEntitiesRepository repo = GameState.Instance.ClientClusterNode;
            if (repo.AssertIfNull(nameof(repo)))
                return;

            var characterId = GameState.Instance.CharacterRuntimeData.CharacterId;
            var pawn = GameState.Instance.GetGoFromGameObjectsForEntities(new OuterRef<IEntity>(characterId, ReplicaTypeRegistry.GetIdByType(typeof(IWorldCharacter))));
            var ego = pawn?.GetComponent<EntityGameObject>();

            if (Guid.Empty == characterId)
            {
                UI.Logger.IfError()?.Message($"{nameof(characterId)} is empty").Write();
                return;
            }
            if (ego != null)
                await AsyncUtils.RunAsyncTask(async () =>
                {
                    var sb = new StringBuilder();
                    var startTime = DateTime.UtcNow;
                    sb.AppendLine("DumpWizard");
                    using (var wrapper = await ego.ClientRepo.Get<IWizardEntityServer>(ego.EntityId))
                    {
                        var wiz = wrapper.Get<IWizardEntityClientFull>(ego.EntityId);
                        sb.AppendLine("LOCAL MASTER");
                        sb.AppendLine(await wiz.DumpEvents());
                    }
                    Logger.IfError()?.Message(sb.ToString()).Write();
                });
            await AsyncUtils.RunAsyncTask(async () =>
            {
                var sb = new StringBuilder();
                var startTime = DateTime.UtcNow;
                sb.AppendLine("DumpWizard");
                using (var wrapper = await repo.Get<IWorldCharacterClientFull>(characterId))
                {
                    var entity = wrapper.Get<IWorldCharacterClientFull>(characterId);
                    using (var wizWrapper = await repo.Get<IWizardEntityClientFull>(entity.Wizard.Id))
                    {
                        var wiz = wizWrapper.Get<IWizardEntityClientFull>(entity.Wizard.Id);
                        sb.AppendLine("REMOTE MASTER");
                        sb.AppendLine(await wiz.DumpEvents());
                    }
                }
                Logger.IfError()?.Message(sb.ToString()).Write();
            });
        }

        [Cheat]
        public static async Task DumpStats(bool remoteDump = true)
        {
            await DumpEntityStats(GameState.Instance.CharacterRuntimeData.CharacterId.ToString(), remoteDump);
        }

        [Cheat]
        public static async Task DumpEntityStats(string idString, bool remoteDump = true, string typeIdString = nameof(IWorldCharacter))
        {
            var id = Guid.Parse(idString);
            var typeId = ReplicaTypeRegistry.GetIdByType(ReplicaTypeRegistry.GetTypeByName(typeIdString));
            IEntitiesRepository repository = ClusterCommands.ClientRepository;

            if (Guid.Empty == id)
            {
                UI.Logger.IfError()?.Message($"{nameof(id)} is empty").Write();
                return;
            }

            await AsyncUtils.RunAsyncTask(async () =>
            {
                using (var wrapper = await repository.Get(typeId, id))
                {
                    var entity = wrapper?.Get<IHasStatsEngineClientBroadcast>(typeId, id, ReplicationLevel.ClientBroadcast);
                    if (remoteDump)
                        Logger.IfInfo()?.Message(await entity.Stats.DumpStats(false)).Write();
                    else
                        Logger.IfInfo()?.Message(await entity.Stats.DumpStatsLocal(false)).Write();
                }
            });
        }

        #endregion

        [Cheat]
        public static async Task SetDebugMode(bool enabled)
        {
            await AsyncUtils.RunAsyncTask(async () =>
            {
                var sceneId = GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId;
                using (var wrapper = await GameState.Instance.ClientClusterNode.Get<ICheatServiceEntityServer>(sceneId))
                {
                    var cheatServiceEntity = wrapper.Get<ICheatServiceEntityServer>(sceneId);
                    await cheatServiceEntity.SetDebugMode(enabled);
                }
            });
        }

        [Cheat]
        public static void SetDebugModeLocal(bool enabled)
        {
             Logger.IfInfo()?.Message("Set local debug mode enabled: {0}",  enabled).Write();
            ServerCoreRuntimeParameters.SetDebugMode(enabled);
        }

        [Cheat]
        public static async Task SetDebugMobs(bool enabledStatus, /*int minutes,*/ bool hard)
        {
            var repo = GameState.Instance.ClientClusterNode;
            await AsyncUtils.RunAsyncTask(async () =>
            {
                var sceneId = GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId;
                using (var wrapper = await repo.Get<ICheatServiceEntityServer>(sceneId))
                {
                    var cheatServiceEntity = wrapper.Get<ICheatServiceEntityServer>(sceneId);
                    await cheatServiceEntity.SetDebugMobs(enabledStatus, hard);
                }
            });
        }

        [Cheat]
        public static async Task SetDebugSpells(bool enabledStatus)
        {
            var repo = GameState.Instance.ClientClusterNode;
            await AsyncUtils.RunAsyncTask(async () =>
            {
                var sceneId = GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId;
                using (var wrapper = await repo.Get<ICheatServiceEntityServer>(sceneId))
                {
                    var cheatServiceEntity = wrapper.Get<ICheatServiceEntityServer>(sceneId);
                    await cheatServiceEntity.SetDebugSpells(enabledStatus);
                }
            });
        }

        [Cheat]
        public static void SetLocoClPosDamperEnabled(bool val)
        {
            GlobalConstsHolder.GlobalConstsDef.DisableLocoClPosDamper = !val;
            Logger.IfInfo()?.Message($"`{nameof(GlobalConstsHolder.GlobalConstsDef.DisableLocoClPosDamper)}` set to {GlobalConstsDef.DebugFlagsGetter.IsDisableLocoClPosDamper(GlobalConstsHolder.GlobalConstsDef)}").Write();
        }

        [Cheat]
        public static async Task PrintBrokenLocomotions()
        {
            //Client:
            PawnDataSharedProxy.Print();
            //Server:
            var repo = GameState.Instance.ClientClusterNode;
            await AsyncUtils.RunAsyncTask(async () =>
            {
                var sceneId = GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId;
                using (var wrapper = await repo.Get<ICheatServiceEntityServer>(sceneId))
                {
                    var cheatServiceEntity = wrapper.Get<ICheatServiceEntityServer>(sceneId);
                    await cheatServiceEntity.PrintBrokenLocomotions();
                }
            });
        }

        #region Position/CurveLogging

        [Flags]    
        public enum CurveLoggingOperation
        {
            Disable = 0x0,
            Enable  = 0x1,
            Dump    = 0x2,
            Show    = 0x4 | Dump,
        }

        [Cheat]
        public static async Task SetDebugMobPositionLogging(CurveLoggingOperation operation)
        {
             Logger.IfInfo()?.Message("0. SetDebugMobPositionLogging").Write();;

            var (enabledStatus, dump, _) = ParseLoggableOperation(operation);

            var debugGui = Object.FindObjectOfType<DebugGui>();
            if (debugGui == null)
            {
                Logger.IfError()?.Message($"Can't find `{nameof(DebugGui)}`").Write();
                return;
            }

            var outerRef = EntityInDebugFocus.Mob?.EntityRef ?? OuterRef.Invalid;
            if (!outerRef.IsValid)
            {
                 Logger.IfError()?.Message("No selected mob (use Ctrl+Shift+\"+\" then Ctrl+Insert to select one).").Write();;
                return;
            }

            var repo = GameState.Instance.ClientClusterNode;
            await AsyncUtils.RunAsyncTask(async () =>
            {
                var sceneId = GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId;
                using (var wrapper = await repo.Get<ICheatServiceEntityClientBroadcast>(sceneId))
                {
                    var cheatServiceEntity = wrapper.Get<ICheatServiceEntityClientBroadcast>(sceneId);
                    if (cheatServiceEntity != null)
                        await cheatServiceEntity.SetDebugMobPositionLogging(outerRef.To<IEntity>(), enabledStatus, dump);
                }
            });
        }

        [Cheat]
        public static async Task SetCurveLogging(CurveLoggingOperation operation, bool callOnServer, bool callOnOtherClients, string loggerName = null)
        {
            var charRef = Helpers.GetCharacterRef();
            if (!charRef.IsValid)
            {
                 Logger.IfError()?.Message("Can't get character ref.").Write();;
                return;
            }

            await SetCurveLogging(operation, callOnServer, callOnOtherClients, loggerName, charRef);
        }
    
        private static async Task SetCurveLogging(CurveLoggingOperation operation, bool callOnServer, bool callOnOtherClients, string loggerName, OuterRef<IEntity> charRef)
        {
            Logger.IfInfo()?.Message($"{nameof(SetCurveLogging)}({operation}, {callOnServer}, {callOnOtherClients}, {loggerName})").Write();

            if (!callOnServer && callOnOtherClients)
            {
                Logger.IfWarn()?.Message($"Unexpected combination of arguments: callOnServer:{callOnServer}, callOnOtherClients:{callOnOtherClients}.").Write();
                callOnServer = true;
            }

            if (!charRef.IsValid)
            {
                 Logger.IfError()?.Message("Invalid character ref.").Write();;
                return;
            }

            var (enabledStatus, dump, show) = ParseLoggableOperation(operation);

            var loggerInstance = CurveLogger.Get(loggerName);

            loggerInstance.Active = enabledStatus;

            var dumpId = Guid.NewGuid();

            var repo = GameState.Instance.ClientClusterNode;
            
            if (!enabledStatus && dump)
                loggerInstance.DumpDataToFile(ServerProvider.IsServer, dumpId, convertAndShow: show);

            if (!callOnServer && !callOnOtherClients)
                return;

            await AsyncUtils.RunAsyncTask(async () =>
            {
                var sceneId = GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId;
                using (var wrapper = await repo.Get<ICheatServiceEntityClientBroadcast>(sceneId))
                {
                    var cheatServiceEntity = wrapper.Get<ICheatServiceEntityClientBroadcast>(sceneId);
                    if (cheatServiceEntity != null)
                    {
                        // The way to do the same on master-copy node:s
                        await cheatServiceEntity.SetCurveLoggerState(charRef, enabledStatus, dump, !callOnOtherClients, loggerName, dumpId);
                    }
                }
            });
        }

        [Cheat]
        public static async Task SetLoggableEnable(bool enabledStatus)
        {
             Logger.IfInfo()?.Message("0. SetEntityLogableEnable").Write();;
            await SetLoggableEnable(EntityInDebugFocus.EntityRef, enabledStatus);
        }

        public static async Task SetLoggableEnable(OuterRef entityRef, bool enabledStatus)
        {
            if (!entityRef.IsValid)
            {
                 Logger.IfError()?.Message("No selected mob (use Ctrl+Shift+\"+\" then Ctrl+Insert to select one).").Write();;
                return;
            }

            var repo = GameState.Instance.ClientClusterNode;
            await AsyncUtils.RunAsyncTask(async () =>
            {
                var sceneId = GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId;
                using (var wrapper = await repo.Get<ICheatServiceEntityClientBroadcast>(sceneId))
                {
                    var cheatServiceEntity = wrapper.Get<ICheatServiceEntityClientBroadcast>(sceneId);
                    if (cheatServiceEntity != null)
                        await cheatServiceEntity.SetLoggableEnable(entityRef.To<IEntity>(), enabledStatus);
                }
            });
        }

        [Cheat]
        public static async Task SetCurveLoggingAll(CurveLoggingOperation operation, bool callOnServer = true, bool callOnOtherClients = true, string loggerName = null)
        {
            //if (DbgLog.Enabled) DbgLog.Log("#P3-11221: SetCurveMobLoggingAll");
            Logger.IfInfo()?.Message($"{nameof(SetCurveLoggingAll)}({operation}, {callOnServer}, {callOnOtherClients}, {loggerName})").Write();

            OuterRef<IEntity> entityRef = EntityInDebugFocus.EntityRef.To<IEntity>();
            if (!entityRef.IsValid)
            {
                 Logger.IfError()?.Message("No selected mob (use Ctrl+Shift+\"+\" then Ctrl+Insert to select one).").Write();;
                return;
            }

            if (string.IsNullOrWhiteSpace(loggerName))
                loggerName = entityRef.Guid.ToString();

            var charRef = EntitiesRepository.IsImplements<IWorldCharacter>(entityRef.TypeId) ? entityRef : Helpers.GetCharacterRef();
            
            var (enabledStatus, _, _) = ParseLoggableOperation(operation);
            await SetLoggableEnable(entityRef, enabledStatus);
            if (EntitiesRepository.IsImplements<IHasMobMovementSync>(EntitiesRepository.GetMasterTypeIdByReplicationLevelType(entityRef.TypeId)))
                await SetDebugMobPositionLogging(operation);
            await SetCurveLogging(operation, callOnServer, callOnOtherClients, loggerName, charRef);
        }

        [Cheat]
        public static void SetCurveLoggingCollectorTarget(bool enabledStatus, bool dump)
        {
            //if (DbgLog.Enabled) DbgLog.Log("#P3-11221: SetCurveMobLoggingAll");
            Logger.IfInfo()?.Message($"{nameof(SetCurveLoggingCollectorTarget)}({enabledStatus}, {dump})").Write();

            if (enabledStatus)
                CurveLoggerTarget.RegisterTarget();
            else
                CurveLoggerTarget.UnregisterTarget(dump);
        }

        private static (bool, bool, bool) ParseLoggableOperation(CurveLoggingOperation operation)
        {
            var enable = (operation & CurveLoggingOperation.Enable) != 0;
            var dump = (operation & CurveLoggingOperation.Dump) != 0;
            var show = (operation & CurveLoggingOperation.Show) == CurveLoggingOperation.Show;
            return (enable, dump, show);
        }


        #endregion Position/CurveLogging

        // @param `isOn` - pass false to stop repeated sleeps
        // @param `delayBeforeSleep` - delay before 1st sleep
        // @param `repeatTime` - pass 0 to do not repeat
        [Cheat]
        public static async Task MainUnityThreadOnServerSleep(bool isOn, float sleepTime, float delayBeforeSleep = 0f, float repeatTime = 0f)
        {
             Logger.IfInfo()?.Message("0. MainUnityThreadOnServerSleep").Write();;

            // var typeId = EntitiesRepositoryBase.GetIdByType(typeof(IWorldCharacter));
            // var characterId = GameState.Instance.CharacterRuntimeData.CharacterId;
            // if (characterId == Guid.Empty)
            // {
            //     Logger.IfError()?.Message($"{nameof(characterId)} is empty").Write();
            //     return;
            // }
            // 
            // var charRef = new OuterRef<IEntity>(characterId, typeId);
            var charRef = Helpers.GetCharacterRef();
            if (!charRef.IsValid)
            {
                 Logger.IfError()?.Message("Can't get character ref.").Write();;
                return;
            }

            var repo = GameState.Instance.ClientClusterNode;
            await AsyncUtils.RunAsyncTask(async () =>
            {
                var sceneId = GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId;
                using (var wrapper = await repo.Get<ICheatServiceEntityClientBroadcast>(sceneId))
                {
                    var cheatServiceEntity = wrapper.Get<ICheatServiceEntityClientBroadcast>(sceneId);
                    if (cheatServiceEntity != null)
                        await cheatServiceEntity.MainUnityThreadOnServerSleep(charRef, isOn, sleepTime, delayBeforeSleep, repeatTime);
                }
            });
        }

        //#moved to helpers
        // private static OuterRef<IEntity> GetCharacterRef()
        // {
        //     var typeId = EntitiesRepositoryBase.GetIdByType(typeof(IWorldCharacter));
        //     var characterId = GameState.Instance.CharacterRuntimeData.CharacterId;
        //     if (characterId == Guid.Empty)
        //     {
        //         Logger.IfError()?.Message($"{nameof(characterId)} is empty").Write();
        //         return default;
        //     }
        // 
        //     var charRef = new OuterRef<IEntity>(characterId, typeId);
        //     return charRef;
        // }

     



        [Cheat]
        public static void Show_Content_Keys()
        {
            var keys = ContentKeyServiceEntity.AllKeys;
            if (keys.Length > 0)
            {
                 Logger.IfInfo()?.Message("Keys active:").Write();;
                foreach (var key in keys)
                {
                    Logger.IfInfo()?.Message(key.ToString()).Write();
                }
            }
            else
                 Logger.IfInfo()?.Message("No keys active").Write();;
        }

        [Cheat]
        public static async Task GetBotsCoords()
        {
            await AsyncUtils.RunAsyncTask(async () =>
            {
                SharedVector3 ourPos;
                Quaternion ourInvRotation;

                using (var wrapper = await GameState.Instance.ClientClusterNode.Get<IWorldCharacterClientBroadcast>(GameState.Instance.CharacterRuntimeData.CharacterId))
                {
                    var ourChar = wrapper.Get<IWorldCharacterClientBroadcast>(GameState.Instance.CharacterRuntimeData.CharacterId);
                    if (ourChar == null)
                    {
                         Logger.IfError()?.Message("Cant get our char replica, GUID {0}",  GameState.Instance.CharacterRuntimeData.CharacterId).Write();
                        return;
                    }
                    ourPos = ourChar.MovementSync.Position;
                    ourInvRotation = Quaternion.Inverse((Quaternion)ourChar.MovementSync.Rotation);
                }

                KeyValuePair<Guid,  bool>[] guids;
                using (var wrapperBots = await GameState.Instance.ClientClusterNode.GetFirstService<IBotCoordinatorClientBroadcast>())
                {
                    var botCoordReplica = wrapperBots.GetFirstService<IBotCoordinatorClientBroadcast>();
                    if (botCoordReplica == null)
                    {
                         Logger.IfError()?.Message("IBotCoordinator not found").Write();;
                        return;
                    }
                    guids = botCoordReplica.BotsByAccount.SelectMany(v => v.Value.Bots).ToArray();
                }

                IEnumerable<System.ValueTuple<Guid, SharedVector3, bool>> coordsWithActive;
                var sceneId = GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId;
                using (var wrapperBots = await GameState.Instance.ClientClusterNode.Get<ICheatServiceEntityServer>(sceneId))
                {
                    var botCoordReplica = wrapperBots.GetFirstService<ICheatServiceEntityServer>();
                    if (botCoordReplica == null)
                    {
                         Logger.IfError()?.Message("IBotCoordinator not found").Write();;
                        return;
                    }
                    var coords = await botCoordReplica.ResolveCharacterCoords(guids.Select(v => v.Key).ToArray());
                    coordsWithActive = Enumerable.Zip(guids, coords, (a, b) => ValueTuple.Create(a.Key, b, a.Value));
                }

                var sb = new StringBuilder();
                sb.AppendFormat($"Total Active Bots: {guids.Where(v => v.Value).Count()} / {guids.Count()}").AppendLine();
                sb.AppendFormat("Our position is {0}", ourPos).AppendLine();

                foreach (var botCood in coordsWithActive.OrderBy(v => v.Item1))
                {
                    Vector3 relCoord = (Vector3)(botCood.Item2 - ourPos);
                    var relDirection = ourInvRotation * relCoord;

                    sb.AppendFormat("Bot {0}: Position ({1}), Relative ({2}), {3}", botCood.Item1, botCood.Item2, relDirection, botCood.Item3 ? "Active" : "Inactive").AppendLine();
                }

                Logger.IfInfo()?.Message(sb.ToString()).Write();

            });
        }

        [Cheat]
        public static async Task SetServerCheatVariable(BaseResource varName, string value)
        {
            var characterId = GameState.Instance.CharacterRuntimeData.CharacterId;
            var worldSceneRepoId = GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId;
            await AsyncUtils.RunAsyncTask(async () =>
            {
                using (var cheatServiceWrapper = await GameState.Instance.ClientClusterNode.Get<ICheatServiceEntityServer>(worldSceneRepoId))
                {
                    var cheatServiceEntity = cheatServiceWrapper.Get<ICheatServiceEntityServer>(worldSceneRepoId);
                    await cheatServiceEntity.SetServerCheatVariable(varName, value);
                }

            });
        }    
        
        [Cheat]
        public static async void AddWorldObjectInformationSubSet(WorldObjectInformationClientSubSetDef subSetDef)
        {
            if (GameState.Instance.ClientClusterNode.AssertIfNull(nameof(GameState.Instance.ClientClusterNode)))
                return;

             Logger.IfInfo()?.Message("Try add world object information client subset {0}",  subSetDef).Write();
            var characterId = GameState.Instance.CharacterRuntimeData.CharacterId;

            await AsyncUtils.RunAsyncTask(async () =>
            {
                using (var wrapper = await GameState.Instance.ClientClusterNode.Get<IWorldCharacterClientFull>(characterId))
                {
                    var worldCharacter = wrapper.Get<IWorldCharacterClientFull>(characterId);
                    var result = await worldCharacter.WorldObjectInformationSetsEngine.AddWorldObjectInformationSubSetCheat(subSetDef);
                     Logger.IfInfo()?.Message("world object information client subset {0} added result {1}",  subSetDef, result).Write();
                }
            });
        }

        [Cheat]
        public static async Task RemoveWorldObjectInformationSubSet(WorldObjectInformationClientSubSetDef subSetDef)
        {
            if (GameState.Instance.ClientClusterNode.AssertIfNull(nameof(GameState.Instance.ClientClusterNode)))
                return;

             Logger.IfInfo()?.Message("Try remove world object information client subset {0}",  subSetDef).Write();
            var characterId = GameState.Instance.CharacterRuntimeData.CharacterId;

            await AsyncUtils.RunAsyncTask(async () =>
            {
                using (var wrapper = await GameState.Instance.ClientClusterNode.Get<IWorldCharacterClientFull>(characterId))
                {
                    var worldCharacter = wrapper.Get<IWorldCharacterClientFull>(characterId);
                    var result = await worldCharacter.WorldObjectInformationSetsEngine.RemoveWorldObjectInformationSubSetCheat(subSetDef);
                     Logger.IfInfo()?.Message("world object information client subset {0} removed result {1}",  subSetDef, result).Write();
                }
            });
        }

        [Cheat]
        public static async Task WizardLogger(bool enable)
        {
            var entityRef = EntityInDebugFocus.EntityRef;
            if (!entityRef.IsValid)
                entityRef = new OuterRef(GameState.Instance.CharacterRuntimeData.CharacterId, ReplicaTypeRegistry.GetIdByType(typeof(IWorldCharacter)));
            
            await AsyncUtils.RunAsyncTask(async () =>
            {
                var worldSceneRepoId = GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId;
                using (var cheatServiceWrapper = await GameState.Instance.ClientClusterNode.Get<ICheatServiceEntityClientBroadcast>(worldSceneRepoId))
                {
                    var cheatServiceEntity = cheatServiceWrapper.Get<ICheatServiceEntityClientBroadcast>(worldSceneRepoId);
                    await cheatServiceEntity.EnableWizardLogger(entityRef, enable);
                }
            });
        }

        [Cheat]
        public static async Task EndGame()
        {
            string[] lines =
            {     
                "addknowledge /Inventory/Knowledge/Quest/Key01Quest/Key01Quest_CompletedKW",
                "addknowledge /Inventory/Knowledge/Quest/Key02Quest/Key02Quest_CompletedKW",
                "addknowledge /Inventory/Knowledge/Quest/Key03Quest/Key03Quest_CompletedKW",
                "addknowledge /Inventory/Knowledge/Quest/Key04Quest/Key04Quest_CompletedKW",
                "addknowledge /Inventory/Knowledge/Quest/Key05Quest/Key05Quest_CompletedKW",
                "addknowledge /Inventory/Knowledge/Quest/Key06Quest/Key06Quest_CompletedKW",
                "spell /UtilPrefabs/Spells/Quests/KeyMainQuest/CapsuleFinal"          
            };
            await Batch(lines);
        }

        [Cheat]
        public static void Pools() => Pool.Dump();

        [Cheat]
        public static void PoolsOverflow() => Pool.DumpWithOverflow();

        [Cheat]
        public static void GeneratePoolsSizes(bool increase = true) => Pool.GeneratePoolSizes(increase);

        [Cheat]
        public static async Task Gender(string gender, string idString = null)
        {
            OuterRef entityRef = ResolveEntityRef(idString);
            await ChangeGender(entityRef, gender);
        }

        private static Task ChangeGender(OuterRef entityRef, string gender)
        {
            GenderDef genderDef;
            switch (gender.ToLower())
            {
                case "male": genderDef = new ResourceRef<GenderDef>("/Inventory/Gender/Male").Target; break;
                case "female": genderDef = new ResourceRef<GenderDef>("/Inventory/Gender/Female").Target; break;
                default: throw new ArgumentException($"Unknown gender: {gender}");
            }
            return InvokeCheatServiceMethod(c => c.SetGender(entityRef, genderDef));
        }

        [Cheat]
        public static Task AddTrauma(string name, string entityId = null)
        {
            OuterRef entityRef = ResolveEntityRef(entityId);
            return InvokeCheatServiceMethod(c => c.InvokeTrauma(entityRef, name));
        }
        
        [Cheat]
        public static Task RemoveTrauma(string name, string entityId = null)
        {
            OuterRef entityRef = ResolveEntityRef(entityId);
            return InvokeCheatServiceMethod(c => c.StopTrauma(entityRef, name));
        }

        
        
        
        #region Support
        
        private static OuterRef ResolveEntityRef(string entityId)
        {
            OuterRef entityRef;
            if (entityId != null)
            {
                entityRef = new OuterRef(Guid.Parse(entityId), ReplicaTypeRegistry.GetIdByType(typeof(IWorldCharacter)));
            }
            else
            {
                entityRef = EntityInDebugFocus.EntityRef;
                if (!entityRef.IsValid)
                    entityRef = new OuterRef(GameState.Instance.CharacterRuntimeData.CharacterId, ReplicaTypeRegistry.GetIdByType(typeof(IWorldCharacter)));
            }

            return entityRef;
        }

        private static async Task InvokeCheatServiceMethod(Func<ICheatServiceEntityClientBroadcast, Task> action)
        {
            await AsyncUtils.RunAsyncTask(async () =>
            {
                var worldSceneRepoId = GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId;
                using (var cheatServiceWrapper = await GameState.Instance.ClientClusterNode.Get<ICheatServiceEntityClientBroadcast>(worldSceneRepoId))
                {
                    var cheatServiceEntity = cheatServiceWrapper.Get<ICheatServiceEntityClientBroadcast>(worldSceneRepoId);
                    await action(cheatServiceEntity);
                }
            });
        }
		
		#endregion
    }
}