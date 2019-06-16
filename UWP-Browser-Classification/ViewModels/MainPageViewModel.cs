﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using UWP_Browser_Classification.Enums;
using UWP_Browser_Classification.ML;

namespace UWP_Browser_Classification.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private Prediction _prediction = new Prediction();

        private bool _enableGoButton;

        public bool EnableGoButton
        {
            get => _enableGoButton;

            private set
            {
                _enableGoButton = value;
                OnPropertyChanged();
            }
        }

        private string _webServiceURL;

        public string WebServiceURL
        {
            get => _webServiceURL;

            set
            {
                _webServiceURL = value;

                OnPropertyChanged();

                EnableGoButton = !string.IsNullOrEmpty(value);
            }
        }

        public Uri BuildUri()
        {
            var webServiceUrl = WebServiceURL;

            if (!webServiceUrl.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase) &&
                !webServiceUrl.StartsWith("https://", StringComparison.CurrentCultureIgnoreCase))
            {
                webServiceUrl = $"http://{webServiceUrl}";
            }

            return new Uri(webServiceUrl);
        }

        public (Classification ClassificationResult, string BrowserContent) Classify(string html)
        {
            var result = _prediction.Predict(html);

            return result == Classification.BENIGN ? 
                (Classification.BENIGN, string.Empty) : 
                (Classification.MALICIOUS, $"<html><body>{WebServiceURL} was found to be a malicious site</body></html>");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}