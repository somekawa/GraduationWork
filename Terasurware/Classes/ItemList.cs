using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemList : ScriptableObject
{	
	public List<Param> param = new List<Param> ();

	[System.SerializableAttribute]
	public class Param
	{		
		public string ItemName;
		public int ChapterNumber;
	}
}