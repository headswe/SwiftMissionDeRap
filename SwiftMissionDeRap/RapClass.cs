using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SwiftMissionDeRap
{
    public class RapClass
    {
        public string Name { get; set; }
        public long pos { get; set; }
        public Dictionary<string, dynamic> Attributes { get; set; } = new Dictionary<string, dynamic>();
        public Dictionary<string, RapClass> Children { get; set; } = new Dictionary<string, RapClass>();
        internal FileStream stream;
        private RapClass(string name, long p, FileStream s) {
            Name = name;
            pos = p;
            stream = s;
        }
        public RapClass()
        {

        }
        public void Read()
        {
            stream.Position = pos;
            var name = ReadString();
            if (!String.IsNullOrEmpty(name))
            {
                Name = name;
            }
            var items = ReadCompressedInt();
            ReadItems(items);
        }

        private void ReadItems(long items)
        {
            for (int i = 0; i < items; i++)
            {
                var packetType = (PacketTypes)stream.ReadByte();
                switch (packetType)
                {
                    case PacketTypes.Token:
                        {
                            VariableTypes varType = (VariableTypes)stream.ReadByte();
                            var name = ReadString();
                            dynamic value = null;
                            switch(varType)
                            {
                                case VariableTypes.Integer:
                                    value = ReadInt32();
                                    break;
                                case VariableTypes.Float:
                                    value = ReadFloat();
                                    break;
                                case VariableTypes.String:
                                    value = ReadString();
                                    break;
                            }
                            Attributes.Add(name, value);
                            break;
                        }
                    case PacketTypes.ClassName:
                        {
                            var name = ReadString();
                            //  var parent = ReadString();
                            var offset = ReadInt32();
                            Children.Add(name, new RapClass(name, offset, stream));
                            break;
                        }

                    case PacketTypes.Array:
                        {
                            var list = new List<dynamic>();
                            var name = ReadString();
                            var elementCount = ReadCompressedInt();
                            for (int elementIndex = 0; elementIndex < elementCount; elementIndex++)
                            {
                                var arrDataType = (ArrayTypes)stream.ReadByte();
                                switch (arrDataType)
                                {
                                    case ArrayTypes.String:
                                        list.Add(ReadString());
                                        break;
                                    case ArrayTypes.Float:
                                        list.Add(ReadFloat());
                                        break;
                                    case ArrayTypes.Int:
                                        list.Add(ReadInt32());
                                        break;
                                    default:
                                        throw new Exception();
                                }
                            }
                            this.Attributes.Add(name, list);
                            break;
                        }
                    default:
                        throw new Exception();
                }
            }
            foreach(var child in Children) {
                child.Value.Read();
            }
        }

        internal uint ReadInt16()
        {
            byte[] buffer = new byte[2];
            stream.Read(buffer, 0, 2);
            return BitConverter.ToUInt16(buffer, 0);
        }
        internal double ReadInt163()
        {
            byte[] buffer = new byte[2];
            stream.Read(buffer, 0, 3);
            return BitConverter.ToSingle(buffer, 0);
        }
        internal uint ReadInt32()
        {
            byte[] buffer = new byte[4];
            stream.Read(buffer, 0, 4);
            return BitConverter.ToUInt32(buffer, 0);
        }
        internal ulong ReadLong()
        {
            byte[] buffer = new byte[8];
            stream.Read(buffer, 0, 8);
            return BitConverter.ToUInt64(buffer, 0);
        }
        internal float ReadFloat()
        {
            byte[] buffer = new byte[4];
            stream.Read(buffer, 0, 4);
            return BitConverter.ToSingle(buffer, 0);
        }
        internal string ReadString()
        {
            List<byte> buffer = new List<byte>();
            int ch = stream.ReadByte();
            while (ch != 0x0)
            {
                buffer.Add((byte)ch);
                ch = stream.ReadByte();
            }
            return Encoding.ASCII.GetString(buffer.ToArray());
        }

        internal uint ReadCompressedInt()
        {
            uint val = (uint)stream.ReadByte();
            var hasExtra = IsBitSet((byte)val, 7);
            while (hasExtra)
            {
                var extra = (byte)stream.ReadByte();
                val += ((uint)extra - 1) * 0x80;
                if(!IsBitSet(extra, 7))
                {
                    break;
                }
            }
            return val;
        }
        bool IsBitSet(byte b, int pos)
        {
            return (b & (1 << pos)) != 0;
        }
    }
}
