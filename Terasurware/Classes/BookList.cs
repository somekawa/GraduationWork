using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BookList : ScriptableObject
{	
	public List<Param> param = new List<Param> ();

	[System.SerializableAttribute]
	public class Param
	{
		
		public string BookName;
		public int WordNumber;
		public string GetCheck;
		public int Price;
		public int ImageNumber;
		public string Info;
	}
}