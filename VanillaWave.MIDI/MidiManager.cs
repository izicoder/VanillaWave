using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.MusicTheory;
using VanillaWave.Logging;

namespace VanillaWave.MIDI;

public enum NoteType
{
    On,
    Off
}

public struct NoteInfo
{
    public NoteName Name;
    public NoteType Type;
    public int Octave;
    public int Channel;
    public int Velocity;
    public int Number;

    public override readonly string ToString()
    {
        return $"Note: {Name}, {Octave}, {Velocity}, {Number}, {Channel}, {Type}";
    }
}

public class MidiManager
{
    public IInputDevice CurrentInputDevice { get; set; }
    public IOutputDevice CurrentOutputDevice { get; set; }

    public delegate void NoteEventHandler(NoteInfo note);
    public delegate void DefaultEventHandelr(MidiEvent midiEvent);

    public event NoteEventHandler? NoteOn;
    public event NoteEventHandler? NoteOff;
    public event DefaultEventHandelr? DefaultEvent;

    public MidiManager(IInputDevice input, IOutputDevice output)
    {
        CurrentInputDevice = input;
        CurrentOutputDevice = output;

        CurrentInputDevice.EventReceived += OnDeviceEvent;
        CurrentInputDevice.StartEventsListening();
        CurrentOutputDevice.PrepareForEventsSending();
    }

    private void OnDeviceEvent(object? _, MidiEventReceivedEventArgs e)
    {
        switch (e.Event.EventType)
        {
            case MidiEventType.NoteOn:
                var noteOn = (NoteOnEvent)e.Event;
                NoteOn?.Invoke(
                    new NoteInfo()
                    {
                        Number = noteOn.NoteNumber,
                        Name = noteOn.GetNoteName(),
                        Octave = noteOn.GetNoteOctave(),
                        Channel = noteOn.Channel,
                        Velocity = noteOn.Velocity,
                        Type = NoteType.On
                    }
                );
                break;
            case MidiEventType.NoteOff:
                var noteOff = (NoteOffEvent)e.Event;
                NoteOff?.Invoke(
                    new NoteInfo()
                    {
                        Number = noteOff.NoteNumber,
                        Name = noteOff.GetNoteName(),
                        Octave = noteOff.GetNoteOctave(),
                        Channel = noteOff.Channel,
                        Velocity = noteOff.Velocity,
                        Type = NoteType.Off
                    }
                );
                break;
            default:
                DefaultEvent?.Invoke(e.Event);
                break;
        }
    }

    public static void PrintRealMidiDevices()
    {
        Logger.Global.Info("Input Devices");
        foreach (var d in InputDevice.GetAll())
        {
            Logger.Global.Info(d.Name);
        }
        Logger.Global.Info("Output devices");
        foreach (var d in OutputDevice.GetAll())
        {
            Logger.Global.Info(d.Name);
        }
    }
}
