using System.Numerics;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.MusicTheory;
using Raylib_CSharp.Camera.Cam3D;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Windowing;
using VanillaWave.MIDI;

namespace VanillaWave;

internal class Game
{
    private Color ClearColor = Color.Blank;
    private bool IsRunning = true;

    public void Run(string[] args)
    {
        Window.SetState(
            ConfigFlags.ResizableWindow
                | ConfigFlags.BorderlessWindowMode
                | ConfigFlags.Msaa4XHint
                | ConfigFlags.VSyncHint
        );
        Window.Init(500, 500, "vanilla");
        Input.SetExitKey(KeyboardKey.Null);
        Init();
        while (IsRunning)
        {
            Update();
            Render();
        }
        Closing();
    }

    private MidiManager MidiManager;
    private InputDevice RealInputDevice;
    private OutputDevice RealOutputDevice;
    private KeyboardMidiDevice KeyboardMidi;

    private void Init()
    {
        MidiManager.PrintRealMidiDevices();

        RealOutputDevice = OutputDevice.GetAll().Where(d => d.Name.Contains("GS")).First();
        if (InputDevice.GetDevicesCount() > 0)
        {
            RealInputDevice = InputDevice.GetAll().First();
            RealInputDevice.SilentNoteOnPolicy = SilentNoteOnPolicy.NoteOff;
            MidiManager = new(RealInputDevice, RealOutputDevice);
        }
        else
        {
            KeyboardMidi = new KeyboardMidiDevice();
            MidiManager = new(KeyboardMidi, RealOutputDevice);
        }
        MidiManager.NoteOn += Midi_NoteOn;
        MidiManager.NoteOff += Midi_NoteOff;
        MidiManager.DefaultEvent += Midi_DefaultEvent;
        MidiManager.CurrentInputDevice.Connect(MidiManager.CurrentOutputDevice);

        camera = new()
        {
            FovY = 90,
            Position = new Vector3(0, 0, 10),
            Projection = CameraProjection.Orthographic,
            Target = Vector3.Zero,
            Up = Vector3.UnitY
        };
    }

    private void Midi_DefaultEvent(MidiEvent midiEvent)
    {
        Console.WriteLine($"Event {midiEvent.EventType}");
    }

    private void KeyboardMidi_EventReceived(object? sender, MidiEventReceivedEventArgs e)
    {
        switch (e.Event.EventType)
        {
            case MidiEventType.NoteOn:
                var noteon = (NoteOnEvent)e.Event;
                Console.WriteLine(noteon.GetNoteName());
                break;
            case MidiEventType.NoteOff:
                var noteoff = (NoteOffEvent)e.Event;
                Console.WriteLine(noteoff.GetNoteName());
                break;
        }
    }

    private Dictionary<NoteName, (bool active, int vel)> activeNotes = [];

    private void Midi_NoteOff(NoteInfo note)
    {
        Console.WriteLine(note);
        activeNotes[note.Name] = (false, note.Velocity);
    }

    private void Midi_NoteOn(NoteInfo note)
    {
        Console.WriteLine(note);
        activeNotes[note.Name] = (true, note.Velocity);
    }

    private void Closing()
    {
        RealInputDevice?.Dispose();
        RealOutputDevice?.Dispose();
        KeyboardMidi?.Dispose();
    }

    private void Update()
    {
        KeyboardMidi?.Poll();

        if (Input.IsKeyPressed(KeyboardKey.Escape) || Window.ShouldClose())
        {
            IsRunning = false;
        }
    }

    private int CurrentMonitor => Window.GetCurrentMonitor();

    private Camera3D camera;

    private void Render()
    {
        Graphics.BeginDrawing();
        Graphics.ClearBackground(ClearColor);
        {
            Graphics.DrawFPS(5, 5);

            int ybase = Window.GetRenderHeight() / 2;
            int xbase = Window.GetRenderWidth() / 12;
            int shift = xbase - (xbase / 2);
            float bumpfactor =
                (float)Window.GetRenderHeight() / Window.GetMonitorHeight(CurrentMonitor);
            foreach (var note in activeNotes)
            {
                if (note.Value.active)
                {
                    Vector2 notepos = Vector2.Zero;
                    notepos.X = (xbase * (int)note.Key) + shift;
                    notepos.Y = ybase - note.Value.vel * bumpfactor;
                    byte velch = (byte)((note.Value.vel / 127.0) * 255);
                    byte posch = (byte)((((int)note.Key + 1) / 12.0) * 255);
                    Color notecolor = new(200, velch, posch, 255);
                    Graphics.DrawCircleV(notepos, (float)(shift / 2.0), notecolor);
                }
            }
        }
        Graphics.EndDrawing();
    }
}
