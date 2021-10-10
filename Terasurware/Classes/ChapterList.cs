using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChapterList : ScriptableObject
{	
	public List<Param> param = new List<Param> ();

	[System.SerializableAttribute]
	public class Param
	{
		public string name1;
		public string name2;
		public string face;
		public string message;
        public int eventNum;
	}
}