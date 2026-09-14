using R2API;
using RiskOfRune;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

public static class SoundBank
{
    public static uint _soundBankId;
    public const string soundBankFolderName = "AGU-DeltaruneMod";
    public const string soundBankFileName = "DeltaruneSoundBank.bnk";
    //public const string soundBankName = "DeltaruneSoundBank";
    public static string SoundBankDirectory = RiskOfRuneMain.assetBundleDir;

    // Setup soundbank
    public static void Init()
    {
        UnityEngine.Debug.Log(SoundBankDirectory);
        try
        {
            string fullBankPath;
            if (SoundBankDirectory.Contains(soundBankFolderName))
            {
                fullBankPath = Path.Combine(SoundBankDirectory, soundBankFileName);
            }
            else
            {
                fullBankPath = Path.Combine(SoundBankDirectory, soundBankFolderName, soundBankFileName);
            }
                    
            UnityEngine.Debug.Log(SoundBankDirectory + " ||| " + soundBankFileName);
            UnityEngine.Debug.Log($"SoundBank size: {new FileInfo(fullBankPath).Length} bytes");

            UnityEngine.Debug.Log($"Attempting to load sound bank...");

            if (!File.Exists(fullBankPath))
            {
                UnityEngine.Debug.Log($"Sound bank path does not exist!!");
                return;
            }

            var result = AkSoundEngine.LoadBank(fullBankPath, out _soundBankId);

            if (result == AKRESULT.AK_Success)
            {
                UnityEngine.Debug.Log($"SoundBank loaded successfully!");
            }
            else
            {
                UnityEngine.Debug.Log($"SoundBank failed to load. {result}");
            }

            SoundAPI.SoundBanks.Add(fullBankPath);
        }
        catch ( Exception ex ) { UnityEngine.Debug.Log("Failed to load soundbank: " + ex); }
            
            
            
    }
}
