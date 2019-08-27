using BaseLibrary.UI.Elements;
using Terraria;

namespace BaseLibrary.UI
{
	public abstract class BaseUI : BaseState
	{
		public UIDraggablePanel panelMain;

		public override void OnInitialize()
		{
			Initialize();
			Append(panelMain);
		}

		public new virtual void Initialize()
		{
		}
	}
}