NET.addAssembly('System.Speech');
ss = System.Speech.Synthesis.SpeechSynthesizer;
ss.Volume = 100;
Speak(ss,'You can use .NET Library in Matrix Laboratory');
