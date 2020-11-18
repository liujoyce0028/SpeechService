using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Win32;
using System;
using System.Threading.Tasks;

namespace SpeechService
{
    static class TextToSpeech
    {
        public static LanguageVoice _LV = LanguageVoice.GetInstance();
        public static SpeechSynthesizer synthesizer;


        public async static Task PlayTheTextAsync()
        {
            // Creates an instance of a speech config with specified subscription key and service region.
            // Replace with your own subscription key and service region (e.g., "westus").
            var config = Microsoft.CognitiveServices.Speech.SpeechConfig.FromSubscription(_LV.subscriptionKey, _LV.serviceRegion);

            synthesizer = new SpeechSynthesizer(config);

            //TODO
            //synthesizer.WordBoundary += (s, e) =>
            //{
            //    // The unit of e.AudioOffset is tick (1 tick = 100 nanoseconds), divide by 10,000 to convert to milliseconds.
            //    _LV.OutputText = $"Word boundary event received. Audio offset: " +
            //        $"{(e.AudioOffset + 5000) / 10000}ms, text offset: {e.TextOffset}, word length: {e.WordLength}.";
            //    stopRecognition.TrySetResult(0);
            //};
        
            // Creates a speech synthesizer using the default speaker as audio output.

            using (var result = await synthesizer.SpeakSsmlAsync(_LV.getInputText_Voice))
            {
                
                if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                {
                    _LV.OutputText = "Speech synthesized to speaker:" + DateTime.Now;
                }
                else if (result.Reason == ResultReason.Canceled)
                {
                    var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                    _LV.OutputText = $"CANCELED: Reason={cancellation.Reason}";

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        _LV.OutputText = $"CANCELED: ErrorCode={cancellation.ErrorCode}"
                            + $"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]";
                    }
                }
            }
          

        }

        public async static Task SaveTheWavOrMp3()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            
            string filterStr = "";
            if (_LV.mp3Selected)
            {
                filterStr = "Supported Audio |*.mp3";
            }
            else
            {
                filterStr = "Supported Audio |*.wav";
            }

            saveFileDialog.Filter = filterStr;
            if (saveFileDialog.ShowDialog() == true)
            {
                // File.WriteAllText(saveFileDialog.FileName, txtEditor.Text);
                var config = SpeechConfig.FromSubscription(_LV.subscriptionKey, _LV.serviceRegion);

                // Creates a speech synthesizer using file as audio output.
                // Replace with your own audio file name.
                var fileName = saveFileDialog.FileName;

                using (var fileOutput = AudioConfig.FromWavFileOutput(fileName))
                {
                    using (var synthesizer = new SpeechSynthesizer(config, fileOutput))
                    {
                        while (true)
                        {
                            // Receives a text from console input and synthesize it to wave file.
                            string text = _LV.getInputText_Voice;
                            if (string.IsNullOrEmpty(text))
                            {
                                break;
                            }

                            using (var result = await synthesizer.SpeakSsmlAsync(text))
                            {
                                if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                                {
                                    _LV.OutputText = $"Speech synthesized for text, and the audio was saved to [{fileName}]";
                                }
                                else if (result.Reason == ResultReason.Canceled)
                                {
                                    var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                                    _LV.OutputText = $"CANCELED: Reason={cancellation.Reason}";

                                    if (cancellation.Reason == CancellationReason.Error)
                                    {
                                        _LV.OutputText = $"CANCELED: ErrorCode={cancellation.ErrorCode}" + $"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]";
                                    }
                                }
                            }
                        }
                    }
                }



            }

        }


    }
}