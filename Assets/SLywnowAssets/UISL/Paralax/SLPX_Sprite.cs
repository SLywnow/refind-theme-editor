using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SLywnow.Paralax
{
	[AddComponentMenu("SLywnow/Paralax/Sprite")]
	public class SLPX_Sprite : MonoBehaviour
	{
		public Camera camera;
		public bool dontMove;
		[ShowFromBool("dontMove", false)]
		public bool X;
		[ShowFromBool("X")]
		public float speedX = 0;
		[ShowFromBool("X")]
		public bool loopX;
		[ShowFromBool("dontMove", false)]
		public bool Y;
		[ShowFromBool("Y")]
		public float speedY = 0;
		[ShowFromBool("Y")]
		public bool loopY;

		[HideInInspector] 
		public bool spawned;
		[HideInInspector] 
		public Vector2 size;
		[HideInInspector]
		public Vector3 defpos;

		SpriteRenderer sr;
		RectTransform rtr;
		bool enable;

		public void Start()
		{
			if (!spawned)
			{
				if (camera == null)
					camera = Camera.main;
				defpos = transform.position;

				sr = GetComponent<SpriteRenderer>();

				if (sr != null)
					size = sr.bounds.size;
				else
				{
					rtr = (RectTransform)transform;
					size = rtr.sizeDelta;
				}

				if (!dontMove)
				{
					if ( (X && loopX) && (!loopY || !Y))
					{
						Transform boofer = Instantiate(gameObject, transform).transform;
						for (int x=-1;x<2;x++)
						{
							if (x != 0)
							{
								Transform tr = Instantiate(boofer, transform);
								tr.gameObject.GetComponent<SLPX_Sprite>().spawned = true;
								tr.localPosition = new Vector2(size.x * x, 0);
								tr.gameObject.name = gameObject.name + " paralax";
							}
						}
						Destroy(boofer.gameObject);
					}
					else if ((Y && loopY) && (!loopX || !X))
					{
						Transform boofer = Instantiate(gameObject, transform).transform;
						for (int y = -1; y < 2; y++)
						{
							if (y != 0)
							{
								Transform tr = Instantiate(boofer, transform);
								tr.gameObject.GetComponent<SLPX_Sprite>().spawned = true;
								tr.localPosition = new Vector2(0, size.y * y);
								tr.gameObject.name = gameObject.name + " paralax";
							}
						}
						Destroy(boofer.gameObject);
					}
					else if ((Y && loopY) && (X && loopX))
					{
						Transform boofer = Instantiate(gameObject, transform).transform;
						for (int x = -1; x < 2; x++)
						{
							for (int y = -1; y < 2; y++)
							{
								if (!(y == 0 && x == 0))
								{
									Transform tr = Instantiate(boofer, transform);
									tr.gameObject.GetComponent<SLPX_Sprite>().spawned = true;
									tr.localPosition = new Vector2(size.x * x, size.y * y);
									tr.gameObject.name = gameObject.name + " paralax";
								}
							}
						}
						Destroy(boofer.gameObject);
					}
				}

				enable = true;
			}
			else
				Destroy(this);
		}

		[HideInInspector]
		public Vector3 pos;
		private void FixedUpdate()
		{
			if (enable)
			{
				if (!dontMove)
				{
					pos = defpos;
					if (X)
					{
						float offset = camera.transform.position.x * (1 - speedX);
						float dist = camera.transform.position.x * speedX;

						if (speedX == 1)
							pos.x = camera.transform.position.x;
						else
							pos.x = defpos.x + dist;

						if (loopX)
						{
							if (offset > defpos.x + size.x) defpos.x += size.x;
							else if (offset < defpos.x - size.x) defpos.x -= size.x;
						}
					}
					if (Y)
					{
						float offset = camera.transform.position.y * (1 - speedY);
						float dist = camera.transform.position.y * speedY;

						if (speedY == 1)
							pos.y = camera.transform.position.y;
						else
							pos.y = defpos.y + dist;

						if (loopY)
						{
							if (offset > defpos.y + size.y) defpos.y += size.y;
							else if (offset < defpos.y - size.y) defpos.y -= size.y;
						}
					}
				}
				else
					pos = new Vector3(camera.transform.position.x, camera.transform.position.y, defpos.z);

				transform.position = pos;
			}
		}

		public void Disable()
		{
			enable = false;
			transform.position = defpos;
		}

		public void Enable()
		{
			Start();
		}
	}
}