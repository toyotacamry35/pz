using UnityEngine;

namespace Assets.Src.Doll
{
    public class MarkerOnWeapon : MonoBehaviour
    {
        [SerializeField] private Identifier _id;
        
        public enum Identifier
        {
            Hand, // Место которым оружие крепится к руке 
            Spine, // Место которым оружие крепится к слоту
            Shot,  // Место к которому крепится FX выстрела  
            Muzzle // Место из которого вылетает прожектайл
        }

        public Identifier Id => _id;
    }
}
