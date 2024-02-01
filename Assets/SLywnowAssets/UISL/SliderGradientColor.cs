using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SLywnow.UI
{
    [RequireComponent(typeof(Slider))]
    public class SliderGradientColor : MonoBehaviour
    {
        public Gradient gradient;
		public enum drawMode { fill, handle, both};
		public drawMode DrawMode;

		Image img;
		Image imgHandle;
		Slider slider;

		private void Start()
		{
			slider = GetComponent<Slider>();
			img = slider.fillRect.GetComponent<Image>();
			imgHandle = slider.handleRect.GetComponent<Image>();
			slider.onValueChanged.AddListener((f) => ColorUpdate(f));
			ColorUpdate(slider.value);
		}

		void ColorUpdate(float f)
		{
			if (DrawMode == drawMode.fill)
				img.color = gradient.Evaluate(f);
			else if (DrawMode == drawMode.handle)
				imgHandle.color = gradient.Evaluate(f);
			else if (DrawMode == drawMode.both)
			{
				img.color = Color.white;
				imgHandle.color = gradient.Evaluate(f);
			}
		}
	}
}