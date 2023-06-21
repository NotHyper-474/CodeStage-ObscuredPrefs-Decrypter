using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeStage_Decrypter
{
    public class PrefsKey
    {
        public PrefsKey(string name, object value, out uint oldHash)
        {
            Name = name;
            Value = value;
            oldHash = this.oldHash;
        }

        public PrefsKey(string name, object value) : this(name, value, out _)
        { }

        public string HashedName
        {
            get => isEditing || isInvalid ? Name : GetHashedKey(Name);
            set
            {
                Name = value.Contains("_h") ? value.Remove(value.IndexOf("_h")) : value;
                if (isInvalid) return;
                oldHash = Hash;
                Hash = GetHash(Name);
            }
        }
        public string Name { get; set; }
        public object Value
        {
            get => _value;
            set
            {
                // PlayerPrefs can only have float, int, and string
                if (value != null && value.GetType() == typeof(double))
                {
                    _value = (float)(double)value;
                    return;
                }
                _value = value;
            }
        }
        public uint Hash { get; set; }

        public uint oldHash { get; protected set; }

        public bool isEditing;
        public bool isInvalid;

        private object _value;

        public void Encrypt(string cryptoKey)
        {
            if (isInvalid) return;
            var decryptedPrefs = Name;
            HashedName = EncrypterDecrypter.Encrypt(Name, cryptoKey.ToCharArray());
            Value = EncrypterDecrypter.EncryptObject(Value, decryptedPrefs);
        }

        public bool Decrypt(string cryptoKey)
        {
            if (!Base64Utils.IsBase64(Name))
                return false;
            // Clear (possible) trailing character
            if (Value is string str)
                Value = str.TrimEnd(char.MinValue);
            HashedName = EncrypterDecrypter.Decrypt(Name, cryptoKey.ToCharArray());
            Value = EncrypterDecrypter.DecryptObject(Value.ToString(), Name);
            return true;
        }

        public static bool IsPrefsKey(string key)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(key, @"^[A-Za-z0-9]*_h[0-9]{1,}$");
        }

        public static uint GetHash(string key)
        {
            // Thanks for this guy in Unity Answers for the code
            // http://answers.unity.com/answers/208076/view.html
            uint hash = 5381;
            foreach (char c in key)
                hash = hash * 33 ^ c;
            return hash;
        }

        public static string GetHashedKey(string key) => key + "_h" + GetHash(key);
    }
}
