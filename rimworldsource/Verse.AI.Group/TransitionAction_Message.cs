using System;
using System.Linq;
namespace Verse.AI.Group
{
	public class TransitionAction_Message : TransitionAction
	{
		public string message;
		public MessageSound sound;
		public TargetInfo lookTarget;
		public TransitionAction_Message(string message) : this(message, MessageSound.Standard)
		{
			this.message = message;
		}
		public TransitionAction_Message(string message, MessageSound messageSound)
		{
			this.lookTarget = TargetInfo.Invalid;
			base..ctor();
			this.message = message;
			this.sound = messageSound;
		}
		public TransitionAction_Message(string message, MessageSound messageSound, TargetInfo lookTarget)
		{
			this.lookTarget = TargetInfo.Invalid;
			base..ctor();
			this.message = message;
			this.sound = messageSound;
			this.lookTarget = lookTarget;
		}
		public override void DoAction(Transition trans)
		{
			TargetInfo targetInfo;
			if (this.lookTarget.IsValid)
			{
				targetInfo = this.lookTarget;
			}
			else
			{
				targetInfo = trans.target.lord.ownedPawns.FirstOrDefault<Pawn>();
			}
			Messages.Message(this.message, targetInfo, this.sound);
		}
	}
}
