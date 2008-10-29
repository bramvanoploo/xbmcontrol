﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace XBMC
{
    public class XBMC_Controls
    {
        XBMC_Communicator parent = null;

        public XBMC_Controls(XBMC_Communicator p)
        {
            parent = p;
        }

        public void Play()
        {
            parent.Request("ExecBuiltIn", "PlayerControl(Play)");
        }

        public void PlayFile(string filename)
        {
            parent.Request("PlayFile(" + filename + ")");
        }

        public void PlayMedia(string media)
        {
            parent.Request("ExecBuiltIn", "PlayMedia(" + media + ")");
        }

        public void Stop()
        {
            parent.Request("ExecBuiltIn", "PlayerControl(Stop)");
        }

        public void Next()
        {
            parent.Request("ExecBuiltIn", "PlayerControl(Next)");
        }

        public void PlayListNext()
        {
            parent.Request("PlayListNext");
        }

        public void Previous()
        {
            parent.Request("ExecBuiltIn", "PlayerControl(Previous)");
        }

        public void ToggleShuffle()
        {
            parent.Request("ExecBuiltIn", "PlayerControl(Random)");
        }

        public void TogglePartymode()
        {
            parent.Request("ExecBuiltIn", "PlayerControl(Partymode(music))");
        }

        public void Repeat(bool enable)
        {
            string mode = (enable) ? "Repeat" : "RepeatOff";
            parent.Request("ExecBuiltIn", "PlayerControl(" + mode + ")");
        }

        public void LastFmLove()
        {
            parent.Request("ExecBuiltIn", "LastFM.Love(false)");
        }

        public void LastFmHate()
        {
            parent.Request("ExecBuiltIn", "LastFM.Ban(false)");
        }

        public void ToggleMute()
        {
            parent.Request("ExecBuiltIn", "Mute");
        }

        public void SetVolume(int percentage)
        {
            parent.Request("ExecBuiltIn", "SetVolume(" + Convert.ToString(percentage) + ")");
        }

        public void SeekPercentage(int percentage)
        {
            parent.Request("SeekPercentage", Convert.ToString(percentage));
        }

        public void Reboot()
        {
            parent.Request("ExecBuiltIn", "Reboot");
        }

        public void Shutdown()
        {
            parent.Request("ExecBuiltIn", "Shutdown");
        }

        public void Restart()
        {
            parent.Request("ExecBuiltIn", "RestartApp");
        }

        public string GetGuiDescription(string field)
        {
            string returnValue = null;
            string[] aGuiDescription = parent.Request("GetGUIDescription");

            for (int x = 0; x < aGuiDescription.Length; x++)
            {
                int splitIndex = aGuiDescription[x].IndexOf(':') + 1;
                if (splitIndex > 1)
                {
                    string resultField = aGuiDescription[x].Substring(0, splitIndex - 1).Replace(" ", "").ToLower();
                    if (resultField == field) returnValue = aGuiDescription[x].Substring(splitIndex, aGuiDescription[x].Length - splitIndex);
                }
            }

            return returnValue;
        }

        public string GetScreenshotBase64()
        {
            string[] base64screenshot = parent.Request("takescreenshot", "screenshot.png;false;0;" + this.GetGuiDescription("width") + ";" + this.GetGuiDescription("height") + ";75;true;");
            return (base64screenshot == null) ? null : base64screenshot[0];
        }

        public Image Base64StringToImage(string base64String)
        {
            Bitmap file = null;
            byte[] bytes = Convert.FromBase64String(base64String);
            MemoryStream stream = new MemoryStream(bytes);

            if (base64String != null && base64String != "")
                file = new Bitmap(Image.FromStream(stream));

            return file;
        }

        public Image GetScreenshot()
        {
            Image screenshot = null;
            string base64ImageString = this.GetScreenshotBase64();

            if (base64ImageString != null)
                screenshot = this.Base64StringToImage(base64ImageString);

            return screenshot;
        }
    }
}
