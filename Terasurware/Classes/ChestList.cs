using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChestList : ScriptableObject
{	
	public List<Param> param = new List<Param> ();

	[System.SerializableAttribute]
	public class Param
	{
		
		public int num1;
		public int num2;
		public string getItem;
	}
}