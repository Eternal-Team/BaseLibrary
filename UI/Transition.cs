// namespace BaseLibrary.UI
// {
// 	public class Transition
// 	{
// 		private float start;
// 		private float end;
// 		public float Rate;
// 		public float Value { get; private set; }
//
// 		public float Start
// 		{
// 			get => start;
// 			set
// 			{
// 				start = value;
// 				if (Value < start) Value = start;
// 			}
// 		}
//
// 		public float End
// 		{
// 			get => end;
// 			set
// 			{
// 				end = value;
// 				if (Value > end) Value = end;
// 			}
// 		}
// 		
// 		public Transition(float start, float end, float rate)
// 		{
// 			this.start = start;
// 			this.end = end;
// 			this.Rate = rate;
// 		}
//
// 		public void Update(bool trigger)
// 		{
// 			if (trigger && Value < end) Value += Rate;
// 			else if (!trigger && Value > start) Value -= Rate;
// 		}
//
// 		public static implicit operator float(Transition transition) => transition.Value;
// 	}
// }