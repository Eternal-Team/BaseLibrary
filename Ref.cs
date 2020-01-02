namespace BaseLibrary
{
	public class Ref<T>
	{
		public T Value;

		public Ref(T value) => Value = value;

		public override string ToString() => Value?.ToString() ?? "";
	}
}