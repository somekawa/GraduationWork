using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MateriaList : ScriptableObject
{	
	public List<Param> param = new List<Param> ();

	[System.SerializableAttribute]
	public class Param
	{
		
		public string MateriaName;
		public int Price_Buy;
		public int Price_Sell;
		public string ImageName;
		public string Explanation;
	}
}