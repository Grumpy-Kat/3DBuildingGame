using UnityEngine;
using System.Collections.Generic;

public static class PoolingSystem {
	
	const int DEFAULT_POOL_SIZE = 3;
	
	class Pool{
		//allows object to be found in heirarchy; to be applied to anything instantiated
		int nextID = 1;
		//holds inactive GameObjects
		//uses Stack instead of List because never need to grab something from the middle, only the end
		Stack<GameObject> inactive;
		//prefab to be pooled
		GameObject prefab;
		
		/// <summary>
		/// initializes a new instance of the <see cref="PoolingSystem+Pool"/> class (constructer)
		/// </summary>
		/// <param name="prefab">prefab to be pooled</param>
		/// <param name="initialQty">initial quantity of that prefab to be generated</param>
		public Pool(GameObject prefab, int initialQty){
			this.prefab = prefab;
			//adds the initial quantity of preloaded prefabs to the Stack
			inactive = new Stack<GameObject>(initialQty);
		}
		
		/// <summary>
		/// spawns the defined prefab at the specified pos and rot
		/// </summary>
		/// <param name="pos">position</param>
		/// <param name="rot">rotation</param>
		public GameObject Spawn(Vector3 pos,Quaternion rot){
			GameObject obj;
			if(inactive.Count == 0){
				//no objects in pool; must instantiate new object
				obj = (GameObject)GameObject.Instantiate(prefab,pos,rot);
				obj.name = prefab.name + " (" + (nextID++) + ")";
				//tells us which pool this can from
				obj.AddComponent<PoolMember>().pool = this;
			} else {
				//grab the latest object from Stack
				obj = inactive.Pop();
				if(obj == null){
					//object no longer exists
					//try next one is Stack
					return Spawn (pos,rot);
				}
			}
			obj.transform.position = pos;
			obj.transform.rotation = rot;
			obj.SetActive(true);
			return obj;
		}
		
		/// <summary>
		/// Return the specified object to pool
		/// </summary>
		/// <param name="obj">object to despawn</param>
		public void Despawn(GameObject obj){
			obj.SetActive(false);
			//return to pool
			inactive.Push(obj);
		}
	}
	
	class PoolMember : MonoBehaviour{
		public Pool pool;
	}
	
	static Dictionary<GameObject,Pool> pools;
	
	/// <summary>
	/// initialize the Dictionary
	/// </summary>
	/// <param name="prefab">prefab to be added to Dictionary</param>
	/// <param name="qty">quantity of that be prefab to instantiated</param>
	static void Init(GameObject prefab = null, int qty = DEFAULT_POOL_SIZE){
		if(pools == null){
			//initialize new dictionary
			pools = new Dictionary<GameObject,Pool>();
		}
		if(prefab != null && pools.ContainsKey(prefab) == false){
			pools[prefab] = new Pool(prefab,qty);
		}
	}
	
	/// <summary>
	/// preload the specified qty of the prefab into the pool
	/// </summary>
	/// <param name="prefab">prefab to be added</param>
	/// <param name="qty">quantity of the prefab to be added</param>
	public static void Preload(GameObject prefab,int qty = 1){
		Init (prefab,qty);
		GameObject[] objs = new GameObject[qty];
		for (int i = 0; i < qty; i++) {
			objs[i] = Spawn(prefab,Vector3.zero,Quaternion.identity);
		}
		for (int i = 0; i < qty; i++) {
			Despawn(objs[i]);
		}
	}
	
	/// <summary>
	/// spawn the specified prefab at pos and rot (only instantiating if needed)
	/// </summary>
	/// <param name="prefab">prefab to spawn</param>
	/// <param name="pos">position to spawn the prefab at</param>
	/// <param name="rot">rotation to spawn the prefab at</param>
	public static GameObject Spawn(GameObject prefab,Vector3 pos,Quaternion rot){
		Init (prefab);
		return pools[prefab].Spawn(pos,rot);
	}
	
	/// <summary>
	/// despawn the specified obj and return to pool
	/// </summary>
	/// <param name="obj">object to be despawned</param>
	public static void Despawn(GameObject obj){
		PoolMember pm = obj.GetComponent<PoolMember>();
		if(pm == null){
			Debug.Log ("Object '" + obj.name + "' wasn't spawned from a pool. Destroying object.");
			GameObject.Destroy(obj);
		} else {
			pm.pool.Despawn(obj);
		}
	}
}