﻿using Microsoft.Xna.Framework;
using OctoAwesome.Client.Components.Input;
using System;
using System.Collections.Generic;

namespace OctoAwesome.Client.Components
{
    internal sealed class InputSet : IInputSet
    {
        public const int SlotTriggerLength = 10;

        private List<IInputSet> inputDevices;

        private MouseInput mouse;
        private KeyboardInput keyboard;
        private GamePadInput gamepad;

        public float MoveX { get; private set; }
        public float MoveY { get; private set; }
        public float HeadX { get; private set; }
        public float HeadY { get; private set; }
        public Trigger<bool> InteractTrigger { get; private set; }
        public Trigger<bool> ApplyTrigger { get; private set; }
        public Trigger<bool> JumpTrigger { get; private set; }
        public Trigger<bool> ToggleFlyMode { get; private set; }
        public Trigger<bool> InventoryTrigger { get; private set; }
        public Trigger<bool>[] SlotTrigger { get; private set; }
        public Trigger<bool> SlotLeftTrigger { get; private set; }
        public Trigger<bool> SlotRightTrigger { get; private set; }

        public InputSet(Game game)
        {
            InteractTrigger = new Trigger<bool>();
            ApplyTrigger = new Trigger<bool>();
            InventoryTrigger = new Trigger<bool>();
            JumpTrigger = new Trigger<bool>();
            ToggleFlyMode = new Trigger<bool>();
            SlotLeftTrigger = new Trigger<bool>();
            SlotRightTrigger = new Trigger<bool>();
            SlotTrigger = new Trigger<bool>[SlotTriggerLength];
            for (int i = 0; i < SlotTrigger.Length; i++)
                SlotTrigger[i] = new Trigger<bool>();

            gamepad = new GamePadInput();
            keyboard = new KeyboardInput();
            mouse = new MouseInput(game);

            inputDevices = new List<IInputSet>{
                gamepad, 
                keyboard, 
                mouse
            };
        }

        public void UpdateInput(GameTime gameTime)
        {

            bool nextInteract = false;
            bool nextJump = false;
            bool nextApply = false;
            bool nextInventory = false;
            bool[] nextSlot = new bool[SlotTriggerLength];
            bool nextSlotLeft = false;
            bool nextToggleFlyMode = false;
            bool nextSlotRight = false;
            MoveX = 0f;
            MoveY = 0f;
            HeadX = 0f;
            HeadY = 0f;

            gamepad.Update();
            keyboard.Update();
            mouse.Update();

            foreach (var device in inputDevices)
            {
                nextInteract |= device.InteractTrigger;
                nextApply |= device.ApplyTrigger;
                nextJump |= device.JumpTrigger;
                nextInventory |= device.InventoryTrigger;
                nextToggleFlyMode |= device.ToggleFlyMode;
                nextSlotLeft |= device.SlotLeftTrigger;
                nextSlotRight |= device.SlotRightTrigger;
                if (device.SlotTrigger != null)
                    for (int i = 0; i < Math.Min(device.SlotTrigger.Length, SlotTriggerLength); i++)
                        nextSlot[i] |= device.SlotTrigger[i];

                MoveX += device.MoveX;
                MoveY += device.MoveY;
                HeadX += device.HeadX;
                HeadY += device.HeadY;
            }

            InteractTrigger.Value = nextInteract;
            ApplyTrigger.Value = nextApply;
            InventoryTrigger.Value = nextInventory;
            JumpTrigger.Value = nextJump;
            ToggleFlyMode.Value = nextToggleFlyMode;
            SlotLeftTrigger.Value = nextSlotLeft;
            SlotRightTrigger.Value = nextSlotRight;
            for (int i = 0; i < SlotTriggerLength; i++)
                SlotTrigger[i].Value = nextSlot[i];

            MoveX = Math.Min(1, Math.Max(-1, MoveX));
            MoveY = Math.Min(1, Math.Max(-1, MoveY));
        }
    }
}