using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using CodeStage_Decrypter.Utils;

namespace CodeStage_Decrypter
{
    public static partial class EncrypterDecrypter
    {
        public const string PrefsKey = "9978e9f39c218d674463dab9dc728bd6";

        public static DeviceLockLevel lockToDevice = DeviceLockLevel.None;

        internal static string deviceId;
        public static string DeviceId
        {
            get
            {
                if (string.IsNullOrEmpty(deviceId))
                {
                    deviceId = GetDeviceId();
                }
                return deviceId;
            }
            set => deviceId = value;
        }

        internal static uint deviceIdHash;
        internal static uint DeviceIdHash
        {
            get
            {
                if (deviceIdHash == 0)
                    deviceIdHash = CalculateChecksum(DeviceId);
                return deviceIdHash;
            }
        }

        public enum DeviceLockLevel : byte
        {
            None,
            Soft,
            Strict
        }
        
        public static byte Version = 3;

        internal static DataType GetRawDataType(string value)
        {
            var result = DataType.Unknown;
            byte[] inputBytes;

            try
            {
                inputBytes = Convert.FromBase64String(value);
            }
            catch
            {
                return result;
            }

            if (inputBytes.Length < 7)
                return result;

            var inputLength = inputBytes.Length;

            result = (DataType)inputBytes[inputLength - 7];

            var version = inputBytes[inputLength - 6];
            if (version > 10)
                result = DataType.Unknown;

            return result;
        }

        public static string Encrypt(string value, char[] key)
        {
            return Encrypt(value.ToCharArray(), key);
        }

        internal static string Encrypt(char[] value, char[] key)
        {
            return Base64Utils.CharArrayToBase64(EncryptDecrypt(value, key));
        }

        internal static char[] Decrypt(char[] value, char[] key)
        {
            var decoded = Base64Utils.FromBase64ToCharArray(value);
            return EncryptDecrypt(decoded, key);
        }

        public static string Decrypt(string value, char[] key)
        {
            return new(Decrypt(value.ToCharArray(), key));
        }

        private static char[] EncryptDecrypt(IReadOnlyList<char> value, IReadOnlyList<char> key)
        {
            var valueLength = value.Count;
            var keyLength = key.Count;

            var result = new char[valueLength];
            for (int i = 0; i < valueLength; i++)
            {
                result[i] = (char)(value[i] ^ key[i % keyLength]);
            }
            return result;
        }

        private static byte[] EncryptDecryptBytes(IReadOnlyList<byte> value, IReadOnlyList<char> key)
        {
            var valueLength = value.Count;
            var keyLength = key.Count;

            var result = new byte[valueLength];
            for (int i = 0; i < valueLength; i++)
            {
                result[i] = (byte)(value[i] ^ key[i % keyLength]);
            }

            return result;
        }

        private static byte[] EncryptDecryptBytesObsolete(IReadOnlyList<byte> value, string key)
        {
            var valueLength = value.Count;
            var keyLength = key.Length;

            var result = new byte[valueLength];
            for (int i = 0; i < valueLength; i++)
            {
                result[i] = (byte)(value[i] ^ key[i % keyLength]);
            }

            return result;
        }

        private static string GetDeviceId()
        {
            var id = "";
            // TODO: Simulate DeviceId for other platforms?
            #if UNITY_IPHONE
            id = UnityEngine.iOS.Device.vendorIdentifier;
            #endif

            #if !ACTK_PREVENT_READ_PHONE_STATE
            if (string.IsNullOrEmpty(id)) id = SystemInfo.deviceUniqueIdentifier;
            #endif
            return id;
        }
    }
}
