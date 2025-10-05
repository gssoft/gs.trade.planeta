using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Interfaces;
using System.Speech.Synthesis;

namespace GS.EventLog
{
    public class SpeakerEventLog : Evl, IEventLog
    {
        private readonly SpeechSynthesizer _synth;
        public int Rate { get; set; }

        public SpeakerEventLog()
        {
            _synth = new SpeechSynthesizer();
        }
        
        public override void Init()
        {
            _synth.SetOutputToDefaultAudioDevice();
            _synth.SelectVoiceByHints(VoiceGender.Male, VoiceAge.Adult);
            _synth.Rate = Rate <= 10 && Rate >= -10 ? Rate : 0;
        }
        public void AddItem(IEventLogItem evli )
        {
            if (evli.ResultCode == EvlResult.FATAL || 
                evli.ResultCode == EvlResult.WARNING ||
                evli.ResultCode == EvlResult.SOS
                )
                    _synth.SpeakAsync(string.Join(",",  evli.ResultCode.ToString(),
                                                        evli.Subject.ToString(),
                                                        evli.Source,
                                                        evli.Operation,
                                                        evli.Description));
        }

        public void AddItem(EvlResult result, string operation, string description)
        {
          //  throw new NotImplementedException();
            _synth.SpeakAsync(string.Join(" : ", result, operation, description));
        }

        public void AddItem(EvlResult result, EvlSubject subject, string source, string entity, string operation, string description,
            string objects)
        {
            
        }

        public void ClearSomeData(int count)
        {
            
        }

        public override IEnumerable<IEventLogItem> Items
        {
            get { return null; }
        }

        public IEventLog Primary { get { return this; } }
    }
}
