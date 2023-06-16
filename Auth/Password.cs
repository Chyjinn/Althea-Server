﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Server.Auth
{
    public class Password
    {
        private const int SaltSize = 16; //128 / 8, length in bytes
        private const int KeySize = 32; //256 / 8, length in bytes
        private const int Iterations = 10000;
        private static readonly HashAlgorithmName _hashAlgorithmName = HashAlgorithmName.SHA512;
        private const char SaltDelimeter = ';';
        public string Hash(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);

            var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, _hashAlgorithmName, KeySize);
            return string.Join(SaltDelimeter, Convert.ToBase64String(salt), Convert.ToBase64String(hash));
        }
        public bool Validate(string passwordHash, string password)
        {
            var pwdElements = passwordHash.Split(SaltDelimeter);
            var salt = Convert.FromBase64String(pwdElements[0]);
            var hash = Convert.FromBase64String(pwdElements[1]);
            var hashInput = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, _hashAlgorithmName, KeySize);
            return CryptographicOperations.FixedTimeEquals(hash, hashInput);
        }
    }
}
