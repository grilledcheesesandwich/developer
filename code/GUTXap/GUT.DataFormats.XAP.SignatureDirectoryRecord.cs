namespace GUT.DataFormats.XAP
{
    using GUT.Architecture;
    using System;

    public class SignatureDirectoryRecord : DataStructure
    {
        [Order(1L)]
        public DataItem_UInt16 DescriptorCount;
        [Order(0L)]
        public DataItem_UInt32 Magic;
        [Order(2L)]
        public DataItem_UInt32 Offset;

        public SignatureDirectoryRecord(DataInByteArray Data) : base(Data)
        {
        }
    }
}
