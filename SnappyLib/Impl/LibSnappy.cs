using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace SnappyLib.Impl
{
    internal static class Libsnappy
    {
        public delegate SnappyStatusEnum snappy_compress(IntPtr input, UIntPtr input_length, IntPtr compressed, ref UIntPtr compressed_length);
        public delegate SnappyStatusEnum snappy_uncompress(IntPtr compressed, UIntPtr compressed_length, IntPtr uncompressed, ref UIntPtr uncompressed_length);
        public delegate UIntPtr snappy_max_compressed_length(UIntPtr source_length);
        public delegate SnappyStatusEnum snappy_uncompressed_length(IntPtr compressed, UIntPtr compressed_length, out UIntPtr result);
        public delegate SnappyStatusEnum snappy_validate_compressed_buffer(IntPtr compressed, UIntPtr compressed_length);

        public readonly static snappy_compress _snappy_compress;
        public readonly static snappy_max_compressed_length _snappy_max_compressed_length;
        public readonly static snappy_uncompress _snappy_uncompress;
        public readonly static snappy_uncompressed_length _snappy_uncompressed_length;
        public readonly static snappy_validate_compressed_buffer _snappy_validate_compressed_buffer;

        static Libsnappy()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                TryWindowsLoadLibrary();

            var types = new[] { typeof(NativeMethods_Centos7), typeof(NativeMethods) };
            foreach (var type in types)
            {
                var methods = type.GetRuntimeMethods().ToArray();
                snappy_max_compressed_length test;
                try
                {
                    test = methods.CreateDelegate<snappy_max_compressed_length>();
                    var res = test(new UIntPtr(0));
                }
                catch
                {
                    continue;
                }

                _snappy_compress = methods.CreateDelegate<snappy_compress>();
                _snappy_uncompress = methods.CreateDelegate<snappy_uncompress>();
                _snappy_max_compressed_length = test;
                _snappy_uncompressed_length = methods.CreateDelegate<snappy_uncompressed_length>();
                _snappy_validate_compressed_buffer = methods.CreateDelegate<snappy_validate_compressed_buffer>();
            }

            if (_snappy_max_compressed_length == null)
                throw new InvalidOperationException(
                    $"Error while loading {NativeMethods.DllName}.dll or its dependencies from {Assembly.GetExecutingAssembly().GetName().EscapedCodeBase}. " +
                    $"Check the directory exists, if not check your deployment process. ");
        }

        private static bool TryWindowsLoadLibrary()
        {
            var baseUri = new Uri(Assembly.GetExecutingAssembly().GetName().EscapedCodeBase);
            var baseDirectory = Path.GetDirectoryName(baseUri.LocalPath);
            var is64 = RuntimeInformation.ProcessArchitecture == Architecture.X64;
            if (!is64 && RuntimeInformation.ProcessArchitecture != Architecture.X86)
                throw new NotImplementedException();
            var dllName = NativeMethods.DllName + ".dll";

            var path = Path.Combine(baseDirectory, is64 ? "x64" : "x86", dllName);
            if (!File.Exists(path))
            {
                path = Path.Combine(baseDirectory, is64 ? @"runtimes\win-x64\native" : @"runtimes\win-x86\native", dllName);
                if (!File.Exists(path))
                    path = Path.Combine(baseDirectory, dllName);
            }

            return WindowsNative.LoadLibraryEx(path, IntPtr.Zero, WindowsNative.LoadLibraryFlags.LOAD_WITH_ALTERED_SEARCH_PATH) != IntPtr.Zero;
        }

        static T CreateDelegate<T>(this MethodInfo[] methods) where T : Delegate
        {
            var type = typeof(T);
            var name = type.Name;

            return (T)methods.Single(m => m.Name == name).CreateDelegate(type);
        }
    }
}
