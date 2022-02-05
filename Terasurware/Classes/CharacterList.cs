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
		public int MaxHP;
		public int MP;
		public int MaxMP;
		public int Attack;
		public int MagicAttack;
		public int Defence;
		public int Speed;
		public int Luck;
		public float AnimMax;
		public int Exp;
        public int MaxExp;
        public int SumExp;
    }
}