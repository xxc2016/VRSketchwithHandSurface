using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRPainting
{
    public class DeleteDemo : MonoBehaviour
    {
        public GlobalState state;

        private void Start()
        {
            state = this.GetComponent<GlobalState>();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

