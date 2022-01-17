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
        public string detail;
		public int clear;
        public int eventNum;
        public string type;
        public string reward;
        public int questType;
        public string delivery;
        public int money;
        public string materia;
        public string item;
    }
}