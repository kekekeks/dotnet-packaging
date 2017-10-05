﻿using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Packaging.Targets.IO
{
    /// <summary>
    /// The header of an entry in an <see cref="ArFile"/>.
    /// </summary>
    public struct ArHeader : IArchiveHeader
    {
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16)]
        private byte[] fileName;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 12)]
        private byte[] lastModified;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 6)]
        private byte[] ownerId;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 6)]
        private byte[] groupId;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 8)]
        private byte[] fileMode;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 10)]
        private byte[] fileSize;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 2)]
        private byte[] endChar;

        /// <summary>
        /// Gets or sets the name of the current file.
        /// </summary>
        public string FileName
        {
            get => GetString(fileName, 16).Trim();
            set => fileName = CreateString(value, 16);
        }

        /// <summary>
        /// Gets or sets the date at which the current file was last modified.
        /// </summary>
        public DateTimeOffset LastModified
        {
            get => DateTimeOffset.FromUnixTimeSeconds(int.Parse(GetString(lastModified, 12).Trim()));
            set => this.lastModified = CreateString(value.ToUnixTimeSeconds().ToString(), 12);
        }

        /// <summary>
        /// Gets of sets the user ID of the owner of the file.
        /// </summary>
        public uint OwnerId
        {
            get => ReadUInt(ownerId);
            set => ownerId = CreateString(value.ToString(), 6);
        }
        
        /// <summary>
        /// Gets or sets group ID of the owner of the file.
        /// </summary>
        public uint GroupId
        {
            get => ReadUInt(groupId);
            set => groupId = CreateString(value.ToString(), 6);
        }

        /// <inheritdoc/>
        public LinuxFileMode FileMode
        {
            get => (LinuxFileMode) Convert.ToUInt32(GetString(fileMode, 8).Trim(), 8);
            set => fileMode = CreateString(Convert.ToString((uint)value, 8), 8);
        }

        /// <inheritdoc/>
        public uint FileSize
        {
            get => ReadUInt(fileSize);
            set => fileSize = CreateString(value.ToString(), 10);
        }

        /// <summary>
        /// Gets or sets the value of the endchar field. This field is used as a checksum.
        /// </summary>
        public string EndChar
        {
            get => GetString(endChar, 2);
            set => endChar = CreateString(value, 2);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.FileName;
        }
        
        private string GetString(byte[] data, int maxLen)
        {
            int len;
            for (len = 0; len < maxLen; len++)
            {
                if (data[len] == 0)
                {
                    break;
                }
            }
            if (len == 0)
                return null;
            return Encoding.UTF8.GetString(data, 0, len);
        }
        
        
        uint ReadUInt(byte[] data) => Convert.ToUInt32(GetString(data, data.Length).Trim());

        byte[] CreateString(string s, int len)
        {
            var target = new byte[len];
            if (s == null)
                return target;
            var buffer = Encoding.UTF8.GetBytes(s);
            if (buffer.Length > len)
                throw new Exception($"String {s} exceeds the limit of {len}");
            for (var c = 0; c < len; c++)
                target[c] = (c < buffer.Length) ? buffer[c] : (byte) 0x20;
            return target;
        }
    }
}
