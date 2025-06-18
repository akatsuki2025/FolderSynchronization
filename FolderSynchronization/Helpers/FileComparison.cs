using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.IO.Hashing;

namespace FolderSynchronization.Helpers
{
    internal static class FileComparison
    {
        private static readonly Dictionary<string, (string Hash, DateTime LastWriteTimeUtc, long FileSize, uint Crc32)> _hashCache = new();
        private const int BUFFER_SIZE = 8192; // 8KB buffer

        public static bool CheckFileMd5(string file1, string file2)
        {
            var sourceFileInfo = new FileInfo(file1);
            string sourceHash = GetSourceFileHash(file1, sourceFileInfo);
            string destinationHash = CalculateMd5(file2);

            return sourceHash == destinationHash;
        }

        private static string GetSourceFileHash(string filePath, FileInfo fileInfo)
        {
            if (!_hashCache.TryGetValue(filePath, out var cachedData))
            {
                string hash = CalculateMd5(filePath);
                _hashCache[filePath] = (hash, fileInfo.LastAccessTimeUtc, fileInfo.Length, CalculateCrc32(filePath));
                return hash;
            }

            bool sameTimestamp = cachedData.LastWriteTimeUtc.Equals(fileInfo.LastWriteTimeUtc);
            bool sameSize = cachedData.FileSize == fileInfo.Length;

            if (!sameTimestamp || !sameSize)
            {
                string hash = CalculateMd5(filePath);
                uint crc = CalculateCrc32(filePath);
                _hashCache[filePath] = (hash, fileInfo.LastWriteTimeUtc, fileInfo.Length, crc);
                return hash;
            }

            uint currentCrc = CalculateCrc32(filePath);
            if (currentCrc == cachedData.Crc32)
            {
                return cachedData.Hash;
            }

            string newHash = CalculateMd5(filePath);
            _hashCache[filePath] = (newHash, fileInfo.LastWriteTimeUtc, fileInfo.Length, currentCrc);
            return newHash;
        }

        private static string CalculateMd5(string filePath)
        {
            using var md5 = MD5.Create();
            using var stream = File.OpenRead(filePath);
            var buffer = new byte[BUFFER_SIZE];
            int bytesRead;

            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                md5.TransformBlock(buffer, 0, bytesRead, buffer, 0);
            }

            md5.TransformFinalBlock(Array.Empty<byte>(), 0, 0);

            byte[]? hash = md5.Hash;

            if (hash is null)
            {
                throw new InvalidOperationException("MD5 hash computation failed");
            }

            if (hash.Length != 16) // MD5 hashes should always be 16 bytes
            {
                throw new InvalidOperationException($"Invalid MD5 hash length: {hash.Length} bytes");
            }

            return Convert.ToBase64String(hash);
        }

        private static uint CalculateCrc32(string filename)
        {
            using var stream = File.OpenRead(filename);
            var crc32 = new Crc32();
            var buffer = new byte[BUFFER_SIZE];
            int bytesRead;

            while((bytesRead = stream.Read(buffer,0, buffer.Length)) > 0)
            {
                crc32.Append(buffer.AsSpan(0, bytesRead));
            }

            return BitConverter.ToUInt32(crc32.GetCurrentHash());
        }
    }
}
