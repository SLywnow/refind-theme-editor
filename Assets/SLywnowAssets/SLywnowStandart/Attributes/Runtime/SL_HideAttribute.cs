using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SLywnow
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
	AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
	public class ShowFromEnumAttribute : PropertyAttribute
	{
		public string TargetProperty = "";
		public int targetval;
		public bool not = false;

		public ShowFromEnumAttribute(string propertyName, int checkval)
		{
			this.TargetProperty = propertyName;
			this.targetval = checkval;
		}
		public ShowFromEnumAttribute(string propertyName, int checkval, bool inverse)
		{
			this.TargetProperty = propertyName;
			this.targetval = checkval;
			this.not = inverse;
		}
	}

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
	AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
	public class ShowFromIntAttribute : PropertyAttribute
	{
		public string TargetProperty = "";
		public int targetval;
		public bool not = false;

		public ShowFromIntAttribute(string propertyName, int checkval)
		{
			this.TargetProperty = propertyName;
			this.targetval = checkval;
		}
		public ShowFromIntAttribute(string propertyName, int checkval, bool inverse)
		{
			this.TargetProperty = propertyName;
			this.targetval = checkval;
			this.not = inverse;
		}
	}

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
	AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
	public class ShowFromFloatAttribute : PropertyAttribute
	{
		public string TargetProperty = "";
		public float targetval;
		public bool not = false;

		public ShowFromFloatAttribute(string propertyName, float checkval)
		{
			this.TargetProperty = propertyName;
			this.targetval = checkval;
		}
		public ShowFromFloatAttribute(string propertyName, float checkval, bool inverse)
		{
			this.TargetProperty = propertyName;
			this.targetval = checkval;
			this.not = inverse;
		}
	}

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
	AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
	public class ShowFromStringAttribute : PropertyAttribute
	{
		public string TargetProperty = "";
		public string targetval;
		public bool not = false;

		public ShowFromStringAttribute(string propertyName, string checkval)
		{
			this.TargetProperty = propertyName;
			this.targetval = checkval;
		}
		public ShowFromStringAttribute(string propertyName, string checkval, bool inverse)
		{
			this.TargetProperty = propertyName;
			this.targetval = checkval;
			this.not = inverse;
		}
	}

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
	AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
	public class ShowFromBoolAttribute : PropertyAttribute
	{
		public string TargetProperty = "";
		public bool targetval;
		public bool not = false;

		public ShowFromBoolAttribute(string propertyName, bool checkval = true)
		{
			this.TargetProperty = propertyName;
			this.targetval = checkval;
		}
		public ShowFromBoolAttribute(string propertyName, bool checkval, bool inverse)
		{
			this.TargetProperty = propertyName;
			this.targetval = checkval;
			this.not = inverse;
		}
	}

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
	AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
	public class ShowFromObjectNotNullAttribute : PropertyAttribute
	{
		public string TargetProperty = "";
		public bool targetval;

		public ShowFromObjectNotNullAttribute(string propertyName, bool isnull = true)
		{
			this.TargetProperty = propertyName;
			this.targetval = isnull;
		}
	}

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
	AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
	public class ShowFromMultipleAttribute : PropertyAttribute
	{
		public List<string> TargetProperty = new List<string>();
		public List<string> valss = new List<string>();
		public List<string> typess = new List<string>();

		public enum mode { and, or };
		public mode Md;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="vals"></param>
		/// <param name="types">"string","int","float","bool","enum","object"</param>
		public ShowFromMultipleAttribute(string[] propertyName, string[] vals, string[] types, mode Mode)
		{
			this.TargetProperty = propertyName.ToList();
			this.valss = vals.ToList();
			this.typess = types.ToList();
			this.Md = Mode;
		}

		public ShowFromMultipleAttribute(string propertyName, string[] vals, string[] types, mode Mode)
		{
			this.valss = vals.ToList();
			this.typess = types.ToList();
			this.Md = Mode;

			List<string> pN = new List<string>();
			foreach (string v in valss)
				pN.Add(propertyName);
			this.TargetProperty = pN;
		}
		public ShowFromMultipleAttribute(string propertyName, string[] vals, string types, mode Mode)
		{
			this.valss = vals.ToList();
			this.Md = Mode;

			List<string> pN = new List<string>();
			List<string> tp = new List<string>();
			foreach (string v in valss)
			{
				pN.Add(propertyName);
				tp.Add(types);
			}
			this.TargetProperty = pN;
			this.typess = tp;
		}
		public ShowFromMultipleAttribute(string[] propertyName, string vals, string types, mode Mode)
		{
			this.TargetProperty = propertyName.ToList();
			this.Md = Mode;

			List<string> pN = new List<string>();
			List<string> tp = new List<string>();
			foreach (string v in propertyName)
			{
				pN.Add(vals);
				tp.Add(types);
			}
			this.valss = pN;
			this.typess = tp;
		}
		public ShowFromMultipleAttribute(string[] propertyName, string[] vals, string types, mode Mode)
		{
			this.TargetProperty = propertyName.ToList();
			this.valss = vals.ToList();
			this.Md = Mode;

			List<string> tp = new List<string>();
			foreach (string v in valss)
			{
				tp.Add(types);
			}
			this.typess = tp;
		}
	}
}