using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterList : ScriptableObject
{	
	public List<Param> param = new List<Param> ();

	[System.SerializableAttribute]
	public class Param
	{
		
		public string Name;
		public int Level;
		public int HP;
		public int MP;
		public int Constitution;
		public int Power;
		public int Attack;
		public int Defence;
		public int Speed;
		public int Luck;
	}
}