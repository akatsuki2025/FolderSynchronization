using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FolderSynchronization.Helpers
{
    internal static class FileComparison
    {
        public static bool CheckFileMd5(string file1, string file2)
        {
            using var md5First = MD5.Create();
            using var md5Second = MD5.Create();
            using var stream1 = File.OpenRead(file1);
            using var stream2 = File.OpenRead(file2);

            const int bufferSize = 8192; // 8KB chunks
            var buffer1 = new byte[bufferSize];
            var buffer2 = new byte[bufferSize];

            int bytesRead;
            while ((bytesRead = stream1.Read(buffer1, 0, bufferSize)) > 0)
            {
                int read2 = stream2.Read(buffer2, 0, bufferSize);
                if (bytesRead != read2)
                    return false;

                md5First.TransformBlock(buffer1, 0, bytesRead, buffer1, 0);
                md5Second.TransformBlock(buffer2, 0, read2, buffer2, 0);
            }

            if (stream2.Read(buffer2, 0, bufferSize) > 0)
                return false;

            md5First.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
            md5Second.TransformFinalBlock(Array.Empty<byte>(), 0, 0);

            var hash1 = md5First.Hash;
            var hash2 = md5Second.Hash;

            if (hash1 is null || hash2 is null)
            {
                throw new InvalidOperationException("MD5 hash computation failed");
            }

            if (hash1.Length != 16 || hash2.Length != 16)
            {
                throw new InvalidOperationException("Invalid MD5 hash length");
            }

            return hash1.SequenceEqual(hash2);
        }
    }
}
