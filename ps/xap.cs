//
// Copyright (c) Microsoft Corporation.  All rights reserved.
//
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Microsoft.MobileDevices.AreaLibrary.Security.LoaderVerifier
{
    static class Utilities
    {
        internal static Random rand = new Random(
            #if UNDER_CE
                Microsoft.WindowsCE.Random.RandomGen.Seed
            #endif
        );
        public static T Random<T>(this IList<T> list)
        {
            return list[rand.Next(list.Count)];
        }
    }

    public class XAP
    {
        /// <summary>
        /// "XapS" hex encoded
        /// </summary>
        private const uint MAGIC_NUMBER = 0x53706158;

        private const uint SIZE_OF_XAP_SDR = 2 * sizeof(uint) + sizeof(ushort);
        public const uint XAP_SIGNATURE_TYPE_AUTHENTICODE = 0x0001;

        public byte[] Contents { get; set; }
        public List<SignatureDescriptor> Descriptors { get; set; }
        public bool IsSigned { get; set; }

        public class SignatureDescriptor
        {
            public ushort Type { get; set; }
            public ushort Version { get; set; }
            public uint Size { get; set; }
            public byte[] Data { get; set; }

            public SignatureDescriptor() { }

            public SignatureDescriptor(SignatureDescriptor sd)
            {
                Type = sd.Type;
                Version = sd.Version;
                Size = sd.Size;
                Data = new byte[sd.Data.Length];
                Array.Copy(sd.Data, Data, sd.Data.Length);
            }
        }

        /// <summary>
        /// Parse a XAP file and build an in-memory representation of the various sections of the XAP
        /// </summary>
        public XAP(string path)
        {
            using (BinaryReader br = new BinaryReader(File.OpenRead(path)))
            {
                br.BaseStream.Seek(-SIZE_OF_XAP_SDR, SeekOrigin.End);
                uint magic = br.ReadUInt32();
                if (magic != MAGIC_NUMBER)
                {
                    IsSigned = false;
                    Contents = new byte[br.BaseStream.Length];
                    br.Read(Contents, 0, (int)br.BaseStream.Length);
                }
                else
                {
                    IsSigned = true;
                    uint sdCount = br.ReadUInt16();
                    uint offset = br.ReadUInt32();

                    Descriptors = new List<SignatureDescriptor>((int)sdCount);

                    long sdStart = br.BaseStream.Seek(-SIZE_OF_XAP_SDR - offset, SeekOrigin.End);

                    for (int i = 0; i < sdCount; i++)
                    {
                        SignatureDescriptor sd = new SignatureDescriptor();
                        sd.Type = br.ReadUInt16();
                        sd.Version = br.ReadUInt16();
                        sd.Size = br.ReadUInt32();
                        sd.Data = br.ReadBytes((int)sd.Size);
                        Descriptors.Add(sd);
                    }

                    magic = br.ReadUInt32();
                    if (magic != MAGIC_NUMBER)
                    {
                        throw new System.IO.InvalidDataException();
                    }

                    br.BaseStream.Seek(0, SeekOrigin.Begin);
                    Contents = br.ReadBytes((int)sdStart);
                }
            }
        }

        /// <summary>
        /// Serialize the in-memory XAP representation to a temporary file and return the path
        /// </summary>
        public string CreateBinary()
        {
            return CreateBinary(-1, false);
        }

        /// <summary>
        /// Serialize the in-memory XAP representation to a temporary file and return the path.  Insert a junk buffer at the specified index in the contents
        /// </summary>
        public string CreateMungedBinary(int padIndex, bool adjustOffset)
        {
            return CreateBinary(padIndex, adjustOffset);
        }

        /*
         * The possible padIndexes are:
         * 
         *      Contents: 0 :Type: 1 :Version: 2 :Size: 3 :Signature data: 4 :Magic #: 5 :SD count: 6 :Offset: 7
         * 
         * The function will insert a random binary blob at the specified index
         */
        private string CreateBinary(int padIndex, bool adjustOffset)
        {
            // Initialize padding buffers to empty, except for specified index
            // The idea here is to munge the XAP file by inserting a random sized, random content blob between any two sections of the XAP
            // Depending on where the blob is inserted, authentication or authorization should fail
            List<byte[]> padBuffers = new List<byte[]>();
            for (int i = 0; i < 8; i++)
            {
                if (i != padIndex)
                {
                    padBuffers.Add(new byte[0]);
                }
                else
                {
                    byte[] bytes = new byte[Utilities.rand.Next(1, 20)];
                    Utilities.rand.NextBytes(bytes);
                    padBuffers.Add(bytes);
                }
            }

            string path = Path.GetTempFileName();
            using (BinaryWriter bw = new BinaryWriter(File.OpenWrite(path)))
            {
                bw.Write(Contents);
                bw.Write(padBuffers[0]);
                foreach (var desc in Descriptors)
                {
                    bw.Write(desc.Type);
                    bw.Write(padBuffers[1]);
                    bw.Write(desc.Version);
                    bw.Write(padBuffers[2]);
                    bw.Write(desc.Size);
                    bw.Write(padBuffers[3]);
                    bw.Write(desc.Data);
                    bw.Write(padBuffers[4]);
                }
                bw.Write(MAGIC_NUMBER);
                bw.Write(padBuffers[5]);
                bw.Write((ushort)Descriptors.Count);
                bw.Write(padBuffers[6]);

                // For each SD, we need to increment the offset by sizeof(WORD) + sizeof(WORD) + sizeof(DWORD) + length of data blob
                uint offset = (uint)(Descriptors.Sum(desc => desc.Data.Length) + Descriptors.Count * (2 + 2 + 4));
                if (adjustOffset)
                {
                    // If we are adjusting the offset to compensate for trash buffers, we need to compensate
                    // only for buffers inserted in and after the Signature Descriptors
                    offset += (uint)(padBuffers.Skip(1).Take(4).Sum(buffer => buffer.Length) * Descriptors.Count);
                }
                bw.Write(offset);
                bw.Write(padBuffers[7]);
            }
            FileInfo fi = new FileInfo(path);
            string dest = Path.Combine(fi.DirectoryName, fi.Name);
            dest += ".xap";
            fi.MoveTo(dest);
            return dest;
        }
    }
}
