﻿using System.Collections.Generic;
using System.IO;

namespace Server
{
	public sealed class QueuedMemoryWriter : BinaryFileWriter
	{
		private readonly MemoryStream _memStream;

		private readonly List<IndexInfo> _orderedIndexInfo = new List<IndexInfo>();

		public QueuedMemoryWriter()
			: base(new MemoryStream(1024 * 1024), true)
		{
			_memStream = UnderlyingStream as MemoryStream;
		}

		protected override int BufferSize => 512;

		public void QueueForIndex(ISerializable serializable, int size)
		{
			IndexInfo info;

			info.size = size;

			info.typeCode = serializable.TypeReference; //For guilds, this will automagically be zero.
			info.serial = serializable.SerialIdentity;

			_orderedIndexInfo.Add(info);
		}

		public void CommitTo(SequentialFileWriter dataFile, SequentialFileWriter indexFile)
		{
			Flush();

			var memLength = (int)_memStream.Position;

			if (memLength > 0)
			{
				var memBuffer = _memStream.GetBuffer();

				var actualPosition = dataFile.Position;

				dataFile.Write(memBuffer, 0, memLength);    //The buffer contains the data from many items.

				//Console.WriteLine("Writing {0} bytes starting at {1}, with {2} things", memLength, actualPosition, _orderedIndexInfo.Count);

				var indexBuffer = new byte[20];

				//int indexWritten = _orderedIndexInfo.Count * indexBuffer.Length;
				//int totalWritten = memLength + indexWritten

				for (var i = 0; i < _orderedIndexInfo.Count; i++)
				{
					var info = _orderedIndexInfo[i];

					var typeCode = info.typeCode;
					var serial = info.serial;
					var length = info.size;

					indexBuffer[0] = (byte)info.typeCode;
					indexBuffer[1] = (byte)(info.typeCode >> 8);
					indexBuffer[2] = (byte)(info.typeCode >> 16);
					indexBuffer[3] = (byte)(info.typeCode >> 24);

					indexBuffer[4] = (byte)info.serial;
					indexBuffer[5] = (byte)(info.serial >> 8);
					indexBuffer[6] = (byte)(info.serial >> 16);
					indexBuffer[7] = (byte)(info.serial >> 24);

					indexBuffer[8] = (byte)actualPosition;
					indexBuffer[9] = (byte)(actualPosition >> 8);
					indexBuffer[10] = (byte)(actualPosition >> 16);
					indexBuffer[11] = (byte)(actualPosition >> 24);
					indexBuffer[12] = (byte)(actualPosition >> 32);
					indexBuffer[13] = (byte)(actualPosition >> 40);
					indexBuffer[14] = (byte)(actualPosition >> 48);
					indexBuffer[15] = (byte)(actualPosition >> 56);

					indexBuffer[16] = (byte)info.size;
					indexBuffer[17] = (byte)(info.size >> 8);
					indexBuffer[18] = (byte)(info.size >> 16);
					indexBuffer[19] = (byte)(info.size >> 24);

					indexFile.Write(indexBuffer, 0, indexBuffer.Length);

					actualPosition += info.size;
				}
			}

			Close();    //We're done with this writer.
		}

		private struct IndexInfo
		{
			public int size;
			public int typeCode;
			public int serial;
		}
	}
}