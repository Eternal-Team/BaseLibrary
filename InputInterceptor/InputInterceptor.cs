using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using BaseLibrary.Utility;
using Microsoft.Xna.Framework;
using Terraria;

namespace BaseLibrary.InputInterceptor
{
	public static class InputInterceptor
	{
		public static Func<bool> InterceptInput = () => false;

		private const int GWL_WNDPROC = -4;

		private static IntPtr oldWndProc;
		private static DllImports.WndProc WndProc;

		public static Dictionary<WindowMessageFlags, Func<uint, IntPtr, IntPtr, int>> Hooks = new Dictionary<WindowMessageFlags, Func<uint, IntPtr, IntPtr, int>>();

		public static void RegisterHook(Func<uint, IntPtr, IntPtr, int> func, params WindowMessageFlags[] msg)
		{
			foreach (WindowMessageFlags flags in msg) Hooks.Add(flags, func);
		}

		private static double time;

		// use structs instead of nameless arguments?
		public static Action<int, int, int> OnMouseMove;
		public static Action<short, short> OnMouseWheel;

		public static Action<int, int> OnLeftMouseDown;
		public static Action<int, int> OnLeftMouseUp;
		public static Action<int, int> OnRightMouseDown;
		public static Action<int, int> OnRightMouseUp;
		public static Action<int, int> OnMiddleMouseDown;
		public static Action<int, int> OnMiddleMouseUp;
		public static Action<int, int, int> OnXMouseDown;
		public static Action<int, int, int> OnXMouseUp;

		public static Action<int, bool> OnKeyDown;
		public static Action<int, bool> OnKeyUp;
		public static Action<int> OnKeyChar;

		private static int leftClickCount = 1;
		private static double leftClickTime;

		private static int rightClickCount = 1;
		private static double rightClickTime;

		private static int middleClickCount = 1;
		private static double middleClickTime;

		private static int xClickCount = 1;
		private static double xClickTime;

		internal static void Load()
		{
			if (Main.dedServ) return;

			Main.OnPostDraw += UpdateTime;

			WndProc = (hWnd, msg, wParam, lParam) =>
			{
				if (InterceptInput() && Hooks.TryGetValue((WindowMessageFlags)msg, out Func<uint, IntPtr, IntPtr, int> func)) return (IntPtr)func.Invoke(msg, wParam, lParam);

				return DllImports.CallWindowProc(oldWndProc, hWnd, msg, wParam, lParam);
			};

			Scheduler.EnqueueMessage(() => oldWndProc = (IntPtr)DllImports.SetWindowLong(Main.instance.Window.Handle, GWL_WNDPROC, (uint)Marshal.GetFunctionPointerForDelegate(WndProc)));

			RegisterHook((msg, wParam, lParam) => 4, WindowMessageFlags.WM_GETDLGCODE);

			RegisterHook((msg, wParam, lParam) =>
			{
				(short x, short y) = lParam.ToOrder();
				OnMouseMove?.Invoke(x, y, wParam.ToInt32());
				return 0;
			}, WindowMessageFlags.WM_MOUSEMOVE);
			RegisterHook((msg, wParam, lParam) =>
			{
				(short modifiers, short delta) = wParam.ToOrder();
				OnMouseWheel?.Invoke(delta, modifiers);
				return 0;
			}, WindowMessageFlags.WM_MOUSEWHEEL);

			RegisterHook((msg, wParam, lParam) =>
			{
				if (time - leftClickTime < 500) leftClickCount++;
				else leftClickCount = 1;

				leftClickTime = time;

				OnLeftMouseDown?.Invoke(leftClickCount, wParam.ToInt32());

				return 0;
			}, WindowMessageFlags.WM_LBUTTONDOWN, WindowMessageFlags.WM_LBUTTONDBLCLK);
			RegisterHook((msg, wParam, lParam) =>
			{
				OnLeftMouseUp?.Invoke(leftClickCount, wParam.ToInt32());

				return 0;
			}, WindowMessageFlags.WM_LBUTTONUP);
			RegisterHook((msg, wParam, lParam) =>
			{
				if (time - rightClickTime < 500) rightClickCount++;
				else rightClickCount = 1;

				rightClickTime = time;

				OnRightMouseDown?.Invoke(rightClickCount, wParam.ToInt32());

				return 0;
			}, WindowMessageFlags.WM_RBUTTONDOWN, WindowMessageFlags.WM_RBUTTONDBLCLK);
			RegisterHook((msg, wParam, lParam) =>
			{
				OnRightMouseUp?.Invoke(rightClickCount, wParam.ToInt32());

				return 0;
			}, WindowMessageFlags.WM_RBUTTONUP);
			RegisterHook((msg, wParam, lParam) =>
			{
				if (time - middleClickTime < 500) middleClickCount++;
				else middleClickCount = 1;

				middleClickTime = time;

				OnMiddleMouseDown?.Invoke(middleClickCount, wParam.ToInt32());

				return 0;
			}, WindowMessageFlags.WM_MBUTTONDOWN, WindowMessageFlags.WM_MBUTTONDBLCLK);
			RegisterHook((msg, wParam, lParam) =>
			{
				OnMiddleMouseUp?.Invoke(middleClickCount, wParam.ToInt32());

				return 0;
			}, WindowMessageFlags.WM_MBUTTONUP);
			RegisterHook((msg, wParam, lParam) =>
			{
				(short modifiers, short button) = wParam.ToOrder();

				if (time - xClickTime < 500) xClickCount++;
				else xClickCount = 1;

				xClickTime = time;

				OnXMouseDown?.Invoke(button, xClickCount, modifiers);

				return 0;
			}, WindowMessageFlags.WM_XBUTTONDOWN, WindowMessageFlags.WM_XBUTTONDBLCLK);
			RegisterHook((msg, wParam, lParam) =>
			{
				(short modifiers, short button) = wParam.ToOrder();

				OnXMouseUp?.Invoke(button, xClickCount, modifiers);

				return 0;
			}, WindowMessageFlags.WM_XBUTTONUP);

			RegisterHook((msg, wParam, lParam) =>
			{
				OnKeyDown?.Invoke(wParam.ToInt32(), msg == (uint)WindowMessageFlags.WM_SYSKEYDOWN);

				return 0;
			}, WindowMessageFlags.WM_KEYDOWN, WindowMessageFlags.WM_SYSKEYDOWN);
			RegisterHook((msg, wParam, lParam) =>
			{
				OnKeyUp?.Invoke(wParam.ToInt32(), msg == (uint)WindowMessageFlags.WM_SYSKEYUP);

				return 0;
			}, WindowMessageFlags.WM_KEYUP, WindowMessageFlags.WM_SYSKEYUP);
			RegisterHook((msg, wParam, lParam) =>
			{
				OnKeyChar?.Invoke(wParam.ToInt32());

				return 0;
			}, WindowMessageFlags.WM_CHAR);
		}

		internal static void Unload()
		{
			if (Main.dedServ) return;

			Scheduler.EnqueueMessage(() => DllImports.SetWindowLong(Main.instance.Window.Handle, GWL_WNDPROC, (uint)oldWndProc));

			Main.OnPostDraw -= UpdateTime;
		}

		private static void UpdateTime(GameTime gameTime)
		{
			time = gameTime.TotalGameTime.TotalMilliseconds;
		}
	}
}