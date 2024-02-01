using SLywnow;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SLywnow.UI
{
   [ExecuteInEditMode]
   public class AutoSize : MonoBehaviour
   {
      public RectTransform target;
      public enum tpe { x, y, both };
      public tpe mode;
      [ShowFromEnum(nameof(mode), 1, true)]
      public float minX = 0;
      [ShowFromEnum(nameof(mode), 0, true)]
      public float minY = 0;
      [ShowFromEnum(nameof(mode), 1, true)]
      public float maxX = -1;
      [ShowFromEnum(nameof(mode), 0, true)]
      public float maxY = -1;
      [ShowFromEnum(nameof(mode), 1, true)]
      public float offsetX;
      [ShowFromEnum(nameof(mode), 0, true)]
      public float offsetY;

      RectTransform me;

      void Start()
      {
         me = GetComponent<RectTransform>();
      }

      void Update()
      {
         if (me != null && target != null)
         {
            Vector2 changed = me.sizeDelta;
            Vector2 main = target.sizeDelta;

            //x
            if (mode == tpe.x || mode == tpe.both)
            {
               changed.x = Mathf.Clamp(main.x + offsetX, minX, maxX == -1 ? float.PositiveInfinity : maxX);
            }
            //y
            if (mode == tpe.y || mode == tpe.both)
            {
               changed.y = Mathf.Clamp(main.y + offsetY, minY, maxY == -1 ? float.PositiveInfinity : maxY);
            }

            me.sizeDelta = changed;
         }
      }
   }
}