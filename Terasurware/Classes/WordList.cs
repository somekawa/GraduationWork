using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WordList : ScriptableObject
{	
	public List<Param> param = new List<Param> ();

	[System.SerializableAttribute]
	public class Param
	{
		
		public string Word;
		public string EnglishWord;
		public int ListNumber;
		public int KindsNumber;
		public int Power;
	}
}