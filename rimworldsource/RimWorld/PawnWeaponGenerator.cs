using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
namespace RimWorld
{
	public static class PawnWeaponGenerator
	{
		private static List<ThingStuffPair> potentialWeapons;
		public static void Reset()
		{
			PawnWeaponGenerator.potentialWeapons = ThingStuffPair.AllWith((ThingDef td) => td.equipmentType == EquipmentType.Primary && td.canBeSpawningInventory && td.weaponTags != null);
		}
		public static float CheapestNonDerpPriceFor(ThingDef weaponDef)
		{
			float num = 9999999f;
			for (int i = 0; i < PawnWeaponGenerator.potentialWeapons.Count; i++)
			{
				ThingStuffPair thingStuffPair = PawnWeaponGenerator.potentialWeapons[i];
				if (thingStuffPair.thing == weaponDef && !PawnWeaponGenerator.IsDerpWeapon(thingStuffPair.thing, thingStuffPair.stuff) && thingStuffPair.Price < num)
				{
					num = thingStuffPair.Price;
				}
			}
			return num;
		}
		public static void TryGenerateWeaponFor(Pawn pawn)
		{
			if (pawn.kindDef.weaponTags == null || pawn.kindDef.weaponTags.Count == 0)
			{
				return;
			}
			if (!pawn.RaceProps.ToolUser)
			{
				return;
			}
			float weaponMoney = pawn.kindDef.weaponMoney.RandomInRange;
			IEnumerable<ThingStuffPair> source = 
				from w in PawnWeaponGenerator.potentialWeapons
				where w.Price <= weaponMoney && (pawn.Faction == null || pawn.Faction.def.techLevel.CanSpawnWithEquipmentFrom(w.thing.techLevel)) && pawn.kindDef.weaponTags.Any((string tag) => w.thing.weaponTags.Contains(tag))
				select w;
			if (!source.Any<ThingStuffPair>())
			{
				return;
			}
			pawn.equipment.DestroyAllEquipment(DestroyMode.Vanish);
			ThingStuffPair thingStuffPair = source.RandomElementByWeight((ThingStuffPair w) => w.Commonality * w.Price);
			ThingWithComps thingWithComps = (ThingWithComps)ThingMaker.MakeThing(thingStuffPair.thing, thingStuffPair.stuff);
			PawnGenerator.PostProcessGeneratedGear(thingWithComps, pawn);
			pawn.equipment.AddEquipment(thingWithComps);
		}
		public static bool IsDerpWeapon(ThingDef thing, ThingDef stuff)
		{
			if (stuff == null)
			{
				return false;
			}
			if (thing.IsMeleeWeapon)
			{
				if (thing.Verbs.NullOrEmpty<VerbProperties>())
				{
					return false;
				}
				DamageArmorCategory armorCategory = thing.Verbs[0].meleeDamageDef.armorCategory;
				if (armorCategory == DamageArmorCategory.Sharp && stuff.GetStatValueAbstract(StatDefOf.SharpDamageMultiplier, null) < 0.7f)
				{
					return true;
				}
				if (armorCategory == DamageArmorCategory.Blunt && stuff.GetStatValueAbstract(StatDefOf.BluntDamageMultiplier, null) < 0.7f)
				{
					return true;
				}
			}
			return false;
		}
		internal static void LogGenerationData()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("All potential weapons:");
			foreach (ThingStuffPair current in PawnWeaponGenerator.potentialWeapons)
			{
				stringBuilder.Append(current.ToString());
				if (PawnWeaponGenerator.IsDerpWeapon(current.thing, current.stuff))
				{
					stringBuilder.Append(" - (DERP)");
				}
				stringBuilder.AppendLine();
			}
			Log.Message(stringBuilder.ToString());
		}
	}
}
