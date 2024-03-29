﻿using System.Security.Cryptography;
using System.Text;

namespace Rnd;

//TODO вынести в отдельную библиотеку
public static class Hash
{
    public static string GenerateStringHash(string stringForHashing)
    {
        return GenerateStringHash(stringForHashing, string.Empty, MD5.Create());
    }

    public static string GenerateStringHash(string stringForHashing, string salt, HashAlgorithm hashAlgorithm)
    {
        return Convert.ToBase64String(GenerateBinaryHash(stringForHashing, salt, hashAlgorithm));
    }

    public static byte[] GenerateBinaryHash(string stringForHashing)
    {
        return GenerateBinaryHash(stringForHashing, String.Empty, MD5.Create());
    }

    public static byte[] GenerateBinaryHash(string stringForHashing, string salt, HashAlgorithm hashAlgorithm)
    {
        var bytes = Encoding.Unicode.GetBytes(stringForHashing);
        var src = Convert.FromBase64String(salt);
        byte[] inArray;
        
        if (hashAlgorithm is KeyedHashAlgorithm keyedHashAlgorithm)
        {
            if (keyedHashAlgorithm.Key.Length == src.Length)
            {
                keyedHashAlgorithm.Key = src;
            }
            else if (keyedHashAlgorithm.Key.Length < src.Length)
            {
                var dst = new byte[keyedHashAlgorithm.Key.Length];
                Buffer.BlockCopy(src, 0, dst, 0, dst.Length);
                keyedHashAlgorithm.Key = dst;
            }
            else
            {
                int count;
                var buffer = new byte[keyedHashAlgorithm.Key.Length];
                for (var i = 0; i < buffer.Length; i += count)
                {
                    count = Math.Min(src.Length, buffer.Length - i);
                    Buffer.BlockCopy(src, 0, buffer, i, count);
                }

                keyedHashAlgorithm.Key = buffer;
            }

            inArray = keyedHashAlgorithm.ComputeHash(bytes);
        }
        else
        {
            var buffer = new byte[src.Length + bytes.Length];
            Buffer.BlockCopy(src, 0, buffer, 0, src.Length);
            Buffer.BlockCopy(bytes, 0, buffer, src.Length, bytes.Length);
            inArray = hashAlgorithm.ComputeHash(buffer);
        }

        return inArray;
    }
}