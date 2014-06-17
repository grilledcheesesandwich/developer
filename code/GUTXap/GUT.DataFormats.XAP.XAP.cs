namespace GUT.DataFormats.XAP
{
    using GUT.Architecture;
    using System;
    using System.Collections.Generic;

    public class XAP : DataFormat
    {
        [Order(0L)]
        public DataItem_ByteArray Contents;
        [Order(1L)]
        public List<SignatureDescriptor> SignatureDescriptors;
        [Order(2L)]
        public GUT.DataFormats.XAP.SignatureDirectoryRecord SignatureDirectoryRecord;

        protected override void ParseData(DataInByteArray Data)
        {
            int num4;
            Data.Seek(Data.Length - ((ulong) 10L));
            if (Data.PeekUInt32() != 0x53706158)
            {
                base.AddParsingNote(ParsingNoteType.Warning, "Magic number not found at expected location, assuming XAP is unsigned");
                this.Contents = new DataItem_ByteArray(Data, Data.Length);
                return;
            }
            this.SignatureDirectoryRecord = new GUT.DataFormats.XAP.SignatureDirectoryRecord(Data);
            ulong num3 = Data.Length - ((ulong) 10L);
            uint? nullable = this.SignatureDirectoryRecord.Offset.Value;
            ulong? nullable2 = nullable.HasValue ? new ulong?(num3 - ((ulong) nullable.GetValueOrDefault())) : null;
            ulong newPosition = nullable2.Value;
            Data.Seek(newPosition);
            this.SignatureDescriptors = new List<SignatureDescriptor>();
            int num2 = 0;
        Label_00EB:
            num4 = num2;
            if (num4 < this.SignatureDirectoryRecord.DescriptorCount.Value)
            {
                SignatureDescriptor item = new SignatureDescriptor(Data);
                this.SignatureDescriptors.Add(item);
                num2++;
                goto Label_00EB;
            }
            Data.Seek(0L);
            this.Contents = new DataItem_ByteArray(Data, newPosition);
        }
    }
}
