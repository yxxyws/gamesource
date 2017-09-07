using System;
using System.Collections.Generic;
namespace Verse
{
	public class Genstep_RandomSelector : Genstep
	{
		public List<RandomGenstepSelectorOption> options = new List<RandomGenstepSelectorOption>();
		public override void Generate()
		{
			this.options.RandomElementByWeight((RandomGenstepSelectorOption opt) => opt.weight).genstep.Generate();
		}
	}
}
