﻿/*
 * Copyright (C) 2011 APS
 *	http://AllPrivateServer.com
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */
 
using System;
using System.Collections.Generic;
using System.Text;


namespace FrameWork
{
    public static class Marshal
    {
        public static string ConvertToString(byte[] cstyle)
		{
			if (cstyle == null)
				return null;

			for (int i = 0; i < cstyle.Length; i++)
			{
                if (cstyle[i] == 0)
                    return Encoding.GetEncoding("iso-8859-1").GetString(cstyle, 0, i);
			}
            return Encoding.GetEncoding("iso-8859-1").GetString(cstyle);
		}

		public static int ConvertToInt32(byte[] val)
		{
			return ConvertToInt32(val, 0);
		}

		public static int ConvertToInt32(byte[] val, int startIndex)
		{
			return ConvertToInt32(val[startIndex], val[startIndex + 1], val[startIndex + 2], val[startIndex + 3]);
		}

		public static int ConvertToInt32(byte v1, byte v2, byte v3, byte v4)
		{
			return ((v1 << 24) | (v2 << 16) | (v3 << 8) | v4);
		}

		public static uint ConvertToUInt32(byte[] val)
		{
			return ConvertToUInt32(val, 0);
		}

		public static uint ConvertToUInt32(byte[] val, int startIndex)
		{
			return ConvertToUInt32(val[startIndex], val[startIndex + 1], val[startIndex + 2], val[startIndex + 3]);
		}

		public static uint ConvertToUInt32(byte v1, byte v2, byte v3, byte v4)
		{
			return (uint) ((v1 << 24) | (v2 << 16) | (v3 << 8) | v4);
		}

        public static float ConvertToFloat(byte v1, byte v2, byte v3, byte v4)
        {
            return (float)((v1 << 24) | (v2 << 16) | (v3 << 8) | v4);
        }

		public static short ConvertToInt16(byte[] val)
		{
			return ConvertToInt16(val, 0);
		}

		public static short ConvertToInt16(byte[] val, int startIndex)
		{
			return ConvertToInt16(val[startIndex], val[startIndex + 1]);
		}

		public static short ConvertToInt16(byte v1, byte v2)
		{
			return (short) ((v1 << 8) | v2);
		}

		public static ushort ConvertToUInt16(byte[] val)
		{
			return ConvertToUInt16(val, 0);
		}

		public static ushort ConvertToUInt16(byte[] val, int startIndex)
		{
			return ConvertToUInt16(val[startIndex], val[startIndex + 1]);
		}

		public static ushort ConvertToUInt16(byte v1, byte v2)
		{
			return (ushort) (v2 | (v1 << 8));
		}
	}
}
