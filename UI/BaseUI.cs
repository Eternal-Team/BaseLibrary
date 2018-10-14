using BaseLibrary.UI.Elements;

namespace TheOneLibrary.Base.UI
{
	public abstract class BaseUI : BaseState
	{
		public UIDraggablePanel panelMain;

		public override void OnInitialize()
		{
			Initialize();
			panelMain.SetPadding(0f);
			Append(panelMain);
		}

		public new virtual void Initialize()
		{
		}
	}
}