using System;
using System.IO;

namespace ChMultiPatcher.BsDiff
{
	/// <summary>
	/// Provides helper methods for working with <see cref="Stream"/>.
	/// </summary>
	public static class StreamUtility
	{
		/// <summary>
		/// Reads exactly <paramref name="count"/> bytes from <paramref name="stream"/>.
		/// </summary>
		/// <param name="stream">The stream to read from.</param>
		/// <param name="count">The count of bytes to read.</param>
		/// <returns>A new byte array containing the data read from the stream.</returns>
		public static byte[] ReadExactly(this Stream stream, int count)
		{
			if (count < 0)
				throw new ArgumentOutOfRangeException("count");
			byte[] buffer = new byte[count];
			ReadExactly(stream, buffer, 0, count);
			return buffer;
		}

		/// <summary>
		/// Reads exactly <paramref name="count"/> bytes from <paramref name="stream"/> into
		/// <paramref name="buffer"/>, starting at the byte given by <paramref name="offset"/>.
		/// </summary>
		/// <param name="stream">The stream to read from.</param>
		/// <param name="buffer">The buffer to read data into.</param>
		/// <param name="offset">The offset within the buffer at which data is first written.</param>
		/// <param name="count">The count of bytes to read.</param>
		public static void ReadExactly(this Stream stream, byte[] buffer, int offset, int count)
		{
			// check arguments
			if (stream == null)
				throw new ArgumentNullException("stream");
			if (buffer == null)
				throw new ArgumentNullException("buffer");
			if (offset < 0 || offset > buffer.Length)
				throw new ArgumentOutOfRangeException("offset");
			if (count < 0 || buffer.Length - offset < count)
				throw new ArgumentOutOfRangeException("count");

			while (count > 0)
			{
				// read data
				int bytesRead = stream.Read(buffer, offset, count);

				// check for failure to read
				if (bytesRead == 0)
					throw new EndOfStreamException();

				// move to next block
				offset += bytesRead;
				count -= bytesRead;
			}
		}
	}
}