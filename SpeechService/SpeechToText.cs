using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Win32;
using System.Threading.Tasks;


namespace SpeechService
{
    public static class SpeechToText
    {
        public static LanguageVoice _LV = LanguageVoice.GetInstance();
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static SpeechRecognizer recognizer;

        public static async Task ContinuousRecognitionMicrophone()
        {

            var config = Microsoft.CognitiveServices.Speech.SpeechConfig.FromSubscription(_LV.subscriptionKey, _LV.serviceRegion);
            var stopRecognition = new TaskCompletionSource<int>();


            recognizer = new SpeechRecognizer(config);


            _LV.OutputText = "Say something..";
            recognizer.Recognizing += (s, e) =>
            {
                _LV.InputText = $"RECOGNIZING: Text={e.Result.Text}";
            };

            recognizer.Recognized += (s, e) =>
            {
                if (e.Result.Reason == ResultReason.RecognizedSpeech)
                {
                    _LV.InputText = $"RECOGNIZED: Text={e.Result.Text}";
                    _log.Info(_LV.InputText);
                }
                else if (e.Result.Reason == ResultReason.NoMatch)
                {
                    _LV.InputText = $"NOMATCH: Speech could not be recognized.";
                    _log.Info(_LV.InputText);
                }
            };

            recognizer.Canceled += (s, e) =>
            {
                _LV.InputText = $"CANCELED: Reason={e.Reason}";
                _log.Info(_LV.InputText);

                if (e.Reason == CancellationReason.Error)
                {
                    _LV.InputText = $"CANCELED: ErrorCode={e.ErrorCode}";
                    _LV.InputText = $"CANCELED: ErrorDetails={e.ErrorDetails}";
                    _LV.InputText = $"CANCELED: Did you update the subscription info?";
                }

                stopRecognition.TrySetResult(0);
            };

            recognizer.SessionStarted += (s, e) =>
            {
                _LV.InputText = "\n    Session started event.";
                _log.Info(_LV.InputText);
            };

            recognizer.SessionStopped += (s, e) =>
            {
                _LV.InputText = "\n    Session stopped event.";
                _log.Info(_LV.InputText);
                _LV.InputText = "\nStop recognition.";
                _log.Info(_LV.InputText);
                stopRecognition.TrySetResult(0);
            };

            // Starts continuous recognition. Uses StopContinuousRecognitionAsync() to stop recognition.
            await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

            Task.WaitAny(new[] { stopRecognition.Task });

            await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
            _LV.OutputText = "Stopped the service";

        }

        public static async Task ContinuousRecognitionWithFileAsync()
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            string filterStr = "";

            if (_LV.InputSource == "Load from mp3")
            {
                filterStr = "Supported Audio |*.mp3";
            }
            else
            {
                filterStr = "Supported Audio |*.wav";
            }

            openFileDialog.Filter = filterStr;
            if (openFileDialog.ShowDialog() == true)
            {
                var fileName = openFileDialog.FileName;
                var config = Microsoft.CognitiveServices.Speech.SpeechConfig.FromSubscription(_LV.subscriptionKey, _LV.serviceRegion);
                var stopRecognition = new TaskCompletionSource<int>();

                //--------------
                using (var audioInput = AudioConfig.FromWavFileInput(fileName))
                {
                    recognizer = new SpeechRecognizer(config, audioInput);

                    // Subscribes to events.
                    recognizer.Recognizing += (s, e) =>
                    {
                        _LV.InputText = $"RECOGNIZING: Text={e.Result.Text}";
                    };

                    recognizer.Recognized += (s, e) =>
                    {
                        if (e.Result.Reason == ResultReason.RecognizedSpeech)
                        {
                            _LV.InputText = $"RECOGNIZED: Text={e.Result.Text}";
                            _log.Info(_LV.InputText);
                        }
                        else if (e.Result.Reason == ResultReason.NoMatch)
                        {
                            _LV.InputText = $"NOMATCH: Speech could not be recognized.";
                            _log.Info(_LV.InputText);
                        }
                    };

                    recognizer.Canceled += (s, e) =>
                    {
                        _LV.InputText = $"CANCELED: Reason={e.Reason}";
                        _log.Info(_LV.InputText);

                        if (e.Reason == CancellationReason.Error)
                        {
                            _LV.InputText = $"CANCELED: ErrorCode={e.ErrorCode}";
                            _log.Info(_LV.InputText);
                            _LV.InputText = $"CANCELED: ErrorDetails={e.ErrorDetails}";
                            _log.Info(_LV.InputText);
                            _LV.InputText = $"CANCELED: Did you update the subscription info?";
                            _log.Info(_LV.InputText);
                        }

                        stopRecognition.TrySetResult(0);
                    };

                    recognizer.SessionStarted += (s, e) =>
                    {
                        _LV.InputText = "\n    Session started event.";
                        _log.Info(_LV.InputText);
                    };

                    recognizer.SessionStopped += (s, e) =>
                    {
                        _LV.InputText = "\n    Session stopped event.";
                        _log.Info(_LV.InputText);
                        _LV.InputText = "\nStop recognition.";
                        _log.Info(_LV.InputText);
                        stopRecognition.TrySetResult(0);
                    };

                    await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);
                    Task.WaitAny(new[] { stopRecognition.Task });
                    await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
                }

            }
        }
    }
}