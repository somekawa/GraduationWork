using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuestInfo : ScriptableObject
{	
	public List<Param> param = new List<Param> ();

	[System.SerializableAttribute]
	public class Param
	{
		public int num;
		public string info;
		public int clear;
        public int eventNum;
        public string type;
	}
}