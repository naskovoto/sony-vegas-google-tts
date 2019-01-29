# Google TTS plugin for Sony Vegas

This is a script for Sony Vegas which will allow you to quickly insert text-to-speech audio clips to your track. It uses the [Google Cloud Text-to-Speech](https://cloud.google.com/text-to-speech/) service.
As an addition there is a plugin which quickly generates a .srt subtitles file based on the text you have entered. You can use this to create closed captions in YouTube.

# Installation

- First you need the Sony Vegas software, of course.
- Next you need to create a Google CLoud project with enabled billing. Follow this guide - (https://cloud.google.com/text-to-speech/docs/quickstart-client-libraries).
Make sure you set the `GOOGLE_APPLICATION_CREDENTIALS` variable properly. I had to set it as a global enviroment variable.
- Copy both folders (Google and "Script Menu") into your Sony Vegas installation folder i.e `C:\Program Files\Sony\Vegas Pro 13.0\`. You should already have a "Script Menu" folder there (don't remove the original one).
- Edit the `vegas130.exe.config` file which is located in the main Sony Vegas installation folder. Add this XML in the `<assemblyBinding>` section:
``` xml
<dependentAssembly>
  <assemblyIdentity name="Google.Apis.Auth" publicKeyToken="4b01fa6e34db77ab" culture="neutral" />
  <bindingRedirect oldVersion="1.21.0.0-1.35.1.0" newVersion="1.35.1.0" />
</dependentAssembly>
```
- In the Google folder there are some DLLs. I have no idea if they will work for you. I took them from the Visual Studio NuGet installation of Google.Cloud.TextToSpeech.V1 There might be a more professional way to do this, but this is my first encounter with .NET and C#. No idea how to run this on Linux or macOS.

# Usage
- Open Sony Vegas
- Create a project and save it somewhere (this is where the plugin will save all the generated MP3 files)
- Create an audio track
- Place the cursor where you want the audio clip inserted.
- Go to `Tools -> Scripting -> GoogleTTS`
- You can assign a keyboard shortcut for quick access - `Options -> Customize Keyboard -> Global -> Script.GoogleTTS`
- Once you have placed all your audio clips you can generate the .srt subtitles file with `Tools -> Scripting -> GoogleTTS to SRT`

# P.S.

This is a very basic implementation which perfectly serves the purpose it was created for but can be greatly improved:

- It would be nice to be able to choose a language, voice, pitch, speed, which are available options in the google API and it should be very easy to do.
- ssml support - this will allow adding breaks and emphasis.
- ability to edit a clip (load the previous text by default and replace the MP3 / audio clip)
- It might be possible to add the Windows voices as a TTS engine
