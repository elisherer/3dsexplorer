using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Reflection;

namespace _3DSExplorer
{
    class MarshalTool
    {
        public static T ByteArrayToStructure<T>(byte[] bytes) where T : struct
        {
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T temp = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();
            return temp;
        }

        public static byte[] StructureToByteArray<T>(T structure) where T : struct
        {
            int size = Marshal.SizeOf(structure);
            byte[] byteArray = new byte[size];
            IntPtr pointer = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(structure, pointer, false);
            Marshal.Copy(pointer, byteArray, 0, size);
            Marshal.FreeHGlobal(pointer);
            return byteArray;
        }

        public static T ReadStruct<T>(Stream fs)
        {
            byte[] buffer = new byte[Marshal.SizeOf(typeof(T))];

            fs.Read(buffer, 0, Marshal.SizeOf(typeof(T)));
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            T temp = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();
            return temp;
        }

        public static T ReadStructBE<T>(Stream fs)
        {
            byte[] buffer = new byte[Marshal.SizeOf(typeof(T))];

            fs.Read(buffer, 0, Marshal.SizeOf(typeof(T)));
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            T temp = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();
            System.Type t = temp.GetType();
            FieldInfo[] fieldInfo = t.GetFields();
            foreach (FieldInfo fi in fieldInfo)
            {
                if (fi.FieldType == typeof(System.Int16))
                {
                    Int16 i16 = (Int16)fi.GetValue(temp);
                    byte[] b16 = BitConverter.GetBytes(i16);
                    byte[] b16r = b16.Reverse().ToArray();
                    fi.SetValueDirect(__makeref(temp), BitConverter.ToInt16(b16r, 0));
                }
                else if (fi.FieldType == typeof(System.Int32))
                {
                    Int32 i32 = (Int32)fi.GetValue(temp);
                    byte[] b32 = BitConverter.GetBytes(i32);
                    byte[] b32r = b32.Reverse().ToArray();
                    fi.SetValueDirect(__makeref(temp), BitConverter.ToInt32(b32r, 0));
                }
                else if (fi.FieldType == typeof(System.Int64))
                {
                    Int64 i64 = (Int64)fi.GetValue(temp);
                    byte[] b64 = BitConverter.GetBytes(i64);
                    byte[] b64r = b64.Reverse().ToArray();
                    fi.SetValueDirect(__makeref(temp), BitConverter.ToInt64(b64r, 0));
                }
                else if (fi.FieldType == typeof(System.UInt16))
                {
                    UInt16 i16 = (UInt16)fi.GetValue(temp);
                    byte[] b16 = BitConverter.GetBytes(i16);
                    byte[] b16r = b16.Reverse().ToArray();
                    fi.SetValueDirect(__makeref(temp), BitConverter.ToUInt16(b16r, 0));
                }
                else if (fi.FieldType == typeof(System.UInt32))
                {
                    UInt32 i32 = (UInt32)fi.GetValue(temp);
                    byte[] b32 = BitConverter.GetBytes(i32);
                    byte[] b32r = b32.Reverse().ToArray();
                    fi.SetValueDirect(__makeref(temp), BitConverter.ToUInt32(b32r, 0));
                }
                else if (fi.FieldType == typeof(System.UInt64))
                {
                    UInt64 i64 = (UInt64)fi.GetValue(temp);
                    byte[] b64 = BitConverter.GetBytes(i64);
                    byte[] b64r = b64.Reverse().ToArray();
                    fi.SetValueDirect(__makeref(temp), BitConverter.ToUInt64(b64r, 0));
                }
            }
            return temp;
        }

    }
}
