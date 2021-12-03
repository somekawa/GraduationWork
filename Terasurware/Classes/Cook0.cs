using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cook0 : ScriptableObject
{	
	public List<Param> param = new List<Param> ();

	[System.SerializableAttribute]
	public class Param
	{
		
		public int num;
		public string name;
		public string info;
		public string statusUp;
		public int needFood;
        public int needNum;
		public int needMoney;
		public int eventNum;
	}
}