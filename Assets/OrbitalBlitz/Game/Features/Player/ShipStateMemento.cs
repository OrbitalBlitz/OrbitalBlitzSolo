using System;
using System.Collections.Generic;
using OrbitalBlitz.Game.Features.Ship.Controllers;
using UnityEngine;

namespace OrbitalBlitz.Game.Features.Player {
    public class ShipStateMemento {
        public struct ShipPhysicsState {
            public Vector3 Position;
            public Quaternion Rotation;
            public Vector3 Velocity;
            public Vector3 AngularVelocity;
        }

        private Rigidbody _rb;
        private AbstractShipController _controller;
        private GameObject _ship;

        private Vector3 initialPosition;
        private Quaternion initialRotation;

        private ShipPhysicsState initialPhysicsState;
        private Stack<ShipPhysicsState> stateStack;

        public ShipStateMemento(GameObject ship) {
            _ship = ship;
            _rb = ship.GetComponentInChildren<Rigidbody>();
            _controller = ship.GetComponentInChildren<AbstractShipController>();
            Reset();
        }
    
        public void Reset() {
            initialPhysicsState = getCurrentPhysicsState();
            initStateStack();
        }

        public void RestoreInitialState() {
            restore(initialPhysicsState);
        }

        public void Rollback(int number) {
            Debug.Log($"Memento.Rollback({number}):");
            var rollbacks = number;
        
            var state = stateStack.Peek();
            Debug.Log($"\tPeeked {state.Position.ToString()}");
            while (rollbacks > 0 && stateStack.Count - rollbacks > 2) {
                state = stateStack.Pop();
                rollbacks--;
                Debug.Log($"\tPoped {state.Position.ToString()} (rollbacks = {rollbacks})");
            }
            
            restore(state);
            // printLastStates();
            Debug.Log($"Memento.Rollback() : Stack size = {stateStack.Count}");

            // printLastStates();
        }

        public void SaveState(float? tolerance = null) {
            var current_state = getCurrentPhysicsState();
            if (tolerance != null && Vector3.Distance(stateStack.Peek().Position,current_state.Position) < tolerance) {
                Debug.Log($"Memento.SaveState({current_state.Position.ToString()}) skipped.");
                return;
            }
            
            Debug.Log($"Memento.SaveState({current_state.Position.ToString()})");
            stateStack.Push(getCurrentPhysicsState());
            Debug.Log($"Memento.SaveState() : Stack size = {stateStack.Count}");

        }
    
        private ShipPhysicsState getCurrentPhysicsState() {
            return new() {
                Position = _rb.transform.position,
                Rotation = _controller.transform.rotation,
                Velocity = _rb.velocity,
                AngularVelocity = _rb.angularVelocity,
            };
        }

        private void restore(ShipPhysicsState state) {
            Debug.Log($"Memento.Restore({state.Position.ToString()})");

            _rb.transform.position = state.Position;
            _controller.transform.rotation  = state.Rotation;
            _rb.velocity = state.Velocity;
            _rb.angularVelocity = state.AngularVelocity;
            
            _controller.Reset();
        }


        private void initStateStack() {
            stateStack = new Stack<ShipPhysicsState>();
            stateStack.Push(initialPhysicsState);
            stateStack.Push(initialPhysicsState);
            Debug.Log($"Memento.initStateStack() : Stack size = {stateStack.Count}");

        }

        // private void printLastStates() {
        //     List<String> states = new List<string>();
        //     foreach (var state in stateStack) {
        //         states.Add(state.Position.ToString());
        //     }
        //     Debug.Log(string.Join("->",states));
        // }
    }
}