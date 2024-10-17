using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.MusicTheory;
using Raylib_CSharp.Interact;

namespace VanillaWave;

internal class KeyboardMidiDevice : IInputDevice
{
    private Dictionary<KeyboardKey, int> KeyToNote =
        new()
        {
            // 3 oct
            { KeyboardKey.Z, 48 },
            { KeyboardKey.S, 49 },
            { KeyboardKey.X, 50 },
            { KeyboardKey.D, 51 },
            { KeyboardKey.C, 52 },
            { KeyboardKey.V, 53 },
            { KeyboardKey.G, 54 },
            { KeyboardKey.B, 55 },
            { KeyboardKey.H, 56 },
            { KeyboardKey.N, 57 },
            { KeyboardKey.J, 58 },
            { KeyboardKey.M, 59 },
            // 4 oct
            { KeyboardKey.Q, 60 },
            { KeyboardKey.Two, 61 },
            { KeyboardKey.W, 62 },
            { KeyboardKey.Three, 63 },
            { KeyboardKey.E, 64 },
            { KeyboardKey.R, 65 },
            { KeyboardKey.Five, 66 },
            { KeyboardKey.T, 67 },
            { KeyboardKey.Six, 68 },
            { KeyboardKey.Y, 69 },
            { KeyboardKey.Seven, 70 },
            { KeyboardKey.U, 71 },
            // 5 oct
            { KeyboardKey.I, 72 },
            { KeyboardKey.Nine, 73 },
            { KeyboardKey.O, 74 },
            { KeyboardKey.Zero, 75 },
            { KeyboardKey.P, 76 }
        };

    private bool IsActive;

    public bool IsListeningForEvents
    {
        get { return IsActive; }
        set { IsActive = value; }
    }

    public event EventHandler<MidiEventReceivedEventArgs> EventReceived;

    public void Poll()
    {
        if (IsActive)
        {
            foreach (var k in KeyToNote)
            {
                var key = (SevenBitNumber)k.Value;
                var vel = (SevenBitNumber)127;
                if (Input.IsKeyPressed(k.Key))
                {
                    EventReceived?.Invoke(
                        this,
                        new MidiEventReceivedEventArgs(new NoteOnEvent(key, vel))
                    );
                }
                if (Input.IsKeyReleased(k.Key))
                {
                    EventReceived?.Invoke(
                        this,
                        new MidiEventReceivedEventArgs(new NoteOffEvent(key, vel))
                    );
                }
            }
        }
    }

    public void Dispose() { }

    public void StartEventsListening()
    {
        IsActive = true;
    }

    public void StopEventsListening()
    {
        IsActive = false;
    }
}
