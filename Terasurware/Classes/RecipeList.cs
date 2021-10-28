using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RecipeList : ScriptableObject
{	
	public List<Param> param = new List<Param> ();

	[System.SerializableAttribute]
	public class Param
	{
		
		public string ItemName;
		public string WantMateria1;
		public string WantMateria2;
		public string WantMateria3;
	}
}