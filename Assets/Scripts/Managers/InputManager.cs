using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;
using Utility.Patterns;

namespace CustomInput
{
    public class InputManager : Singleton<InputManager>
    {
        public Vector2 Directional { get; private set; }
        public bool Shoot { get => Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.Space); }
        public bool Shield { get => Input.GetKeyDown(KeyCode.K) || Input.GetKeyDown(KeyCode.LeftShift); }

        Axis horizontal = new Axis(KeyCode.A, KeyCode.D);
        public Axis Horizontal { get => horizontal; }

        Axis vertical = new Axis(KeyCode.S, KeyCode.W);
        public Axis Vertical { get => vertical; }

        private void Update()
        {
            horizontal.Update();
            vertical.Update();

            //Directional = UVector.New(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
            Directional = UVector.New(horizontal.GetValue(), vertical.GetValue()).normalized;
        }
    }

    public class Axis
    {
        KeyCode min;
        KeyCode max;

        public Axis(KeyCode min, KeyCode max)
        {
            this.min = min;
            this.max = max;
        }

        private float oldValue = 0f;
        private float currentValue = 0f;
        public bool HasChanged { get => oldValue != currentValue; }

        internal void Update()
        {
            oldValue = currentValue;
            currentValue = (Input.GetKey(min) ? -1f : 0f) + (Input.GetKey(max) ? 1f : 0f);
        }

        public float GetValue()
        {
            return currentValue;
        }
    }
}
