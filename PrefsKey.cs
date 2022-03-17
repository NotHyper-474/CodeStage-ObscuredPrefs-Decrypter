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

        public string HashedName
        {
            get => isEditing ? Name : GetHashedKey(Name);
            set
            {
                Name = value.Contains("_h") ? value.Remove(value.IndexOf("_h")) : value;
                oldHash = Hash;
                Hash = GetHash(Name);
            }
        }
        public string Name { get; set; }
        public object Value { get; set; }
        public uint Hash { get; set; }

        public uint oldHash { get; protected set; }

        public bool isEditing;

        public void Encrypt(string key)
        {
            HashedName = EncrypterDecrypter.Encrypt(Name, key.ToCharArray());
            Value = EncrypterDecrypter.EncryptObject(Value, key);
        }

        public bool Decrypt(string key)
        {
            if (!Base64Utils.IsBase64(Name))
                return false;
            // Clear (possible) trailing character
            if (Value.GetType() == typeof(string))
                Value = ((string)Value).TrimEnd(char.MinValue);
            HashedName = EncrypterDecrypter.Decrypt(Name, key.ToCharArray());
            Value = EncrypterDecrypter.DecryptObject(Value.ToString(), key);
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
