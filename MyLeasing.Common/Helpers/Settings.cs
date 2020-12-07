using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyLeasing.Common.Helpers
{
    public class Settings
    {

        private const string _property = "property";
        private static readonly string _stringDefault = string.Empty;
        private const string _token = "token";
        private const string _owner = "owner";

        private static ISettings AppSettings => CrossSettings.Current;

        public static string Property
        {
            get => AppSettings.GetValueOrDefault(_property, _stringDefault);
            set => AppSettings.AddOrUpdateValue(_property, value);
        }

        public static string Token
        {
            get => AppSettings.GetValueOrDefault(_token, _stringDefault);
            set => AppSettings.AddOrUpdateValue(_token, value);
        }

        public static string Owner
        {
            get => AppSettings.GetValueOrDefault(_owner, _stringDefault);
            set => AppSettings.AddOrUpdateValue(_owner, value);
        }

  



    }
}
