namespace Axis.Libra.Utils
{
    /// <summary>
    /// </summary>
    public interface IBinarySerializable
    {
        /// <summary>
        /// Serialize the properties of the underlying type into a byte array.
        /// </summary>
        /// <returns></returns>
        byte[] Serialize();
    }
}
