namespace GUT.DataFormats.XAP
{
    using GUT.Architecture;
    using System;

    public class SignatureDescriptor : DataStructure
    {
        [Order(3L)]
        public DataItem_ByteArray Data;
        public int ListIndex;
        [Order(2L)]
        public DataItem_UInt32 SignatureSize;
        [Order(0L)]
        public DataItem_UInt16 Type;
        [Order(1L)]
        public DataItem_UInt16 Version;

        public SignatureDescriptor(DataInByteArray Data)
        {
            this.Type = new DataItem_UInt16(Data);
            this.Version = new DataItem_UInt16(Data);
            this.SignatureSize = new DataItem_UInt32(Data);
            this.Data = new DataItem_ByteArray(Data, (ulong) this.SignatureSize.Value.Value);
        }
    }
}
