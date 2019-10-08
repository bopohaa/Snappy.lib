using SnappyLib.Impl;
using System;
using System.Linq;
using System.Reflection;

namespace SnappyLib
{
    public static class Snappy
    {
        public static int Compress(byte[] src, int src_offset, int src_size, byte[] dst, int dst_offset, int dst_size)
        {
            unsafe
            {
                fixed (byte* srcPtr = src)
                fixed (byte* dstPtr = dst)
                {
                    var ptrSrc = new IntPtr((void*)srcPtr) + src_offset;
                    var ptrDst = new IntPtr((void*)dstPtr) + dst_offset;
                    var size = new UIntPtr((uint)dst_size);
                    var status = Libsnappy._snappy_compress(ptrSrc, new UIntPtr((uint)src_size), ptrDst, ref size);

                    if (status == SnappyStatusEnum.SNAPPY_OK)
                        return (int)size.ToUInt32();

                    throw new SnappyException(status);
                }
            }
        }

        public static int Uncompress(byte[] src, int src_offset, int src_size, byte[] dst, int dst_offset, int dst_size)
        {
            unsafe
            {
                fixed (byte* srcPtr = src)
                fixed (byte* dstPtr = dst)
                {
                    var ptrSrc = new IntPtr((void*)srcPtr) + src_offset;
                    var ptrDst = new IntPtr((void*)dstPtr) + dst_offset;
                    var size = new UIntPtr((uint)dst_size);
                    var status = Libsnappy._snappy_uncompress(ptrSrc, new UIntPtr((uint)src_size), ptrDst, ref size);

                    if (status == SnappyStatusEnum.SNAPPY_OK)
                        return (int)size.ToUInt32();

                    throw new SnappyException(status);
                }
            }
        }

        public static int MaxCompressedLength(int source_length)
        {
            return (int)Libsnappy._snappy_max_compressed_length(new UIntPtr((uint)source_length)).ToUInt32();
        }

        public static int UncompressedLength(byte[] src, int src_offset, int src_size)
        {
            unsafe
            {
                fixed (byte* srcPtr = src)
                {
                    var ptrSrc = new IntPtr((void*)srcPtr) + src_offset;
                    var status = Libsnappy._snappy_uncompressed_length(ptrSrc, new UIntPtr((uint)src_size), out var size);

                    if (status == SnappyStatusEnum.SNAPPY_OK)
                        return (int)size.ToUInt32();

                    throw new SnappyException(status);
                }
            }
        }


        public static SnappyStatusEnum ValidateCompressedBuffer(byte[] src, int src_offset, int src_size)
        {
            unsafe
            {
                fixed (byte* srcPtr = src)
                {
                    var ptrSrc = new IntPtr((void*)srcPtr) + src_offset;
                    return Libsnappy._snappy_validate_compressed_buffer(ptrSrc, new UIntPtr((uint)src_size));
                }
            }
        }

    }
}
