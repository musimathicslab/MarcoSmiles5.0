using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using Sanford.Multimedia.Midi;
using System;


public class tesi{

    private OutputDevice outD;
    private ChannelMessageBuilder builder;

    public tesi(){}

    /// <summary>
    /// Send signal Midi Note On to output device.
    /// </summary>
    /// <param name="note">Note to send</param>
    /// <param name="octave">Octave</param>
    public void SendEvent(int note, int octave){

        // Note
        builder.Data1 = note + (octave * 12);

        // Velocity
        builder.Data2 = 105;

        //Most Significant Bit
        //Signal NoteOn (1001)
        builder.Command = ChannelCommand.NoteOn;

        //Least Significant Bit
        //MidiChannel (0-15)
        builder.MidiChannel = 0;

        // System Byte
        // MSB | LSB
        //1001 | 0000

        // Build message
        builder.Build();

        // Send message
        outD.Send(builder.Result);

    }

    /// <summary>
    /// Search MarcoSmiles device
    /// </summary>
    public void FindMidi(){
        int DevId = 0;

        int numDevice = OutputDevice.DeviceCount;

        for (int i = 0; i < numDevice; i++){

            MidiOutCaps dev = OutputDevice.GetDeviceCapabilities(i);

            if (dev.name == "MarcoSmiles"){
                DevId = i;
            }
        }
        outD = new OutputDevice(DevId);
        builder = new ChannelMessageBuilder();
    }

    /// <summary>
    /// Send signal Midi Note Off to output device.
    /// </summary>
    /// <param name="note">Note</param>
    /// <param name="octave">Octave</param>
    public void SendMidiOff(int note, int octave){

        // Note to release
        builder.Data1 = (note + (octave * 12));

        // Velocity
        builder.Data2 = 0;

        // Most Significant Bit
        // NoteOff (1000)
        builder.Command = ChannelCommand.NoteOff;

        //Least Significant Bit
        //MidiChannel (0-15)
        builder.MidiChannel = 0;

        // System Byte
        // MSB | LSB
        //1000 | 0000

        // Build Message
        builder.Build();

        // Send message
        outD.Send(builder.Result);
    }
}