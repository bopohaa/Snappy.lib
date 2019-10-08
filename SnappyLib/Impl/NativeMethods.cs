using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace SnappyLib.Impl
{
    internal class NativeMethods
    {
        public const string DllName = "libsnappy";

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern SnappyStatusEnum snappy_compress(IntPtr input, UIntPtr input_length, IntPtr compressed, ref UIntPtr compressed_length);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern SnappyStatusEnum snappy_uncompress(IntPtr compressed, UIntPtr compressed_length, IntPtr uncompressed, ref UIntPtr uncompressed_length);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern UIntPtr snappy_max_compressed_length(UIntPtr source_length);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern SnappyStatusEnum snappy_uncompressed_length(IntPtr compressed, UIntPtr compressed_length, out UIntPtr result);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern SnappyStatusEnum snappy_validate_compressed_buffer(IntPtr compressed, UIntPtr compressed_length);
    }
}
