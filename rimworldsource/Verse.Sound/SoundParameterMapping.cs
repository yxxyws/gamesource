using System;
namespace Verse.Sound
{
	public class SoundParameterMapping
	{
		[Description("The independent parameter that the game will change to drive this relationship.\n\nOn the graph, this is the X axis.")]
		public SoundParamSource inParam;
		[Description("The dependent parameter that will respond to changes to the in-parameter.\n\nThis must match something the game can change about this sound.\n\nOn the graph, this is the y-axis.")]
		public SoundParamTarget outParam;
		[Description("Determines when sound parameters should be applies to samples.\n\nConstant means the parameters are updated every frame and can change continuously.\n\nOncePerSample means that the parameters are applied exactly once to each sample that plays.")]
		public SoundParamUpdateMode paramUpdateMode;
		[EditorHidden]
		public SimpleCurve curve;
		public SoundParameterMapping()
		{
			this.curve = new SimpleCurve();
			this.curve.Add(new CurvePoint(0f, 0f));
			this.curve.Add(new CurvePoint(1f, 1f));
		}
		public void DoEditWidgets(WidgetRow widgetRow)
		{
			string title = ((this.inParam == null) ? "null" : this.inParam.Label) + " -> " + ((this.outParam == null) ? "null" : this.outParam.Label);
			if (widgetRow.DoTextButton("Edit curve", "Edit the curve mapping the in parameter to the out parameter.", true, false))
			{
				Find.WindowStack.Add(new EditWindow_CurveEditor(this.curve, title));
			}
		}
		public void Apply(Sample samp)
		{
			if (this.inParam == null || this.outParam == null)
			{
				return;
			}
			float num = this.inParam.ValueFor(samp);
			float value = this.curve.Evaluate(num);
			this.outParam.SetOn(samp, value);
			if (Game.isDebugBuild && this.curve.HasView)
			{
				this.curve.View.SetDebugInput(samp, num);
			}
		}
	}
}
