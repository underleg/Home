using System.Collections;
using UnityEngine;

    public class GlobalsMB : MonoBehaviour
    {
        private static GlobalsMB instance;
        public static GlobalsMB Instance { get { return instance; } }
        
        public LayerMask movingCollisionObjects;



        // Use this for initialization
        void Awake()
        {
            instance = this;
        }

        // Update is called once per frame
        void Update()
        {

        }
    
}