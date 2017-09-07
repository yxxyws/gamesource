using System;
using Verse;
namespace RimWorld
{
	public static class DamageDefOf
	{
		public static DamageDef Cut;
		public static DamageDef Crush;
		public static DamageDef Arrow;
		public static DamageDef Blunt;
		public static DamageDef Stab;
		public static DamageDef Bullet;
		public static DamageDef Bomb;
		public static DamageDef Scratch;
		public static DamageDef Bite;
		public static DamageDef Flame;
		public static DamageDef SurgicalCut;
		public static DamageDef Frostbite;
		public static DamageDef Stun;
		public static DamageDef EMP;
		public static DamageDef Repair;
		public static DamageDef HealInjury;
		public static DamageDef HealGlobal;
		public static DamageDef RestoreBodyPart;
		public static DamageDef Extinguish;
		public static DamageDef Deterioration;
		public static DamageDef Mining;
		public static DamageDef Rotting;
		public static void RebindDefs()
		{
			DefOfHelper.BindDefsFor<DamageDef>(typeof(DamageDefOf));
		}
	}
}
