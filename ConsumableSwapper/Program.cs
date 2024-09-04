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
      .SetTypicalOpen(GameRelease.SkyrimSE, new ModKey("ScriptItemsMarker.esp", ModType.Plugin))
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
    var apothecaryEsp = "Apothecary.esp";

    var ingestibles = new List<IIngestibleGetter[]>
    {
      new[] // Potion of Strength
      {
        state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x3EB02)),
        state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x3EB05)),
        state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x3EB03)),
        state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x3EB04)),
        state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0xFF9FE))
      },
      new[] // Potion of Defender
      {
        state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x3EB30)),
        state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x398FD)),
        state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x39903)),
        state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x39904)),
        state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(apothecaryEsp), 0x21421C))
      },
      new[] // Potion of Shroud
      {
        state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(apothecaryEsp), 0xF387D)),
        state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(apothecaryEsp), 0xF387E)),
        state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(apothecaryEsp), 0xF387F)),
        state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(apothecaryEsp), 0xF387C))
      },
      new[] // Poison Burden
      {
        state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x73F37)),
        state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x73F38)),
        state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(apothecaryEsp), 0x162EDF)),
        state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(apothecaryEsp), 0x162EE1))
      },
      new[] // Poison Calm
      {
        state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x73F90)),
        state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x73F91)),
        state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x73F92)),
        state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x73F93)),
        state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x73F94))
      },
      new[] // Poison Command
      {
        state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x73F8B)),
        state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x73F8C)),
        state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x73F8D)),
        state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x73F8E)),
        state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0xFFA00))
      },
      new[] // Poison Corrosion
      {
        state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x73F47)),
        state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0x73F48)),
        state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(skyrimEsm), 0xFFA02)),
        state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(apothecaryEsp), 0x162EDB)),
        state.LinkCache.Resolve<IIngestibleGetter>(new FormKey(ModKey.FromNameAndExtension(apothecaryEsp), 0x162EDD))
      },
    };


    var stream = File.CreateText("ConsumableSwapper_SWAP.ini");
    stream.WriteLine("[Forms]");
    
      
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

    foreach (var leveledList in state.LoadOrder.PriorityOrder.WinningOverrides<ILeveledItemGetter>())
    {
      if (leveledList.IsDeleted || leveledList.Entries is not {Count: > 0})
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

    // foreach (var npc in state.LoadOrder.PriorityOrder.WinningOverrides<INpcGetter>())
    // {
    //   if (!(weapon.VirtualMachineAdapter?.Scripts.Count > 0) || !weapon.Template.IsNull) continue;
    //   var modifiedWeapon = state.PatchMod.Weapons.GetOrAddAsOverride(weapon);
    //   modifiedWeapon.Keywords ??= new ExtendedList<IFormLinkGetter<IKeywordGetter>>();
    //   modifiedWeapon.Keywords.Add(exclusiveKeyword);
    //   SynthesisLog($"Add keyword to weapon: {modifiedWeapon.EditorID}");
    // }
// 
    // foreach (var formList in state.LoadOrder.PriorityOrder.WinningOverrides<IFormListGetter>())
    // {
    //   if (!(weapon.VirtualMachineAdapter?.Scripts.Count > 0) || !weapon.Template.IsNull) continue;
    //   var modifiedWeapon = state.PatchMod.Weapons.GetOrAddAsOverride(weapon);
    //   modifiedWeapon.Keywords ??= new ExtendedList<IFormLinkGetter<IKeywordGetter>>();
    //   modifiedWeapon.Keywords.Add(exclusiveKeyword);
    //   SynthesisLog($"Add keyword to weapon: {modifiedWeapon.EditorID}");
    // }
// 
    // foreach (var container in state.LoadOrder.PriorityOrder.WinningOverrides<IContainerGetter>())
    // {
    //   if (!(weapon.VirtualMachineAdapter?.Scripts.Count > 0) || !weapon.Template.IsNull) continue;
    //   var modifiedWeapon = state.PatchMod.Weapons.GetOrAddAsOverride(weapon);
    //   modifiedWeapon.Keywords ??= new ExtendedList<IFormLinkGetter<IKeywordGetter>>();
    //   modifiedWeapon.Keywords.Add(exclusiveKeyword);
    //   SynthesisLog($"Add keyword to weapon: {modifiedWeapon.EditorID}");
    // }

    SynthesisLog("Done patching consum swapper!", true);
  }
}