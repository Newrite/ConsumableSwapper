namespace ConsumableSwapper;

#pragma warning disable CA1416
using System;
using Mutagen.Bethesda.WPF.Reflection.Attributes;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Noggog;
using static Mutagen.Bethesda.FormKeys.SkyrimSE.Skyrim.Keyword;
using System.Threading.Tasks;

// public class Settings
// {
//   [SettingName("Mod name")]
//   [Tooltip("Full name including extension")]
//   public string ModName = "SomeEsp.esp";
// 
//   [SettingName("Keyword Form Id")]
//   [Tooltip("Form Id of keyword in mod")]
//   public uint FormId = 0x800;
// }

public static class Program
{
    // static Lazy<Settings> _settings = new();
    public static async Task<int> Main(string[] args)
    {
        return await SynthesisPipeline.Instance.AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
            .SetTypicalOpen(GameRelease.SkyrimSE, new ModKey("ConsumableSwapper.esp", ModType.Plugin))
            .Run(args);
    }

    private static void SynthesisLog(string message, bool special = false)
    {
        if (special)
        {
            Console.WriteLine();
            Console.Write(">>> ");
        }

        Console.WriteLine(message);
        if (special) Console.WriteLine();
    }

    private static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
    {
        var skyrimEsm = "Skyrim.esm";
        var dragonbornEsm = "Dragonborn.esm";
        var apothecaryEsp = "Apothecary.esp";
        var reSimonrimEsp = "RESimonrim.esp";

        var kiEnergyDurationKeyword =
            state.LinkCache.Resolve<IKeywordGetter>(new FormKey(ModKey.FromNameAndExtension(reSimonrimEsp), 0x951));

        var excludePatcherKeyword =
            state.LinkCache.Resolve<IKeywordGetter>(new FormKey(ModKey.FromNameAndExtension(reSimonrimEsp), 0x9FA));

        var loadOrderLinkCache = state.LoadOrder.ToImmutableLinkCache();

        var ingestibles = new List<IIngestibleGetter[]>
        {
            new[] // Potion of Strength
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EB02)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EB05)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EB03)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EB04)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0xFF9FE))
            },
            new[] // Potion of Defender
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EB30)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x398FD)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x39903)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x39904)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(apothecaryEsp),
                    0x21421C))
            },
            new[] // Potion of Shroud
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(apothecaryEsp),
                    0xF387D)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(apothecaryEsp),
                    0xF387E)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(apothecaryEsp),
                    0xF387F)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(apothecaryEsp),
                    0xF387C))
            },
            new[] // Poison Burden
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x73F37)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x73F38)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(apothecaryEsp),
                    0x162EDF)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(apothecaryEsp),
                    0x162EE1))
            },
            new[] // Poison Calm
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x73F90)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x73F91)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x73F92)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x73F93)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x73F94))
            },
            new[] // Poison Command
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x73F8B)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x73F8C)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x73F8D)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x73F8E)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0xFFA00))
            },
            new[] // Poison Corrosion
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x73F47)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x73F48)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0xFFA02)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(apothecaryEsp),
                    0x162EDB)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(apothecaryEsp),
                    0x162EDD))
            },
            new[] // Poison Damage Health
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3A5A4)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x73F31)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x73F32)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x73F33)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x73F34))
            },
            new[] // Poison Damage Health Linger
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x65A6F)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x73F35)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x73F36)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(apothecaryEsp),
                    0x1C82EB)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(apothecaryEsp),
                    0x241B1E))
            },
            new[] // Poison Damage Magicka
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x65A65)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x73F39)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x73F3A)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x73F3B)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x73F3C))
            },
            new[] // Poison Damage Magicka Linger
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x65A70)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x73F3D)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x73F3E)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(apothecaryEsp),
                    0x1C82E8)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(apothecaryEsp),
                    0x241B1F))
            },
            new[] // Poison Damage Stamina
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x65A66)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x73F41)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x73F42)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x73F43)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x73F44))
            },
            new[] // Poison Damage Stamina Linger
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x65A71)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x73F45)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x73F46)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(apothecaryEsp),
                    0x1C82E9)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(apothecaryEsp),
                    0x241B20))
            },
            new[] // Poison Damage Weapon
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x73F3F)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x73F40)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(apothecaryEsp),
                    0x162ED8)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(apothecaryEsp),
                    0x162ED9))
            },
            new[] // Potion Dlc2 restore all
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(dragonbornEsm),
                    0x01AADD)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(dragonbornEsm),
                    0x01AADE)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(dragonbornEsm),
                    0x01AADF)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(dragonbornEsm),
                    0x01AAE0)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(dragonbornEsm),
                    0x01AAE1)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(dragonbornEsm),
                    0x01AAE2))
            },
            new[] // Poison Fear
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x65A68)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x73F5E)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x73F5F)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x73F60)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x73F61))
            },
            new[] // Potion Defender
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EB30)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x398FD)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x39903)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x39904)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(apothecaryEsp),
                    0x21421C))
            },
            new[] // Potion Strength
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EB02)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EB05)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EB03)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EB04)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0xFF9FE))
            },
            new[] // Potion Health Rate
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EB09)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EB0A)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EB0B)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EB0C)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(apothecaryEsp),
                    0x1F0B00))
            },
            new[] // Potion Health
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EAF2)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EAF4)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EAF6)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EAF5)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(apothecaryEsp),
                    0x1F0AFE))
            },
            new[] // Potion Magicka
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EAF7)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EAFA)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EAFB)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EAFC)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0xFF9FC))
            },
            new[] // Potion Magicka Rate
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EB0D)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EB0E)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EB0F)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EB10)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(apothecaryEsp),
                    0x1F0AFC))
            },
            new[] // Potion SpeedMult
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(apothecaryEsp),
                    0x1CD3F0)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(apothecaryEsp),
                    0x1CD3EF)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(apothecaryEsp),
                    0x1CD3F2))
            },
            new[] // Potion Barbarian
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x39CFB)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x39D02)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x39D0A)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x39D12))
            },
            new[] // Potion Voice
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EB2F)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x398F4)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x398F5)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x398F6)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(apothecaryEsp),
                    0x1F0B02))
            },
            new[] // Potion Alteration
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EB36)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3988E)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x39899)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x3989A))
            },
            new[] // Potion Merchant
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EB35)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x39970)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x39971)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x39973))
            },
            new[] // Potion Gladiator
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EB32)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3989B)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x398AB)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x398AC))
            },
            new[] // Potion Conjuration
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EB37)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x398AD)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x398EC)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x398ED))
            },
            new[] // Potion Destruction
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EB38)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x398EE)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x398F1)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x398F3))
            },
            new[] // Potion Illusion
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EB39)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3994F)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x39950)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x39951))
            },
            new[] // Potion Locksmith
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x39930)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x39945)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x39946)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x39948))
            },
            new[] // Potion Marksman
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EB2C)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x39949)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3994B)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x3994E))
            },
            new[] // Potion Marksman
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EB2A)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x39954)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x39955)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x39956))
            },
            new[] // Potion Cutpurse
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EB31)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x39957)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x39959)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x39958))
            },
            new[] // Potion Restoration
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EB3A)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3995A)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3995B)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x3995C))
            },
            new[] // Potion Footpad
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EB33)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x39968)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x39969)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x3996F))
            },
            new[] // Potion Champion
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EB2B)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x39974)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x39980)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x39AA6))
            },
            new[] // Potion Pugilist
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EB2E)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3995D)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x39962)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x39967))
            },
            new[] // Potion Assassin
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0xD6948)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0xD6949)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0xD697F)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0xD6980))
            },
            new[] // Potion Stamina
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EAFD)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EB00)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EAFE)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EAFF)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(apothecaryEsp),
                    0x1F0AF7))
            },
            new[] // Potion Stamina Rate
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EB11)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EB12)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x39B0C)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EB14)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(apothecaryEsp),
                    0x1F0AF8))
            },
            new[] // Poison Frenzy
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x65A69)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x73F5A)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x73F5B)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x73F5C)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x73F5D))
            },
            new[] // Potion Invisibility
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EB3E)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EB40)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EB3F)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x3EB41))
            },
            new[] // Potion Muffle
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0xFF9FD)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0xFFA01)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0xFF9FF)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x65A64))
            },
            new[] // Poison Paralysis
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x65A6A)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x74A38)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x74A39)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x74A3A)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x74A3B))
            },
            new[] // Potion Mirror
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x65A6E)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x73F56)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x73F57)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x73F58)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x73F59))
            },
            new[] // Potion Fire Res
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x39B4A)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EAE9)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x39B4B)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x3EAEF))
            },
            new[] // Potion Frost Res
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x39B8A)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EAED)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x39BDF)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x3EAF0))
            },
            new[] // Potion Magicka Res
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x39E52)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x39E53)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x39E54)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x39E55))
            },
            new[] // Potion Shock Res
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x39BE1)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EAEE)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x39BE3)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x3EAF1))
            },
            new[] // Potion Poison Res
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(apothecaryEsp),
                    0xF898F)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(apothecaryEsp),
                    0xF8991)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(apothecaryEsp),
                    0xF8993)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(apothecaryEsp),
                    0xF8995))
            },
            new[] // Poison Silence
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(apothecaryEsp),
                    0xF8999)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(apothecaryEsp),
                    0xF899A)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(apothecaryEsp),
                    0xF899B)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(apothecaryEsp),
                    0xF899C)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(apothecaryEsp),
                    0xF899D))
            },
            new[] // Potion Conflict
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0xF84B5)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0xF84B6)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0xF84B7)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0xF84B8)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0xF84B9)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0xF84BA))
            },
            new[] // Potion Escape
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0xF84AE)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0xF84AF)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0xF84B0)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0xF84B1)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0xF84B2)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0xF84B3))
            },
            new[] // Potion Keenshot
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0xF84BB)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0xF84BC)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0xF84BD)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0xF84BE)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0xF84BF)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0xF84C0))
            },
            new[] // Potion Larceny
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0xF84A7)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0xF84A8)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0xF84A9)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0xF84AA)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0xF84AB)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0xF84AC))
            },
            new[] // Potion Plunder
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0xF84C1)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0xF84C2)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0xF84C3)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0xF84C4)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0xF84C5)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0xF84C6))
            },
            new[] // Poison Weakness Fire
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x73F49)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x73F4A)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x73F4B)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x73F4C))
            },
            new[] // Poison Weakness Frost
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x73F4D)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x73F4E)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x73F4F)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x73F50))
            },
            new[] // Poison Weakness Shock
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x65A6D)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x73F53)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x73F54)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x73F55))
            },
            new[] // Poison Weakness Poison
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x65A6B)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x65A6C)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x73F52)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(apothecaryEsp),
                    0x24BD23))
            },
            new[] // Potion Waterbreathing
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3AC2E)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3AC2F)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3AC30)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x3AC31))
            },
            new[] // Potion Waterwalking
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(apothecaryEsp),
                    0xF386E)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(apothecaryEsp),
                    0xF386F)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(apothecaryEsp),
                    0xF3870)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(dragonbornEsm),
                    0x390E0))
            },
            new[] // Potion Restore Health \ Vampirism
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EADD)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EADE)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EADF)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EAE3)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x39BE4)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x39BE5))
            },
            new[] // Potion Restore Magicka  \ Magic Shield
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EAE0)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EAE1)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EAE2)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EAE4)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x39BE6)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x39BE7))
            },
            new[] // Potion Restore Stamina  \ Petrified Blood
            {
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EAE5)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x39BE8)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EAE7)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EAE8)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm),
                    0x3EAE6)),
                state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x39CF3))
            },
        };

        var stream = File.CreateText($"{state.DataFolderPath}\\ConsumableSwapper_SWAP.ini");
        stream.WriteLine("[Forms]");

        if (kiEnergyDurationKeyword != null)
        {
            foreach (var magicEffectGetter in state.LoadOrder.PriorityOrder.WinningOverrides<IMagicEffectGetter>())
            {
                if (magicEffectGetter == null || magicEffectGetter.IsDeleted ||
                    (magicEffectGetter.Archetype.Type != MagicEffectArchetype.TypeEnum.ValueModifier &&
                     magicEffectGetter.Archetype.Type != MagicEffectArchetype.TypeEnum.PeakValueModifier))
                {
                    continue;
                }

                if (magicEffectGetter.Archetype.ActorValue == ActorValue.CarryWeight &&
                    !magicEffectGetter.HasKeyword(excludePatcherKeyword))
                {
                    SynthesisLog(
                        $"Patch CarryWeight magic effect Ki Duration: {magicEffectGetter?.EditorID}");
                    var modifiedEffect = state.PatchMod.MagicEffects.GetOrAddAsOverride(magicEffectGetter);
                    // modifiedEffect.BaseCost *= 10.0f;
                    modifiedEffect.Keywords ??= new ExtendedList<IFormLinkGetter<IKeywordGetter>>();
                    modifiedEffect.Keywords.Add(kiEnergyDurationKeyword);
                    modifiedEffect.Archetype = new MagicEffectArchetype(MagicEffectArchetype.TypeEnum.Script);
                    // if ((modifiedEffect.Flags & MagicEffect.Flag.Detrimental) == 0)
                    // {
                    //     modifiedEffect.Flags |= MagicEffect.Flag.Detrimental;
                    // }
                    // else
                    // {
                    //     modifiedEffect.Flags &= ~MagicEffect.Flag.Detrimental;
                    // }
                    // modifiedEffect.Flags.SetFlag(MagicEffect.Flag.Detrimental, false);
                }
            }

            // foreach (var ingestibleGetter in state.LoadOrder.PriorityOrder.WinningOverrides<IIngestibleGetter>())
            // {
            //     if (ingestibleGetter == null || ingestibleGetter.IsDeleted || ingestibleGetter.Effects.Count <= 0)
            //     {
            //         continue;
            //     }
// 
            //     var modifiedIngestible = ingestibleGetter.DeepCopy();
            //     bool overriden = false;
// 
            //     for (int i = 0; i < modifiedIngestible.Effects.Count; i++)
            //     {
            //         var hasKeyword = modifiedIngestible?.Effects[i]?.BaseEffect?.TryResolve(loadOrderLinkCache)
            //             ?.HasKeyword(kiEnergyDurationKeyword);
// 
            //         if (hasKeyword.HasValue && hasKeyword.Value)
            //         {
            //             overriden = true;
            //             var magnitude = modifiedIngestible.Effects[i].Data!.Magnitude / 10.0f;
            //             SynthesisLog(
            //                 $"Ingestible Ki Duration Patch: {modifiedIngestible?.EditorID} new magnitude: {magnitude}");
            //             modifiedIngestible.Effects[i].Data!.Magnitude = magnitude;
            //         }
            //     }
// 
            //     if (overriden)
            //     {
            //         state.PatchMod.Ingestibles.Set(modifiedIngestible);
            //     }
            // }

            // foreach (var ench in state.LoadOrder.PriorityOrder.WinningOverrides<IObjectEffectGetter>())
            // {
            //     if (ench == null || ench.IsDeleted || ench.Effects.Count <= 0)
            //     {
            //         continue;
            //     }
// 
            //     for (int i = 0; i < ench.Effects.Count; i++)
            //     {
            //         var hasKeyword = ench?.Effects[i]?.BaseEffect?.TryResolve(loadOrderLinkCache)
            //             ?.HasKeyword(kiEnergyDurationKeyword);
// 
            //         if (hasKeyword.HasValue && hasKeyword.Value)
            //         {
            //             var modifiedEnch = state.PatchMod.ObjectEffects.GetOrAddAsOverride(ench);
            //             var magnitude = (modifiedEnch.Effects[i].Data!.Magnitude / 10.0f); // * 2.0f;
            //             SynthesisLog(
            //                 $"Ench Ki Duration Patch: {ench?.EditorID} new magnitude: {magnitude}");
            //             modifiedEnch.Effects[i].Data!.Magnitude = magnitude;
            //         }
            //     }
            // }

            // foreach (var spell in state.LoadOrder.PriorityOrder.WinningOverrides<ISpellGetter>())
            // {
            //     if (spell == null || spell.IsDeleted || spell.Effects.Count <= 0)
            //     {
            //         continue;
            //     }
// 
            //     var modifiedSpell = spell.DeepCopy();
            //     bool overriden = false;
// 
            //     for (int i = 0; i < spell.Effects.Count; i++)
            //     {
            //         var hasKeyword = spell?.Effects[i]?.BaseEffect?.TryResolve(loadOrderLinkCache)
            //             ?.HasKeyword(kiEnergyDurationKeyword);
// 
            //         if (hasKeyword.HasValue && hasKeyword.Value)
            //         {
            //             overriden = true;
            //             var magnitude = (modifiedSpell.Effects[i].Data.Magnitude / 10.0f); //* 2.0f;
            //             SynthesisLog(
            //                 $"Spell Ki Duration Patch: {spell?.EditorID} new magnitude: {magnitude}");
            //             modifiedSpell.Effects[i].Data!.Magnitude = magnitude;
            //         }
            //     }
// 
            //     if (overriden)
            //     {
            //         state.PatchMod.Spells.Set(modifiedSpell);
            //     }
            // }
        }


        foreach (var listIngestible in ingestibles)
        {
            var firstIngestible = state.PatchMod.Ingestibles.GetOrAddAsOverride(listIngestible[0]);
            for (var i = 0; i < listIngestible.Length; i++)
            {
                if (i == 0 || firstIngestible == null)
                {
                    continue;
                }


                var modifiedIngestible = state.PatchMod.Ingestibles.GetOrAddAsOverride(listIngestible[i]);
                SynthesisLog(
                    $"Copy Stats and write to BOS config: {modifiedIngestible.EditorID} | {firstIngestible.EditorID}");
                stream.WriteLine($"{modifiedIngestible.EditorID}|{firstIngestible.EditorID}");

                modifiedIngestible.Name = firstIngestible.Name;
                modifiedIngestible.Weight = firstIngestible.Weight;
                modifiedIngestible.Keywords = firstIngestible.Keywords;
                modifiedIngestible.Value = firstIngestible.Value;
                modifiedIngestible.Effects.Clear();

                foreach (var effect in firstIngestible.Effects)
                {
                    modifiedIngestible.Effects.Add(effect);
                }
            }
        }

        stream.Flush();
        stream.Close();

        // SynthesisLog("Write Powers", true);
// 
        // foreach (var spell in state.LoadOrder.PriorityOrder.WinningOverrides<ISpellGetter>())
        // {
        //   if (spell.IsDeleted || spell.Type is not (SpellType.Power or SpellType.LesserPower))
        //   {
        //     continue;
        //   }
        //   SynthesisLog($"Add power: {spell.EditorID}");
        //   state.PatchMod.Spells.GetOrAddAsOverride(spell);
        // }

        SynthesisLog(
            "Start patch LeveledList", true);

        foreach (var leveledList in state.LoadOrder.PriorityOrder.WinningOverrides<ILeveledItemGetter>())
        {
            if (leveledList.IsDeleted || leveledList.Entries is not { Count: > 0 })
            {
                continue;
            }

            for (var j = 0; j < leveledList.Entries.Count; j++)
            {
                var entrie = leveledList.Entries[j];
                foreach (var listIngestible in ingestibles)
                {
                    for (var i = 0; i < listIngestible.Length; i++)
                    {
                        if (i == 0)
                        {
                            continue;
                        }

                        if (entrie?.Data?.Reference.FormKey != listIngestible[i].FormKey) continue;

                        SynthesisLog(
                            $"Swap forms: {entrie?.Data?.Reference.FormKey} | {listIngestible[i].EditorID} to {listIngestible[0].EditorID}");
                        var modifiedLeveledList = state.PatchMod.LeveledItems.GetOrAddAsOverride(leveledList);
                        modifiedLeveledList.Entries?[j].Data?.Reference.SetTo(listIngestible[0]);
                    }
                }
            }
        }

        SynthesisLog(
            "Start patch NPC", true);

        foreach (var npc in state.LoadOrder.PriorityOrder.WinningOverrides<INpcGetter>())
        {
            if (npc.IsDeleted || npc.Items is not { Count: > 0 })
            {
                continue;
            }

            for (var j = 0; j < npc.Items.Count; j++)
            {
                foreach (var listIngestible in ingestibles)
                {
                    for (var i = 0; i < listIngestible.Length; i++)
                    {
                        if (i == 0)
                        {
                            continue;
                        }

                        if (npc.Items[j].Item?.Item.FormKey != listIngestible[i].FormKey) continue;

                        SynthesisLog(
                            $"Swap forms: {npc.Items[j].Item?.Item.FormKey} | {listIngestible[i].EditorID} to {listIngestible[0].EditorID}");
                        var modifiedNpc = state.PatchMod.Npcs.GetOrAddAsOverride(npc);
                        modifiedNpc.Items?[j].Item.Item.SetTo(listIngestible[0]);
                    }
                }
            }
        }

        SynthesisLog(
            "Start patch FormList", true);

        foreach (var formList in state.LoadOrder.PriorityOrder.WinningOverrides<IFormListGetter>())
        {
            if (formList.IsDeleted || formList.Items is not { Count: > 0 })
            {
                continue;
            }

            for (var j = 0; j < formList.Items.Count; j++)
            {
                foreach (var listIngestible in ingestibles)
                {
                    for (var i = 0; i < listIngestible.Length; i++)
                    {
                        if (i == 0)
                        {
                            continue;
                        }

                        if (formList.Items[j].FormKey != listIngestible[i].FormKey) continue;

                        SynthesisLog(
                            $"Swap forms: {formList.Items[j].FormKey} | {listIngestible[i].EditorID} to {listIngestible[0].EditorID}");
                        var modifiedFormList = state.PatchMod.FormLists.GetOrAddAsOverride(formList);
                        modifiedFormList.Items[j].AsSetter().SetTo(listIngestible[0]);
                    }
                }
            }
        }

        SynthesisLog(
            "Start patch Containers", true);

        foreach (var container in state.LoadOrder.PriorityOrder.WinningOverrides<IContainerGetter>())
        {
            if (container.IsDeleted || container.Items is not { Count: > 0 })
            {
                continue;
            }

            for (var j = 0; j < container.Items.Count; j++)
            {
                foreach (var listIngestible in ingestibles)
                {
                    for (var i = 0; i < listIngestible.Length; i++)
                    {
                        if (i == 0)
                        {
                            continue;
                        }

                        if (container.Items[j].Item?.Item.FormKey != listIngestible[i].FormKey) continue;

                        SynthesisLog(
                            $"Swap forms: {container.Items[j].Item?.Item.FormKey} | {listIngestible[i].EditorID} to {listIngestible[0].EditorID}");
                        var modifiedContainer = state.PatchMod.Containers.GetOrAddAsOverride(container);
                        modifiedContainer.Items?[j].Item.Item.SetTo(listIngestible[0]);
                    }
                }
            }
        }

        SynthesisLog(
            "Start patch NPCs", true);
        // foreach (var npc in state.LoadOrder.PriorityOrder.WinningOverrides<INpcGetter>())
        // {
        //     if (npc.IsDeleted)
        //     {
        //         continue;
        //     }
// 
        //     var pclevelmult = npc.Configuration.Level as PcLevelMult;
        //     if (pclevelmult != null)
        //     {
        //         // if (pclevelmult.LevelMult > 1.0f && npc.Configuration.CalcMinLevel <= 30) continue;
        //         // if (npc.Configuration.CalcMinLevel <= 30) continue;
        //         if (pclevelmult.LevelMult < 1.5f || npc.Configuration.CalcMinLevel <= 30)
        //         {
        //             if ((npc.Configuration.Flags & NpcConfiguration.Flag.Summonable) != 0) { continue; }
        //             var modifiedNpc = state.PatchMod.Npcs.GetOrAddAsOverride(npc);
        //             var levelMult = (modifiedNpc.Configuration.Flags & NpcConfiguration.Flag.Unique) != 0 ? 1.5f : 1.0f;
        //             if (pclevelmult.LevelMult < levelMult)
        //             {
        //                 modifiedNpc.Configuration.Level = new PcLevelMult() { LevelMult = levelMult };
        //             }
// 
        //             if (npc.Configuration.CalcMinLevel > 30)
        //             {
        //                 var result = (short)(npc.Configuration.CalcMinLevel / 2);
        //                 if (result < 25)
        //                 {
        //                     result = 25;
        //                 }
// 
        //                 modifiedNpc.Configuration.CalcMinLevel = result;
        //             }
        //         }
        //     }

            // var staticlevel = npc.Configuration.Level as NpcLevel;
            // if (staticlevel != null)
            // {
            //     var currentLevel = staticlevel.Level;
            //     var minLevel = currentLevel;
            //     var maxLevel = (short)(minLevel * 2.5f);
            //     var modifiedNpc = state.PatchMod.Npcs.GetOrAddAsOverride(npc);
            //     var levelMult = (modifiedNpc.Configuration.Flags & NpcConfiguration.Flag.Unique) != 0 ? 1.5f : 1.0f;
            //     modifiedNpc.Configuration.Level = new PcLevelMult() { LevelMult = levelMult };
            //     modifiedNpc.Configuration.CalcMinLevel = minLevel;
            //     modifiedNpc.Configuration.CalcMaxLevel = maxLevel;
            // }
        // }

        SynthesisLog("Done patching consum swapper!", true);
    }
}