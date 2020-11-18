using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System;


namespace SpeechService
{
    public class LanguageVoice : INotifyPropertyChanged,IDisposable
    {
        private static LanguageVoice _instance = new LanguageVoice();

        public static LanguageVoice GetInstance() { return _instance; }

        //it has to be the pattern by using the ssml method.
        //put a sample string here for reference.
        private static string _ssmlStringSample =
             "<speak version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\" xml:lang=\"en-US\">" + "\r\n" +
             "<voice name=\"en-US-GuyNeural\">" + "\r\n" +
             " <prosody rate=\"-1\" pitch=\"-2.0st\">" + "\r\n" +
             " Welcome to Microsoft Cognitive Services Text-to-Speech API.TestBB3.1" + "\r\n" +
             "</prosody></voice></speak > ";

        private static string _ssmlString =
            "<speak version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\" xml:lang=\"en-US\">" + "\r\n" +
            "<voice name=\"{0}\">" + "\r\n" +
            " <prosody rate=\"{1}\" pitch=\"{2}st\">" + "\r\n" +
            "{3}" + "\r\n" +
            "</prosody></voice></speak > ";

        private static string _subscriptionKey = ConfigurationManager.AppSettings["subscriptionKey"].ToString();
        private static string _serviceRegion = ConfigurationManager.AppSettings["serviceRegion"].ToString();

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyRaised(string propertyname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }

        private string getInputText()
        {

            string rate = _rate.ToString();
            string pitch = _pitchNumber >= 0 ? string.Format("+{0}", pitchNumber) : string.Format("{0}", pitchNumber);
            return string.Format(_ssmlString, Voice, rate, pitch, _inputText);
        }
        private double _pitchNumber;
        private double _rate;
        private string _inputText;
        private string _outputText;
        private bool _mp3Selected=false;
        private bool _wavSelected=true;
        private string _languagestr;
        private string _maleOrfemale;
        private string _inputSource;
        private LanguageVoice() { }
        private static string getVoiceString(string language, string maleOrFemaleVoice)
        {
            //var voice = "Microsoft Server Speech Text to Speech Voice (en-US, GuyNeural)";
            string voice = "";
            switch (language)
            {
                case "English(Australia)":
                    if (maleOrFemaleVoice == "Male")
                    {
                        // voice = "Microsoft Server Speech Text to Speech Voice (en-US, GuyNeural)";
                        voice = "en-US-GuyNeural";
                    }
                    else
                    {
                        //en-AU-NatashaNeural
                        //voice = "Microsoft Server Speech Text to Speech Voice (en-US, NatashaNeural)";
                        voice = "en-AU-NatashaNeural";
                    }

                    break;
                case "French(France)":
                    if (maleOrFemaleVoice == "Male")
                    {
                        //fr-FR-HenriNeural
                        //voice = "Microsoft Server Speech Text to Speech Voice (fr-FR, HenriNeural)";
                        voice = "fr-FR-HenriNeural";
                    }
                    else
                    {
                        //fr-FR-DeniseNeural
                        //voice = "Microsoft Server Speech Text to Speech Voice (fr-FR, DeniseNeural)";
                        voice = "fr-FR-DeniseNeural";
                    }
                    break;
                case "German(Germany)":
                    if (maleOrFemaleVoice == "Male")
                    {
                        //de-DE-ConradNeural
                        //voice = "Microsoft Server Speech Text to Speech Voice (de-DE, ConradNeural)";
                        voice = "de-DE-ConradNeural";
                    }
                    else
                    {
                        //de-DE-KatjaNeural
                        //voice = "Microsoft Server Speech Text to Speech Voice (de-DE, KatjaNeural)";
                        voice = "de-DE-KatjaNeural";
                    }
                    break;

                default:
                    voice = "voice";
                    break;
            }
            return voice;

        }

        #region publicproperties
        public string getInputText_Voice
        {
            get
            {
                OnPropertyRaised("getInputText_Voice");
                return getInputText();
            }
        }

        public bool mp3Selected
        {
            get { return _mp3Selected; }
            set { _mp3Selected = value; OnPropertyRaised("mp3Selected"); }
        }

        public bool wavSelected
        {
            get { return _wavSelected; }
            set { _wavSelected = value; OnPropertyRaised("wavSelected"); }
        }

        public double pitchNumber
        {
            get
            {
                return _pitchNumber;
            }
            set
            {
                _pitchNumber = value;
                OnPropertyRaised("pitchNumber");
            }
        }

        public double speedRate
        {
            get { return _rate; }
            set
            {
                _rate = value;
                OnPropertyRaised("speedRate");
            }
        }

        public string InputText
        {
            get { return _inputText; }
            set { _inputText = value; OnPropertyRaised("InputText"); }
        }

        public string OutputText
        {
            get
            {
                return _outputText;
            }
            set
            {
                _outputText = value;

                OnPropertyRaised("OutputText");

            }
        }

        public string LanguageStr
        {
            get
            {
                return _languagestr;
            }
            set
            {
                _languagestr = value; OnPropertyRaised("LanguageStr");
                OnPropertyRaised("Voice");
            }
        }
        public string MaleOrFemale
        {
            get
            {
                return _maleOrfemale;
            }
            set
            {
                _maleOrfemale = value; OnPropertyRaised("MaleOrFemale");
                OnPropertyRaised("Voice");
            }
        }

        public string InputSource
        {
            get { return _inputSource; }
            set { _inputSource = value; OnPropertyRaised("InputSource"); }
        }

        public string subscriptionKey { get { return _subscriptionKey; } set { _subscriptionKey = value; OnPropertyRaised("subscriptionKey"); } }

        public string serviceRegion { get { return _serviceRegion; } set { _serviceRegion = value; OnPropertyRaised("serviceRegion"); } }

        public List<string> Languages { get { return getLanguages(); } }
        public List<string> MaleOrFemales { get { return getMaleOrFemale(); } }
        public List<string> InputSources { get { return getInputSources(); } }

        public string Voice
        {
            get
            {
                return getVoiceString(_languagestr, _maleOrfemale);
            }
        }

        public static List<string> getMaleOrFemale()
        {
            List<string> data = new List<string>()
            {
                "Female",
                "Male"
            };

            return data;
        }

        public static List<string> getLanguages()
        {
            //Speech service language support:
            //https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/language-support

            List<string> data = new List<string>() {
                "English(Australia)",
                "French(France)",
                "German(Germany)"
            };

            return data;
        }

        public static List<string> getInputSources()
        {
            List<string> data = new List<string>()
            {
                "Microphone speak",
                "Load from wav",
                "Load from mp3"
            };
            return data;
        }

        public void Dispose()
        {
            _instance?.Dispose();
            GC.SuppressFinalize(this);
        }
        #endregion


    }
}
