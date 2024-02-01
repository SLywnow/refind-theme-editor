using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SLywnow;

namespace SLywnow.Paralax
{
    [AddComponentMenu("SLywnow/Paralax/Camera Move")]
    public class SLPX_CameraMove : MonoBehaviour
    {
        public Transform camera;
        public enum movetpe { strict, lerp};
        public movetpe moveType;
        [ShowFromEnum(nameof(moveType), 1)]
        public float speed = 0.5f;
        public Transform border;
        [ShowFromObjectNotNull(nameof(border))]
        public Vector2 borderMin;
		[ShowFromObjectNotNull(nameof(border))]
		public Vector2 borderMax;

        Vector2 defpos;
        Vector2 topos;
        Vector2 min;
        Vector2 max;

        private void Start()
        {
            defpos = camera.position;
            if (border !=null)
            {
				Vector2 borderMin;
				Vector2 borderMax;

				borderMin.x = (-(border.localScale.x / 2)) + border.position.x;
				borderMin.y = (-(border.localScale.y / 2)) + border.position.y;
				borderMax.x = (border.localScale.x / 2) + border.position.x;
				borderMax.y = (border.localScale.y / 2) + border.position.y;
            }
            min.x = borderMin.x <= borderMax.x ? borderMin.x : borderMax.x;
            min.y = borderMin.y <= borderMax.y ? borderMin.y : borderMax.y;
            max.x = borderMin.x > borderMax.x ? borderMin.x : borderMax.x;
			max.y = borderMin.y > borderMax.y ? borderMin.y : borderMax.y;
		}

        void Update()
        {
			//x - (Input.mousePosition.x / Screen.width)
			//y - (Input.mousePosition.y / Screen.height)

			Vector2 outv2 = new Vector2(Mathf.Lerp(min.x,max.x, (Input.mousePosition.x / Screen.width)) , Mathf.Lerp(min.y, max.y, (Input.mousePosition.y / Screen.height)));

            if (moveType == movetpe.strict)
                camera.localPosition = outv2;
            else if (moveType==movetpe.lerp)
            {
                topos = outv2;
                camera.localPosition = Vector2.Lerp(camera.localPosition, topos, speed * Time.deltaTime);
			}
        }
    }
}