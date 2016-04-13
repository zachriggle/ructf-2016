using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Node.Connections;
using Node.Connections.Tcp;
using Node.Encryption;
using Node.Serialization;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    internal class Encryption_Tests
    {
        [Test]
        public void Should_decode_encoded_text()
        {
            var privateKey = BluemeshEncryptor.GeneratePrivateKey(SerializeAddress(new TcpUtility().ParseAddress("127.0.0.1:16800")));
            var publicKey = BluemeshEncryptor.GeneratePublicKey(privateKey);

            Console.WriteLine("e = {0}, d = {1}", publicKey, privateKey);

            var buffer = new byte[32];

            var message = "Hello, World!";
            Encoding.ASCII.GetBytes(message, 0, message.Length, buffer, 0);

            Console.WriteLine(BitConverter.ToString(buffer));
            Console.WriteLine(Encoding.ASCII.GetString(buffer));

            BluemeshEncryptor.EncryptBytes(buffer, 0, buffer.Length, publicKey);

            Console.WriteLine(BitConverter.ToString(buffer));
            Console.WriteLine(Encoding.ASCII.GetString(buffer));

            BluemeshEncryptor.EncryptBytes(buffer, 0, buffer.Length, privateKey);

            Console.WriteLine(BitConverter.ToString(buffer));
            Console.WriteLine(Encoding.ASCII.GetString(buffer));

            Encoding.ASCII.GetString(buffer).Should().StartWith(message);
        }

        private static byte[] SerializeAddress(IAddress address)
        {
            var bytes = new byte[12];
            using (var stream = new MemoryStream(bytes))
                new StreamSerializer(stream).Write(address);
            return bytes;
        }


        // a = private key
        // b = ulong_max + 1
       
    }
}