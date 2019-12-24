using BaseLibrary.UI.Elements;
using System;
using Terraria.Audio;

namespace BaseLibrary.UI
{
	public interface IHasUI
	{
		Guid UUID { get; set; }

		BaseUIPanel UI { get; set; }

		LegacySoundStyle CloseSound { get; }

		LegacySoundStyle OpenSound { get; }
	}

	public class BaseUIPanel : UIDraggablePanel
	{
		public IHasUI Container;
	}

	public class BaseUIPanel<T> : BaseUIPanel where T : IHasUI
	{
		public new T Container => (T)base.Container;
	}
}