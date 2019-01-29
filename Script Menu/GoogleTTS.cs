/**
 * This script converts stereo events to pairs of mono events.  It
 * only operates on the selected audio events or, if none are
 * selected, all audio events.
 *
 * Revision Date: March 26, 2010.
 **/

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
// using System.Diagnostics;
using Google.Cloud.TextToSpeech.V1;

using Sony.Vegas;

public class EntryPoint {

    Vegas myVegas;
    String defaultBasePath = "Untitled_";

    public void FromVegas(Vegas vegas) {
        myVegas = vegas;
        List<AudioEvent> events = new List<AudioEvent>();
        String projectPath = myVegas.Project.FilePath;
        AudioTrack track;
        if (String.IsNullOrEmpty(projectPath))
        {
            MessageBox.Show("Please first create a project");
        }
        else if ((track = GetSelectedAudioTrack()) == null) {
            MessageBox.Show("You need to have at least 1 selected audio track");
        }
        else
        {

            AddSelectedAudioEvents(events);


            String dir = Path.GetDirectoryName(projectPath);
            String fileName = Path.GetFileNameWithoutExtension(projectPath);
            defaultBasePath = fileName + "_tts.mp3";
            defaultBasePath = GetNextAvailableFilename(dir, defaultBasePath);
            String saveFileName = dir + "\\" + defaultBasePath;

            DialogResult result = ShowGoogleTTSDialog();
            if (DialogResult.OK == result)
            {
                if (TTSText.Text.Length == 0) {
                    MessageBox.Show("Enter some text will you?");
                } else {
                   GetSound(TTSText.Text, saveFileName);

                    // AudioEvent videoEvent = track.AddVideoEvent(start, length);
                    // Take take = videoEvent.AddTake(media.GetVideoStreamByIndex(0));
                    // myVegas.Transport.CursorPosition = cursorPosition;
                   myVegas.OpenFile(saveFileName);
                   AudioEvent evnt = GetSelectedAudioEvent();
                   evnt.ActiveTake.Name = TTSText.Text;


                    // MessageBox.Show();
                }
            }


            // MessageBox.Show(defaultBasePath);
        }

    }

    AudioTrack GetSelectedAudioTrack()
    {
        foreach (Track track in myVegas.Project.Tracks)
            if (track.IsAudio() && track.Selected)
                return (AudioTrack) track;

        return null;
    }
    AudioEvent GetSelectedAudioEvent()
    {
        foreach (Track track in myVegas.Project.Tracks)
        {
            if (track.IsAudio())
            {
                foreach (AudioEvent audioEvent in track.Events)
                {
                    if (audioEvent.Selected)
                    {
                        return audioEvent;
                    }
                }
            }
        }

        return null;
    }
    void AddSelectedAudioEvents(List<AudioEvent> events)
    {
        foreach (Track track in myVegas.Project.Tracks)
        {
            if (track.IsAudio())
            {
                foreach (AudioEvent audioEvent in track.Events)
                {
                    if (audioEvent.Selected)
                    {
                        events.Add(audioEvent);
                    }
                }
            }
        }
    }


    TextBox TTSText;

    DialogResult ShowGoogleTTSDialog()
    {
        int buttonWidth = 80;
        int buttonTop = 230;

        Form dlog = new Form();
        dlog.Text = "Google Text-to-Speech";
        dlog.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        dlog.MaximizeBox = false;
        dlog.StartPosition = FormStartPosition.CenterScreen;
        dlog.Width = 610;
        dlog.Height = 300;
        // dlog.FormClosing += this.HandleFormClosing;

        Label label = new Label();
        label.AutoSize = true;
        label.Text = "Text:";
        label.Left = 4;
        label.Top = 4;
        dlog.Controls.Add(label);

        TTSText = new TextBox();
        TTSText.Multiline = true;
        TTSText.Left = 4;
        TTSText.Top = 22;
        TTSText.Width = 585;
        TTSText.Height = 200;
        TTSText.Text = "";
        dlog.Controls.Add(TTSText);



        // BUTTONS
        Button okButton = new Button();
        okButton.Text = "OK";
        okButton.Left = dlog.Width - (2*(buttonWidth+20));
        okButton.Top = buttonTop;
        okButton.Width = buttonWidth;
        okButton.Height = okButton.Font.Height + 12;
        okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
        // dlog.AcceptButton = okButton;
        dlog.Controls.Add(okButton);

        Button cancelButton = new Button();
        cancelButton.Text = "Cancel";
        cancelButton.Left = dlog.Width - (1*(buttonWidth+20));
        cancelButton.Top = buttonTop;
        cancelButton.Height = cancelButton.Font.Height + 12;
        cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        dlog.CancelButton = cancelButton;
        dlog.Controls.Add(cancelButton);



        return dlog.ShowDialog(myVegas.MainWindow);
    }

    void GetSound(String sayText, String fileName)
    {

        TextToSpeechClient client = TextToSpeechClient.Create();


        // Set the text input to be synthesized.
        SynthesisInput input = new SynthesisInput
        {
            Text = sayText
        };

        // Build the voice request, select the language code ("en-US"),
        // and the SSML voice gender ("neutral").
        VoiceSelectionParams voice = new VoiceSelectionParams
        {
            LanguageCode = "en-US",
            SsmlGender = SsmlVoiceGender.Neutral
        };

        // Select the type of audio file you want returned.
        AudioConfig config = new AudioConfig
        {
            AudioEncoding = AudioEncoding.Mp3
        };

        // Perform the Text-to-Speech request, passing the text input
        // with the selected voice parameters and audio file type
        var response = client.SynthesizeSpeech(new SynthesizeSpeechRequest
        {
            Input = input,
            Voice = voice,
            AudioConfig = config
        });

        // Write the binary AudioContent of the response to an MP3 file.
        using (Stream output = File.Create(fileName))
        {
            response.AudioContent.WriteTo(output);
            //Console.WriteLine($"Audio content written to file 'sample.mp3'");
        }


    }


    public string GetNextAvailableFilename(string dir, string filename)
    {
        if (!System.IO.File.Exists(dir + "\\" + filename)) return filename;

        string alternateFilename;
        int fileNameIndex = 1;
        do
        {
            fileNameIndex += 1;
            alternateFilename = CreateNumberedFilename(filename, fileNameIndex);
        } while (System.IO.File.Exists(dir + "\\" + alternateFilename));

        return alternateFilename;
    }

    private string CreateNumberedFilename(string filename, int number)
    {
        string plainName = System.IO.Path.GetFileNameWithoutExtension(filename);
        string extension = System.IO.Path.GetExtension(filename);
        return string.Format("{0}{1}{2}", plainName, number, extension);
    }


}