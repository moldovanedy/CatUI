using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace CatUI.Utils
{
    public static class BinaryUtils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GetBit(int value, int index)
        {
            return (value & (1 << index)) != 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GetBit(long value, int index)
        {
            return (value & (1L << index)) != 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBit(ref int value, bool bitValue, int index)
        {
            if (bitValue)
            {
                value |= 1 << index;
            }
            else
            {
                value &= ~(1 << index);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBit(ref long value, bool bitValue, int index)
        {
            if (bitValue)
            {
                value |= 1L << index;
            }
            else
            {
                value &= ~(1L << index);
            }
        }

        #region Value to bytes

        public static byte[] ConvertIntToBytes(int variable)
        {
            byte[] bytes = new byte[4];
            bytes[0] = (byte)(variable >> 24);
            bytes[1] = (byte)(variable >> 16);
            bytes[2] = (byte)(variable >> 8);
            bytes[3] = (byte)variable;
            return bytes;
        }

        public static byte[] ConvertUintToBytes(uint variable)
        {
            byte[] bytes = new byte[4];
            bytes[0] = (byte)(variable >> 24);
            bytes[1] = (byte)(variable >> 16);
            bytes[2] = (byte)(variable >> 8);
            bytes[3] = (byte)variable;
            return bytes;
        }

        public static byte[] ConvertShortToBytes(int variable)
        {
            byte[] bytes = new byte[2];
            bytes[0] = (byte)(variable >> 8);
            bytes[1] = (byte)variable;
            return bytes;
        }

        public static byte[] ConvertUshortToBytes(uint variable)
        {
            byte[] bytes = new byte[2];
            bytes[0] = (byte)(variable >> 8);
            bytes[1] = (byte)variable;
            return bytes;
        }

        public static unsafe byte[] ConvertFloatToBytes(float variable)
        {
            int valAsInt = *(int*)&variable;
            return ConvertIntToBytes(valAsInt);
        }

        public static byte[] ConvertBoolToBytes(bool variable)
        {
            return [(byte)(variable ? 1 : 0)];
        }

        public static byte[] ConvertCharToBytes(char variable)
        {
            return [(byte)variable];
        }

        public static byte[] ConvertStringToBytes(string variable)
        {
            byte[] buffer = new byte[variable.Length + 1];
            Encoding.ASCII.GetBytes(variable).CopyTo(buffer, 0);
            //string terminator
            buffer[variable.Length] = 0x00;

            return buffer;
        }

        public static byte[] ConvertLongToBytes(long variable)
        {
            byte[] bytes = new byte[8];
            bytes[0] = (byte)(variable >> 56);
            bytes[1] = (byte)(variable >> 48);
            bytes[2] = (byte)(variable >> 40);
            bytes[3] = (byte)(variable >> 32);
            bytes[4] = (byte)(variable >> 24);
            bytes[5] = (byte)(variable >> 16);
            bytes[6] = (byte)(variable >> 8);
            bytes[7] = (byte)variable;
            return bytes;
        }

        public static unsafe byte[] ConvertDoubleToBytes(double variable)
        {
            long valAsLong = *(long*)&variable;
            return ConvertLongToBytes(valAsLong);
        }


        /// <summary>
        /// Converts an int to bytes without allocating and returning a new byte array. Instead, it modifies the passed byte array.
        /// </summary>
        /// <param name="variable">The variable to convert.</param>
        /// <param name="bytes">The byte array to modify.</param>
        /// <param name="startIndex">The index from which to start the modification (will modify 4 bytes).</param>
        public static void ConvertIntToBytes(int variable, byte[] bytes, int startIndex)
        {
            bytes[startIndex] = (byte)(variable >> 24);
            bytes[startIndex + 1] = (byte)(variable >> 16);
            bytes[startIndex + 2] = (byte)(variable >> 8);
            bytes[startIndex + 3] = (byte)variable;
        }

        public static void ConvertUintToBytes(uint variable, byte[] bytes, int startIndex)
        {
            bytes[startIndex] = (byte)(variable >> 24);
            bytes[startIndex + 1] = (byte)(variable >> 16);
            bytes[startIndex + 2] = (byte)(variable >> 8);
            bytes[startIndex + 3] = (byte)variable;
        }

        public static void ConvertShortToBytes(int variable, byte[] bytes, int startIndex)
        {
            bytes[startIndex] = (byte)(variable >> 8);
            bytes[startIndex + 1] = (byte)variable;
        }

        public static void ConvertUshortToBytes(uint variable, byte[] bytes, int startIndex)
        {
            bytes[startIndex] = (byte)(variable >> 8);
            bytes[startIndex + 1] = (byte)variable;
        }

        /// <summary>
        /// Converts a float to bytes without allocating and returning a new byte array. Instead, it modifies the passed byte array.
        /// Uses unsafe context.
        /// </summary>
        /// <param name="variable">The variable to convert.</param>
        /// <param name="bytes">The byte array to modify.</param>
        /// <param name="startIndex">The index from which to start the modification (will modify 4 bytes).</param>
        public static unsafe void ConvertFloatToBytes(float variable, byte[] bytes, int startIndex)
        {
            int valAsInt = *(int*)&variable;
            ConvertIntToBytes(valAsInt, bytes, startIndex);
        }

        /// <summary>
        /// Converts a bool to bytes without allocating and returning a new byte array. Instead, it modifies the passed byte array.
        /// </summary>
        /// <param name="variable">The variable to convert.</param>
        /// <param name="bytes">The byte array to modify.</param>
        /// <param name="startIndex">The index from which to start the modification (will modify 1 byte).</param>
        public static void ConvertBoolToBytes(bool variable, byte[] bytes, int startIndex)
        {
            bytes[startIndex] = (byte)(variable ? 1 : 0);
        }

        /// <summary>
        /// Converts a char to bytes (as ASCII) without allocating and returning a new byte array. Instead, it modifies the passed byte array.
        /// </summary>
        /// <param name="variable">The variable to convert.</param>
        /// <param name="bytes">The byte array to modify.</param>
        /// <param name="startIndex">The index from which to start the modification (will modify 1 byte).</param>
        public static void ConvertCharToBytes(char variable, byte[] bytes, int startIndex)
        {
            //bytes[startIndex] = Encoding.ASCII.GetBytes(new char[] { variable })[0];
            bytes[startIndex] = (byte)variable;
        }

        /// <summary>
        /// Converts a string to bytes (as ASCII) without allocating and returning a new byte array. Instead, it modifies the passed byte array.
        /// Will require a number of bytes equal to the number of string characters plus one for the string terminator.
        /// </summary>
        /// <param name="variable">The variable to convert.</param>
        /// <param name="bytes">The byte array to modify.</param>
        /// <param name="startIndex">The index from which to start the modification (will modify a number of bytes equal to the number of string characters plus one for the string terminator).</param>
        public static void ConvertStringToBytes(string variable, byte[] bytes, int startIndex)
        {
            Encoding.ASCII.GetBytes(variable).CopyTo(bytes, startIndex);
            bytes[variable.Length] = 0x00;
        }

        public static void ConvertLongToBytes(long variable, byte[] bytes, int startIndex)
        {
            bytes[startIndex] = (byte)(variable >> 56);
            bytes[startIndex + 1] = (byte)(variable >> 48);
            bytes[startIndex + 2] = (byte)(variable >> 40);
            bytes[startIndex + 3] = (byte)(variable >> 32);
            bytes[startIndex + 4] = (byte)(variable >> 24);
            bytes[startIndex + 5] = (byte)(variable >> 16);
            bytes[startIndex + 6] = (byte)(variable >> 8);
            bytes[startIndex + 7] = (byte)variable;
        }

        public static unsafe void ConvertDoubleToBytes(double variable, byte[] bytes, int startIndex)
        {
            long valAsLong = *(long*)&variable;
            ConvertLongToBytes(valAsLong, bytes, startIndex);
        }

        #endregion

        #region Bytes to value

        public static int ConvertBytesToInt(byte[] bytes, int startIndex)
        {
            if (bytes.Length < 4)
            {
                int value = 0;
                for (int i = bytes.Length - 1; i >= 0; i--)
                {
                    value |= bytes[startIndex + i] << (8 * (bytes.Length - 1 - i));
                }

                return value;
            }
            else
            {
                return
                    (bytes[startIndex] << 24) |
                    (bytes[startIndex + 1] << 16) |
                    (bytes[startIndex + 2] << 8) |
                    bytes[startIndex + 3];
            }
        }

        public static uint ConvertBytesToUint(byte[] bytes, int startIndex)
        {
            if (bytes.Length < 4)
            {
                uint value = 0;
                for (int i = bytes.Length - 1; i >= 0; i--)
                {
                    value |= (uint)bytes[startIndex + i] << (8 * (bytes.Length - 1 - i));
                }

                return value;
            }
            else
            {
                return
                    (uint)((bytes[startIndex] << 24) |
                           (bytes[startIndex + 1] << 16) |
                           (bytes[startIndex + 2] << 8) |
                           bytes[startIndex + 3]);
            }
        }

        public static short ConvertBytesToShort(byte[] bytes, int startIndex)
        {
            if (bytes.Length < 2)
            {
                short value = 0;
                for (int i = bytes.Length - 1; i >= 0; i--)
                {
                    value |= (short)(bytes[startIndex + i] << (8 * (bytes.Length - 1 - i)));
                }

                return value;
            }
            else
            {
                return
                    (short)((bytes[startIndex] << 8) |
                            bytes[startIndex + 1]);
            }
        }

        public static ushort ConvertBytesToUshort(byte[] bytes, int startIndex)
        {
            if (bytes.Length < 2)
            {
                ushort value = 0;
                for (int i = bytes.Length - 1; i >= 0; i--)
                {
                    value |= (ushort)(bytes[startIndex + i] << (8 * (bytes.Length - 1 - i)));
                }

                return value;
            }
            else
            {
                return
                    (ushort)((bytes[startIndex] << 8) |
                             bytes[startIndex + 1]);
            }
        }

        public static unsafe float ConvertBytesToFloat(byte[] bytes, int startIndex)
        {
            int valAsInt = ConvertBytesToInt(bytes, startIndex);
            return *(float*)&valAsInt;
        }

        public static bool ConvertBytesToBool(byte[] bytes, int startIndex)
        {
            return bytes[startIndex] != 0;
        }

        public static char ConvertBytesToChar(byte[] bytes, int startIndex)
        {
            return (char)bytes[startIndex];
        }

        public static string ConvertBytesToString(byte[] bytes, int startIndex)
        {
            if (bytes.Length == 0)
            {
                return string.Empty;
            }

            int length = 0;
            while (bytes[startIndex] != 0)
            {
                startIndex++;
                length++;
            }

            return Encoding.ASCII.GetString(bytes, startIndex, length);
        }

        public static long ConvertBytesToLong(byte[] bytes, int startIndex)
        {
            if (bytes.Length < 8)
            {
                int value = 0;
                for (int i = bytes.Length - 1; i >= 0; i--)
                {
                    value |= bytes[startIndex + i] << (8 * (bytes.Length - 1 - i));
                }

                return value;
            }
            else
            {
                return
                    ((long)bytes[startIndex] << 56) |
                    ((long)bytes[startIndex + 1] << 48) |
                    ((long)bytes[startIndex + 2] << 40) |
                    ((long)bytes[startIndex + 3] << 32) |
                    ((long)bytes[startIndex + 4] << 24) |
                    ((long)bytes[startIndex + 5] << 16) |
                    ((long)bytes[startIndex + 6] << 8) |
                    bytes[startIndex + 7];
            }
        }

        public static unsafe double ConvertBytesToDouble(byte[] bytes, int startIndex)
        {
            long valAsLong = ConvertBytesToLong(bytes, startIndex);
            return *(double*)&valAsLong;
        }

        /// <summary>
        /// Converts an array of bytes to a string. Does not perform allocations (except string).
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="startIndex"></param>
        /// <param name="length">The length of the string (should not include string terminator (0x00)).</param>
        /// <returns></returns>
        public static string ConvertBytesToString(byte[] bytes, int startIndex, int length)
        {
            return Encoding.ASCII.GetString(bytes, startIndex, length);
        }

        #endregion

        /// <summary>
        /// Given a number, will return the minimum bytes required for storing that uint
        /// (ex. if the number is under 256, it will need 1 byte, else if it's under 65 536 will need 2 bytes etc.).
        /// </summary>
        /// <param name="data">The number to analyze.</param>
        /// <returns>The minimum number of bytes required for storing the number or 0 if something went wrong.</returns>
        public static byte GetUintMinimumStorageData(uint data)
        {
            switch (data)
            {
                case <= 0xff:
                    return 1;
                case <= 0xff_ff:
                    return 2;
                case <= 0xff_ff_ff:
                    return 3;
                default:
                    return 4;
            }
        }

        /// <summary>
        /// Given a number, will return the minimum bytes required for storing that uint
        /// (ex. if the absolute value of the number is under 256, it will need 1 byte, else if it's under 65 536 will need 2 bytes etc.).
        /// </summary>
        /// <param name="data">The number to analyze.</param>
        /// <returns>The minimum number of bytes required for storing the number or 0 if something went wrong.</returns>
        public static byte GetIntMinimumStorageData(int data)
        {
            data = Math.Abs(data);

            switch (data)
            {
                case <= 0xff:
                    return 1;
                case <= 0xff_ff:
                    return 2;
                case <= 0xff_ff_ff:
                    return 3;
                default:
                    return 4;
            }
        }

        public static byte[] TruncateIntValue(byte[] value, int byteCount)
        {
            byte[] newValue = new byte[byteCount];
            for (int i = 3; i >= 4 - byteCount; i--)
            {
                newValue[byteCount - (3 - i) - 1] = value[i];
            }

            return newValue;
        }
    }
}
