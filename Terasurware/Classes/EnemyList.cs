using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyList : ScriptableObject
{	
	public List<Param> param = new List<Param> ();

	[System.SerializableAttribute]
	public class Param
	{
		public string Name;
		public int Level;
		public int HP;
		public int MP;
		public int Attack;
        public int MagicAttack;
		public int Defence;
		public int Speed;
		public int Luck;
		public float AnimMax;
		public int Exp;
		public string Drop;
        public float MoveTime;
        public float MoveDistance;
        public string WeaponTagObjName;
        public int Weak;
        public int Bst;
    }
}