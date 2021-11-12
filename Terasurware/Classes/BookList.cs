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
		public string Word;
		public string AddNum;
		public int Price;
		public string Info;
	}
}