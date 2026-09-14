using R2API.Networking.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class Components
{
    // Set target and gameobject will follow
    public class FollowTarget : MonoBehaviour
    {
        public Transform target;
        public Vector3 offset = Vector3.zero;
        public bool followRotation = false;

        private void FixedUpdate()
        {
            if (!target) return;
            //Debug.Log(transform.gameObject.name + "following " + target.gameObject.name + "\n" + target.position + "\n" + transform.position);
            transform.position = target.position + offset;

            if (followRotation)
            {
                transform.rotation = target.rotation;
            }
        }
    }

    // Set mesh reference, change text when neeeded
    public class TextController : NetworkBehaviour
    {
        public TextMeshPro textMesh;

        public void SetText(string newText)
        {
            textMesh.text = newText;
        }

    }
    
    // This is an evil proof of conept that is horrid
    public class Timer : MonoBehaviour
    {
        float timer = 0;
        bool timerActive = false;

        public bool CallTimer(float interval)
        {
            timer = interval;
            timerActive = true;
            while (timer > 0)
            {
                Debug.Log("Timer: " + timer); 
            }
            return true;
        }
        private void Update()
        {
            if (timerActive)
            {
                timer -= Time.deltaTime;
            }
        }
    }
}
