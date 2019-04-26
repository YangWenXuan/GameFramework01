using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    public class Client : MonoBehaviour
    {
        public static Client Ins{get;private set;}

        public static Client Create()
        {
            var go=new GameObject("Client");
            DontDestroyOnLoad(go);
            Ins=go.AddComponent<Client>();
            return Ins;
        }
        
        void Start()
        {
            
        }

        void Update()
        {
            
        }
    }
}
