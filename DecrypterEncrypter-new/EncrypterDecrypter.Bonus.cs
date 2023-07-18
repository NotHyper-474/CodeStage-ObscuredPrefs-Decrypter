using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using CodeStage_Decrypter.Utils;

namespace CodeStage_Decrypter
{
    public static partial class EncrypterDecrypter
    {
        private static readonly Type UIntType = typeof(UInt32);
        private static readonly Type IntType = typeof(Int32);
        private static readonly Type ULongType = typeof(UInt64);
        private static readonly Type LongType = typeof(Int64);
        private static readonly Type FloatType = typeof(Single);
        private static readonly Type DoubleType = typeof(Double);
        private static readonly Type BooleanType = typeof(Boolean);
        private static readonly Type StringType = typeof(String);

        internal enum DataType : byte
        {
            Unknown = 0,
            Short = 2,
            UShort = 4,
            Int = 5,
            UInt = 10,
            String = 15,
            Float = 20,
            Double = 25,
            Long = 30,
            ULong = 32,
            Boolean = 35,
            ByteArray = 40,
            Vector2 = 45,
            Vector2Int = 66,
            Vector3 = 50,
            Vector3Int = 70,
            Vector4 = 52,
            Quaternion = 55,
            Color = 60,
            Rect = 65
        }
        private const string RawNotFound = "{not_found}";
        internal static readonly byte[] savesTampered = Encoding.UTF8.GetBytes("SAVES_TAMPERED");
        internal static readonly byte[] olderVersion = Encoding.UTF8.GetBytes("Value uses older version of ACTk. Migration is not yet supported.");

        internal static char[] SuffixedCryptoKey(string dynamicSuffix = null)
        {
            dynamicSuffix ??= string.Empty;
            var suffixChars = dynamicSuffix.ToCharArray();
            var key = Properties.Settings.Default.SavedKey.ToCharArray();
            var result = new char[key.Length + suffixChars.Length];
            Buffer.BlockCopy(key, 0, result, 0, key.Length);
            Buffer.BlockCopy(suffixChars, 0, result, key.Length + 1, suffixChars.Length);
            return result;
        }

        public static string EncryptObject(object value, string key)
        {
            if (value is IConvertible convertible)
            {
                return EncryptValue(convertible, key);
            }
            if (value == null)
                return EncryptData(key, Array.Empty<byte>(), DataType.Unknown);

            var type = value.GetType();
            #region Vector Byte Conversions
            if (type == typeof(Vector2))
            {
                byte[] cleanBytes = new byte[2 * 4];
                var vec = (Vector2)value;
                var x = BitConverter.GetBytes(vec.x);
                var y = BitConverter.GetBytes(vec.y);
                Buffer.BlockCopy(x, 0, cleanBytes, 0, 4);
                Buffer.BlockCopy(y, 0, cleanBytes, 4, 4);
                return EncryptData(key, cleanBytes, DataType.Vector2);
            }
            else if (type == typeof(Vector3))
            {
                byte[] cleanBytes = new byte[3 * sizeof(float)];
                var vec = (Vector3)value;
                var x = BitConverter.GetBytes(vec.x);
                var y = BitConverter.GetBytes(vec.y);
                var z = BitConverter.GetBytes(vec.z);
                Buffer.BlockCopy(x, 0, cleanBytes, 0, 4);
                Buffer.BlockCopy(y, 0, cleanBytes, 4, 4);
                Buffer.BlockCopy(z, 0, cleanBytes, 8, 4);
                return EncryptData(key, cleanBytes, DataType.Vector3);
            }
            else if (type == typeof(Vector4))
            {
                byte[] cleanBytes = new byte[2 * 4];
                var vec = (Vector4)value;
                var x = BitConverter.GetBytes(vec.x);
                var y = BitConverter.GetBytes(vec.y);
                var z = BitConverter.GetBytes(vec.z);
                var w = BitConverter.GetBytes(vec.w);
                Buffer.BlockCopy(x, 0, cleanBytes, 0, 4);
                Buffer.BlockCopy(y, 0, cleanBytes, 4, 4);
                Buffer.BlockCopy(z, 0, cleanBytes, 8, 4);
                Buffer.BlockCopy(w, 0, cleanBytes, 12, 4);
                return EncryptData(key, cleanBytes, DataType.Vector4);
            }
            else if (type == typeof(Quaternion))
            {
                byte[] cleanBytes = new byte[2 * 4];
                var quat = (Quaternion)value;
                var x = BitConverter.GetBytes(quat.x);
                var y = BitConverter.GetBytes(quat.y);
                var z = BitConverter.GetBytes(quat.z);
                var w = BitConverter.GetBytes(quat.w);
                Buffer.BlockCopy(x, 0, cleanBytes, 0, 4);
                Buffer.BlockCopy(y, 0, cleanBytes, 4, 4);
                Buffer.BlockCopy(z, 0, cleanBytes, 8, 4);
                Buffer.BlockCopy(w, 0, cleanBytes, 12, 4);
                return EncryptData(key, cleanBytes, DataType.Quaternion);
            }
            else if (type == typeof(Color32))
            {
                var color = (Color32)value;
                var encodedColor = (uint)((color.a << 24) | (color.r << 16) | (color.g << 8) | color.b);
                byte[] cleanBytes = BitConverter.GetBytes(encodedColor);
                return EncryptData(key, cleanBytes, DataType.Color);
            }
            else if (type == typeof(byte[]))
                return EncryptData(key, (byte[])value, DataType.ByteArray);
            #endregion
            // TODO: Add more types
            return null;
        }

        public static object DecryptObject(string value, string key)
        {
            var cleanBytes = DecryptData(key, value);
            var kind = GetRawDataType(value);
            if (kind == DataType.ByteArray)
                return cleanBytes;
            if (cleanBytes == null || cleanBytes.Length == 0)
                return null;

            switch (kind)
            {
                case DataType.Unknown:
                    return null;
                case DataType.Short:
                    return BitConverter.ToInt16(cleanBytes, 0);
                case DataType.UShort:
                    return BitConverter.ToUInt16(cleanBytes, 0);
                case DataType.Int:
                    return BitConverter.ToInt32(cleanBytes, 0);
                case DataType.UInt:
                    return BitConverter.ToUInt32(cleanBytes, 0);
                case DataType.String:
                    return Encoding.UTF8.GetString(cleanBytes, 0, cleanBytes.Length);
                case DataType.Float:
                    return BitConverter.ToSingle(cleanBytes, 0);
                case DataType.Double:
                    return BitConverter.ToDouble(cleanBytes, 0);
                case DataType.Long:
                    return BitConverter.ToInt64(cleanBytes, 0);
                case DataType.ULong:
                    return BitConverter.ToUInt64(cleanBytes, 0);
                case DataType.Boolean:
                    return BitConverter.ToBoolean(cleanBytes, 0);
                case DataType.Vector2:
                case DataType.Vector3:
                case DataType.Vector4:
                case DataType.Quaternion:
                case DataType.Rect:
                    return GetVector(cleanBytes, kind);
                case DataType.Vector2Int:
                    break;
                case DataType.Vector3Int:
                    break;
                case DataType.Color:
                    var encodedColor = BitConverter.ToUInt32(cleanBytes);
                    var a = (byte)(encodedColor >> 24);
                    var r = (byte)(encodedColor >> 16);
                    var g = (byte)(encodedColor >> 8);
                    var b = (byte)(encodedColor >> 0);
                    return new Color32(r, g, b, a);
            }
            return null;
        }

        private static object GetVector(byte[] bytes, DataType type)
        {
            // Technically all these value types get encrypted/decrypted the same way
            //const float IGNORE = 0xDEADBEEF;
            var x = BitConverter.ToSingle(bytes, 0);
            var y = BitConverter.ToSingle(bytes, 4);
            if (type == DataType.Vector2)
            {
                return new Vector2(x, y);
            }
            var z = BitConverter.ToSingle(bytes, 8);
            if (type == DataType.Vector3)
            {
                return new Vector3(x, y, z);
            }
            var w = BitConverter.ToSingle(bytes, 12);
            return type switch
            {
                DataType.Vector4 => new Vector4(x, y, z, w),
                DataType.Quaternion => new Quaternion(x, y, z, w),
                _ => new Rect(x, y, z, w)
            };
        }

        private static string EncryptData(string key, byte[] cleanBytes, DataType type)
        {
            var dataLength = cleanBytes.Length;
            byte[] encryptedBytes;
            if (Version >= 3)
                encryptedBytes = EncryptDecryptBytes(cleanBytes, SuffixedCryptoKey(key));
            else
                encryptedBytes = EncryptDecryptBytesObsolete(cleanBytes, key + Properties.Settings.Default.SavedKey);

            var dataHash = xxHash.CalculateHash(cleanBytes, dataLength, 0);
            var dataHashBytes = new byte[4];
            dataHashBytes[0] = (byte)(dataHash & 0xFF);
            dataHashBytes[1] = (byte)((dataHash >> 8) & 0xFF);
            dataHashBytes[2] = (byte)((dataHash >> 16) & 0xFF);
            dataHashBytes[3] = (byte)((dataHash >> 24) & 0xFF);

            byte[] deviceHashBytes = null;
            int finalBytesLength;
            if (lockToDevice != DeviceLockLevel.None)
            {
                // 4 device id hash + 1 data type + 1 device lock mode + 1 version + 4 data hash
                finalBytesLength = dataLength + 11;
                var deviceHash = DeviceIdHash;
                deviceHashBytes = new byte[4]; // replaces BitConverter.GetBytes(hash);
                deviceHashBytes[0] = (byte)(deviceHash & 0xFF);
                deviceHashBytes[1] = (byte)((deviceHash >> 8) & 0xFF);
                deviceHashBytes[2] = (byte)((deviceHash >> 16) & 0xFF);
                deviceHashBytes[3] = (byte)((deviceHash >> 24) & 0xFF);
            }
            else
            {
                // 1 data type + 1 device lock mode + 1 version + 4 data hash
                finalBytesLength = dataLength + 7;
            }

            var finalBytes = new byte[finalBytesLength];
            Buffer.BlockCopy(encryptedBytes, 0, finalBytes, 0, dataLength);
            if (deviceHashBytes != null)
                Buffer.BlockCopy(deviceHashBytes, 0, finalBytes, dataLength, 4);

            finalBytes[finalBytesLength - 7] = (byte)type;
            finalBytes[finalBytesLength - 6] = Version;
            finalBytes[finalBytesLength - 5] = (byte)lockToDevice;
            
            Buffer.BlockCopy(dataHashBytes, 0, finalBytes, finalBytesLength - 4, 4);

            return Convert.ToBase64String(finalBytes);
        }

        private static byte[] DecryptData(string key, string encryptedInput)
        {
            byte[] inputBytes;

            try
            {
                inputBytes = Convert.FromBase64String(encryptedInput);
            }
            catch (Exception)
            {
                if (encryptedInput.TrimEnd().EndsWith('.') || encryptedInput.TrimEnd().EndsWith(char.MinValue))
                    MessageBox.Show("Invalid character at end of input '.'", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                Console.Error.WriteLine(encryptedInput);
                return savesTampered;
            }

            if (inputBytes.Length <= 0)
            {
                var tamp = savesTampered;
                tamp[^7] = (byte)DataType.ByteArray;
                return tamp;
            }

            var inputLength = inputBytes.Length;

            var inputVersion = inputBytes[inputLength - 6];
            if (inputVersion != Version)
            {
                if (inputVersion < Version)
                {
                    return olderVersion;
                }
            }

            var inputLockToDevice = (DeviceLockLevel)inputBytes[inputLength - 5];

            var dataHashBytes = new byte[4];
            Buffer.BlockCopy(inputBytes, inputLength - 4, dataHashBytes, 0, 4);
            var inputDataHash = (uint)(dataHashBytes[0] | dataHashBytes[1] << 8 | dataHashBytes[2] << 16 | dataHashBytes[3] << 24);

            int dataBytesLength;
            uint inputDeviceHash = 0;

            if (inputLockToDevice != DeviceLockLevel.None)
            {
                dataBytesLength = inputLength - 11;
                if (lockToDevice != DeviceLockLevel.None)
                {
                    var deviceHashBytes = new byte[4];
                    Buffer.BlockCopy(inputBytes, dataBytesLength, deviceHashBytes, 0, 4);
                    inputDeviceHash = (uint)(deviceHashBytes[0] | deviceHashBytes[1] << 8 | deviceHashBytes[2] << 16 | deviceHashBytes[3] << 24);
                }
            }
            else dataBytesLength = inputLength - 7;

            if (dataBytesLength < 0)
            {
                MessageBox.Show("Invalid input data for value.", "Invalid input data", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return Array.Empty<byte>();
            }

            var encryptedBytes = new byte[dataBytesLength];
            Buffer.BlockCopy(inputBytes, 0, encryptedBytes, 0, dataBytesLength);

            byte[] cleanBytes;
            if (inputVersion >= 3 && Version >= 3)
                cleanBytes = EncryptDecryptBytes(encryptedBytes, SuffixedCryptoKey(key));
            else
                cleanBytes = EncryptDecryptBytesObsolete(encryptedBytes, key + Properties.Settings.Default.SavedKey);

            var realDataHash = xxHash.CalculateHash(cleanBytes, dataBytesLength, 0);
            if (realDataHash != inputDataHash)
            {
                Console.WriteLine($"Possible tampered save in: {encryptedInput}");
                return savesTampered;
            }

            if (inputDeviceHash != 0)
            {
                var realDeviceHash = DeviceIdHash;
                if (inputDeviceHash != realDeviceHash)
                {
                    return Encoding.UTF8.GetBytes("POSSIBLE_FOREIGN_SAVE");
                }
            }

            return cleanBytes;
        }

        internal static string EncryptValue<T>(T value, string key) where T : IConvertible
        {
            var cleanBytes = default(byte[]);
            var dataType = DataType.Unknown;

            var genericType = typeof(T);
            if (genericType == typeof(IConvertible))
                genericType = value.GetType();

            if (genericType == IntType)
            {
                dataType = DataType.Int;
                cleanBytes = BitConverter.GetBytes(value.ToInt32(null));
            }
            else if (genericType == UIntType)
            {
                dataType = DataType.UInt;
                cleanBytes = BitConverter.GetBytes(value.ToUInt32(null));
            }
            else if (genericType == ULongType)
            {
                dataType = DataType.ULong;
                cleanBytes = BitConverter.GetBytes(value.ToUInt64(null));
            }
            else if (genericType == LongType)
            {
                dataType = DataType.Long;
                cleanBytes = BitConverter.GetBytes(value.ToInt64(null));
            }
            else if (genericType == FloatType)
            {
                dataType = DataType.Float;
                cleanBytes = BitConverter.GetBytes(value.ToSingle(null));
            }
            else if (genericType == DoubleType)
            {
                dataType = DataType.Double;
                cleanBytes = BitConverter.GetBytes(value.ToDouble(null));
            }
            else if (genericType == BooleanType)
            {
                dataType = DataType.Boolean;
                cleanBytes = BitConverter.GetBytes(value.ToBoolean(null));
            }
            else if (genericType == StringType)
            {
                dataType = DataType.String;
                cleanBytes = Encoding.UTF8.GetBytes(value.ToString());
            }

            return EncryptData(key, cleanBytes, dataType);
        }

        internal static T DecryptValue<T>(string key, string encryptedKey, T defaultValue, string encryptedInput = null)
        {
            if (encryptedInput == RawNotFound)
                return defaultValue;

            var cleanBytes = DecryptData(key, encryptedInput);
            if (cleanBytes == null)
                return defaultValue;

            var cleanValue = default(T);
            var genericType = typeof(T);

            if (genericType == IntType)
            {
                cleanValue = (T)(object)BitConverter.ToInt32(cleanBytes, 0);
            }
            else if (genericType == UIntType)
            {
                cleanValue = (T)(object)BitConverter.ToUInt32(cleanBytes, 0);
            }
            else if (genericType == StringType)
            {
                cleanValue = (T)(object)Encoding.UTF8.GetString(cleanBytes, 0, cleanBytes.Length);
            }
            else if (genericType == FloatType)
            {
                cleanValue = (T)(object)BitConverter.ToSingle(cleanBytes, 0);
            }
            else if (genericType == DoubleType)
            {
                cleanValue = (T)(object)BitConverter.ToDouble(cleanBytes, 0);
            }
            else if (genericType == LongType)
            {
                cleanValue = (T)(object)BitConverter.ToInt64(cleanBytes, 0);
            }
            else if (genericType == ULongType)
            {
                cleanValue = (T)(object)BitConverter.ToUInt64(cleanBytes, 0);
            }
            else if (genericType == BooleanType)
            {
                cleanValue = (T)(object)BitConverter.ToBoolean(cleanBytes, 0);
            }

            return cleanValue;
        }

        internal static uint CalculateChecksum(string input)
        {
            // TODO: Probably change this if migration is implemented
            byte[] inputBytes = Encoding.UTF8.GetBytes(SuffixedCryptoKey(input));
            return xxHash.CalculateHash(inputBytes, inputBytes.Length, 0);
        }
    }
}
