using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Verse;
namespace RimWorld
{
	public class PawnGroupMaker_Normal : PawnGroupMaker
	{
		public List<PawnGenOption> options = new List<PawnGenOption>();
		public override float MinPointsToGenerateAnything
		{
			get
			{
				return this.options.Min((PawnGenOption g) => g.Cost);
			}
		}
		public override bool CanGenerateFrom(IncidentParms parms)
		{
			return !parms.traderCaravan && base.CanGenerateFrom(parms) && PawnGroupMakerUtility.ChoosePawnGenOptionsByPoints(parms.points, this.options, parms).Any<PawnGenOption>();
		}
		[DebuggerHidden]
		public override IEnumerable<Pawn> GenerateArrivingPawns(IncidentParms parms, bool errorOnZeroResults = true)
		{
			PawnGroupMaker_Normal.<GenerateArrivingPawns>c__IteratorAF <GenerateArrivingPawns>c__IteratorAF = new PawnGroupMaker_Normal.<GenerateArrivingPawns>c__IteratorAF();
			<GenerateArrivingPawns>c__IteratorAF.parms = parms;
			<GenerateArrivingPawns>c__IteratorAF.errorOnZeroResults = errorOnZeroResults;
			<GenerateArrivingPawns>c__IteratorAF.<$>parms = parms;
			<GenerateArrivingPawns>c__IteratorAF.<$>errorOnZeroResults = errorOnZeroResults;
			<GenerateArrivingPawns>c__IteratorAF.<>f__this = this;
			PawnGroupMaker_Normal.<GenerateArrivingPawns>c__IteratorAF expr_2A = <GenerateArrivingPawns>c__IteratorAF;
			expr_2A.$PC = -2;
			return expr_2A;
		}
	}
}
