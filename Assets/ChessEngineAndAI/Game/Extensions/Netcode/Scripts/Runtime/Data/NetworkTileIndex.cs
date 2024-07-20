using Unity.Netcode;

namespace ChessEngine.Networking
{
    // Netcode serialization extensions.
    public static class TileIndex_SerializationExtensions
    {
        public static void ReadValueSafe(this FastBufferReader pReader, out TileIndex pValue)
        {
            pValue = new TileIndex();
            // Read the values.
            pReader.ReadValueSafe(out pValue.x);
            pReader.ReadValueSafe(out pValue.y);
        }

        public static void WriteValueSafe(this FastBufferWriter pWriter, in TileIndex pValue)
        {
            // Write the values.
            pWriter.WriteValueSafe(pValue.x);
            pWriter.WriteValueSafe(pValue.y);
        }
    }
}
