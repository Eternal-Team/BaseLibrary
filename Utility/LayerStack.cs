using System.Collections;
using System.Collections.Generic;

namespace BaseLibrary
{
	public class LayerStack : IEnumerable<Layer>
	{
		public IEnumerator<Layer> GetEnumerator()
		{
			for (int i = layers.Count - 1; i >= 0; i--)
			{
				if (!layers[i].Enabled) continue;
				yield return layers[i];
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		private int layerInsertIndex;
		private List<Layer> layers;

		public LayerStack() => layers = new List<Layer>();

		public void PushLayer(Layer layer)
		{
			layers.Insert(layerInsertIndex++, layer);
			//layer.OnAttach();
		}

		public void PushOverlay(Layer layer)
		{
			layers.Add(layer);
			//layer.OnAttach();
		}

		public void PopLayer(Layer layer)
		{
			int index = layers.IndexOf(layer);
			if (index >= 0 && index < layerInsertIndex)
			{
				//layer.OnDetach();
				layers.Remove(layer);
				layerInsertIndex--;
			}
		}

		public void PopOverlay(Layer layer)
		{
			int index = layers.IndexOf(layer);
			if (index >= layerInsertIndex && index < layers.Count)
			{
				//layer.OnDetach();
				layers.Remove(layer);
			}
		}
	}
}