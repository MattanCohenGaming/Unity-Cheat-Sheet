using System;
using UnityEngine;

namespace MCG.UnityCheatSheet
{
    [Serializable]
    public class AnyClass
    {
        /**
            this class can contain anything you need to serialize
    
            NO NESTED SERIALIZATION YET!
        */

        public int someInt; // example
        public string someString; // example
    }

    // this class is just so you can use SerializedClass in your code. creating an object that is editable
    // from the editor on another script will show on the editor view :)
    [Serializable]
    public class SerializedClass : AnyClass, ISerializationCallbackReceiver
    {
        public void OnBeforeSerialize()
        {
            /*
                things in here will happen before every change in the editor.
            */

            // // give it a try:
            // Debug.Log("OnBeforeSerialize ");
        }

        public void OnAfterDeserialize()
        {
            /*
                things in here will happen after every change in the editor.
            */

            // // give it a try:
            // Debug.Log("OnAfterDeserialize ");
        }
    }
}
