using MsgPack.Serialization;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using mp = MsgPack;
namespace SES.Serialization.MsgPack
{
    public class MsgPackSerializer : IAsyncEventSerializer
    {
        private readonly mp.Serialization.SerializationContext serializationContext;

        public MsgPackSerializer() : this(new SerializationContext()){}

        public MsgPackSerializer(mp.Serialization.SerializationContext serializationContext)
        {
            this.serializationContext = serializationContext ?? throw new ArgumentNullException(nameof(serializationContext));
        }
        public async Task<T> DeserializeAsync<T>(Stream data)
        {
            var ser = serializationContext.GetSerializer<T>();
            using(var decompressionStream = new GZipStream(data,CompressionMode.Decompress,false))
            {
                return await ser.UnpackAsync(decompressionStream).ConfigureAwait(false);
            }
        }
        public async Task SerializeAsync<T>(T @event, Stream stream)
        {
            var ser = serializationContext.GetSerializer<T>();
            using (var compressionStream = new GZipStream(stream, CompressionMode.Compress, true))
            {
                await ser.PackAsync(compressionStream, @event).ConfigureAwait(false);
                await compressionStream.FlushAsync();
            }
        }

        public Task SerializeAsync<T>(T @event, byte[] data)
        {
            using(var ms = new MemoryStream(data))
            {
                return SerializeAsync(@event, ms);
            }
        }

        public Task<T> DeserializeAsync<T>(byte[] bytes)
        {
            var ser = serializationContext.GetSerializer<T>();
            using(var ms = new MemoryStream(bytes))
            {
                return DeserializeAsync<T>(ms);
            }
        }



        public string ContentType => "application/msgpack";
    }
}
